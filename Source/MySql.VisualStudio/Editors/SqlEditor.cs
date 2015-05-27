// Copyright © 2008, 2015, Oracle and/or its affiliates. All rights reserved.
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
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class will handle the logic for the MySQL Files Editor.
  /// </summary>
  internal partial class SqlEditor : GenericEditor
  {
    /// <summary>
    /// Gets the pane for the current editor. In this case, the pane is from type SqlEditorPane.
    /// </summary>
    internal SqlEditorPane Pane { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEditor"/> class.
    /// </summary>
    /// <exception cref="System.Exception">MySql Data Provider is not correctly registered</exception>
    public SqlEditor()
    {
      InitializeComponent();
      factory = MySqlClientFactory.Instance;
      if (factory == null)
      {
        throw new Exception("MySql Data Provider is not correctly registered");
    }
      tabControl1.TabPages.Clear();
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
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
    /// </value>
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
      d.Connection = connection;
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
@"Error establishing the database connection.
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
    private void runSqlButton_Click(object sender, EventArgs e)
    {
      string sql = codeEditor.Text.Trim();
      tabControl1.TabPages.Clear();
      string[] sqlStmt = sql.BreakSqlStatements().ToArray();
      int ctr = 1;
      for (int sqlIdx = 0; sqlIdx <= sqlStmt.Length - 1; sqlIdx++)
      {
        bool isResultSet = LanguageServiceUtil.DoesStmtReturnResults(sqlStmt[sqlIdx], (MySqlConnection)connection);
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

    /// <summary>
    /// Reads the current database from the last query executed or batch
    /// of queries.
    /// </summary>
    private void StoreCurrentDatabase()
    {
      MySqlConnection con = (MySqlConnection)connection;
      MySqlCommand cmd = new MySqlCommand("select database();", con);
      object val = cmd.ExecuteScalar();
      if (val is DBNull) CurrentDatabase = "";
      else CurrentDatabase = (string)val;
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
        TabPage newResPage = CreateResultPage(counter);
        DetailedResultsetView detailedData = new DetailedResultsetView();
        detailedData.Dock = DockStyle.Fill;
        detailedData.SetQuery((MySqlConnection)connection, sql);
        newResPage.Controls.Add(detailedData);
        tabControl1.TabPages.Add(newResPage);
      }
      catch (Exception ex)
      {
        Utils.WriteToOutputWindow(ex.Message, Messagetype.Error);
      }
      }

    /// <summary>
    /// In DataGridView column with blob data type are by default associated with a DataGridViewImageColumn
    /// this column internally uses the System.Drawing APIs to try to load images, obviously not all blobs
    /// are images, so that fails.
    ///   The fix implemented in this function represents blobs a a fixed &lt;Blob&gt; string.
    /// </summary>
    private void SanitizeBlobs()
    {
      DataGridViewColumnCollection coll = resultsGrid.Columns;
      _isColBlob = new bool[coll.Count];
      for (int i = 0; i < coll.Count; i++)
      {
        DataGridViewColumn col = coll[i];
        DataGridViewTextBoxColumn newCol = null;
        if (!(col is DataGridViewImageColumn)) continue;
        coll.Insert(i, newCol = new DataGridViewTextBoxColumn()
        {
          DataPropertyName = col.DataPropertyName,
          HeaderText = col.HeaderText,
          ReadOnly = true
        });
        coll.Remove(col);
        _isColBlob[i] = true;
      }
    }

    /// <summary>
    /// Handles the CellFormatting event of the resultsGrid control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="DataGridViewCellFormattingEventArgs"/> instance containing the event data.</param>
    private void resultsGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.ColumnIndex == -1) return;
      if (_isColBlob[e.ColumnIndex])
      {
        if (e.Value == null || e.Value is DBNull)
          e.Value = "<NULL>";
        else
          e.Value = "<BLOB>";
      }
    }

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

      MySqlScript script = new MySqlScript((MySqlConnection)connection, sql);
      try
      {
        int rows = script.Execute();
        Utils.WriteToOutputWindow(string.Format("{0} row(s) affected", rows), Messagetype.Information);
      }
      catch (Exception ex)
      {
        Utils.WriteToOutputWindow(ex.Message, Messagetype.Error);
      }
      }

    /// <summary>
    /// Handles the Click event of the disconnectButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void disconnectButton_Click(object sender, EventArgs e)
    {
      connection.Close();
      UpdateButtons();
    }

    /// <summary>
    /// Updates the buttons.
    /// </summary>
    private void UpdateButtons()
    {
      bool connected = connection.State == ConnectionState.Open;
      runSqlButton.Enabled = connected;
      disconnectButton.Enabled = connected;
      connectButton.Enabled = !connected;
      serverLabel.Text = String.Format("Server: {0}",
          connected ? connection.ServerVersion : "<none>");
      DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
      builder.ConnectionString = connection.ConnectionString;
      userLabel.Text = String.Format("User: {0}",
          connected ? builder["userid"] as string : "<none>");
      dbLabel.Text = String.Format("Database: {0}",
          connected ? connection.Database : "<none>");
    }

    private TabPage CreateResultPage(int counter)
    {
      TabPage newResPage = new TabPage();
      newResPage.AutoScroll = true;
      newResPage.Text = string.Format("Result{0}", (counter > 0 ? counter.ToString() : ""));
      newResPage.ImageIndex = 1;
      newResPage.UseVisualStyleBackColor = true;
      newResPage.Padding = new Padding(3);
      return newResPage;
  }
  }
}
