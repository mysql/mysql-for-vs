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
using System.Text;
using MySql.Data.Types;
using System.Diagnostics;
using System.Collections.Generic;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    internal class TracingDriver : Driver
    {
        private Dictionary<int, ResultSet> activeResults = new Dictionary<int, ResultSet>();

        public TracingDriver(MySqlConnectionStringBuilder settings)
            : base(settings)
        {
        }

        private TraceSource Source
        {
            get { return MySqlTrace.Source; }
        }

        public override void Open()
        {
            base.Open();
            Source.TraceEvent(TraceEventType.Information, ThreadID, 
                Resources.TraceOpenConnection, MySqlTraceEventType.ConnectionOpened, Settings.ConnectionString);
        }

        public override void Close()
        {
            base.Close();
            Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceCloseConnection,
                MySqlTraceEventType.ConnectionClosed);
        }

        public override void SendQuery(MySqlPacket p)
        {
            string cmdText = Encoding.GetString(p.Buffer, 5, p.Length - 5);
            if (cmdText.Length > 300)
                cmdText = cmdText.Substring(0, 300);

            base.SendQuery(p);

            Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceQueryText,
                MySqlTraceEventType.QueryOpened, 0, cmdText);
        }

        protected override int GetResult(int statementId, ref int affectedRows, ref int insertedId)
        {
            int fieldCount = base.GetResult(statementId, ref affectedRows, ref insertedId);

            Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceResult,
                    MySqlTraceEventType.ResultOpened, statementId, fieldCount, affectedRows, insertedId);

            ReportUsageAdvisorWarnings(statementId, null);

            return fieldCount;
        }

        public override ResultSet NextResult(int statementId)
        {
            // first let's see if we already have a resultset on this statementId
            ResultSet oldRS = null;
            if (activeResults.ContainsKey(statementId))
            {
                oldRS = activeResults[statementId];
                if (Settings.UseUsageAdvisor)
                    ReportUsageAdvisorWarnings(statementId, oldRS);
                Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceResultClosed,
                    MySqlTraceEventType.ResultClosed, statementId, oldRS.TotalRows, oldRS.SkippedRows, 0);
                activeResults.Remove(statementId);
            }

            ResultSet rs = base.NextResult(statementId);
            if (rs != null)
            {
                activeResults[statementId] = rs;
                return rs;
            }
            if (oldRS != null)
                Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceQueryDone,
                    MySqlTraceEventType.QueryClosed, statementId);
            return null;
        }

        public override int PrepareStatement(string sql, ref MySqlField[] parameters)
        {
            int statementId = base.PrepareStatement(sql, ref parameters);
            return statementId;
        }

        public override void CloseStatement(int id)
        {
            base.CloseStatement(id);
        }

        public override void SetDatabase(string dbName)
        {
            base.SetDatabase(dbName);
            Source.TraceEvent(TraceEventType.Information, ThreadID, Resources.TraceSetDatabase,
                MySqlTraceEventType.NonQuery, -1, dbName);
        }

        public override void ExecuteStatement(MySqlPacket packetToExecute)
        {
            base.ExecuteStatement(packetToExecute);
        }

        public override bool FetchDataRow(int statementId, int columns)
        {
            bool b = base.FetchDataRow(statementId, columns);
            return b;
        }

        private bool AllFieldsAccessed(ResultSet rs)
        {
            if (rs.Fields == null || rs.Fields.Length == 0) return true;

            for (int i = 0; i < rs.Fields.Length; i++)
                if (!rs.FieldRead(i)) return false;
            return true;
        }

        private void ReportUsageAdvisorWarnings(int statementId, ResultSet rs)
        {
            if (!Settings.UseUsageAdvisor) return;
            if (rs == null)
            {
                if (HasStatus(ServerStatusFlags.NoIndex))
                    Source.TraceEvent(TraceEventType.Warning, ThreadID, Resources.TraceUAWarningNoIndex,
                            MySqlTraceEventType.UsageAdvisorWarning, statementId, UsageAdvisorWarningFlags.NoIndex);
                else if (HasStatus(ServerStatusFlags.BadIndex))
                    Source.TraceEvent(TraceEventType.Warning, ThreadID, Resources.TraceUAWarningBadIndex,
                            MySqlTraceEventType.UsageAdvisorWarning, statementId, UsageAdvisorWarningFlags.BadIndex);
            }
            else
            {
                // report abandoned rows
                if (rs.SkippedRows > 0)
                    Source.TraceEvent(TraceEventType.Warning, ThreadID, Resources.TraceUAWarningSkippedRows,
                            MySqlTraceEventType.UsageAdvisorWarning, statementId, UsageAdvisorWarningFlags.SkippedRows, rs.SkippedRows);

                // report not all fields accessed
                if (!AllFieldsAccessed(rs))
                {
                    StringBuilder notAccessed = new StringBuilder("");
                    string delimiter = "";
                    for (int i = 0; i < rs.Size; i++)
                        if (!rs.FieldRead(i))
                        {
                            notAccessed.AppendFormat("{0}{1}", delimiter, rs.Fields[i].ColumnName);
                            delimiter = ",";
                        }
                    Source.TraceEvent(TraceEventType.Warning, ThreadID, Resources.TraceUAWarningSkippedColumns,
                            MySqlTraceEventType.UsageAdvisorWarning, statementId, UsageAdvisorWarningFlags.SkippedColumns, 
                            notAccessed.ToString());
                }

                // report type conversions if any
                if (rs.Fields != null)
                {
                    foreach (MySqlField f in rs.Fields)
                    {
                        StringBuilder s = new StringBuilder();
                        string delimiter = "";
                        foreach (Type t in f.TypeConversions)
                        {
                            s.AppendFormat("{0}{1}", delimiter, t.Name);
                            delimiter = ",";
                        }
                        if (s.Length > 0)
                            Source.TraceEvent(TraceEventType.Warning, ThreadID, Resources.TraceUAWarningFieldConversion,
                                MySqlTraceEventType.UsageAdvisorWarning, statementId, UsageAdvisorWarningFlags.FieldConversion,
                                f.ColumnName, s.ToString());
                    }
                }
            }
        }
    }
}
