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

namespace MySql.Parser.Tests.DDL.Alter
{
  public class AlterUser
  {
    [Fact]
    public void Simple()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.7/en/alter-user.html
      //Utility.ParseSql(@"ALTER USER 'jeffrey'@'localhost' PASSWORD EXPIRE;", false, new Version(5, 7, 12));
    }

    [Fact]
    public void Simple55()
    {
      string result = Utility.ParseSql(@"ALTER USER 'jeffrey'@'localhost' PASSWORD EXPIRE;", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("This syntax is only allowed for server versions starting with 5.6.6. The current version is 5.5.0", StringComparison.InvariantCultureIgnoreCase) != -1);
    }

    [Fact]
    public void AlterUser_IfExists_5_6()
    {
      string sql = @"ALTER USER IF EXISTS 'jeffrey'@'localhost' PASSWORD EXPIRE;";
      Utility.ParseSql(sql, true, new Version(5, 6, 0));
    }

    [Fact]
    public void AlterUser_IfExists_5_7()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.7/en/alter-user.html
      //string sql = @"ALTER USER IF EXISTS 'jeffrey'@'localhost' PASSWORD EXPIRE;";
      //Utility.ParseSql(sql, false, new Version(5, 7, 12));
    }
  }
}
