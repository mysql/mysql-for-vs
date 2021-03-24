// Copyright (c) 2021, Oracle and/or its affiliates.
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
using System;
using System.Text;
using Xunit;

namespace MySql.Parser.Tests
{
  public class Set
  {
    [Fact]
    public void SetVariable()
    {
      Utility.ParseSql("SET max_connections = 1000");
      Utility.ParseSql("SET @max_connections = 1000");
    }

    [Fact]
    public void SetGlobal()
    {
      Utility.ParseSql("SET GLOBAL max_connections = 1000");
      Utility.ParseSql("SET @@GLOBAL.max_connections = 1000");
    }

    [Fact]
    public void SetSession()
    {
      Utility.ParseSql("SET SESSION max_connections = 1000");
      Utility.ParseSql("SET @@SESSION.max_connections = 1000");
    }

    [Fact]
    public void SetPersist80()
    {
      StringBuilder sb;
      Utility.ParseSql("SET PERSIST max_connections = 1000", false, out sb, new Version(8, 0));
      Utility.ParseSql("SET @@PERSIST.max_connections = 1000", false, out sb, new Version(8, 0));
    }

    [Fact]
    public void SetPersist57OrLower()
    {
      Utility.ParseSql("SET PERSIST max_connections = 1000", true, new Version(5, 7));
      Utility.ParseSql("SET @@PERSIST.max_connections = 1000", true, new Version(5, 7));
    }

    [Fact]
    public void SetPersistOnly802()
    {
      Utility.ParseSql("SET PERSIST_ONLY max_connections = 1000", false, new Version(8, 0, 2));
      Utility.ParseSql("SET @@PERSIST_ONLY.max_connections = 1000", false, new Version(8, 0, 2));
    }

    [Fact]
    public void SetPersistOnly801OrLower()
    {
      Utility.ParseSql("SET PERSIST_ONLY max_connections = 1000", true, new Version(8, 0, 1));
      Utility.ParseSql("SET @@PERSIST_ONLY.max_connections = 1000", true, new Version(8, 0, 1));
    }
  }
}
