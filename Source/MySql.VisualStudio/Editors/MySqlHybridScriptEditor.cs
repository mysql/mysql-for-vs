// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MySQL.Utility.Classes;
using MySql.Data.VisualStudio.MySqlX;
using MySqlX;
using System.Text;
using ConsoleTables.Core;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class will handle the logic for the Script Files Editor.
  /// </summary>
  internal partial class MySqlHybridScriptEditor : BaseEditorControl
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
    /// Variable to store the value to know if the user wants to execute the statements in batch mode or in console mode
    /// </summary>
    private ExecutionModeOption _executionModeOption = ExecutionModeOption.BatchMode;

    /// <summary>
    /// Variable used to executes the script
    /// </summary>
    private MySqlXProxy _xShellWrapper;

    /// <summary>
    /// The script type.
    /// </summary>
    public ScriptType ScriptType;

    /// <summary>
    /// Variable to verify if the query execution result is not an empty document
    /// </summary>
    private bool _resultIsNotEmptyDocument;

    /// <summary>
    /// Variable to hold the number of tabs created in the output pane
    /// </summary>
    private int _tabCounter;

    /// <summary>
    /// Constant to hold the MySqlX "Result" string type
    /// </summary>
    private const string MySqlXResultType = "mysqlx.result";

    /// <summary>
    /// Constant to hold the MySqlX "DocResult" string type
    /// </summary>
    private const string MySqlXDocResultType = "mysqlx.docresult";

    /// <summary>
    /// Constant to hold the MySqlX "RowResult" string type
    /// </summary>
    private const string MySqlXRowResultType = "mysqlx.rowresult";

    /// <summary>
    /// Constant to hold the MySqlX "SqlResult" string type
    /// </summary>
    private const string MySqlXSqlResultType = "mysqlx.sqlresult";

    /// <summary>
    /// Constant to hold the "System.String" string type
    /// </summary>
    private const string SystemStringType = "system.string";

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlHybridScriptEditor"/> class.
    /// </summary>
    /// <exception cref="System.Exception">MySql Data Provider is not correctly registered</exception>
    public MySqlHybridScriptEditor()
    {
      InitializeComponent();
      factory = MySqlClientFactory.Instance;
      if (factory == null)
      {
        throw new Exception("MySql Data Provider is not correctly registered");
      }

      tabControl1.TabPages.Clear();
      //The tab control needs to be invisible when it has 0 tabs so the background matches the theme.
      tabControl1.Visible = false;
      ScriptType = ScriptType.JavaScript;
      SetXShellConsoleEditorPromptString();
      ToggleEditors(ExecutionModeOption.BatchMode);
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
      xShellConsoleEditor1.Controls.SetColors();
      xShellConsoleEditor1.BackColor = Utils.EditorBackgroundColor;
      xShellConsoleEditor1.PromptColor = Utils.FontColor;
      xShellConsoleEditor1.ForeColor = Utils.FontColor;
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
      SetXShellConsoleEditorPromptString();
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
          {
            connection.Open();
          }

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

      // Get elapsed time for xShell results in case an exception is thrown
      Stopwatch sw = new Stopwatch();
      try
      {
        switch (_sessionOption)
        {
          case SessionOption.UseSameSession:
            if (_xShellWrapper == null)
            {
              _xShellWrapper = new MySqlXProxy(((MySqlConnection)connection).ToXFormat(), true);
            }

            break;
          case SessionOption.UseNewSession:
            _xShellWrapper = new MySqlXProxy(((MySqlConnection)connection).ToXFormat(), false);
            break;
        }

        sw = Stopwatch.StartNew();
        if (_executionModeOption == ExecutionModeOption.BatchMode)
        {
          ExecuteBatchScript(script);
        }

        if (_executionModeOption == ExecutionModeOption.ConsoleMode)
        {
          ExecuteConsoleScript(script);
        }

        sw.Stop();
      }
      catch (Exception ex)
      {
        sw.Stop();
        WriteToMySqlOutput(script, ex.Message, string.Format("{0} sec", TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).TotalSeconds), MessageType.Error);
      }
    }

    /// <summary>
    /// Executes the script in batch mode.
    /// </summary>
    /// <param name="script">The script.</param>
    private void ExecuteBatchScript(string script)
    {
      List<string> statements = new List<string>();
      switch (ScriptType)
      {
        case ScriptType.JavaScript:
          statements = script.BreakJavaScriptStatements();
          break;
        case ScriptType.Python:
          statements = script.BreakPythonStatements();
          break;
      }

      List<MySqlXResult> results = _xShellWrapper.ExecuteScript(statements.ToArray(), ScriptType);
      if (results == null)
      {
        return;
      }

      _tabCounter = 1;
      _resultIsNotEmptyDocument = false;
      int statementIndex = 0;
      foreach (MySqlXResult result in results)
      {
        PrintResult(statements[statementIndex], result.Result, result.ExecutionTime);
        statementIndex++;
        tabControl1.Visible = tabControl1.TabPages.Count > 0;
        codeEditor.Focus();
      }
    }

    /// <summary>
    /// Executes the script in console mode.
    /// </summary>
    /// <param name="script">The script.</param>
    private void ExecuteConsoleScript(string script)
    {
      MySqlXResult result = _xShellWrapper.ExecuteQuery(script, ScriptType);
      PrintResult(script, result.Result, result.ExecutionTime);
      xShellConsoleEditor1.Focus();
    }

    /// <summary>
    /// Writes to the My SQL Output Tool Window.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="messageType">Type of the message.</param>
    private void WriteToMySqlOutput(string action, string message, string duration, MessageType messageType)
    {
      Utils.WriteToMySqlOutputWindow(action, message, duration, messageType);
      if (_executionModeOption == ExecutionModeOption.ConsoleMode)
      {
        xShellConsoleEditor1.AddMessage(message);
      }
    }

    /// <summary>
    /// Prints the proper query execution result in the output window, either DocResult, or RowResult, SqlResult or Result,
    /// showing extended information about the execution result (items affected, execution time, etc.).
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="executionResult">The xShell execution result.</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    private void PrintResult(string script, Object executionResult, string duration)
    {
      string type = executionResult.GetType().ToString().ToLowerInvariant();
      switch (type)
      {
        case MySqlXResultType:
          PrintResult(script, (Result)executionResult, duration);
          break;
        case MySqlXDocResultType:
          PrintDocResult(script, (DocResult)executionResult, _executionModeOption, duration);
          break;
        case MySqlXRowResultType:
          PrintRowResult(script, (RowResult)executionResult, _executionModeOption, duration);
          break;
        case MySqlXSqlResultType:
          PrintSqlResult(script, (SqlResult)executionResult, _executionModeOption, duration);
          break;
        case SystemStringType:
          if (string.IsNullOrEmpty(executionResult.ToString()))
          {
            return;
          }

          MessageType messageType = MessageType.Information;
          if (executionResult.ToString().Contains("error", StringComparison.InvariantCultureIgnoreCase))
          {
            messageType = MessageType.Error;
          }

          WriteToMySqlOutput(script, executionResult.ToString(), duration, messageType);
          break;
      }
    }

    /// <summary>
    /// Prints the Result type query execution results in the output window
    /// showing extended information about the execution result (items affected, execution time, etc.).
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="result">The Result execution result.</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    private void PrintResult(string script, Result result, string duration)
    {
      StringBuilder resultMessage = new StringBuilder();
      resultMessage.Append("Query OK");
      long affectedItems = result.GetAffectedItemCount();
      if (affectedItems >= 0)
      {
        resultMessage.AppendFormat(", {0} items affected", affectedItems);
      }

      WriteToMySqlOutput(script, resultMessage.ToString(), string.Format("{0} / {1}", duration, result.GetExecutionTime()), MessageType.Information);
      PrintWarnings(script, result, duration);
    }

    /// <summary>
    /// Prints the warnings (if exists) of the query execution results, in the output window
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="result">The BaseResult execution result.</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    private void PrintWarnings(string script, BaseResult result, string duration)
    {
      if (result.GetWarningCount() > 0)
      {
        StringBuilder warningsMessages = new StringBuilder();
        warningsMessages.AppendFormat(" Warning Count: {0}\n", result.GetWarningCount());
        List<Dictionary<String, Object>> warnings = result.GetWarnings();
        foreach (Dictionary<String, Object> warning in warnings)
        {
          warningsMessages.AppendFormat("{0} ({1}): {2}\n", warning["Level"], warning["Code"], warning["Message"]);
        }

        WriteToMySqlOutput(script, warningsMessages.ToString(), string.Format("{0} / {1}", duration, result.GetExecutionTime()), MessageType.Warning);
      }
    }

    /// <summary>
    /// Prints the DocResult type query execution results in the output window
    /// showing extended information about the execution result (documents returned, execution time, etc.).
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="result">The DocResult execution result.</param>
    /// <param name="executionMode">The statement(s) execution mode (batch or console).</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    private void PrintDocResult(string script, DocResult result, ExecutionModeOption executionMode, string duration)
    {
      switch (executionMode)
      {
        case ExecutionModeOption.BatchMode:
          _resultIsNotEmptyDocument = result.FetchAll().Count > 0;
          if (_resultIsNotEmptyDocument)
          {
            CreateResultPane(result, _tabCounter);
            _tabCounter++;
          }

          break;
        case ExecutionModeOption.ConsoleMode:
          List<Dictionary<string, object>> data = result.FetchAll();
          foreach (Dictionary<string, object> row in data)
          {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            int i = 0;
            foreach (KeyValuePair<string, object> kvp in row)
            {
              sb.AppendFormat("\t\"{0}\" : \"{1}\"{2}\n", kvp.Key, kvp.Value, (i == row.Count - 1) ? "" : ",");
              i++;
            }

            sb.AppendLine("},");
            xShellConsoleEditor1.AddMessage(sb.ToString());
          }

          break;
      }

      WriteToMySqlOutput(script, string.Format("{0} documents in set.", result.FetchAll().Count), string.Format("{0} / {1}", duration, result.GetExecutionTime()), MessageType.Information);
      PrintWarnings(script, result, duration);
    }

    /// <summary>
    /// Prints the RowResult type query execution results in the output window
    /// showing extended information about the execution result (rows returned, execution time, etc.).
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="result">The RowResult execution result.</param>
    /// <param name="executionMode">The statement(s) execution mode (batch or console).</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    /// <param name = "affectedItems" > The number of affected items on a DML script.</param>
    private void PrintRowResult(string script, RowResult result, ExecutionModeOption executionMode, string duration, long affectedItems = 0)
    {
      switch (executionMode)
      {
        case ExecutionModeOption.BatchMode:
          var doc = _xShellWrapper.RowResultToDictionaryList(result);
          _resultIsNotEmptyDocument = doc.Count > 0;
          if (_resultIsNotEmptyDocument)
          {
            CreateResultPane(doc, _tabCounter);
            _tabCounter++;
          }

          break;
        case ExecutionModeOption.ConsoleMode:
          // Get columns names
          string[] columns = new string[result.GetColumns().Count];
          int i = 0;
          foreach (Column col in result.GetColumns())
          {
            columns[i++] += col.GetColumnName();
          }

          // Create console table object for output format
          var table = new ConsoleTable(columns);
          object[] record = result.FetchOne();
          while (record != null)
          {
            object[] columnValue = new object[result.GetColumns().Count];
            i = 0;
            foreach (object o in record)
            {
              if (o == null)
              {
                columnValue[i++] = "null";
              }
              else
              {
                columnValue[i++] = o.ToString();
              }
            }

            table.AddRow(columnValue);
            record = result.FetchOne();
          }

          if (table.Rows.Count > 0)
          {
            xShellConsoleEditor1.AddMessage(table.ToStringAlternative());
          }

          break;
      }

      StringBuilder resultMessage = new StringBuilder();
      // If no items are returned, it is a DDL statement (Drop, Create, etc.)
      int totalItems = result.FetchAll().Count;
      if (totalItems == 0)
      {
        resultMessage.AppendFormat("Query OK, {0} rows affected, {1} warning(s)", affectedItems, result.GetWarningCount());
      }
      else
      {
        resultMessage.AppendFormat("{0} rows in set.", totalItems);
      }

      WriteToMySqlOutput(script, resultMessage.ToString(), string.Format("{0} / {1}", duration, result.GetExecutionTime()), MessageType.Information);
      PrintWarnings(script, result, duration);
    }

    /// <summary>
    /// Prints the SqlResult type query execution results in the output window,
    /// showing extended information about the execution result (rows returned, execution time, etc.).
    /// </summary>
    /// <param name="script">The executed command.</param>
    /// <param name="result">The RowResult execution result.</param>
    /// <param name="executionMode">The statement(s) execution mode (batch or console).</param>
    /// <param name = "duration" > The elapsed time for xShell results that doesn't contain the "GetExecutionTime" property.</param>
    private void PrintSqlResult(string script, SqlResult result, ExecutionModeOption executionMode, string duration)
    {
      if ((bool)result.HasData())
      {
        PrintRowResult(script, result, executionMode, duration, result.GetAffectedRowCount());
      }
      else
      {
        PrintWarnings(script, result, duration);
      }
    }

    /// <summary>
    /// Creates a Tab Page for a ResultSet provided and add it to the tabs result
    /// </summary>
    /// <param name="data">Data to load</param>
    /// <param name="resultNumber">Result counter</param>
    private void CreateResultPane(List<Dictionary<string, object>> data, int resultNumber)
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
    /// Creates a Tab Page for a ResultSet provided and add it to the tabs result
    /// </summary>
    /// <param name="data">Data to load</param>
    /// <param name="resultNumber">Result counter</param>
    private void CreateResultPane(DocResult data, int resultNumber)
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
      if (_xShellWrapper != null)
      {
        _xShellWrapper.CleanConnection();
        _xShellWrapper = null;
      }

      foreach (ToolStripMenuItem item in toolStripSplitButton.DropDownItems)
      {
        item.Checked = item.Name == clickedItem.Name;
      }
    }

    /// <summary>
    /// Event fired when the execution Mode Option is changed
    /// </summary>
    /// <param name="sender">Sender that calls the event (item clicked)</param>
    /// <param name="e">Event arguments</param>
    private void ToolStripMenuItemExecutionMode_ClickHandler(object sender, EventArgs e)
    {
      ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
      _executionModeOption = (ExecutionModeOption)clickedItem.Tag;
      ToggleEditors(_executionModeOption);
      foreach (ToolStripMenuItem item in tssbExecutionModeButton.DropDownItems)
      {
        item.Checked = item.Name == clickedItem.Name;
      }
    }

    /// <summary>
    /// Toggles the editors between batch mode and console mode.
    /// </summary>
    /// <param name="executionMode">The execution mode.</param>
    private void ToggleEditors(ExecutionModeOption executionMode)
    {
      try
      {
        panel1.SuspendLayout();
        if (executionMode == ExecutionModeOption.BatchMode)
        {
          panel1.Controls.Remove(xShellConsoleEditor1);
          panel1.Controls.Add(tabControl1);
          panel1.Controls.Add(splitter1);
          // Register the code editor, to add back its handles and events
          codeEditor.RegisterEditor();
          panel1.Controls.Add(codeEditor);
          runScriptButton.Enabled = true;
          codeEditor.Focus();
        }
        else
        {
          panel1.Controls.Remove(tabControl1);
          panel1.Controls.Remove(splitter1);
          // Unregister the code editor, to remove its handles and events
          codeEditor.UnregisterEditor();
          panel1.Controls.Remove(codeEditor);
          xShellConsoleEditor1.Dock = DockStyle.Fill;
          panel1.Controls.Add(xShellConsoleEditor1);
          runScriptButton.Enabled = false;
          xShellConsoleEditor1.Focus();
        }
      }
      finally
      {
        panel1.ResumeLayout();
      }
    }

    /// <summary>
    /// Handles the Command event of the xShellConsoleEditor1 control, and execute the command received.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="XShellConsoleCommandEventArgs"/> instance containing the event data.</param>
    private void xShellConsoleEditor1_Command(object sender, XShellConsoleCommandEventArgs e)
    {
      if (e.Command == "cls")
      {
        xShellConsoleEditor1.ClearMessages();
        e.Cancel = true;
        return;
      }

      ExecuteScript(e.Command);
    }

    /// <summary>
    /// Set the XShellConsoleEditor prompt string according the ScriptType the file format list.
    /// </summary>
    private void SetXShellConsoleEditorPromptString()
    {
      switch (ScriptType)
      {
        case ScriptType.Sql:
          xShellConsoleEditor1.PromptString = "mysql-slq>";
          break;
        case ScriptType.Python:
          xShellConsoleEditor1.PromptString = "mysql-py>";
          break;
        case ScriptType.JavaScript:
        default:
          xShellConsoleEditor1.PromptString = "mysql-js>";
          break;
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

        if (_xShellWrapper != null)
        {
          _xShellWrapper.CleanConnection();
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
