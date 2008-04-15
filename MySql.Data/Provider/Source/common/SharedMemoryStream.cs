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
			UnmapViewOfFile(dataView);
			CloseHandle(dataMap);
		}

		private void GetConnectNumber(uint timeOut)
		{
			AutoResetEvent connectRequest = new AutoResetEvent(false);
			IntPtr handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			memoryName + "_" + "CONNECT_REQUEST");
#if NET20
			connectRequest.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			connectRequest.Handle = handle;
#endif

			AutoResetEvent connectAnswer = new AutoResetEvent(false);
			handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			memoryName + "_" + "CONNECT_ANSWER");
#if NET20
			connectAnswer.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			connectAnswer.Handle = handle;
#endif

			IntPtr connectFileMap = OpenFileMapping(FILE_MAP_WRITE, false,
				memoryName + "_" + "CONNECT_DATA");
			IntPtr connectView = MapViewOfFile(connectFileMap, FILE_MAP_WRITE,
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
			dataMap = OpenFileMapping(FILE_MAP_WRITE, false,
				dataMemoryName + "_DATA");
			dataView = (IntPtr)MapViewOfFile(dataMap, FILE_MAP_WRITE,
					 0, 0, (IntPtr)(int)BUFFERLENGTH);

			serverWrote = new AutoResetEvent(false);
			IntPtr handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
				 dataMemoryName + "_SERVER_WROTE");
			Debug.Assert(handle != IntPtr.Zero);
#if NET20
			serverWrote.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			serverWrote.Handle = handle;
#endif

			serverRead = new AutoResetEvent(false);
			handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_SERVER_READ");
			Debug.Assert(handle != IntPtr.Zero);
#if NET20
			serverRead.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			serverRead.Handle = handle;
#endif

			clientWrote = new AutoResetEvent(false);
			handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_CLIENT_WROTE");
			Debug.Assert(handle != IntPtr.Zero);
#if NET20
			clientWrote.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			clientWrote.Handle = handle;
#endif

			clientRead = new AutoResetEvent(false);
			handle = OpenEvent(SYNCHRONIZE | EVENT_MODIFY_STATE, false,
			dataMemoryName + "_CLIENT_READ");
			Debug.Assert(handle != IntPtr.Zero);
#if NET20
			clientRead.SafeWaitHandle = new SafeWaitHandle(handle, true);
#else
			clientRead.Handle = handle;
#endif

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
			FlushViewOfFile(dataView, 0);
		}

		public bool IsClosed()
		{
			try
			{
				dataView = MapViewOfFile(dataMap, FILE_MAP_WRITE, 0, 0, (IntPtr)(int)BUFFERLENGTH);
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
			while (bytesLeft == 0)
			{
				while (!serverWrote.WaitOne(500, false))
				{
					if (IsClosed()) return 0;
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

			while (leftToDo > 0)
			{
				if (!serverRead.WaitOne())
					throw new MySqlException("Writing to shared memory failed");

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



		#region Imports
		[DllImport("kernel32.dll")]
		static extern IntPtr OpenEvent(uint dwDesiredAccess, bool bInheritHandle,
			string lpName);

		//		[DllImport("kernel32.dll")]
		//		static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll")]
		static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle,
			string lpName);

		[DllImport("kernel32.dll")]
		static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint
			dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow,
			IntPtr dwNumberOfBytesToMap);

		[DllImport("kernel32.dll")]
		static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int FlushViewOfFile(IntPtr address, uint numBytes);

		#endregion


	}
#endif
}
