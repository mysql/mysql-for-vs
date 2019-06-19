// Copyright (c) 2015, 2019, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Utility.Classes.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySql.Data.VisualStudio.DDEX
{
  /// <summary>
  /// Static class holding extension methods for the DbCommand command, to validate whether a stored procedure contains any "SIGNAL" commands,
  /// so they can be by-passed in order for the stored procedure to be executed properly without returning the error produced by the SIGNAL commands, using the other
  /// extension methods listed in this same class to get the stored procedure definition (and the "SELECT" statement - replacing the parameters with a static value when applies-),
  /// and to get the schema from the stored procedure.
  /// </summary>
  internal static class DbCommandExtensions
  {
    /// <summary>
    /// This method verifies whether the specified stored procedure contains any "SIGNAL" command in its definition, that can avoid getting the columns returned by the execution of the same.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand representing the stored procedure to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns true if the stored procedure definition contains any "SIGNAL" commands. Otherwise, returns false.
    /// </returns>
    public static bool DoesStoredProcedureContainsSignals(this DbCommand cmd, object[] restrictions)
    {
      return cmd != null && GetStoredProcedureDefinition(cmd, restrictions).Contains("SIGNAL");
    }

    /// <summary>
    /// This method creates a datatable containing the schema of the stored procedure definition, extracting the definition from the stored procedure itself
    /// in order to replace the parameters with a static value.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand representing the stored procedure to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a DataTable with the schema of the overriden stored procedure, or null in case of any errors.
    /// </returns>
    public static DataTable GetSchemaDataTableFromStoredProcedureDefinition(this DbCommand cmd, object[] restrictions)
    {
      if (cmd == null || restrictions == null)
        return null;

      string sProcSelectCommand = GetSelectStatementFromStoredProcedure(cmd, restrictions);

      return string.IsNullOrEmpty(sProcSelectCommand) ? null : GetSchemaDataTableFromQuery(cmd, sProcSelectCommand, restrictions);
    }

    /// <summary>
    /// This method gets the definition of the specified stored procedure.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand representing the stored procedure to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a string containing the stored procedure definition, or an empty string in case of any errors.
    /// </returns>
    public static string GetStoredProcedureDefinition(this DbCommand cmd, object[] restrictions)
    {
      if (cmd == null || restrictions == null)
        return string.Empty;

      try
      {
        if (cmd.Connection.State != ConnectionState.Open)
          cmd.Connection.Open();

        string sProcDefinition = string.Empty;
        var schemaName = restrictions.Length > 1 ? restrictions[1] : string.Empty;
        var routineName = restrictions.Length > 2 ? restrictions[2] : string.Empty;
        CommandType originalCommandType = cmd.CommandType;
        cmd.CommandText = string.Format(@"SELECT ROUTINE_DEFINITION FROM information_schema.routines WHERE ROUTINE_SCHEMA='{0}' AND ROUTINE_NAME='{1}';", schemaName, routineName);
        cmd.CommandType = CommandType.Text;

        using (IDataReader reader = cmd.ExecuteReader())
        {
          if (reader.Read())
          {
            sProcDefinition = reader.GetString(0);
          }
        }

        cmd.CommandType = originalCommandType;
        return sProcDefinition;
      }
      catch (Exception ex)
      {
        string errorMessage = string.Format("{0}{1}", ex.Message, ex.InnerException != null ? string.Format(". {0}", ex.InnerException.Message) : string.Empty);
        Logger.LogError($"An error ocurred when trying to execute the reader. \n{errorMessage}.", true);
        return string.Empty;
      }
    }

    /// <summary>
    /// This method gets the "SELECT" statement from a stored procedure definition, replacing any existing parameters with a static value ("1")
    /// so the column names can be properly populated.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand representing the stored procedure to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a string containing the "SELECT" statement of the stored procedure, or an empty string in case of any errors.
    /// </returns>
    public static string GetSelectStatementFromStoredProcedure(this DbCommand cmd, object[] restrictions)
    {
      if (cmd == null || restrictions == null)
        return string.Empty;

      string sProcDefinition = GetStoredProcedureDefinition(cmd, restrictions);
      if (string.IsNullOrEmpty(sProcDefinition) || !sProcDefinition.ToUpper().Contains("SELECT"))
      {
        return string.Empty;
      }

      string selectCommand = string.Empty;
      int startIndex = sProcDefinition.ToUpper().IndexOf("SELECT");
      int finalIndex = sProcDefinition.IndexOf(";", startIndex) + 1;
      selectCommand = sProcDefinition.Substring(startIndex, finalIndex - startIndex);
      selectCommand = OverrideStoredProcedureParameters(cmd, restrictions, selectCommand);
      return selectCommand;
    }

    /// <summary>
    /// This method is used to overwrite any parameter name contained in the "SELECT" statement with a static value ("1").
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand representing the stored procedure to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <param name="storedProcedureDefinition">
    /// A string containing the stored procedure definition.
    /// </param>
    /// <returns>
    /// Returns a string containing the "SELECT" statement of the stored procedure with the parameters names overriden with the static value "1",
    /// or the original stored procedure definition in case of failure.
    /// </returns>
    private static string OverrideStoredProcedureParameters(DbCommand cmd, object[] restrictions, string storedProcedureDefinition)
    {
      if (cmd == null || restrictions == null || string.IsNullOrEmpty(storedProcedureDefinition))
        return string.Empty;

      string[] parmRest = new string[4];
      parmRest[0] = (string)restrictions[0];
      parmRest[1] = (string)restrictions[1];
      parmRest[2] = (string)restrictions[2];
      parmRest[3] = (string)restrictions[3];
      DataTable parmTable = cmd.Connection.GetSchema("Procedure Parameters", parmRest);

      foreach (DataRow row in parmTable.Rows)
      {
        storedProcedureDefinition = storedProcedureDefinition.Replace(row["PARAMETER_NAME"].ToString(), "1");
      }

      return storedProcedureDefinition;
    }

    /// <summary>
    /// This method will get the a schema of a stored procedure, storing it in a predefined DataTable which holds a schema
    /// that matches with what Visual Studio is expecting in order to populate the column names properly.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand created previously in the parent method "EnumerateObjects".
    /// </param>
    /// <param name="spName">
    /// A string specifying the stored procedure name.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a DataTable with the schema of the specified stored procedure or command, or null in case of any errors.
    /// </returns>
    public static DataTable GetSchemaDataTableFromStoredProcedureExecution(this DbCommand cmd, string spName, object[] restrictions)
    {
      return GetSchemaDataTable(cmd, spName, CommandType.StoredProcedure, restrictions);
    }

    /// <summary>
    /// This method will get the a schema of a command (query), storing it in a predefined DataTable which holds a schema
    /// that matches with what Visual Studio is expecting in order to populate the column names properly.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand created previously in the parent method "EnumerateObjects".
    /// </param>
    /// <param name="query">
    /// A string containing the command query to be executed.
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a DataTable with the schema of the specified stored procedure or command, or null in case of any errors.
    /// </returns>
    public static DataTable GetSchemaDataTableFromQuery(this DbCommand cmd, string query, object[] restrictions)
    {
      return GetSchemaDataTable(cmd, query, CommandType.Text, restrictions);
    }

    /// <summary>
    /// This method will get the schema of the specified command (a query or a stored procedure), storing it in a predefined DataTable which holds a schema
    /// that matches with what Visual Studio is expecting in order to populate the column names properly.
    /// </summary>
    /// <param name="cmd">
    /// The DbCommand created previously in the parent method "EnumerateObjects".
    /// </param>
    /// <param name="commandText">
    /// A string specifying the command to be executed (whether a query or the stored procedure name).
    /// </param>
    /// <param name="commandType">
    /// CommandType specifying the type of the command to be executed (whether a query or a stored procedure).
    /// </param>
    /// <param name="restrictions">
    /// An array of objects containing information about the database name, the schema and the sProc name (among other information).
    /// </param>
    /// <returns>
    /// Returns a DataTable with the schema of the specified stored procedure or command, or null in case of any errors.
    /// </returns>
    private static DataTable GetSchemaDataTable(DbCommand cmd, string commandText, CommandType commandType, object[] restrictions)
    {
      if (cmd == null || string.IsNullOrEmpty(commandText))
        return null;

      try
      {
        if (cmd.Connection.State != ConnectionState.Open)
          cmd.Connection.Open();

        CommandType originalCommandType = cmd.CommandType;
        string originalCommandText = cmd.CommandText;
        cmd.CommandText = commandText;
        cmd.CommandType = commandType;

        using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
        {
          DataTable dataTable = new DataTable();
          DataTable schemaTable = reader.GetSchemaTable();
          if (schemaTable == null)
          {
            schemaTable = new DataTable();
          }
          else
          {
            dataTable.Locale = System.Globalization.CultureInfo.CurrentCulture;
            dataTable.Columns.Add("Database", typeof(string));
            dataTable.Columns.Add("Schema", typeof(string));
            dataTable.Columns.Add("StoredProcedure", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Ordinal", typeof(int));
            dataTable.Columns.Add("ProviderType", typeof(int));
            dataTable.Columns.Add("FrameworkType", typeof(Type));
            dataTable.Columns.Add("MaxLength", typeof(int));
            dataTable.Columns.Add("Precision", typeof(short));
            dataTable.Columns.Add("Scale", typeof(short));
            dataTable.Columns.Add("IsNullable", typeof(bool));

            if (schemaTable != null)
            {
              foreach (DataRow row in schemaTable.Rows)
              {
                dataTable.Rows.Add(
                  restrictions[0],
                  restrictions[1],
                  restrictions[2],
                  row["ColumnName"],
                  row["ColumnOrdinal"],
                  row["ProviderType"],
                  row["DataType"],
                  row["ColumnSize"],
                  row["NumericPrecision"],
                  row["NumericScale"],
                  row["AllowDBNull"]);
              }
            }
          }

          cmd.CommandText = originalCommandText;
          cmd.CommandType = originalCommandType;

          return dataTable;
        }
      }
      catch (Exception ex)
      {
        string errorMessage = String.Format("{0}{1}", ex.Message, ex.InnerException != null ? string.Format(". {0}", ex.InnerException.Message) : string.Empty);
        Logger.LogError($"An error ocurred when trying to execute the reader. \n{errorMessage}.", true);
        return null;
      }
    }
  }
}
