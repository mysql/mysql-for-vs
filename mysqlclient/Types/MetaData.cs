using System;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Types
{
    internal class MetaData
    {
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
					if (connection.driver.Version.isAtLeast(5,0,3))
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
				case "int" : 
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
				case "blob":
				case "text":
					return MySqlDbType.Blob;
				case "longblob":
				case "longtext":
					return MySqlDbType.LongBlob;
				case "mediumblob":
				case "mediumtext":
					return MySqlDbType.MediumBlob;
				case "tinyblob":
				case "tinytext":
					return MySqlDbType.TinyBlob;
			}
			throw new MySqlException("Unhandled type encountered");
		}

    }
}
