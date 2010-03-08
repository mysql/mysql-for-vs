// Copyright (c) 2009-2010 Sun Microsystems, Inc.
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
using System.Reflection;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    public class MySqlTrace
    {
        private static string qaHost;
        private static bool qaEnabled = false;

#if !CF
        private static TraceSource source = new TraceSource("mysql");

        static MySqlTrace()
        {
            foreach (TraceListener listener in source.Listeners)
            {
                if (listener.GetType().ToString().Contains("MySql.EMTrace.EMTraceListener"))
                {
                    qaEnabled = true;
                    break;
                }
            }
        }

        public static TraceListenerCollection Listeners
        {
            get { return source.Listeners; }
        }

        public static SourceSwitch Switch 
        {
            get { return source.Switch; }
            set { source.Switch = value; }
        }

        public static bool QueryAnalysisEnabled
        {
            get { return qaEnabled; }
        }

        public static void EnableQueryAnalyzer(string host, int postInterval)
        {
            if (qaEnabled) return;
            // create a EMTraceListener and add it to our source
            TraceListener l = (TraceListener)Activator.CreateInstance("MySql.EMTrace", 
                "MySql.EMTrace.EMTraceListener", false, BindingFlags.CreateInstance,
                null, new object[] { host, postInterval }, null, null, null).Unwrap();
            if (l == null)
                throw new MySqlException(Resources.UnableToEnableQueryAnalysis);
            source.Listeners.Add(l);
            Switch.Level = SourceLevels.All;
        }

        public static void DisableQueryAnalyzer()
        {
            qaEnabled = false;
            foreach (TraceListener l in source.Listeners)
                if (l.GetType().ToString().Contains("EMTraceListener"))
                {
                    source.Listeners.Remove(l);
                    break;
                }
        }

        internal static TraceSource Source
        {
            get { return source; }
        }
#endif

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

#if !CF
        internal static void TraceEvent(TraceEventType eventType,
            MySqlTraceEventType mysqlEventType, string msgFormat, params object[] args)
        {
            Source.TraceEvent(eventType, (int)mysqlEventType, msgFormat, args);
        }
#endif
    }

    public enum MySqlTraceEventType : int
    {
        ConnectionOpened = 1,
        ConnectionClosed,
        QueryOpened,
        ResultOpened,
        ResultClosed,
        QueryClosed,
        StatementPrepared,
        StatementExecuted,
        StatementClosed,
        NonQuery,
        UsageAdvisorWarning,
        Warning,
        Error,
        QueryNormalized
    }

    public enum UsageAdvisorWarningFlags
    {
        NoIndex = 1,
        BadIndex,
        SkippedRows,
        SkippedColumns,
        FieldConversion
    }
}
