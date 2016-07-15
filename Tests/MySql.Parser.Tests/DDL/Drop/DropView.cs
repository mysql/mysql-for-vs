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

  public class DropView
  {
    [Fact]
    public void SimpleNoSchema()
    {
      Utility.ParseSql("DROP VIEW `viewname`");
    }

    [Fact]
    public void SimpleWithSchema()
    {
      Utility.ParseSql("DROP VIEW `schema1`.`viewname`");
    }

    [Fact]
    public void MissingViewName()
    {
      Utility.ParseSql("DROP VIEW", true);
    }

    [Fact]
    public void MultipleViews()
    {
      Utility.ParseSql("DROP VIEW `view1`, schema2.view2, `schema3`.`view3`, view4");
    }

    [Fact]
    public void IfExists()
    {
      Utility.ParseSql("DROP VIEW IF EXISTS `viewname`");
    }

    [Fact]
    public void CascadeOrRestrict()
    {
      Utility.ParseSql("DROP VIEW IF EXISTS `viewname` CASCADE");
      Utility.ParseSql("DROP VIEW IF EXISTS `viewname` RESTRICT");
      Utility.ParseSql("DROP VIEW IF EXISTS `viewname` RESTRICT CASCADE", true);
    }
  }
}
