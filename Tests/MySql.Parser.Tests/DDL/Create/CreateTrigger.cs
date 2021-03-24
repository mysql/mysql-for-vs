// Copyright (c) 2014, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Antlr.Runtime;
using System;
using System.Text;
using Xunit;


namespace MySql.Parser.Tests
{
  
  public class CreateTrigger
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql(@"CREATE TRIGGER testref BEFORE INSERT ON test1
  FOR EACH ROW BEGIN
    INSERT INTO test2 SET a2 = NEW.a1;
    DELETE FROM test3 WHERE a3 = NEW.a1;
    UPDATE test4 SET b4 = b4 + 1 WHERE a4 = NEW.a1;
  END;");
    }

    [Fact]
    public void BeforeInsert()
    {
      Utility.ParseSql(@"CREATE TRIGGER sdata_insert BEFORE INSERT ON `sometable`
FOR EACH ROW
BEGIN
SET NEW.guid = UUID();
END");
    }

    [Fact]
    public void AfterInsert()
    {
      Utility.ParseSql(@"CREATE TRIGGER sdata_insert AFTER INSERT ON `sometable`
FOR EACH ROW
BEGIN
SET NEW.guid = UUID();
END");
    }

    [Fact]
    public void BeforeInsert2()
    {
      Utility.ParseSql(
        @"CREATE TRIGGER user_insert BEFORE INSERT ON `user` FOR EACH ROW SET NEW.TimeStampCreated = NOW(), 
        NEW.Password = DES_ENCRYPT(NEW.Password);");
    }

    [Fact]
    public void BeforeUpdate()
    {
      Utility.ParseSql(
        @"CREATE TRIGGER user_update BEFORE UPDATE ON `user` FOR EACH ROW SET NEW.Password = DES_ENCRYPT(NEW.Password);");
    }

    [Fact]
    public void BeforeInsert3()
    {
      Utility.ParseSql(
        @"CREATE TRIGGER mytrigger BEFORE INSERT ON TABLE_1 FOR EACH ROW SET NEW.MY_DATETIME_COLUMN = NOW(), 
        NEW.MY_DATE_COLUMN = CURDATE()");
    }

    [Fact]
    public void ForEachRow()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON someTable
FOR EACH ROW
BEGIN
CALL doSanityCheck(@resultBool, @resultMessage);
IF @resultBool = 0 THEN
UPDATE ThereWasAnError_Call_privilegeSanityCheck_ToViewTheError SET ThereWas='an error';
END IF;
END;");
    }

    [Fact]
    public void ForEachRow2()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON my_table
FOR EACH ROW
BEGIN
IF something THEN
SET NEW.S_ID = 0 ;
END IF;
END;");
    }

    [Fact]
    public void FollowsTriggerSyntax56()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON my_table
FOR EACH ROW FOLLOWS mytrigger
BEGIN
IF something THEN
SET NEW.S_ID = 0 ;
END IF;
END;", true, new Version( 5, 6 ) );
    }

    [Fact]
    public void FollowsTriggerSyntax57()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON my_table
FOR EACH ROW FOLLOWS mytrigger
BEGIN
IF something THEN
SET NEW.S_ID = 0 ;
END IF;
END;", false, new Version(5, 7));
    }

    [Fact]
    public void PrecedesTriggerSyntax56()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON my_table
FOR EACH ROW PRECEDES mytrigger
BEGIN
IF something THEN
SET NEW.S_ID = 0 ;
END IF;
END;", true, new Version(5, 6));
    }

    [Fact]
    public void PrecedesTriggerSyntax57()
    {
      Utility.ParseSql(@"CREATE TRIGGER sanityCheck
BEFORE INSERT ON my_table
FOR EACH ROW PRECEDES mytrigger
BEGIN
IF something THEN
SET NEW.S_ID = 0 ;
END IF;
END;", false, new Version(5, 7));
    }
//    [Fact]
//    public void ForEachRow3()
//    {
//      Utility.ParseSql(@"create trigger trg_trigger_test_ins before insert on trigger_test
//for each row 
//begin
//declare msg varchar(255);
//if new.id < 0 then
//set msg = 
//  concat('MyTriggerError: Trying to insert a negative value in trigger_test: ', 
//    cast(new.id as char));
//signal sqlstate '45000' set message_text = msg;
//end if;
//end");
    //}
  }
}
