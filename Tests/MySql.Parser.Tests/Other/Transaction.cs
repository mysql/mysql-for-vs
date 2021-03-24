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
  
  public class Transaction
  {
    [Fact]
    public void TransactionSimpleUsage()
    {
      string sql = @"START TRANSACTION;
SELECT @A:=SUM(salary) FROM table1 WHERE type=1;
UPDATE table2 SET summary=@A WHERE type=1;
COMMIT;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetAutocommit()
    {
      string sql = "set autocommit = 1";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetAutocommit2()
    {
      string sql = "set autocommit = 0";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetAutocommitWrong()
    {
      string sql = "set autocommit = 2";
      Utility.ParseSql(sql, true);
    }

    [Fact]
    public void SetLevel()
    {
      string sql = "set transaction isolation level read uncommitted";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetLevel2()
    {
      string sql = "set global transaction isolation level read committed";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetLevel3()
    {
      string sql = "set session transaction isolation level repeatable read";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SetLevel4()
    {
      string sql = "set transaction isolation level serializable";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt()
    {
      string sql = "start transaction with consistent snapshot";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt0()
    {
      string sql = "begin work";
      StringBuilder sb;
      Utility.ParseSqlQuery(sql, false, out sb);
    }

    [Fact]
    public void TransactionStmt2()
    {
      string sql = "commit work and no chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt3()
    {
      string sql = "commit and no chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt4()
    {
      string sql = "commit no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt5()
    {
      string sql = "commit work release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt6()
    {
      string sql = "commit and chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt7()
    {
      string sql = "rollback work and no chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt8()
    {
      string sql = "rollback work and no chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt9()
    {
      string sql = "rollback and no chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt10()
    {
      string sql = "rollback no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt11()
    {
      string sql = "rollback work release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void TransactionStmt12()
    {
      string sql = "rollback and chain no release";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SavePoint()
    {
      string sql = "SAVEPOINT a";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SavePoint2()
    {
      string sql = "ROLLBACK TO x";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SavePoint3()
    {
      string sql = "RELEASE SAVEPOINT yy";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void SavePoint4()
    {
      string sql = "ROLLBACK WORK TO SAVEPOINT identifier";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void LockTables()
    {
      string sql = "LOCK TABLES t1 READ;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void LockTables2()
    {
      string sql = "LOCK TABLES t WRITE, t AS t1 READ;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void LockTables3()
    {
      string sql = "LOCK tables t low_priority WRITE, t AS t1 READ, t2 read;";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void LockTables4()
    {
      string sql = "unlock tables";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa()
    {
      string sql = "xa start 'fdfdf' join";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa2()
    {
      string sql = "xa begin 'fdfdf' resume";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa3()
    {
      string sql = "xa start 'fdfdf' resume";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa4()
    {
      string sql = "xa end b'0111','',5";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa5()
    {
      string sql = "xa end b'0111','',5 suspend";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa6()
    {
      string sql = "xa end b'0111','',5 suspend for migrate";
      Utility.ParseSql(sql, false);
    }    

    [Fact]
    public void Xa7()
    {
      string sql = "xa prepare b'0111','',5 ";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa8()
    {
      string sql = "xa commit b'1010','',5";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa9()
    {
      string sql = "xa commit b'1010','',5 one phase";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa10()
    {
      string sql = " xa rollback '1-a00640d:c09d:4ac454ef:b284c0','a00640d:c09d:4ac454ef:b284c2',131075";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void Xa11()
    {
      string sql = "xa recover";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void StartTransactionReadOnly_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"start transaction read only;", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("read") != -1);
    }

    [Fact]
    public void StartTransactionReadOnly_56()
    {
      Utility.ParseSql(
        @"start transaction read only;", false, new Version(5, 6));
    }

    [Fact]
    public void StartTransactionReaWrite_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"start transaction read write;", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("read") != -1);
    }

    [Fact]
    public void StartTransactionReadWrite_56()
    {
      Utility.ParseSql(
        @"start transaction read write;", false, new Version(5, 6));
    }

    [Fact]
    public void LockInstanceForBackup()
    {
      Utility.ParseSql(
        @"LOCK INSTANCE FOR BACKUP", false, new Version(8, 0, 3));
      Utility.ParseSql(
        @"LOCK INSTANCE FOR BACKUP", true, new Version(8, 0, 2));
    }

    [Fact]
    public void UnlockInstance()
    {
      Utility.ParseSql(
        @"UNLOCK INSTANCE", false, new Version(8, 0, 3));
      Utility.ParseSql(
        @"UNLOCK INSTANCE", true, new Version(8, 0, 2));
    }
  }
}
