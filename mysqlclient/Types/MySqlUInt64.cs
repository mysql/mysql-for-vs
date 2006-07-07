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

	internal struct MySqlUInt64 : IMySqlValue
	{
		private ulong	mValue;
		private	bool	isNull;

		public MySqlUInt64(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0;
		}

		public MySqlUInt64(ulong val)
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
			get	{ return MySqlDbType.UInt64; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.UInt64; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public ulong Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(ulong); }
		}

		public string MySqlTypeName
		{
			get	{ return "BIGINT"; }
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			ulong v = Convert.ToUInt64( val );
			if (binary)
				writer.Write( BitConverter.GetBytes(v));
			else
				writer.WriteStringNoNull(v.ToString());		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlUInt64(true);

			if (length == -1) 
				return new MySqlUInt64((ulong)reader.ReadLong(8));
			else 
				return new MySqlUInt64(UInt16.Parse(reader.ReadString( length )));
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			reader.SkipBytes(8);
		}

		#endregion

        internal static void SetDSInfo(DataTable dsTable)
        {
            // we use name indexing because this method will only be called
            // when GetSchema is called for the DataSourceInformation 
            // collection and then it wil be cached.
            DataRow row = dsTable.NewRow();
            row["TypeName"] = "BIGINT";
            row["ProviderDbType"] = MySqlDbType.UInt64;
            row["ColumnSize"] = 0;
            row["CreateFormat"] = "BIGINT UNSIGNED";
            row["CreateParameters"] = null;
            row["DataType"] = "System.UInt64";
            row["IsAutoincrementable"] = true;
            row["IsBestMatch"] = true;
            row["IsCaseSensitive"] = false;
            row["IsFixedLength"] = true;
            row["IsFixedPrecisionScale"] = true;
            row["IsLong"] = false;
            row["IsNullable"] = true;
            row["IsSearchable"] = true;
            row["IsSearchableWithLike"] = false;
            row["IsUnsigned"] = true;
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
	/// Summary description for MySqlUInt64.
	/// </summary>
	internal class MySqlUInt64 : MySqlValue
	{
		private ulong	mValue;

		public MySqlUInt64() : base()
		{
			dbType = DbType.UInt64;
			mySqlDbType = MySqlDbType.Int64;
		}

		internal override void Serialize(PacketWriter writer, bool binary, object value, int length)
		{
			ulong v = Convert.ToUInt64( value );
			if (binary)
				writer.Write( BitConverter.GetBytes( v ) );
			else
				writer.WriteStringNoNull( v.ToString() );
		}

		public ulong Value
		{
			get { return mValue; }
			set { mValue = value; objectValue = value; }
		}

		internal override Type SystemType
		{
			get { return typeof(UInt64); }
		}

		internal override string GetMySqlTypeName()
		{
			return "BIGINT";
		}

		internal override MySqlValue ReadValue( PacketReader reader, long length )
		{
			if (length == -1) 
			{
				Value = reader.ReadLong( 8 );
			}
			else 
			{
				string value = reader.ReadString( length );
				Value = UInt64.Parse( value );
			}
			return this;
		}

		internal override void Skip(PacketReader reader)
		{
			reader.Skip(8);
		}
	}*/
}
