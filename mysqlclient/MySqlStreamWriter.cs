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
using System.Text;
using MySql.Data.Common;

namespace MySql.Data.MySqlClient
{
	internal class MySqlStreamWriter
	{
		private MemoryStream	memStream;
		private Stream			realStream;
		private	Stream			stream;
		private Encoding		encoding;
		private DBVersion		version;

		public MySqlStreamWriter(Stream stream, Encoding enc)
		{
			this.realStream = stream;
			memStream = new MemoryStream();
			this.stream = realStream;
			encoding = enc;
		}

		#region Properties

		public Encoding Encoding 
		{ 
			get { return encoding; }
			set { encoding = value; }
		}

		public Stream Stream 
		{
			get { return stream; }
		}

		public DBVersion Version 
		{
			get { return version; }
			set { version = value; }
		}

		#endregion

		public void StartPacket(long len, bool resetSeq) 
		{
			if (len == 0) 
				stream = memStream;
			if (resetSeq)
				((MySqlStream)realStream).SequenceByte = 0;
			stream.SetLength(len);
		}

		public void Flush() 
		{
			if (stream is MemoryStream) 
			{
				realStream.SetLength(memStream.Length);
				realStream.Write(memStream.GetBuffer(), 0, (int)memStream.Length);
				stream = realStream;
			}
			stream.Flush();
		}


		#region Bytes Methods

		public void WriteByte(byte b) 
		{
			stream.WriteByte(b);
		}

		public void Write(byte[] buffer) 
		{
			stream.Write(buffer, 0, buffer.Length);
		}

		public void Write(byte[] buffer, int offset, int count) 
		{
			stream.Write(buffer, offset, count);
		}

		#endregion

		#region Numeric Methods

		public void WriteLength( long length ) 
		{
			if (length < 251)
				WriteByte( (byte)length );
			else if ( length < 65536L )
			{
				WriteByte( 252 );
				WriteInteger( length, 2 );
			}
			else if ( length < 16777216L )
			{
				WriteByte( 253 );
				WriteInteger( length, 3 );
			}
			else 
			{
				WriteByte( 254 );
				WriteInteger( length, 4 );
			}
		}

		/// <summary>
		/// WriteInteger
		/// </summary>
		/// <param name="v"></param>
		/// <param name="numbytes"></param>
		public void WriteInteger( long v, int numbytes )
		{
			long val = v;

			if (numbytes < 1 || numbytes > 4) 
				throw new ArgumentOutOfRangeException("Wrong byte count for WriteInteger");

			for (int x=0; x < numbytes; x++)
			{
				stream.WriteByte( (byte)(val&0xff) );
				val >>= 8;
			}
		}

		#endregion

		#region String methods

		public void WriteLenString( string s )
		{
			byte[] bytes = encoding.GetBytes(s);
			WriteLength( bytes.Length );
			stream.Write( bytes, 0,bytes.Length );
		}

		public void WriteString(string v )
		{
			WriteStringNoNull( v );
			stream.WriteByte(0);
		}
 
		public void WriteStringNoNull(string v )
		{
			byte[] bytes = encoding.GetBytes(v);
			stream.Write(bytes, 0, bytes.Length);
		}

		#endregion

	}
}
