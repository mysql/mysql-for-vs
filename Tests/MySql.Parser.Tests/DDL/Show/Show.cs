// Copyright © 2013, 2016, Oracle and/or its affiliates. All rights reserved.
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
using Xunit;


namespace MySql.Parser.Tests.DDL.Show
{
  public class Show
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql("SHOW AUTHORS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW CONTRIBUTORS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW EVENTS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW PROFILES", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW PRIVILEGES", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW SLAVE HOSTS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW SLAVE STATUS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW INNODB STATUS", false, new Version(5, 0, 0));
      Utility.ParseSql("SHOW MASTER STATUS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW ENGINES", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW BINARY LOGS", false, new Version(5, 6, 0));
      Utility.ParseSql("SHOW MASTER LOGS", false, new Version(5, 6, 0));
    }

    [Fact]
    public void ShowRoutines()
    {
      Utility.ParseSql("SHOW FUNCTION CODE `myfunc`");
      Utility.ParseSql("SHOW PROCEDURE CODE `myproc`");
      //TODO: implement tests for these
      //SHOW FUNCTION STATUS [like_or_where]
      //SHOW PROCEDURE STATUS [like_or_where]
    }

    [Fact]
    public void ShowGrants()
    {
      Utility.ParseSql("SHOW GRANTS FOR 'userx'");
      Utility.ParseSql("show grants for current_user");
      Utility.ParseSql("show grants for current_user()");
    }

    // TODO: implement tests for these
    //SHOW OPEN TABLES [FROM db_name] [like_or_where]
    //SHOW TABLE STATUS [FROM db_name] [like_or_where]
    //SHOW TRIGGERS [FROM db_name] [like_or_where]

    [Fact]
    public void ShowWarningCount()
    {
      string sql = @"SHOW COUNT(*) WARNINGS";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void ShowFull()
    {
      Utility.ParseSql("SHOW PROCESSLIST");
      Utility.ParseSql("SHOW FULL PROCESSLIST");
    }

    [Fact]
    public void ShowCharacterSet()
    {
      Utility.ParseSql("SHOW CHARACTER SET");
      Utility.ParseSql("SHOW CHARACTER SET LIKE '%utf8%'");
      Utility.ParseSql("SHOW CHARACTER SET WHERE colname LIKE '%utf8%'");
    }

    [Fact]
    public void ShowCollation()
    {
      Utility.ParseSql("SHOW COLLATION");
      Utility.ParseSql("SHOW COLLATION LIKE '%utf8%'");
      Utility.ParseSql("SHOW COLLATION WHERE colname LIKE '%utf8%'");
    }

    [Fact]
    public void ShowDatabase()
    {
      Utility.ParseSql("SHOW DATABASES");
      Utility.ParseSql("SHOW DATABASES LIKE '%utf8%'");
      Utility.ParseSql("SHOW DATABASES WHERE colname LIKE '%utf8%'");
    }

    [Fact]
    public void BadShow()
    {
      Utility.ParseSql("SHOW BAD", true);
    }

    [Fact]
    public void Profile1()
    {
      Utility.ParseSql("show profiles", false);
    }

    [Fact]
    public void Profile2()
    {
      Utility.ParseSql("show profile", false);
    }

    [Fact]
    public void Profile3()
    {
      Utility.ParseSql("show profile for query 1", false);
    }

    [Fact]
    public void Profile4()
    {
      Utility.ParseSql("show profiles cpu for query 2", true);
    }

    [Fact]
    public void ShowInnodbStatus()
    {
      string result = Utility.ParseSql("show innodb status", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("unexpected 'innodb' (identifier)", StringComparison.InvariantCultureIgnoreCase) != -1);
    }

    [Fact]
    public void ShowInnodbStatus2()
    {
      Utility.ParseSql("show innodb status", false, new Version(5, 0, 0));
    }

    [Fact]
    public void ShowPlugins50()
    {
      string result = Utility.ParseSql("show plugins", true, new Version(5, 0, 0));
      Assert.True(result.IndexOf("'plugins' (plugins) is not valid input here", StringComparison.InvariantCultureIgnoreCase) != -1);
    }

    [Fact]
    public void ShowPlugins()
    {
      Utility.ParseSql("show plugins", false, new Version(5, 5, 0));
    }
  }
}
