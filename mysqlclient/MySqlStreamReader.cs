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
using System.Diagnostics;
using System.Text;
using System.IO;
using MySql.Data.Common;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for MySqlStreamReader.
	/// </summary>
	internal class MySqlStreamReader
	{
		private Stream		stream;
		private Encoding	encoding;
		private DBVersion	version;
		private bool		isLastPacket;

		public MySqlStreamReader(Stream str, Encoding enc)
		{
			stream = str;
			encoding = enc;
		}

		#region Properties

		public DBVersion Version 
		{
			get { return version; }
			set { version = value; }
		}

		public Stream Stream 
		{
			get { return stream; }
		}

		public Encoding Encoding 
		{
			get { return encoding; }
			set { encoding = value; }
		}

		public bool HasMoreData
		{
			get 
			{ 
				if (stream is MySqlStream)
					return ((MySqlStream)stream).HasMoreData;
				return stream.Length > stream.Position; 
			}
		}

		public bool IsLastPacket
		{
			get { return isLastPacket; }
		}

		#endregion

		public void Close() 
		{
			stream.Close();
		}

		#region Byte Reading Methods

		public byte ReadByte() 
		{
			int b = stream.ReadByte();

			if (b == -1)
				throw new MySqlException("Lost connection to server");
			return (byte)b;
		}

		public int Read(byte[] buf, int offset, int len)
		{
			try 
			{
				int totalRead = 0;
				while (len > 0) 
				{
					int bytesRead = stream.Read(buf, offset, len);

					if (bytesRead == 0)
						throw new MySqlException( "Connection unexpectedly terminated", true, null );

					offset += bytesRead;
					len -= bytesRead;
					totalRead += bytesRead;
				}

				return totalRead;
			}
			catch (IOException ioe) 
			{
				Logger.LogException( ioe ) ;
				throw new MySqlException( "Connection unexpectedly terminated", true, ioe );
			}		
		}

		public int ReadNBytes()
		{
			byte c = (byte)ReadByte();
			if (c < 1 || c > 4) throw new MySqlException("Unexpected byte count received");
			return ReadInteger((int)c);
		}

		public void SkipBytes(int len)
		{
			while (len-- > 0)
				stream.ReadByte();
		}

		#endregion

		#region String Functions

		public string ReadString(long length) 
		{
			byte[] buf = new byte[length];
			stream.Read(buf, 0, (int)length);
			return encoding.GetString(buf, 0, buf.Length);
		}

		public string ReadString() 
		{
			MemoryStream ms = new MemoryStream();

			int b = stream.ReadByte();
			while (b != 0 && b != -1)
			{
				ms.WriteByte((byte)b);
				b = stream.ReadByte();
			}

			return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
		}

		public string ReadLenString()
		{
			long len = ReadPackedInteger();
			return ReadString(len);
		}

		#endregion

		#region Numeric Functions

		public long GetFieldLength() 
		{
			byte c  = (byte)ReadByte();

			switch(c) 
			{
				case 251 : return (long)-1;
				case 252 : return (long)ReadInteger(2);
				case 253 : return (long)ReadInteger(3);
				case 254 : return (long)ReadInteger(8);
				default  : return c;
			}
		}

		public ulong ReadLong(int numbytes) 
		{
			ulong val = 0;
			int raise = 1;
			for (int x=0; x < numbytes; x++)
			{
				int b = ReadByte();
				val += (ulong)(b*raise);
				raise *= 256;
			}
			return val;
		}

		public int ReadInteger(int numbytes)
		{
			return (int)ReadLong(numbytes);
		}

		public int ReadPackedInteger()
		{
			byte c  = (byte)ReadByte();

			switch(c) 
			{
				case 251 : return -1;
				case 252 : return ReadInteger(2);
				case 253 : return ReadInteger(3);
				case 254 : return ReadInteger(4);
				default  : return c;
			}
		}

		#endregion

		#region Packet methods

		public void OpenPacket() 
		{
			MySqlStream ms = (stream as MySqlStream);
			if (ms == null)
				Debug.Assert(ms != null);

			// make sure we have read all the data from the previous packet
			Debug.Assert(HasMoreData == false, "HasMoreData is true in OpenPacket");

			ms.LoadPacket();

			int peek = ms.PeekByte();
			if (peek == 0xff) 
			{
				int code = ReadInteger(2);
				string msg = ReadString();
				throw new MySqlException(msg, code);
			}
			isLastPacket = (peek == 0xfe && (stream.Length < 9));
		}

		public void SkipPacket() 
		{
			MySqlStream ms = (stream as MySqlStream);
			if (ms == null)
				System.Diagnostics.Debug.Assert(ms != null);

			byte[] tempBuf = new byte[1024];
            long left = ms.Length - ms.Position;
            while (left > 0)
            {
                long toRead = Math.Min(left, tempBuf.LongLength);
                ms.Read(tempBuf, 0, (int)toRead);
                left -= toRead;
            }
		}

		#endregion

	}
}
