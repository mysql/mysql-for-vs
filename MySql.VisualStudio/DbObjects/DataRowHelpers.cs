using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.DbObjects
{
    internal static class DataRowHelpers
    {
        public static int GetValueAsInt32(DataRow row, string column)
        {
            if (row[column] == DBNull.Value) return 0;
            return Convert.ToInt32(row[column]);
        }

        public static ulong GetValueAsUInt64(DataRow row, string column)
        {
            if (row[column] == DBNull.Value) return 0;
            return Convert.ToUInt64(row[column]);
        }
    }
}
