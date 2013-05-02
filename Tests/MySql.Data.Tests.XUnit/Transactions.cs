﻿// Copyright © 2013 Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Transactions;
using System.Data.Common;
using System.Data;
using System.Threading;
using System.Diagnostics;

namespace MySql.Data.MySqlClient.Tests.XUnit
{
  public class Transactions : IUseFixture<SetUpClass>, IDisposable
  {
    private SetUpClass st;

    public void SetFixture(SetUpClass data)
    {
      st = data;
    }

    public void Dispose()
    {
      //Nothing to clean      
    }

    void TransactionScopeInternal(bool commit)
    {
      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
      using (MySqlConnection c = new MySqlConnection(st.GetConnectionString(true)))
      {
        MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c);

        using (TransactionScope ts = new TransactionScope())
        {
          c.Open();          
          cmd.ExecuteNonQuery();
          if (commit)
            ts.Complete();
        }

        cmd.CommandText = "SELECT COUNT(*) FROM Test";
        object count = cmd.ExecuteScalar();
        Assert.Equal(commit ? 1 : 0, Convert.ToInt32(count));
      }
    }

    [Fact]
    public void TransactionScopeRollback()
    {
      Debug.Print("Enter TransactionScopeRollback");
      TransactionScopeInternal(false);
      Debug.Print("Out TransactionScopeRollback");
    }

    [Fact]
    public void TransactionScopeCommit()
    {
      Debug.Print("Enter TranscationScopeCommit");
      TransactionScopeInternal(true);
      Debug.Print("Out TranscationScopeCommit");
    }

    /// <summary>
    /// Bug #34448 Connector .Net 5.2.0 with Transactionscope doesn´t use specified IsolationLevel 
    /// </summary>
    [Fact]
    public void TransactionScopeWithIsolationLevel()
    {

      Debug.Print("Enter TransactionScopeWithIsolationLevel");
      
      TransactionOptions opts = new TransactionOptions();
      opts.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

      using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, opts))
      {
        string connStr = st.GetConnectionString(true);
        using (MySqlConnection myconn = new MySqlConnection(connStr))
        {
          myconn.Open();
          MySqlCommand cmd = new MySqlCommand("SHOW VARIABLES LIKE 'tx_isolation'", myconn);
          using (MySqlDataReader reader = cmd.ExecuteReader())
          {
            reader.Read();
            string level = reader.GetString(1);
            Assert.Equal("READ-COMMITTED", level);
          }
        }
      }
      Debug.Print("Out TransactionScopeWithIsolationLevel");
    }

    /// <summary>
    /// Bug #27289 Transaction is not rolledback when connection close 
    /// </summary>
    [Fact]
    public void RollingBackOnClose()
    {
      Debug.Print("Enter RollingBackOnClose");

      st.execSQL("DROP TABLE IF EXISTS Test");
      st.execSQL("CREATE TABLE Test (id INT) ENGINE=InnoDB");

      string connStr = st.GetPoolingConnectionString();
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        c.Open();
        MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (1)", c);
        c.BeginTransaction();
        cmd.ExecuteNonQuery();
      }

      using (MySqlConnection c2 = new MySqlConnection(connStr))
      {
        c2.Open();
        MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) from Test", c2);
        c2.BeginTransaction();
        object count = cmd2.ExecuteScalar();
        Assert.Equal(0, Convert.ToInt32(count));
      }

      MySqlConnection connection = new MySqlConnection(connStr);
      connection.Open();
      st.KillConnection(connection);
      Debug.Print("Enter RollingBackOnClose");
    }

    /// <summary>
    /// NullReferenceException thrown on TransactionScope dispose
    /// </summary>
//    [Fact]
//    public void LockedTable()
//    {

//      Debug.Print("Enter LockedTable");

//      string connStr = st.GetConnectionString(true);
      
//      st.execSQL("DROP TABLE IF EXISTS t1");
//      st.execSQL("DROP TABLE IF EXISTS t2");

//      connStr = String.Format(@"Use Affected Rows=true;allow user variables=yes;Server=localhost;Port={0};
//            Database={1};Uid=root;Connect Timeout=35;default command timeout=90;charset=utf8", st.port, st.database0);


//      st.execSQL(@"CREATE TABLE `t1` (
//                `Key` int(10) unsigned NOT NULL auto_increment,
//                `Val` varchar(100) NOT NULL,
//                `Val2` varchar(100) NOT NULL default '',
//                PRIMARY KEY  (`Key`)
//                ) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1");
//      st.execSQL(@"CREATE TABLE `t2` (
//                `Key` int(10) unsigned NOT NULL auto_increment,
//                `Val` varchar(100) NOT NULL,
//                PRIMARY KEY  (`Key`)
//                ) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1");

//      st.execSQL("lock tables t2 read");      

//      using (TransactionScope scope = new TransactionScope())
//      {
//        using (MySqlConnection conn = new MySqlConnection(connStr))
//        using (MySqlCommand cmd = conn.CreateCommand())
//        {
//          conn.Open();                
//          cmd.CommandText = @"insert into t1 (Val,Val2) values (?value1, ?value2)"; ;
//          cmd.CommandTimeout = 5;
//          cmd.Parameters.AddWithValue("?value1", new Random().Next());
//          cmd.Parameters.AddWithValue("?value2", new Random().Next());
//          cmd.ExecuteNonQuery();
//        }

//        using (MySqlConnection conn = new MySqlConnection(connStr))
//        using (MySqlCommand cmd = conn.CreateCommand())
//        {
//          conn.Open();          
//          cmd.CommandText = @"insert into t2 (Val) values (?value)";
//          cmd.CommandTimeout = 5;
//          cmd.Parameters.AddWithValue("?value", new Random().Next());
//          Exception ex = Assert.Throws<MySqlException>(() => cmd.ExecuteNonQuery());
//          Assert.Equal(ex.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.");         
//        }               

//        scope.Complete();
//      }     

//      MySqlPoolManager.ClearAllPools();
      
//      Debug.Print("Out LockedTable");
//    }


    /// <summary>
    /// Bug #22042 mysql-connector-net-5.0.0-alpha BeginTransaction 
    /// </summary>
    [Fact]
    public void Bug22042()
    {
      Debug.Print("Enter Bug22042");
      DbProviderFactory factory =
          new MySql.Data.MySqlClient.MySqlClientFactory();
      using (DbConnection conexion = factory.CreateConnection())
      {
        conexion.ConnectionString = st.GetConnectionString(true);
        conexion.Open();
        DbTransaction trans = conexion.BeginTransaction();
        trans.Rollback();
      }
      
      Debug.Print("Out Bug22042");
    }

    /// <summary>
    /// Bug #26754  	EnlistTransaction throws false MySqlExeption "Already enlisted"
    /// </summary>
    [Fact]
    public void EnlistTransactionNullTest()
    {
      Debug.Print("Enter EnlistTransactionNullTest");

      try
      {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = st.conn;
        cmd.Connection.EnlistTransaction(null);
      }
      catch { }

      using (TransactionScope ts = new TransactionScope())
      {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = st.conn;
        cmd.Connection.EnlistTransaction(Transaction.Current);
      }
      Debug.Print("Out EnlistTransactionNullTest");
    }

    /// <summary>
    /// Bug #26754  	EnlistTransaction throws false MySqlExeption "Already enlisted"
    /// </summary>
    [Fact]
    public void EnlistTransactionWNestedTrxTest()
    {
      Debug.Print("Enter EnlistTransactionWNestedTrxTest");

      MySqlTransaction t = st.conn.BeginTransaction();

      using (TransactionScope ts = new TransactionScope())
      {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = st.conn;
        try
        {
          cmd.Connection.EnlistTransaction(Transaction.Current);
        }
        catch (InvalidOperationException)
        {
          /* caught NoNestedTransactions */
        }
      }

      t.Rollback();

      using (TransactionScope ts = new TransactionScope())
      {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = st.conn;
        cmd.Connection.EnlistTransaction(Transaction.Current);
      }

      Debug.Print("Out EnlistTransactionWNestedTrxTest");

    }

    [Fact]
    public void ManualEnlistment()
    {
      Debug.Print("Enter ManualEnlistment");

      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
      string connStr = st.GetConnectionString(true) + ";auto enlist=false";
      MySqlConnection c = null;
      using (TransactionScope ts = new TransactionScope())
      {
        c = new MySqlConnection(connStr);
        c.Open();
        MySqlCommand cmd = new MySqlCommand("LOCK TABLES test WRITE;", c);
        cmd.ExecuteNonQuery();
        cmd = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c);
        cmd.ExecuteNonQuery();
        st.execSQL("UNLOCK TABLES");
      }
      MySqlCommand cmd2 = new MySqlCommand("LOCK TABLES test READ; SELECT COUNT(*) FROM test", c);
      Assert.Equal(1, Convert.ToInt32(cmd2.ExecuteScalar()));
      st.execSQL("UNLOCK TABLES");
      c.Dispose();
      st.KillPooledConnection(connStr);

      Debug.Print("Out ManualEnlistment");
    }

    private void ManuallyEnlistingInitialConnection(bool complete)
    {
      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
      string connStr = st.GetConnectionString(true) + ";auto enlist=false";

      using (TransactionScope ts = new TransactionScope())
      {
        using (MySqlConnection c1 = new MySqlConnection(connStr))
        {
          c1.Open();
          c1.EnlistTransaction(Transaction.Current);
          MySqlCommand cmd1 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('a')", c1);
          cmd1.ExecuteNonQuery();
        }

        using (MySqlConnection c2 = new MySqlConnection(connStr))
        {
          c2.Open();
          c2.EnlistTransaction(Transaction.Current);
          MySqlCommand cmd2 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('b')", c2);
          cmd2.ExecuteNonQuery();
        }
        if (complete)
          ts.Complete();
      }

      st.KillPooledConnection(connStr);
    }

    [Fact]
    public void ManuallyEnlistingInitialConnectionTest()
    {
      Debug.Print("Enter ManuallyEnlistingInitialConnectionTest");
      ManuallyEnlistingInitialConnection(true);
      Debug.Print("Out ManuallyEnlistingInitialConnectionTest");
    }

    [Fact]
    public void ManuallyEnlistingInitialConnectionNoComplete()
    {
      Debug.Print("Enter ManuallyEnlistingInitialConnectionNoComplete");
      ManuallyEnlistingInitialConnection(false);
      Debug.Print("Out ManuallyEnlistingInitialConnectionNoComplete");
    }

    [Fact]
    public void ManualEnlistmentWithActiveConnection()
    {
      Debug.Print("Enter ManualEnlistmentWithActiveConnection");
      using (TransactionScope ts = new TransactionScope())
      {
        string connStr = st.GetConnectionString(true);

        using (MySqlConnection c1 = new MySqlConnection(connStr))
        {
          c1.Open();

          connStr += "; auto enlist=false";
          using (MySqlConnection c2 = new MySqlConnection(connStr))
          {
            c2.Open();
            try
            {
              c2.EnlistTransaction(Transaction.Current);
            }
            catch (NotSupportedException)
            {
            }
          }
        }
      }
      Debug.Print("Out ManualEnlistmentWithActiveConnection");
    }

    [Fact]
    public void AttemptToEnlistTwoConnections()
    {
      using (TransactionScope ts = new TransactionScope())
      {
        string connStr = st.GetConnectionString(true);

        using (MySqlConnection c1 = new MySqlConnection(connStr))
        {
          c1.Open();

          using (MySqlConnection c2 = new MySqlConnection(connStr))
          {
            try
            {
              c2.Open();
            }
            catch (NotSupportedException)
            {
            }
          }
        }
      }
    }


    private void NestedScopeInternalTest(
        TransactionScopeOption nestedOption,
        bool innerComplete,
        bool outerComplete,
        bool expectInnerChangesVisible,
        bool expectOuterChangesVisible)
    {
      st.createTable("CREATE TABLE T(str varchar(10))", "INNODB");
      try
      {
        using (TransactionScope outer = new TransactionScope())
        {
          string connStr = st.GetConnectionString(true);
          using (MySqlConnection c1 = new MySqlConnection(connStr))
          {
            c1.Open();
            //MySqlCommand cmd1 = new MySqlCommand("LOCK TABLES T WRITE;", c1);
            //cmd1.ExecuteNonQuery();
            MySqlCommand  cmd1 = new MySqlCommand("INSERT INTO T VALUES ('outer')", c1);
            cmd1.ExecuteNonQuery();
            //cmd1 = new MySqlCommand("UNLOCK TABLES", c1);
            //cmd1.ExecuteNonQuery();          
            using (TransactionScope inner = new TransactionScope(nestedOption))
            {

              MySqlConnection c2;
              if (nestedOption == TransactionScopeOption.Required)
              {
                // inner scope joins already running ambient
                // transaction, we cannot use new connection here
                c2 = c1;
              }
              else
              {
                // when TransactionScopeOption.RequiresNew or 
                // new TransactionScopeOption.Suppress is used,
                // we have to use a new transaction. We create a
                // new connection for it.
                c2 = new MySqlConnection(connStr);
                c2.Open();
              }
              //MySqlCommand cmd2 = new MySqlCommand("LOCK TABLES T WRITE;", c2);
              //cmd2.ExecuteNonQuery();
              MySqlCommand  cmd2 = new MySqlCommand("INSERT INTO T VALUES ('inner')", c2);
              cmd2.ExecuteNonQuery();
              //cmd2 = new MySqlCommand("UNLOCK TABLES", c2);
              //cmd2.ExecuteNonQuery();
              if (innerComplete)
              {
                inner.Complete();                
              }

              // Dispose connection if it was created.
              if (c2 != c1)
                c2.Dispose();
            }          
          }

          if (outerComplete)         
            outer.Complete();            

        }
        bool innerChangesVisible =
           ((long)MySqlHelper.ExecuteScalar(st.conn, "select count(*) from T where str = 'inner'") == 1);
        bool outerChangesVisible =
            ((long)MySqlHelper.ExecuteScalar(st.conn, "select count(*) from T where str = 'outer'") == 1);
        Assert.Equal(innerChangesVisible, expectInnerChangesVisible);
        Assert.Equal(outerChangesVisible, expectOuterChangesVisible);
      }
      finally
      {
        MySqlHelper.ExecuteNonQuery(st.conn, "DROP TABLE T");
      }
    }

    /// <summary>
    /// Test inner/outer scope behavior, with different scope options, 
    /// completing either inner or outer scope, or both.
    /// </summary>
    [Fact]
    public void NestedScope()
    {

      Debug.Print("Enter NestedScope");
      // inner scope joins the ambient scope, neither inner not outer  scope completes
      // Expect empty table.
      NestedScopeInternalTest(TransactionScopeOption.Required, false, false, false, false);

      // inner scope joins the ambient scope, inner does not complete, outer completes
      // Expect exception while disposing outer transaction
      //try
      //{
        Assert.Throws<TransactionAbortedException>(() => NestedScopeInternalTest(TransactionScopeOption.Required, false, true, false, false));        
      //}
      //catch (TransactionAbortedException)
      //{
      //}

      // inner scope joins the ambient scope, inner completes, outer does not
      // Expect empty table.
      NestedScopeInternalTest(TransactionScopeOption.Required, true, false, false, false);

      // inner scope joins the ambient scope, both complete.
      // Expect table with entries for inner and outer scope
      NestedScopeInternalTest(TransactionScopeOption.Required, true, true, true, true);



      // inner scope creates new transaction, neither inner not outer  scope completes
      // Expect empty table.
      NestedScopeInternalTest(TransactionScopeOption.RequiresNew, false, false, false, false);

      // inner scope creates new transaction, inner does not complete, outer completes
      // Expect changes by outer transaction visible ??
      NestedScopeInternalTest(TransactionScopeOption.RequiresNew, false, true, false, true);

      // inner scope creates new transactiion, inner completes, outer does not
      // Expect changes by inner transaction visible
      NestedScopeInternalTest(TransactionScopeOption.RequiresNew, true, false, true, false);

      // inner scope creates new transaction, both complete
      NestedScopeInternalTest(TransactionScopeOption.RequiresNew, true, true, true, true);


      // inner scope suppresses transaction, neither inner not outer  scope completes
      // Expect changes made by inner scope to be visible
      NestedScopeInternalTest(TransactionScopeOption.Suppress, false, false, true, false);

      // inner scope supresses transaction, inner does not complete, outer completes
      // Expect changes by inner scope to be visible ??
      NestedScopeInternalTest(TransactionScopeOption.Suppress, true, false, true, false);

      // inner scope supresses transaction, inner completes, outer does not
      // Expect changes by inner transaction visible
      NestedScopeInternalTest(TransactionScopeOption.Suppress, true, false, true, false);

      // inner scope supresses transaction, both complete
      NestedScopeInternalTest(TransactionScopeOption.Suppress, true, true, true, true);
      
      Debug.Print("Enter NestedScope");
    }



    private void ReusingSameConnection(bool pooling, bool complete)
    {
      int c1Thread;
      st.execSQL("TRUNCATE TABLE Test");

      using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
      {
        string connStr = st.GetConnectionString(true);
        if (!pooling)
          connStr += ";pooling=false";

        using (MySqlConnection c1 = new MySqlConnection(connStr))
        {
          c1.Open();
          MySqlCommand cmd1 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('a')", c1);
          cmd1.ExecuteNonQuery();
          c1Thread = c1.ServerThread;
        }

        using (MySqlConnection c2 = new MySqlConnection(connStr))
        {
          c2.Open();
          MySqlCommand cmd2 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('b')", c2);
          cmd2.ExecuteNonQuery();
          Assert.Equal(c1Thread, c2.ServerThread);
        }

        if (complete)
          ts.Complete();
      }

      MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", st.conn);
      DataTable dt = new DataTable();
      da.Fill(dt);
      if (complete)
      {
        Assert.Equal(2, dt.Rows.Count);
        Assert.Equal("a", dt.Rows[0][0]);
        Assert.Equal("b", dt.Rows[1][0]);
      }
      else
      {
        Assert.Equal(0, dt.Rows.Count);
      }
    }

    [Fact]
    public void ReusingSameConnectionTest()
    {
      Debug.Print("Enter ReusingSameConnectionTest");
      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
      ReusingSameConnection(true, true);
      //            Assert.Equal(processes + 1, CountProcesses());

      ReusingSameConnection(true, false);
      //          Assert.Equal(processes + 1, CountProcesses());

      ReusingSameConnection(false, true);
      //        Assert.Equal(processes + 1, CountProcesses());

      ReusingSameConnection(false, false);
      //      Assert.Equal(processes + 1, CountProcesses());
      Debug.Print("Out ReusingSameConnectionTest");
    }

    /// <summary>
    /// bug#35330 - even if transaction scope has expired, rows can be inserted into
    /// the table, due to race condition with the thread doing rollback
    /// </summary>
    [Fact]
    public void ScopeTimeoutWithMySqlHelper()
    {

      Debug.Print("Enter ScopeTimeoutWithMySqlHelper");
      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (id int)", "INNODB");
      string connStr = st.GetConnectionString(true);
      using (new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromSeconds(1)))
      {
        try
        {
          for (int i = 0; ; i++)
          {
            MySqlHelper.ExecuteNonQuery(connStr, String.Format("INSERT INTO Test VALUES({0})", i)); ;
          }
        }
        catch (Exception)
        {
        }
      }
      long count = (long)MySqlHelper.ExecuteScalar(connStr, "select count(*) from test");
      Assert.Equal(0, count);
      Debug.Print("Out ScopeTimeoutWithMySqlHelper");
    }

    /// <summary>
    /// Variation of previous test, with a single connection and maual enlistment.
    /// Checks that  transaction rollback leaves the connection intact (does not close it) 
    /// and  checks that no command is possible after scope has expired and 
    /// rollback by timer thread is finished.
    /// </summary>
    [Fact]
    public void AttemptToUseConnectionAfterScopeTimeout()
    {

      Debug.Print("Enter AttemptToUseConnectionAfterScopeTimeout"); 
      st.execSQL("DROP TABLE IF EXISTS Test");
      st.createTable("CREATE TABLE Test (id int)", "INNODB");
      string connStr = st.GetConnectionString(true);
      using (MySqlConnection c = new MySqlConnection(connStr))
      {
        c.Open();
        MySqlCommand cmd = new MySqlCommand("select 1", c);
        using (new TransactionScope(TransactionScopeOption.RequiresNew,
            TimeSpan.FromSeconds(1)))
        {
          c.EnlistTransaction(Transaction.Current);
          cmd = new MySqlCommand("select 1", c);
          try
          {
            for (int i = 0; ; i++)
            {
              cmd.CommandText = String.Format("INSERT INTO Test VALUES({0})", i);
              cmd.ExecuteNonQuery();
            }
          }
          catch (Exception)
          {
            // Eat exception
          }

          // Here, scope is timed out and rollback is in progress.
          // Wait until timeout thread finishes rollback then try to 
          // use an aborted connection.
          Thread.Sleep(500);
          //try
          //{
            
            Assert.Throws<TransactionAbortedException>(() => cmd.ExecuteNonQuery());
          //}
          //catch (TransactionAbortedException)
          //{
          //}
        }
        Assert.True(c.State == ConnectionState.Open);
        cmd.CommandText = "select count(*) from Test";
        long count = (long)cmd.ExecuteScalar();
        Assert.Equal(0, count);
      }
      Debug.Print("Out AttemptToUseConnectionAfterScopeTimeout"); 
    }


    /// <summary>
    /// Bug#54681 : Null Reference exception when using transaction
    /// scope in more that one thread
    /// </summary>
    [Fact]
    public void TransactionScopeWithThreads()
    {

      Debug.Print("Enter TransactionScopeWithThreads"); 
      // use transaction scope in the current thread
      DoThreadWork();

      //use transaction scope in another thread (used to crash with null
      // reference exception)
      Thread t = new Thread(new ThreadStart(DoThreadWork));
      t.Start();
      t.Join();
      Debug.Print("Out TransactionScopeWithThreads"); 

    }

    [Fact]
    public void SnapshotIsolationLevelThrowsNotSupportedException()
    {
      Debug.Print("Enter SnapshotIsolationLevelThrowsNotSupportedException"); 
      using (MySqlConnection connection = new MySqlConnection(st.GetConnectionString(true)))
      {
        connection.Open();        
        Exception ex = Assert.Throws<NotSupportedException>(() => connection.BeginTransaction(System.Data.IsolationLevel.Snapshot));
        Assert.Equal(ex.Message, "Snapshot isolation level is not supported.");
      }
      Debug.Print("Out SnapshotIsolationLevelThrowsNotSupportedException");
    }

    private void DoThreadWork()
    {
      using (TransactionScope ts = new TransactionScope())
      {
        string connStr = st.GetConnectionString(true);
        using (MySqlConnection c1 = new MySqlConnection(connStr))
        {
          c1.Open();
        }
      }
    }

  }
}
