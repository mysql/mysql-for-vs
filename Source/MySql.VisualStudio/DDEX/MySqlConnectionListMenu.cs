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

using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes.MySql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace MySql.Data.VisualStudio.DDEX
{
  internal class MySqlConnectionListMenu
  {
    private int _baselistID = (int)PkgCmdIDList.cmdidMRUList;
    private List<IVsDataExplorerConnection> _connectionsList;

    public MySqlConnectionListMenu(ref OleMenuCommandService mcs, List<IVsDataExplorerConnection> connList)
    {
      _connectionsList = connList;
      InitMruMenu(ref mcs);
    }

    internal void InitMruMenu(ref OleMenuCommandService mcs)
    {
      if (mcs == null)
      {
        return;
      }

      var dynamicItemRootId = new CommandID(GuidList.GuidIdeToolbarCmdSet, _baselistID);
      var dynamicMenuCommand = new DynamicItemMenuCommand(dynamicItemRootId, IsValidDynamicItem, OnInvokedDynamicItem, OnBeforeQueryStatusDynamicItem);
      mcs.AddCommand(dynamicMenuCommand);
    }

    private bool IsValidDynamicItem(int commandId)
    {
      int connectionsCount = 0;
      if (MySqlDataProviderPackage.Instance != null)
      {
        connectionsCount = MySqlDataProviderPackage.Instance.GetMySqlConnections().Count;
      }

      return ((commandId - _baselistID) < connectionsCount && (commandId - _baselistID) >= 0);
    }

    private void OnInvokedDynamicItem(object sender, EventArgs args)
    {
      var invokedCommand = sender as DynamicItemMenuCommand;
      if (invokedCommand == null)
      {
        return;
      }

      bool isRootItem = invokedCommand.MatchedCommandId == 0;
      int indexForDisplay = isRootItem ? 0 : invokedCommand.MatchedCommandId - _baselistID;
      if (MySqlDataProviderPackage.Instance == null)
      {
        return;
      }

      try
      {
        if (indexForDisplay < _connectionsList.Count)
        {
          var connection = (MySqlConnection)_connectionsList[indexForDisplay].Connection.GetLockedProviderObject();
          try
          {
            if (connection != null)
            {
              MySqlDataProviderPackage.Instance.SelectedMySqlConnection = connection;
            }

            var itemOp = MySqlDataProviderPackage.Instance.GetDTE2().ItemOperations;
            itemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
          }
          finally
          {
            _connectionsList[indexForDisplay].Connection.UnlockProviderObject();
          }
        }
        else
        {
          var itemOp = MySqlDataProviderPackage.Instance.GetDTE2().ItemOperations;
          itemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
        }
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, null, Resources.MySqlScriptWindowLaunchError, true);
      }
    }

    private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
    {
      DynamicItemMenuCommand matchedCommand = (DynamicItemMenuCommand)sender;
      if (MySqlDataProviderPackage.Instance != null)
      {
        _connectionsList = MySqlDataProviderPackage.Instance.GetMySqlConnections();
      }

      bool isRootItem = (matchedCommand.MatchedCommandId == 0);
      if (_connectionsList.Count == 0)
      {
        matchedCommand.Visible = false;
      }
      else
      {
        matchedCommand.Enabled = true;
        matchedCommand.Visible = true;
        int indexForDisplay = (isRootItem ? 0 : (matchedCommand.MatchedCommandId - _baselistID));
        matchedCommand.Text = _connectionsList[indexForDisplay].DisplayName;
      }

      matchedCommand.MatchedCommandId = 0;
    }
  }
}