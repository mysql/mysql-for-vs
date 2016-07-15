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

namespace MySql.Parser.Tests.DDL.Drop
{
  public class DropTable
  {
    [Fact]
    public void SimpleNoSchema()
    {
      Utility.ParseSql("DROP TABLE `tablename`");
    }

    [Fact]
    public void SimpleWithSchema()
    {
      Utility.ParseSql("DROP TABLE `schema1`.`tablename`");
    }

    [Fact]
    public void MissingTableName()
    {
      Utility.ParseSql("DROP TABLE", true);
    }

    [Fact]
    public void MultipleTables()
    {
      Utility.ParseSql("DROP TABLE `table1`, schema2.table2, `schema3`.`table3`, table4");
    }

    [Fact]
    public void IfExists()
    {
      Utility.ParseSql("DROP TABLE IF EXISTS `tablename`");
    }

    [Fact]
    public void Temporary()
    {
      Utility.ParseSql("DROP TEMPORARY TABLE IF EXISTS `tablename`");
    }

    [Fact]
    public void CascadeOrRestrict()
    {
      Utility.ParseSql("DROP TABLE IF EXISTS `tablename` CASCADE");
      Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT");
    }

    [Fact]
    public void CascadeAndRestrict()
    {
      Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT CASCADE", true);
    }
  }
}
