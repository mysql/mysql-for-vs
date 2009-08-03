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

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Stream that supports timeout of IO operations.
    /// This class is used is used to support timeouts for SQL command, where a typical operation involves several network reads/writes. 
    /// Timeout here is defined as the accumulated duration of all IO operations.
    /// </summary>
    /// <remarks> Any IO exception or timeout exception closes the stream </remarks>
    internal class TimedStream : Stream
    {
        Stream baseStream;
        Stream inStream;
        Stream outStream;
        int timeout;
        Stopwatch stopwatch;
        bool isClosed;


        /// <summary>
        /// Construct a TimedStream
        /// </summary>
        /// <param name="baseStream"> Undelying stream</param>
        /// <param name="bufferedInput">If input buffering should be used</param>
        /// <param name="bufferedOutput">If output buffering should be used</param>
        public TimedStream(Stream baseStream, bool bufferedInput, bool bufferedOutput)
        {
            this.baseStream = baseStream;
            if (bufferedInput)
                inStream = new BufferedStream(baseStream);
            else
                inStream = baseStream;

            if (bufferedOutput)
                outStream = new BufferedStream(baseStream);
            else
                outStream = baseStream;
            timeout = System.Threading.Timeout.Infinite;
            isClosed = false;
            stopwatch = new Stopwatch();
        }

        private void StartTimer()
        {
            baseStream.ReadTimeout = baseStream.WriteTimeout =
                timeout;
            stopwatch.Start();
        }
        private void StopTimer()
        {
            stopwatch.Stop();

            if (timeout != System.Threading.Timeout.Infinite)
            {
                // Normally, a timeout exception would be thrown  by stream itself, since we set the read/write timeout 
                // for the stream.  However there is a gap between  end of IO operation and stopping the stop watch, 
                // and it makes it possible for timeout to exceed even after IO completed successfully.
                if (stopwatch.ElapsedMilliseconds > timeout)
                {
                    throw new TimeoutException();
                }
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
                StartTimer();
                outStream.Flush();
                StopTimer();
            }
            catch (Exception e)
            {
                HandleTimeout(e);
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
                if (inStream != baseStream)
                {
                    inStream.Position = value;
                }
                if (outStream != baseStream)
                {
                    outStream.Position = value;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                StartTimer();
                int retval = inStream.Read(buffer, offset, count);
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleTimeout(e);
                throw;
            }
        }

        public override int ReadByte()
        {
            try
            {
                StartTimer();
                int retval = inStream.ReadByte();
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleTimeout(e);
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
                StartTimer();
                outStream.Write(buffer, offset, count);
                StopTimer();
            }
            catch (Exception e)
            {
                HandleTimeout(e);
                throw;
            }
        }

        public override bool CanTimeout
        {
            get { return baseStream.CanTimeout; }
        }

        public override int ReadTimeout
        {
            get { return inStream.ReadTimeout; }
            set { inStream.ReadTimeout = value; }
        }
        public override int WriteTimeout
        {
            get { return outStream.WriteTimeout; }
            set { outStream.WriteTimeout = value; }
        }

        public override void Close()
        {
            if (isClosed)
                return;
            baseStream.Close();
            if (inStream != baseStream)
            {
                inStream.Close();
            }
            if (outStream != baseStream)
            {
                outStream.Close();
            }
            isClosed = true;
        }

        public void ResetTimeout(int newTimeout)
        {

            if (newTimeout == System.Threading.Timeout.Infinite)
                timeout = newTimeout;
            else if (newTimeout < 0)
                throw new ArgumentException("Invalid timeout value");
            else if (newTimeout == 0)
                timeout = System.Threading.Timeout.Infinite;
            else
                timeout = newTimeout;
            stopwatch.Reset();
        }


        /// <summary>
        /// Examine the exception chain for exceptions throw during read/write operations
        /// If timeout exception is detected in the chain, new TimeoutException is thrown.
        /// </summary>
        /// <param name="e">original exception</param>
        void HandleTimeout(Exception e)
        {
            stopwatch.Stop();
            Close();
            Exception currentException = e;

            while (currentException != null)
            {
                if (currentException is SocketException)
                {
                    SocketException socketException = (SocketException)currentException;
#if CF
                    if (socketException.NativeErrorCode == 10060)
#else
                    if (socketException.SocketErrorCode == SocketError.TimedOut)
#endif
                    {
                        throw new TimeoutException(e.Message, e);
                    }
                }
                currentException = currentException.InnerException;
            }
        }

    }
}
