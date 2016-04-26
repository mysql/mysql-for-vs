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
    #region Constants

    /// <summary>
    /// Key used to set and get the execution plan query in a queries dictionary
    /// </summary>
    private const string EXECUTION_PLAN_KEY = "execPlan";

    /// <summary>
    /// Key used to set and get the field types query in a queries dictionary
    /// </summary>
    private const string FIELD_TYPE_KEY = "fieldTypes";

    /// <summary>
    /// Key used to set and get the performance schema configuration query in a queries dictionary
    /// </summary>
    private const string PERFORMANCE_SCHEMA_KEY = "perfSchemaConfig";

    /// <summary>
    /// Key used to set and get the original query given in a queries dictionary
    /// </summary>
    private const string QUERY_KEY = "baseQuery";

    /// <summary>
    /// Key used to set and get the query statistic query in a queries dictionary
    /// </summary>
    private const string QUERY_STATISTICS_KEY = "queryStats";

    #endregion Constants

    #region Fields

    /// <summary>
    /// This property stores the query used to configure the Performance_Schema database
    /// </summary>
    private string _basePerformanceSchemaConfigurationQuery;

    /// <summary>
    /// This property stores the query used to get the field types information
    /// </summary>
    private string _baseFieldTypeQuery;

    /// <summary>
    /// This property stores the query used to get the query statistic information
    /// </summary>
    private string _baseQueryStatisticsQuery;

    /// <summary>
    /// This property stores the query used to get the execution plan in json format
    /// </summary>
    private string _baseExecutionPlanQuery;

    /// <summary>
    /// This property stores the MySqlConnection object used to execute the queries
    /// </summary>
    internal MySqlConnection Connection;

    /// <summary>
    /// Stores the Server version used in the current MySqlConnection
    /// </summary>
    private int _currentServerVersion;

    /// <summary>
    /// Dictionary used to store the queries that are executed in the database which are generated after a query is received
    /// </summary>
    private Dictionary<string, string> _queries;
    
    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the DetailedResultsetView class.
    /// </summary>
    public DetailedResultsetView()
    {
      InitializeComponent();
#if !VS_SDK_2010
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
#endif
    }

    /// <summary>
    /// Set the query that will be used to generate the information views
    /// </summary>
    /// <param name="connection">MySqlConnection to execute the query</param>
    /// <param name="query">Query that will be executed</param>
    public void SetQuery(MySqlConnection connection, string query)
    {
      if (string.IsNullOrEmpty(query))
      {
        return;
      }

      if (!query.Contains(";"))
      {
        query += ";";
      }

      Connection = connection;
      ValidateServerVersion();
      LoadResources();
      GenerateQueryBatch(query);
    }

    /// <summary>
    /// Generates the queries that will be executed in the database basis on the original query received
    /// </summary>
    /// <param name="baseQuery">Original query</param>
    private void GenerateQueryBatch(string baseQuery)
    {
      if (string.IsNullOrEmpty(baseQuery))
      {
        return;
      }

      _queries = new Dictionary<string, string>();
      _queries.Add(QUERY_KEY, baseQuery);
      var columns = GetColumnsFromQuery(baseQuery);
      if (!string.IsNullOrEmpty(columns))
      {
        if (columns == "*")
        {
          columns = string.Empty;
        }

        var tables = GetTablesFromQuery(baseQuery);
        _queries.Add(FIELD_TYPE_KEY, string.Format(_baseFieldTypeQuery, columns, string.IsNullOrEmpty(columns) ? string.Empty : " and ", tables));
        _queries.Add(EXECUTION_PLAN_KEY, string.Format(_baseExecutionPlanQuery, baseQuery));
      }

      if (_currentServerVersion > 55)
      {
        _queries.Add(PERFORMANCE_SCHEMA_KEY, _basePerformanceSchemaConfigurationQuery);
        _queries.Add(QUERY_STATISTICS_KEY, string.Format(_baseQueryStatisticsQuery, baseQuery.Substring(0, baseQuery.LastIndexOf(';')).Trim().Replace("'", "''")));
      }

      LoadData();
    }

    /// <summary>
    /// Executes the queries in the database and the data returned is loaded in its corresponding view
    /// </summary>
    private void LoadData()
    {
      DataTable resultDataTable = new DataTable();
      DataTable fieldTypesDataTable = new DataTable();
      DataTable queryStatisticsDataTable = new DataTable();
      string executionPlanJsonData = "";
      DataTable executionPlanDataTable = new DataTable();
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
          if (_queries.ContainsKey(PERFORMANCE_SCHEMA_KEY) && !string.IsNullOrEmpty(_queries[PERFORMANCE_SCHEMA_KEY]))
          {
            cmd.CommandText = _queries[PERFORMANCE_SCHEMA_KEY];
            cmd.ExecuteNonQuery();
          }

          cmd.CommandText = _queries[QUERY_KEY];
          resultDataTable.Load(cmd.ExecuteReader());

          if (_queries.ContainsKey(FIELD_TYPE_KEY) && !string.IsNullOrEmpty(_queries[FIELD_TYPE_KEY]))
          {
            cmd.CommandText = _queries[FIELD_TYPE_KEY];
            fieldTypesDataTable.Load(cmd.ExecuteReader());
          }

          if (_queries.ContainsKey(QUERY_STATISTICS_KEY) && !string.IsNullOrEmpty(_queries[QUERY_STATISTICS_KEY]))
          {
            cmd.CommandText = _queries[QUERY_STATISTICS_KEY];
            queryStatisticsDataTable.Load(cmd.ExecuteReader());
          }

          if (_queries.ContainsKey(EXECUTION_PLAN_KEY) && !string.IsNullOrEmpty(_queries[EXECUTION_PLAN_KEY]))
          {
            if (_currentServerVersion > 55)
            {
              cmd.CommandText = _queries[EXECUTION_PLAN_KEY];
              var reader = cmd.ExecuteReader();
              reader.Read();
              executionPlanJsonData = reader[0].ToString();
              reader.Close();
            }
            else
            {
              cmd.CommandText = _queries[EXECUTION_PLAN_KEY];
              executionPlanDataTable.Load(cmd.ExecuteReader());
            }
          }

          tran.Commit();
        }
        catch (Exception ex)
        {
          tran.Rollback();
          Utils.WriteToOutputWindow(string.Format("Error trying to load the data: {0}", ex), MessageType.Error);
        }
        finally
        {
          if (closeConn)
          {
            Connection.Close();
          }
        }
      }

      ctrlResultSet.SetData(resultDataTable);
      ctrlFieldtypes.SetData(fieldTypesDataTable);

      if (_currentServerVersion > 55)
      {
        ctrlExecPlan.SetData(executionPlanJsonData);
      }
      else
      {
        ctrlExecPlan.SetData(executionPlanDataTable);
      }

      ctrlQueryStats.SetData(queryStatisticsDataTable, (ServerVersion)_currentServerVersion);
    }

    /// <summary>
    /// Extracts the tables contained in the original query
    /// </summary>
    /// <param name="query">Original query</param>
    /// <returns>Tables separated by comma</returns>
    private string GetTablesFromQuery(string query)
    {
      if (string.IsNullOrEmpty(query))
      {
        return string.Empty;
      }

      query = query.ToLower().Replace("`", string.Empty);
      var result = new StringBuilder();
      string whereOrSemicolon = query.Contains(" where ") ? "where" : ";";
      int fromIndex = query.IndexOf("from", StringComparison.Ordinal);
      var tablesSubstr = query.Substring(fromIndex + 4, query.IndexOf(whereOrSemicolon, StringComparison.Ordinal) - (fromIndex + 4));
      result.Append(" `table_name` in (");
      var tables = tablesSubstr.Split(new[] { "join" }, StringSplitOptions.None);
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
      if (string.IsNullOrEmpty(query) || !query.Contains("select"))
      {
        return string.Empty;
      }

      var selectIndex = query.IndexOf("select", StringComparison.Ordinal);
      query = query.ToLower().Replace("`", string.Empty);
      var result = new StringBuilder();
      var colsSubstr = query.Substring(selectIndex + 6, query.IndexOf("from", StringComparison.Ordinal) - (selectIndex + 6));
      if (colsSubstr.Contains("*"))
      {
        return "*";
      }

      result.Append("column_name in (");
      var columns = colsSubstr.Split(',');
      for (int ctr = 0; ctr < columns.Length; ctr++)
      {
        var col = columns[ctr];
        result.Append(!col.Contains(".")
          ? string.Format("'{0}'", col.Trim())
          : string.Format("'{0}'", col.Substring(col.IndexOf(".", StringComparison.Ordinal) + 1).Trim()));
        if (ctr + 1 < columns.Length)
        {
          result.Append(", ");
        }
      }

      result.Append(")");
      return result.ToString();
    }

    /// <summary>
    /// Loads strings from the resources files and store it in local properties
    /// </summary>
    private void LoadResources()
    {
      ComponentResourceManager resources = new ComponentResourceManager(typeof(DetailedResultsetView));
      _baseFieldTypeQuery = resources.GetString("baseFieldTypeQuery");

      if (_currentServerVersion < 56)
      {
        _baseExecutionPlanQuery = resources.GetString("baseExecPlanQuery51_55");
      }
      else
      {
        _basePerformanceSchemaConfigurationQuery = resources.GetString("basePerfSchemaConfig");
        _baseQueryStatisticsQuery = resources.GetString("baseQueryStatsQuery");
        _baseExecutionPlanQuery = resources.GetString("baseFormatJson");
      }
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
                                    ClickEvent = delegate { ShowControl(ctrlResultSet); } },
        new VerticalMenuButton() {
                                    ButtonText = "Field\nTypes",
                                    Name = "btnFieldTypes",
                                    ToolTip = "Field Types",
                                    ImageToLoad = ImageType.FieldType,
                                    ClickEvent = delegate { ShowControl(ctrlFieldtypes); } },
        new VerticalMenuButton() {
                                    ButtonText = "Execution\nPlan",
                                    Name = "btnExecPlan",
                                    ToolTip = "Text Execution Plan",
                                    ImageToLoad = ImageType.ExecutionPlan,
                                    ClickEvent = delegate { ShowControl(ctrlExecPlan); } },
        new VerticalMenuButton() {
                                    ButtonText = "Query\nStats",
                                    Name = "btnQueryStats",
                                    ToolTip = "Query Stats",
                                    ImageToLoad = ImageType.QueryStats,
                                    ClickEvent = delegate { ShowControl(ctrlQueryStats); } }
      };

      ctrlMenu.ConfigureControl(buttons);
    }

    /// <summary>
    /// Choose wich information view will be shown to the user basis in the enum option given
    /// </summary>
    /// <param name="controlToShow">Pane that will be displayed</param>
    private void ShowControl(UserControl controlToShow)
    {
      ctrlFieldtypes.Visible = (controlToShow is FieldTypesGrid);
      ctrlExecPlan.Visible = (controlToShow is ExecutionPlanView);
      ctrlQueryStats.Visible = (controlToShow is QueryStatsView);
      ctrlResultSet.Visible = (controlToShow is GridResultSet);
    }

    private void DetailedResultsetView_Load(object sender, EventArgs e)
    {
      LoadResources();
      ConfigureMenu();
    }

    /// <summary>
    /// Get the Server version from the current MySqlConnection and store it in a internal property
    /// </summary>
    private void ValidateServerVersion()
    {
      if (Connection != null)
      {
        Version serverVer = Parser.ParserUtils.GetVersion(Connection.ServerVersion);
        _currentServerVersion = (serverVer.Major * 10) + serverVer.Minor;
      }
    }
  }
}
