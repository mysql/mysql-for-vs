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
using MySql.Data.Common;
using System.Diagnostics;
using zlib;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for CompressedStream.
	/// </summary>
	internal class CompressedStream : Stream
	{
		// writing fields
		private	Stream					baseStream;
		private MemoryStream			cache;

		// reading fields
        private byte[] localByte;
        private int inPos;
        private int maxInPos;
        private int maxBlockSize;
        private ZInputStream zInStream;

		public CompressedStream( Stream baseStream )
		{
            maxBlockSize = Int32.MaxValue;
			this.baseStream = baseStream;
            localByte = new byte[1];
			cache = new MemoryStream();
		}

		#region Properties

		//TODO: remove comment
/*		public Stream BaseStream 
		{
			get { return baseStream; }
		}
*/
		public override bool CanRead
		{
			get	{ return baseStream.CanRead; }
		}

		public override bool CanWrite
		{
			get	{ return baseStream.CanWrite; }
		}

		public override bool CanSeek
		{
			get	{ return baseStream.CanSeek; }
		}

		public override long Length
		{
			get { return baseStream.Length; }
		}

		public override long Position
		{
			get	{ return baseStream.Position; }
			set	{ baseStream.Position = value; }
		}

		#endregion

		public override void Close()
		{
			baseStream.Close();
			base.Close ();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(Resources.CSNoSetLength);
		}

		public override int ReadByte()
		{
            Read(localByte, 0, 1);
            return localByte[0];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer", Resources.BufferCannotBeNull);
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentOutOfRangeException("offset", Resources.OffsetMustBeValid);
			if ((offset + count) > buffer.Length)
				throw new ArgumentException(Resources.BufferNotLargeEnough, "buffer");

            int totalRead = 0;
            while (count > 0)
            {
                if (inPos == maxInPos)
                {
                    if (maxInPos != 0 && maxInPos < maxBlockSize)
                        break;
                    PrepareNextPacket();
                }
                int countToRead = Math.Min(count, maxInPos-inPos);
                int countRead = 0;
                if (zInStream != null)
                    countRead = zInStream.read(buffer, offset, countToRead);
                else
                    countRead =baseStream.Read(buffer, offset, countToRead);
                offset += countRead;
                count -= countRead;
                inPos += countRead;
                totalRead += countRead;
            }
            
            // if we have read everything from this packet, then reset maxInPos so 
            // that the next time we come here we will load another packet
            if (inPos == maxInPos)
                inPos = maxInPos = 0;

			return totalRead;
		}

        private void PrepareNextPacket()
        {
			// read off the uncompressed and compressed lengths
			byte b1 = (byte)baseStream.ReadByte();
			byte b2 = (byte)baseStream.ReadByte();
			byte b3 = (byte)baseStream.ReadByte();
			int compressedLength = b1 + (b2 << 8) + (b3 << 16);

			baseStream.ReadByte();  // seq
			int unCompressedLength = baseStream.ReadByte() + (baseStream.ReadByte() << 8) + 
				(baseStream.ReadByte() << 16);

            if (unCompressedLength == 0)
            {
                unCompressedLength = compressedLength;
                zInStream = null;
            }
            else
            {
                zInStream = new ZInputStream(baseStream);
            }

            inPos = 0;
            maxInPos = unCompressedLength;
        }

        private MemoryStream CompressCache()
        {
            // small arrays almost never yeild a benefit from compressing
            if (cache.Length < 50)
                return null;

            byte[] cacheBytes = cache.GetBuffer();
            MemoryStream compressedBuffer = new MemoryStream();
            ZOutputStream zos = new ZOutputStream(compressedBuffer, zlibConst.Z_DEFAULT_COMPRESSION);
            zos.Write(cacheBytes, 0, (int)cache.Length);
            zos.finish();

            // if the compression hasn't helped, then just return null
            if (compressedBuffer.Length >= cache.Length)
                return null;
            return compressedBuffer;
        }

        private void CompressAndSendCache()
        {
            long compressedLength = 0, uncompressedLength = 0;

            // we need to save the sequence byte that is written
            byte[] cacheBuffer = cache.GetBuffer();
            byte seq = cacheBuffer[3];
            cacheBuffer[3] = 0;

            // first we compress our current cache
            MemoryStream compressedBuffer = CompressCache();

            // now we set our compressed and uncompressed lengths
            // based on if our compression is going to help or not
            if (compressedBuffer == null)
                compressedLength = cache.Length;
            else
            {
                compressedLength = compressedBuffer.Length;
                uncompressedLength = cache.Length;
            }

            baseStream.WriteByte((byte)(compressedLength & 0xff));
            baseStream.WriteByte((byte)((compressedLength >> 8) & 0xff));
            baseStream.WriteByte((byte)((compressedLength >> 16) & 0Xff));
            baseStream.WriteByte(seq);
            baseStream.WriteByte((byte)(uncompressedLength & 0xff));
            baseStream.WriteByte((byte)((uncompressedLength >> 8) & 0xff));
            baseStream.WriteByte((byte)((uncompressedLength >> 16) & 0Xff));

            if (compressedBuffer == null)
                baseStream.Write(cacheBuffer, 0, (int)cache.Length);
            else
            {
                byte[] compressedBytes = compressedBuffer.GetBuffer();
                baseStream.Write(compressedBytes, 0, (int)compressedBuffer.Length);
            }

            baseStream.Flush();

            cache.SetLength(0);
        }

		public override void Flush() 
		{
            if (!InputDone()) return;

            CompressAndSendCache();
        }

		private bool InputDone() 
		{
            // if we have not done so yet, see if we can calculate how many bytes we are expecting
            if (cache.Length < 4) return false;
            byte[] buf = cache.GetBuffer();
            int expectedLen = buf[0] + (buf[1] << 8) + (buf[2] << 16);
            if (cache.Length < (expectedLen + 4)) return false;
            return true;
		}

		public override void WriteByte(byte value)
		{
			cache.WriteByte(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
            cache.Write(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return baseStream.Seek( offset, origin );
		}

	}
}
