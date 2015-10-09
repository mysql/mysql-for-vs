// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Data.Common;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Collections.Generic;
using MySqlX.Shell;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class will handle the logic for the Script Files Editor.
  /// </summary>
  internal partial class MySqlHybridScriptEditor : GenericEditor
  {
    /// <summary>
    /// Gets the pane for the current editor. In this case, the pane is from type MySqlScriptEditorPane.
    /// </summary>
    internal MySqlHybridScriptEditorPane Pane { get; private set; }

    /// <summary>
    /// Variable to store the value to know if the user wants to execute the statements in the same session or not
    /// </summary>
    private SessionOption _sessionOption = SessionOption.UseSameSession;

    /// <summary>
    /// Variable used to executes the script
    /// </summary>
    private NgShellWrapper _ngWrapper;

    /// <summary>
    /// The script type.
    /// </summary>
    public ScriptType ScriptType;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlHybridScriptEditor"/> class.
    /// </summary>
    /// <exception cref="System.Exception">MySql Data Provider is not correctly registered</exception>
    public MySqlHybridScriptEditor()
    {
      InitializeComponent();
      factory = MySqlClientFactory.Instance;
      if (factory == null) throw new Exception("MySql Data Provider is not correctly registered");

      tabControl1.TabPages.Clear();
      //The tab control needs to be invisible when it has 0 tabs so the background matches the theme.
      tabControl1.Visible = false;
      ScriptType = ScriptType.JavaScript;
#if !VS_SDK_2010
      VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
      SetColors();
    }

    /// <summary>
    /// Responds to the event when Visual Studio theme changed.
    /// </summary>
    /// <param name="e">The <see cref="ThemeChangedEventArgs"/> instance containing the event data.</param>
    void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
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
    /// Initializes a new instance of the <see cref="MySqlHybridScriptEditor"/> class.
    /// </summary>
    /// <param name="sp">The service provider.</param>
    /// <param name="pane">The pane.</param>
    /// <param name="scriptType">Indicates the script type.</param>
    internal MySqlHybridScriptEditor(ServiceProvider sp, MySqlHybridScriptEditorPane pane, ScriptType scriptType = ScriptType.JavaScript)
      : this()
    {
      ScriptType = scriptType;
      Pane = pane;
      serviceProvider = sp;
      codeEditor.Init(sp, this);

      var package = MySqlDataProviderPackage.Instance;
      if (package != null)
      {
        if (package.MysqlConnectionSelected != null)
        {
          connection = package.MysqlConnectionSelected;
          if (connection.State != ConnectionState.Open)
            connection.Open();
          UpdateButtons();
        }
      }
    }

    #region Overrides

    /// <summary>
    /// Gets the file format list.
    /// </summary>
    /// <returns>The string with the file name and extensions for the 'Save as' dialog.</returns>
    protected override string GetFileFormatList()
    {
      switch (ScriptType)
      {
        case ScriptType.Sql:
          return "MySql Script Files (*.mysql)\n*.mysql\n\n";
        case ScriptType.JavaScript:
          return "MyJs Script Files (*.myjs)\n*.myjs\n\n";
        case ScriptType.Python:
          return "MyPy Script Files (*.mypy)\n*.mypy\n\n";
        default:
          return "MyJs Script Files (*.myjs)\n*.myjs\n\n";
      }
    }

    /// <summary>
    /// Gets the full document path, including file name and extension
    /// </summary>
    /// <returns>The full document path, including file name and extension</returns>
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
        writer.Write(codeEditor.Text);
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
        codeEditor.Text = sql;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    protected override bool IsDirty
    {
      get { return codeEditor.IsDirty; }
      set { codeEditor.IsDirty = value; }
    }

    #endregion

    /// <summary>
    /// Handles the Click event of the connectButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void connectButton_Click(object sender, EventArgs e)
    {
      resultsPage.Hide();
      ConnectDialog d = new ConnectDialog();
      d.Connection = Connection;
      DialogResult r = d.ShowDialog();
      if (r == DialogResult.Cancel) return;
      try
      {
        connection = d.Connection;
        UpdateButtons();
      }
      catch (MySqlException)
      {
        MessageBox.Show(
@"Error establishing the database Connection.
Check that the server is running, the database exist and the user credentials are valid.", "Error", MessageBoxButtons.OK);
      }
      finally
      {
        d.Dispose();
      }
    }

    /// <summary>
    /// Handles the Click event of the runSqlButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void runScriptButton_Click(object sender, EventArgs e)
    {
      string script = codeEditor.Text.Trim();
      tabControl1.TabPages.Clear();
      //The tab control needs to be invisible when it has 0 tabs so the background matches the theme.
      tabControl1.Visible = false;
      ExecuteScript(script);
      StoreCurrentDatabase();
    }

    /// <summary>
    /// Reads the current database from the last query executed or batch
    /// of queries.
    /// </summary>
    private void StoreCurrentDatabase()
    {
      MySqlConnection con = (MySqlConnection)Connection;
      MySqlCommand cmd = new MySqlCommand("select database();", con);
      object val = cmd.ExecuteScalar();
      if (val is DBNull) CurrentDatabase = "";
      else CurrentDatabase = (string)val;
    }

    /// <summary>
    /// Executes the script typed by the user and create a tab page for each statement that returns a resultset from the database otherwise write statement execution info to the output window
    /// </summary>
    /// <param name="script">Script to execute</param>
    private void ExecuteScript(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return;
      }

      try
      {
        switch (_sessionOption)
        {
          case SessionOption.UseSameSession:
            _ngWrapper = new NgShellWrapper(((MySqlConnection)connection).ToNgFormat(), true);
            break;
          case SessionOption.UseNewSession:
            _ngWrapper = new NgShellWrapper(((MySqlConnection)connection).ToNgFormat(), false);
            break;
        }

        List<string> statements = new List<string>();
        switch (ScriptType)
        {
          case Editors.ScriptType.JavaScript:
            statements = script.BreakJavaScriptStatements();
            break;
          case Editors.ScriptType.Python:
            statements = script.BreakPythonStatements();
            break;
        }

        List<ResultSet> results = _ngWrapper.ExecuteScript(statements.ToArray(), ScriptType);
        if (results == null)
        {
          return;
        }

        int tabCounter = 1;
        foreach (ResultSet result in results)
        {
          DocumentResultSet data = result as DocumentResultSet;
          if (data != null)
          {
            CreateResultPane(data, tabCounter);
          }
          else
          {
            TableResultSet tableResult = result as TableResultSet;
            if (tableResult != null)
            {
              CreateResultPane(_ngWrapper.TableResultToDocumentResult(tableResult), tabCounter);
            }
            else
            {
              Utils.WriteToOutputWindow(string.Format("Statement executed in {0}. Affected Rows: {1} - Warnings: {2}.", result.GetExecutionTime(), result.GetAffectedRows(), result.GetWarningCount()), Messagetype.Information);
            }
          }

          tabCounter++;
        }

        tabControl1.Visible = tabControl1.TabPages.Count > 0;
      }
      catch (Exception ex)
      {
        Utils.WriteToOutputWindow(ex.Message, Messagetype.Error);
      }
    }

    /// <summary>
    /// Creates a Tab Page for a ResultSet provided and add it to the tabs result
    /// </summary>
    /// <param name="data">Data to load</param>
    /// <param name="resultNumber">Result counter</param>
    private void CreateResultPane(DocumentResultSet data, int resultNumber)
    {
      if (data == null)
      {
        return;
      }

      TabPage newResPage = Utils.CreateResultPage(resultNumber);
      MySqlHybridScriptResultsetView resultViews = new MySqlHybridScriptResultsetView();
      resultViews.Dock = DockStyle.Fill;
      resultViews.LoadData(data);
      newResPage.Controls.Add(resultViews);
      tabControl1.TabPages.Add(newResPage);
    }

    /// <summary>
    /// Handles the Click event of the disconnectButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void disconnectButton_Click(object sender, EventArgs e)
    {
      Connection.Close();
      UpdateButtons();
    }

    /// <summary>
    /// Updates the buttons.
    /// </summary>
    private void UpdateButtons()
    {
      bool connected = Connection.State == ConnectionState.Open;
      runScriptButton.Enabled = connected;
      disconnectButton.Enabled = connected;
      connectButton.Enabled = !connected;
      serverLabel.Text = String.Format("Server: {0}",
          connected ? Connection.ServerVersion : "<none>");
      DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
      builder.ConnectionString = Connection.ConnectionString;
      userLabel.Text = String.Format("User: {0}",
          connected ? builder["userid"] as string : "<none>");
      dbLabel.Text = String.Format("Database: {0}",
          connected ? Connection.Database : "<none>");
    }

    /// <summary>
    /// Event fired when the Session Option is changed
    /// </summary>
    /// <param name="sender">Sender that calls the event (item clicked)</param>
    /// <param name="e">Event arguments</param>
    private void ToolStripMenuItemClickHandler(object sender, EventArgs e)
    {
      ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
      _sessionOption = (SessionOption)clickedItem.Tag;

      if (_ngWrapper != null)
      {
        _ngWrapper.CleanConnection();
        _ngWrapper = null;
      }

      foreach (ToolStripMenuItem item in toolStripSplitButton.DropDownItems)
      {
        item.Checked = item.Name == clickedItem.Name;
      }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }

        if (_ngWrapper != null)
        {
          _ngWrapper.CleanConnection();
        }

        if (connection.State != ConnectionState.Closed)
        {
          connection.Close();
        }
      }
      base.Dispose(disposing);
    }
  }
}
