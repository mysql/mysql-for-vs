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
using NUnit.Framework;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Diagnostics;

namespace MySql.Data.MySqlClient.Tests
{
    /// <summary>
    /// Summary description for BaseTest.
    /// </summary>
    public class BaseTest
    {
        protected MySqlConnection conn;
        protected MySqlConnection rootConn;
        protected string table;
        protected string csAdditions = String.Empty;
        protected static string host;
        protected static string user;
        protected static string password;
        protected static int port;
        protected static string pipeName;
        protected static string memoryName;
        protected static string rootUser;
        protected static string rootPassword;
        protected static string database0;
        protected static string database1;
        protected Version version;
        protected bool pooling;
        protected static int maxPacketSize;

        public BaseTest()
        {
			if (host == null)
				LoadStaticConfiguration();
        }

		protected virtual void LoadStaticConfiguration()
		{
			Debug.Assert(host == null);

			user = "test";
			password = "test";
			port = 3306;
			rootUser = "root";
			rootPassword = "";

			host = ConfigurationManager.AppSettings["host"];
			string strPort = ConfigurationManager.AppSettings["port"];
			pipeName = ConfigurationManager.AppSettings["pipename"];
			memoryName = ConfigurationManager.AppSettings["memory_name"];

			if (strPort != null)
				port = Int32.Parse(strPort);
			if (host == null)
				host = "localhost";
			if (pipeName == null)
				pipeName = "MYSQL";
			if (memoryName == null)
				memoryName = "MYSQL";

			// we don't use FileVersion because it's not available
			// on the compact framework
			if (database0 == null)
			{
				string fullname = Assembly.GetExecutingAssembly().FullName;
				string[] parts = fullname.Split(new char[] { '=' });
				string[] versionParts = parts[1].Split(new char[] { '.' });
				database0 = String.Format("db{0}{1}{2}-a", versionParts[0], versionParts[1], port - 3300);
				database1 = String.Format("db{0}{1}{2}-b", versionParts[0], versionParts[1], port - 3300);
			}
		}

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            // open up a root connection
            string connStr = String.Format("server={0};user id={1};password={2};database=mysql;" +
                "persist security info=true;pooling=false;", host, rootUser, rootPassword);
            connStr += GetConnectionInfo();
            rootConn = new MySqlConnection(connStr);
            rootConn.Open();
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            rootConn.Close();
        }

        #region Properties

        protected Version Version
        {
            get { return version; }
        }

        #endregion


        protected virtual string GetConnectionInfo()
        {
            return String.Format("protocol=sockets;port={0};use procedure bodies=false", port);
        }

        protected string GetConnectionStringBasic(bool includedb)
        {
            string connStr = String.Format("server={0};user id={1};password={2};" +
                 "persist security info=true;connection reset=true;allow user variables=true;", 
                 host, user, password);
            if (includedb)
                connStr += String.Format("database={0};", database0);
            if (!pooling)
                connStr += ";pooling=false;";
            connStr += GetConnectionInfo();
            return connStr;
        }

        protected string GetConnectionString(bool includedb)
        {
            string connStr = String.Format("{0};{1}", 
                GetConnectionStringBasic(includedb), csAdditions);
            return connStr;
        }

        protected string GetConnectionStringEx(string user, string pw, bool includedb)
        {
            string connStr = String.Format("server={0};user id={1};" +
                 "persist security info=true;{2}", host, user, csAdditions);
            if (includedb)
                connStr += String.Format("database={0};", database0);
            if (pw != null)
                connStr += String.Format("password={0};", pw);
            connStr += GetConnectionInfo();
            return connStr;
        }

        protected void Open()
        {
            try
            {
                string connString = GetConnectionString(true);
                conn = new MySqlConnection(connString);
                conn.Open();


                string ver = conn.ServerVersion;

                int x = 0;
                foreach (char c in ver)
                {
                    if (!Char.IsDigit(c) && c != '.')
                        break;
                    x++;
                }
                ver = ver.Substring(0, x);
                version = new Version(ver);
            }
            catch (Exception ex)
            {
#if !CF
                System.Diagnostics.Trace.WriteLine(ex.Message);
#endif
                throw;
            }
        }

        protected void Close()
        {
            try
            {
                // delete the table we created.
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                execSQL("DROP TABLE IF EXISTS Test");
                conn.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        protected void SetAccountPerms(bool includeProc)
        {
            // now allow our user to access them
            suExecSQL(String.Format(@"GRANT ALL ON `{0}`.* to 'test'@'localhost' 
				identified by 'test'", database0));
            suExecSQL(String.Format(@"GRANT SELECT ON `{0}`.* to 'test'@'localhost' 
				identified by 'test'", database1));
            if (Version.Major >= 5)
                suExecSQL(String.Format(@"GRANT EXECUTE ON `{0}`.* to 'test'@'localhost' 
				    identified by 'test'", database1));

            if (includeProc)
            {
                // now allow our user to access them
                suExecSQL(@"GRANT ALL ON mysql.proc to 'test'@'localhost' identified by 'test'");
            }
            
            suExecSQL("FLUSH PRIVILEGES");
        }

        [SetUp]
        public virtual void Setup()
        {
            try
            {
                // now create our databases
                suExecSQL(String.Format("DROP DATABASE IF EXISTS `{0}`; CREATE DATABASE `{0}`", database0));
                suExecSQL(String.Format("DROP DATABASE IF EXISTS `{0}`; CREATE DATABASE `{0}`", database1));

                SetAccountPerms(false);

                rootConn.ChangeDatabase(database0);

                Open();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TearDown]
        public virtual void Teardown()
        {
            suExecSQL(String.Format("DROP DATABASE IF EXISTS `{0}`", database0));
            suExecSQL(String.Format("DROP DATABASE IF EXISTS `{0}`", database1));
            Close();
        }

        protected void KillConnection(MySqlConnection c)
        {
            int threadId = c.ServerThread;
            MySqlCommand cmd = new MySqlCommand("KILL " + threadId, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            // the kill flag might need a little prodding to do its thing
            try
            {
                cmd.CommandText = "SELECT 1";
                cmd.Connection = c;
                cmd.ExecuteNonQuery();
            }
            catch (Exception) { }

            // now wait till the process dies
            bool processStillAlive = false;
            while (true)
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SHOW PROCESSLIST", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                    if (row["Id"].Equals(threadId))
                        processStillAlive = true;
                if (!processStillAlive) break;
                System.Threading.Thread.Sleep(500);
            }
        }

        protected void createTable(string sql, string engine)
        {
            if (Version >= new Version(4,1))
                sql += " ENGINE=" + engine;
            else
                sql += " TYPE=" + engine;
            execSQL(sql);
        }

        protected void suExecSQL(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, rootConn);
            cmd.ExecuteNonQuery();
        }

        protected void execSQL(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        protected IDataReader execReader(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            return cmd.ExecuteReader();
        }

        protected int CountProcesses()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SHOW PROCESSLIST", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt.Rows.Count;
        }

        protected bool TableExists(string tableName)
        {
            string[] restrictions = new string[4];
            restrictions[2] = tableName;
            DataTable dt = conn.GetSchema("Tables", restrictions);
            return dt.Rows.Count > 0;
        }

        protected DataTable FillTable(string sql)
        {
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
