using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MySql.Debugger
{
  [DebuggerNonUserCode]
  public class DebugSyntaxException : DebuggerException
  {
    public DebugSyntaxException(string message)
      : base(message)
    {
    }

    public DebugSyntaxException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
