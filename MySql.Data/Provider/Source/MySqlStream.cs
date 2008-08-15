// Copyright (C) 2004-2007 MySQL AB
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
using System.Diagnostics;
using System.Text;
using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for MySqlStream.
	/// </summary>
	internal class MySqlStream
	{
		private byte sequenceByte;
		private Encoding encoding;
		private MemoryStream bufferStream;
		private int maxBlockSize;
		private ulong maxPacketSize;
		private Stream inStream;
		private Stream outStream;
		private byte[] tempBuffer = new byte[4];
        MySqlPacket packet = new MySqlPacket();


		public MySqlStream(Encoding encoding)
		{
			// we have no idea what the real value is so we start off with the max value
			// The real value will be set in NativeDriver.Configure()
			maxPacketSize = ulong.MaxValue;

			// we default maxBlockSize to MaxValue since we will get the 'real' value in 
			// the authentication handshake and we know that value will not exceed 
			// true maxBlockSize prior to that.
			maxBlockSize = Int32.MaxValue;

			this.encoding = encoding;
			bufferStream = new MemoryStream();
		}

        public MySqlStream(Stream baseStream, Encoding encoding, bool compress)
            : this(encoding)
        {

            inStream = new BufferedStream(baseStream);
            outStream = baseStream;
            if (compress)
            {
                inStream = new CompressedStream(inStream);
                outStream = new CompressedStream(outStream);
            }
        }

		public void Close()
		{
			inStream.Close();
            // no need to close outStream because closing
            // inStream closes the underlying network stream
            // for us.
		}

		#region Properties

		public Encoding Encoding
		{
			get { return encoding; }
			set { encoding = value; }
		}

		public byte SequenceByte
		{
			get { return sequenceByte; }
			set { sequenceByte = value; }
		}

		public int MaxBlockSize
		{
			get { return maxBlockSize; }
			set { maxBlockSize = value; }
		}

		public ulong MaxPacketSize
		{
			get { return maxPacketSize; }
			set { maxPacketSize = value; }
		}

		#endregion

		#region Packet methods

		/// <summary>
		/// ReadPacket is called by NativeDriver to start reading the next
		/// packet on the stream.
		/// </summary>
		public MySqlPacket ReadPacket()
		{
//			if (HasMoreData)
//			{
//				SkipBytes((int)(inLength - inPos));
//			}
			// make sure we have read all the data from the previous packet
			//Debug.Assert(HasMoreData == false, "HasMoreData is true in OpenPacket");

			LoadPacket();

            // now we check if this packet is a server error
			if (packet.Buffer[0] == 0xff)
			{
				packet.ReadByte();  // read off the 0xff

				int code = packet.ReadInteger(2);
				string msg = packet.ReadString();
                if (msg.StartsWith("#"))
                {
                    msg.Substring(1, 5);  /* state code */
                    msg = msg.Substring(6);
                }
				throw new MySqlException(msg, code);
			}

            packet.Encoding = encoding;
            return packet;
		}

		/// <summary>
		/// LoadPacket loads up and decodes the header of the incoming packet.
		/// </summary>
		public void LoadPacket()
		{
			try
			{
                packet.Length = 0;
                int offset = 0;
                while (true)
                {
                    int b1 = inStream.ReadByte();
                    int b2 = inStream.ReadByte();
                    int b3 = inStream.ReadByte();
                    int seqByte = inStream.ReadByte();

                    if (b1 == -1 || b2 == -1 || b3 == -1 || seqByte == -1)
                        throw new MySqlException(
                             Resources.ConnectionBroken, true, null);

                    sequenceByte = (byte)++seqByte;
                    int length = (int)(b1 + (b2 << 8) + (b3 << 16));
                    int leftToRead = length;

                    // make roo for the next block
                    packet.Length += length;

                    while (leftToRead > 0)
                    {
                        int read = inStream.Read(packet.Buffer, offset, leftToRead);
                        leftToRead -= read;
                        offset += read;
                    }
                    // if this block was < maxBlock then it's last one in a multipacket series
                    if (length < maxBlockSize) break;
                }
                packet.Position = 0;
            }
			catch (IOException ioex)
			{
				throw new MySqlException(Resources.ReadFromStreamFailed, true, ioex);
			}
		}

        public void SendPacket(MySqlPacket packet)
        {
            byte[] buffer = packet.Buffer;
            int length = packet.Position-4;

            if ((ulong)length > maxPacketSize)
                throw new MySqlException(Resources.QueryTooLarge, (int)MySqlErrorCode.PacketTooLarge);

            int offset = 0;
            while (length > 0)
            {
                int lenToSend = length > maxBlockSize ? maxBlockSize : length;
                buffer[offset] = (byte)(lenToSend & 0xff);
                buffer[offset+1] = (byte)((lenToSend >> 8) & 0xff);
                buffer[offset+2] = (byte)((lenToSend >> 16) & 0xff);
                buffer[offset+3] = sequenceByte++;

                outStream.Write(buffer, offset, lenToSend + 4);
                outStream.Flush();
                length -= lenToSend;
                offset += lenToSend;
            }
        }

		/// <summary>
		/// SkipPacket will read the remaining bytes of a packet into a small
		/// local buffer and discard them.
		/// </summary>
/*		public void SkipPacket()
		{
			byte[] tempBuf = new byte[1024];
			while (inPos < inLength)
			{
				int toRead = (int)Math.Min((ulong)tempBuf.Length, (inLength - inPos));
				Read(tempBuf, 0, toRead);
            }
		}*/

        public void SendEntirePacketDirectly(byte[] buffer, int count)
        {
            buffer[0] = (byte)(count & 0xff);
            buffer[1] = (byte)((count >> 8) & 0xff);
            buffer[2] = (byte)((count >> 16) & 0xff);
            buffer[3] = sequenceByte++;
            outStream.Write(buffer, 0, count + 4);
            outStream.Flush();
        }

        /// <summary>
		/// StartOutput is used to reset the write state of the stream.
		/// </summary>
/*		public void StartOutput(ulong length, bool resetSequence)
		{
			outLength = outPos = 0;
			if (length > 0)
			{
				if (length > maxPacketSize)
					throw new MySqlException(Resources.QueryTooLarge, (int)MySqlErrorCode.PacketTooLarge);
				outLength = length;
			}

			if (resetSequence)
				sequenceByte = 0;
		}
*/
		/// <summary>
		/// Writes out the header that is used at the start of a transmission
		/// and at the beginning of every packet when multipacket is used.
		/// </summary>
//		private void WriteHeader()
//		{
//			int len = (int)Math.Min((outLength - outPos), (ulong)maxBlockSize);
        //
		//	outStream.WriteByte((byte)(len & 0xff));
		//	outStream.WriteByte((byte)((len >> 8) & 0xff));
		//	outStream.WriteByte((byte)((len >> 16) & 0xff));
		//	outStream.WriteByte(sequenceByte++);
		//}

/*		public void SendEmptyPacket()
		{
			outLength = 0;
			outPos = 0;
			WriteHeader();
			outStream.Flush();
		}
*/
		#endregion

/*		#region Byte methods

		public int ReadNBytes()
		{
			byte c = (byte)ReadByte();
			if (c < 1 || c > 4)
				throw new MySqlException(Resources.IncorrectTransmission);
			return ReadInteger(c);
		}

		public void SkipBytes(int len)
		{
			while (len-- > 0)
				ReadByte();
		}

		/// <summary>
		/// Reads the next byte from the incoming stream
		/// </summary>
		/// <returns></returns>
		public int ReadByte()
		{
			int b;
			if (peekByte != -1)
			{
				b = PeekByte();
				peekByte = -1;
				inPos++;   // we only do this here since Read will also do it
			}
			else
			{
				// we read the byte this way because we might cross over a 
				// multipacket boundary
				int cnt = Read(byteBuffer, 0, 1);
				if (cnt <= 0)
					return -1;
				b = byteBuffer[0];
			}
			return b;
		}*/

		/// <summary>
		/// Reads a block of bytes from the input stream into the given buffer.
		/// </summary>
		/// <returns>The number of bytes read.</returns>
/*		public int Read(byte[] buffer, int offset, int count)
		{
			// we use asserts here because this is internal code
			// and we should be calling it correctly in all cases
			Debug.Assert(buffer != null);
			Debug.Assert(offset >= 0 &&
				(offset < buffer.Length || (offset == 0 && buffer.Length == 0)));
			Debug.Assert(count >= 0);
			Debug.Assert((offset + count) <= buffer.Length);

			int totalRead = 0;

			while (count > 0)
			{
				// if we have peeked at a byte, then read it off first.
				if (peekByte != -1)
				{
					buffer[offset++] = (byte)ReadByte();
					count--;
                    totalRead++;
					continue;
				}

				// check if we are done reading the current packet
				if (inPos == inLength)
				{
					// if yes and this block is not max size, then we are done
					if (inLength < (ulong)maxBlockSize)
						return 0;

					// the current block is maxBlockSize so we need to read
					// in another block to continue
					LoadPacket();
				}

				int lenToRead = Math.Min(count, (int)(inLength - inPos));
				try
				{
					int read = inStream.Read(buffer, offset, lenToRead);

					// we don't throw an exception here even though this probably
					// indicates a broken connection.  We leave that to the 
					// caller.
					if (read == 0)
						break;

					count -= read;
					offset += read;
					totalRead += read;
					inPos += (ulong)read;
				}
				catch (IOException ioex)
				{
					throw new MySqlException(Resources.ReadFromStreamFailed, true, ioex);
				}
			}

			return totalRead;
		}



		public void Write(byte[] buffer, int offset, int count)
		{
			Debug.Assert(buffer != null && offset >= 0 && count >= 0);

			// if we are buffering, then just write it to the buffer
			if (outLength == 0)
			{
				bufferStream.Write(buffer, offset, count);
				return;
			}

			// make sure the inputs to the method make sense
			Debug.Assert(outLength > 0 && (outPos + (ulong)count) <= outLength);

			int pos = 0;
			// if we get here, we are not buffering.  
			// outLength is the total amount of data we are going to send
			// This means that multiple calls to write could be combined.
			while (count > 0)
			{
				int cntToWrite = (int)Math.Min((outLength - outPos), (ulong)count);
				cntToWrite = Math.Min(maxBlockSize - (int)(outPos % (ulong)maxBlockSize), cntToWrite);

				// if we are at a block border, then we need to send a new header
				if ((outPos % (ulong)maxBlockSize) == 0)
					WriteHeader();

				try
				{
					outStream.Write(buffer, pos, cntToWrite);
				}
				catch (IOException ioex)
				{
					throw new MySqlException(Resources.WriteToStreamFailed, true, ioex);
				}

				outPos += (ulong)cntToWrite;
				pos += cntToWrite;
				count -= cntToWrite;
			}
		}


		public void Flush()
		{
			if (outLength == 0)
			{
				if (bufferStream.Length > 0)
				{
					byte[] bytes = bufferStream.GetBuffer();
					StartOutput((ulong)bufferStream.Length, false);
					Write(bytes, 0, (int)bufferStream.Length);
				}
				bufferStream.SetLength(0);
				bufferStream.Position = 0;
			}

            try
            {
                outStream.Flush();
            }
            catch (IOException ioex)
            {
                throw new MySqlException(Resources.WriteToStreamFailed, true, ioex);
            }
        }
        */
		//#endregion


	}
}
