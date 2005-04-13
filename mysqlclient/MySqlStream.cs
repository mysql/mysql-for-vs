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

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for MySqlStream.
	/// </summary>
	internal class MySqlStream : Stream
	{
		private Stream	baseStream;
		private long	maxSinglePacket;
		private long	readLength;
		private long	readPos;
		private long	writePos;
		private long	writeLength;
		private	byte	sequenceByte;
		private int		peekByte;
		private int		leftInChunk;
		private byte[]	buffer;
		private int		bufferPos;
		private int		bufferLength;

		public MySqlStream(Stream baseStr)
		{
			baseStream = baseStr;
			maxSinglePacket = 255*255*255;
			leftInChunk = 0;
			peekByte = -1;
			bufferLength = 0x1000;
			buffer = new byte[bufferLength];
		}

		#region Properties

		public byte SequenceByte 
		{
			get { return sequenceByte; }
			set { sequenceByte = value; }
		}

		public bool HasMoreData 
		{
			get { return (readLength == maxSinglePacket || readPos < readLength); }
		}

		public long MaxSinglePacket 
		{
			get { return maxSinglePacket; }
			set { maxSinglePacket = value; }
		}

		public override bool CanRead
		{
			get	{ return true; }
		}

		public override bool CanWrite
		{
			get	{ return true;	}
		}

		public override bool CanSeek
		{
			get	{ return false;	}
		}

		public override long Length
		{
			get	{ return readLength;	}
		}

		public override long Position
		{
			get	{ return readPos;	}
			set	{	}
		}

		#endregion

		public override void Flush()
		{
			FlushWrite();
			baseStream.Flush();
		}

		public int PeekByte() 
		{
			if (peekByte == -1)  
			{
				peekByte = ReadByte();
				readPos--;
			}
			return peekByte;
		}

		public override int ReadByte()
		{
			byte[] buf = new byte[1];
			int cnt = Read(buf, 0, 1);
			if (cnt == 0) return -1;
			return (int)buf[0];
		}


		public override int Read(byte[] buffer, int offset, int count)
		{
			int totalRead = 0;

			while (count > 0) 
			{
				int read = 0;

				if (peekByte != -1) 
				{
					buffer[offset] = (byte)peekByte;
					peekByte = -1;
					read++;
				}
				else 
				{
					if (readPos == readLength) 
					{
						if (readPos == MaxSinglePacket)
							LoadPacket();
						else
							break;
					}

					int lenToRead = Math.Min(count, (int)(readLength-readPos));
					read = baseStream.Read(buffer, offset, lenToRead);
				}

				if (read == 0) break;
				count -= read;
				offset += read;
				totalRead += read;
				readPos += read;
			}

			return totalRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;
		}

		public override void SetLength(long value)
		{
			writeLength = value;
			writePos = 0;
			WriteHeader();
		}

		private void WriteHeader() 
		{
			int tempLeft = (int)Math.Min((long)maxSinglePacket, writeLength);
			WriteByte((byte)(tempLeft & 0xff));
			WriteByte((byte)((tempLeft >> 8) & 0xff));
			WriteByte((byte)((tempLeft >> 16) & 0xff));
			WriteByte((byte)sequenceByte++);
			leftInChunk = tempLeft;
		}

		private void FlushWrite() 
		{
			baseStream.Write(buffer, 0, bufferPos);
			bufferPos = 0;
		}

		public override void WriteByte(byte value)
		{
			if (bufferPos == bufferLength)
				FlushWrite();
			buffer[bufferPos++] = value;
			leftInChunk--;
			writePos++;
		}

		public override void Write(byte[] src, int offset, int count)
		{
			while (count > 0)
			{
				if (bufferPos == bufferLength)
					FlushWrite();

				System.Diagnostics.Debug.Assert(leftInChunk > 0 || writeLength > 0);
				if (leftInChunk == 0 && writeLength > 0)
					WriteHeader();

				int toWrite = Math.Min(count, leftInChunk);
				toWrite = Math.Min(toWrite, bufferLength-bufferPos);
				Buffer.BlockCopy(src, offset, buffer, bufferPos, toWrite);
				offset += toWrite;
				bufferPos += toWrite;
				count -= toWrite;
				leftInChunk -= toWrite;
				writePos += toWrite;
			}

		}

		public void LoadPacket() 
		{
			int b1 = baseStream.ReadByte();
			int b2 = baseStream.ReadByte();
			int b3 = baseStream.ReadByte();
			int seqByte = baseStream.ReadByte();

			if (b1 == -1 || b2 == -1 || b3 == -1 || seqByte == -1)
				throw new MySqlException( "Connection unexpectedly terminated", true, null );

			sequenceByte = (byte)++seqByte;
			readLength = b1 + (b2 << 8) + (b3 << 16);
			readPos = 0;
		}

	}
}
