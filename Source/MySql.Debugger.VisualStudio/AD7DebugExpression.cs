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
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;
using System.Diagnostics;

namespace MySql.Debugger.VisualStudio
{
  public class AD7DebugExpression : IDebugExpression2
  {
    private string _expr;
    private AD7StackFrame _stackFrame;

    public AD7DebugExpression( AD7StackFrame stackFrame, string expr )
    {
      _expr = expr;
      _stackFrame = stackFrame;
    }

    int IDebugExpression2.Abort()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IDebugExpression2.EvaluateAsync(
      enum_EVALFLAGS dwFlags, 
      IDebugEventCallback2 pExprCallback)
    {
      // For now, no async evaluation supported.
      return VSConstants.E_NOTIMPL;
    }

    int IDebugExpression2.EvaluateSync(
      enum_EVALFLAGS dwFlags, 
      uint dwTimeout, 
      IDebugEventCallback2 pExprCallback, 
      out IDebugProperty2 ppResult)
    {
      if (MySql.Debugger.Debugger.GetTagHashCode(_stackFrame._rs.OwningRoutine.SourceCode) !=
        DebuggerManager.Instance.Debugger.CurrentScope.OwningRoutine.Hash)
      {
        // This should never happen.
        ppResult = null;
        return VSConstants.E_NOTIMPL;
      }
      AD7Property prop = new AD7Property( _expr,_stackFrame.Node, _stackFrame._rs, true );
      ppResult = prop;
      // Send evaluation complete event
      DebuggerManager.Instance._events.ExpressionEvalCompleted( 
        _stackFrame.Node, ( IDebugExpression2 )this, ( IDebugProperty2 )prop );
      return VSConstants.S_OK;
    }
  }
}
