// Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Xunit;

namespace MySql.Parser.Tests
{
  
  public class Call
  {
    [Fact]
    public void CallSimple()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp", false, out sb);
    }

    [Fact]
    public void CallSimple2()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp()", false, out sb);
    }

    [Fact]
    public void CallWithArgs()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp( 4 + 3, 'myname', (( 3.1416 * 20 ) + 7 ) / 2.71)", false, out sb);
    }

    [Fact]
    public void CallWrong()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call ", true, out sb);
    }
  }
}
