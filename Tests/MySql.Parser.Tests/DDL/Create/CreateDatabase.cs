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

using Antlr.Runtime;
using Xunit;

namespace MySql.Parser.Tests
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
