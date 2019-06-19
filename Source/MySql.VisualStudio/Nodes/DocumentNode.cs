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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.Data.Common;
using System.Data;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using MySql.Data.VisualStudio.Editors;
using Microsoft.VisualStudio.Shell;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  abstract class DocumentNode : BaseNode, IVsPersistDocData
  {
    public DocumentNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
    }

    private uint DocumentCookie;

    protected abstract void Load();
    public abstract string GetSaveSql();
    protected abstract string GetCurrentName();

    public event EventHandler Saving;

    #region "Gathering Connection Logic"

    protected static EnvDTE.DTE Dte = null;
    private static Dictionary<string, BaseNode> dic = new Dictionary<string, BaseNode>();

    internal static void RegisterNode(BaseNode node)
    {
      lock (typeof(DocumentNode))
      {
        if (Dte == null)
        {
          Dte = (EnvDTE.DTE)node.HierarchyAccessor.ServiceProvider.GetService(typeof(EnvDTE.DTE));
        }
        string name = node.Moniker;
        dic.Remove(name);
        dic.Add(name, node);
      }
    }

    internal static void UpdateRegisteredNode(string oldMoniker, string newMoniker)
    {
      BaseNode node = dic[oldMoniker];
      dic.Remove(oldMoniker);
      if (dic.ContainsKey(newMoniker))
      {
        dic[newMoniker] = node;
      }
      else
      {
        dic.Add(newMoniker, node);
      }
    }

    /// <summary>
    /// Gets the connection of the currently edited document.
    /// </summary>
    public static DbConnection GetCurrentConnection()
    {
      if ((Dte == null ) || ( Dte.ActiveDocument == null)) return null;
      string curDoc = Dte.ActiveDocument.FullName;
      BaseNode node = null;
      if (dic.TryGetValue(curDoc, out node))
      {
        try
        {
          DbConnection c = (DbConnection)node.HierarchyAccessor.Connection.GetLockedProviderObject();
          return c;
        }
        finally
        {
          node.HierarchyAccessor.Connection.UnlockProviderObject();
        }
      }
      return null;
    }

    #endregion

    protected void OnSaving()
    {
      if (Saving != null)
        Saving(this, EventArgs.Empty);
    }

    protected virtual bool Save()
    {
      OnSaving();
      ExecuteSQL(GetSaveSql());
      return true;
    }

    #region IVsPersistDocData Members

    public int Close()
    {
      //throw new Exception("The method or operation is not implemented.");
      return VSConstants.S_OK;
    }

    public int GetGuidEditorType(out Guid pClassID)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int IsDocDataDirty(out int pfDirty)
    {
      pfDirty = Dirty ? 1 : 0;
      return VSConstants.S_OK;
    }

    public int IsDocDataReloadable(out int pfReloadable)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int LoadDocData(string pszMkDocument)
    {
      Debug.Assert(pszMkDocument == Moniker);
      Load();
      return VSConstants.S_OK;
    }

    public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
    {
      DocumentCookie = docCookie;
      Debug.Assert(HierarchyAccessor.Hierarchy == pHierNew, "Registration in wrong hierarchy");
      return VSConstants.S_OK;
    }

    public int ReloadDocData(uint grfFlags)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
    {
      return VSConstants.S_OK;
    }

    public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
    {
      string oldMoniker = Moniker;
      pfSaveCanceled = 1;
      pbstrMkDocumentNew = null;

      try
      {
        // Call out to the derived nodes to do the save work
        if (Save())
          pfSaveCanceled = 0;
      }
      catch (Exception ex)
      {
        Logger.LogError($"Unable to save object with error: {ex.Message}", true);
        return VSConstants.S_OK;
      }

      if (pfSaveCanceled == 0)
      {
        // then mark the document has clean and unchanged
        Dirty = false;
        IsNew = false;

        //notify any listeners that our save is done
        OnDataSaved();

        Name = GetCurrentName();
        DocumentNode.UpdateRegisteredNode(oldMoniker, Moniker);
        pbstrMkDocumentNew = String.Format("/Connection/{0}s/{1}", NodeId, Name);
        VsShellUtilities.RenameDocument(MySqlDataProviderPackage.Instance, oldMoniker, Moniker);

        // update server explorer
        Refresh();

        Load();
      }
      return VSConstants.S_OK;
    }

    public int SetUntitledDocPath(string pszDocDataPath)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    #region Events

    public event EventHandler DataLoaded;
    public event EventHandler DataChanged;
    public event EventHandler DataSaved;

    protected void OnDataLoaded()
    {
      if (DataLoaded != null)
        DataLoaded(this, null);
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged(this, null);
    }

    private void OnDataSaved()
    {
      if (DataSaved != null)
        DataSaved(this, null);
    }

    #endregion

  }
}
