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
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture()]
	public class CultureTests : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), PRIMARY KEY(id))");
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}


		[Test]
		public void TestFloats() 
		{
			InternalTestFloats(false);
			if (! Is41 && ! Is50) return;
			InternalTestFloats(true);
		}

		private void InternalTestFloats(bool prepared)
		{
			CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo curUICulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo c = new CultureInfo("de-DE");
			Thread.CurrentThread.CurrentCulture = c;
			Thread.CurrentThread.CurrentUICulture = c;

			execSQL( "DROP TABLE IF EXISTS Test" );
			execSQL( "CREATE TABLE Test (fl FLOAT, db DOUBLE, dec1 DECIMAL(5,2))" );

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?fl, ?db, ?dec)", conn);
			cmd.Parameters.Add( "?fl", MySqlDbType.Float );
			cmd.Parameters.Add( "?db", MySqlDbType.Double );
			cmd.Parameters.Add( "?dec", MySqlDbType.Decimal );
			cmd.Parameters[0].Value = 2.3;
			cmd.Parameters[1].Value = 4.6;
			cmd.Parameters[2].Value = 23.82;
			if (prepared)
				cmd.Prepare();
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count );

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test";
				if (prepared) cmd.Prepare();
				reader = cmd.ExecuteReader();
				reader.Read();
				Assert.AreEqual( 2.3, reader.GetFloat(0) );
				Assert.AreEqual( 4.6, reader.GetDouble(1) );
				Assert.AreEqual( 23.82, reader.GetDecimal(2) );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
				Thread.CurrentThread.CurrentCulture = curCulture;
				Thread.CurrentThread.CurrentUICulture = curUICulture;
			}
		}

		/// <summary>
		/// Bug #8228  	turkish character set causing the error
		/// </summary>
		[Test]
		public void Turkish() 
		{
			CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo curUICulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo c = new CultureInfo("tr-TR");
			Thread.CurrentThread.CurrentCulture = c;
			Thread.CurrentThread.CurrentUICulture = c;

			try 
			{
				MySqlConnection newConn = new MySqlConnection(GetConnectionString(true));
				newConn.Open();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			Thread.CurrentThread.CurrentCulture = c;
			Thread.CurrentThread.CurrentUICulture = c;
		}
	}
}
