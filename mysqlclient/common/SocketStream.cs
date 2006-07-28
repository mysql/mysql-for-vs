using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for MySqlSocket.
	/// </summary>
	internal sealed class SocketStream : Stream
	{
		private Socket	socket;

		public SocketStream(AddressFamily addressFamily, SocketType socketType, ProtocolType protocol)
			: base()
		{
			socket = new Socket(addressFamily, socketType, protocol);
		}

		#region Properties

		public Socket Socket 
		{
			get { return socket; }
		}

		public override bool CanRead
		{
			get	{ return true;	}
		}

		public override bool CanSeek
		{
			get	{ return false;	}
		}

		public override bool CanWrite
		{
			get	{ return true; }
		}

		public override long Length
		{
			get	{ return 0;	}
		}

		public override long Position
		{
			get	{ return 0;	}
			set	{ throw new NotSupportedException("SocketStream does not support seek"); }
		}

		#endregion

		#region Stream Implementation

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return socket.Receive(buffer, offset, count, SocketFlags.None);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("SocketStream does not support seek");
		}

		public override void SetLength(long value)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			socket.Send(buffer, offset, count, SocketFlags.None);
		}


		#endregion

		public bool Connect(EndPoint remoteEP, int timeout)
		{
            // set the socket to non blocking
            socket.Blocking = false;

			// then we star the connect
			SocketAddress addr = remoteEP.Serialize();
			byte[] buff = new byte[addr.Size];
			for (int i=0; i<addr.Size; i++)
				buff[i] = addr[i];

			int result = NativeMethods.connect(socket.Handle, buff, addr.Size);
			int wsaerror = NativeMethods.WSAGetLastError();
			if (wsaerror != 10035)
				throw new Exception("Error creating MySQLSocket");

			// next we wait for our connect timeout or until the socket is connected
			ArrayList write = new ArrayList();
			write.Add(socket);
			ArrayList error = new ArrayList();
			error.Add(socket);

			Socket.Select(null, write, error, timeout*1000*1000);

			if (write.Count == 0) return false;

			// set socket back to blocking mode
            socket.Blocking = true;
			return true;
		}


	}
}
