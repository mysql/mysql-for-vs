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

using System;
using System.Data;
using System.IO;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using System.Text;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class MySqlHelperTests : BaseTest
	{
        protected override void Setup()
        {
            base.Setup();
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), PRIMARY KEY(id))");
        }

		/// <summary>
		/// Bug #11490  	certain incorrect queries trigger connection must be valid and open message
		/// </summary>
		[Test]
		public void Bug11490()
		{
            if (version < new Version(4, 1)) return;

            MySqlDataReader reader = null;

			try 
			{
                StringBuilder sb = new StringBuilder();
                for (int i=0; i < 254; i++)
                    sb.Append('a');
                string sql = "INSERT INTO test (name) VALUES ('" + sb.ToString() + "')";
				reader = MySqlHelper.ExecuteReader(this.GetConnectionString(true), sql);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null)
					reader.Close();
			}
		}

	}
}
