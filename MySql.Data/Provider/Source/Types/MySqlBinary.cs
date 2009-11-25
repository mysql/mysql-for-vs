// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Data;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{

	internal struct MySqlBinary : IMySqlValue
	{
		private MySqlDbType type;
		private byte[] mValue;
		private bool isNull;

		public MySqlBinary(MySqlDbType type, bool isNull)
		{
			this.type = type;
			this.isNull = isNull;
			mValue = null;
		}

		public MySqlBinary(MySqlDbType type, byte[] val)
		{
			this.type = type;
			this.isNull = false;
			mValue = val;
		}

		#region IMySqlValue Members

		public bool IsNull
		{
			get { return isNull; }
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get { return type; }
		}

		DbType IMySqlValue.DbType
		{
			get { return DbType.Binary; }
		}

		object IMySqlValue.Value
		{
			get { return mValue; }
		}

		public byte[] Value
		{
			get { return mValue; }
		}

		Type IMySqlValue.SystemType
		{
			get { return typeof(byte[]); }
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				switch (type)
				{
					case MySqlDbType.TinyBlob: return "TINY_BLOB";
					case MySqlDbType.MediumBlob: return "MEDIUM_BLOB";
					case MySqlDbType.LongBlob: return "LONG_BLOB";
					case MySqlDbType.Blob:
					default:
						return "BLOB";
				}
			}
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
            byte[] buffToWrite = (val as byte[]);
            if (buffToWrite == null)
            {
                char[] valAsChar = (val as Char[]);
                if (valAsChar != null)
                    buffToWrite = packet.Encoding.GetBytes(valAsChar);
                else
                {
                    string s = val.ToString();
                    if (length == 0)
                        length = s.Length;
                    else
                        s = s.Substring(0, length);
                    buffToWrite = packet.Encoding.GetBytes(s);
                }
            }

			// we assume zero length means write all of the value
			if (length == 0)
				length = buffToWrite.Length;

			if (buffToWrite == null)
				throw new MySqlException("Only byte arrays and strings can be serialized by MySqlBinary");

			if (binary)
			{
                packet.WriteLength(length);
                packet.Write(buffToWrite, 0, length);
			}
			else
			{
                if (packet.Version.isAtLeast(4, 1, 0))
                    packet.WriteStringNoNull("_binary "); 

                packet.WriteByte((byte)'\'');
				EscapeByteArray(buffToWrite, length, packet);
                packet.WriteByte((byte)'\'');
			}
		}

		private static void EscapeByteArray(byte[] bytes, int length, MySqlPacket packet)
		{
			for (int x = 0; x < length; x++)
			{
				byte b = bytes[x];
				if (b == '\0')
				{
                    packet.WriteByte((byte)'\\');
                    packet.WriteByte((byte)'0');
				}

				else if (b == '\\' || b == '\'' || b == '\"')
				{
                    packet.WriteByte((byte)'\\');
                    packet.WriteByte(b);
				}
				else
                    packet.WriteByte(b);
			}
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
            MySqlBinary b;
            if (nullVal)
                b = new MySqlBinary(type, true);
            else
            {
                if (length == -1)
                    length = (long)packet.ReadFieldLength();

                byte[] newBuff = new byte[length];
                packet.Read(newBuff, 0, (int)length);
                b = new MySqlBinary(type, newBuff);
            }
            return b;
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
            int len = packet.ReadFieldLength();
            packet.Position += len;
		}

		#endregion

		public static void SetDSInfo(DataTable dsTable)
		{
			string[] types = new string[] { "BLOB", "TINYBLOB", "MEDIUMBLOB", "LONGBLOB" };
			MySqlDbType[] dbtype = new MySqlDbType[] { MySqlDbType.Blob, 
                MySqlDbType.TinyBlob, MySqlDbType.MediumBlob, MySqlDbType.LongBlob };

			// we use name indexing because this method will only be called
			// when GetSchema is called for the DataSourceInformation 
			// collection and then it wil be cached.
			for (int x = 0; x < types.Length; x++)
			{
				DataRow row = dsTable.NewRow();
				row["TypeName"] = types[x];
				row["ProviderDbType"] = dbtype[x];
				row["ColumnSize"] = 0;
				row["CreateFormat"] = types[x];
				row["CreateParameters"] = null;
				row["DataType"] = "System.Byte[]";
				row["IsAutoincrementable"] = false;
				row["IsBestMatch"] = true;
				row["IsCaseSensitive"] = false;
				row["IsFixedLength"] = false;
				row["IsFixedPrecisionScale"] = true;
				row["IsLong"] = true;
				row["IsNullable"] = true;
				row["IsSearchable"] = true;
				row["IsSearchableWithLike"] = true;
				row["IsUnsigned"] = false;
				row["MaximumScale"] = 0;
				row["MinimumScale"] = 0;
				row["IsConcurrencyType"] = DBNull.Value;
				row["IsLiteralsSupported"] = false;
				row["LiteralPrefix"] = null;
				row["LiteralSuffix"] = null;
				row["NativeDataType"] = null;
				dsTable.Rows.Add(row);
			}
		}
	}
}
