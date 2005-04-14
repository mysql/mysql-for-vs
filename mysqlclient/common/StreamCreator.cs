// Copyright (C) 2004 MySQL AB
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
#if __MonoCS__ 
using Mono.Posix;
#endif

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
		int					timeOut;

		public StreamCreator( string hosts, uint port, string pipeName)
		{
			hostList = hosts;
			if (hostList == null || hostList == String.Empty)
				hostList = "localhost";
			this.port = port;
			this.pipeName = pipeName;
		}

		public Stream GetStream(int timeOut) 
		{
			this.timeOut = timeOut;

			if (hostList.StartsWith("/"))
				return CreateUnixSocketStream();

			string [] dnsHosts = hostList.Split('&');
			ArrayList ipAddresses = new ArrayList();
			ArrayList hostNames = new ArrayList();

			//
			// Each host name specified may contain multiple IP addresses
			// Lets look at the DNS entries for each host name
			foreach (string h in dnsHosts)
			{
				IPHostEntry hostAddress = Dns.GetHostByName(h);
				foreach (IPAddress addr in hostAddress.AddressList)
				{
					ipAddresses.Add( addr );
					hostNames.Add( hostAddress.HostName );
				}
			}

			System.Random random = new Random((int)DateTime.Now.Ticks);
			int index = random.Next(ipAddresses.Count-1);

			bool usePipe = pipeName != String.Empty;
			Stream stream = null;
			for (int i=0; i < ipAddresses.Count; i++)
			{
				if ( pipeName != null )
					stream = CreateNamedPipeStream( (string)hostNames[index] );
				else
					stream = CreateSocketStream( (IPAddress)ipAddresses[index], port );
				if (stream != null) return stream;

				index++;
				if (index == ipAddresses.Count) index = 0;
			}

			return stream;
		}

		private Stream CreateUnixSocketStream() 
		{
#if __MonoCS__ && !WINDOWS

			Socket socket = new Socket (AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

			try
			{
				UnixEndPoint endPoint = new UnixEndPoint (hostList[0]);
				socket.Connect (endPoint);
				return new NetworkStream (socket, true);
			}
			catch (Exception ex)
			{
				return null;
			}
#else
			throw new PlatformNotSupportedException ("Unix sockets are only supported on this platform");
#endif		
		}

		private Stream CreateNamedPipeStream( string hostname ) 
		{
			string pipePath;
			if (hostname.ToLower().Equals("localhost"))
				pipePath = @"\\.\pipe\" + pipeName;
			else
				pipePath = String.Format(@"\\{0}\pipe\{1}", hostname.ToString(), pipeName);
			return new NamedPipeStream(pipePath, FileAccess.ReadWrite);
		}

		private void ConnectSocketCallback( IAsyncResult iar )
		{
			Socket socket = (Socket)iar.AsyncState;
			try 
			{
				socket.EndConnect( iar );
			}
			catch (Exception) 
			{
				// we swallow the exception here because we are working async and are on 
				// a worker thread.
			}
		}

		private Stream CreateSocketStream( IPAddress ip, uint port ) 
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp);

			try
			{
				//
				// Lets try to connect
				IPEndPoint endPoint	= new IPEndPoint( ip, (int)port);

				IAsyncResult iar = socket.BeginConnect( endPoint, 
					new AsyncCallback(ConnectSocketCallback), socket );

				int timeLeft = this.timeOut*1000;
				while (! socket.Connected && timeLeft > 0) 
				{
					Thread.Sleep(100);
					timeLeft -= 100;
				}
				if (! socket.Connected) return null;
				

				socket.SetSocketOption( SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1 );
				return new NetworkStream( socket, true );
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
