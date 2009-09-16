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
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace MySql.Data.Common
{
#if !PocketPC
	/// <summary>
	/// Summary description for SharedMemoryStream.
	/// </summary>
	internal class SharedMemoryStream : Stream
	{
		private string memoryName;
		private AutoResetEvent serverRead;
		private AutoResetEvent serverWrote;
		private AutoResetEvent clientRead;
		private AutoResetEvent clientWrote;
		private IntPtr dataMap;
		private IntPtr dataView;
		private int bytesLeft;
		private int position;
		private int connectNumber;

		private const uint SYNCHRONIZE = 0x00100000;
//		private const uint READ_CONTROL = 0x00020000;
		private const uint EVENT_MODIFY_STATE = 0x2;
//		private const uint EVENT_ALL_ACCESS = 0x001F0003;
		private const uint FILE_MAP_WRITE = 0x2;
		private const int BUFFERLENGTH = 16004;
		private int readTimeout = System.Threading.Timeout.Infinite;
		private int writeTimeout = System.Threading.Timeout.Infinite;

		public SharedMemoryStream(string memName)
		{
			memoryName = memName;
		}

		public void Open(uint timeOut)
		{
			GetConnectNumber(timeOut);
			SetupEvents();
		}

		public override void Close()
		{
			NativeMethods.UnmapViewOfFile(dataView);
            NativeMethods.CloseHandle(dataMap);
		}

		private void GetConnectNumber(uint timeOut)
		{
			AutoResetEvent connectRequest = new AutoResetEvent(false);
            IntPtr handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			memoryName + "_" + "CONNECT_REQUEST");
			if (handle == IntPtr.Zero)
			{
				// If server runs as service, its shared memory is global 
				// And if connector runs in user session, it needs to prefix
				// shared memory name with "Global\"
				string prefixedMemoryName= @"Global\" + memoryName;
				handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
					prefixedMemoryName + "_" + "CONNECT_REQUEST");
				if (handle != IntPtr.Zero)
					memoryName = prefixedMemoryName;
			}
			connectRequest.SafeWaitHandle = new SafeWaitHandle(handle, true);

			AutoResetEvent connectAnswer = new AutoResetEvent(false);
            handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			memoryName + "_" + "CONNECT_ANSWER");
			connectAnswer.SafeWaitHandle = new SafeWaitHandle(handle, true);

            IntPtr connectFileMap = NativeMethods.OpenFileMapping(FILE_MAP_WRITE, false,
				memoryName + "_" + "CONNECT_DATA");
            IntPtr connectView = NativeMethods.MapViewOfFile(connectFileMap, FILE_MAP_WRITE,
				0, 0, (IntPtr)4);

			// now start the connection
			if (!connectRequest.Set())
				throw new MySqlException("Failed to open shared memory connection");

			connectAnswer.WaitOne((int)(timeOut * 1000), false);

			connectNumber = Marshal.ReadInt32(connectView);
		}

		private void SetupEvents()
		{
			string dataMemoryName = memoryName + "_" + connectNumber;
            dataMap = NativeMethods.OpenFileMapping(FILE_MAP_WRITE, false,
				dataMemoryName + "_DATA");
            dataView = (IntPtr)NativeMethods.MapViewOfFile(dataMap, FILE_MAP_WRITE,
					 0, 0, (IntPtr)(int)BUFFERLENGTH);

			serverWrote = new AutoResetEvent(false);
            IntPtr handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
				 dataMemoryName + "_SERVER_WROTE");
			Debug.Assert(handle != IntPtr.Zero);
			serverWrote.SafeWaitHandle = new SafeWaitHandle(handle, true);

			serverRead = new AutoResetEvent(false);
            handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_SERVER_READ");
			Debug.Assert(handle != IntPtr.Zero);
			serverRead.SafeWaitHandle = new SafeWaitHandle(handle, true);

			clientWrote = new AutoResetEvent(false);
            handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_CLIENT_WROTE");
			Debug.Assert(handle != IntPtr.Zero);
			clientWrote.SafeWaitHandle = new SafeWaitHandle(handle, true);

			clientRead = new AutoResetEvent(false);
            handle = NativeMethods.OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_CLIENT_READ");
			Debug.Assert(handle != IntPtr.Zero);
			clientRead.SafeWaitHandle = new SafeWaitHandle(handle, true);

			// tell the server we are ready
			serverRead.Set();
		}

		#region Properties
		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override long Length
		{
			get { throw new NotSupportedException("SharedMemoryStream does not support seeking - length"); }
		}

		public override long Position
		{
			get { throw new NotSupportedException("SharedMemoryStream does not support seeking - postition"); }
			set { }
		}

		#endregion

		public override void Flush()
		{
            NativeMethods.FlushViewOfFile(dataView, 0);
		}

		public bool IsClosed()
		{
			try
			{
                dataView = NativeMethods.MapViewOfFile(dataMap, FILE_MAP_WRITE, 0, 0, (IntPtr)(int)BUFFERLENGTH);
				if (dataView == IntPtr.Zero) return true;
				return false;
			}
			catch (Exception)
			{
				return true;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			Stopwatch stopwatch = new Stopwatch();
			int timeLeft = readTimeout;
			while (bytesLeft == 0)
			{
				if (IsClosed()) return 0;
				if (!serverWrote.WaitOne(timeLeft, false))
				{
					throw new TimeoutException("Timeout when reading from shared memory");
				}
				if (readTimeout != System.Threading.Timeout.Infinite)
				{
					timeLeft = readTimeout - (int)stopwatch.ElapsedMilliseconds;
					if (timeLeft < 0)
						throw new TimeoutException("Timeout when reading from shared memory");
				}

				bytesLeft = Marshal.ReadInt32(dataView);
				position = 4;
			}

			int len = Math.Min(count, bytesLeft);
			long baseMem = dataView.ToInt64() + position;

			for (int i = 0; i < len; i++, position++)
				buffer[offset + i] = Marshal.ReadByte((IntPtr)(baseMem + i));

			bytesLeft -= len;

			if (bytesLeft == 0)
				clientRead.Set();

			return len;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("SharedMemoryStream does not support seeking");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int leftToDo = count;
			int buffPos = offset;

			Stopwatch stopwatch = new Stopwatch();
			int timeLeft = writeTimeout;

			while (leftToDo > 0)
			{
				if (!serverRead.WaitOne(timeLeft))
					throw new TimeoutException("Timeout when writing to shared memory");
				if (writeTimeout != System.Threading.Timeout.Infinite)
				{
					timeLeft = writeTimeout - (int)stopwatch.ElapsedMilliseconds;
					if (timeLeft < 0)
						throw new TimeoutException("Timeout when writing to shared memory");
				}
				int bytesToDo = Math.Min(leftToDo, BUFFERLENGTH);

				long baseMem = dataView.ToInt64() + 4;
				Marshal.WriteInt32(dataView, bytesToDo);
				for (int i = 0; i < bytesToDo; i++, buffPos++)
					Marshal.WriteByte((IntPtr)(baseMem + i), buffer[buffPos]);
				leftToDo -= bytesToDo;
				if (!clientWrote.Set())
					throw new MySqlException("Writing to shared memory failed");
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("SharedMemoryStream does not support seeking");
		}

        public override bool CanTimeout
        {
            get
            {
                return true;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return readTimeout;
            }
            set
            {
                readTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return writeTimeout;
            }
            set
            {
                writeTimeout = value;
            }
        }

    }
#endif
}
