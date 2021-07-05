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

/*
 * This file contains implementation of data object enumerator.
 */
using System;
using System.Data;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Implements custom database objects enumerator for MySQL databases. 
  /// Uses information_schema database to enumerate objects.
  /// </summary>
  public class MySqlDataObjectEnumerator : AdoDotNetObjectEnumerator
  {
    /// <summary>
    /// Enumerates items for a set of data objects of the specified type 
    /// with the specified restrictions and sort string, if supported. 
    /// </summary>
    /// <param name="typeName">Name of the type of the object to enumerate.</param>
    /// <param name="items">
    /// The set of items to enumerate, specified as strings where named items are available, 
    /// otherwise as indexes. In cases in which a data provider does not support items 
    /// filtering, this parameter is ignored.
    /// NOT SUPPORTED.
    /// </param>
    /// <param name="restrictions">
    /// A set of filtering restrictions to apply to the set of returned objects.
    /// </param>
    /// <param name="sort">
    /// A sort string, which follows syntax for the SQL Server ORDER BY clause. 
    /// The actual sort order should be source-based; that is, if the client is 
    /// English and the source is Chinese, the sort should be applied in Chinese.
    /// NOT SUPPORTED.
    /// </param>
    /// <param name="parameters">
    /// An array whose contents are defined by the given implementation of 
    /// EnumerateObjects, and which is specified by the Data Object Support 
    /// XML file. Information supplied in this parameter can be used to provide 
    /// extra data indicating how to perform the enumeration, allowing 
    /// implementations of this method to be more data driven.
    /// NOT USED.
    /// </param>
    /// <returns>
    /// Returns a DataReader object containing the results of the enumeration call.
    /// </returns>
    public override DataReader EnumerateObjects(string typeName,
        object[] items, object[] restrictions, string sort, object[] parameters)
    {
      if (typeName == null)
      {
        throw new ArgumentNullException("typeName");
      }

      if (typeName == string.Empty)
      {
        return EnumerateRoot();
      }
      else
      {
        return EnumerateSchemaObjects(parameters[0] as string, restrictions);
      }
    }

    private DataReader EnumerateRoot()
    {
      DbConnection conn = (DbConnection)Connection.GetLockedProviderObject();
      try
      {
        DataTable table = new DataTable();
        table.Columns.Add("SERVER_NAME");
        table.Columns.Add("CATALOG_NAME");
        table.Columns.Add("SCHEMA_NAME");
        DataRow row = table.NewRow();
        row["SERVER_NAME"] = conn.DataSource;
        row["SCHEMA_NAME"] = conn.Database;
        table.Rows.Add(row);
        return new AdoDotNetDataTableReader(table);
      }
      finally
      {
        Connection.UnlockProviderObject();
      }
    }

    private DataReader EnumerateSchemaObjects(string typeName, object[] restrictions)
    {
      DbConnection conn = (DbConnection)Connection.GetLockedProviderObject();
      try
      {
        string[] rest;
        DataTable tables = null;

        if (restrictions != null)
        {
          int i = 0;
          rest = new string[restrictions.Length];
          foreach (object o in restrictions)
          {
            if (o is DBNull)
            {
              rest[i++] = "";
            }
            else
            {
              rest[i++] = (string)o;
            }
          }
          tables = conn.GetSchema(typeName, rest);
        }
        else
        {
          tables = conn.GetSchema(typeName);
        }

        foreach (DataRow row in tables.Rows)
        {
          row["TABLE_CATALOG"] = DBNull.Value;
        }

        return new AdoDotNetDataTableReader(tables);
      }
      finally
      {
        Connection.UnlockProviderObject();
      }
    }
  }
}
