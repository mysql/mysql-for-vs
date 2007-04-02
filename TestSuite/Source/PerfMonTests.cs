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

#if !MONO

using System;
using System.Data;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System.Diagnostics;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for StoredProcedure.
	/// </summary>
	[TestFixture]
	public class PerfMonTests : BaseTest
	{

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			csAdditions = ";pooling=false;use performance monitor=true;";
			Open();
			execSQL("DROP TABLE IF EXISTS Test; CREATE TABLE Test (id INT, name VARCHAR(100))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Close();
		}

        /// <summary>
        /// This test doesn't work from the CI setup currently
        /// </summary>
		[Test]
		[Category("5.0")]
        [Category("NotWorking")]
		public void ProcedureFromCache()
		{
			execSQL("DROP PROCEDURE IF EXISTS spTest");
			execSQL("CREATE PROCEDURE spTest(id int) BEGIN END");

			PerformanceCounter hardQuery = new PerformanceCounter(
				 ".NET Data Provider for MySQL", "HardProcedureQueries", true);
			PerformanceCounter softQuery = new PerformanceCounter(
				 ".NET Data Provider for MySQL", "SoftProcedureQueries", true);
			long hardCount = hardQuery.RawValue;
			long softCount = softQuery.RawValue;

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("?id", 1);
			cmd.ExecuteScalar();

			Assert.AreEqual(hardCount + 1, hardQuery.RawValue);
			Assert.AreEqual(softCount, softQuery.RawValue);
			hardCount = hardQuery.RawValue;

			MySqlCommand cmd2 = new MySqlCommand("spTest", conn);
			cmd2.CommandType = CommandType.StoredProcedure;
			cmd2.Parameters.AddWithValue("?id", 1);
			cmd2.ExecuteScalar();

			Assert.AreEqual(hardCount, hardQuery.RawValue);
			Assert.AreEqual(softCount + 1, softQuery.RawValue);
		}

	}
}

#endif
