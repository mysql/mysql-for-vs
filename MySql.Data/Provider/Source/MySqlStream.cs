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
            //Debug.Assert(packet.Position == packet.Length);

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
         /// Reads the specified number of bytes from the stream and stores them at given 
         /// offset in the buffer.
         /// Throws EndOfStreamException if not all bytes can be read.
         /// </summary>
         /// <param name="stream">Stream to read from</param>
         /// <param name="buffer"> Array to store bytes read from the stream </param>
         /// <param name="offset">The offset in buffer at which to begin storing the data read from the current stream. </param>
         /// <param name="count">Number of bytes to read</param>
         internal static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
         {
             int numRead = 0;
             int numToRead = count;
             while (numToRead > 0)
             {
                 int read = stream.Read(buffer, offset + numRead, numToRead);
                 if (read == 0)
                 {
                     throw new EndOfStreamException();
                 }
                 numRead += read;
                 numToRead -= read;
             }
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
 
                    // make roo for the next block
                    packet.Length += length;

                    ReadFully(inStream, packet.Buffer, offset, length);
                    offset += length;

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

        public void SendEntirePacketDirectly(byte[] buffer, int count)
        {
            buffer[0] = (byte)(count & 0xff);
            buffer[1] = (byte)((count >> 8) & 0xff);
            buffer[2] = (byte)((count >> 16) & 0xff);
            buffer[3] = sequenceByte++;
            outStream.Write(buffer, 0, count + 4);
            outStream.Flush();
        }

		#endregion
	}
}
