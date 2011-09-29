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
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using NUnit.Framework;
using System.Configuration;
using System.Security;
using System.Security.Permissions;
using System.Net;

namespace MySql.Data.MySqlClient.Tests
{
  /// <summary>
  /// Summary description for ConnectionTests.
  /// </summary>
  [TestFixture]
  public class ConnectionTests : BaseTest
  {
    [Test]
    public void TestConnectionStrings()
    {
      MySqlConnection c = new MySqlConnection();

      // public properties
      Assert.AreEqual(15, c.ConnectionTimeout, "ConnectionTimeout");
      Assert.AreEqual("", c.Database, "Database");
      Assert.AreEqual(String.Empty, c.DataSource, "DataSource");
      Assert.AreEqual(false, c.UseCompression, "Use Compression");
      Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");

      c = new MySqlConnection("connection timeout=25; user id=myuser; " +
          "password=mypass; database=Test;server=myserver; use compression=true; " +
          "pooling=false;min pool size=5; max pool size=101");
      // public properties
      Assert.AreEqual(25, c.ConnectionTimeout, "ConnectionTimeout");
      Assert.AreEqual("Test", c.Database, "Database");
      Assert.AreEqual("myserver", c.DataSource, "DataSource");
      Assert.AreEqual(true, c.UseCompression, "Use Compression");
      Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");

      c.ConnectionString = "connection timeout=15; user id=newuser; " +
          "password=newpass; port=3308; database=mydb; data source=myserver2; " +
          "use compression=true; pooling=true; min pool size=3; max pool size=76";

      // public properties
      Assert.AreEqual(15, c.ConnectionTimeout, "ConnectionTimeout");
      Assert.AreEqual("mydb", c.Database, "Database");
      Assert.AreEqual("myserver2", c.DataSource, "DataSource");
      Assert.AreEqual(true, c.UseCompression, "Use Compression");
      Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");
    }

    [Test]
    public void TestConnectingSocketBadUserName()
    {
      suExecSQL("DELETE FROM mysql.user WHERE length(user) = 0");
      suExecSQL("FLUSH PRIVILEGES");

      string connStr = "server={0};user id=dummy;password=;database=Test;pooling=false";
      MySqlConnection c = new MySqlConnection(
          String.Format(connStr, host));
      try
      {
        c.Open();
        c.Close();
        throw new Exception("Open should not have worked");
      }
      catch (MySqlException)
      {
      }
    }

    [Test]
    public void TestConnectingSocketBadDbName()
    {
      string connStr = "server={0};user id={1};password={2};database=dummy; " +
          "pooling=false";
      MySqlConnection c = new MySqlConnection(
          String.Format(connStr, host, user, password));
      try
      {
        c.Open();
        c.Close();
        throw new Exception("Open should not have worked");
      }
      catch (MySqlException)
      {
      }
    }

    [Test]
    public void TestPersistSecurityInfoCachingPasswords()
    {
      string connStr = GetConnectionString(true);
      MySqlConnection c = new MySqlConnection(connStr);
      c.Open();
      c.Close();

      // this shouldn't work
      connStr = GetConnectionString(user, "bad_password", true);
      c = new MySqlConnection(connStr);
      try
      {
        c.Open();
        Assert.Fail("Thn is should not work");
        c.Close();
        return;
      }
      catch (MySqlException)
      {
      }

      // this should work
      connStr = GetConnectionString(true);
      c = new MySqlConnection(connStr);
      c.Open();
      c.Close();
    }

    [Test]
    public void ChangeDatabase()
    {
      string connStr = GetConnectionString(true);
      MySqlConnection c = new MySqlConnection(connStr + ";pooling=false");
      c.Open();
      Assert.IsTrue(c.State == ConnectionState.Open);

      Assert.AreEqual(database0.ToLower(), c.Database.ToLower());

      c.ChangeDatabase(database1);

      Assert.AreEqual(database1.ToLower(), c.Database.ToLower());

      c.Close();
    }

    [Test]
    public void ConnectionTimeout()
    {
      MySqlConnection c = new MySqlConnection(
          "server=1.1.1.1;user id=bogus;pwd=bogus;Connection timeout=5;" +
          "pooling=false");
      DateTime start = DateTime.Now;
      try
      {
        c.Open();
      }
      catch (Exception)
      {
        TimeSpan diff = DateTime.Now.Subtract(start);
        Assert.IsTrue(diff.TotalSeconds < 15, "Timeout exceeded");
      }
    }

    /*        [Test]
            public void AnonymousLogin()
            {
                suExecSQL(String.Format("GRANT ALL ON *.* to ''@'{0}' IDENTIFIED BY 'set_to_blank'", host));
                suExecSQL("UPDATE mysql.user SET password='' WHERE password='set_to_blank'");

                MySqlConnection c = new MySqlConnection(String.Empty);
                c.Open();
                c.Close();
            }
            */
    [Test]
    public void ConnectInVariousWays()
    {
      // connect with no db
      string connStr2 = GetConnectionString(false);
      MySqlConnection c = new MySqlConnection(connStr2);
      c.Open();
      c.Close();

      suExecSQL("GRANT ALL ON *.* to 'nopass'@'%'");
      suExecSQL("GRANT ALL ON *.* to 'nopass'@'localhost'");
      suExecSQL("FLUSH PRIVILEGES");

      // connect with no password
      connStr2 = GetConnectionString("nopass", null, false);
      c = new MySqlConnection(connStr2);
      c.Open();
      c.Close();

      connStr2 = GetConnectionString("nopass", "", false);
      c = new MySqlConnection(connStr2);
      c.Open();
      c.Close();
    }

    [Test]
    public void ConnectingAsUTF8()
    {
      if (Version < new Version(4, 1)) return;

      string connStr = GetConnectionString(true) + ";charset=utf8";
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        c.Open();

        MySqlCommand cmd = new MySqlCommand(
            "CREATE TABLE test (id varbinary(16), active bit) CHARACTER SET utf8", conn);
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x1234567890 AS Binary), true)";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x123456789a AS Binary), true)";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x123456789b AS Binary), true)";
        cmd.ExecuteNonQuery();
      }

      using (MySqlConnection d = new MySqlConnection(connStr))
      {
        d.Open();

        MySqlCommand cmd2 = new MySqlCommand("SELECT id, active FROM test", d);
        using (MySqlDataReader reader = cmd2.ExecuteReader())
        {
          Assert.IsTrue(reader.Read());
          Assert.IsTrue(reader.GetBoolean(1));
        }
      }
    }

    /// <summary>
    /// Bug #10281 Clone issue with MySqlConnection 
    /// Bug #27269 MySqlConnection.Clone does not mimic SqlConnection.Clone behaviour 
    /// </summary>
    [Test]
    public void TestConnectionClone()
    {
      MySqlConnection c = new MySqlConnection();
      MySqlConnection clone = (MySqlConnection)((ICloneable)c).Clone();
      clone.ToString();

      string connStr = GetConnectionString(true);
      connStr = connStr.Replace("persist security info=true", "persist security info=false");
      c = new MySqlConnection(connStr);
      c.Open();
      c.Close();
      MySqlConnection c2 = (MySqlConnection)((ICloneable)c).Clone();
      c2.Open();
      c2.Close();
    }

    /// <summary>
    /// Bug #13321  	Persist security info does not woek
    /// </summary>
    [Test]
    public void PersistSecurityInfo()
    {
      string s = GetConnectionString(true).ToLower();
      int start = s.IndexOf("persist security info");
      int end = s.IndexOf(";", start);
      string connStr = s.Substring(0, start);
      connStr += s.Substring(end, s.Length - (end));

      string p = "password";
      if (connStr.IndexOf("pwd") != -1)
        p = "pwd";
      else if (connStr.IndexOf("passwd") != -1)
        p = "passwd";

      string newConnStr = connStr + ";persist security info=true";
      MySqlConnection conn2 = new MySqlConnection(newConnStr);
      Assert.IsTrue(conn2.ConnectionString.IndexOf(p) != -1);
      conn2.Open();
      conn2.Close();
      Assert.IsTrue(conn2.ConnectionString.IndexOf(p) != -1);

      newConnStr = connStr + ";persist security info=false";
      conn2 = new MySqlConnection(newConnStr);
      Assert.IsTrue(conn2.ConnectionString.IndexOf(p) != -1);
      conn2.Open();
      conn2.Close();
      Assert.IsTrue(conn2.ConnectionString.IndexOf(p) == -1);
    }

    /// <summary>
    /// Bug #13658  	connection.state does not update on Ping()
    /// </summary>
    [Test]
    public void PingUpdatesState()
    {
      MySqlConnection conn2 = new MySqlConnection(GetConnectionString(true));
      conn2.Open();
      KillConnection(conn2);
      Assert.IsFalse(conn2.Ping());
      Assert.IsTrue(conn2.State == ConnectionState.Closed);
      conn2.Open();
      conn2.Close();
    }

    /// <summary>
    /// Bug #16659  	Can't use double quotation marks(") as password access server by Connector/NET
    /// </summary>
    [Test]
    public void ConnectWithQuotePassword()
    {
      suExecSQL("GRANT ALL ON *.* to 'quotedUser'@'%' IDENTIFIED BY '\"'");
      suExecSQL("GRANT ALL ON *.* to 'quotedUser'@'localhost' IDENTIFIED BY '\"'");
      string connStr = GetConnectionString("quotedUser", null, false);
      connStr += ";pwd='\"'";
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        c.Open();
      }
    }

    /// <summary>
    /// Bug #24802 Error Handling 
    /// </summary>
    [Test]
    public void TestConnectingSocketBadHostName()
    {
      string connStr = "server=foobar;user id=foouser;password=;database=Test;" +
          "pooling=false";
      try
      {
        using (MySqlConnection c = new MySqlConnection(connStr))
        {
          c.Open();
        }
      }
      catch (MySqlException ex)
      {
        Assert.AreEqual((int)MySqlErrorCode.UnableToConnectToHost, ex.Number);
      }
    }

    /// <summary>
    /// Bug #29123  	Connection String grows with each use resulting in OutOfMemoryException
    /// </summary>
    [Test]
    public void ConnectionStringNotAffectedByChangeDatabase()
    {
      for (int i = 0; i < 10; i++)
      {
        string connStr = GetConnectionString(true) + ";pooling=false";
        connStr = connStr.Replace("database", "Initial Catalog");
        connStr = connStr.Replace("persist security info=true",
            "persist security info=false");
        using (MySqlConnection c = new MySqlConnection(connStr))
        {
          c.Open();
          string str = c.ConnectionString;
          int index = str.IndexOf("Database=");
          Assert.AreEqual(-1, index);
        }
      }
    }


    class ConnectionClosedCheck
    {
      public bool closed = false;
      public void stateChangeHandler(object sender, StateChangeEventArgs e)
      {
        if (e.CurrentState == ConnectionState.Closed)
          closed = true;
      }
    }

    [Test]
    public void ConnectionCloseByGC()
    {
      int threadId;
      ConnectionClosedCheck check = new ConnectionClosedCheck();
      string connStr = GetConnectionString(true) + ";pooling=true";
      MySqlConnection c = new MySqlConnection(connStr);
      c.StateChange += new StateChangeEventHandler(check.stateChangeHandler);
      c.Open();
      threadId = c.ServerThread;
      c = null;
      GC.Collect();
      GC.WaitForPendingFinalizers();
      Assert.IsTrue(check.closed);

      MySqlCommand cmd = new MySqlCommand("KILL " + threadId, conn);
      cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Bug #30964 StateChange imperfection 
    /// </summary>
    MySqlConnection rqConnection;
    [Test]
    public void RunningAQueryFromStateChangeHandler()
    {
      string connStr = GetConnectionString(true);
      using (rqConnection = new MySqlConnection(connStr))
      {
        rqConnection.StateChange += new StateChangeEventHandler(RunningQueryStateChangeHandler);
        rqConnection.Open();
      }
    }

    void RunningQueryStateChangeHandler(object sender, StateChangeEventArgs e)
    {
      if (e.CurrentState == ConnectionState.Open)
      {
        MySqlCommand cmd = new MySqlCommand("SELECT 1", rqConnection);
        object o = cmd.ExecuteScalar();
        Assert.AreEqual(1, o);
      }
    }

    /// <summary>
    /// Bug #31262 NullReferenceException in MySql.Data.MySqlClient.NativeDriver.ExecuteCommand 
    /// </summary>
    [Test]
    public void ConnectionNotOpenThrowningBadException()
    {
      MySqlConnection c2 = new MySqlConnection();
      c2.ConnectionString = GetConnectionString(true); // "DataSource=localhost;Database=test;UserID=root;Password=********;PORT=3306;Allow Zero Datetime=True;logging=True;";
      //conn.Open();                      << REM
      MySqlCommand command = new MySqlCommand();
      command.Connection = c2;

      MySqlCommand cmdCreateTable = new MySqlCommand("DROP TABLE IF EXISTS `test`.`contents_catalog`", c2);
      cmdCreateTable.CommandType = CommandType.Text;
      cmdCreateTable.CommandTimeout = 0;
      try
      {
        cmdCreateTable.ExecuteNonQuery();
      }
      catch (InvalidOperationException)
      {
      }
    }

    /// <summary>
    /// Bug #31433 Username incorrectly cached for logon where case sensitive 
    /// </summary>
    [Test]
    public void CaseSensitiveUserId()
    {
      string connStr = GetConnectionString("Test", "test", true);
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        try
        {
          c.Open();
        }
        catch (MySqlException)
        {
        }
      }

      connStr = GetConnectionString("test", "test", true);
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        c.Open();
      }
    }

    /// <summary>
    /// Bug #35619 creating a MySql connection from toolbox generates an error 
    /// </summary>
    [Test]
    public void NullConnectionString()
    {
      MySqlConnection c = new MySqlConnection();
      c.ConnectionString = null;
    }

    /// <summary>
    /// Bug #53097  	Connection.Ping() closes connection if executed on a connection with datareader
    /// </summary>
    [Test]
    public void PingWhileReading()
    {
      MySqlCommand command = new MySqlCommand("SELECT 1", conn);

      using (MySqlDataReader reader = command.ExecuteReader())
      {
        reader.Read();
        try
        {
          conn.Ping();
          Assert.Fail("Test Failed.");
        }
        catch (MySqlException ex)
        {
          Assert.AreEqual(Resources.DataReaderOpen, ex.Message);
        }
      }
    }

    /// <summary>
    /// Test if keepalive parameters work.
    /// </summary>
    [Test]
    public void Keepalive()
    {
      string connstr = GetConnectionString("test", "test", true);
      connstr += ";keepalive=1;";
      using (MySqlConnection c = new MySqlConnection(connstr))
      {
        c.Open();
      }
    }

#if !CF
    [Test]
    public void CanOpenConnectionInMediumTrust()
    {
      AppDomain appDomain = PartialTrustSandbox.CreatePartialTrustDomain();

      PartialTrustSandbox sandbox = (PartialTrustSandbox)appDomain.CreateInstanceAndUnwrap(
          typeof(PartialTrustSandbox).Assembly.FullName,
          typeof(PartialTrustSandbox).FullName);

      try
      {
        MySqlConnection connection = sandbox.TryOpenConnection(GetConnectionString(true));
        Assert.IsNotNull(connection);
        Assert.IsTrue(connection.State == ConnectionState.Open);
        connection.Close();

        //Now try with logging enabled
        connection = sandbox.TryOpenConnection(GetConnectionString(true) + ";logging=true");
        Assert.IsNotNull(connection);
        Assert.IsTrue(connection.State == ConnectionState.Open);
        connection.Close();

        //Now try with Usage Advisor enabled
        connection = sandbox.TryOpenConnection(GetConnectionString(true) + ";Use Usage Advisor=true");
        Assert.IsNotNull(connection);
        Assert.IsTrue(connection.State == ConnectionState.Open);
        connection.Close();
      }
      finally
      {
        AppDomain.Unload(appDomain);
      }
    }

    /// <summary>
    /// A client can connect to MySQL server using SSL and a pfx file.
    /// <remarks>
    /// This test requires starting the server with SSL support. 
    /// For instance, the following command line enables SSL in the server:
    /// mysqld --no-defaults --standalone --console --ssl-ca='MySQLServerDir'\mysql-test\std_data\cacert.pem --ssl-cert='MySQLServerDir'\mysql-test\std_data\server-cert.pem --ssl-key='MySQLServerDir'\mysql-test\std_data\server-key.pem
    /// </remarks>
    /// </summary>
    [Test]
    public void CanConnectUsingFileBasedCertificate()
    {
      if (Version < new Version(5, 1)) return;

      string connstr = GetConnectionString(true);
      connstr += ";CertificateFile=client.pfx;CertificatePassword=pass;SSL Mode=Required;";
      using (MySqlConnection c = new MySqlConnection(connstr))
      {
        c.Open();
        Assert.AreEqual(ConnectionState.Open, c.State);
      }
    }
#endif

#if CF
        /// <summary>
        /// A client running in .NET Compact Framework can't connect to MySQL server using SSL and a pfx file.
        /// <remarks>
        /// This test requires starting the server with SSL support. 
        /// For instance, the following command line enables SSL in the server:
        /// mysqld --no-defaults --standalone --console --ssl-ca='MySQLServerDir'\mysql-test\std_data\cacert.pem --ssl-cert='MySQLServerDir'\mysql-test\std_data\server-cert.pem --ssl-key='MySQLServerDir'\mysql-test\std_data\server-key.pem
        /// </remarks>
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotConnectUsingFileBasedCertificateInCF()
        {
            string connstr = GetConnectionString(true);
            connstr += ";CertificateFile=client.pfx;CertificatePassword=pass;SSL Mode=Required;";

            MySqlConnection c = new MySqlConnection(connstr);
        }
#endif

    [Test]
    public void CanOpenConnectionAfterAborting()
    {
      MySqlConnection connection = new MySqlConnection(GetConnectionString(true));
      connection.Open();
      Assert.AreEqual(ConnectionState.Open, connection.State);

      connection.Abort();
      Assert.AreEqual(ConnectionState.Closed, connection.State);

      connection.Open();
      Assert.AreEqual(ConnectionState.Open, connection.State);

      connection.Close();
    }
  }
}
