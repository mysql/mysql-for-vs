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
using System.Threading;
using System.Runtime.InteropServices;

namespace MySql.Data.Common
{
    internal class Semaphore : WaitHandle
    {
        public Semaphore(int initialCount, int maximumCount)
        {
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            IntPtr handle = CreateSemaphore(ref sa, initialCount, 
                maximumCount, null);
            if (handle.Equals(IntPtr.Zero))
            {
                throw new Exception("Unable to create semaphore");
            }
            base.Handle = handle;
        }

        public int Release()
        {
            IntPtr previous = IntPtr.Zero;
            if (!ReleaseSemaphore(base.Handle, 1, previous))
                throw new Exception("Unable to release semaphore");
            return previous.ToInt32();
        }

        public override bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            if ((millisecondsTimeout < 0) && (millisecondsTimeout != -1))
                throw new ArgumentOutOfRangeException("millisecondsTimeout");

            if (exitContext)
                throw new ArgumentException(null, "exitContext");

            int result = WaitForSingleObject(Handle, millisecondsTimeout);
            if (0 == result) return true;
            return false;
        }

        [DllImport("coredll.dll")]
        static extern bool ReleaseSemaphore(IntPtr hSemaphore, 
            int lReleaseCount, IntPtr lpPreviousCount);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern IntPtr CreateSemaphore(
            ref SECURITY_ATTRIBUTES securityAttributes, int initialCount, 
            int maximumCount, string name);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int WaitForSingleObject(IntPtr handle, int millis);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }
}
