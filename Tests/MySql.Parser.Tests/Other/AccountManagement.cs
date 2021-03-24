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
  
  public class AccountManagement
  {
    [Fact]
    public void CreateUser1()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost' IDENTIFIED BY 'mypass';";
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser2()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost';";
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser3()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689';";
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser4()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689', 'me'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689';";
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser()
    {
      // Test all auth plugins.
      ParseSqlFor80(@"CREATE USER IF NOT EXISTS 'rootnative'@'localhost'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH mysql_native_password BY 'guidev!'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH sha256_password BY 'guidev!'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH caching_sha2_password BY 'guidev!'");
      Assert.Throws<Xunit.Sdk.EqualException>(() => ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH caching_sha2_passwords BY 'guidev!'"));

      // Other allowed syntax.
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED BY 'guidev!'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED BY PASSWORD 'guidev!'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH caching_sha2_password");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH mysql_native_password BY 'guidev!'");
      ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED WITH mysql_native_password AS 'guidev!'");

      // Non-allowed syntax.
      Assert.Throws<Xunit.Sdk.EqualException>(() => ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' IDENTIFIED BY caching_sha2_passwords WITH 'guidev!'"));
      Assert.Throws<Xunit.Sdk.EqualException>(() => ParseSqlFor80(@"CREATE USER 'rootnative'@'localhost' WITH caching_sha2_passwords"));

      // Variants.
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' IDENTIFIED BY 'new_password' PASSWORD EXPIRE DEFAULT");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' IDENTIFIED BY 'new_password' PASSWORD EXPIRE");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' IDENTIFIED WITH sha256_password BY 'new_password' PASSWORD EXPIRE INTERVAL 180 DAY");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' IDENTIFIED WITH mysql_native_password BY 'new_password1', 'jeanne'@'localhost' IDENTIFIED WITH sha256_password BY 'new_password2' REQUIRE X509 WITH MAX_QUERIES_PER_HOUR 60 ACCOUNT LOCK");
      ParseSqlFor80(@"CREATE USER 'joe'@'10.0.0.1' DEFAULT ROLE 'administrator', 'developer'");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' REQUIRE NONE");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' REQUIRE SSL");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' REQUIRE ISSUER '/C=SE/ST=Stockholm/L=Stockholm/O=MySQL/CN=CA/emailAddress=ca@example.com'");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' REQUIRE CIPHER 'EDH-RSA-DES-CBC3-SHA'");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' REQUIRE SUBJECT '/C=SE/ST=Stockholm/L=Stockholm/O=MySQL demo client certificate/CN=client/emailAddress=client@example.com' AND ISSUER '/C=SE/ST=Stockholm/L=Stockholm/O=MySQL/CN=CA/emailAddress=ca@example.com' AND CIPHER 'EDH-RSA-DES-CBC3-SHA'");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' WITH MAX_QUERIES_PER_HOUR 500 MAX_UPDATES_PER_HOUR 100");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' PASSWORD HISTORY 6");
      ParseSqlFor80(@"CREATE USER 'jeffrey'@'localhost' PASSWORD REUSE INTERVAL 360 DAY");
    }

    public AstParserRuleReturnScope<object, IToken> ParseSqlFor80(string sql)
    {
      return Utility.ParseSql(sql, false, new Version(8,0));
    }
    
    [Fact]
    public void DropUser()
    {
      string sql = @"DROP USER 'jeffrey'@'localhost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void DropUser2()
    {
      string sql = @"DROP USER 'jeffrey'@'localhost', 'me'@'localhost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant()
    {
      string sql = @"GRANT ALL ON db1.* TO 'jeffrey'@'localhost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant2()
    {
      string sql = @"GRANT SELECT ON db2.invoice TO 'jeffrey'@'localhost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant3()
    {
      string sql = @"GRANT USAGE ON *.* TO 'jeffrey'@'localhost' WITH MAX_QUERIES_PER_HOUR 90;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant4()
    {
      string sql = @"GRANT ALL ON *.* TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant5()
    {
      string sql = @"GRANT SELECT, INSERT ON *.* TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant6()
    {
      string sql = @"GRANT ALL ON mydb.* TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant7()
    {
      string sql = @"GRANT SELECT, INSERT ON mydb.* TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant8()
    {
      string sql = @"GRANT SELECT (col1), INSERT (col1,col2) ON mydb.mytbl TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant9()
    {
      string sql = @"GRANT CREATE ROUTINE ON mydb.* TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant10()
    {
      string sql = @"GRANT EXECUTE ON PROCEDURE mydb.myproc TO 'someuser'@'somehost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant11()
    {
      string sql = @"GRANT ALL ON test.* TO ''@'localhost'";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant12()
    {
      string sql = @"GRANT USAGE ON *.* TO ''@'localhost' WITH MAX_QUERIES_PER_HOUR 500 MAX_UPDATES_PER_HOUR 100;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant13()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost' IDENTIFIED BY 'goodsecret' REQUIRE SSL;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant14()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret' REQUIRE X509;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant15()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE ISSUER '/C=FI/ST=Some-State/L=Helsinki/
    O=MySQL Finland AB/CN=Tonu Samuel/emailAddress=tonu@example.com';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant16()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE SUBJECT '/C=EE/ST=Some-State/L=Tallinn/
    O=MySQL demo client certificate/
    CN=Tonu Samuel/emailAddress=tonu@example.com';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant17()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE CIPHER 'EDH-RSA-DES-CBC3-SHA';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant18()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE SUBJECT '/C=EE/ST=Some-State/L=Tallinn/O=MySQL demo client certificate/
    CN=Tonu Samuel/emailAddress=tonu@example.com'
  AND ISSUER '/C=FI/ST=Some-State/L=Helsinki/O=MySQL Finland AB/CN=Tonu Samuel/emailAddress=tonu@example.com'
  AND CIPHER 'EDH-RSA-DES-CBC3-SHA';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant19()
    {
      string sql = @"GRANT REPLICATION CLIENT ON *.* TO 'user'@'10.10.10.%'";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Grant20()
    {
      string sql = @"GRANT USAGE ON *.* TO 'bob'@'%.loc.gov' IDENTIFIED BY 'newpass';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void GrantProxy51()
    {
      string sql = @"GRANT PROXY ON 'localuser'@'localhost' TO 'externaluser'@'somehost';";
      StringBuilder sb;
      Utility.ParseSql(sql, true, out sb, new Version( 5, 1 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'PROXY'") != -1);
    }

    [Fact]
    public void GrantProxy55()
    {
      string sql = @"GRANT PROXY ON 'localuser'@'localhost' TO 'externaluser'@'somehost';";
      Utility.ParseSql(sql, false, new Version( 5, 5 ));
    }

    [Fact]
    public void Rename()
    {
      string sql = "RENAME USER 'jeffrey'@'localhost' TO 'jeff'@'127.0.0.1';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Revoke()
    {
      string sql = "REVOKE INSERT ON *.* FROM 'jeffrey'@'localhost';";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Revoke2()
    {
      string sql = "REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'jeffrey'@'localhost', 'jeff'@'127.0.0.1', 'me'@'localhost'";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void RevokeProxy51()
    {
      string sql = "REVOKE PROXY ON 'jeffrey'@'localhost' FROM 'jeff'@'127.0.0.1', 'me'@'localhost'";
      StringBuilder sb;
      Utility.ParseSql(sql, true, out sb, new Version( 5, 1 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'PROXY'") != -1);
    }

    [Fact]
    public void RevokeProxy55()
    {
      string sql = "REVOKE PROXY ON 'jeffrey'@'localhost' FROM 'jeff'@'127.0.0.1', 'me'@'localhost'";
      Utility.ParseSql(sql, false, new Version( 5, 5 ));
    }

    [Fact]
    public void SetPassword()
    {
      string sql = "SET PASSWORD FOR 'bob'@'%.loc.gov' = PASSWORD('newpass');";
      Utility.ParseSql(sql, false);
    }
  }
}
