using System;
using System.Globalization;

namespace MySql.Data.Types
{
	/// <summary>
	/// Summary description for NumberFormat.
	/// </summary>
	internal class NumberFormat
	{
		private static	NumberFormat		mysql;
		private			NumberFormatInfo	numberFormatInfo;


		private NumberFormat()
		{
			numberFormatInfo = null;
		}

		public static NumberFormat MySql() 
		{
			if (mysql == null) 
			{
				mysql = new NumberFormat();
			}
			return mysql;
		}

		public NumberFormatInfo NumberFormatInfo 
		{
			get { return numberFormatInfo; }
		}
	}
}
