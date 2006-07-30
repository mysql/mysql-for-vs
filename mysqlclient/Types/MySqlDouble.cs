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
using System.Globalization;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{

	internal struct MySqlDouble : IMySqlValue
	{
		private double	mValue;
		private	bool	isNull;

		public MySqlDouble(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0.0;
		}

		public MySqlDouble(double val)
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
			get	{ return MySqlDbType.Double; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.Double; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public double Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(double); }
		}

		public string MySqlTypeName
		{
			get	{ return "DOUBLE"; }
		}

		void IMySqlValue.WriteValue(MySqlStream stream, bool binary, object val, int length)
		{
			double v = Convert.ToDouble(val);
            if (binary)
                stream.Write(BitConverter.GetBytes(v));
            else
                stream.WriteStringNoNull(v.ToString(
                    CultureInfo.InvariantCulture));		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStream stream, long length, 
            bool nullVal)
		{
			if (nullVal) 
                return new MySqlDouble(true);

			if (length == -1) 
			{
				byte[] b = new byte[8];
				stream.Read(b, 0, 8);
				return new MySqlDouble(BitConverter.ToDouble(b, 0));
			}
			return new MySqlDouble(Double.Parse(stream.ReadString(length), 
                CultureInfo.InvariantCulture));
		}

		void IMySqlValue.SkipValue(MySqlStream stream)
		{
			stream.SkipBytes(8);
		}

		#endregion

        internal static void SetDSInfo(DataTable dsTable)
        {
            // we use name indexing because this method will only be called
            // when GetSchema is called for the DataSourceInformation 
            // collection and then it wil be cached.
            DataRow row = dsTable.NewRow();
            row["TypeName"] = "DOUBLE";
            row["ProviderDbType"] = MySqlDbType.Double;
            row["ColumnSize"] = 0;
            row["CreateFormat"] = "DOUBLE";
            row["CreateParameters"] = null;
            row["DataType"] = "System.Double";
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
