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

namespace MySql.VSTools
{
    /// <summary>
    /// This control host the editor (an extended RichTextBox) and is responsible for
    /// handling the commands targeted to the editor as well as saving and loading
    /// the document. This control also implement the search and replace functionalities.
    /// </summary>
    public sealed class EditorPane : System.Windows.Forms.UserControl,
                                IVsPersistDocData,
                                IPersistFileFormat,
                                IOleCommandTarget,
                                IVsWindowPane,
                                IVsFindTarget,
                                IVsDocDataFileChangeControl,
                                IVsFileChangeEvents

    {
        private const uint myFormat = 0;
        private const string myExtension = ".xxx";

        #region Fields
        private ServiceProvider vsServiceProvider = null;
        private MyVSTools myPackage = null;

        private string fileName;
        private bool isDirty;
        // Flag true when we are loading the file. It is used to avoid to change the isDirty flag
        // when the changes are related to the load operation.
        private bool loading;
        // This flag is true when we are asking the QueryEditQuerySave service if we can edit the
        // file. It is used to avoid to have more than one request queued.
        private bool gettingCheckoutStatus;
        private MyEditor textBox1 = null;

        // Find and replace related variables
        private object findState = null;
        private bool passedEndOfFile;
        private bool forceNewSearch;
        private int currentSearchStart;
        private int searchCursorLocation;
        private bool forceCursorWrapAround;
        private bool replaced;

        // Counter of the file system changes to ignore.
        private int changesToIgnore;
        // Cookie for the subscription to the file system notification events.
        private uint fileChangeNotifyCookie;
        // This flag is used to know when the reload procedure is running.
        private bool isFileReloading;

        private const char EndLine = (char)10;


        private IVsRunningDocumentTable runningDocTable;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Timer reloadTimer;
        #endregion

        private EditorPane()
        {
            PrivateInit(null);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public EditorPane(MyVSTools package)
        {
            PrivateInit(package);

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private void PrivateInit(MyVSTools package)
        {
            myPackage = package;
            loading = false;
            gettingCheckoutStatus = false;
            isFileReloading = false;
            fileChangeNotifyCookie = VSConstants.VSCOOKIE_NIL;
            changesToIgnore = 0;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                // Unsubscribe from the notification of file system events
                AdviseFileChange(false);
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
            IntPtr    docData;
            int hr = runningDocTable.FindAndLockDocument(
                (uint)_VSRDTFLAGS.RDT_ReadLock,
                fileName,
                out hierarchy,
                out itemID,
                out docData,
                out docCookie
            );
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
            this.textBox1 = new MyEditor();
            this.reloadTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.HideSelection = false;
            this.textBox1.Location = new System.Drawing.Point(8, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(136, 136);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "";
            this.textBox1.TextChanged += new System.EventHandler(this.OnTextChange);
            //
            // reloadTimer
            // 
            this.reloadTimer.Tick += new System.EventHandler(this.reloadTimer_Tick);
            // 
            // EditorPane
            // 
            this.Controls.Add(this.textBox1);
            this.Name = "EditorPane";
            this.ResumeLayout(false);

        }
        #endregion


        #region IOleCommandTarget Members

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
            pnFormatIndex = myFormat;
            ppszFilename = fileName;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            if (nFormatIndex != myFormat)
            {
                throw new ArgumentException(MyVSTools.GetResourceString("UnknownFileFormat"));
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
            ppszFormatList = MyVSTools.GetResourceString("EditorFormatString");
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            if ( (pszFilename == null) && 
                 ((fileName == null) || (fileName.Length == 0)) )
            {
                throw new ArgumentNullException(MyVSTools.GetResourceString("NullFilename"));
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
                    // Unsubscribe from the notification of the changes in the previous file.
                    AdviseFileChange(false);
                    fileName = pszFilename;
                }
                // Load the file
                textBox1.LoadFile(fileName, RichTextBoxStreamType.PlainText);
                isDirty = false;

                // Subscribe for the notification on file changes.
                if ( !isReload )
                {
                    AdviseFileChange(true);
                }

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
            // We don't want to be notify for this change of the file.
            AdviseFileChange(false);

            // If file is null or same --> SAVE
            if (pszFilename == null || pszFilename == fileName)
            {
                textBox1.SaveFile(fileName, RichTextBoxStreamType.PlainText);
                isDirty = false;
            }
                // If remember --> SaveAs
            else if (remember != 0)
            {
                fileName = pszFilename;
                textBox1.SaveFile(fileName, RichTextBoxStreamType.PlainText);
                isDirty = false;
            }
            // Else, Save a Copy As
            else
            {
                textBox1.SaveFile(pszFilename, RichTextBoxStreamType.PlainText);
            }
            // Now that the file is saved (and maybe renamed) we can subscribe again
            // for the file system events.
            AdviseFileChange(true);

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
                            throw new COMException(MyVSTools.GetResourceString("SCCError"));
                    }
                    break;
                }
                case VSSAVEFLAGS.VSSAVE_SaveAs:
                case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
                {
                    // Make sure the file name as the right extension
                    if (string.Compare(myExtension, System.IO.Path.GetExtension(fileName), true, CultureInfo.InvariantCulture) != 0)
                    {
                        fileName += myExtension;
                    }
                    // Call the shell to do the save for us
                    IVsUIShell uiShell = (IVsUIShell)GetVsService(typeof(SVsUIShell));
                    hr = uiShell.SaveDocDataToFile(dwSave, (IPersistFileFormat)this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                    if ( ErrorHandler.Failed(hr) )
                        return hr;
                    break;
                }
                default:
                    throw new ArgumentException(MyVSTools.GetResourceString("BadSaveFlags"));
            };

            return VSConstants.S_OK;
        }

        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
        }

        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            return ((IPersistFileFormat)this).InitNew(myFormat);
        }

        int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
        {
            return ((IPersistFileFormat)this).GetClassID(out pClassID);
        }

        int IVsPersistDocData.Close()
        {
            if (textBox1 != null)
            {
                textBox1.Dispose();
            }
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


        #region IVsFindTarget Members

        /// <summary>
        /// Return the object that was requested
        /// </summary>
        /// <param name="propid">Id of the requested object</param>
        /// <param name="pvar">Object returned</param>
        /// <returns>HResult</returns>
        public int GetProperty(uint propid, out object pvar)
        {
            pvar = null;

            switch(propid)
            {
                case (uint)__VSFTPROPID.VSFTPROPID_DocName:
                {
                    // Return the file name
                    pvar = fileName;
                    break;
                }
                case (uint)__VSFTPROPID.VSFTPROPID_InitialPattern:
                case (uint)__VSFTPROPID.VSFTPROPID_InitialPatternAggressive:
                {
                    // Return the selected text
                    pvar = textBox1.SelectedText;
                    break;
                }
                case (uint)__VSFTPROPID.VSFTPROPID_WindowFrame:
                {
                    // Return the Window frame
                    pvar = (IVsWindowFrame)GetVsService(typeof(SVsWindowFrame));
                    break;
                }
                case (uint)__VSFTPROPID.VSFTPROPID_IsDiskFile:
                {
                    // We assume the file is on disk
                    pvar = true;
                    break;
                }
                default:
                {
                    return VSConstants.E_NOTIMPL;
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Not implemented.
        /// We don't need this function since we implement Find and Replace
        /// </summary>
        /// <param name="grfOptions"></param>
        /// <param name="ppSpans"></param>
        /// <param name="ppTextImage"></param>
        public int GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage)
        {
            ppTextImage = null;
            return VSConstants.E_NOTIMPL; // "GetSearchImage is not implemented, Find and Replace should be used instead"
        }

        /// <summary>
        /// Retrieve a previously stored object
        /// </summary>
        /// <returns>The object that is being asked</returns>
        public int GetFindState(out object state)
        {
            state = findState;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Search for the string in the text of our editor.
        /// Options specify how we do the search
        /// </summary>
        /// <param name="pszSearch">Search string</param>
        /// <param name="grfOptions">Search options</param>
        /// <param name="fResetStartPoint">Is this a new search?</param>
        /// <param name="pHelper">We are not using it</param>
        /// <param name="pResult">True if we found the search string</param>
        public int Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult)
        {
            pResult = 0;
            int result = 0;
            bool reverse = false;

            // Transform the VS search options to RTF search options
            RichTextBoxFinds options = RichTextBoxFinds.None;
            if ((grfOptions & (int)__VSFINDOPTIONS.FR_WholeWord) != 0)
                options |= RichTextBoxFinds.WholeWord;
            if ((grfOptions & (int)__VSFINDOPTIONS.FR_MatchCase) != 0)
                options |= RichTextBoxFinds.MatchCase;
            if ((grfOptions & (int)__VSFINDOPTIONS.FR_Backwards) != 0)
            {
                options |= RichTextBoxFinds.Reverse;
                reverse = true;
            }

            // Verify if this is a new search
            if (fResetStartPoint != 0
                || forceNewSearch
                || textBox1.SelectionStart != searchCursorLocation)
            {
                forceNewSearch = false;
                currentSearchStart = textBox1.SelectionStart + textBox1.SelectionLength;
                // If our start location is the end of the file, start at the beginging
                if (currentSearchStart == textBox1.Text.Length)
                    currentSearchStart = 0;
                searchCursorLocation = currentSearchStart;
                // Let replace know we are starting from scratch (in case this find is for a replace)
                replaced = false;
            }
            else
            {
                // Continue search
                // Updated search position
                if (forceCursorWrapAround)
                {
                    forceCursorWrapAround = false;
                    // set location to begining of next part to be searched
                    if (reverse)
                        searchCursorLocation = -1; // -1 = end of file
                    else
                        searchCursorLocation = 0;
                }
                else
                {
                    // increment cursor location so we search past the last found item
                    if (reverse)
                        --searchCursorLocation;
                    else
                        ++searchCursorLocation;
                }
            }


            // Compute search range
            // Note that Start < End, even when doing reverse search (-1 = end of file)
            int searchStart = 0;
            int searchEnd = -1;
            // First we search from cursor location.
            // Once we have reached the end of the file,
            // we search until the original start location
            // (we will be called a second time if we return "passed end of file")
            if (reverse)
            {
                searchEnd = searchCursorLocation;
                if (passedEndOfFile)
                    searchStart = currentSearchStart;
            }
            else
            {
                searchStart = searchCursorLocation;
                if (passedEndOfFile)
                    searchEnd = currentSearchStart;
            }

            // Perform actual search
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "About to search from {0}, to {1}", searchStart, searchEnd));
            if (searchStart != searchEnd)
            {
                result = textBox1.Find(pszSearch, searchStart, searchEnd, options);
            }
            else
            {
                result = -1;
            }

            // Set result value
            if (result != -1)
            {
                // We found the string
                pResult = (uint)__VSFINDRESULT.VSFR_Found;
            }
            else
            {
                // If we searched the whole file or this is not a selection search
                if (passedEndOfFile
                    || (grfOptions & (int)__VSFINDOPTIONS.FR_Selection) != 0)
                {
                    // We did not find anything
                    pResult = (uint)__VSFINDRESULT.VSFR_NotFound;
                    passedEndOfFile = false;
                    // if we did replace something before getting here (then this is a replace)
                    if (replaced)
                    {
                        replaced = false;
                        // adjust cursor final position
                        if ((grfOptions & (int)__VSFINDOPTIONS.FR_Backwards) != 0)
                        {
                            if  (textBox1.SelectionStart > 0)
                                textBox1.SelectionStart -= 1;
                        }
                        else if (textBox1.SelectionStart < (textBox1.Text.Length-1))
                            textBox1.SelectionStart += 1;
                    }
                }
                else
                {
                    // We did not find the string, but we reached the end of the file
                    // Let VS know, and we will get called a second time to search the rest of the document
                    pResult = (uint)__VSFINDRESULT.VSFR_EndOfDoc;
                    passedEndOfFile = true;
                    forceCursorWrapAround = true;
                }
            }

            // Save cursor position for next call
            searchCursorLocation = textBox1.SelectionStart;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Bring the focus to a specific position in the document
        /// </summary>
        /// <param name="pts">Location where to move the cursor to</param>
        public int NavigateTo(Microsoft.VisualStudio.TextManager.Interop.TextSpan[] pts)
        {
            int hr = VSConstants.S_OK;
            // Activate the window
            IVsWindowFrame frame = (IVsWindowFrame)GetVsService(typeof(SVsWindowFrame));
            if (frame != null)
            {
                hr = frame.Show();
                if ( ErrorHandler.Failed(hr) )
                    return hr;
            }
            else
                return VSConstants.E_NOTIMPL;

            // Now navigate to the specified location (if any)
            if (ErrorHandler.Succeeded(hr) && (pts.Length > 0))
            {
                // first set start location
                int newPosition = textBox1.GetCharIndexOfLine(pts[0].iStartLine);
                newPosition += pts[0].iStartIndex;
                if (newPosition > textBox1.Text.Length)
                    newPosition = textBox1.Text.Length;
                textBox1.SelectionStart = newPosition;

                // now set the length of the selection
                newPosition = textBox1.GetCharIndexOfLine(pts[0].iEndLine);
                newPosition += pts[0].iEndIndex;
                if (newPosition > textBox1.Text.Length)
                    newPosition = textBox1.Text.Length;
                int length = newPosition - textBox1.SelectionStart;
                if (length >= 0)
                    textBox1.SelectionLength = newPosition;
                else
                    textBox1.SelectionLength = 0;
            }
            return hr;
        }

        /// <summary>
        /// Get current cursor location
        /// </summary>
        /// <param name="pts">Current location</param>
        /// <returns>Hresult</returns>
        public int GetCurrentSpan(Microsoft.VisualStudio.TextManager.Interop.TextSpan[] pts)
        {
            Debug.Assert(pts.Length >0, "Array should not be empty");
            pts[0].iStartIndex = textBox1.SelectionStart;
            pts[0].iEndIndex = textBox1.SelectionStart + textBox1.SelectionLength;
            pts[0].iStartLine = 0;
            pts[0].iEndLine = 0;


            return VSConstants.S_OK;
        }

        public int MarkSpan(Microsoft.VisualStudio.TextManager.Interop.TextSpan[] pts)
        {
            return VSConstants.E_NOTIMPL; //"Only IVsTextImage providers implement this method"
        }

        public int Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced)
        {
            pfReplaced = 0;

            // If the selection is empty, exit
            if ( textBox1.SelectionLength == 0)
            {
                pfReplaced = 0;
                return VSConstants.S_OK;
            }

            // Get the selection
            string sel = textBox1.SelectedText;
            bool ignoreCase = (grfOptions & (int)__VSFINDOPTIONS.FR_MatchCase) == 0;

            // Check if the selection matchs the search
            if ( string.Compare(sel, pszSearch, ignoreCase, CultureInfo.CurrentCulture) == 0 )
            {
                // Recalculate search start position if we make the text longer/shorter
                if (textBox1.SelectionStart <= currentSearchStart)
                {
                    currentSearchStart += pszReplace.Length - sel.Length;
                }
                // Do the actual replace
                textBox1.SelectedText = pszReplace;
                // Adjust position so the next find is done correctly
                if ((grfOptions & (int)__VSFINDOPTIONS.FR_Backwards) != 0)
                {
                    textBox1.SelectionStart -= (pszReplace.Length);
                }
                else
                {
                    textBox1.SelectionStart -= 1;
                }
                searchCursorLocation = textBox1.SelectionStart;

                pfReplaced = 1;
                replaced = true;
            }
            else
            {
                pfReplaced = 0;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Store an object that will later be returned
        /// </summary>
        /// <returns>The object that is being stored</returns>
        public int SetFindState(object pUnk)
        {
            findState = pUnk;
            return VSConstants.S_OK;
        }


        /// <summary>
        /// This implementation does not use notification
        /// </summary>
        /// <param name="notification"></param>
        public int NotifyFindTarget(uint notification)
        {
            return VSConstants.S_OK;
        }


        /// <summary>
        /// Specify which search option we support.
        /// </summary>
        /// <param name="pfImage">Do we support IVsTextImage?</param>
        /// <param name="pgrfOptions">Supported options</param>
        public int GetCapabilities(bool[] pfImage, uint[] pgrfOptions)
        {
            // We do not support IVsTextImage
            if (pfImage != null && pfImage.Length > 0)
                pfImage[0] = false;

            if (pgrfOptions != null && pgrfOptions.Length > 0)
            {
                pgrfOptions[0] = (uint)__VSFINDOPTIONS.FR_Backwards;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_CommonOptions;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_Document;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_ActionMask;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_Plain;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_Project;
                pgrfOptions[0] |= (uint)__VSFINDOPTIONS.FR_Solution;
                // Only support selection if something is selected
                if (textBox1.SelectionLength == 0)
                    pgrfOptions[0] &= ~((uint)__VSFINDOPTIONS.FR_Selection);
            }

            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsFindTarget.GetMatchRect(RECT[] prc)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsDocDataFileChangeControl

        /// <summary>
        /// Called by the shell to notify if a file change must be ignored.
        /// </summary>
        /// <param name="fIgnore">Flag not zero if the file change must be ignored.</param>
        int IVsDocDataFileChangeControl.IgnoreFileChanges(int fIgnore)
        {
            if (0 != fIgnore)
            {
                // The changes must be ignored, so increase the counter of changes to ignore
                ++changesToIgnore;
            }
            else
            {
                if (changesToIgnore > 0)
                {
                    --changesToIgnore;
                }
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsFileChangeEvents

        /// <summary>
        /// Event called when a directory changes.
        /// </summary>
        /// <param name="pszDirectory">Path if the changed directory.</param>
        int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
        {
            // Do nothing: we are not interested in this event.
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Event called when there are file changes.
        /// </summary>
        /// <param name="cChanges">Number of files changed.</param>
        /// <param name="rgpszFile">Path of the files.</param>
        /// <param name="rggrfChange">Flags with the kind of changes.</param>
        int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile , uint[] rggrfChange)
        {
            // Check the number of changes.
            if (0 == cChanges)
            {
                // Why this event was called if there is no change???
                return VSConstants.E_UNEXPECTED;
            }

            // If the counter of the changes to ignore (set by IVsDocDataFileChangeControl.IgnoreFileChanges)
            // is zero we can process this set of changes, otherwise ignore it.
            if (0 != changesToIgnore)
                return VSConstants.S_OK;

            // Now scan the list of the changed files to find if the one opened in the editor
            // is one of the changed
            for (int i=0; i < cChanges; ++i)
            {
                if (string.Compare(fileName, rgpszFile[i], true, CultureInfo.InvariantCulture) == 0)
                {
                    // The file opened in the editor is changed.
                    // The first step now is to find the kind of change.
                    uint contentChange = (uint)_VSFILECHANGEFLAGS.VSFILECHG_Size | (uint)_VSFILECHANGEFLAGS.VSFILECHG_Time;
                    if ( (rggrfChange[i] & contentChange) != 0 )
                    {
                        // The content of the file is changed outside the editor, so we need to
                        // prompt the user for re-load the file. It is possible to have multiple
                        // notification for this change, but we want to prompt the user and reload
                        // the file only once
                        if ( reloadTimer.Enabled || isFileReloading)
                        {
                            // The reload is running, so we don't need to start it again.
                            return VSConstants.S_OK;
                        }

                        // Set the timer timeout and start it
                        reloadTimer.Interval = 500;
                        reloadTimer.Enabled = true;
                        reloadTimer.Start();
                    }
                }
            }

            return VSConstants.S_OK;
        }

        #endregion

        private void AdviseFileChange(bool subscribe)
        {
            // Get the VsFileChangeEx service; this service will call us back when a file system
            // event will occur on the file(s) that we register.
            IVsFileChangeEx fileChangeService = (IVsFileChangeEx)GetVsService(typeof(SVsFileChangeEx));
            int hr = VSConstants.S_OK;

            if ( !subscribe )
            {
                // If the goal is to unsubscribe, but there is no subscription active, exit.
                if (fileChangeNotifyCookie == VSConstants.VSCOOKIE_NIL)
                    return;

                // Now there is an active subscription, so unsubscribe.
                hr = fileChangeService.UnadviseFileChange(fileChangeNotifyCookie);
                // No more subscription active, so set the cookie to NIL
                fileChangeNotifyCookie = VSConstants.VSCOOKIE_NIL;
                ErrorHandler.ThrowOnFailure(hr);
                return;
            }

            // Here we want to subscribe for notification when the file opened in the editor changes.
            uint eventsToSubscribe = (uint)_VSFILECHANGEFLAGS.VSFILECHG_Size | 
                                     (uint)_VSFILECHANGEFLAGS.VSFILECHG_Time;
            hr = fileChangeService.AdviseFileChange(
                fileName,                        // The file to check
                eventsToSubscribe,                // Filter to use for the notification
                (IVsFileChangeEvents)this,        // The callback to call
                out fileChangeNotifyCookie);    // Cookie used to identify this subscription.
            ErrorHandler.ThrowOnFailure(hr);
        }

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
                        textBox1.Undo();
                        return;
                    }

                    // It is possible to change the file, so update the status.
                    isDirty = true;
                }
            }
        }
        private void reloadTimer_Tick(object sender, System.EventArgs e)
        {
            // Here we want to check if we can process the reload.
            // We don't want to show the reload popup when the shell is not the active window,
            // so as first we check if the shell has the focus.
            // To do so we use the Win32 function GetActiveWindow that will return the handle
            // of the active window in the current application and NULL (0) if no window has
            // the focus.
            if (0 != NativeMethods.GetActiveWindow())
            {
                try
                {
                    // Set the flag about the reload status. 
                    // It is important to avoid that the popup will show multiple times for the same file.
                    isFileReloading = true;

                    // The timer was used only to not run this procedure in syncronously inside the
                    // notification function, so now we can stop end disable it.
                    reloadTimer.Stop();
                    reloadTimer.Enabled = false;

                    // Build the title and message for the popup.
                    string message = fileName + "\n\n" + MyVSTools.GetResourceString("OutsideEditorFileChange");
                    string title = "Microsoft Visual Studio";
                    // Show the popup
                    System.Windows.Forms.DialogResult res = MessageBox.Show(
                        this, 
                        message, 
                        title, 
                        System.Windows.Forms.MessageBoxButtons.YesNo, 
                        System.Windows.Forms.MessageBoxIcon.Question);
                    if (res == System.Windows.Forms.DialogResult.Yes)
                    {
                        // The user wants to reload the data, so let's call the function that will do the job.
                        int hr = ((IVsPersistDocData)this).ReloadDocData(0);
                        // If this fail, this is not fatal, but we should try to understand what went wrong.
                        Debug.Assert(hr >= 0, "Failed to close IVsWindowFrame while disposing of the package");
                    }
                }
                finally
                {
                    // Reset the flag about the reload status.
                    isFileReloading = false;
                }
            }
        }

        internal static class NativeMethods
        {
                // We need this Win32 function to find out if the shell has the focus.
                [DllImport("user32.Dll")]
                static public extern int GetActiveWindow();
        }
    }
}
