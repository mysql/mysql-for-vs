using System;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;

namespace Vsip.MyVSTools
{
    public class BaseEditor : UserControl, IVsPersistDocData, IPersistFileFormat,
                              IVsDocDataFileChangeControl, IOleCommandTarget,
                              IVsWindowPane 
    {
        protected bool isDirty;
        private IVsRunningDocumentTable rdt;
        private IVsUIShell vsUIShell;
        protected uint cookie;
        protected IVsHierarchy hierarchy;
        protected uint itemID;
        private bool guard;
        private ServiceProvider vsServiceProvider = null;


        public BaseEditor()
            : base()
        {
//            rdt = (IVsRunningDocumentTable)PackageSingleton.Package.GetMyService(
  //              typeof(SVsRunningDocumentTable));
    //        vsUIShell = (IVsUIShell)PackageSingleton.Package.GetMyService(
      //          typeof(SVsUIShell));
        }

        protected virtual Guid EditorGuid
        {
            get { return Guid.Empty; }
        }

        protected uint Cookie
        {
            get { return cookie; }
        }

        protected bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        public virtual string Filename
        {
            get { return null; }
        }

        protected virtual bool CanCopyAndCut
        {
            get { return false; }
        }

        protected virtual bool CanPaste
        {
            get { return false; }
        }

        protected virtual bool CanRedo
        {
            get { return false; }
        }

        protected virtual bool CanUndo
        {
            get { return false; }
        }

        #region Command Virtuals

        protected virtual void Copy() 
        {
        }

        protected virtual void Cut()
        {
        }

        protected virtual void Paste()
        {
        }

        protected virtual void Redo()
        {
        }

        protected virtual void Undo()
        {
        }

        protected virtual void SelectAll()
        {
        }



        #endregion

        protected bool CanEdit()
        {
            if (guard)
                return false;

            try
            {
                // Protect against recursion
                guard = true;

                // Get the QueryEditQuerySave service
                IVsQueryEditQuerySave2 qeqsService = (IVsQueryEditQuerySave2)
                    PackageSingleton.Package.GetMyService((typeof(SVsQueryEditQuerySave)));

                // Now call the QueryEdit method to find the edit status of this file
                string[] docsToCheck = { Filename };
                uint editResult;
                uint flags;

                // Note that this function can popup a dialog to ask the user to checkout the file.
                // When this dialog is visible, it is possible to receive other request to change
                // the file and this is the reason for the recursion guard.
                int result = qeqsService.QueryEditFiles(
                    0,              // Flags
                    docsToCheck.Length, docsToCheck, null, null,
                    out editResult, out flags);

                if ((ErrorHandler.Succeeded(result)) && 
                    (editResult == (uint)tagVSQueryEditResult.QER_EditOK))
                    return true;
            }
            finally
            {
                guard = false;
            }
            return false;
        }

        protected void NotifyDocChanged()
        {
            // Lock the document
            int result = rdt.LockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, cookie);
            ErrorHandler.ThrowOnFailure(result);

            result = rdt.NotifyDocumentChanged(cookie, 
                (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);

            // we have to unlock the document in *all* cases
            rdt.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, cookie);

            // Check if the call to NotifyDocChanged failed.
            ErrorHandler.ThrowOnFailure(result);
        }

        protected void RefreshCommandUI()
        {
            vsUIShell.UpdateCommandUI(1);
        }

        #region IVsPersistDocData Members

        int IVsPersistDocData.Close()
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
        {
            return (this as IPersistFileFormat).GetClassID(out pClassID);
        }

        int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
        {
            return (this as IPersistFileFormat).IsDirty(out pfDirty);
        }

        int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            uint flags;
            uint readlocks;
            uint editlocks;
            string doc;
            IVsHierarchy hier;
            uint itemid;
            IntPtr punk;
            int result = rdt.GetDocumentInfo(cookie, out flags, out readlocks, 
                out editlocks, out doc, out hier, out itemid, out punk);
            NotifyDocChanged();
            return VSConstants.S_OK; 
        }

        int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            cookie = docCookie;
            hierarchy = pHierNew;
            itemID = itemidNew;

            return VSConstants.S_OK;
        }

        int IVsPersistDocData.ReloadDocData(uint grfFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, 
                                 string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, 
                               out int pfSaveCanceled)
        {
            pfSaveCanceled = 0;
            pbstrMkDocumentNew = "saved";
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            return VSConstants.S_OK;
        }

        #endregion

        int Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID(out Guid pClassID)
        {
            pClassID = EditorGuid;
            return VSConstants.S_OK;
        }


        #region IPersistFileFormat Members

        int IPersistFileFormat.GetClassID(out Guid pClassID)
        {
            ((Microsoft.VisualStudio.OLE.Interop.IPersist)this).GetClassID(out pClassID);
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            ppszFilename = null;
            pnFormatIndex = 0;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetFormatList(out string ppszFormatList)
        {
            ppszFormatList = null;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IPersistFileFormat.IsDirty(out int pfIsDirty)
        {
            pfIsDirty = isDirty ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.SaveCompleted(string pszFilename)
        {
            return VSConstants.S_OK;
        }

        #endregion


        #region IVsDocDataFileChangeControl Members

        int IVsDocDataFileChangeControl.IgnoreFileChanges(int fIgnore)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Exec() of: {0}", this.ToString()));

            if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                // Process standard Visual Studio Commands
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.Copy:
                        Copy();
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Cut:
                        Cut();
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Paste:
                        Paste();
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Redo:
                        Redo();
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Undo:
                        Undo();
                        break;
                    case (uint)VSConstants.VSStd97CmdID.SelectAll:
                        SelectAll();
                        break;
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (guidCmdGroup == GuidList.guidMyVSToolsCmdSet)
            {
                switch (nCmdID)
                {
                    // if we had commands specific to our editor, they would be processed here
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;

            return VSConstants.S_OK;
        }

        int IOleCommandTarget.QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            Debug.Assert(cCmds == 1, "Multiple commands");
            Debug.Assert(prgCmds != null, "NULL argument");

            if ((prgCmds == null))
                return VSConstants.E_INVALIDARG;

            OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

            if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                // Process standard Commands
                switch (prgCmds[0].cmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.SelectAll:
                        // Always enabled
                        cmdf = OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Copy:
                    case (uint)VSConstants.VSStd97CmdID.Cut:
                        // Enable if something is selected
                        if (CanCopyAndCut)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;

                    case (uint)VSConstants.VSStd97CmdID.Paste:
                        if (CanPaste)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Redo:
                        if (CanRedo)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    case (uint)VSConstants.VSStd97CmdID.Undo:
                        if (CanUndo)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (guidCmdGroup == GuidList.guidMyVSToolsCmdSet)
            {
                // Process our Commands
                switch (prgCmds[0].cmdID)
                {
                    // if we had commands specific to our editor, they would be processed here
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else
                return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED); ;

            prgCmds[0].cmdf = (uint)cmdf;

            return VSConstants.S_OK;
        }

        #endregion


        #region IVsWindowPane Members

        int IVsWindowPane.ClosePane()
        {
            this.Dispose(true);
            return VSConstants.S_OK;
        }

        int IVsWindowPane.CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
        {
            Win32Methods.SetParent(Handle, hwndParent);
            hwnd = Handle;

            Size = new System.Drawing.Size(cx - x, cy - y);
            return VSConstants.S_OK;
        }

        int IVsWindowPane.GetDefaultSize(SIZE[] pSize)
        {
            if (pSize.Length >= 1)
            {
                pSize[0].cx = Size.Width;
                pSize[0].cy = Size.Height;
            }

            return VSConstants.S_OK;
        }

        int IVsWindowPane.LoadViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.SaveViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        int IVsWindowPane.TranslateAccelerator(MSG[] lpmsg)
        {
            return VSConstants.S_FALSE;
        }

        #endregion
    }
}
