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
  public class AD7DebugExpressionContext : IDebugExpressionContext2
  {
    private AD7StackFrame _stackFrame;

    public AD7DebugExpressionContext( AD7StackFrame stackFrame )
    {
      _stackFrame = stackFrame;
    }

    int IDebugExpressionContext2.GetName(out string pbstrName)
    {
      pbstrName = _stackFrame.Node.FileName;
      return VSConstants.S_OK;
    }

    int IDebugExpressionContext2.ParseText(
      string pszCode, enum_PARSEFLAGS dwFlags, uint nRadix, 
      out IDebugExpression2 ppExpr, 
      out string pbstrError, out uint pichError)
    {
      pbstrError = null;
      ppExpr = null;
      pichError = 0;
      try
      {
        AD7DebugExpression expr = new AD7DebugExpression(_stackFrame, pszCode);
        bool success = DebuggerManager.Instance.Debugger.TryParseExpression(pszCode, out pbstrError);
        if (success)
          ppExpr = (IDebugExpression2)expr;
      }
      catch (Exception e)
      {
        pbstrError = e.Message;
        return VSConstants.E_FAIL;
      }
      return VSConstants.S_OK;
    }
  }
}
