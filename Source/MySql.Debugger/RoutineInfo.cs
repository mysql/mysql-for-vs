using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Debugger
{
  /// <summary>
  /// Meta info for routines (stored procedures, functions, triggers).
  /// </summary>
  public class RoutineInfo
  {
    public string Schema { get; set; }
    public string Name { get; set; }

    private string _sourceCode;
    public string SourceCode
    {
      get { return _sourceCode; }
      set
      {
        if (value != null)
          _hash = Debugger.GetTagHashCode(value);
        else
          _hash = "".GetHashCode();
        _sourceCode = value;
      }
    }

    private int _hash;
    public int Hash { get { return _hash; } }

    internal string InstrumentedSourceCode { get; set; }
    public RoutineInfoType Type { get; set; }
    internal CommonTokenStream TokenStream { get; set; }
    internal CommonTree ParsedTree { get; set; }
    internal string PreInstrumentationCode { get; set; }
    internal string PostInstrumentationCode { get; set; }
    internal bool _endOfDeclare { get; set; }
    internal string _leaveLabel { get; set; }
    internal Dictionary<string, StoreType> Locals { get; set; }
    private Dictionary<int, ITree> _statementsPerLine = new Dictionary<int, ITree>();

    internal void RegisterStatement(ITree tree)
    {
      ITree t;
      int line = TokenStream.Get(tree.TokenStartIndex).Line;
      if (!_statementsPerLine.TryGetValue(line, out t))
      {
        _statementsPerLine.Add(line, tree);
      }
    }

    internal bool HasLineValidStatement(int line)
    {
      ITree t;
      return _statementsPerLine.TryGetValue(line, out t);
    }

    public string FullName
    {
      get
      {
        if (string.IsNullOrEmpty(Schema))
          return Name;
        else
          return string.Format("{0}.{1}", Schema, Name);
      }
    }

    internal RoutineInfo()
    {
    }
  }
}
