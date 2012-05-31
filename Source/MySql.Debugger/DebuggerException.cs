using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace MySql.Debugger
{
  public class DebuggerException : ApplicationException
  {
    protected IToken _token;

    public IToken Token { get { return _token; } }

    public DebuggerException(string message)
      : base(message)
    {
    }

    public DebuggerException(string message, Exception innerException) :
      base(message, innerException)
    {
    }

    public DebuggerException(IToken token)
      : base(token.Text)
    {
      _token = token;
    }
  }
}
