// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.DbObjects
{
  internal class Metadata
  {
    private static List<string> DataTypes;

    public static bool IsStringType(string dataType)
    {
      dataType = dataType.ToLowerInvariant();
      int index = dataType.IndexOf('(');
      if (index != -1)
        dataType = dataType.Substring(0, index);

      return dataType.IndexOf("char") != -1 ||
             dataType.IndexOf("text") != -1;
    }

    public static string[] GetDataTypes(bool includeParens)
    {
      if (DataTypes == null)
        PopulateArray();
      string[] dataTypes = DataTypes.ToArray();
      if (!includeParens)
      {
        for (int i = 0; i < dataTypes.Length; i++)
          dataTypes[i] = RemoveParens(dataTypes[i]);
      }
      return dataTypes;
    }

    private static string RemoveParens(string dataType)
    {
      int index = dataType.IndexOf('(');
      if (index != -1)
        dataType = dataType.Substring(0, index);
      return dataType;
    }

    private static void PopulateArray()
    {
      DataTypes = new List<string>();

      DataTypes.AddRange(new string[] {
            "bit(10)",
            "tinyint",
            "boolean",
            "smallint",
            "mediumint",
            "int",
            "serial",
            "float",
            "double",
            "decimal",
            "date",
            "datetime",
            "timestamp",
            "time",
            "year",
            "char(10)",
            "varchar(10)",
            "binary(10)",
            "varbinary(10)",
            "tinyblob",
            "tinytext",
            "blob",
            "text",
            "mediumblob",
            "mediumtext",
            "longblob",
            "longtext",
            "enum(x,y,z)",
            "set(x,y,z)"});
    }
  }
}
