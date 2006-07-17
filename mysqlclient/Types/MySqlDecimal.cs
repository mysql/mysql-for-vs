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
using System.Globalization;

namespace MySql.Data.Types
{

	internal struct MySqlDecimal : IMySqlValue
	{
		private byte	precision;
		private byte	scale;
		private Decimal	mValue;
		private	bool	isNull;

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

		public MySql.Data.MySqlClient.MySqlDbType MySqlDbType
		{
			get	{ return MySqlDbType.Decimal; }
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


		public System.Data.DbType DbType
		{
			get	{ return DbType.Decimal; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public decimal Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(decimal); }
		}

		public string MySqlTypeName
		{
			get	{ return "DECIMAL"; }
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			decimal v = Convert.ToDecimal(val);
			if (binary)
                writer.WriteLenString(v.ToString(
                    CultureInfo.InvariantCulture));
			else
                writer.WriteStringNoNull(v.ToString(
                    CultureInfo.InvariantCulture));
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlDecimal(true);

            if (length == -1)
                return new MySqlDecimal(Decimal.Parse(reader.ReadLenString()));
            else
                return new MySqlDecimal(Decimal.Parse(reader.ReadString(),
                    CultureInfo.InvariantCulture));
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			long len = reader.GetFieldLength();
			reader.SkipBytes((int)len);
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
/*
	/// <summary>
	/// Summary description for MySqlDecimal.
	/// </summary>
	internal class MySqlDecimal : MySqlValue
	{
		private byte	precision;
		private byte	scale;
		private Decimal	mValue;

		public MySqlDecimal() : base()
		{
			dbType = DbType.Decimal;
			mySqlDbType = MySqlDbType.Decimal;
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

		internal override void Serialize(PacketWriter writer, bool binary, object value, int length)
		{
			Decimal v = Convert.ToDecimal( value );
			if (binary) 
			{
				writer.WriteLenString( v.ToString(numberFormat) );
			}
			else 
			{
				writer.WriteStringNoNull(v.ToString(numberFormat));
			}
		}


		public Decimal Value
		{
			get { return mValue; }
			set { mValue = value; objectValue = value; }
		}

		internal override Type SystemType
		{
			get { return typeof(Decimal); }
		}

		internal override string GetMySqlTypeName()
		{
			return "DECIMAL";
		}

		internal override MySqlValue ReadValue(PacketReader reader, long length)
		{
			if (length == -1) 
			{
				string value = reader.ReadLenString();
				Value = Decimal.Parse( value, numberFormat );
			}
			else 
			{
				string value = reader.ReadString( length );
				Value = Decimal.Parse( value, numberFormat );
			}
			return this;
		}

		internal override void Skip(PacketReader reader)
		{
			long len = reader.GetFieldLength();
			reader.Skip( len );
		}
		
	}*/
}
