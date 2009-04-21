// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
