// Copyright (C) 2004-2006 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for StreamCreator.
	/// </summary>
	internal class StreamCreator
	{
		string				hostList;
		uint				port;
		string				pipeName;
		uint				timeOut;

		public StreamCreator( string hosts, uint port, string pipeName)
		{
			hostList = hosts;
			if (hostList == null || hostList.Length == 0)
				hostList = "localhost";
			this.port = port;
			this.pipeName = pipeName;
        }

		public Stream GetStream(uint timeOut) 
		{
			this.timeOut = timeOut;

			if (hostList.StartsWith("/"))
				return CreateSocketStream(null, 0, true);

			string [] dnsHosts = hostList.Split('&');

            System.Random random = new Random((int)DateTime.Now.Ticks);
            int index = random.Next(dnsHosts.Length);
            int pos = 0;
            bool usePipe = (pipeName != null && pipeName.Length != 0);
            Stream stream = null;

            while (pos < dnsHosts.Length)
            {
                if (usePipe)
                    stream = CreateNamedPipeStream(dnsHosts[index]);
                else
                {
#if NET20
                    IPHostEntry ipHE = Dns.GetHostEntry(dnsHosts[index]);
#else
				    IPHostEntry ipHE = Dns.GetHostByName(dnsHosts[index]);
#endif

                    foreach (IPAddress address in ipHE.AddressList)
                    {
                        stream = CreateSocketStream(address, port, false);
                        if (stream != null)
                            break;
                    }
                }
                if (stream != null)
                    break;
                index++;
                if (index == dnsHosts.Length)
                    index = 0;
                pos++;
            }

			return stream;
		}

		private Stream CreateNamedPipeStream( string hostname ) 
		{
			string pipePath;
			if (0 == String.Compare(hostname, "localhost", true))
				pipePath = @"\\.\pipe\" + pipeName;
			else
				pipePath = String.Format(@"\\{0}\pipe\{1}", hostname.ToString(), pipeName);
			return new NamedPipeStream(pipePath, FileAccess.ReadWrite);
		}
		
		private EndPoint CreateUnixEndPoint(string host)
		{
			// first we need to load the Mono.posix assembly
#if NET20
            Assembly a = Assembly.Load("Mono.Posix");
#else
			Assembly a = Assembly.LoadWithPartialName("Mono.Posix");
#endif

			// then we need to construct a UnixEndPoint object
			EndPoint ep = (EndPoint)a.CreateInstance("Mono.Posix.UnixEndPoint", 
				false, BindingFlags.CreateInstance, null, 
				new object[1] { host }, null, null);
			return ep;
        }

        private Stream CreateSocketStream(IPAddress ip, uint port, bool unix)
        {
            SocketStream ss = null;
            try
            {
                //
                // Lets try to connect
                EndPoint endPoint;

                if (!Platform.IsWindows() && unix)
                    endPoint = CreateUnixEndPoint(hostList);
                else
                    endPoint = new IPEndPoint(ip, (int)port);

                ss = unix ?
                    new SocketStream(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP) :
                    new SocketStream(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                ss.Connect(endPoint, (int)timeOut);
                return ss;
            }
            catch (Exception)
            {
                return null;
            }
        }
 
	}
}
