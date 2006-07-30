using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VSStd97CmdID = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This control host the editor (an extended RichTextBox) and is responsible for
    /// handling the commands targeted to the editor as well as saving and loading
    /// the document. This control also implement the search and replace functionalities.
    /// </summary>
    public class BaseEditor : System.Windows.Forms.UserControl,
                                IVsPersistDocData,
                                IPersistFileFormat,
                                //IOleCommandTarget,
                                IVsWindowPane
    {
        protected Microsoft.VisualStudio.Data.DataConnection connection;

        #region Fields
        private Microsoft.VisualStudio.Shell.ServiceProvider vsServiceProvider;
        private MyPackage myPackage;

        private string fileName;
        private bool isDirty;
        // Flag true when we are loading the file. It is used to avoid to change the isDirty flag
        // when the changes are related to the load operation.
        private bool loading;
        // This flag is true when we are asking the QueryEditQuerySave service if we can edit the
        // file. It is used to avoid to have more than one request queued.
        private bool gettingCheckoutStatus;

        private IVsRunningDocumentTable runningDocTable;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        public BaseEditor()
        {
            PrivateInit(null);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public BaseEditor(MyPackage package)
        {
            PrivateInit(package);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private void PrivateInit(MyPackage package)
        {
            myPackage = package;
            loading = false;
            gettingCheckoutStatus = false;
        }

        public Microsoft.VisualStudio.Data.DataConnection Connection
        {
            set { connection = value; }
            get { return connection; }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
                GC.SuppressFinalize(this);
            }
            base.Dispose( disposing );
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode)
        {
              // If we're the top-level form or control, we need to do the mnemonic handling
              if (charCode != ' ' && ProcessMnemonic(charCode))
              {
                    return true;
              }
              return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        /// Retrieves the requested service from the Shell.
        /// </summary>
        /// <param name="serviceType">Service that is being requested</param>
        /// <returns>An object which type is as requested</returns>
        public object GetVsService(Type serviceType) 
        {
            return vsServiceProvider.GetService(serviceType);
        }

        private void NotifyDocChanged()
        {
            // Get a reference to the Running Document Table
            if ( null == runningDocTable )
            {
                runningDocTable = (IVsRunningDocumentTable)GetVsService(typeof(SVsRunningDocumentTable));
            }

            // Lock the document
            uint docCookie;
            IVsHierarchy hierarchy;
            uint itemID;
            IntPtr    docData = IntPtr.Zero;
            int hr = runningDocTable.FindAndLockDocument(
                (uint)_VSRDTFLAGS.RDT_ReadLock,
                fileName,
                out hierarchy,
                out itemID,
                out docData,
                out docCookie
            );
            // Release the docData because we don't use it.
            if (IntPtr.Zero != docData)
            {
                Marshal.Release(docData);
            }
            ErrorHandler.ThrowOnFailure(hr);

            // Send the notification. Note that we do not throw now in case of error because before
            // we have to unlock the document.
            hr = runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);

            // Unlock the document.
            // Note that we have to unlock the document even if the previous call failed.
            runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);

            // Check if the call to NotifyDocChanged failed.
            ErrorHandler.ThrowOnFailure(hr);
        }

        public virtual string EditorType
        {
            get { return ""; }
        }

        protected virtual void LoadData(string fileName)
        {
        }

        protected virtual void SaveData(string fileName)
        {
        }

    #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support.
        /// normally you would not modify the contents of this method with the code editor and instead
        /// manipulate it with the design view instead.  In the case where you are creating a new VS
        /// editor and no longer need use of the wizard-generated controls, you can feel free to edit
        /// this code in the code view or in the design view.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // EditorPane
            // 
            this.Name = "EditorPane";
            this.ResumeLayout(false);

        }
        #endregion

        protected string TrimFileName(string name)
        {
            int index = name.IndexOf(':');
            return name.Substring(index + 1);
        }

    #region IOleCommandTarget Members
/*
        /// <summary>
        /// The shell call this function to know if a menu item should be visible and
        /// if it should be enabled/disabled.
        /// Note that this function will only be called when an instance of this editor
        /// is open.
        /// </summary>
        /// <param name="guidCmdGroup">Guid describing which set of command the current command(s) belong to</param>
        /// <param name="cCmds">Number of command which status are being asked for</param>
        /// <param name="prgCmds">Information for each command</param>
        /// <param name="pCmdText">Used to dynamically change the command text</param>
        /// <returns>HRESULT</returns>
        public int QueryStatus(ref Guid guidCmdGroup, uint cCmds, OLECMD[] prgCmds, System.IntPtr pCmdText)
        {
            Debug.Assert(cCmds == 1, "Multiple commands");
            Debug.Assert(prgCmds!=null, "NULL argument");

            if ((prgCmds == null))
                return VSConstants.E_INVALIDARG;

            OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

            if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                // Process standard Commands
                switch (prgCmds[0].cmdID)
                {
                    case (uint)VSStd97CmdID.SelectAll:
                    {
                        // Always enabled
                        cmdf = OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;
                        break;
                    }
                    case (uint)VSStd97CmdID.Copy:
                    case (uint)VSStd97CmdID.Cut:
                    {
                        // Enable if something is selected
                        if (textBox1.SelectionLength > 0)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    }
                    case (uint)VSStd97CmdID.Paste:
                    {
                        // Enable if clipboard has content we can paste
                        if (textBox1.CanPaste(DataFormats.GetFormat(DataFormats.Text)))
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    }
                    case (uint)VSStd97CmdID.Redo:
                    {
                        if (textBox1.CanRedo)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    }
                    case (uint)VSStd97CmdID.Undo:
                    {
                        if (textBox1.CanUndo)
                            cmdf |= OLECMDF.OLECMDF_ENABLED;
                        break;
                    }
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (guidCmdGroup == GuidList.guidMySqlVisualStudioCmdSet)
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
                return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);;

            prgCmds[0].cmdf = (uint)cmdf;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Execute a command
        /// Typically, only the first 2 arguments are used (to identify which command should be run)
        /// </summary>
        /// <param name="guidCmdGroup">Guid describing which set of command the current command(s) belong to</param>
        /// <param name="nCmdID">Command that should be executed</param>
        /// <param name="nCmdexecopt">options for the command</param>
        /// <param name="pvaIn">Pointer to input arguments</param>
        /// <param name="pvaOut">Pointer to command output</param>
        /// <returns></returns>
        public int Exec(ref Guid guidCmdGroup, uint nCmdID, uint nCmdexecopt, System.IntPtr pvaIn, System.IntPtr pvaOut)
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Exec() of: {0}", this.ToString()));

            if (guidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                // Process standard Visual Studio Commands
                switch (nCmdID)
                {
                    case (uint)VSStd97CmdID.Copy:
                    {
                        textBox1.Copy();
                        break;
                    }
                    case (uint)VSStd97CmdID.Cut:
                    {
                        textBox1.Cut();
                        break;
                    }
                    case (uint)VSStd97CmdID.Paste:
                    {
                        textBox1.Paste();
                        break;
                    }
                    case (uint)VSStd97CmdID.Redo:
                    {
                        textBox1.Redo();
                        break;
                    }
                    case (uint)VSStd97CmdID.Undo:
                    {
                        textBox1.Undo();
                        break;
                    }
                    case (uint)VSStd97CmdID.SelectAll:
                    {
                        textBox1.SelectAll();
                        break;
                    }
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (guidCmdGroup == GuidList.guidMySqlVisualStudioCmdSet)
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
        }*/
        #endregion


    #region IVsWindowPane Members

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.Shell.Interop.IVsWindowPane.TranslateAccelerator(MSG[] lpmsg)
        {
            return VSConstants.S_FALSE;
        }

        int Microsoft.VisualStudio.Shell.Interop.IVsWindowPane.SaveViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.Shell.Interop.IVsWindowPane.LoadViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.Shell.Interop.IVsWindowPane.GetDefaultSize(SIZE[] size)
        {
            if (size.Length >= 1)
            {
                size[0].cx = Size.Width;
                size[0].cy = Size.Height;
            }

            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.Shell.Interop.IVsWindowPane.CreatePaneWindow(System.IntPtr hwndParent, int x, int y, int cx, int cy, out System.IntPtr hwnd)
        {
            Win32Methods.SetParent(Handle, hwndParent);
            hwnd = Handle;

            Size = new System.Drawing.Size(cx - x, cy - y);
            return VSConstants.S_OK;
        }

        public int ClosePane()
        {
            this.Dispose(true);
            return VSConstants.S_OK;
        }

        #endregion 


        int Microsoft.VisualStudio.OLE.Interop.IPersist.GetClassID(out Guid pClassID)
        {
            pClassID = GuidList.guidEditorFactory;
            return VSConstants.S_OK;
        }

        #region IPersistFileFormat Members

        int IPersistFileFormat.SaveCompleted(string pszFilename)
        {
            // TODO:  Add Editor.SaveCompleted implementation
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            // We only support 1 format so return its index
            pnFormatIndex = 0;
            ppszFilename = fileName;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            if (nFormatIndex != 0)
            {
                throw new ArgumentException(Resources.UnknownFileFormat);
            }
            // until someone change the file, we can consider it not dirty as
            // the user would be annoyed if we prompt him to save an empty file
            isDirty = false;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetClassID(out Guid pClassID)
        {
            ((Microsoft.VisualStudio.OLE.Interop.IPersist)this).GetClassID(out pClassID);
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetFormatList(out string ppszFormatList)
        {
            ppszFormatList = Resources.EditorFormatString;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            if ( (pszFilename == null) && 
                 ((fileName == null) || (fileName.Length == 0)) )
            {
                throw new ArgumentNullException(Resources.NullFilename);
            }

            loading = true;
            int hr = VSConstants.S_OK;
            try
            {
                bool isReload = false;

                // If the new file name is null, then this operation is a reload
                if (pszFilename == null)
                {
                    isReload = true;
                }

                // Show the wait cursor while loading the file
                IVsUIShell vsUiShell = (IVsUIShell)GetVsService(typeof(SVsUIShell));
                if (vsUiShell != null)
                {
                    // Note: we don't want to throw or exit if this call fails, so
                    // don't check the return code.
                    vsUiShell.SetWaitCursor();
                }

                // Set the new file name
                if ( !isReload )
                {
                    fileName = pszFilename;
                }
                LoadData(fileName);
                isDirty = false;

                // Notify the load or reload
                NotifyDocChanged();
            }
            finally
            {
                loading = false;
            }
            return hr;
        }

        int IPersistFileFormat.IsDirty(out int pfIsDirty)
        {
            if (isDirty)
            {
                pfIsDirty = 1;
            }
            else
            {
                pfIsDirty = 0;
            }
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Save(string pszFilename, int remember, uint nFormatIndex)
        {
            // If file is null or same --> SAVE
            if (pszFilename == null || pszFilename == fileName)
            {
                SaveData(fileName);
                isDirty = false;
            }
                // If remember --> SaveAs
            else if (remember != 0)
            {
                fileName = pszFilename;
//                textBox1.SaveFile(fileName, RichTextBoxStreamType.PlainText);
                isDirty = false;
            }
            // Else, Save a Copy As
            else
            {
  //              textBox1.SaveFile(pszFilename, RichTextBoxStreamType.PlainText);
            }

            return VSConstants.S_OK;
        }

        #endregion


        #region IVsPersistDocData Members

        int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
        {
            return ((IPersistFileFormat)this).IsDirty(out pfDirty);
        }

        int IVsPersistDocData.SaveDocData(Microsoft.VisualStudio.Shell.Interop.VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;
            int hr = VSConstants.S_OK;

            switch (dwSave)
            {
                case VSSAVEFLAGS.VSSAVE_Save:
                case VSSAVEFLAGS.VSSAVE_SilentSave:
                {
                    IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetVsService(typeof(SVsQueryEditQuerySave));

                    // Call QueryEditQuerySave
                    uint result=0;
                    hr = queryEditQuerySave.QuerySaveFile(
                                        fileName,        // filename
                                        0,    // flags
                                        null,            // file attributes
                                        out result);    // result
                    if ( ErrorHandler.Failed(hr) )
                        return hr;

                    // Process according to result from QuerySave
                    switch ( (tagVSQuerySaveResult)result )
                    {
                        case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                            // Note that this is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
                            // two tags have the same value.
                            pfSaveCanceled = ~0;
                            break;

                        case tagVSQuerySaveResult.QSR_SaveOK:
                            {
                                // Call the shell to do the save for us
                                IVsUIShell uiShell = (IVsUIShell)GetVsService(typeof(SVsUIShell));
                                hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                                if ( ErrorHandler.Failed(hr) )
                                    return hr;
                            }
                            break;

                        case tagVSQuerySaveResult.QSR_ForceSaveAs:
                            {
                                // Call the shell to do the SaveAS for us
                                IVsUIShell uiShell = (IVsUIShell)GetVsService(typeof(SVsUIShell));
                                hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                                if ( ErrorHandler.Failed(hr) )
                                    return hr;
                            }
                            break;

                        case tagVSQuerySaveResult.QSR_NoSave_Continue:
                            // In this case there is nothing to do.
                            break;

                        default:
                            throw new COMException(Resources.SCCError);
                    }
                    break;
                }
                default:
                    throw new ArgumentException(Resources.BadSaveFlags);
            };

            return VSConstants.S_OK;
        }

        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
        }

        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            return ((IPersistFileFormat)this).InitNew(0);
        }

        int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
        {
            return ((IPersistFileFormat)this).GetClassID(out pClassID);
        }

        int IVsPersistDocData.Close()
        {
//            if (textBox1 != null)
  //          {
    //            textBox1.Dispose();
      //      }
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
        {
            // Allow file to be reloaded
            pfReloadable = 1;
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            // TODO:  Add EditorPane.RenameDocData implementation
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.ReloadDocData(uint grfFlags)
        {
            return ((IPersistFileFormat)this).Load(null, grfFlags, 0);
        }

        int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            return VSConstants.S_OK;
        }

        #endregion

        /// <summary>
        /// This function asks to the QueryEditQuerySave service if it is possible to
        /// edit the file.
        /// </summary>
        private bool CanEditFile()
        {
            Trace.WriteLine("\t**** CanEditFile called ****");

            // Check the status of the recursion guard
            if (gettingCheckoutStatus)
                return false;

            try
            {
                // Set the recursion guard
                gettingCheckoutStatus = true;

                // Get the QueryEditQuerySave service
                IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetVsService(typeof(SVsQueryEditQuerySave));

                // Now call the QueryEdit method to find the edit status of this file
                string[] documents = { this.fileName };
                uint result;
                uint outFlags;

                // Note that this function can popup a dialog to ask the user to checkout the file.
                // When this dialog is visible, it is possible to receive other request to change
                // the file and this is the reason for the recursion guard.
                int hr = queryEditQuerySave.QueryEditFiles(
                    0,              // Flags
                    1,              // Number of elements in the array
                    documents,      // Files to edit
                    null,           // Input flags
                    null,           // Input array of VSQEQS_FILE_ATTRIBUTE_DATA
                    out result,     // result of the checkout
                    out outFlags    // Additional flags
                );
                if (ErrorHandler.Succeeded(hr) && (result == (uint)tagVSQueryEditResult.QER_EditOK))
                {
                    // In this case (and only in this case) we can return true from this function.
                    return true;
                }
            }
            finally
            {
                gettingCheckoutStatus = false;
            }
            return false;
        }

        private void OnTextChange(object sender, System.EventArgs e)
        {
            // During the load operation the text of the control will change, but
            // this change must not be stored in the status of the document.
            if (!loading)
            {
                // The only interesting case is when we are changing the document
                // for the first time
                if (!isDirty)
                {
                    // Check if the QueryEditQuerySave service allow us to change the file
                    if (!CanEditFile())
                    {
                        // We can not change the file (e.g. a checkout operation failed),
                        // so undo the change and exit.
                        //textBox1.Undo();
                        return;
                    }

                    // It is possible to change the file, so update the status.
                    isDirty = true;
                }
            }
        }
    }
}
