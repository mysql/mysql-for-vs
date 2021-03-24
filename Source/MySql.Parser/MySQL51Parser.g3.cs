// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

//#define CSharp3Target

using System;
using Antlr.Runtime;
using System.Collections.Generic;

namespace MySql.Parser
{

  

#if CSharp3Target
  partial class MySQL51Parser
  
  public partial class MySQL51Parser
  {
  }
#else

  public class MySQLParserBase : Antlr.Runtime.Parser
    //: MySQL51Parser
#endif
  {
    protected internal Stack<BlockKind> _blockStack = new Stack<BlockKind>();

    public MySQLParserBase( ITokenStream input, RecognizerSharedState state ) : base( input, state )
    {
      // Default value.
      _mysqlVersion = "8.0.0";

      _blockStack.Push(BlockKind.None);
    }

    // Holds values like 5.6, 5.7, 8.0, etc.
    protected string _mysqlVersion;

    public Version MySqlVersion {
      get { return new Version(_mysqlVersion); }
      set { _mysqlVersion = value.ToString(); }
    }

    protected int simple_table_ref_no_alias_existing_cnt;

    protected Stack<string> Scope = new Stack<string>();
    protected static Dictionary<string, string> errKeywords = new Dictionary<string, string>();
    //private Stack<int> cntTablesUpdate = new Stack<int>();
    protected int cntUpdateTables = 0;
    protected bool multiTableDelete = false;

    static MySQLParserBase()
    {
      errKeywords.Add("table_factor", "");
      errKeywords.Add("simple_table_ref_no_alias_existing", "");
      errKeywords.Add("column_name", "");
      errKeywords.Add("proc_name", "");
    }

    protected virtual bool IsSelectSeen()
    {
      return false;
    }
  }

  public enum BlockKind
  {
    None = 0,
    BeginEnd = 1
  }

  public class MySQLParser : MySQL51Parser
  {
    public MySQLParser(ITokenStream input) : base( input )
    {
    }

#if CSharp3Target
      partial void EnterRule_declare_handler()
#else
    protected override void EnterRule_declare_handler()
#endif
    {
    }

#if CSharp3Target
      partial void EnterRule_begin_end_stmt()
#else
    protected override void EnterRule_begin_end_stmt()
#endif
    {
      _blockStack.Push( BlockKind.BeginEnd );
    }

#if CSharp3Target
      partial void LeaveRule_begin_end_stmt()
#else
    protected override void LeaveRule_begin_end_stmt()
#endif
    {
      _blockStack.Pop();
    }


#if CSharp3Target
      partial void EnterRule_drop_table()
#else
    protected override void EnterRule_drop_table()
#endif
    {
    }

#if CSharp3Target
      partial void EnterRule_simple_obj_ref_no_alias()
#else
    protected override void EnterRule_simple_obj_ref_no_alias()
#endif
    {
    }

#if CSharp3Target
      partial void EnterRule_update()
#else
    protected override void EnterRule_update()
#endif
    {
      cntUpdateTables = 0;
    }

#if CSharp3Target
      partial void EnterRule_statement_list()
#else
    protected override void EnterRule_statement_list()
#endif
    {
      Scope.Push("statement_list");
    }

#if CSharp3Target
      partial void LeaveRule_statement_list()
#else
    protected override void LeaveRule_statement_list()
#endif
    {
      Scope.Pop();
    }

#if CSharp3Target
      partial void EnterRule_expr()
#else
    protected override void EnterRule_expr()
#endif
    {
      Scope.Push("expr");
    }

#if CSharp3Target
      partial void LeaveRule_expr()
#else
    protected override void LeaveRule_expr()
#endif
    {
      Scope.Pop();
    }

#if CSharp3Target
      partial void EnterRule_field_name()
#else
    protected override void EnterRule_field_name()
#endif
    {
      Scope.Push("field_name");
    }

#if CSharp3Target
      partial void LeaveRule_field_name()
#else
    protected override void LeaveRule_field_name()
#endif
    {
      Scope.Pop();
    }

#if CSharp3Target
      partial void EnterRule_simple_table_ref_no_alias_existing()
#else
    protected override void EnterRule_simple_table_ref_no_alias_existing()
#endif
    {
      simple_table_ref_no_alias_existing_cnt++;
    }

#if CSharp3Target
      partial void LeaveRule_simple_table_ref_no_alias_existing()
#else
    protected override void LeaveRule_simple_table_ref_no_alias_existing()
#endif
    {
      simple_table_ref_no_alias_existing_cnt--;
    }

#if CSharp3Target
    partial void EnterRule_primary()
#else
    protected override void EnterRule_primary()
#endif
    {
      Scope.Push("expr");
    }

#if CSharp3Target
    partial void LeaveRule_primary()
#else
    protected override void LeaveRule_primary()
#endif
    {
      Scope.Pop();
    }

    protected override bool IsSelectSeen()
    {
      int i = 1;
      while (input.LA(i) == MySQL51Lexer.LPAREN) i++;
      return (input.LA(i) == MySQL51Lexer.SELECT);
    }

    public override string GetErrorMessage(
        RecognitionException e,
        string[] tokenNames)
    {
      if (e is NoViableAltException)
      {
        NoViableAltException nvae = (e as NoViableAltException);
        if (errKeywords.ContainsKey(nvae.GrammarDecisionDescription))
        {
          /* Returns previous implementation format plus expected rule */
          return string.Format("no viable alternative at input {0}. Expected {1}.",
            this.GetTokenErrorDisplay(e.Token), (e as NoViableAltException).GrammarDecisionDescription);
        }
      }
      return base.GetErrorMessage(e, tokenNames);
    }
  }
}

