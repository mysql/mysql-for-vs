// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.LanguageService;
using MySQL.Utility.Classes.MySQL;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class will handle the logic for the MySQL Files Editor.
  /// </summary>
  internal sealed partial class SqlEditor : BaseEditorControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEditor"/> class.
    /// </summary>
    /// <exception cref="System.Exception">MySql Data Provider is not correctly registered</exception>
    public SqlEditor()
    {
      InitializeComponent();

      Factory = MySqlClientFactory.Instance;
      if (Factory == null)
      {
        throw new Exception("MySql Data Provider is not correctly registered");
      }

      SetConnection(null, string.Empty, null);
      IsHybrid = false;
      EditorActionsToolStrip = EditorToolStrip;
      Package = MySqlDataProviderPackage.Instance;
      SetBaseEvents();
      ClearResults();
#if !VS_SDK_2010
      VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
      SetColors();
    }

    /// <summary>
    /// Responds to the event when Visual Studio theme changed.
    /// </summary>
    /// <param name="e">The <see cref="ThemeChangedEventArgs"/> instance containing the event data.</param>
    private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
    {
      SetColors();
    }

    /// <summary>
    /// Sets the colors corresponding to current Visual Studio theme.
    /// </summary>
    private void SetColors()
    {
      Controls.SetColors();
      BackColor = Utils.BackgroundColor;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEditor"/> class.
    /// </summary>
    /// <param name="sp">The sp.</param>
    /// <param name="pane">The pane.</param>
    internal SqlEditor(ServiceProvider sp, SqlEditorPane pane)
      : this()
    {
      Pane = pane;
      ServiceProvider = sp;
      CodeEditor.Init(sp, this);
      if (Package == null)
      {
        return;
      }

      SetConnection(Package.SelectedMySqlConnection, Package.SelectedMySqlConnectionName, Package.SelectedMySqlWorkbenchConnection);
    }

    /// <summary>
    /// Gets or sets the pane for the current editor.}
    /// In this case, the pane is from type <see cref="SqlEditorPane"/>.
    /// </summary>
    internal SqlEditorPane Pane { get; set; }

    #region Overrides

    /// <summary>
    /// Gets the file format list.
    /// </summary>
    /// <returns>The string with the file name and extensions for the 'Save as' dialog.</returns>
    protected override string GetFileFormatList()
    {
      return "MySQL Script Files (*.mysql)\n*.mysql\n\n";
    }
    /// <summary>
    /// Gets the document path.
    /// </summary>
    /// <returns></returns>
    public override string GetDocumentPath()
    {
      return Pane.DocumentPath;
    }

    /// <summary>
    /// Saves the file.
    /// </summary>
    /// <param name="newFileName">New name of the file.</param>
    protected override void SaveFile(string newFileName)
    {
      using (StreamWriter writer = new StreamWriter(newFileName, false))
      {
        writer.Write(CodeEditor.Text);
      }
    }

    /// <summary>
    /// Loads the file.
    /// </summary>
    /// <param name="newFileName">New name of the file.</param>
    protected override void LoadFile(string newFileName)
    {
      if (!File.Exists(newFileName)) return;
      using (StreamReader reader = new StreamReader(newFileName))
      {
        string sql = reader.ReadToEnd();
        CodeEditor.Text = sql;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
    /// </value>
    protected override bool IsDirty
    {
      get { return CodeEditor.IsDirty; }
      set { CodeEditor.IsDirty = value; }
    }

    #endregion

    /// <summary>
    /// Executes the script.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    private void ExecuteScript(string sql)
    {
      if (string.IsNullOrEmpty(sql))
      {
        return;
      }

      var script = new MySqlScript(Connection as MySqlConnection, sql);
      try
      {
        int rows = script.Execute();
        Utils.WriteToOutputWindow(string.Format("{0} row(s) affected", rows), MessageType.Information);
      }
      catch (Exception ex)
      {
        Utils.WriteToOutputWindow(ex.Message, MessageType.Error);
        MySqlSourceTrace.WriteAppErrorToLog(ex, false);
      }
    }

    /// <summary>
    /// Executes the select.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="counter">Query counter</param>
    private void ExecuteSelect(string sql, int counter)
    {
      if (string.IsNullOrEmpty(sql))
      {
        return;
      }

      try
      {
        var newResPage = Utils.CreateResultPage(counter);
        var detailedData = new DetailedResultsetView
        {
          Dock = DockStyle.Fill
        };

        bool querySuccess = detailedData.SetQuery((MySqlConnection) Connection, sql);
        newResPage.Controls.Add(detailedData);
        ResultsTabControl.TabPages.Add(newResPage);
        ResultsTabControl.Visible = querySuccess;
      }
      catch (Exception ex)
      {
        Utils.WriteToOutputWindow(ex.Message, MessageType.Error);
        MySqlSourceTrace.WriteAppErrorToLog(ex, false);
      }
      finally
      {
        CodeEditor.Dock = ResultsTabControl.Visible ? DockStyle.Top : DockStyle.Fill;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="RunScriptToolStripButton"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void RunScriptToolStripButton_Click(object sender, EventArgs e)
    {
      string sql = CodeEditor.Text.Trim();
      ClearResults();
      string[] sqlStmt = sql.BreakSqlStatements().ToArray();
      int ctr = 1;
      for (int sqlIdx = 0; sqlIdx <= sqlStmt.Length - 1; sqlIdx++)
      {
        bool isResultSet = LanguageServiceUtil.DoesStmtReturnResults(sqlStmt[sqlIdx], (MySqlConnection)Connection);
        if (isResultSet)
        {
          ExecuteSelect(sqlStmt[sqlIdx], ctr);
          ctr++;
        }
        else
        {
          ExecuteScript(sqlStmt[sqlIdx]);
        }
      }

      StoreCurrentDatabase();
    }
  }
}
