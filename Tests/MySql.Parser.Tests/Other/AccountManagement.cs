// Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
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
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser2()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser3()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void CreateUser4()
    {
      string sql = @"CREATE USER 'jeffrey'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689', 'me'@'localhost'
IDENTIFIED BY PASSWORD '*90E462C37378CED12064BB3388827D2BA3A9B689';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }
    
    [Fact]
    public void DropUser()
    {
      string sql = @"DROP USER 'jeffrey'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void DropUser2()
    {
      string sql = @"DROP USER 'jeffrey'@'localhost', 'me'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant()
    {
      string sql = @"GRANT ALL ON db1.* TO 'jeffrey'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant2()
    {
      string sql = @"GRANT SELECT ON db2.invoice TO 'jeffrey'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant3()
    {
      string sql = @"GRANT USAGE ON *.* TO 'jeffrey'@'localhost' WITH MAX_QUERIES_PER_HOUR 90;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant4()
    {
      string sql = @"GRANT ALL ON *.* TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant5()
    {
      string sql = @"GRANT SELECT, INSERT ON *.* TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant6()
    {
      string sql = @"GRANT ALL ON mydb.* TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant7()
    {
      string sql = @"GRANT SELECT, INSERT ON mydb.* TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant8()
    {
      string sql = @"GRANT SELECT (col1), INSERT (col1,col2) ON mydb.mytbl TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant9()
    {
      string sql = @"GRANT CREATE ROUTINE ON mydb.* TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant10()
    {
      string sql = @"GRANT EXECUTE ON PROCEDURE mydb.myproc TO 'someuser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant11()
    {
      string sql = @"GRANT ALL ON test.* TO ''@'localhost'";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant12()
    {
      string sql = @"GRANT USAGE ON *.* TO ''@'localhost' WITH MAX_QUERIES_PER_HOUR 500 MAX_UPDATES_PER_HOUR 100;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant13()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost' IDENTIFIED BY 'goodsecret' REQUIRE SSL;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant14()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret' REQUIRE X509;";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant15()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE ISSUER '/C=FI/ST=Some-State/L=Helsinki/
    O=MySQL Finland AB/CN=Tonu Samuel/emailAddress=tonu@example.com';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant16()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE SUBJECT '/C=EE/ST=Some-State/L=Tallinn/
    O=MySQL demo client certificate/
    CN=Tonu Samuel/emailAddress=tonu@example.com';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant17()
    {
      string sql = @"GRANT ALL PRIVILEGES ON test.* TO 'root'@'localhost'
  IDENTIFIED BY 'goodsecret'
  REQUIRE CIPHER 'EDH-RSA-DES-CBC3-SHA';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
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
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant19()
    {
      string sql = @"GRANT REPLICATION CLIENT ON *.* TO 'user'@'10.10.10.%'";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Grant20()
    {
      string sql = @"GRANT USAGE ON *.* TO 'bob'@'%.loc.gov' IDENTIFIED BY 'newpass';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void GrantProxy51()
    {
      string sql = @"GRANT PROXY ON 'localuser'@'localhost' TO 'externaluser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, true, out sb, new Version( 5, 1 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'GRANT'") != -1);
    }

    [Fact]
    public void GrantProxy55()
    {
      string sql = @"GRANT PROXY ON 'localuser'@'localhost' TO 'externaluser'@'somehost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb, new Version( 5, 5 ));
    }

    [Fact]
    public void Rename()
    {
      string sql = "RENAME USER 'jeffrey'@'localhost' TO 'jeff'@'127.0.0.1';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Revoke()
    {
      string sql = "REVOKE INSERT ON *.* FROM 'jeffrey'@'localhost';";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void Revoke2()
    {
      string sql = "REVOKE ALL PRIVILEGES, GRANT OPTION FROM 'jeffrey'@'localhost', 'jeff'@'127.0.0.1', 'me'@'localhost'";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }

    [Fact]
    public void RevokeProxy51()
    {
      string sql = "REVOKE PROXY ON 'jeffrey'@'localhost' FROM 'jeff'@'127.0.0.1', 'me'@'localhost'";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, true, out sb, new Version( 5, 1 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'REVOKE'") != -1);
    }

    [Fact]
    public void RevokeProxy55()
    {
      string sql = "REVOKE PROXY ON 'jeffrey'@'localhost' FROM 'jeff'@'127.0.0.1', 'me'@'localhost'";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb, new Version( 5, 5 ));
    }

    [Fact]
    public void SetPassword()
    {
      string sql = "SET PASSWORD FOR 'bob'@'%.loc.gov' = PASSWORD('newpass');";
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql(sql, false, out sb);
    }
  }
}
