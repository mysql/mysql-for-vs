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


namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for API.
	/// </summary>
	internal class NamedPipeStream : Stream
	{

		int			pipeHandle;
		FileAccess	_mode;

		public NamedPipeStream(string host, FileAccess mode)
		{
			pipeHandle = 0;
			Open(host, mode);
		}

		public void Open( string host, FileAccess mode )
		{
			_mode = mode;
			uint pipemode = 0;

			if ((mode & FileAccess.Read) > 0)
				pipemode |= Win32.GENERIC_READ;
			if ((mode & FileAccess.Write) > 0)
				pipemode |= Win32.GENERIC_WRITE;

			pipeHandle = Win32.CreateFile( host, pipemode,
						0, null, Win32.OPEN_EXISTING, 0, 0 );
//			try 
//			{
//				stream = new FileStream( (IntPtr)pipeHandle, FileAccess.ReadWrite );
//			}
//			catch (Exception ex) 
//			{
//				Console.WriteLine( ex.Message );
//			}

		}

		public bool DataAvailable
		{
			get 
			{
				uint bytesRead=0, avail=0, thismsg=0;

				bool result = Win32.PeekNamedPipe( pipeHandle, 
					null, 0, ref bytesRead, ref avail, ref thismsg );
				return (result == true && avail > 0);
			}
	}

		public override bool CanRead
		{
			get { return (_mode & FileAccess.Read) > 0; }
		}

		public override bool CanWrite
		{
			get { return (_mode & FileAccess.Write) > 0; }
		}

		public override bool CanSeek
		{
			get { throw new NotSupportedException("NamedPipeStream does not support seeking"); }
		}

		public override long Length
		{
			get { throw new NotSupportedException("NamedPipeStream does not support seeking"); }
		}

		public override long Position 
		{
			get { throw new NotSupportedException("NamedPipeStream does not support seeking"); }
			set { }
		}

		public override void Flush() 
		{
//			if (stream != null)
//				stream.Flush();
			if ( pipeHandle == 0 )
				throw new ObjectDisposedException("NamedPipeStream", "The stream has already been closed");
			Win32.FlushFileBuffers(pipeHandle);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
/*			try 
			{
				uint bytesRead=0, avail=0, thismsg=0;

				bool result = Win32.PeekNamedPipe( pipeHandle, 
					null, 0, ref bytesRead, ref avail, ref thismsg );
				if (result)
					return stream.Read( buffer, offset, (int)avail );
				else
					return -1;
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
			}
			return -1;*/
			if (buffer == null) 
				throw new ArgumentNullException("buffer", "The buffer to read into cannot be null");
			if (buffer.Length < (offset + count))
				throw new ArgumentException("Buffer is not large enough to hold requested data", "buffer");
			if (offset < 0) 
				throw new ArgumentOutOfRangeException("offset", offset, "Offset cannot be negative");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Count cannot be negative");
			if (! CanRead)
				throw new NotSupportedException("The stream does not support reading");
			if (pipeHandle == 0)
				throw new ObjectDisposedException("NamedPipeStream", "The stream has already been closed");

			// first read the data into an internal buffer since ReadFile cannot read into a buf at
			// a specified offset
			uint read=0;
			byte[] buf = new Byte[count];
			Win32.ReadFile( pipeHandle, buf, (uint)count, out read, null );
			
			for (int x=0; x < read; x++) 
			{
				buffer[offset+x] = buf[x];
			}
			return (int)read;
		}

		public override void Close()
		{
//			stream.Close();
			//stream = null;
			Win32.CloseHandle(pipeHandle);
			pipeHandle = 0;
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException("NamedPipeStream doesn't support SetLength");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
//			try 
//			{
//				stream.Write( buffer, offset, count );
//			}
//			catch (Exception ex) 
//			{
//				Console.WriteLine( ex.Message );
//			}
			if (buffer == null) 
				throw new ArgumentNullException("buffer", "The buffer to write into cannot be null");
			if (buffer.Length < (offset + count))
				throw new ArgumentException("Buffer does not contain amount of requested data", "buffer");
			if (offset < 0) 
				throw new ArgumentOutOfRangeException("offset", offset, "Offset cannot be negative");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Count cannot be negative");
			if (! CanWrite)
				throw new NotSupportedException("The stream does not support writing");
			if (pipeHandle == 0)
				throw new ObjectDisposedException("NamedPipeStream", "The stream has already been closed");
			
			// copy data to internal buffer to allow writing from a specified offset
			uint bytesWritten = 0;
			bool result;

			if (offset == 0  && count <= 65535)
				result = Win32.WriteFile( pipeHandle, buffer, (uint)count, out bytesWritten, null );
			else
			{
				byte[] localBuf = new byte[65535];

				result = true;
				uint thisWritten;
				while (count != 0 && result)
				{
					int cnt = Math.Min( count, 65535 );
					Array.Copy( buffer, offset, localBuf, 0, cnt );
					result = Win32.WriteFile( pipeHandle, localBuf, (uint)cnt, out thisWritten, null );
					bytesWritten += thisWritten;
					count -= cnt;
					offset += cnt;
				}

//				byte[] tempBuf = new byte[count];
//				try 
//				{
//					Array.Copy( buffer, offset, tempBuf, 0, count );
//				}
//				catch (Exception ex) 
//				{
//					Console.Write(ex.Message);
//				}
//				localBuf = tempBuf;
			}

//			bool result = Win32.WriteFile( pipeHandle, localBuf, (uint)count, out bytesWritten, null );
//			byte[] buf = new Byte[count];
//			for (int x=0; x < count; x++) 
//			{
//				buf[x] = buffer[offset+x];
//			}
//			uint written=0;
//			GCHandle h = GCHandle.Alloc( buffer, GCHandleType.Pinned );
//			IntPtr addr = Marshal.UnsafeAddrOfPinnedArrayElement( buffer, offset );
//			bool result = WriteFile( pipeHandle, addr, (uint)count, ref written, IntPtr.Zero );
//			h.Free();

			if (! result)
			{
				uint err = Win32.GetLastError();
				throw new IOException("Writing to the stream failed");
			}
			if (bytesWritten < count)
				throw new IOException("Unable to write entire buffer to stream");
		}

		public override long Seek( long offset, SeekOrigin origin )
		{
			throw new NotSupportedException("NamedPipeStream doesn't support seeking");
		}
	}
}


