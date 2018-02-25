// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.Data.VisualStudio
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
