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

using Xunit;

namespace MySql.Parser.Tests.Other
{
  public class RenameTable
  {
    [Fact]
    public void SimpleNoSchema()
    {
      Utility.ParseSql("RENAME TABLE `table1` TO `table2`");
    }

    [Fact]
    public void SimpleWithSchema()
    {
      Utility.ParseSql(
        "RENAME TABLE `schema1`.`table1` TO `schema2`.`table2`");
    }

    [Fact]
    public void MissingFromTableName()
    {
      Utility.ParseSql("RENAME TABLE", true);
    }

    [Fact]
    public void MissingToTableName()
    {
      Utility.ParseSql("RENAME TABLE table1 TO", true);
    }

    [Fact]
    public void MultipleRenames()
    {
      Utility.ParseSql(
        @"RENAME TABLE table1 TO table2, schema1.table4 TO table5,
				`schema3`.table6 TO `schema7`.table8");
    }
  }
}
