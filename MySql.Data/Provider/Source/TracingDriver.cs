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

namespace MySql.Data.MySqlClient
{
    internal class TracingDriver : Driver
    {
        private Stopwatch timer = new Stopwatch();
        private bool firstResult;
        private MySqlTraceQueryInfo qi;
        private ResultSet lastResult;

        public TracingDriver(MySqlConnectionStringBuilder settings)
            : base(settings)
        {
        }

        public override void SendQuery(MySqlPacket p)
        {
            qi = new MySqlTraceQueryInfo();
            qi.Server = Settings.Server;
            //TODO see if output buffering removes this ickyness
            string cmdText = Encoding.GetString(p.Buffer, 5, p.Length - 5);
            if (cmdText.Length > 300)
                cmdText = cmdText.Substring(0, 300);
            qi.CommandText = cmdText;
            qi.TimeOfQuery = DateTime.Now;
            base.SendQuery(p);
            firstResult = true;
            timer.Reset();
            timer.Start();
        }

        protected override int GetResult(ref int affectedRows, ref int insertedId)
        {
            int fieldCount = base.GetResult(ref affectedRows, ref insertedId);
            if (firstResult)
            {
                timer.Stop();
                qi.ExecutionTime = timer.Elapsed;
            }
            return fieldCount;
        }

        public override ResultSet NextResult(int statementId)
        {
            if (lastResult != null)
            {
                MySqlTraceResultInfo ri = new MySqlTraceResultInfo();
                ri.InsertedId = lastResult.InsertedId;
                ri.RowsRead = lastResult.TotalRows;
                ri.RowsSkipped = lastResult.SkippedRows;
                ri.RowsChanged = lastResult.AffectedRows;
                if (HasStatus(ServerStatusFlags.NoIndex))
                    ri.UAFlags |= UsageAdvisorFlags.NoIndex;
                if (HasStatus(ServerStatusFlags.BadIndex))
                    ri.UAFlags |= UsageAdvisorFlags.BadIndex;
                if (!AllFieldsAccessed(lastResult))
                {
                    ri.UAFlags |= UsageAdvisorFlags.PartialRowSet;
                    for (int i = 0; i < lastResult.Size; i++)
                        if (!lastResult.FieldRead(i))
                            ri.FieldsNotAccessed.Add(lastResult.Fields[i].ColumnName);
                }
                qi.Results.Add(ri);
            }

            lastResult = base.NextResult(statementId);
            firstResult = false;
            if (lastResult == null)
            {
                MySqlTrace.LogInformation(qi.ToString());
            }
            return lastResult;
        }

        public void ReportTypeConversion(string fieldName, MySqlDbType originalType, Type newType)
        {
        }

        private bool AllFieldsAccessed(ResultSet rs)
        {
            if (rs.Fields == null || rs.Fields.Length == 0) return true;

            for (int i = 0; i < rs.Fields.Length; i++)
                if (!rs.FieldRead(i)) return false;
            return true;
        }
    }
}
