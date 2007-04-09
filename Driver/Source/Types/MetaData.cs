// Copyright (C) 2004-2007 MySQL AB
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
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Types
{
	internal class MetaData
	{
        public static bool IsNumericType(string typename)
        {
            string lowerType = typename.ToLower(CultureInfo.InvariantCulture);
            switch (lowerType)
            {
                case "int":
                case "integer":
                case "numeric":
                case "decimal":
                case "dec":
                case "fixed":
                case "tinyint":
                case "mediumint":
                case "bigint":
                case "real":
                case "double":
                case "float":
                case "serial":
                case "smallint": return true;
            }
            return false;
        }

		public static MySqlDbType NameToType(string typeName, bool unsigned,
			 bool realAsFloat, MySqlConnection connection)
		{
			switch (typeName)
			{
				case "char": return MySqlDbType.String;
				case "varchar": return MySqlDbType.VarChar;
				case "date": return MySqlDbType.Date;
				case "datetime": return MySqlDbType.Datetime;
				case "numeric":
				case "decimal":
				case "dec":
				case "fixed":
					if (connection.driver.Version.isAtLeast(5, 0, 3))
						return MySqlDbType.NewDecimal;
					else
						return MySqlDbType.Decimal;
				case "year":
					return MySqlDbType.Year;
				case "time":
					return MySqlDbType.Time;
				case "timestamp":
					return MySqlDbType.Timestamp;
				case "set": return MySqlDbType.Set;
				case "enum": return MySqlDbType.Enum;
				case "bit": return MySqlDbType.Bit;

				case "tinyint":
				case "bool":
				case "boolean":
					return MySqlDbType.Byte;
				case "smallint":
					return unsigned ? MySqlDbType.UInt16 : MySqlDbType.Int16;
				case "mediumint":
					return unsigned ? MySqlDbType.UInt24 : MySqlDbType.Int24;
				case "int":
				case "integer":
					return unsigned ? MySqlDbType.UInt32 : MySqlDbType.Int32;
				case "serial":
					return MySqlDbType.UInt64;
				case "bigint":
					return unsigned ? MySqlDbType.UInt64 : MySqlDbType.Int64;
				case "float": return MySqlDbType.Float;
				case "double": return MySqlDbType.Double;
				case "real": return
					 realAsFloat ? MySqlDbType.Float : MySqlDbType.Double;
                case "text":
                    return MySqlDbType.Text;
                case "blob":
					return MySqlDbType.Blob;
				case "longblob":
                    return MySqlDbType.LongBlob;
                case "longtext":
                    return MySqlDbType.LongText;
				case "mediumblob":
                    return MySqlDbType.MediumBlob;
                case "mediumtext":
                    return MySqlDbType.MediumText;
				case "tinyblob":
                    return MySqlDbType.TinyBlob;
				case "tinytext":
					return MySqlDbType.TinyText;
                case "binary":
                    return MySqlDbType.Binary;
                case "varbinary":
                    return MySqlDbType.VarBinary;
			}
			throw new MySqlException("Unhandled type encountered");
		}

	}
}
