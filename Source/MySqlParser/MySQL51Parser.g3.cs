using Antlr.Runtime;
using System.Collections.Generic;

namespace MySql.Parser
{
	partial class MySQL51Parser
	{
		/*
		partial void EnterRule_drop_table()
		{
			return;
		}

		partial void LeaveRule_drop_table()
		{
			return;
		}*/      

      private int simple_table_ref_no_alias_existing_cnt;

      private Stack<string> Scope = new Stack<string>();
      private Dictionary<string, string> errKeywords = new Dictionary<string, string>();
      //private Stack<int> cntTablesUpdate = new Stack<int>();
      private int cntUpdateTables = 0;
      private bool multiTableDelete = false;

      partial void OnCreated()
      {
        errKeywords.Add("table_factor", "");
        errKeywords.Add("simple_table_ref_no_alias_existing", "");
        errKeywords.Add("column_name", "");
        errKeywords.Add("proc_name", "");
      }

      partial void EnterRule_drop_table()
      {
      }

      partial void EnterRule_simple_obj_ref_no_alias()
      {
      }

      partial void EnterRule_update()
      {
        cntUpdateTables = 0;
      }

      partial void EnterRule_statement_list()
      {
        Scope.Push("statement_list");        
      }

      partial void LeaveRule_statement_list()
      {
        Scope.Pop();
      }

      partial void EnterRule_expr()
      {
        Scope.Push("expr");
      }

      partial void LeaveRule_expr()
      {
        Scope.Pop();
      }

      partial void EnterRule_field_name()
      {
        Scope.Push("field_name");
      }

      partial void LeaveRule_field_name()
      {
        Scope.Pop();
      }

      partial void EnterRule_simple_table_ref_no_alias_existing()
      {
        simple_table_ref_no_alias_existing_cnt++;
      }

      partial void LeaveRule_simple_table_ref_no_alias_existing()
      {
        simple_table_ref_no_alias_existing_cnt--;
      }

      public override string GetErrorMessage(
          RecognitionException e,
          string[] tokenNames)
      {
        if (e is NoViableAltException)
        {          
          NoViableAltException nvae = (e as NoViableAltException);
          if ( errKeywords.ContainsKey( nvae.GrammarDecisionDescription ) )
          {
            /* Returns previous implementation format plus expected rule */
            return string.Format("no viable alternative at input {0}. Expected {1}.",
              this.GetTokenErrorDisplay(e.Token), (e as NoViableAltException).GrammarDecisionDescription);
          }
        }
        return base.GetErrorMessage(e, tokenNames);
      }
      /*
      public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
      {
        base.DisplayRecognitionError(tokenNames, e);
        System.Diagnostics.Debug.Write(
          (new System.Diagnostics.StackTrace(true).GetFrame(0).GetMethod().GetCustomAttributes(true)[0] as
          GrammarRuleAttribute).Name);
      }

      public override string GetErrorMessage(
          RecognitionException e,
          string[] tokenNames)
      {
        IList<string> stack = GetRuleInvocationStack(
            new System.Diagnostics.StackTrace(true)
          //e, this.getClass().getName() 
     );
        string msg = null;
        if (e is NoViableAltException)
        {
          NoViableAltException nvae = (NoViableAltException)e;
          msg = " no viable alt; token=" + e.Token +
          " (decision=" + nvae.DecisionNumber +
          " state " + nvae.StateNumber + ")" +
          " decision=<<" + nvae.GrammarDecisionDescription + ">>";
        }
        else
        {
          msg = base.GetErrorMessage(e, tokenNames);
        }
        return stack + " " + msg;
      }

      public override string GetTokenErrorDisplay(IToken t)
      {
        return t.ToString();
      }
       * */
	}
}

