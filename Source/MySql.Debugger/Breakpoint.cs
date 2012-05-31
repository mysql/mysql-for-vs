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
    public bool Disabled { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public CommonTree Condition { get; set; }
    public bool IsFake { get; set; }
    public int Hash { get; set; }

    public bool EvalCondition()
    {
      // TODO: Eval Condition
      throw new NotImplementedException();
    }

    public Breakpoint()
    {
      IsFake = false;
      Disabled = false;
    }
  }
}
