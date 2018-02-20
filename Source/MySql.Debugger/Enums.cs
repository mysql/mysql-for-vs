// Copyright (c) 2004, 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public enum SteppingTypeEnum : int
  {
    None = 0,
    StepInto = 1,
    StepOver = 2,
    StepOut = 3
  };

  public enum RoutineInfoType : int
  {
    Procedure = 1,
    Function = 2,
    Trigger = 3
  }

  public enum RoutineType : int
  {
    Procedure = 1,
    Function = 2
  }

  public enum TriggerActionTiming : int
  {
    Before = 1,
    After = 2
  }

  public enum TriggerEvent : int
  {
    Insert = 1,
    Update = 2,
    Delete = 3
  }

  public enum ArgTypeEnum : int
  {
    In = 0,
    Out = 1,
    InOut = 2
  }

  public enum VarKindEnum : int
  {
    Local = 0,
    Argument = 1,
    Session = 2,
    Global = 3,
    Internal = 4
  }

  /// <summary>
  /// List of ids data rows in debugdata (as generaged in script Schema.sql).
  /// </summary>
  public enum DebugDataEnum : int
  {
    ScopeLevel = 1,
    LastInsertId = 2,
    RowCount = 3,
    NoDebugging = 4,
    FoundRows = 5
  }
}
