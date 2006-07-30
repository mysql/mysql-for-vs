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
	/// <summary>
	/// Summary description for MySqlUInt64.
	/// </summary>
	internal struct MySqlBit : IMySqlValue
	{
        private ulong mValue;
        private bool isNull;
        private byte[] buffer;

		public MySqlBit(bool isnull)
		{
            mValue = 0;
            isNull = isnull;
            buffer = new byte[8];
		}

        public bool IsNull
        {
            get { return isNull; }
        }

        public MySqlDbType MySqlDbType
        {
            get { return MySqlDbType.Bit; }
        }

        public DbType DbType
        {
            get { return DbType.UInt64; }
        }

        object IMySqlValue.Value
        {
            get { return mValue; }
        }

        Type IMySqlValue.SystemType
        {
            get { return typeof(UInt64); }
        }

        public string MySqlTypeName
        {
            get { return "BIT"; }
        }

        public void WriteValue(MySqlStream stream, bool binary, object value, int length)
        {
			ulong v = Convert.ToUInt64(value);
			if (binary)
				stream.Write(BitConverter.GetBytes(v));
			else
				stream.WriteStringNoNull(v.ToString());
        }

        public IMySqlValue ReadValue(MySqlStream stream, long length, bool isNull)
        {
            if (buffer == null)
                buffer = new byte[8];
			if (length == -1) 
			{
				length = stream.ReadFieldLength();
			}
			Array.Clear(buffer, 0, buffer.Length);
			for (long i=length-1; i >= 0; i--)
				buffer[i] = (byte)stream.ReadByte();
			mValue = BitConverter.ToUInt64(buffer, 0);
			return this;
        }

        public void SkipValue(MySqlStream stream)
        {
			long len = stream.ReadFieldLength();
            stream.SkipBytes((int)len);
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
            row["DataType"] = "UInt64";
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
