// Copyright (C) 2004 MySQL AB
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


	internal struct MySqlInt16 : IMySqlValue
	{
		private short	mValue;
		private	bool	isNull;

		public MySqlInt16(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0;
		}

		public MySqlInt16(short val)
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
			get	{ return MySqlDbType.Int16; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.Int16; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public short Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(short); }
		}

		public string MySqlTypeName
		{
			get	{ return "SMALLINT"; }
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			int v = Convert.ToInt32( val );
			if (binary)
				writer.Write( BitConverter.GetBytes(v));
			else
				writer.WriteStringNoNull(v.ToString());		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlInt16(true);

			if (length == -1) 
				return new MySqlInt16((short)reader.ReadInteger(2));
			else 
				return new MySqlInt16(Int16.Parse(reader.ReadString( length )));
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			reader.SkipBytes(2);
		}

		#endregion

	}
/*
	/// <summary>
	/// Summary description for MySqlInt16.
	/// </summary>
	internal class MySqlInt16 : MySqlValue
	{
		private short	mValue;

		public MySqlInt16() : base()
		{
			dbType = DbType.Int16;
			mySqlDbType = MySqlDbType.Int16;
		}

		internal override void Serialize(PacketWriter writer, bool binary, object value, int length)
		{
			short v = Convert.ToInt16( value );
			if (binary)
				writer.Write( BitConverter.GetBytes( v ) );
			else
				writer.WriteStringNoNull( v.ToString() );
		}


		public short Value
		{
			get { return mValue; }
			set { mValue = value; objectValue = value; }
		}

		internal override Type SystemType
		{
			get { return typeof(Int16); }
		}

		internal override string GetMySqlTypeName()
		{
			return "SMALLINT";
		}

		internal override MySqlValue ReadValue(PacketReader reader, long length)
		{
			if (length == -1) 
			{
				Value = (short)reader.ReadInteger(2);
			}
			else 
			{
				string value = reader.ReadString( length );
				Value = Int16.Parse( value );
			}
			return this;
		}

		internal override void Skip(PacketReader reader)
		{
			reader.Skip( 2 );
		}
	}*/
}
