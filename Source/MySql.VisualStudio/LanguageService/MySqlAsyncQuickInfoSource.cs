// Copyright (c) 2012, 2021, Oracle and/or its affiliates.
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

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio.LanguageService
{
  internal class MySqlAsyncQuickInfoSource : IAsyncQuickInfoSource
  {
    private Dictionary<string, string> m_dictionary;
    private bool m_isDisposed;
    private MySqlAsyncQuickInfoSourceProvider m_provider;
    private ITextBuffer m_subjectBuffer;

    public MySqlAsyncQuickInfoSource(MySqlAsyncQuickInfoSourceProvider provider, ITextBuffer subjectBuffer)
    {
      m_provider = provider;
      m_subjectBuffer = subjectBuffer;
    }

    public void Dispose()
    {
      if (!m_isDisposed)
      {
        GC.SuppressFinalize(this);
        m_isDisposed = true;
      }
    }

    public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
    {
      // Skip for files other than the ones with .mysql extension.
      if (!m_subjectBuffer.ContentType.TypeName.Equals("MySql", StringComparison.OrdinalIgnoreCase))
      {
        return Task.FromResult<QuickInfoItem>(null);
      }

      ITrackingSpan applicableToSpan = null;
      if (m_dictionary == null)
      {
        if (MySqlQuickInfoController.Connection == null
           || string.IsNullOrEmpty(MySqlQuickInfoController.Connection.Database))
        {
          return Task.FromResult<QuickInfoItem>(null);
        }

        LoadDictionary();

        // No need to continue if no elements have been recovered from the database.
        if (m_dictionary != null
            && m_dictionary.Count == 0)
        {
          return Task.FromResult<QuickInfoItem>(null);
        }
      }

      // Map the trigger point down to our buffer.
      SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(m_subjectBuffer.CurrentSnapshot);
      if (!subjectTriggerPoint.HasValue)
      {
        applicableToSpan = null;
        return Task.FromResult<QuickInfoItem>(null);
      }

      ITextSnapshot currentSnapshot = subjectTriggerPoint.Value.Snapshot;
      SnapshotSpan querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);

      // Get hover token.
      var line = querySpan.Snapshot.GetText();
      var tokens = line.Split(' ', '`', ';');
      var searchText = string.Empty;
      var index = 0;
      foreach (var token in tokens)
      {
        index += token.Length + 1;
        if (index >= querySpan.Start.Position)
        {
          searchText = token;
          break;
        }
      }

      ContainerElement content;
      var lineSnapshot = subjectTriggerPoint.Value.GetContainingLine();
      var lineSpan = m_subjectBuffer.CurrentSnapshot.CreateTrackingSpan(
        lineSnapshot.Extent,
        SpanTrackingMode.EdgeInclusive);
      foreach (string key in m_dictionary.Keys)
      {
        if (!key.Equals(searchText, StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        int foundIndex = 0;
        int span = querySpan.Start.Add(foundIndex).Position;
        applicableToSpan = currentSnapshot.CreateTrackingSpan(
          span,
          Math.Min(
            span + currentSnapshot.Length - subjectTriggerPoint.Value.Position,
            currentSnapshot.Length - span),
            SpanTrackingMode.EdgeInclusive);

        string value;
        m_dictionary.TryGetValue(key, out value);
        if (value != null)
        {
          content = new ContainerElement(
            ContainerElementStyle.Wrapped,
            new ClassifiedTextElement(
              new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, "Element: "),
              new ClassifiedTextRun(PredefinedClassificationTypeNames.Identifier, value)));

          return Task.FromResult(
          new QuickInfoItem(
            lineSpan,
            content));
        }
      }

      return Task.FromResult<QuickInfoItem>(null);
    }

    /// <summary>
    /// Loads a dictionary containing the list of tables, views and routines that exist in the current database connection.
    /// </summary>
    private void LoadDictionary()
    {
      if (MySqlQuickInfoController.Connection.State != System.Data.ConnectionState.Open)
      {
        MySqlQuickInfoController.Connection.OpenWithDefaultTimeout();
      }

      string sql = @"
        select table_name as object_name, 'table' as type from information_schema.tables where table_schema = '{0}'
        union all
        select table_name as object_name, 'view' as type from information_schema.views where table_schema = '{0}'
        union all
        select routine_name as object_name, routine_type as type from information_schema.routines where routine_schema = '{0}';
        ";
      DbCommand cmd = MySqlQuickInfoController.Connection.CreateCommand();
      cmd.CommandText = string.Format(sql, MySqlQuickInfoController.Connection.Database);
      m_dictionary = new Dictionary<string, string>();
      DbDataReader r = cmd.ExecuteReader();
      try
      {
        while (r.Read())
        {
          string objectName = r.GetString(0);
          string type = r.GetString(1).ToLower();
          string description = type + " " + objectName;
          if (m_dictionary.ContainsKey(objectName))
          {
            if (string.Compare(type, "view", StringComparison.OrdinalIgnoreCase) == 0)
            {
              m_dictionary[objectName] = description;
            }
          }
          else
          {
            m_dictionary.Add(objectName, description);
          }
        }
      }
      finally
      {
        r.Close();
      }
    }
  }
}
