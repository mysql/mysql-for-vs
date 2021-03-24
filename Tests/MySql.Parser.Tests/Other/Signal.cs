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
  
  public class Signal
  {
    [Fact]
    public void Signal_51()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (pval INT)
BEGIN
  DECLARE specialty CONDITION FOR SQLSTATE '45000';
  IF pval = 0 THEN
    SIGNAL SQLSTATE '01000';
  ELSEIF pval = 1 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'An error occurred';
  ELSEIF pval = 2 THEN
    SIGNAL specialty
      SET MESSAGE_TEXT = 'An error occurred';
  ELSE
    SIGNAL SQLSTATE '01000'
      SET MESSAGE_TEXT = 'A warning occurred', MYSQL_ERRNO = 1000;
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'An error occurred', MYSQL_ERRNO = 1001;
  END IF;
END;", true, out sb, new Version(5, 1));
      Assert.True(sb.ToString().IndexOf("no viable alternative", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void Signal_1_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (pval INT)
BEGIN
  DECLARE specialty CONDITION FOR SQLSTATE '45000';
  IF pval = 0 THEN
    SIGNAL SQLSTATE '01000';
  ELSEIF pval = 1 THEN
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'An error occurred';
  ELSEIF pval = 2 THEN
    SIGNAL specialty
      SET MESSAGE_TEXT = 'An error occurred';
  ELSE
    SIGNAL SQLSTATE '01000'
      SET MESSAGE_TEXT = 'A warning occurred', MYSQL_ERRNO = 1000;
    SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'An error occurred', MYSQL_ERRNO = 1001;
  END IF;
END;
", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_2_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (divisor INT)
BEGIN
  IF divisor = 0 THEN
    SIGNAL SQLSTATE '22012';
  END IF;
END;", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_3_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (divisor INT)
BEGIN
  DECLARE divide_by_zero CONDITION FOR SQLSTATE '22012';
  IF divisor = 0 THEN
    SIGNAL divide_by_zero;
  END IF;
END;", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_4_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (pval INT)
BEGIN
  DECLARE no_such_table CONDITION FOR 1051;
  SIGNAL no_such_table;
END;
", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_5_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p (divisor INT)
BEGIN
  DECLARE my_error CONDITION FOR SQLSTATE '45000';
  IF divisor = 0 THEN
    BEGIN
      DECLARE my_error CONDITION FOR SQLSTATE '22012';
      SIGNAL my_error;
    END;
  END IF;
  SIGNAL my_error;
END;", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_6_55()
    {
      Utility.ParseSql(
        @"
CREATE PROCEDURE p ()
BEGIN
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    SIGNAL SQLSTATE VALUE '99999'
      SET MESSAGE_TEXT = 'An error occurred';
  END;
  DROP TABLE no_such_table;
END;", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_7_55()
    {
      Utility.ParseSql(
        @"
CREATE FUNCTION f () RETURNS INT
BEGIN
  SIGNAL SQLSTATE '01234';  -- signal a warning
  RETURN 5;
END;", false, new Version(5, 5));
    }

    [Fact]
    public void Signal_8_55()
    {
      Utility.ParseSql(
        @"
DROP TABLE IF EXISTS xx;
CREATE PROCEDURE p ()
BEGIN
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    SET @error_count = @error_count + 1;
    IF @a = 0 THEN RESIGNAL SQLSTATE '45000' SET MYSQL_ERRNO=5; END IF;
  END;
  DROP TABLE xx;
END;
SET @error_count = 0;
SET @a = 0;
SET @@max_error_count = 2;
CALL p();
SHOW ERRORS;", false, new Version(5, 6));
    }

    [Fact]
    public void Signal_9_55()
    {
      Utility.ParseSql(
        @"
CREATE FUNCTION f () RETURNS INT
BEGIN
  RESIGNAL;
  RETURN 5;
END;
CREATE PROCEDURE p ()
BEGIN
  DECLARE EXIT HANDLER FOR SQLEXCEPTION SET @a=f();
  SIGNAL SQLSTATE '55555';
END;
CALL p();", false, new Version(5, 6));
    }

    [Fact]
    public void Signal_10_55()
    {
      Utility.ParseSql(
        @"CREATE TRIGGER t_bi BEFORE INSERT ON t FOR EACH ROW RESIGNAL;", false, new Version(5, 6));
    }
  }
}
