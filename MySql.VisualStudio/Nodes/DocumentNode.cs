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

        protected virtual bool Save()
        {
            DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
            try
            {
                string sql = GetSaveSql();
                ExecuteSQL(sql);
                Dirty = false;
                IsNew = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save object with error: " + ex.Message);
                return false;
            }
            finally
            {
                HierarchyAccessor.Connection.UnlockProviderObject();
            }
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
            throw new Exception("The method or operation is not implemented.");
        }

        public int SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            pfSaveCanceled = Save() ? 0 : 1;
            pbstrMkDocumentNew = null;
            if (pfSaveCanceled == 0)
            {
                if (IsNew) SaveNode();
                Dirty = false;
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

        #endregion

	}
}
