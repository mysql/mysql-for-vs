// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Types
{

	/// <summary>
	/// 
	/// </summary>
	public struct MySqlDateTime : IMySqlValue, IConvertible, IComparable
	{
		private bool isNull;
		private MySqlDbType type;
		private int year, month, day, hour, minute, second;
		private int millisecond;

		/// <summary>
		/// Constructs a new <b>MySqlDateTime</b> object by setting the individual time properties to
		/// the given values.
		/// </summary>
		/// <param name="year">The year to use.</param>
		/// <param name="month">The month to use.</param>
		/// <param name="day">The day to use.</param>
		/// <param name="hour">The hour to use.</param>
		/// <param name="minute">The minute to use.</param>
		/// <param name="second">The second to use.</param>
		public MySqlDateTime(int year, int month, int day, int hour, int minute, int second)
			: this(MySqlDbType.DateTime, year, month, day, hour, minute, second)
		{
		}

		/// <summary>
		/// Constructs a new <b>MySqlDateTime</b> object by using values from the given <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="dt">The <see cref="DateTime"/> object to copy.</param>
		public MySqlDateTime(DateTime dt)
			: this(MySqlDbType.DateTime, dt)
		{
		}

		/// <summary>
		/// Constructs a new <b>MySqlDateTime</b> object by copying the current value of the given object.
		/// </summary>
		/// <param name="mdt">The <b>MySqlDateTime</b> object to copy.</param>
		public MySqlDateTime(MySqlDateTime mdt)
		{
			year = mdt.Year;
			month = mdt.Month;
			day = mdt.Day;
			hour = mdt.Hour;
			minute = mdt.Minute;
			second = mdt.Second;
			millisecond = 0;
			type = MySqlDbType.DateTime;
			isNull = false;
		}

		/// <summary>
		/// Enables the contruction of a <b>MySqlDateTime</b> object by parsing a string.
		/// </summary>
		public MySqlDateTime(string dateTime)
			: this(MySqlDateTime.Parse(dateTime))
		{
		}

		internal MySqlDateTime(MySqlDbType type, int year, int month, int day, int hour, int minute,
			int second)
		{
			this.isNull = false;
			this.type = type;
			this.year = year;
			this.month = month;
			this.day = day;
			this.hour = hour;
			this.minute = minute;
			this.second = second;
			this.millisecond = 0;
		}

		internal MySqlDateTime(MySqlDbType type, bool isNull)
			: this(type, 0, 0, 0, 0, 0, 0)
		{
			this.isNull = isNull;
		}

		internal MySqlDateTime(MySqlDbType type, DateTime val)
			: this(type, 0, 0, 0, 0, 0, 0)
		{
			this.isNull = false;
			year = val.Year;
			month = val.Month;
			day = val.Day;
			hour = val.Hour;
			minute = val.Minute;
			second = val.Second;
			millisecond = val.Millisecond;
		}

		#region Properties

		/// <summary>
		/// Indicates if this object contains a value that can be represented as a DateTime
		/// </summary>
		public bool IsValidDateTime
		{
			get
			{
				return year != 0 && month != 0 && day != 0;
			}
		}

		/// <summary>Returns the year portion of this datetime</summary>
		public int Year
		{
			get { return year; }
			set { year = value; }
		}

		/// <summary>Returns the month portion of this datetime</summary>
		public int Month
		{
			get { return month; }
			set { month = value; }
		}

		/// <summary>Returns the day portion of this datetime</summary>
		public int Day
		{
			get { return day; }
			set { day = value; }
		}

		/// <summary>Returns the hour portion of this datetime</summary>
		public int Hour
		{
			get { return hour; }
			set { hour = value; }
		}

		/// <summary>Returns the minute portion of this datetime</summary>
		public int Minute
		{
			get { return minute; }
			set { minute = value; }
		}

		/// <summary>Returns the second portion of this datetime</summary>
		public int Second
		{
			get { return second; }
			set { second = value; }
		}

		/// <summary>
		/// Retrieves the millisecond value of this object.
		/// </summary>
		public int Millisecond
		{
			get { return millisecond; }
			set { millisecond = value; }
		}

		#endregion

		#region IMySqlValue Members

		/// <summary>
		/// Returns true if this datetime object has a null value
		/// </summary>
		public bool IsNull
		{
			get { return isNull; }
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get { return type; }
		}

		DbType IMySqlValue.DbType
		{
			get
			{
				if (type == MySqlDbType.Date || type == MySqlDbType.Newdate)
					return DbType.Date;
				return DbType.DateTime;
			}
		}

		object IMySqlValue.Value
		{
			get { return GetDateTime(); }
		}

		/// <summary>
		/// Retrieves the value of this <see cref="MySqlDateTime"/> as a DateTime object.
		/// </summary>
		public DateTime Value
		{
			get { return GetDateTime(); }
		}

		Type IMySqlValue.SystemType
		{
			get { return typeof(DateTime); }
		}

		string IMySqlValue.MySqlTypeName
		{
			get
			{
				switch (type)
				{
					case MySqlDbType.Date: return "DATE";
					case MySqlDbType.Newdate: return "NEWDATE";
					case MySqlDbType.Timestamp: return "TIMESTAMP";
				}
				return "DATETIME";
			}
		}


		private void SerializeText(MySqlPacket packet, MySqlDateTime value)
		{
			string val = String.Empty;

			val = String.Format("{0:0000}-{1:00}-{2:00}",
                value.Year, value.Month, value.Day);
            if (type != MySqlDbType.Date)
                val = String.Format("{0} {1:00}:{2:00}:{3:00}", val,
                    value.Hour, value.Minute, value.Second);
            packet.WriteStringNoNull("'" + val + "'");
		}

        void IMySqlValue.WriteValue(MySqlPacket packet, bool binary, object value, int length)
		{
			MySqlDateTime dtValue;

            string valueAsString = value as string;

			if (value is DateTime)
				dtValue = new MySqlDateTime(type, (DateTime)value);
			else if (valueAsString != null)
				dtValue = new MySqlDateTime(type, DateTime.Parse(valueAsString, CultureInfo.CurrentCulture));
			else if (value is MySqlDateTime)
				dtValue = (MySqlDateTime)value;
			else
				throw new MySqlException("Unable to serialize date/time value.");

			if (!binary)
			{
                SerializeText(packet, dtValue);
				return;
			}

			if (type == MySqlDbType.Timestamp)
                packet.WriteByte(11);
			else
                packet.WriteByte(7);

            packet.WriteInteger(dtValue.Year, 2);
            packet.WriteByte((byte)dtValue.Month);
            packet.WriteByte((byte)dtValue.Day);
			if (type == MySqlDbType.Date)
			{
                packet.WriteByte(0);
                packet.WriteByte(0);
                packet.WriteByte(0);
			}
			else
			{
                packet.WriteByte((byte)dtValue.Hour);
                packet.WriteByte((byte)dtValue.Minute);
                packet.WriteByte((byte)dtValue.Second);
			}

			if (type == MySqlDbType.Timestamp)
                packet.WriteInteger(dtValue.Millisecond, 4);
		}

		static internal MySqlDateTime Parse(string s)
		{
			MySqlDateTime dt = new MySqlDateTime();
			return dt.ParseMySql(s);
		}

		static internal MySqlDateTime Parse(string s, Common.DBVersion version)
		{
			MySqlDateTime dt = new MySqlDateTime();
			return dt.ParseMySql(s);
		}

		private MySqlDateTime ParseMySql(string s)
		{
			string[] parts = s.Split('-', ' ', ':', '/');

			int year = int.Parse(parts[0]);
			int month = int.Parse(parts[1]);
			int day = int.Parse(parts[2]);

			int hour = 0, minute = 0, second = 0;
			if (parts.Length > 3)
			{
				hour = int.Parse(parts[3]);
				minute = int.Parse(parts[4]);
				second = int.Parse(parts[5]);
			}

			return new MySqlDateTime(type, year, month, day, hour, minute, second);
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlPacket packet, long length, bool nullVal)
		{
			if (nullVal) return new MySqlDateTime(type, true);

			if (length >= 0)
			{
                string value = packet.ReadString(length);
                return ParseMySql(value);
			}

            long bufLength = packet.ReadByte();
			int year = 0, month = 0, day = 0;
			int hour = 0, minute = 0, second = 0;

			if (bufLength >= 4)
			{
                year = packet.ReadInteger(2);
                month = packet.ReadByte();
                day = packet.ReadByte();
			}

			if (bufLength > 4)
			{
                hour = packet.ReadByte();
                minute = packet.ReadByte();
                second = packet.ReadByte();
			}

			if (bufLength > 7)
                packet.ReadInteger(4);

			return new MySqlDateTime(type, year, month, day, hour, minute, second);
		}

		void IMySqlValue.SkipValue(MySqlPacket packet)
		{
            int len = packet.ReadByte();
            packet.Position += len;
		}

		#endregion

		/// <summary>Returns this value as a DateTime</summary>
		public DateTime GetDateTime()
		{
			if (!IsValidDateTime)
				throw new MySqlConversionException("Unable to convert MySQL date/time value to System.DateTime");

			return new DateTime(year, month, day, hour, minute, second);
		}

        private static string FormatDateCustom(string format, int monthVal, int dayVal, int yearVal)
        {
            format = format.Replace("MM", "{0:00}");
            format = format.Replace("M", "{0}");
            format = format.Replace("dd", "{1:00}");
            format = format.Replace("d", "{1}");
            format = format.Replace("yyyy", "{2:0000}");
            format = format.Replace("yy", "{3:00}");
            format = format.Replace("y", "{4:0}");

            int year2digit = yearVal - ((yearVal / 1000) * 1000);
            year2digit -= ((year2digit / 100) * 100);
            int year1digit = year2digit - ((year2digit / 10) * 10);

            return String.Format(format, monthVal, dayVal, yearVal, year2digit, year1digit);
        }

        /// <summary>Returns a MySQL specific string representation of this value</summary>
        public override string ToString()
        {
            if (this.IsValidDateTime)
            {
                DateTime d = new DateTime(year, month, day, hour, minute, second);
                return (type == MySqlDbType.Date) ? d.ToString("d") : d.ToString();
            }

            string dateString = FormatDateCustom(
                CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern, month, day, year);
            if (type == MySqlDbType.Date)
                return dateString;

            DateTime dt = new DateTime(1, 2, 3, hour, minute, second);
            dateString = String.Format("{0} {1}", dateString, dt.ToLongTimeString());
            return dateString;
        }

		/// <summary></summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static explicit operator DateTime(MySqlDateTime val)
		{
			if (!val.IsValidDateTime) return DateTime.MinValue;
			return val.GetDateTime();
		}

		internal static void SetDSInfo(DataTable dsTable)
		{
			string[] types = new string[] { "DATE", "DATETIME", "TIMESTAMP" };
			MySqlDbType[] dbtype = new MySqlDbType[] { MySqlDbType.Date, 
				MySqlDbType.DateTime, MySqlDbType.Timestamp };

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
				row["DataType"] = "System.DateTime";
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
				row["IsLiteralSupported"] = false;
				row["LiteralPrefix"] = null;
				row["LiteralSuffix"] = null;
				row["NativeDataType"] = null;
				dsTable.Rows.Add(row);
			}
		}

		#region IConvertible Members

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return 0;
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			// TODO:  Add MySqlDateTime.ToSByte implementation
			return 0;
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return 0;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this.GetDateTime();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return 0;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return false;
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return 0;
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return 0;
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return 0;
		}

		string System.IConvertible.ToString(IFormatProvider provider)
		{
			return null;
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return 0;
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return '\0';
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return 0;
		}

		System.TypeCode IConvertible.GetTypeCode()
		{
			return new System.TypeCode();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return 0;
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return null;
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return 0;
		}

		#endregion

		#region IComparable Members

		int IComparable.CompareTo(object obj)
		{
			MySqlDateTime otherDate = (MySqlDateTime)obj;

			if (Year < otherDate.Year) return -1;
			else if (Year > otherDate.Year) return 1;

			if (Month < otherDate.Month) return -1;
			else if (Month > otherDate.Month) return 1;

			if (Day < otherDate.Day) return -1;
			else if (Day > otherDate.Day) return 1;

			if (Hour < otherDate.Hour) return -1;
			else if (Hour > otherDate.Hour) return 1;

			if (Minute < otherDate.Minute) return -1;
			else if (Minute > otherDate.Minute) return 1;

			if (Second < otherDate.Second) return -1;
			else if (Second > otherDate.Second) return 1;

			if (Millisecond < otherDate.Millisecond) return -1;
			else if (Millisecond > otherDate.Millisecond) return 1;

			return 0;
		}

		#endregion

	}
}
