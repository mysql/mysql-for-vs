// Copyright � 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
	/// <summary>
	/// Summary description for MySqlUInt64.
	/// </summary>
	internal struct MySqlBit : IMySqlValue
	{
		private ulong mValue;
		private bool isNull;

		public MySqlBit(bool isnull)
		{
			mValue = 0;
			isNull = isnull;
		}

		public bool IsNull
		{
			get { return isNull; }
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get { return MySqlDbType.Bit; }
		}

		DbType IMySqlValue.DbType
		{
			get { return DbType.UInt64; }
		}

		object IMySqlValue.Value
		{
			get 
			{
				return mValue; 
			}
		}

		Type IMySqlValue.SystemType
		{
			get 
			{
				return typeof(UInt64); 
			}
		}

		string IMySqlValue.MySqlTypeName
		{
			get { return "BIT"; }
		}

		public void WriteValue(MySqlPacket packet, bool binary, object value, int length)
		{
            ulong v = (value is UInt64) ? (UInt64)value : Convert.ToUInt64(value);
			if (binary)
                packet.WriteInteger((long)v, 8);
			else
                packet.WriteStringNoNull(v.ToString());
		}

        public IMySqlValue ReadValue(MySqlPacket packet, long length, bool isNull)
		{
            this.isNull = isNull;
            if (isNull)
                return this;

			if (length == -1)
                length = packet.ReadFieldLength();

            mValue = (UInt64)packet.ReadBitValue((int)length);
			return this;
		}

		public void SkipValue(MySqlPacket packet)
		{
            int len = packet.ReadFieldLength();
            packet.Position += len;
		}

		public static void SetDSInfo(DataTable dsTable)
		{
			// we use name indexing because this method will only be called
			// when GetSchema is called for the DataSourceInformation 
			// collection and then it wil be cached.
			DataRow row = dsTable.NewRow();
			row["TypeName"] = "BIT";
			row["ProviderDbType"] = MySqlDbType.Bit;
			row["ColumnSize"] = 64;
			row["CreateFormat"] = "BIT";
			row["CreateParameters"] = null;
            row["DataType"] = typeof(UInt64).ToString();
			row["IsAutoincrementable"] = false;
			row["IsBestMatch"] = true;
			row["IsCaseSensitive"] = false;
			row["IsFixedLength"] = false;
			row["IsFixedPrecisionScale"] = true;
			row["IsLong"] = false;
			row["IsNullable"] = true;
			row["IsSearchable"] = true;
			row["IsSearchableWithLike"] = false;
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
