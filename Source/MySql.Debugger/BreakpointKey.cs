using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public class BreakpointKey : IEquatable<BreakpointKey>
  {
    public int Line { get; set; }
    public int Hash { get; set; }
    public string Tag
    {
      get { return string.Empty; }
      set
      {
        if (value == null)
          value = string.Empty;
        Hash = Debugger.GetTagHashCode(value);
      }
    }

    bool IEquatable<BreakpointKey>.Equals(BreakpointKey other)
    {
      if (other == null) return false;
      return (other.Hash == this.Hash) && (other.Line == this.Line);
    }
  }
}
