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
using System.Data;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Types
{

	internal struct MySqlDecimal : IMySqlValue
	{
		private byte precision;
		private byte scale;
		private Decimal mValue;
		private bool isNull;

		public MySqlDecimal(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0;
			precision = scale = 0;
		}

		public MySqlDecimal(decimal val)
		{
			this.isNull = false;
			precision = scale = 0;
			mValue = val;
		}

		#region IMySqlValue Members

		public bool IsNull
		{
			get { return isNull; }
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get { return MySqlDbType.Decimal; }
		}

		public byte Precision
		{
			get { return precision; }
			set { precision = value; }
		}

		public byte Scale
		{
			get { return scale; }
			set { scale = value; }
		}


		DbType IMySqlValue.DbType
		{
			get { return DbType.Decimal; }
		}

		object IMySqlValue.Value
		{
			get { return mValue; }
		}

		public decimal Value
		{
			get { return mValue; }
		}

		Type IMySqlValue.SystemType
		{
			get { return typeof(decimal); }
		}

		string IMySqlValue.MySqlTypeName
		{
			get { return "DECIMAL"; }
		}

		void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object val, int length)
		{
			decimal v = (val is decimal) ? (decimal)val : Convert.ToDecimal(val);
			string valStr = v.ToString(CultureInfo.InvariantCulture);
			if (binary)
                packet.WriteLenString(valStr);
			else
                packet.WriteStringNoNull(valStr);
		}

        IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal)
				return new MySqlDecimal(true);

			if (length == -1)
			{
                string s = packet.ReadLenString();
				return new MySqlDecimal(Decimal.Parse(s,
					 CultureInfo.InvariantCulture));
			}
			else
			{
                string s = packet.ReadString(length);
				return new MySqlDecimal(Decimal.Parse(s,
					 CultureInfo.InvariantCulture));
			}
		}

        void IMySqlValue.SkipValue(MySqlPacket packet)
		{
            int len = packet.ReadFieldLength();
            packet.Position += len;
		}

		#endregion

		internal static void SetDSInfo(DataTable dsTable)
		{
			// we use name indexing because this method will only be called
			// when GetSchema is called for the DataSourceInformation 
			// collection and then it wil be cached.
			DataRow row = dsTable.NewRow();
			row["TypeName"] = "DECIMAL";
			row["ProviderDbType"] = MySqlDbType.NewDecimal;
			row["ColumnSize"] = 0;
			row["CreateFormat"] = "DECIMAL({0},{1})";
			row["CreateParameters"] = "precision,scale";
			row["DataType"] = "System.Decimal";
			row["IsAutoincrementable"] = false;
			row["IsBestMatch"] = true;
			row["IsCaseSensitive"] = false;
			row["IsFixedLength"] = true;
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
