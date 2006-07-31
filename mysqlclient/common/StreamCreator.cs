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
        ManualResetEvent    waitHandle;        

		public StreamCreator( string hosts, uint port, string pipeName)
		{
			hostList = hosts;
			if (hostList == null || hostList.Length == 0)
				hostList = "localhost";
			this.port = port;
			this.pipeName = pipeName;
            waitHandle = new ManualResetEvent(false);
        }

		public Stream GetStream(uint timeOut) 
		{
			this.timeOut = timeOut;

			if (hostList.StartsWith("/"))
				return CreateSocketStream(null, 0, true);

			string [] dnsHosts = hostList.Split('&');
			ArrayList ipAddresses = new ArrayList();
			ArrayList hostNames = new ArrayList();

			//
			// Each host name specified may contain multiple IP addresses
			// Lets look at the DNS entries for each host name
			foreach (string h in dnsHosts)
			{
#if NET20
                IPHostEntry hostAddress = Dns.GetHostEntry(h);
#else
				IPHostEntry hostAddress = Dns.GetHostByName(h);
#endif
				foreach (IPAddress addr in hostAddress.AddressList)
				{
					ipAddresses.Add( addr );
					hostNames.Add( hostAddress.HostName );
				}
			}

			System.Random random = new Random((int)DateTime.Now.Ticks);
			int index = random.Next(ipAddresses.Count);

			bool usePipe = (pipeName != null && pipeName.Length != 0);
			Stream stream = null;
			for (int i=0; i < ipAddresses.Count; i++)
			{
				if (usePipe)
					stream = CreateNamedPipeStream( (string)hostNames[index] );
				else
					stream = CreateSocketStream( (IPAddress)ipAddresses[index], port, false );
				if (stream != null) return stream;

				index++;
				if (index == ipAddresses.Count) index = 0;
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

        private void ConnectCallback(IAsyncResult ias)
        {
            Socket s = (ias.AsyncState as Socket);
            if (s.Connected)
                waitHandle.Set();
        }

		private Stream CreateSocketStream( IPAddress ip, uint port, bool unix ) 
		{
			try
			{
				//
				// Lets try to connect
                EndPoint endPoint;
                
				if (!Platform.IsWindows() && unix)
					endPoint = CreateUnixEndPoint(hostList);
				else
					endPoint = 	new IPEndPoint(ip, (int)port);

                waitHandle.Reset();
                Socket s = unix ?
                    new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP) :
                    new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult ias = s.BeginConnect(endPoint, 
                    new AsyncCallback(ConnectCallback), s);

                if (ias.CompletedSynchronously || 
                    waitHandle.WaitOne((int)timeOut * 1000, false))
                    return new NetworkStream(s);
                else 
                {
                    s.EndConnect(ias);
                    return null;
                }
            }
			catch (ArgumentNullException are)
			{
				Logger.LogException(are);
                return null;
			}
			catch (SocketException se) 
			{
				Logger.LogException(se);
                return null;
			}
			catch (ObjectDisposedException ode) 
			{
				Logger.LogException(ode);
				return null;
			}
		}
 
	}
}
