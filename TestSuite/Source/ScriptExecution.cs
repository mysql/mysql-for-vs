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
using MbUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class ScriptExecution : BaseTest
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup ();
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), PRIMARY KEY(id))");
		}

        private int statementCount;
        private string statementTemplate1 = @"CREATE PROCEDURE `spTest{0}`() NOT DETERMINISTIC
					CONTAINS SQL SQL SECURITY DEFINER COMMENT '' 
					BEGIN
						SELECT 1,2,3;
					END{1}";
        [Test]
        public void ExecuteScriptWithProcedures()
        {
            if (version < new Version(5, 0)) return;

            statementCount = 0;
            string scriptText = String.Empty;
            for (int i=0; i < 10; i++)
            {
                scriptText += String.Format(statementTemplate1, i, "$$");
            }
            MySqlScript script = new MySqlScript(scriptText);
            script.QueryExecuted += new MySqlQueryExecutedEventHandler(ExecuteScriptWithProcedures_QueryExecuted);
            script.Connection = conn;
            script.Delimiter = "$$";
            int count = script.Execute();
            Assert.AreEqual(10, count);

            MySqlCommand cmd = new MySqlCommand(
                String.Format(@"SELECT COUNT(*) FROM information_schema.routines WHERE
                routine_schema = '{0}' AND routine_name LIKE 'spTest%'",
                database0), conn);
            Assert.AreEqual(10, cmd.ExecuteScalar());
        }

        void ExecuteScriptWithProcedures_QueryExecuted(object sender, QueryExecutedEventArgs e)
        {
            string stmt = String.Format(statementTemplate1, statementCount++, null);
            Assert.AreEqual(stmt, e.Query);
        }

        private string statementTemplate2 = @"INSERT INTO Test (id, name) VALUES ({0}, 'a "" na;me'){1}";
        [Test]
        public void ExecuteScriptWithInserts()
        {
            statementCount = 0;
            string scriptText = String.Empty;
            for (int i = 0; i < 10; i++)
            {
                scriptText += String.Format(statementTemplate2, i, ";");
            }
            MySqlScript script = new MySqlScript(scriptText);
            script.Connection = conn;
            script.QueryExecuted += new MySqlQueryExecutedEventHandler(ExecuteScriptWithInserts_QueryExecuted);
            int count = script.Execute();
            Assert.AreEqual(10, count);

            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Test", conn);
            Assert.AreEqual(10, cmd.ExecuteScalar());
        }

        void ExecuteScriptWithInserts_QueryExecuted(object sender, QueryExecutedEventArgs e)
        {
            string stmt = String.Format(statementTemplate2, statementCount++, null);
            Assert.AreEqual(stmt, e.Query);
        }
    }
}
