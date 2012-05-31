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
      if (MySql.Debugger.Debugger.GetTagHashCode(_stackFrame.Node.ProgramContents) !=
        DebuggerManager.Instance.Debugger.CurrentScope.OwningRoutine.Hash)
      {
        // Not implemented when the selected stackframe is different than current one.
        ppResult = null;
        return VSConstants.E_NOTIMPL;
      }
      MySql.Debugger.Debugger _dbg = DebuggerManager.Instance.Debugger;
      object value = _dbg.Eval(_expr);
      AD7Property prop = new AD7Property( _expr, _dbg.FormatValue(value), _stackFrame.Node );
      ppResult = prop;
      // Send evaluation complete event
      DebuggerManager.Instance._events.ExpressionEvalCompleted( 
        _stackFrame.Node, ( IDebugExpression2 )this, ( IDebugProperty2 )prop );
      return VSConstants.S_OK;
    }
  }
}
