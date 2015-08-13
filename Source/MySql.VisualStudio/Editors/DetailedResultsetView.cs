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

using Microsoft.VisualStudio.PlatformUI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// DetailedResultsetView control class
  /// </summary>
  public partial class DetailedResultsetView : UserControl
  {
    #region PrivateFields
    /// <summary>
    /// Key used to set and get the performance schema configuration query in a queries dictionary
    /// </summary>
    private readonly string _perfSchemaKey = "perfSchemaConfig";

    /// <summary>
    /// Key used to set and get the field types query in a queries dictionary
    /// </summary>
    private readonly string _fieldTypeKey = "fieldTypes";

    /// <summary>
    /// Key used to set and get the query statistic query in a queries dictionary
    /// </summary>
    private readonly string _queryStatKey = "queryStats";

    /// <summary>
    /// Key used to set and get the execution plan query in a queries dictionary
    /// </summary>
    private readonly string _formatJsonKey = "formatJson";

    /// <summary>
    /// Key used to set and get the original query given in a queries dictionary
    /// </summary>
    private readonly string _queryKey = "baseQuery";

    /// <summary>
    /// Dictionary used to store the queries that are executed in the database which are generated after a query is received
    /// </summary>
    private Dictionary<string, string> _queries;
    #endregion

    #region Properties
    /// <summary>
    /// This property stores the query used to configure the Performance_Schema database
    /// </summary>
    private string BasePerfSchemaConfig
    {
      get;
      set;
    }

    /// <summary>
    /// This property stores the query used to get the field types information
    /// </summary>
    private string BaseFieldTypeQuery
    {
      get;
      set;
    }

    /// <summary>
    /// This property stores the query used to get the query statistic information
    /// </summary>
    private string BaseQueryStatsQuery
    {
      get;
      set;
    }

    /// <summary>
    /// This property stores the query used to get the execution plan in json format
    /// </summary>
    private string BaseQueryFormatJson
    {
      get;
      set;
    }

    /// <summary>
    /// This property stores the MySqlConnection object used to execute the queries
    /// </summary>
    internal MySqlConnection Connection
    {
      private set;
      get;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the DetailedResultsetView class.
    /// </summary>
    public DetailedResultsetView()
    {
      InitializeComponent();
      VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
      Controls.SetColors();
    }

    /// <summary>
    /// Set colors to match the selected visual studio theme.
    /// </summary>
    /// <param name="e">The <see cref="ThemeChangedEventArgs"/> instance containing the event data.</param>
    void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
    {
      Controls.SetColors();
    }

    /// <summary>
    /// Set the query that will be used to generate the information views
    /// </summary>
    /// <param name="connection">MySqlConnection to execute the query</param>
    /// <param name="query">Query that will be executed</param>
    public void SetQuery(MySqlConnection connection, string query)
    {
      if (!string.IsNullOrEmpty(query))
      {
        if (!query.Contains(";"))
        {
          query += ";";
        }
        Connection = connection;
        GenerateQueryBatch(query);
      }
    }

    /// <summary>
    /// Generates the queries that will be executed in the database basis on the original query received
    /// </summary>
    /// <param name="baseQuery">Original query</param>
    private void GenerateQueryBatch(string baseQuery)
    {
      _queries = new Dictionary<string, string>();
      _queries.Add(_perfSchemaKey, BasePerfSchemaConfig);
      _queries.Add(_queryKey, baseQuery);

      var columns = GetColumnsFromQuery(baseQuery);
      _queries.Add(_fieldTypeKey, string.Format(BaseFieldTypeQuery, columns, string.IsNullOrEmpty(columns) ? string.Empty : " and ", GetTablesFromQuery(baseQuery)));

      _queries.Add(_queryStatKey, string.Format(BaseQueryStatsQuery, baseQuery.Substring(0, baseQuery.LastIndexOf(';')).Trim().Replace("'", "''")));
      _queries.Add(_formatJsonKey, string.Format(BaseQueryFormatJson, baseQuery));

      LoadData();
    }

    /// <summary>
    /// Executes the queries in the database and the data returned is loaded in its corresponding view
    /// </summary>
    private void LoadData()
    {
      DataTable result = new DataTable("result");
      DataTable fieldTypes = new DataTable("fieldTypes");
      DataTable queryStats = new DataTable("queryStats");
      string execPlan = "";
      bool closeConn = false;


      if (Connection.State != ConnectionState.Open)
      {
        Connection.Open();
        closeConn = true;
      }

      var tran = Connection.BeginTransaction();
      using (var cmd = new MySqlCommand())
      {
        cmd.Connection = Connection;
        cmd.Transaction = tran;
        try
        {
          cmd.CommandText = _queries[_perfSchemaKey];
          cmd.ExecuteNonQuery();

          cmd.CommandText = _queries[_queryKey];
          result.Load(cmd.ExecuteReader());

          cmd.CommandText = _queries[_fieldTypeKey];
          fieldTypes.Load(cmd.ExecuteReader());

          cmd.CommandText = _queries[_queryStatKey];
          queryStats.Load(cmd.ExecuteReader());

          cmd.CommandText = _queries[_formatJsonKey];
          var reader = cmd.ExecuteReader();
          reader.Read();
          execPlan = reader[0].ToString();
          reader.Close();

          tran.Commit();
        }
        catch (Exception ex)
        {
          tran.Rollback();
          throw ex;
        }
        finally
        {
          if (closeConn)
          {
            Connection.Close();
          }
        }
      }

      ctrlResultSet.SetData(result);
      ctrlFieldtypes.SetData(fieldTypes);
      ctrlExecPlan.SetData(execPlan);
      ctrlQueryStats.SetData(queryStats);
    }

    /// <summary>
    /// Extracts the tables contained in the original query
    /// </summary>
    /// <param name="query">Original query</param>
    /// <returns>Tables separated by comma</returns>
    private string GetTablesFromQuery(string query)
    {
      query = query.ToLower();
      var result = new StringBuilder();

      string tablesSubstr = "";
      if (query.Contains(" where "))
      {
        tablesSubstr = query.Substring(query.IndexOf("from") + 4, query.IndexOf("where") - (query.IndexOf("from") + 4));
      }
      else
      {
        tablesSubstr = query.Substring(query.IndexOf("from") + 4, query.IndexOf(";") - (query.IndexOf("from") + 4)).Trim();
      }

      result.Append(" `table_name` in (");
      var tables = tablesSubstr.Split(new string[] { "join" }, StringSplitOptions.None);
      for (int ctr = 0; ctr < tables.Count(); ctr++)
      {
        result.Append(string.Format("'{0}'", tables[ctr].TrimStart().Split(' ')[0].Trim()));

        if (ctr + 1 < tables.Length)
        {
          result.Append(", ");
        }
      }

      result.Append(")");
      return result.ToString();
    }

    /// <summary>
    /// Extracts the columns contained in the original query
    /// </summary>
    /// <param name="query">Original query</param>
    /// <returns>Columns separated by comma</returns>
    private string GetColumnsFromQuery(string query)
    {
      query = query.ToLower();
      var result = new StringBuilder();
      var colsSubstr = query.Substring(query.IndexOf("select") + 6, query.IndexOf("from") - (query.IndexOf("select") + 6));

      if (!colsSubstr.Contains("*"))
      {
        result.Append("column_name in (");

        var columns = colsSubstr.Split(',');
        for (int ctr = 0; ctr < columns.Length; ctr++)
        {
          var col = columns[ctr];
          if (!col.Contains("."))
          {
            result.Append(string.Format("'{0}'", col.Trim()));
          }
          else
          {
            result.Append(string.Format("'{0}'", col.Substring(col.IndexOf(".") + 1).Trim()));
          }
          if (ctr + 1 < columns.Length)
          {
            result.Append(", ");
          }
        }
        result.Append(")");
        return result.ToString();
      }
      return string.Empty;
    }

    /// <summary>
    /// Loads strings from the resources files and store it in local properties
    /// </summary>
    private void LoadResources()
    {
      ComponentResourceManager resources = new ComponentResourceManager(typeof(DetailedResultsetView));
      BasePerfSchemaConfig = resources.GetString("basePerfSchemaConfig");
      BaseFieldTypeQuery = resources.GetString("baseFieldTypeQuery");
      BaseQueryStatsQuery = resources.GetString("baseQueryStatsQuery");
      BaseQueryFormatJson = resources.GetString("baseFormatJson");
    }

    /// <summary>
    /// Create the list of buttons that will be loaded in the vertical menu control
    /// </summary>
    private void ConfigureMenu()
    {
      List<VerticalMenuButton> buttons = new List<VerticalMenuButton>() {
        new VerticalMenuButton() {
                                    ButtonText = "Result\nGrid",
                                    Name = "btnResultGrid",
                                    ToolTip = "Query Result",
                                    ImageToLoad = ImageType.Resultset,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.ResultSet); } },
        new VerticalMenuButton() {
                                    ButtonText = "Field\nTypes",
                                    Name = "btnFieldTypes",
                                    ToolTip = "Field Types",
                                    ImageToLoad = ImageType.FieldType,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.FieldTypes); } },
        new VerticalMenuButton() {
                                    ButtonText = "Execution\nPlan",
                                    Name = "btnExecPlan",
                                    ToolTip = "Text Execution Plan",
                                    ImageToLoad = ImageType.ExecutionPlan,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.ExecutionPlan); } },
        new VerticalMenuButton() {
                                    ButtonText = "Query\nStats",
                                    Name = "btnQueryStats",
                                    ToolTip = "Query Stats",
                                    ImageToLoad = ImageType.QueryStats,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.Querystats); } }
      };

      ctrlMenu.ConfigureControl(buttons);
    }

    /// <summary>
    /// Choose wich information view will be shown to the user basis in the enum option given
    /// </summary>
    /// <param name="controlToShow">Pane that will be displayed</param>
    private void ShowControl(DataViewOption controlToShow)
    {
      ctrlResultSet.Visible = (controlToShow == DataViewOption.ResultSet);
      ctrlFieldtypes.Visible = (controlToShow == DataViewOption.FieldTypes);
      ctrlExecPlan.Visible = (controlToShow == DataViewOption.ExecutionPlan);
      ctrlQueryStats.Visible = (controlToShow == DataViewOption.Querystats);
    }

    private void DetailedResultsetView_Load(object sender, EventArgs e)
    {
      LoadResources();
      ConfigureMenu();
    }
  }
}
