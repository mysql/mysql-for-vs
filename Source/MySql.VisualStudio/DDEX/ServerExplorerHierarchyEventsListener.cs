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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.Data.VisualStudio.DDEX
{
  public class ServerExplorerHierarchyEventsListener : IVsHierarchyEvents, IDisposable
  {
    private readonly IVsUIHierarchy hierarchy;
    private uint cookie;

    public ServerExplorerHierarchyEventsListener(IVsUIHierarchy hierarchy)
    {
      this.hierarchy = hierarchy;
      int hr = this.hierarchy.AdviseHierarchyEvents(this, out cookie);
      ErrorHandler.ThrowOnFailure(hr);
    }

    public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
    {
      const int Property = (int)__VSHPROPID.VSHPROPID_Caption;
      object value;
      int hr = this.hierarchy.GetProperty(itemidAdded, Property, out value);
      if (hr == VSConstants.S_OK && value != null)
      {                        
        if (MySqlDataProviderPackage.Instance != null)
        {
          MySqlDataProviderPackage.Instance.GetMySqlConnections();
        }
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
      return VSConstants.S_OK;
    }

    public int OnItemsAppended(uint itemidParent)
    {
      return VSConstants.S_OK;
    }

    public int OnPropertyChanged(uint itemid, int propid, uint flags)
    {
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
