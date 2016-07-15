// Copyright © 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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
  public class GetDiagnostics
  {
    [Fact]
    public void Simple_55()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //string result = Utility.ParseSql(@"GET DIAGNOSTICS CONDITION 1 @errno = MYSQL_ERRNO;", true, new Version(5, 5, 0));
      //Assert.True(result.IndexOf("rule savepoint_ident failed predicate: { input.LT(1).Text.ToLower() == \"savepoint\" }?",
      //  StringComparison.InvariantCultureIgnoreCase) != -1);
    }

    [Fact]
    public void Simple_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //Utility.ParseSql(@"GET DIAGNOSTICS CONDITION 1 @errno = MYSQL_ERRNO;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Simple_2_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //Utility.ParseSql(@"GET DIAGNOSTICS @cno = NUMBER;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Simple_3_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //Utility.ParseSql(@"GET DIAGNOSTICS CONDITION @cno @errno = MYSQL_ERRNO;", false, new Version(5, 6, 31));
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
END;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Compound__2_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
//      Utility.ParseSql(
//        @"GET DIAGNOSTICS CONDITION 1
//@p3 = SCHEMA_NAME, @p4 = TABLE_NAME;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Compound_3_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
    //  Utility.ParseSql(
    //    @"GET DIAGNOSTICS CONDITION 1
    //@p1 = RETURNED_SQLSTATE, @p2 = MESSAGE_TEXT;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Compound_4_56()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //Utility.ParseSql(@"GET DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void Stacked_56()
    {
      Utility.ParseSql(@"GET STACKED DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", true, new Version(5, 6, 31));
    }

    [Fact]
    public void Stacked_57()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: https://dev.mysql.com/doc/refman/5.6/en/get-diagnostics.html
      //Utility.ParseSql(@"GET STACKED DIAGNOSTICS @p1 = ROW_COUNT, @p2 = NUMBER;", false, new Version(5, 7, 12));
    }
  }
}
