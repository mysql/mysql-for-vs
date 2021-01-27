// Copyright (c) 2014, 2021, Oracle and/or its affiliates.
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace MySql.Debugger.Tests
{
  public class TestUtils
  {
    public static readonly string CONNECTION_STRING = "server=localhost;User Id=root;password=test;database=test;Port=3306;Allow User Variables=true;Pooling=false;Allow Zero DateTime=true;";
    public static readonly string CONNECTION_STRING_SAKILA = "server=localhost;User Id=root;password=test;database=sakila;Port=3306;Allow User Variables=true;Pooling=false;Allow Zero DateTime=true;";
    public static readonly string CONNECTION_STRING_WITHOUT_DB = "server=localhost;User Id=root;password=test;Port=3306;Allow User Variables=true;Pooling=false;Allow Zero DateTime=true;";
  }

  public class SteppingTraceInfo
  {
    public SteppingTraceInfo(string RoutineName, int Line, int Column)
    {
      this.RoutineName = RoutineName;
      this.Line = Line;
      this.Column = Column;
    }

    public string RoutineName { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
  }

  public class SteppingTraceInfoList : List<SteppingTraceInfo>
  {
    private int i = 0;

    public SteppingTraceInfoList(SteppingTraceInfo[] a)
    {
      this.AddRange(a);
    }

    public void AssertBreakpoint(Breakpoint bp)
    {
      SteppingTraceInfo sti = this[i++];
      Assert.Equal(sti.RoutineName, bp.RoutineName);
      Assert.Equal(sti.Line, bp.Line);
      Assert.Equal(sti.Column, bp.StartColumn);
    }

    public void AssertFinal()
    {
      Assert.Equal(this.Count, i);
    }
  }
}
