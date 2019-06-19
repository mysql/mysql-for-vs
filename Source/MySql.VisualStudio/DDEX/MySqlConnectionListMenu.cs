// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Data.Services;
using System.Collections.Generic;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{  
  internal class MySqlConnectionListMenu
  {
    private int _baselistID = (int)PkgCmdIDList.cmdidMRUList;    
    private List<IVsDataExplorerConnection> _connectionsList;


    public MySqlConnectionListMenu(ref OleMenuCommandService mcs, List<IVsDataExplorerConnection> connList)
    {
      _connectionsList = connList;            
      InitMRUMenu(ref mcs);
    }    

    internal void InitMRUMenu(ref OleMenuCommandService mcs)
    {            
      if (mcs != null)
      {                
            CommandID dynamicItemRootId = new CommandID(GuidList.guidIDEToolbarCmdSet, _baselistID);
            DynamicItemMenuCommand dynamicMenuCommand = new DynamicItemMenuCommand(dynamicItemRootId, IsValidDynamicItem,
                                                         OnInvokedDynamicItem,                                                         
                                                         OnBeforeQueryStatusDynamicItem);
              mcs.AddCommand(dynamicMenuCommand);                  
      }
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
      DynamicItemMenuCommand invokedCommand = (DynamicItemMenuCommand)sender;      
      
      if (null != invokedCommand)
      {
        bool isRootItem = (invokedCommand.MatchedCommandId == 0);
        int indexForDisplay = (isRootItem ? 0 : (invokedCommand.MatchedCommandId - _baselistID));
        
        if (MySqlDataProviderPackage.Instance == null)
          return;
        try
        {
          if (indexForDisplay < _connectionsList.Count)
          {
            MySqlConnection connection = (MySqlConnection)_connectionsList[indexForDisplay].Connection
                                        .GetLockedProviderObject();
            try
            {
              if (connection != null)
              {
                MySqlDataProviderPackage.Instance.MysqlConnectionSelected = connection;
              }
              var ItemOp = MySqlDataProviderPackage.Instance.GetDTE2().ItemOperations;
              ItemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
            }
            finally
            {
              _connectionsList[indexForDisplay].Connection.UnlockProviderObject();
            }
            return;
          }
          else
          {
            var ItemOp = MySqlDataProviderPackage.Instance.GetDTE2().ItemOperations;
            ItemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");           
          }
        }
        catch (Exception ex)
        {
          Logger.LogError($"An error ocurred when trying to launch a MySql Script window: {ex.Message}", true);
        }
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
