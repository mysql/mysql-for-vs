﻿// Copyright © 2013, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using System.Text;
using Xunit;

namespace MySql.Parser.Tests.Other
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
      // ToDo: MYSQLFORVS-612 - This should be working
//      Utility.ParseSql(
//        @"
//begin
//  DECLARE CONTINUE HANDLER FOR SQLWARNING BEGIN END;
//end;",
//        false);
    }

    [Fact]
    public void Timestamp50()
    {
      string result = Utility.ParseSql(
        @"create procedure sp()
begin
  DECLARE mystamp timestamp( 20 );
END;",
        true, new Version(5, 0, 0));
      Assert.True(result.Length != 0);
    }

    [Fact]
    public void Timestamp56()
    {
      Utility.ParseSql(
      @"create procedure sp()
begin
  DECLARE mystamp timestamp( 20 );
END;",
      false, new Version(5, 6, 0));
    }
  }
}
