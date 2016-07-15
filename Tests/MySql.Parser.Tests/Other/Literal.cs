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
using Xunit;

namespace MySql.Parser.Tests.Other
{
  public class Literal
  {
    [Fact]
    public void UnderscoreCharsetAndCollate()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.7/en/charset-literal.html
      //string sql = @"SELECT _latin1'test' COLLATE utf8_bin;";
      //Utility.ParseSql(sql, false, new Version(5, 7, 12));
    }

    [Fact]
    public void MultipleStrings()
    {
      string sql = @"SELECT 'test' 'ab'";
      Utility.ParseSql(sql, false);
    }

    [Fact]
    public void MultipleStrings2()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.7/en/charset-literal.html
      //string sql = @"SELECT _latin1'test' 'ab'";
      //Utility.ParseSql(sql, false);
    }
  }
}
