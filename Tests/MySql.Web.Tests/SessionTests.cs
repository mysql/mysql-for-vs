// Copyright © 2004,2010, Oracle and/or its affiliates.  All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

//  This code was contributed by Sean Wright (srwright@alcor.concordia.ca) on 2007-01-12
//  The copyright was assigned and transferred under the terms of
//  the MySQL Contributor License Agreement (CLA)

using NUnit.Framework;
using System.Web.Security;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System;
using System.IO;
using System.Net;
using System.Configuration.Provider;
using System.Threading;
using MySql.Web.Security;
using System.Web.Hosting;
using System.Web;
using Microsoft.Win32;

namespace MySql.Web.Tests
{
  [TestFixture]
  public class SessionTests : BaseWebTest
  {
    public class ThreadRequestData
    {
      public string pageName;
      public ManualResetEvent signal;
      public bool FirstDateToUpdate;
    }

    delegate WebResponse GetResponse();
    delegate void ThreadRequest( ThreadRequestData data );

    [Test]
    [Timeout(1000000)]
    public void SessionLocking()
    {
      // Copy updated configuration file for web server process 
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      ConnectionStringSettings css = config.ConnectionStrings.ConnectionStrings["LocalMySqlServer"];
      string curDir = Directory.GetCurrentDirectory();
      string sessionLockingdir = Path.GetFullPath(".");
#if CLR4
      string pathSessionLocking = Path.GetFullPath(@"..\..\SessionLocking");
#else
      string pathSessionLocking = Path.GetFullPath( @"..\..\..\..\..\Tests\MySql.Web.Tests\SessionLocking" );
#endif
      Directory.SetCurrentDirectory(pathSessionLocking);
      string webconfigPath = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), @"web.config");
      string webconfigPathSrc = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), @"web_config_src.txt");

      string text = File.ReadAllText(webconfigPathSrc);
      text = text.Replace("connection_string_here", css.ConnectionString);
#if CLR2
      text = text.Replace("<compilation debug=\"true\" targetFramework=\"4.0\" />", "");
#endif
      File.WriteAllText(webconfigPath, text);

      int port = 12224;
#if CLR4
      string webserverPath = @"C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\WebDev.WebServer40.exe";
#else
      string webserverPath = @"C:\Program Files (x86)\Common Files\microsoft shared\DevServer\9.0\WebDev.WebServer.exe";
#endif
      string webserverArgs = string.Format(" /port:{0} /path:{1}", port,
        Path.GetFullPath(@"."));
      
      DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(curDir));      
      Directory.CreateDirectory(Path.GetFullPath(@".\bin"));
      foreach (FileInfo fi in di.GetFiles("*.dll"))
      {
        File.Copy(fi.FullName, Path.Combine(Path.GetFullPath(@".\bin\"), fi.Name), true);
      }

      Process webserver = Process.Start(webserverPath, webserverArgs);
      System.Threading.Thread.Sleep(2000);

      // This dummy request is just to get the ASP.NET sessionid to reuse.
      HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://localhost:12224/InitSessionLocking.aspx");
      HttpWebResponse res = (HttpWebResponse)req.GetResponse();
      WebHeaderCollection headers = new WebHeaderCollection();

      string url = res.ResponseUri.ToString().Replace("InitSessionLocking.aspx", "");
      Debug.Write(url);

      try
      {
        DateTime? firstDt = null;
        DateTime? secondDt = null;

        ManualResetEvent[] re = new ManualResetEvent[2];
        re[0] = new ManualResetEvent(false);
        re[1] = new ManualResetEvent(false);
        ParameterizedThreadStart ts =
          ( object data1 ) =>
          {
            ThreadRequestData data = (ThreadRequestData)data1;
            Debug.WriteLine( string.Format( "Requesting {0}", data.pageName ) );
            HttpWebRequest req1 = 
              (HttpWebRequest)WebRequest.Create(string.Format(@"{0}{1}", url, data.pageName ));
            req1.Timeout = 2000000;
            WebResponse res1 = req1.GetResponse();
            Debug.WriteLine( string.Format( "Response from {0}", data.pageName) );
            Stream s = res1.GetResponseStream();
            while (s.ReadByte() != -1)
              ;
            res1.Close();
            if( data.FirstDateToUpdate )
            {
              firstDt = DateTime.Now;
            } else {
              secondDt = DateTime.Now;
            }            
            data.signal.Set();
          };
        
        Thread t = new Thread( ts );
        Thread t2 = new Thread(ts);
        t.Start( new ThreadRequestData() {
          pageName = "write.aspx", 
          FirstDateToUpdate = true,
          signal = re[ 0 ] 
        } );
        t2.Start(new ThreadRequestData() {
          pageName = "read.aspx",
          FirstDateToUpdate = false,
          signal = re[1]
        });
        WaitHandle.WaitAll(re);
        re[0].Reset();
        Thread t3 = new Thread(ts);
        t3.Start(new ThreadRequestData()
        {
          pageName = "write2.aspx",
          FirstDateToUpdate = false,
          signal = re[0]
        });
        WaitHandle.WaitAll(re);
        double totalMillisecs = Math.Abs((secondDt.Value - firstDt.Value).TotalMilliseconds);
        // OK if wait is less than session timeout
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(totalMillisecs);
        Assert.IsTrue(totalMillisecs < 30000);
      }
      finally
      {
        webserver.Kill();
      }
    }

    public volatile static ManualResetEvent mtxReader = null;
    public volatile static ManualResetEvent mtxWriter = null;

    public static void WaitSyncCreation( bool writer )
    {
      if ( writer )
      {
        while (true)
        {
          if (mtxWriter == null)
            Thread.Sleep(100);
          else
            break;
        }
        mtxWriter.WaitOne();
      }
      else
      {
        while (true)
        {
          if (mtxReader == null)
            Thread.Sleep(100);
          else
            break;
        }
        mtxReader.WaitOne();
      }
    }
  }
}
