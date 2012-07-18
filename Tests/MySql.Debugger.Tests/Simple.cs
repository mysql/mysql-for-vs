// Copyright © 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void ScalarFunctionCall2()
    {
      string sql =
        @"delimiter //
drop procedure if exists `SimpleScalar` //
CREATE PROCEDURE `SimpleScalar`()
begin
 
    update CalcData set z = DoSum( x, y ) where x = 5;

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
@"CREATE PROCEDURE `SimpleScalar`()
begin
 
    update CalcData set z = DoSum( x, y ) where x = 5;

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) => {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
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
        dbg.OnBreakpoint += (bp) => {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
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
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
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

    declare val1 int;
    set val1 = DoSum( 1, 2 );
    set val1 = val1 + 2;

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void CommaSeparatedDeclare()
    {
      string sql =
        @"delimiter //
drop procedure if exists spTest2 //

create DEFINER=`root`@`localhost` PROCEDURE `spTest2`()
begin
    declare n,x,y,z int;
	declare str varchar(1100);
    set n = 1;
	set str = 'Armando';

    while n < 1000 do
    begin
    
        set n = n + 1;
		set x = n * 2;
		set y = n * 5;
		set z = n * 10;
		set str = CONCAT(str, 'o');
    
    end;
    end while;

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
@"create DEFINER=`root`@`localhost` PROCEDURE `spTest2`()
begin
    declare n,x,y,z int;
	declare str varchar(1100);
    set n = 1;
	set str = 'Armando';

    while n < 10 do
    begin
    
        set n = n + 1;
		set x = n * 2;
		set y = n * 5;
		set z = n * 10;
		set str = CONCAT(str, 'o');
    
    end;
    end while;

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) => {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void LoopWithIfs()
    {
      string sql =
        @"delimiter //
drop procedure if exists doloopif //

DELIMITER //
CREATE PROCEDURE doloopif (p1 INT)
BEGIN
	DECLARE var_x INT;
	SET var_x=0;
	loop_test: LOOP
		IF var_x < p1 THEN
			SET var_x = var_x+1;
		ELSE
			LEAVE loop_test;
		END IF;
	END LOOP loop_test;
	SELECT CONCAT ('The final LOOP and IF number is: ', var_x) AS Results;
END
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
@"CREATE PROCEDURE doloopif (p1 INT)
BEGIN
	DECLARE var_x INT;
	SET var_x=0;
	loop_test: LOOP
		IF var_x < p1 THEN
			SET var_x = var_x+1;
		ELSE
			LEAVE loop_test;
		END IF;
	END LOOP loop_test;
	SELECT CONCAT ('The final LOOP and IF number is: ', var_x) AS Results;
END;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[1] { "3" } );
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void DoHandler()
    {
      string sql =
        @"
delimiter //

drop table if exists d_table //

CREATE TABLE d_table (s1 int, primary key (s1)) //

drop procedure if exists dohandler //

DELIMITER //
CREATE PROCEDURE dohandler()
BEGIN
	DECLARE dup_keys CONDITION FOR  SQLSTATE '23000';
	DECLARE CONTINUE HANDLER FOR dup_keys SET @GARBAGE = 1;
	SET @x = 1;
	INSERT INTO world.d_table VALUES (1);
	SET @x = 2;
	INSERT INTO world.d_table VALUES (1);
	set @x = 3;

END //
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
@"CREATE PROCEDURE dohandler()
BEGIN
	DECLARE dup_keys CONDITION FOR  SQLSTATE '23000';
	DECLARE CONTINUE HANDLER FOR dup_keys SET @GARBAGE = 1;
	SET @x = 1;
	INSERT INTO test6.d_table VALUES (1);
	SET @x = 2;
	INSERT INTO test6.d_table VALUES (1);
	set @x = 3;

END;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void DoRepeat()
    {
      string sql =
        @"
delimiter //

drop procedure if exists DoRepeat //

DELIMITER //
CREATE PROCEDURE DoRepeat()
BEGIN
  DECLARE i INT default 3;
  DECLARE done1 INT default 0;  
  
  retry: REPEAT
  begin
  DECLARE CONTINUE HANDLER FOR SQLWARNING
          BEGIN
            SET done1 = TRUE;
          END;  
        IF done1 OR i < 0 THEN
          LEAVE retry;
        END IF;
        SET i = i - 1;      
        end;
    UNTIL FALSE END REPEAT;
END  //
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
@"CREATE PROCEDURE DoRepeat()
BEGIN
  DECLARE i INT default 3;
  DECLARE done1 INT default 0;  
  
  retry: REPEAT
  begin
  DECLARE CONTINUE HANDLER FOR SQLWARNING
          BEGIN
            SET done1 = TRUE;
          END;  
        IF done1 OR i < 0 THEN
          LEAVE retry;
        END IF;
        SET i = i - 1;      
        end;
    UNTIL FALSE END REPEAT;
END;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[0]);
      }
      finally
      {        
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void MutipleInsert()
    {
      string sql =
        @"
delimiter //

drop procedure if exists MultipleInsert //

drop table if exists test3 //

DELIMITER //
create procedure MultipleInsert( id int, name varchar( 10 ))
begin
	create table test3( id2 int );
	insert into test3 values (1);
end //
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
@"create procedure MultipleInsert( id int, name varchar( 10 ))
begin
	create table test3( id2 int );
	insert into test3 values (1);
end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run(new string[] { "3", "'a'" });
      }
      finally
      {
        dbg.RestoreRoutinesBackup();
      }
    }

    [Test]
    public void SteppingIntoTriggers()
    {
      string sql =
        @"
delimiter //

drop table if exists TriggerTable //

create table TriggerTable ( 
  myid int,
  myname varchar( 30 )
) //

create trigger trTriggerTable before insert on TriggerTable for each row
begin

    set new.myid = new.myid + 1;

end //

drop procedure if exists DoInsertTriggerTable //

create procedure DoInsertTriggerTable()
begin

  insert into TriggerTable( myid, myname ) values ( 1, 'val' );

end //
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
@"create procedure DoInsertTriggerTable()
begin

  insert into TriggerTable( myid, myname ) values ( 1, 'val' );

end;
";
        dbg.SqlInput = sql;
        dbg.SteppingType = SteppingTypeEnum.StepInto;
        dbg.OnBreakpoint += (bp) =>
        {
          Debug.WriteLine(string.Format("NonScalarFunction breakpoint at line {0}:{1}", bp.RoutineName, bp.Line));
        };
        dbg.Run( new string[0] );
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