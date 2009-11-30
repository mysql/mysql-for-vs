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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MySql.Data.MySqlClient
{
    public class MySqlTrace
    {
        private static TraceSource source = new TraceSource("mysql");

        public static TraceListenerCollection Listeners
        {
            get { return source.Listeners; }
        }

        public static SourceSwitch Switch 
        {
            get { return source.Switch; }
            set { source.Switch = value; }
        }

        internal static TraceSource Source
        {
            get { return source; }
        }

        internal static void LogInformation(int id, string msg)
        {
#if !CF
            Source.TraceEvent(TraceEventType.Information, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceInformation(msg);
#endif
        }

        internal static void LogWarning(int id, string msg)
        {
#if !CF
            Source.TraceEvent(TraceEventType.Warning, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceWarning(msg);
#endif
        }

        internal static void LogError(int id, string msg)
        {
#if !CF
            Source.TraceEvent(TraceEventType.Error, id, msg, MySqlTraceEventType.NonQuery, -1);
            Trace.TraceError(msg);
#endif
        }
    }

    public enum MySqlTraceEventType
    {
        ConnectionOpened = 1,
        ConnectionClosed,
        QueryOpened,
        ResultOpened,
        ResultClosed,
        QueryClosed,
        NonQuery,
        UsageAdvisorWarning
    }

    public enum UsageAdvisorWarningFlags
    {
        NoIndex,
        BadIndex,
        SkippedRows,
        SkippedColumns,
        FieldConversion
    }
}
