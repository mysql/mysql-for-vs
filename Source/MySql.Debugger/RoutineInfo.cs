// Copyright © 2004, 2012, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
