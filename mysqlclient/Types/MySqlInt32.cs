// Copyright (C) 2004-2006 MySQL AB
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
	internal struct MySqlInt32 : IMySqlValue
	{
		private int		mValue;
		private	bool	isNull;
		private bool	is24Bit;

		private MySqlInt32(MySqlDbType type) 
		{
			is24Bit = type == MySqlDbType.Int24 ? true : false;
			isNull = true;
			mValue = 0;
		}

		public MySqlInt32(MySqlDbType type, bool isNull) : this(type)
		{
			this.isNull = isNull;
		}

		public MySqlInt32(MySqlDbType type, int val) : this(type)
		{
			this.isNull = false;
			mValue = val;
		}

		#region IMySqlValue Members

		public bool IsNull
		{
			get { return isNull; }
		}

		public MySql.Data.MySqlClient.MySqlDbType MySqlDbType
		{
			get	{ return MySqlDbType.Int32; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.Int32; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public int Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(Int32); }
		}

		public string MySqlTypeName
		{
			get	{ return is24Bit ? "MEDIUMINT" : "INT"; }
		}

		void IMySqlValue.WriteValue(MySqlStream stream, bool binary, object val, int length)
		{
			int v = Convert.ToInt32(val);
			if (binary)
				stream.Write(BitConverter.GetBytes(v));
			else
				stream.WriteStringNoNull(v.ToString());		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStream stream, long length, bool nullVal)
		{
			if (nullVal) 
                return new MySqlInt32(MySqlDbType, true);

			if (length == -1) 
				return new MySqlInt32(MySqlDbType, stream.ReadInteger(4));
			else 
				return new MySqlInt32(MySqlDbType, Int32.Parse(
                    stream.ReadString(length )));
		}

		void IMySqlValue.SkipValue(MySqlStream stream)
		{
			stream.SkipBytes(4);
		}

		#endregion

        internal static void SetDSInfo(DataTable dsTable)
        {
            string[] types = new string[] { "INT", "YEAR", "MEDIUMINT" };
            MySqlDbType[] dbtype = new MySqlDbType[] { MySqlDbType.Int32, 
                MySqlDbType.Year, MySqlDbType.Int24 };

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
                row["DataType"] = "System.Int32";
                row["IsAutoincrementable"] = dbtype[x] == MySqlDbType.Year ? false : true;
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
}
