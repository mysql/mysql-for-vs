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
        internal static TraceSource Source;
        public static bool Enabled;

        static MySqlTrace()
        {
            Source = new TraceSource("MySQL", SourceLevels.All);
            // check the app.config to see if we have any listeners registered for
            // the mysql source
            // if so then we have tracing enabled globally
        }

        public static TraceListenerCollection Listeners
        {
            get { return Source.Listeners; }
        }

        public static void LogWarning(string msg)
        {
            Source.TraceEvent(TraceEventType.Warning, 0, 
                String.Format("[{0}] - {1}", DateTime.Now, msg));
        }

        public static void LogError(string msg)
        {
            Source.TraceEvent(TraceEventType.Error, 0,
                String.Format("[{0}] - {1}", DateTime.Now, msg));
        }

        //public static void EnableEnterpriseMonitoring()
        //{
        //}

        //public static void DisableEnterpriseMonitoring()
        //{
        //}
    }

    internal enum UsageAdvisorFlags
    {
        NoIndex = 1,
        BadIndex = 2,
        PartialRowSet = 4
    }

    internal class MySqlTraceResultInfo
    {
        public int RowsRead;
        public int RowsChanged;
        public int RowsSkipped;
        public int InsertedId;
        public List<string> FieldsNotAccessed = new List<string>();
        public UsageAdvisorFlags UAFlags;

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("-- Result --");
            if (RowsChanged > -1)
                msg.AppendLine(String.Format("Rows affected: {0}", RowsChanged));
            if (InsertedId > -1)
                msg.AppendLine(String.Format("Inserted Id (if any): {0}", InsertedId));
            if (RowsRead > 0)
                msg.AppendLine(String.Format("Rows returned: {0}", RowsRead));
            if (UAFlags != 0)
            {
                if (RowsSkipped > 0)
                    msg.AppendLine(String.Format(
                        "UA Warning: not all rows were read.  Skipped {0} rows", RowsSkipped));
                if ((UAFlags & UsageAdvisorFlags.NoIndex) != 0)
                    msg.AppendLine("UA Warning: query did not use an index");
                if ((UAFlags & UsageAdvisorFlags.BadIndex) != 0)
                    msg.AppendLine("UA Warning: query used a bad index");
                if ((UAFlags & UsageAdvisorFlags.PartialRowSet) != 0)
                {
                    msg.Append("UA Warning: some fields not accessed (");
                    string delimiter = "";
                    foreach (string colName in FieldsNotAccessed)
                    {
                        msg.AppendFormat("{0}{1}", delimiter, colName);
                        delimiter = ", ";
                    }
                    msg.AppendLine(")");
                }
            }
            return msg.ToString();
        }
    }

    internal class MySqlTraceQueryInfo
    {
        public string CommandText;
        public string Server;
        public TimeSpan ExecutionTime;
        public DateTime TimeOfQuery;
        public List<MySqlTraceResultInfo> Results = new List<MySqlTraceResultInfo>();

        public override string ToString()
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine();
            msg.AppendLine("====== Query logged ======");
            msg.AppendLine(String.Format("Time of query: {0}", TimeOfQuery));
            msg.AppendLine(String.Format("host: {0}", Server));
            msg.AppendLine(String.Format("time of execution: {0} seconds ({1} milliseconds",
                ExecutionTime.TotalSeconds, ExecutionTime.TotalMilliseconds));

            foreach (MySqlTraceResultInfo ri in Results)
                msg.Append(ri.ToString());
            msg.AppendLine("Command text:");
            msg.AppendLine(CommandText);
            msg.AppendLine("====== End of Query ======");
            return msg.ToString();
        }
    }
}
