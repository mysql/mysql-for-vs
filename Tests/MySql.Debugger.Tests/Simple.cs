using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using MySql.Debugger;
using MySql.Parser;
using MySql.Data.MySqlClient;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Debugger.Tests
{
  [TestFixture]
  public class Simple : BaseTest
  {
    [Test]
    public void VerySimpleTest()
    {
      string sql = 
@"create procedure spTest()
begin
    declare n int;
    set n = 1;
    while n < 5 do
    begin
    
        set n = n + 1;
    
    end;
    end while;

end;
";
      
      Debugger dbg = new Debugger();
      try
      {
        dbg.SqlInput = sql;
        dbg.Connection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.UtilityConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.LockingConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        DumpConnectionThreads(dbg);
        MySqlScript script = new MySqlScript(dbg.Connection, string.Format( "delimiter // drop procedure if exists spTest; {0} //", sql ));
        script.Execute();
        Watch w = dbg.SetWatch("n");
        dbg.SetBreakpoint( sql, 8);
        dbg.SetBreakpoint( sql, 13);
        bool bpHit = false;
        int i = 0;
        dbg.OnBreakpoint += (bp) =>
        {
          bpHit = true;
          int val = Convert.ToInt32(w.Eval());
          if (bp.Line == 8)
          {
            Assert.AreEqual(++i, val);
            Debug.Write(val);
            Debug.WriteLine(" within simpleproc");
          }
          else if (bp.Line == 13)
          {
            Assert.AreEqual( 5, val );
            Debug.Write(val);
            Debug.WriteLine(" within simpleproc");
          }
        };
        dbg.Run(new string[0]);
        Assert.IsTrue(bpHit);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void NonScalarFunction()
    {
      string sql =
        @"delimiter //
drop procedure if exists `SimpleNonScalar` //
CREATE PROCEDURE `SimpleNonScalar`()
begin
 
    update CalcData set z = DoSum( x, y );

end //
drop function if exists `DoSum`
//
CREATE FUNCTION `DoSum`( a int, b int ) RETURNS int(11)
begin

    declare a1 int;
    declare b1 int;
    
    set a1 = a;
    set b1 = b;
    return a1 + b1;

end
//
drop table if exists `calcdata`;
//
CREATE TABLE `calcdata` (
  `x` int(11) DEFAULT NULL,
  `y` int(11) DEFAULT NULL,
  `z` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 //
insert into `calcdata`( x, y, z ) values ( 5, 10, 0 ) //
insert into `calcdata`( x, y, z ) values ( 8, 4, 0 ) //
insert into `calcdata`( x, y, z ) values ( 6, 7, 0 ) //
";
      Debugger dbg = new Debugger();
      try
      {
        dbg.Connection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.UtilityConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.LockingConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        DumpConnectionThreads(dbg);
        MySqlScript script = new MySqlScript(dbg.Connection, sql);
        script.Execute();
        sql =
@"CREATE PROCEDURE `SimpleNonScalar`()
begin
 
    update CalcData set z = DoSum( x, y );

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.None;
        //dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void NestedCall()
    {
      string sql =
        @"delimiter //
drop procedure if exists `NestedCall` //
CREATE PROCEDURE `NestedCall`()
begin
 
    call dummyCall();

end //
drop procedure if exists `DummyCall`
//
create procedure DummyCall()
begin

    update CalcData set z = -1;

end
//
drop table if exists `calcdata`;
//
CREATE TABLE `calcdata` (
  `x` int(11) DEFAULT NULL,
  `y` int(11) DEFAULT NULL,
  `z` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 //
insert into `calcdata`( x, y, z ) values ( 5, 10, 0 ) //
insert into `calcdata`( x, y, z ) values ( 8, 4, 0 ) //
insert into `calcdata`( x, y, z ) values ( 6, 7, 0 ) //
";
      Debugger dbg = new Debugger();
      try
      {
        dbg.Connection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.UtilityConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.LockingConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        DumpConnectionThreads(dbg);
        MySqlScript script = new MySqlScript(dbg.Connection, sql);
        script.Execute();
        sql =
@"CREATE PROCEDURE `NestedCall`()
begin
 
    call dummyCall();

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        //dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }


    [Test]
    public void NestedCallWithVars()
    {
      string sql =
        @"delimiter //
drop procedure if exists `NestedCall` //
CREATE PROCEDURE `NestedCall`()
begin
 
    declare val int;
    call dummyCall( val );

end //
drop procedure if exists `DummyCall`
//
create procedure DummyCall()
begin

    update CalcData set z = -1;

end
//
drop table if exists `calcdata`;
//
CREATE TABLE `calcdata` (
  `x` int(11) DEFAULT NULL,
  `y` int(11) DEFAULT NULL,
  `z` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 //
insert into `calcdata`( x, y, z ) values ( 5, 10, 0 ) //
insert into `calcdata`( x, y, z ) values ( 8, 4, 0 ) //
insert into `calcdata`( x, y, z ) values ( 6, 7, 0 ) //
";
      Debugger dbg = new Debugger();
      try
      {
        dbg.Connection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.UtilityConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.LockingConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        DumpConnectionThreads(dbg);
        MySqlScript script = new MySqlScript(dbg.Connection, sql);
        script.Execute();
        sql =
@"CREATE PROCEDURE `NestedCall`()
begin
 
    call dummyCall();

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        //dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void ScalarFunctionCall()
    {
      string sql =
        @"delimiter //

drop procedure if exists NestedFunction //

create procedure NestedFunction()
begin

    declare val int;    
    set val = DoSum( 1, 2 );
    set val = val + 2;

end
   //
drop function if exists `DoSum`
//
CREATE FUNCTION `DoSum`( a int, b int ) RETURNS int(11)
not deterministic modifies sql data
begin

declare a1 int;
   declare b1 int;
    
    set a1 = a;
    set b1 = b;
    return a1 + b1;
#return a + b;

end
//
";
      Debugger dbg = new Debugger();
      try
      {
        dbg.Connection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.UtilityConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        dbg.LockingConnection = new MySqlConnection(TestUtils.CONNECTION_STRING);
        DumpConnectionThreads(dbg);
        MySqlScript script = new MySqlScript(dbg.Connection, sql);
        script.Execute();
        sql =
@"create procedure NestedFunction()
begin

    declare val int;
    set val = DoSum( 1, 2 );
    set val = val + 2;

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.None;
        dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    private void DumpConnectionThreads(Debugger dbg)
    {
      dbg.Connection.Open();
      dbg.UtilityConnection.Open();
      dbg.LockingConnection.Open();
      //dbg.LockingConnection2.Open();
      Debug.WriteLine(string.Format("Debugger thread id: {0}", dbg.UtilityConnection.ServerThread));
      Debug.WriteLine(string.Format("Debuggee thread id: {0}", dbg.Connection.ServerThread));
      Debug.WriteLine(string.Format("Locking thread id: {0}", dbg.LockingConnection.ServerThread));
      //Debug.WriteLine(string.Format("Locking2 thread id: {0}", dbg.LockingConnection2.ServerThread));
    }
  }
}