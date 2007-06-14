// Copyright (C) 2006-2007 MySQL AB
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

using System;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data;

namespace MySql.Data.VisualStudio
{
    class StoredProcedureColumnEnumerator : DataObjectEnumerator
    {
        public override DataReader EnumerateObjects(string typeName, object[] items, 
            object[] restrictions, string sort, object[] parameters)
        {
            DataConnectionWrapper connectionWrapper = new DataConnectionWrapper(Connection);
            string spName = String.Format("{0}.{1}", restrictions[1], restrictions[2]);

            DbConnection c = connectionWrapper.GetOpenConnection();
            try
            {
                string[] parmRest = new string[5];
                parmRest[1] = (string)restrictions[1];
                parmRest[2] = (string)restrictions[2];
                parmRest[3] = (string)restrictions[3];
                DataTable parmTable = c.GetSchema("Procedure Parameters", parmRest);

                DbCommand cmd = c.CreateCommand();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (DataRow row in parmTable.Rows)
                {
                    if (row["IS_RESULT"].Equals("YES")) continue;

                    DbParameter p = cmd.CreateParameter();
                    p.ParameterName = row["PARAMETER_NAME"].ToString();
                    p.Value = GetDefaultValue(row["DATA_TYPE"].ToString());
                    cmd.Parameters.Add(p);
                }

                using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    DataTable dt = reader.GetSchemaTable();

                    dt.Columns.Add(new DataColumn("RoutineName", typeof(string)));
                    foreach (DataRow row in dt.Rows)
                        row["RoutineName"] = restrictions[2];

                    return new AdoDotNetDataTableReader(dt);
                }
            }
            finally
            {
                connectionWrapper.ReleaseConnection();
            }
        }

        private object GetDefaultValue(string dataType)
        {
            if (dataType == "VARCHAR" || dataType == "VARBINARY" ||
                dataType == "ENUM" || dataType == "SET" || dataType == "CHAR")
                return "";
            return 0;
        }
    }
}
