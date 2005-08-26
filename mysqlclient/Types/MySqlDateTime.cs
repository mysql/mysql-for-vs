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
using System.IO;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{

	/// <summary>
	/// 
	/// </summary>
	public struct MySqlDateTime : IMySqlValue, IConvertible, IComparable
	{
		private	bool			isNull;
		private MySqlDbType		type;
		private	DateTime		comparingDate;
		private int				year, month, day, hour, minute, second;
		private static string	fullPattern;
		private static string	shortPattern;


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

			// we construct a date that is guaranteed not have zeros in the date part
			// we do this for comparison 
			DateTime d = DateTime.MinValue;
			d = d.AddYears(year+1).AddMonths(month+1).AddDays(day+1).AddHours(hour);
			d = d.AddMinutes(minute).AddSeconds(second);
			comparingDate = d;

			if (fullPattern == null)
				ComposePatterns();
		}

		internal MySqlDateTime(MySqlDbType type, bool isNull) : this(type, 0, 0, 0, 0, 0, 0)
		{
			this.isNull = isNull;
		}

		internal MySqlDateTime(MySqlDbType type, DateTime val) : this(type, 0, 0, 0, 0, 0, 0)
		{
			this.isNull = false;
			year = val.Year;
			month = val.Month;
			day = val.Day;
			hour = val.Hour;
			minute = val.Minute;
			second = val.Second;
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

		#endregion

		#region IMySqlValue Members

		/// <summary>
		/// Returns true if this datetime object has a null value
		/// </summary>
		public bool IsNull
		{
			get { return isNull; }
		}

		public MySql.Data.MySqlClient.MySqlDbType MySqlDbType
		{
			get	{ return type; }
		}

		public System.Data.DbType DbType
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

		public DateTime Value
		{
			get { return GetDateTime(); }
		}

		public Type SystemType
		{
			get	{ return typeof(DateTime); }
		}

		public string MySqlTypeName
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


		private void SerializeText(MySqlStreamWriter writer, DateTime value) 
		{
			string val = String.Empty;

			if (type == MySqlDbType.Timestamp && !writer.Version.isAtLeast(4,1,0))
				val = String.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}",
					value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second );
			else 
			{
				val = String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", value.Year, value.Month, 
					value.Day, value.Hour, value.Minute, value.Second );
			}
			writer.WriteStringNoNull( "'" + val + "'" );
		}

		void IMySqlValue.WriteValue(MySqlStreamWriter writer, bool binary, object val, int length)
		{
			if (val is MySqlDateTime)
				val = ((MySqlDateTime)val).GetDateTime();

			if (! (val is DateTime))
				throw new MySqlException( "Only DateTime objects can be serialized by MySqlDateTime" );

			DateTime dtValue = (DateTime)val;
			if (! binary)
			{
				SerializeText( writer, dtValue );
				return;
			}

			if (type == MySqlDbType.Timestamp)
				writer.WriteByte( 11 );
			else
				writer.WriteByte( 7 );

			writer.WriteInteger( dtValue.Year, 2 );
			writer.WriteByte( (byte)dtValue.Month );
			writer.WriteByte( (byte)dtValue.Day );
			if (type == MySqlDbType.Date) 
			{
				writer.WriteByte( 0 );
				writer.WriteByte( 0 );
				writer.WriteByte( 0 );
			}
			else 
			{
				writer.WriteByte( (byte)dtValue.Hour );
				writer.WriteByte( (byte)dtValue.Minute  );
				writer.WriteByte( (byte)dtValue.Second );
			}
			
			if (type == MySqlDbType.Timestamp)
				writer.WriteInteger( dtValue.Millisecond, 4 );
		}

		private MySqlDateTime Parse40Timestamp( string s ) 
		{
			string format = "yy";

			while (s.Length > pos) 
			{
				if (index == 0 && (s.Length == 8 || s.Length == 14) ) 
				{
					vals[index] = short.Parse( s.Substring(pos,4));
					pos += 4;
				}
				else 
				{
					vals[index] = short.Parse( s.Substring(pos,2));
					pos += 2;
				}
				index++;
			}

			return new MySqlDateTime(type, vals[0], vals[1], vals[2], vals[3], vals[4], vals[5]);			
		}

		private MySqlDateTime ParseMySql( string s, bool is41 ) 
		{
			if (type == MySqlDbType.Timestamp && ! is41)
				return Parse40Timestamp(s);

			string[] parts = s.Split( '-', ' ', ':' );
			
			int year = int.Parse( parts[0] );
			int month = int.Parse( parts[1] );
			int day = int.Parse( parts[2] );

			int hour = 0, minute = 0, second = 0;
			if (parts.Length > 3) 
			{
				hour = int.Parse( parts[3] );
				minute = int.Parse( parts[4] );
				second = int.Parse( parts[5] );
			}

			return new MySqlDateTime(type, year, month, day, hour, minute, second);
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStreamReader reader, long length, bool nullVal)
		{
			if (nullVal) return new MySqlDateTime(type,true);

			if (length >= 0) 
			{
				string value = reader.ReadString( length );
				return ParseMySql( value, reader.Version.isAtLeast(4,1,0) );
			}

			long bufLength = reader.ReadByte();

			int year = reader.ReadInteger(2);
			int month = reader.ReadByte();
			int day = reader.ReadByte();
			int hour = 0, minute = 0, second = 0;

			if (bufLength > 4) 
			{
				hour = reader.ReadByte();
				minute = reader.ReadByte();
				second = reader.ReadByte();
			}
		
			if (bufLength > 7)
				reader.ReadInteger(4);
		
			return new MySqlDateTime(type, year, month, day, hour, minute, second);
		}

		void IMySqlValue.SkipValue(MySqlStreamReader reader)
		{
			long len = reader.ReadByte();
			reader.SkipBytes((int)len);
		}

		#endregion

		/// <summary>Returns this value as a DateTime</summary>
		public DateTime GetDateTime()
		{
			if (! IsValidDateTime)
				throw new MySqlConversionException("Unable to convert MySQL date/time value to System.DateTime");			

			return new DateTime( year, month, day, hour, minute, second );
		}

		/// <summary>Returns a MySQL specific string representation of this value</summary>
		public override string ToString()
		{
			if (this.IsValidDateTime) 
			{
				DateTime d = new DateTime( year, month, day, hour, minute, second );
				return (type == MySqlDbType.Date) ? d.ToString("d") : d.ToString();
			}

			if (type == MySqlDbType.Date)
				return String.Format( shortPattern, year, month, day);

			if (hour >= 12)
				fullPattern = fullPattern.Replace("A", "P");
			return String.Format( fullPattern, year, month, day, hour, minute, second);
		}

		private void ComposePatterns() 
		{
			DateTime tempDT = new DateTime(1, 2, 3, 4, 5, 6);
			fullPattern = tempDT.ToString();
			fullPattern = fullPattern.Replace( "0001", "{0:0000}" );
			if (fullPattern.IndexOf("02") != -1)
				fullPattern = fullPattern.Replace( "02", "{1:00}" );
			else
				fullPattern = fullPattern.Replace("2", "{1}" );
			if (fullPattern.IndexOf("03") != -1)
				fullPattern = fullPattern.Replace( "03", "{2:00}" );
			else
				fullPattern = fullPattern.Replace("3", "{2}" );
			if (fullPattern.IndexOf("04") != -1)
				fullPattern = fullPattern.Replace( "04", "{3:00}" );
			else
				fullPattern = fullPattern.Replace("4", "{3}" );
			if (fullPattern.IndexOf("05") != -1)
				fullPattern = fullPattern.Replace( "05", "{4:00}" );
			else
				fullPattern = fullPattern.Replace("5", "{4}" );
			if (fullPattern.IndexOf("06") != -1)
				fullPattern = fullPattern.Replace( "06", "{5:00}" );
			else
				fullPattern = fullPattern.Replace("6", "{5}" );

			shortPattern = tempDT.ToString("d");
			shortPattern = shortPattern.Replace( "0001", "{0:0000}" );
			if (shortPattern.IndexOf("02") != -1)
				shortPattern = shortPattern.Replace( "02", "{1:00}" );
			else
				shortPattern = shortPattern.Replace("2", "{1}" );
			if (shortPattern.IndexOf("03") != -1)
				shortPattern = shortPattern.Replace( "03", "{2:00}" );
			else
				shortPattern = shortPattern.Replace("3", "{2}" );
		}


		/// <summary></summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static explicit operator DateTime( MySqlDateTime val ) 
		{
			if (! val.IsValidDateTime) return DateTime.MinValue;
			return val.GetDateTime();
		}

		private void ComputeTicks()
		{
			int[] daysInMonths = new int[12] { 31, 28, 31, 30, 31, 30, 31, 30, 31, 30, 31, 30 };

			if (DateTime.IsLeapYear( Year ))
				daysInMonths[1]++;
			
			
		}

		#region IConvertible Members

		ulong IConvertible.ToUInt64 (IFormatProvider provider)
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
			return new System.TypeCode ();
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
			MySqlDateTime other = (MySqlDateTime)obj;

			return comparingDate.CompareTo( other.comparingDate );
		}

		#endregion

	}
}
