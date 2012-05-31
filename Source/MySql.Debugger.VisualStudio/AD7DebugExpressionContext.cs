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
