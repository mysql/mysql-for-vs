// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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

namespace MySql.Data.VisualStudio
{
	abstract class DocumentNode : BaseNode, IVsPersistDocData
	{
        protected IEditor editor;

        public DocumentNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
            base(hierarchyAccessor, id)
        {
        }

        #region Properties

        private uint DocumentCookie { get; set; }

        #endregion

        protected abstract void Load();
        public abstract string GetSaveSql();
        protected abstract string GetCurrentName();

        protected virtual bool Save()
        {
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
            OnDataLoaded();
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
                MessageBox.Show("Unable to save object with error: " + ex.Message);
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
                pbstrMkDocumentNew = String.Format("/Connection/{0}s/{1}", NodeId, Name);
                VsShellUtilities.RenameDocument(MySqlDataProviderPackage.Instance, oldMoniker, Moniker);

                // update server explorer
                RefreshServerExplorer();
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

        private void OnDataLoaded()
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
