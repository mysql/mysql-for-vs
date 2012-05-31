using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public class BreakpointKeyDictionaryComparer : IEqualityComparer<BreakpointKey>
  {

    bool IEqualityComparer<BreakpointKey>.Equals(BreakpointKey x, BreakpointKey y)
    {
      if (x == null)
      {
        if (y == null) return true;
        else return false;
      }
      else
      {
        if (y == null) return false;
        else
          return (x.Line == y.Line) && (x.Hash == y.Hash);
      }
    }

    int IEqualityComparer<BreakpointKey>.GetHashCode(BreakpointKey obj)
    {
      if (obj == null) return "".GetHashCode();
      return unchecked(obj.Hash + obj.Line);
    }
  }    
}
