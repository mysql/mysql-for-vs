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
    /// Port of Stopwatch class from Compact framework.
    /// The implementation uses Tick counts rather then DateTime.Now
    /// (DateTime.Now can go back when daylight savings are changed)
    /// </summary>
    class Stopwatch
    {
        long millis;
        long startTime;
        public Stopwatch()
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
        }
    }
}
