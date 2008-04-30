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

using System.Text;
using MySql.Data.Common;
using MySql.Data.Types;
using System.Globalization;
using System.Text.RegularExpressions;
using System;

namespace MySql.Data.MySqlClient
{
    internal enum ColumnFlags : int
    {
        NOT_NULL = 1,
        PRIMARY_KEY = 2,
        UNIQUE_KEY = 4,
        MULTIPLE_KEY = 8,
        BLOB = 16,
        UNSIGNED = 32,
        ZERO_FILL = 64,
        BINARY = 128,
        ENUM = 256,
        AUTO_INCREMENT = 512,
        TIMESTAMP = 1024,
        SET = 2048,
        NUMBER = 32768
    } ;

    /// <summary>
    /// Summary description for Field.
    /// </summary>
    internal class MySqlField
    {
        #region Fields

        // public fields
        public string CatalogName;
        public int ColumnLength;
        public string ColumnName;
        public string OriginalColumnName;
        public string TableName;
        public string RealTableName;
        public string DatabaseName;
        public Encoding Encoding;
        public int maxLength;

        // protected fields
        protected ColumnFlags colFlags;
        protected int charSetIndex;
        protected byte precision;
        protected byte scale;
        protected MySqlDbType mySqlDbType;
        protected DBVersion connVersion;
        protected MySqlConnection connection;
        protected bool binaryOk;

        #endregion

        public MySqlField(MySqlConnection connection)
        {
            this.connection = connection;
            connVersion = connection.driver.Version;
            maxLength = 1;
            binaryOk = true;
        }

        #region Properties

        public int CharacterSetIndex
        {
            get { return charSetIndex; }
            set { charSetIndex = value; }
        }

        public MySqlDbType Type
        {
            get { return mySqlDbType; }
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

        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; }
        }

        public ColumnFlags Flags
        {
            get { return colFlags; }
        }

        public bool IsAutoIncrement
        {
            get { return (colFlags & ColumnFlags.AUTO_INCREMENT) > 0; }
        }

        public bool IsNumeric
        {
            get { return (colFlags & ColumnFlags.NUMBER) > 0; }
        }

        public bool AllowsNull
        {
            get { return (colFlags & ColumnFlags.NOT_NULL) == 0; }
        }

        public bool IsUnique
        {
            get { return (colFlags & ColumnFlags.UNIQUE_KEY) > 0; }
        }

        public bool IsPrimaryKey
        {
            get { return (colFlags & ColumnFlags.PRIMARY_KEY) > 0; }
        }

		public bool IsBlob
		{
			get
			{
				return (mySqlDbType >= MySqlDbType.TinyBlob &&
				mySqlDbType <= MySqlDbType.Blob) ||
				(mySqlDbType >= MySqlDbType.TinyText &&
				mySqlDbType <= MySqlDbType.Text) ||
				(colFlags & ColumnFlags.BLOB) > 0;
			}
		}

        public bool IsBinary
        {
            get
            {
                if (connVersion.isAtLeast(4, 1, 0))
                    return binaryOk && (CharacterSetIndex == 63);
                return binaryOk && ((colFlags & ColumnFlags.BINARY) > 0);
            }
        }

        public bool IsUnsigned
        {
            get { return (colFlags & ColumnFlags.UNSIGNED) > 0; }
        }

		public bool IsTextField
		{
			get
			{
				return Type == MySqlDbType.VarString || Type == MySqlDbType.VarChar ||
					 (IsBlob && !IsBinary);
			}
		}

        #endregion

        public void SetTypeAndFlags(MySqlDbType type, ColumnFlags flags)
        {
            colFlags = flags;
            mySqlDbType = type;

            // if our type is an unsigned number, then we need
            // to bump it up into our unsigned types
            // we're trusting that the server is not going to set the UNSIGNED
            // flag unless we are a number
            if (IsUnsigned)
            {
                switch (type)
                {
                    case MySqlDbType.Byte:
                        mySqlDbType = MySqlDbType.UByte;
                        return;
                    case MySqlDbType.Int16:
                        mySqlDbType = MySqlDbType.UInt16;
                        return;
                    case MySqlDbType.Int24:
                        mySqlDbType = MySqlDbType.UInt24;
                        return;
                    case MySqlDbType.Int32:
                        mySqlDbType = MySqlDbType.UInt32;
                        return;
                    case MySqlDbType.Int64:
                        mySqlDbType = MySqlDbType.UInt64;
                        return;
                }
            }

            if (IsBlob)
            {
                // handle blob to UTF8 conversion if requested.  This is only activated
                // on binary blobs
                if (IsBinary && connection.Settings.TreatBlobsAsUTF8)
                {
                    bool convertBlob = false;
                    Regex includeRegex = connection.Settings.BlobAsUTF8IncludeRegex;
                    Regex excludeRegex = connection.Settings.BlobAsUTF8ExcludeRegex;
                    if (includeRegex != null && includeRegex.IsMatch(ColumnName))
                        convertBlob = true;
                    else if (includeRegex == null && excludeRegex != null &&
                        !excludeRegex.IsMatch(ColumnName))
                        convertBlob = true;

                    if (convertBlob)
                    {
                        binaryOk = false;
                        Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                        charSetIndex = -1;  // lets driver know we are in charge of encoding
                        maxLength = 4;
                    }
                }

                if (!IsBinary)
                {
                    if (type == MySqlDbType.TinyBlob)
                        mySqlDbType = MySqlDbType.TinyText;
                    else if (type == MySqlDbType.MediumBlob)
                        mySqlDbType = MySqlDbType.MediumText;
                    else if (type == MySqlDbType.Blob)
                        mySqlDbType = MySqlDbType.Text;
                    else if (type == MySqlDbType.LongBlob)
                        mySqlDbType = MySqlDbType.LongText;
                }
            }

            // now determine if we really should be binary
            if (connection.Settings.RespectBinaryFlags)
                CheckForExceptions();
            if (!IsBinary) return;

            if (connection.Settings.RespectBinaryFlags)
            {
                if (type == MySqlDbType.String)
                    mySqlDbType = MySqlDbType.Binary;
                else if (type == MySqlDbType.VarChar ||
                         type == MySqlDbType.VarString)
                    mySqlDbType = MySqlDbType.VarBinary;
            }
		}

        private void CheckForExceptions()
        {
			string colName = String.Empty;
			if (OriginalColumnName != null)
				colName = OriginalColumnName.ToLower(CultureInfo.InvariantCulture);
            if (colName.StartsWith("char("))
                binaryOk = false;
            else if (connection.IsExecutingBuggyQuery)
                binaryOk = false;
        }

        public IMySqlValue GetValueObject()
        {
            IMySqlValue v = GetIMySqlValue(Type);
            if (v is MySqlByte && ColumnLength == 1 && MaxLength == 1 && connection.Settings.TreatTinyAsBoolean)
            {
                MySqlByte b = (MySqlByte)v;
                b.TreatAsBoolean = true;
                v = b;
            }
            else if (Type == MySqlDbType.Binary && ColumnLength == 16)
            {
                MySqlBinary b = (MySqlBinary)v;
                b.IsGuid = true;
                v = b;
            }
            return v;
        }

        public static IMySqlValue GetIMySqlValue(MySqlDbType type)
        {
            switch (type)
            {
                case MySqlDbType.Byte:
                    return new MySqlByte();
                case MySqlDbType.UByte:
                    return new MySqlUByte();
                case MySqlDbType.Int16:
                    return new MySqlInt16();
                case MySqlDbType.UInt16:
                    return new MySqlUInt16();
                case MySqlDbType.Int24:
                case MySqlDbType.Int32:
                case MySqlDbType.Year:
                    return new MySqlInt32(type, true);
                case MySqlDbType.UInt24:
                case MySqlDbType.UInt32:
                    return new MySqlUInt32(type, true);
                case MySqlDbType.Bit:
                    return new MySqlBit();
                case MySqlDbType.Int64:
                    return new MySqlInt64();
                case MySqlDbType.UInt64:
                    return new MySqlUInt64();
                case MySqlDbType.Time:
                    return new MySqlTimeSpan();
                case MySqlDbType.Date:
                case MySqlDbType.DateTime:
                case MySqlDbType.Newdate:
                case MySqlDbType.Timestamp:
                    return new MySqlDateTime(type, true);
                case MySqlDbType.Decimal:
                case MySqlDbType.NewDecimal:
                    return new MySqlDecimal();
                case MySqlDbType.Float:
                    return new MySqlSingle();
                case MySqlDbType.Double:
                    return new MySqlDouble();
                case MySqlDbType.Set:
                case MySqlDbType.Enum:
                case MySqlDbType.String:
                case MySqlDbType.VarString:
                case MySqlDbType.VarChar:
                case MySqlDbType.Text:
                case MySqlDbType.TinyText:
                case MySqlDbType.MediumText:
                case MySqlDbType.LongText:
                case (MySqlDbType) Field_Type.NULL:
                    return new MySqlString(type, true);
                case MySqlDbType.Geometry:
                case MySqlDbType.Blob:
                case MySqlDbType.MediumBlob:
                case MySqlDbType.LongBlob:
                case MySqlDbType.TinyBlob:
                case MySqlDbType.Binary:
                case MySqlDbType.VarBinary:
                    return new MySqlBinary(type, true);
                default:
                    throw new MySqlException("Unknown data type");
            }
        }
    }
}
