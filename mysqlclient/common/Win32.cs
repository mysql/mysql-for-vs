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
using System.Runtime.InteropServices;

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for Win32.
	/// </summary>
	internal class Win32
	{
		[StructLayout(LayoutKind.Sequential)]
		public class SecurityAttributes 
		{
			public SecurityAttributes() 
			{
				Length = Marshal.SizeOf(typeof(SecurityAttributes));
			}
			public int Length;
			public IntPtr securityDescriptor = IntPtr.Zero;
			public bool inheritHandle = false;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class Overlapped 
		{
			public IntPtr Internal = IntPtr.Zero;
			public IntPtr InternalHigh = IntPtr.Zero;
			public uint Offset = 0;
			public uint OffsetHigh = 0;
			public IntPtr Event = IntPtr.Zero;
		}

		[DllImport("kernel32.dll")]
		public static extern uint GetLastError();

		[DllImport("Kernel32")]
		static extern public int CreateFile(String fileName,
			uint desiredAccess,
			uint shareMode, 
			SecurityAttributes securityAttributes,
			uint creationDisposition,
			uint flagsAndAttributes,
			uint templateFile);

		[DllImport("kernel32.dll", EntryPoint="PeekNamedPipe", SetLastError=true)]
		static extern public bool PeekNamedPipe( int handle,
			byte[] buffer, 
			uint nBufferSize, 
			ref uint bytesRead,
			ref uint bytesAvail, 
			ref uint BytesLeftThisMessage);

		[DllImport("Kernel32")]
		static extern public bool ReadFile(
			int fileHandle,            // handle to file
			byte[] buffer,				// data buffer
			uint numberOfBytesToRead,	// number of bytes to read
			out uint numberOfBytesRead,	// number of bytes read
			Overlapped overlapped		// overlapped buffer
			);

		[DllImport("Kernel32")]
		static extern public bool WriteFile(
			int fileHandle,				// handle to file
			byte[] buffer,					// data buffer
			uint numberOfBytesToWrite,		// number of bytes to write
			out uint numberOfBytesWritten,	// number of bytes written
			Overlapped overlapped			// overlapped buffer
			);

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool CloseHandle( int handle );

		[DllImport("kernel32.dll", SetLastError=true)]
		public static extern bool FlushFileBuffers( int handle );

		//Constants for dwDesiredAccess:
		public const UInt32 GENERIC_READ = 0x80000000;
		public const UInt32 GENERIC_WRITE = 0x40000000;

		//Constants for return value:
		public const Int32 INVALIDpipeHandle_VALUE = -1;

		//Constants for dwFlagsAndAttributes:
		public const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;
		public const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;

		//Constants for dwCreationDisposition:
		public const UInt32 OPEN_EXISTING = 3;
	}
}
