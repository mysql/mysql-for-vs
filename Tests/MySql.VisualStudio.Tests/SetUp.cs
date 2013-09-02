// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MySql.VisualStudio.Tests
{
  public class SetUp : IDisposable
  {
    internal protected int maxPacketSize;
    internal protected MySqlConnection conn;
    internal protected string host;
    internal protected string user;
    internal protected string password;
    internal protected int port;
    internal protected string pipeName;
    internal protected string memoryName;
    internal protected string rootUser;
    internal protected string rootPassword;
    internal protected string database;
    internal protected string database1;
    internal protected string database2;

    public SetUp()
    {
      LoadConfiguration();
      CreateData();
    }

    private void CreateData()
    {         
      Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.VisualStudio.Tests.Properties.Setup.sql");
      StreamReader sr = new StreamReader(stream);
      StringBuilder sql = new StringBuilder(sr.ReadToEnd());
      sr.Close();
      sql.Replace("{0}",database);
      sql.Replace("{1}", database1);
      sql.Replace("{2}", database2);
      ExecuteSQLAsRoot(sql.ToString());
    }


    private void LoadConfiguration()
    {        
      user = "test";
      password = "test";
      string portString = null;

      rootUser = ConfigurationManager.AppSettings["rootuser"];
      rootPassword = ConfigurationManager.AppSettings["rootpassword"];
      host = ConfigurationManager.AppSettings["host"];
      portString = ConfigurationManager.AppSettings["port"];
      pipeName = ConfigurationManager.AppSettings["pipename"];
      memoryName = ConfigurationManager.AppSettings["memory_name"];
      database = ConfigurationManager.AppSettings["database"];
      if (string.IsNullOrEmpty(rootUser))
          rootUser = "root";
      if (string.IsNullOrEmpty(rootPassword))
          rootPassword = string.Empty;
      if (string.IsNullOrEmpty(host))
          host = "localhost";
      if (string.IsNullOrEmpty(portString))
          port = 3305;
      else
          port = int.Parse(portString);
      if (string.IsNullOrEmpty(pipeName))
          pipeName = "MYSQL";
      if (string.IsNullOrEmpty(memoryName))
          memoryName = "MYSQL";
      if (string.IsNullOrEmpty(database))
        database = "DumpTest";
      if (string.IsNullOrEmpty(database1))
          database1 = "SecondTest";
      if (string.IsNullOrEmpty(database2))
          database2 = "ThirdTest";

    }


    internal protected string GetConnectionString(string userId, string pw, bool persistSecurityInfo, bool includedb)
    {
      string connStr = String.Format("server={0};user id={1};pooling=false;" +
           "persist security info={2};connection reset=true;allow user variables=true;port={3};",
           host, userId, persistSecurityInfo.ToString().ToLower(), port);
      if (pw != null)
        connStr += String.Format(";password={0};", pw);
      if (includedb)
        connStr += String.Format("database={0};", database); 
      return connStr;
    }


    public virtual void Dispose()
    {
      var sql = string.Format("DROP DATABASE IF EXISTS {0}",database);
      ExecuteSQLAsRoot(sql);
    }

    internal protected void ExecuteSQLAsRoot(string sql)
    {
      MySqlScript s = new MySqlScript(new MySqlConnection(GetConnectionString("root","",false,false)), sql);
      s.Execute();
    }
  }
}
