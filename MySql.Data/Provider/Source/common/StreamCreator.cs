// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Reflection;

namespace MySql.Data.Common
{
    /// <summary>
    /// Summary description for StreamCreator.
    /// </summary>
    internal class StreamCreator
    {
        string hostList;
        uint port;
        string pipeName;
        uint timeOut;

        public StreamCreator(string hosts, uint port, string pipeName)
        {
            hostList = hosts;
            if (hostList == null || hostList.Length == 0)
                hostList = "localhost";
            this.port = port;
            this.pipeName = pipeName;
        }

        public Stream GetStream(uint timeout)
        {
            timeOut = timeout;

            if (hostList.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                return CreateSocketStream(null, true);

            string[] dnsHosts = hostList.Split('&');

            Random random = new Random((int)DateTime.Now.Ticks);
            int index = random.Next(dnsHosts.Length);
            int pos = 0;
            bool usePipe = (pipeName != null && pipeName.Length != 0);
            Stream stream = null;

            while (pos < dnsHosts.Length)
            {
                try
                {
                    if (usePipe)
                    {
#if !CF
                        stream = NamedPipeStream.Create(pipeName, dnsHosts[index]);
#endif
                    }
                    else
                    {
                        IPHostEntry ipHE = GetHostEntry(dnsHosts[index]);
                        foreach (IPAddress address in ipHE.AddressList)
                        {
                            // MySQL doesn't currently support IPv6 addresses
                            if (address.AddressFamily == AddressFamily.InterNetworkV6)
                                continue;
                            stream = CreateSocketStream(address, false);
                            if (stream != null) break;
                        }
                    }
                    if (stream != null)
                        break;
                    index++;
                    if (index == dnsHosts.Length)
                        index = 0;
                    pos++;
                }
                catch (Exception)
                {
                    // if on last host then throw
                    if (pos >= dnsHosts.Length - 1) throw;
                    // else continue
                }
            }

            return stream;
        }

        private static IPHostEntry GetHostEntry(string hostname)
        {
            IPHostEntry ipHE;
#if !CF
            IPAddress addr;
            if (IPAddress.TryParse(hostname, out addr))
            {
                ipHE = new IPHostEntry();
                ipHE.AddressList = new IPAddress[1];
                ipHE.AddressList[0] = addr;
            }
            else
#endif
            ipHE = Dns.GetHostEntry(hostname);
            return ipHE;
        }

#if !CF

		private static EndPoint CreateUnixEndPoint(string host)
		{
			// first we need to load the Mono.posix assembly
			Assembly a = Assembly.Load("Mono.Posix");

			// then we need to construct a UnixEndPoint object
			EndPoint ep = (EndPoint)a.CreateInstance("Mono.Posix.UnixEndPoint",
				false, BindingFlags.CreateInstance, null,
				new object[1] { host }, null, null);
			return ep;
		}
#endif

        private Stream CreateSocketStream(IPAddress ip, bool unix)
        {
            EndPoint endPoint;
#if !CF
			if (!Platform.IsWindows() && unix)
				endPoint = CreateUnixEndPoint(hostList);
			else
#endif
            endPoint = new IPEndPoint(ip, (int)port);

			Socket socket = unix ?
				new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP) :
				new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IAsyncResult ias = socket.BeginConnect(endPoint, null, null);
			if (!ias.AsyncWaitHandle.WaitOne((int)timeOut * 1000, false))
			{
				socket.Close();
				return null;
			}
			try
			{
				socket.EndConnect(ias);
			}
			catch (Exception)
			{
				socket.Close();
                throw;
			}
            NetworkStream stream = new NetworkStream(socket, true);
            GC.SuppressFinalize(socket);
            GC.SuppressFinalize(stream);
            return stream;
		}

    }
}
