// Copyright © 2008, 2017, Oracle and/or its affiliates. All rights reserved.
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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MySql.Utility.Classes.MySqlWorkbench;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MySql.Data.VisualStudio.DDEX
{
  public class ServerExplorerHierarchyEventsListener : IVsHierarchyEvents, IDisposable
  {
    private uint cookie;
    private readonly IVsUIHierarchy hierarchy;

    public ServerExplorerHierarchyEventsListener(IVsUIHierarchy hierarchy)
    {
      this.hierarchy = hierarchy;
    }

    public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
    {
      const int PROPERTY = (int)__VSHPROPID.VSHPROPID_Caption;
      object value;
      int hr = this.hierarchy.GetProperty(itemidAdded, PROPERTY, out value);
      if (hr == VSConstants.S_OK && value != null && MySqlDataProviderPackage.Instance != null)
      {
        MySqlDataProviderPackage.Instance.GetMySqlConnections();
        MySqlDataProviderPackage.Instance.UpdateMySqlConnectionNames();
      }

      return VSConstants.S_OK;
    }

    public int OnInvalidateIcon(IntPtr hicon)
    {
      return VSConstants.S_OK;
    }


    public int OnInvalidateItems(uint itemidParent)
    {
      return VSConstants.S_OK;
    }

    public int OnItemDeleted(uint itemid)
    {
      if (MySqlDataProviderPackage.Instance != null)
      {
        MySqlDataProviderPackage.Instance.UpdateMySqlConnectionNames();
      }

      return VSConstants.S_OK;
    }

    public int OnItemsAppended(uint itemidParent)
    {
      return VSConstants.S_OK;
    }

    public int OnPropertyChanged(uint itemid, int propid, uint flags)
    {
      object value;
      __VSHPROPID changedProperty;
      const int PROPERTY = (int)__VSHPROPID.VSHPROPID_Caption;
      int hr = this.hierarchy.GetProperty(itemid, PROPERTY, out value);
      Enum.TryParse(propid.ToString(),true, out changedProperty);

      // Return if a property other than the name has been modified.
      if (hr != VSConstants.S_OK || changedProperty != __VSHPROPID.VSHPROPID_BrowseObject || value == null || MySqlDataProviderPackage.Instance == null)
      {
        return VSConstants.S_OK;
      }

      // Check if a connection has been renamed.
      var newConnectionName = value.ToString();
      var connectionNames = MySqlDataProviderPackage.Instance.MySqlConnectionsNameList;
      if (connectionNames == null)
      {
        return VSConstants.S_OK;
      }

      foreach (var connectionName in connectionNames)
      {
        if (connectionName == newConnectionName)
        {
          MySqlDataProviderPackage.Instance.UpdateMySqlConnectionNames();
          return VSConstants.S_OK;
        }
      }

      // Find the renamed connection.
      var newConnections = MySqlDataProviderPackage.Instance.GetMySqlConnections();
      if (newConnections == null)
      {
        return VSConstants.S_OK;
      }

      var newConnectionNames = newConnections.Select(o => o.DisplayName).ToList();
      string oldConnectionName = null;
      foreach (var connectionName in connectionNames)
      {
        var connectionNameExists = newConnectionNames.Exists(o => o == connectionName);
        if (!connectionNameExists)
        {
          oldConnectionName = connectionName;
          break;
        }
      }

      // Rename connection in connections.xml.
      if (MySqlWorkbench.Connections == null)
      {
        return VSConstants.S_OK;
      }

      foreach (var mySqlWorkbenchConnection in MySqlWorkbench.Connections)
      {
        if (mySqlWorkbenchConnection.Name != oldConnectionName)
        {
          continue;
        }

        mySqlWorkbenchConnection.Name = newConnectionName;
        mySqlWorkbenchConnection.SavedStatus = MySqlWorkbenchConnection.SavedStatusType.Updated;
        MySqlWorkbench.Connections.SaveConnection(mySqlWorkbenchConnection);
        MySqlDataProviderPackage.Instance.UpdateMySqlConnectionNames();
        break;
      }

      return VSConstants.S_OK;
    }

    public void Dispose()
    {
      if (this.cookie != 0)
      {
        this.hierarchy.UnadviseHierarchyEvents(this.cookie);
        this.cookie = 0;
      }
    }
  }
}
