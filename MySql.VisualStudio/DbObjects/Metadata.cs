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
