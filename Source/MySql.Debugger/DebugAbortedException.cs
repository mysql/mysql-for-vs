using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Antlr.Runtime;

namespace MySql.Debugger
{
  [DebuggerNonUserCode]
  public class DebugAbortedException : DebuggerException
  {
    public DebugAbortedException(string message)
      : base(message)
    {
    }

    public DebugAbortedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public DebugAbortedException(IToken token)
      : base(token)
    {
    }
  }
}
