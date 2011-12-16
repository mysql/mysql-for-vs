using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MySql.Data.VisualStudio.LanguageService
{
  internal class MySqlQuickInfoSource : IQuickInfoSource
  {
    private MySqlQuickInfoSourceProvider m_provider;
    private ITextBuffer m_subjectBuffer;
    private Dictionary<string, string> m_dictionary;

    public MySqlQuickInfoSource(MySqlQuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
    {
      m_provider = provider;
      m_subjectBuffer = subjectBuffer;

      //these are the method names and their descriptions
      //m_dictionary = new Dictionary<string, string>();
      //m_dictionary.Add("add", "int add(int firstInt, int secondInt)\nAdds one integer to another.");
      //m_dictionary.Add("subtract", "int subtract(int firstInt, int secondInt)\nSubtracts one integer from another.");
      //m_dictionary.Add("multiply", "int multiply(int firstInt, int secondInt)\nMultiplies one integer by another.");
      //m_dictionary.Add("divide", "int divide(int firstInt, int secondInt)\nDivides one integer by another.");
    }

    private void LoadDictionary( DbConnection con )
    {
      string sql = @"
        select table_name as object_name, 'table' as type from information_schema.tables where table_schema = '{0}'
        union all
        select table_name as object_name, 'view' as type from information_schema.views where table_schema = '{0}'
        union all
        select routine_name as object_name, routine_type as type from information_schema.routines where routine_schema = '{0}';
        ";
      DbCommand cmd = con.CreateCommand();
      cmd.CommandText = string.Format(sql, con.Database);
      m_dictionary = new Dictionary<string, string>();
      DbDataReader r = cmd.ExecuteReader();
      while (r.Read())
      {
        m_dictionary.Add( r.GetString( 0 ), r.GetString( 1 ).ToLower() + " " + r.GetString( 0 ));
      }
      r.Close();
    }

    public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
    {

      applicableToSpan = null;
      if (m_dictionary == null)
      {
        DbConnection connection =
          MySql.Data.VisualStudio.Editors.EditorBroker.Broker.GetCurrentConnection();
        if (connection == null || string.IsNullOrEmpty( connection.Database )) return;
        LoadDictionary(connection);
      }
      // Map the trigger point down to our buffer.
      SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(m_subjectBuffer.CurrentSnapshot);
      if (!subjectTriggerPoint.HasValue)
      {
        applicableToSpan = null;
        return;
      }

      ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
      SnapshotSpan querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);

      //look for occurrences of our QuickInfo words in the span
      ITextStructureNavigator navigator =
        m_provider.NavigatorService.GetTextStructureNavigator(m_subjectBuffer);
      TextExtent extent = navigator.GetExtentOfWord(subjectTriggerPoint.Value);
      string searchText = extent.Span.GetText();

      foreach (string key in m_dictionary.Keys)
      {
        //int foundIndex = searchText.IndexOf(key, StringComparison.CurrentCultureIgnoreCase);
        //if (foundIndex > -1)
        if( key == searchText )
        {
          int foundIndex = 0;
          int span = querySpan.Start.Add(foundIndex).Position;
          applicableToSpan = currentSnapshot.CreateTrackingSpan
              (
              span, 
              Math.Min( 
                span + currentSnapshot.Length - subjectTriggerPoint.Value.Position,
                currentSnapshot.Length - span ),
              /*currentSnapshot.Length - subjectTriggerPoint.Value.Position,*/ SpanTrackingMode.EdgeInclusive
              );

          string value;
          m_dictionary.TryGetValue(key, out value);
          if (value != null)
            qiContent.Add(value);
          else
            qiContent.Add("");

          return;
        }
      }

      applicableToSpan = null;
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
