using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using MySql.Parser;
using System.IO;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Data.VisualStudio
{
  internal class MySqlCompletionSource : ICompletionSource
  {
    private MySqlCompletionSourceProvider m_sourceProvider;
    private ITextBuffer m_textBuffer;
    private List<Completion> m_compList;
    private Dictionary<string, string> dicStartTokens = new Dictionary<string, string>();

    public MySqlCompletionSource(MySqlCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
    {
      m_sourceProvider = sourceProvider;
      m_textBuffer = textBuffer;
      dicStartTokens.Add("drop", "");
      dicStartTokens.Add("select", "");
      dicStartTokens.Add("update", "");
      dicStartTokens.Add("delete", "");
      dicStartTokens.Add("insert", "");
      dicStartTokens.Add("truncate", "");
      dicStartTokens.Add("rename", "");
      dicStartTokens.Add("call", "");
      dicStartTokens.Add("show", "");
      dicStartTokens.Add("create", "");
      //dicStartTokens.Add("", "");
    }

    /// <summary>
    /// Removes a token using the enhanced token stream class.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private CommonTokenStream RemoveToken(string sql, SnapshotPoint snapPos)
    {
      MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQL51Lexer lexer = new MySQL51Lexer(input);
      TokenStreamRemovable tokens = new TokenStreamRemovable(lexer);
      //CommonTokenStream tokens = new CommonTokenStream(lexer);
      IToken tr = null;
      int position = snapPos.Position;
      tokens.Fill();
      //position--;   // we want a zero-based index.
      if (!char.IsWhiteSpace(snapPos.GetChar()))
      {
        foreach (IToken t in tokens.GetTokens())
        {
          if ((t.StartIndex <= position) && (t.StopIndex >= position))
          {
            tr = t;
            break;
          }
        }
        tokens.Remove(tr);
      }
      return tokens;
    }

    //private ITree FindStmtRecursive(ITree t)
    //{
    //  ITree treeStmt = null;
    //  ITree child = null;
    //  for (int idx = 0; idx < t.ChildCount; idx++)
    //  {
    //    child = t.GetChild(idx);
    //    if ((child.Text.Equals("create", StringComparison.OrdinalIgnoreCase)) &&
    //          (child.GetChild(child.ChildCount - 1).Text.Equals("begin_stmt", StringComparison.OrdinalIgnoreCase)))
    //    {
    //      treeStmt = FindStmtRecursive(child);
    //      if (treeStmt != null)
    //      {
    //        break;
    //      }
    //    }
    //    else
    //    {
    //      if (child.TokenStartIndex == -1 || child.TokenStopIndex == -1) return null;
    //      if ((position >= tokens.Get(child.TokenStartIndex).StartIndex) &&
    //          (position <= tokens.Get(child.TokenStopIndex).StopIndex))
    //      {
    //        treeStmt = child;
    //        break;
    //      }
    //    }
    //  }
    //  return treeStmt;
    //}

    private ITree FindStmt(ITree t)
    {
      ITree treeStmt = null;      
      ITree child = null;
      for (int idx = 0; idx < t.ChildCount; idx++)
      {
        if (t.GetChild(idx).Text == "<EOF>") continue;
        child = t.GetChild(idx);
        if ( (child.Text.Equals("create", StringComparison.OrdinalIgnoreCase)) && 
              (child.GetChild( child.ChildCount - 1 ).Text.Equals( "begin_stmt", StringComparison.OrdinalIgnoreCase ) ))
        {
          treeStmt = FindStmt( child );
          if( treeStmt != null )
          {
            break;
          }
        } else {
          if (child.TokenStartIndex == -1 || child.TokenStopIndex == -1) return null;
          if ((position >= tokens.Get(child.TokenStartIndex).StartIndex) &&
              (position <= tokens.Get(child.TokenStopIndex).StopIndex))
          {
            treeStmt = child;
            break;
          }
        }
      }
      if (t.IsNil)
      {
        treeStmt = child;
      }
      return treeStmt;
    }

    private int position;
    private CommonTokenStream tokens;

    private void GetCompleteStatement(
      ITextSnapshot snapshot, SnapshotPoint snapPos, out StringBuilder sbErrors, out ITree treeStmt)
    {
      string sql = snapshot.GetText();
      position = snapPos.Position;
      tokens = RemoveToken(sql, snapPos);
      MySQL51Parser.program_return r =
        LanguageServiceUtil.ParseSql(sql, false, out sbErrors, tokens);
      ITree t = r.Tree as ITree;
      treeStmt = t;
      // locate current statement's AST    
      if (t.IsNil)
      {
        ITree tmp = FindStmt(t);
        if (tmp != null) treeStmt = tmp;
      }
    }

    private string GetCompleteStatement2(
      ITextSnapshot snapshot, SnapshotPoint snapPos, out StringBuilder sbErrors, out ITree treeStmt)
    {
      int position = snapPos.Position;
      StringBuilder sb = new StringBuilder();
      CommonTokenStream tokens;
      string sql = snapshot.GetText();
      MySQL51Parser.program_return r = 
        LanguageServiceUtil.ParseSql( sql, false, out sb, out tokens);
      sbErrors = sb;
      if (sbErrors.Length != 0)
        position--;
      StringBuilder sbTokens = new StringBuilder();
      ITree t = r.Tree as ITree;
      treeStmt = t;
      if (t.IsNil)
      {
        ITree child = null;
        for (int idx = 0; idx < t.ChildCount; idx++)
        {
          child = t.GetChild(idx);
          if (child.TokenStartIndex == -1 || child.TokenStopIndex == -1) return null;
          if ((position >= tokens.Get(child.TokenStartIndex).StartIndex) &&
              (position <= tokens.Get(child.TokenStopIndex).StopIndex))
          {
            break;
          }
        }
        treeStmt = child;
      }
      else
      {
        treeStmt = t;        
      }
      //int upperToken = (sb.Length == 0) ? treeStmt.TokenStopIndex - 1 : treeStmt.TokenStopIndex;
      string sqlOutput;
      int lastToken = treeStmt.TokenStopIndex;
      // Get last not EOF token
      while (tokens.Get(lastToken).Text == "<EOF>" && lastToken > 0)
        lastToken--;
      int len = 
        tokens.Get( treeStmt.TokenStopIndex ).StopIndex - 
        tokens.Get( treeStmt.TokenStartIndex ).StartIndex + 1;
      if (char.IsWhiteSpace(snapPos.GetChar()))
      {
        sqlOutput = sql.Substring(tokens.Get(treeStmt.TokenStartIndex).StartIndex,
          Math.Min(len, sql.Length - tokens.Get(treeStmt.TokenStartIndex).StartIndex));
      }
      else
      {
        // remove last token
        // sometimes the parser itself removes the last token.
        if ((sbErrors.Length == 0) || (tokens.Get(lastToken).StopIndex - 1 == position))
        {
          if (tokens.Get(lastToken).StopIndex - 1 == position && 
            lastToken != treeStmt.TokenStartIndex && lastToken > 0  ) lastToken--;
          int start = tokens.Get(treeStmt.TokenStartIndex).StartIndex,
            stop = tokens.Get( lastToken ).StartIndex;
          sqlOutput = sql.Substring(start, Math.Min(stop - start, sql.Length - start));
        }
        else
        {
          sqlOutput = sql.Substring(tokens.Get(treeStmt.TokenStartIndex).StartIndex, 
            Math.Min(len, sql.Length - tokens.Get(treeStmt.TokenStartIndex).StartIndex));
        }
      }
      //if ( /*treeStmt.TokenStartIndex < treeStmt.TokenStopIndex && */
      //    char.IsWhiteSpace(snapPos.GetChar()))
      //{
      //  stop = tokens.Get(treeStmt.TokenStopIndex - 1).StopIndex;
      //}
      //else
      //{
      //  stop = tokens.Get(treeStmt.TokenStopIndex).StartIndex;
      //}
      //int start = tokens.Get(treeStmt.TokenStartIndex).StartIndex;
      //  //stop = tokens.Get(upperToken).StopIndex;
      ////string sqlOutput = sql.Substring(start, Math.Min(stop - start + 1, sql.Length - start));
      
      //if (treeStmt is CommonErrorNode || sbErrors.Length != 0)
      //{
      //  sqlOutput = sql.Substring(start, sql.Length - start);
      //}
      //else
      //{
      //  sqlOutput = sql.Substring(start, Math.Min(stop - start + 1, sql.Length - start));
      //}
      treeStmt = LanguageServiceUtil.ParseSql(sqlOutput, false, out sbErrors).Tree as ITree;
      return sqlOutput;      
    }

    /*
    private string GetCompleteStatement3(
      ITextSnapshot snapshot, ITextSnapshotLine line, ref int position )
    {
      string sql;
      int lineNo = line.LineNumber;
      do
      {
        sql = snapshot.GetLineFromLineNumber(lineNo).GetTextIncludingLineBreak();
        string[] arr = sql.Split( ' ', '\r', '\n' );
        if (arr.Length < 1) continue;
        string firstWord = arr[0];
        if (dicStartTokens.ContainsKey(firstWord))
        {
          break;
        }
        lineNo--;
      } while (lineNo >= 0);
      StringBuilder sb = new StringBuilder();
      if (lineNo == -1)
      {
        sql = line.GetText();
      }
      else
      {
        int idx = lineNo;
        do
        {
          string s = snapshot.GetLineFromLineNumber(idx++).GetText();
          if( idx != line.LineNumber ) position += s.Length + 1;
          sb.Append( s );
          sb.Append(' ');
        } while ( idx <= line.LineNumber );
        sql = sb.ToString();
      }
      return sql;
    }
    */



    void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
    {
      DbConnection connection = LanguageServiceUtil.GetConnection();
      if( connection != null && !string.IsNullOrEmpty( connection.Database ) )
      //if (LanguageServiceConnection.Current.Connection != null)
      {
        //*
        //MySqlConnection connection = LanguageServiceConnection.Current.Connection as MySqlConnection;

        //ITrackingSpan span = FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer), session);
        SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
        ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
        TextExtent extent = navigator.GetExtentOfWord(currentPoint);
        ITrackingSpan span = currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        
        StringBuilder sbErrors;
        ITextSnapshot snapshot = currentPoint.Snapshot;
        ITextSnapshotLine line = currentPoint.GetContainingLine();
        int position = currentPoint.Position;
        // Get starting token
        ITree t;
        GetCompleteStatement(snapshot, currentPoint, out sbErrors, out t);
        //string sql = line.GetText();
        if( snapshot.Length == 0 ) return;        
        /*
        //int idx = sql.LastIndexOf(' ');
        //if (idx != -1)
        //{
        //  sql = sql.Substring(0, idx);
        //}
        int lidx = -1;
        // The current position normalized
        int curPosNorm = Math.Min( position - line.Start, sql.Length - 1);
        if (!char.IsWhiteSpace(currentPoint.GetChar()))
        {
          lidx = sql.LastIndexOf(' ', Math.Max( curPosNorm - 1, 0 ));
        }
        if (lidx != -1)
        {
          sql = sql.Substring(0, lidx) + sql.Substring( curPosNorm + 1 );
        }
        MySQL51Parser.statement_list_return parsedSql = LanguageServiceUtil.ParseSql(sql, false, out sbErrors );
        ITree t = ( parsedSql.Tree as ITree );
        */
        string s = sbErrors.ToString();
        Match m = new Regex(@"Expected (?<item>.*)\.").Match( s );
        string expectedToken = "";
        if( m.Success )
        {
          expectedToken = m.Groups["item"].Value;
        }
        if (expectedToken == "table_factor" || 
          expectedToken == "simple_table_ref_no_alias_existing")
        {

          m_compList = new List<Completion>();
          DataTable schema = connection.GetSchema("Tables");
          string completionItem = null, completionItemUnq = null;

          foreach (DataRow row in schema.Rows)
          {
            completionItemUnq = row["TABLE_NAME"].ToString();
            completionItem = string.Format("`{0}`", row["TABLE_NAME"].ToString());
            m_compList.Add(new Completion(completionItemUnq, completionItem, completionItem, null, null));
          }          

          completionSets.Add(new CompletionSet(
            "MySqlTokens",    //the non-localized title of the tab
            "MySQL Tokens",    //the display title of the tab
            FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer), session),
            m_compList,
            null));
        }
        if (expectedToken == "proc_name")
        //else if (s.EndsWith("no viable alternative at input '<EOF>'\r\n", 
        //  StringComparison.CurrentCultureIgnoreCase))
        {
          //if( t is CommonErrorNode )
          //{
          //  if (((CommonErrorNode)t).Text.Equals( "CALL", StringComparison.CurrentCultureIgnoreCase ) )
          //  {
              // Get a list of stored procedures
              m_compList = new List<Completion>();
              DataTable schema = connection.GetSchema("PROCEDURES WITH PARAMETERS");
              DataView vi = schema.DefaultView;
              vi.Sort = "specific_name asc";
              string completionItem = null;
              string description = null;
              foreach (DataRowView row in vi)
              {
                if ("procedure".CompareTo(row["routine_type"].ToString().ToLower()) == 0)
                {
                  completionItem = row["specific_name"].ToString();
                  description = string.Format("procedure {0}.{1}({2})",
                    row["routine_schema"], row["specific_name"], row["ParameterList"]);
                  m_compList.Add(new Completion(completionItem, completionItem,
                    description, null, null));
                }
              }

              completionSets.Add(new CompletionSet(
                "MySqlTokens",    //the non-localized title of the tab
                "MySQL Tokens",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer), session),
                m_compList,
                null));
          //  }
          //}
        }
        else if (expectedToken == "column_name")
        {
          if( ( t.ChildCount != 0 ) || 
              (( t is CommonErrorNode ) && 
               ( ( t as CommonErrorNode ).Text.Equals("SELECT", 
                StringComparison.CurrentCultureIgnoreCase) )))
          {
            List<TableWithAlias> tables = new List<TableWithAlias>();
            ParserUtils.GetTables(t, tables);
            List<string> cols = GetColumns(connection, tables);
            CreateCompletionList(cols, session, completionSets);
          }
        }
        //else if (s.EndsWith("no viable alternative at input 'FROM'\r\n", 
        //  StringComparison.CurrentCultureIgnoreCase))
        //{
        //  // if there are only syntax error in columns, we still try to extract the table info.
        //  List<TableWithAlias> tables = new List<TableWithAlias>();
        //  sql = currentPoint.GetContainingLine().GetText();
        //  Match m = new Regex(@"select (?<columns>.*) (?<from>from .*$)").Match(sql);
        //  if (m.Success)
        //  {
        //    sql = string.Format("select c {0}", m.Groups["from"].Value);
        //    parsedSql = LanguageServiceUtil.ParseSql(sql, false, out sbErrors);
        //    // if there were more syntax errors we cannot do more, so abort
        //    if (sbErrors.Length != 0) return;
        //    // Get table names
        //    ParserUtils.GetTables((ITree)parsedSql.Tree, tables);
        //    List<string> cols = GetColumns(connection, tables);
        //    CreateCompletionList(cols, session, completionSets);
        //  }
        //  else
        //  {
        //    // if not match we can still try other constructs like insert, update, etc.
        //    // (provided that they generate the same error: no viable input at 'from'... 
        //    // which won't be always the case).
        //  }
        //}
        //*/
      }
    }

    private void CreateCompletionList(
      List<string> l, ICompletionSession session, IList<CompletionSet> completionSets)
    {
      m_compList = new List<Completion>();
      foreach (string c in l)
      {
        m_compList.Add(new Completion(c.Replace( "`", "" ), c, c, null, null));
      }
      completionSets.Add(new CompletionSet(
          "MySqlTokens",    //the non-localized title of the tab
          "MySQL Tokens",    //the display title of the tab
          FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer), session),
          m_compList,
          null));
    }

    private string BuildWhereGetColumns( string database, string sql, List<TableWithAlias> tables, out bool hasDbExplicit)
    {
      StringBuilder sb = new StringBuilder();
      string tableTemp = " and table_name = '{0}' ) or ";
      string schemaTemp = " ( table_schema = '{0}'";
      string defaultSchema = string.Format(schemaTemp, database);
      hasDbExplicit = false;
      if (tables.Count != 0)
      {
        foreach (TableWithAlias table in tables)
        {
          if (string.IsNullOrEmpty(table.Database))
          {
            sb.Append(defaultSchema).Append(string.Format(tableTemp, table.TableName));
          }
          else
          {
            hasDbExplicit = true;
            sb.Append(string.Format(schemaTemp, table.Database)).Append(string.Format(tableTemp, table.TableName));
          }
        }
        sb.Length = sb.Length - 4;
      }
      else
      {
        sb.Append(defaultSchema).Append(")");
      }
      return string.Format(sql, sb.ToString());
    }

    private Dictionary<string, List<string>> BuildColumnList(DbDataReader r, bool IncludeDb )
    {      
      Dictionary<string, List<string>> dicColumns = new Dictionary<string, List<string>>();
      List<string> cols = null;
      string prevTbl = "";
      while (r.Read())
      {
        string dbName = r.GetString(0)/*.ToLower() */;
        string tableName = r.GetString(1) /*.ToLower() */;
        string colName = r.GetString(2);
        string finalTableName = IncludeDb ? string.Format("{0}.{1}", dbName, tableName) :
          string.Format("{0}", tableName);

        if (prevTbl != finalTableName)
        {
          if (!string.IsNullOrEmpty(prevTbl))
          {
            dicColumns.Add(prevTbl, cols);
          }
          cols = new List<string>();
          prevTbl = finalTableName;
        }
        cols.Add(colName);
      }
      if (!string.IsNullOrEmpty(prevTbl))
      {
        dicColumns.Add(prevTbl, cols);
      }
      return dicColumns;
    }

    private List<string> GetColumns(DbConnection con, List<TableWithAlias> tables)
    {
      DbCommand cmd = con.CreateCommand();
      // information_schema.columns is available from MySql 5.0 and up.
      string sql =
        @"select table_schema, table_name, column_name from information_schema.columns 
          where ( 1 = 1 ) and ( {0} )
          order by table_schema, table_name, column_name";
      bool hasDbExplicit;
      cmd.CommandText = BuildWhereGetColumns( con.Database, sql, tables, out hasDbExplicit);      
      Dictionary<string, List<string>> dicColumns = null;
      DbDataReader r = cmd.ExecuteReader();
      try
      {
        dicColumns = BuildColumnList(r, tables.Count != 0 );
      }
      finally
      {
        r.Close();
      }
      List<string> columns = new List<string>();
      List<string> cols = new List<string>();
      if (tables.Count != 0)
      {
        foreach (TableWithAlias ta in tables)
        {
          string key = string.Format("{0}.{1}", 
            !string.IsNullOrEmpty( ta.Database )? ta.Database : con.Database.ToLower(), ta.TableName);
          // use db only if no alias defined and db was explicitely used.
          string tblTempl = (hasDbExplicit && string.IsNullOrEmpty(ta.Alias)) ? "`{0}`.`{1}`.`{2}`" : "`{1}`.`{2}`";
          dicColumns.TryGetValue(key, out cols);
          foreach (string col in cols)
          {
            columns.Add(string.Format(tblTempl, ta.Database,
              !string.IsNullOrEmpty(ta.Alias) ? ta.Alias : ta.TableName, col));
          }
        }
      }
      else
      {
        string tblTempl = "`{0}`.`{1}`";
        foreach ( KeyValuePair<string, List<string>> kvp in dicColumns)
        {
          foreach (string s in kvp.Value)
          {
            columns.Add( string.Format( tblTempl, kvp.Key, s ));
          }
        }
      }
      return columns;
    }

    private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
    {
      SnapshotPoint? triggerPoint = session.GetTriggerPoint(m_textBuffer.CurrentSnapshot);

      ITextSnapshotLine line = triggerPoint.Value.GetContainingLine();
      SnapshotPoint start = triggerPoint.Value;

      while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
      {
        start -= 1;
      }
      ITextSnapshot snapshot = m_textBuffer.CurrentSnapshot;
      ITrackingSpan applicableTo = snapshot.CreateTrackingSpan( new SnapshotSpan(start, triggerPoint.Value), SpanTrackingMode.EdgeInclusive);
      return applicableTo;
      //SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
      //ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);      
      //TextExtent extent = navigator.GetExtentOfWord(currentPoint);
      //return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);      
    }

    private bool m_isDisposed;
    public void Dispose()
    {
      if (!m_isDisposed)
      {
        GC.SuppressFinalize(this);
        m_isDisposed = true;
      }
    }
  }
}
