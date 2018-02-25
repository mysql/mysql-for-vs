// Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Debugger
{
  /// <summary>
  /// A debugger breakpoint.
  /// </summary>
  public class Breakpoint
  {
    protected Debugger _debugger { get; set; }
    public bool Disabled { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public int StartColumn { get; set; }
    public int EndColumn { get; set; }
    public int Line { get { return StartLine; } }
    public string Condition { get; protected set; }
    private object _previousConditionValue;
    public bool IsFake { get; set; }
    public int Hash { get; set; }
    public string RoutineName { get; set; }
    public EnumBreakpointConditionStyle ConditionStyle { get; protected set; }
    public uint HitCount { get; set; }
    public uint PassCount { get; protected set; }
    public EnumBreakpointPassCountStyle PassCountStyle { get; protected set; }

    public void SetCondition(string condition, EnumBreakpointConditionStyle style)
    {
      ConditionStyle = style;
      Condition = condition;
      _previousConditionValue = _debugger.Eval(condition);
    }

    public void SetPassCount(uint passCount, EnumBreakpointPassCountStyle style)
    {
      PassCount = passCount;
      PassCountStyle = style;
      HitCount = 0;
    }

    /// <summary>
    /// Returns true of the breakpoint is triggered either becuase pass count has been reached, or the condition has been evaluated to true.
    /// </summary>
    /// <returns></returns>
    public bool IsTriggered()
    {
      if (ConditionStyle != EnumBreakpointConditionStyle.None)
      {
        object newValue = _debugger.Eval(Condition);
        try
        {
          if ((ConditionStyle == EnumBreakpointConditionStyle.WhenTrue) && (Convert.ToInt32(newValue) == 1))
            return true;
          else if ((ConditionStyle == EnumBreakpointConditionStyle.WhenChanged) && (_previousConditionValue != newValue))
            return true;
        }
        finally
        {
          _previousConditionValue = newValue;
        }
      }
      else if (PassCountStyle != EnumBreakpointPassCountStyle.None)
      {
        HitCount++;
        if ((PassCountStyle == EnumBreakpointPassCountStyle.Equal) && (HitCount == PassCount))
          return true;
        else if ((PassCountStyle == EnumBreakpointPassCountStyle.EqualOrGreater) && (HitCount >= PassCount))
          return true;
        else if ((PassCountStyle == EnumBreakpointPassCountStyle.Mod) && ((PassCount % HitCount) == 0))
          return true;
      }
      else
      {
        return true;
      }
      return false;
    }

    public Breakpoint( Debugger debugger )
    {
      IsFake = false;
      Disabled = false;
      StartColumn = 0;
      EndColumn = UInt16.MaxValue;
      _debugger = debugger;
      ConditionStyle = EnumBreakpointConditionStyle.None;
      PassCountStyle = EnumBreakpointPassCountStyle.None;
    }
  }

  public enum EnumBreakpointConditionStyle : int
  {
    None = 0,
    WhenTrue = 1,
    WhenChanged = 2
  }

  public enum EnumBreakpointPassCountStyle : int
  {
    None = 0,
    Equal = 1,
    EqualOrGreater = 2,
    Mod = 3
  }
}
