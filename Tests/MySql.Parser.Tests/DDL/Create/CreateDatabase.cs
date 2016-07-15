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

namespace MySql.Parser.Tests.DDL.Create
{

  public class CreateDatabase
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql("CREATE DATABASE dbname");
    }

    [Fact]
    public void SimpleSchema()
    {
      Utility.ParseSql("CREATE SCHEMA dbname");
    }

    [Fact]
    public void MissingDbName()
    {
      Utility.ParseSql("CREATE DATABASE", true);
    }

    [Fact]
    public void IfNotExists()
    {
      Utility.ParseSql("CREATE DATABASE IF NOT EXISTS `dbname`");
    }

    [Fact]
    public void CharacterSet()
    {
      Utility.ParseSql("CREATE DATABASE `dbname` CHARACTER SET 'utf8'");
      Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT CHARACTER SET = 'bku'");
    }

    [Fact]
    public void Collation()
    {
      Utility.ParseSql("CREATE DATABASE `dbname` COLLATE 'utf8_bin'");
      Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT COLLATE = 'bku'");
    }

    [Fact]
    public void CharSetWithCollation()
    {
      Utility.ParseSql("CREATE DATABASE `dbname` CHARACTER SET 'utf8' COLLATE 'utf8_bin'");
      Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT CHARACTER SET 'utf8_bin' DEFAULT COLLATE 'bku'");
    }
  }
}