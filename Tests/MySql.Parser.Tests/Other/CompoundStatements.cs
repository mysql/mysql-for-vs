// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
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

using System;
using System.Text;
using Antlr.Runtime;
using Xunit;

namespace MySql.Parser.Tests
{
  
  public class CompoundStatements
  {
    [Fact]
    public void Iterate()
    {
      Utility.ParseSql(
  @"CREATE PROCEDURE doiterate(p1 INT)
BEGIN
  label1: LOOP
    SET p1 = p1 + 1;
    IF p1 < 10 THEN
      ITERATE label1;
    END IF;
    LEAVE label1;
  END LOOP label1;
  SET @x = p1;
END;
",
  false);
    }

    [Fact]
    public void Handler()
    {
      Utility.ParseSql(
        @"CREATE PROCEDURE handlerdemo()
     BEGIN
       DECLARE CONTINUE HANDLER FOR SQLSTATE '23000' SET @x2 = 1;
       SET @x = 1;
       INSERT INTO test.t VALUES (1);
       SET @x = 2;
       INSERT INTO test.t VALUES (1);
       SET @x = 3;
     END;",
        false);
    }

    [Fact]
    public void Handler2()
    {
      Utility.ParseSql(
        @"
begin 
  DECLARE CONTINUE HANDLER FOR SQLWARNING BEGIN END;
end;",
        false);
    }

    [Fact]
    public void Handler3()
    {
      Utility.ParseSql(
        @"
begin
  DECLARE CONTINUE HANDLER FOR SQLWARNING BEGIN END;
end;",
        false);
    }

    [Fact]
    public void Timestamp50()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"create procedure sp() 
begin 
  DECLARE mystamp timestamp( 20 );
END;",
        true, out sb, new Version( 5, 0 ));
      Assert.True(sb.Length != 0);
    }

    [Fact]
    public void Timestamp51()
    {
      Utility.ParseSql(
        @"create procedure sp() 
begin 
  DECLARE mystamp timestamp( 20 );
END;",
        false, new Version(5, 1));
    }
  }
}
