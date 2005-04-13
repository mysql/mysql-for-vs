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

	internal struct MySqlUByte : IMySqlValue
	{
		private byte	mValue;
		private	bool	isNull;

		public MySqlUByte(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0;
		}

		public MySqlUByte(byte val)
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
			get	{ return MySqlDbType.UByte; }
		}

		public System.Data.DbType DbType
		{
			get	{ return DbType.Byte; }
		}

		object IMySqlValue.Value 
		{
			get { return mValue; }
		}

		public byte Value
		{
			get { return mValue; }
		}

		public Type SystemType
		{
			get	{ return typeof(byte); }
		}

		public string MySqlTypeName
		{
			get	{ return "TINYINT"; }
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			sbyte v = ((IConvertible)val).ToSByte(null); 
			if (binary)
				writer.Write( BitConverter.GetBytes(v));
			else
				writer.WriteStringNoNull(v.ToString());		
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlUByte(true);

			if (length == -1) 
				return new MySqlUByte((byte)reader.ReadByte());
			else 
				return new MySqlUByte(Byte.Parse(reader.ReadString(length)));
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			reader.ReadByte();
		}

		#endregion

	}
/*
	/// <summary>
	/// Summary description for MySqlByte.
	/// </summary>
	internal class MySqlUByte : MySqlValue
	{
		private byte	mValue;

		public MySqlUByte() : base()
		{
			dbType = DbType.Byte;
			mySqlDbType = MySqlDbType.Byte;
		}

		internal override void Serialize(PacketWriter writer, bool binary, object value, int length)
		{	
			byte v = Convert.ToByte( value );
			if (binary)
				writer.WriteByte( v );
			else
				writer.WriteStringNoNull( v.ToString() );
		}

		public byte Value
		{
			get { return mValue; }
			set { mValue = value; objectValue = value; }
		}

		internal override Type SystemType
		{
			get { return typeof(Byte); }
		}

		internal override string GetMySqlTypeName()
		{ 
			if (mySqlDbType == MySqlDbType.Year)
				return "YEAR";
			return "TINYINT"; 
		}

		internal override MySqlValue ReadValue(PacketReader reader, long length)
		{
			if (length == -1)
				Value = (byte)reader.ReadByte();
			else
				Value = Byte.Parse( reader.ReadString(length));
			return this;
		}

		internal override void Skip(PacketReader reader)
		{
			reader.ReadByte();
		}
	}*/
}
