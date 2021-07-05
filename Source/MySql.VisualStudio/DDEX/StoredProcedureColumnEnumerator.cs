// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data;
using MySql.Data.VisualStudio.DDEX;
using MySql.Data.VisualStudio.Common;
using MySql.Data.MySqlClient;

namespace MySql.Data.VisualStudio
{
  class StoredProcedureColumnEnumerator : DataObjectEnumerator
  {
    public override DataReader EnumerateObjects(string typeName, object[] items,
        object[] restrictions, string sort, object[] parameters)
    {
      DbConnection conn = (DbConnection)Connection.GetLockedProviderObject();
      try
      {
        string spName = String.Format("{0}.{1}", restrictions[1], restrictions[2]);
        DataTable schemaDataTable;

        if (conn.State != ConnectionState.Open)
        {
          conn.OpenWithDefaultTimeout();
        }

        DbCommand cmd = conn.CreateCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        SetCommandParameters(conn, cmd, restrictions);

        if (cmd.DoesStoredProcedureContainsSignals(restrictions))
        {
          schemaDataTable = cmd.GetSchemaDataTableFromStoredProcedureDefinition(restrictions);
        }

        else
        {
          schemaDataTable = cmd.GetSchemaDataTableFromStoredProcedureExecution(spName, restrictions);
        }

        if (schemaDataTable == null)
        {
          schemaDataTable = new DataTable();
        }

        return new AdoDotNetDataTableReader(schemaDataTable);
      }
      finally
      {
        Connection.UnlockProviderObject();
      }
    }

    private object GetDefaultValue(string dataType)
    {
      if (dataType == "VARCHAR" || dataType == "VARBINARY" ||
          dataType == "ENUM" || dataType == "SET" || dataType == "CHAR")
        return "";

      return 0;
    }

    /// <summary>
    /// This method set the parameters to the DbCommand object, which will be used by the stored procedure.
    /// The parameters are pulled from the "restrictions" object.
    /// </summary>
    /// <param name="conn">
    /// The DbConnection created previously in the parent method "EnumerateObjects"
    /// </param>
    /// <param name="cmd">
    /// The DbCommand created previously in the parent method "EnumerateObjects"
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    private void SetCommandParameters(DbConnection conn, DbCommand cmd, object[] restrictions)
    {
      string[] parmRest = new string[4];
      parmRest[0] = (string)restrictions[0];
      parmRest[1] = (string)restrictions[1];
      parmRest[2] = (string)restrictions[2];
      parmRest[3] = (string)restrictions[3];
      DataTable parmTable = conn.GetSchema("Procedure Parameters", parmRest);

      foreach (DataRow row in parmTable.Rows)
      {
        if (row["ORDINAL_POSITION"].Equals(0)) continue;

        DbParameter p = cmd.CreateParameter();
        p.ParameterName = row["PARAMETER_NAME"].ToString();
        p.Value = GetDefaultValue(row["DATA_TYPE"].ToString());
        switch (row["PARAMETER_MODE"].ToString())
        {
          case "IN":
            p.Direction = ParameterDirection.Input;
            break;
          case "OUT":
            p.Direction = ParameterDirection.Output;
            break;
          case "INOUT":
            p.Direction = ParameterDirection.InputOutput;
            break;
        }

        cmd.Parameters.Add(p);
      }
    }
  }
}
