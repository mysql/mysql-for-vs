// Copyright (c) 2009 Sun Microsystems, Inc.
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
using System.Net.Sockets;
using System.Diagnostics;
using MySql.Data.Common;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Stream that supports timeout of IO operations.
    /// This class is used is used to support timeouts for SQL command, where a 
    /// typical operation involves several network reads/writes. 
    /// Timeout here is defined as the accumulated duration of all IO operations.
    /// </summary>
    
    internal class TimedStream : Stream
    {
        Stream baseStream;

        int timeout;
        int lastReadTimeout;
        int lastWriteTimeout;
        LowResolutionStopwatch stopwatch;
        bool isClosed;


        enum IOKind
        {
            Read,
            Write
        };

        /// <summary>
        /// Construct a TimedStream
        /// </summary>
        /// <param name="baseStream"> Undelying stream</param>
        public TimedStream(Stream baseStream)
        {
            this.baseStream = baseStream;
#if !CF
            timeout = baseStream.ReadTimeout;
#else
            timeout = System.Threading.Timeout.Infinite;
#endif
            isClosed = false;
            stopwatch = new LowResolutionStopwatch();
        }


        /// <summary>
        /// Figure out whether it is necessary to reset timeout on stream.
        /// We track the current value of timeout and try to avoid
        /// changing it too often, because setting Read/WriteTimeout property
        /// on network stream maybe a slow operation that involves a system call 
        /// (setsockopt). Therefore, we allow a small difference, and do not 
        /// reset timeout if current value is slightly greater than the requested
        /// one (within 0.1 second).
        /// </summary>

        private bool ShouldResetStreamTimeout(int currentValue, int newValue)
        {
            if (newValue == System.Threading.Timeout.Infinite
                && currentValue != newValue)
                return true;
            if (newValue > currentValue)
                return true;
            if (currentValue>= newValue + 100)
                return true;

            return false;

        }
        private void StartTimer(IOKind op)
        {

            int streamTimeout;

            if (timeout == System.Threading.Timeout.Infinite)
                streamTimeout = System.Threading.Timeout.Infinite;
            else
                streamTimeout = timeout - (int)stopwatch.ElapsedMilliseconds;

            if (op == IOKind.Read)
            {
                if (ShouldResetStreamTimeout(lastReadTimeout, streamTimeout))
                {
#if !CF
                    baseStream.ReadTimeout = streamTimeout;
#endif
                    lastReadTimeout = streamTimeout;
                }
            }
            else
            {
                if (ShouldResetStreamTimeout(lastWriteTimeout, streamTimeout))
                {
#if !CF
                    baseStream.WriteTimeout = streamTimeout;
#endif
                    lastWriteTimeout = streamTimeout;
                }
            }

            if (timeout == System.Threading.Timeout.Infinite)
                return;

            stopwatch.Start();
        }
        private void StopTimer()
        {
            if (timeout == System.Threading.Timeout.Infinite)
                return;

            stopwatch.Stop();

            // Normally, a timeout exception would be thrown  by stream itself, 
            // since we set the read/write timeout  for the stream.  However 
            // there is a gap between  end of IO operation and stopping the 
            // stop watch,  and it makes it possible for timeout to exceed 
            // even after IO completed successfully.
            if (stopwatch.ElapsedMilliseconds > timeout)
            {
                ResetTimeout(System.Threading.Timeout.Infinite);
                throw new TimeoutException("Timeout in IO operation");
            }
        }
        public override bool CanRead
        {
            get { return baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return baseStream.CanWrite; }
        }

        public override void Flush()
        {
            try
            {
                StartTimer(IOKind.Write);
                baseStream.Flush();
                StopTimer();
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override long Length
        {
            get { return baseStream.Length; }
        }

        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                baseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                StartTimer(IOKind.Read);
                int retval = baseStream.Read(buffer, offset, count);
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override int ReadByte()
        {
            try
            {
                StartTimer(IOKind.Read);
                int retval = baseStream.ReadByte();
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                StartTimer(IOKind.Write);
                baseStream.Write(buffer, offset, count);
                StopTimer();
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override bool CanTimeout
        {
            get { return baseStream.CanTimeout; }
        }

        public override int ReadTimeout
        {
            get { return baseStream.ReadTimeout; }
            set { baseStream.ReadTimeout = value; }
        }
        public override int WriteTimeout
        {
            get { return baseStream.WriteTimeout; }
            set { baseStream.WriteTimeout = value; }
        }

        public override void Close()
        {
            if (isClosed)
                return;
            isClosed = true;
            baseStream.Close();
        }

        public void ResetTimeout(int newTimeout)
        {
            if (newTimeout == System.Threading.Timeout.Infinite || newTimeout == 0)
                timeout = System.Threading.Timeout.Infinite;
            else
                timeout = newTimeout;
            stopwatch.Reset();
        }


        /// <summary>
        /// Common handler for IO exceptions.
        /// Resets timeout to infinity if timeout exception is 
        /// detected and stops the times.
        /// </summary>
        /// <param name="e">original exception</param>
        void HandleException(Exception e)
        {
            stopwatch.Stop();
            ResetTimeout(-1);
        }
    }
}
