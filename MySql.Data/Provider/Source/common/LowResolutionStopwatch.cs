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

namespace MySql.Data.Common
{
    /// <summary>
    /// This class is modeled after .NET Stopwatch. It provides better
    /// performance (no system calls).It is however less precise than
    /// .NET Stopwatch, measuring in milliseconds. It is adequate to use
    /// when high-precision is not required (e.g for measuring IO timeouts),
    /// but not for other tasks.
    /// </summary>
    class LowResolutionStopwatch
    {
        long millis;
        long startTime;
        public static readonly long Frequency = 1000; // measure in milliseconds
        public static readonly bool isHighResolution = false;

        public LowResolutionStopwatch()
        {
            millis = 0;
        }
        public long ElapsedMilliseconds
        {
            get { return millis; }
        }
        public void Start()
        {
            startTime = Environment.TickCount;
        }

        public void Stop()
        {
            long now = Environment.TickCount;
            // Calculate time different, handle possible overflow
            long elapsed = (now < startTime)?Int32.MaxValue - startTime + now : now - startTime;
            millis += elapsed;
        }

        public void Reset()
        {
            millis = 0;
            startTime = 0;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return new TimeSpan(0, 0, 0, 0, (int)millis);
            }
        }

        public static LowResolutionStopwatch StartNew()
        {
            LowResolutionStopwatch sw = new LowResolutionStopwatch();
            sw.Start();
            return sw;
        }

        public static long GetTimestamp()
        {
            return Environment.TickCount;
        }

        bool IsRunning()
        {
            return (startTime != 0);
        }
    }
}
