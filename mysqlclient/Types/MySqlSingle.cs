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

	internal struct MySqlSingle : IMySqlValue
	{
		private float	mValue;
		private	bool	isNull;

		public MySqlSingle(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0.0f;
		}

		public MySqlSingle(float val)
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
			get	{ return MySqlDbType.Float; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.Single; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public float Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(float); }
		}

		public string MySqlTypeName
		{
			get	{ return "FLOAT"; }
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			double v = Convert.ToSingle( val );
			if (binary)
				writer.Write( BitConverter.GetBytes(v));
			else
				writer.WriteStringNoNull(v.ToString(NumberFormat.MySql().NumberFormatInfo));		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlSingle(true);

			if (length == -1) 
			{
				byte[] b = new byte[4];
				reader.Read(b, 0, 4);
				return new MySqlSingle(BitConverter.ToSingle( b, 0 ));
			}
			return new MySqlSingle(Single.Parse(reader.ReadString(length), NumberFormat.MySql().NumberFormatInfo));
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			reader.SkipBytes(4);
		}

		#endregion

        internal static void SetDSInfo(DataTable dsTable)
        {
            // we use name indexing because this method will only be called
            // when GetSchema is called for the DataSourceInformation 
            // collection and then it wil be cached.
            DataRow row = dsTable.NewRow();
            row["TypeName"] = "FLOAT";
            row["ProviderDbType"] = MySqlDbType.Float;
            row["ColumnSize"] = 0;
            row["CreateFormat"] = "FLOAT";
            row["CreateParameters"] = null;
            row["DataType"] = "System.Single";
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
	/// Summary description for MySqlFloat.
	/// </summary>
	internal class MySqlFloat : MySqlValue
	{
		private Single	mValue;

		public MySqlFloat() : base()
		{
			dbType = DbType.Single;
			mySqlDbType = MySqlDbType.Float;
		}

		internal override void Serialize(PacketWriter writer, bool binary, object value, int length)
		{
			Single v = Convert.ToSingle( value );
			if (binary)
				writer.Write( BitConverter.GetBytes( v ) );
			else
				writer.WriteStringNoNull( v.ToString(numberFormat) );
		}

		public Single Value
		{
			get { return mValue; }
			set { mValue = value; objectValue = value; }
		}

		public static float MaxValue 
		{
			get { return float.Parse(float.MaxValue.ToString("R")); }
		}

		public static float MinValue 
		{
			get { return float.Parse(float.MinValue.ToString("R")); }
		}

		internal override Type SystemType
		{
			get { return typeof(Single); }
		}

		internal override string GetMySqlTypeName()
		{
			return "FLOAT";
		}

		internal override MySqlValue ReadValue(PacketReader reader, long length)
		{
			if (length == -1) 
			{
				byte[] b = new byte[4];
				reader.Read( ref b, 0, 4 );
				Value = BitConverter.ToSingle( b, 0 );
			}
			else 
			{
				string value = reader.ReadString( length );
				Value = Parse(value);
			}
			return this;
		}

		internal override void Skip(PacketReader reader)
		{
			reader.Skip(4);
		}

<<<<<<< .working
	}*/
}
