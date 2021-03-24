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

using System;
using System.Text;
using Antlr.Runtime;
using Xunit;


namespace MySql.Parser.Tests
{
  
  public class GetDiagnostics
  {
    [Fact]
    public void Simple_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"GET DIAGNOSTICS CONDITION 1 @errno = MYSQL_ERRNO;", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("rule savepoint_ident failed predicate: { input.LT(1).Text.ToLower() == \"savepoint\" }?") != -1);
    }

    [Fact]
    public void Simple_56()
    {
      Utility.ParseSql(
        @"GET DIAGNOSTICS CONDITION 1 @errno = MYSQL_ERRNO;", false, new Version(5, 6));
    }

    [Fact]
    public void Simple_2_56()
    {
       Utility.ParseSql(
        @"GET DIAGNOSTICS @cno = NUMBER;", false, new Version(5, 6));
    }

    [Fact]
    public void Simple_3_56()
    {
      Utility.ParseSql(
        @"GET DIAGNOSTICS CONDITION @cno @errno = MYSQL_ERRNO;", false, new Version(5, 6));
    }

    [Fact]
    public void Compound_56()
    {
      Utility.ParseSql(
        @"CREATE PROCEDURE do_insert(`value` INT)
BEGIN
  -- declare variables to hold diagnostics area information
  DECLARE `code` CHAR(5) DEFAULT '00000';
  DECLARE msg TEXT;
  DECLARE `rows` INT;
  DECLARE result TEXT;
  -- declare exception handler for failed insert
  DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
      GET DIAGNOSTICS CONDITION 1
        `code` = RETURNED_SQLSTATE, msg = MESSAGE_TEXT;
    END;

  -- perform the insert
  INSERT INTO t1 (int_col) VALUES( `value` );
  -- check whether the insert was successful
  IF `code` = '00000' THEN
    GET DIAGNOSTICS `rows` = ROW_COUNT;
    SET result = CONCAT('insert succeeded, row count = ',`rows`);
  ELSE
    SET result = CONCAT('insert failed, error = ',`code`,', message = ',msg);
  END IF;
  -- say what happened
  SELECT result;
END;", false, new Version(5, 6));
    }

    [Fact]
    public void Compound__2_56()
    {
      Utility.ParseSql(
        @"GET DIAGNOSTICS CONDITION 1
@p3 = SCHEMA_NAME, @p4 = TABLE_NAME;", false, new Version(5, 6));
    }

    [Fact]
    public void Compound_3_56()
    {
      Utility.ParseSql(
        @"GET DIAGNOSTICS CONDITION 1
    @p1 = RETURNED_SQLSTATE, @p2 = MESSAGE_TEXT;", false, new Version(5, 6));
    }

    [Fact]
    public void Compound_4_56()
    {
      Utility.ParseSql(@"GET DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", false, new Version(5, 6));
    }

    [Fact]
    public void Stacked_56()
    {
      Utility.ParseSql(@"GET STACKED DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", true, new Version(5, 6));
    }

    [Fact]
    public void Stacked_57()
    {
      Utility.ParseSql(@"GET STACKED DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", false, new Version(5, 7));
    }
  }
}
