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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// QueryStatsView control class
  /// </summary>
  public partial class QueryStatsView : UserControl
  {
    /// <summary>
    /// Message that will be displayed when the Server version is not supported
    /// </summary>
    private const string _infoNotAvailableMsg = "This information is not supported on MySql Server versions lower that 5.6";

    /// <summary>
    /// Initializes a new instance of the QueryStatsView class.
    /// </summary>
    public QueryStatsView()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Loads the data received into the control
    /// </summary>
    /// <param name="data">The data to load</param>
    /// <param name="currentServerVer">Server version of the current MySqlConnection</param>
    internal void SetData(DataTable data, ServerVersion currentServerVer)
    {
      if (data == null)
      {
        return;
      }

      if ((int)currentServerVer < 56)
      {
        lblInfoNotAvailable.Text = _infoNotAvailableMsg;
        DisplayLabelInformation(true);
      }
      else
      {
        bsQueryStatsData.DataSource = data;
        DisplayLabelInformation(false);
      }

      AddDataBindings();
    }

    /// <summary>
    /// Configure the data binding to the labels used to display data
    /// </summary>
    private void AddDataBindings()
    {
      //This bind the Text property of a label to a data member of the bsQueryStatsData datasource
      lblServerExecutionTimeVal.DataBindings.Add("Text", bsQueryStatsData, "server_execution_time");
      lblLockTimeVal.DataBindings.Add("Text", bsQueryStatsData, "lock_time");
      lblErrorsVal.DataBindings.Add("Text", bsQueryStatsData, "errors");
      lblWarningsVal.DataBindings.Add("Text", bsQueryStatsData, "warnings");
      lblRowsAffectedVal.DataBindings.Add("Text", bsQueryStatsData, "rows_affected");
      lblRowsSentVal.DataBindings.Add("Text", bsQueryStatsData, "rows_sent");
      lblRowsExaminedVal.DataBindings.Add("Text", bsQueryStatsData, "rows_examined");
      lblTempDiskTablesVal.DataBindings.Add("Text", bsQueryStatsData, "created_tmp_disk_tables");
      lblTempTablesVal.DataBindings.Add("Text", bsQueryStatsData, "created_tmp_tables");
      lblSelectScanVal.DataBindings.Add("Text", bsQueryStatsData, "select_scan");
      lblSelectFullJoinVal.DataBindings.Add("Text", bsQueryStatsData, "select_full_join");
      lblSelectFullRangeJoinVal.DataBindings.Add("Text", bsQueryStatsData, "select_full_range_join");
      lblSelectRangeCheckVal.DataBindings.Add("Text", bsQueryStatsData, "select_range_check");
      lblSelectRangeVal.DataBindings.Add("Text", bsQueryStatsData, "select_range");
      lblSortRowsVal.DataBindings.Add("Text", bsQueryStatsData, "sort_rows");
      lblSortMergePassesVal.DataBindings.Add("Text", bsQueryStatsData, "sort_merge_passes");
      lblSortRangeVal.DataBindings.Add("Text", bsQueryStatsData, "sort_range");
      lblSortScanVal.DataBindings.Add("Text", bsQueryStatsData, "sort_scan");
      lblIndexUsedVal.DataBindings.Add("Text", bsQueryStatsData, "index_used");
      lblEventIdVal.DataBindings.Add("Text", bsQueryStatsData, "event_id");
      lblThreadIdVal.DataBindings.Add("Text", bsQueryStatsData, "thread_id");
    }

    /// <summary>
    /// Display the proper information basis on the Server version
    /// </summary>
    /// <param name="versionNotSupported">The current server version is supported?</param>
    private void DisplayLabelInformation(bool versionNotSupported)
    {
      this.Controls.OfType<Label>().ToList().ForEach(label => label.Visible = label.Visible ^ versionNotSupported);
    }
  }
}
