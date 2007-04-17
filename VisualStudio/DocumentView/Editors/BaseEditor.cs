// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of the base class for all editors
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using Microsoft.VisualStudio;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using VSStd97CmdID = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using MySql.Data.VisualStudio.Properties;


namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is the base class for all editors. It implements IEditor 
    /// interface and supports several useful additional methods to 
    /// use and override.
    /// </summary>
    public class BaseEditor : System.Windows.Forms.UserControl,
                                IEditor,
                                IOleCommandTarget,                                
                                IVsFindTarget
    {
        #region Initialization
        /// <summary>
        /// Default constructor for form designer.
        /// </summary>
        protected BaseEditor()
        {
        }

        /// <summary>
        /// Constructs and initializes editor.
        /// </summary>        
        /// <param name="document">Reference to document object.</param>
        protected BaseEditor(IDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            this.documentRef = document;
            Document.DataLoaded += new EventHandler(OnDocumentDataLoaded);
            Document.DataChanged += new EventHandler(OnDocumentDataChanged);
            Document.Saving += new CancelEventHandler(OnDocumentSaving);
            Document.SuccessfullySaved += new EventHandler(OnDocumentSuccessfullySaved);
            Document.SaveAttemptFailed += new EventHandler(OnDocumentSaveAttemptFailed);

            // Initialize selection container
            selectionContainerRef.SelectableObjects = SelectableObjects;
            selectionContainerRef.SelectedObjects = SelectedObjects;

            // Add document to selection
            SelectableObjects.Add(Document);
        }
        #endregion
        
        #region Virtual methods
        /// <summary>
        /// This method is called when database object changes are successfully saved. 
        /// Base implementation does nothing, but successors might add functionality, 
        /// if necessary.
        /// </summary>
        protected virtual void AcceptChanges()
        {
        }

        /// <summary>
        /// This method is called to perform successor specific data 
        /// view initialization. Base class calls this method from 
        /// DataLoaded event handler.
        /// </summary>
        protected virtual void InitializeView()
        {
            SelectObject(Document);
            RefreshView();
        }

        /// <summary>
        /// Loads configuration settings
        /// </summary>
        protected virtual void LoadSettings()
        {
        }

        /// <summary>
        /// This method is called then data representaion need to be
        /// refreshed. It is called after initialization, after any
        /// document changes and after successfull save.
        /// </summary>
        protected virtual void RefreshView()
        {
        }

        /// <summary>
        /// Saves configuration settings
        /// </summary>
        protected virtual void SaveSettings()
        {
        }

        /// <summary>
        /// This method is called before object changes are saved in database.
        /// Successor views should flush all cached changes in overridden method.
        /// </summary>
        /// <returns>
        /// Returns false if changes can't be flushed. In this case saving will terminates.
        /// </returns>
        protected virtual bool ValidateAndFlushChanges()
        {
            return true;
        }
        #endregion

        #region Property Window integration
        /// <summary>
        /// Collection of objects which can be selected in the property window.
        /// </summary>
        protected IList SelectedObjects
        {
            get
            {
                return selectedObjectsArray;
            }
        }

        /// <summary>
        /// Collection of objects which are selected in the property window.
        /// </summary>
        protected IList SelectableObjects
        {
            get
            {
                return selectableObjectsArray;
            }
        }

        /// <summary>
        /// This method is used to select single object for property window.
        /// </summary>
        /// <param name="selection">Object to select. Must be a member of SelectableObjects list.</param>
        protected void SelectObject(object selection)
        {
            SelectedObjects.Clear();
            if (selection != null)
            {
                if (!SelectableObjects.Contains(selection))
                {
                    Debug.Fail("Trying to select object which is not in the selectable collection!");
                    return;
                }
                SelectedObjects.Add(selection);
            }
        }
            
        #endregion

        #region Properties
        /// <summary>
        /// Reference to the owned windows frame. Used to change caption.
        /// </summary>
        public IVsWindowFrame OwnerFrame
        {
            get
            {
                return ownerFrameRef;
            }
            set
            {
                ownerFrameRef = value;
                if (ownerFrameRef != null)
                    RefreshCaption();
            }
        }

        /// <summary>
        /// Command group unique identifier for this editor, used 
        /// to register editor in RDT.
        /// </summary>
        public Guid CommandGroupID
        {
            get
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Reference to the document which is displayed by this view.
        /// </summary>
        public IDocument Document
        {
            get
            {
                return documentRef;
            }
        }

        /// <summary>
        /// Owner caption for window frame
        /// </summary>
        protected virtual string OwnerCaption
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture, Resources.OwnerCaption_Template, 
                    Document.Server, Document.Schema);                
            }
        }

        /// <summary>
        /// Editor caption for window frame
        /// </summary>
        protected virtual string EditorCaption
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture, Resources.EditorCaption_Template, Document.Name);
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Retrieves the requested service from the Shell.
        /// </summary>
        /// <param name="serviceType">Service that is being requested</param>
        /// <returns>An object which type is as requested</returns>
        protected object GetVsService(Type serviceType)
        {
            return vsServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// This method refreshes caption of the Visual Studio editor. 
        /// Used during initialization and after object is saved.
        /// </summary>
        protected void RefreshCaption()
        {
            Debug.Assert(OwnerFrame != null, "Frame is not initialized!");
            if (OwnerFrame == null)
                return;
            
            // Extract current caption first
            object result;

            // Extract owner caption
            result = null;
            OwnerFrame.GetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, out result);
            string ownerCaption = result is string ? result as string : String.Empty;

            // Extract editor caption
            result = null;
            OwnerFrame.GetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, out result);
            string editorCaption = result is string ? result as string : String.Empty;

            // Change frame caption if should be changed
            if (!StringComparer.CurrentCulture.Equals(ownerCaption, OwnerCaption))
                OwnerFrame.SetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, OwnerCaption);
            if (!StringComparer.CurrentCulture.Equals(editorCaption, EditorCaption))
                OwnerFrame.SetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, EditorCaption);            
        }

        /// <summary>
        /// Refreshes content of the properties window.
        /// </summary>
        protected void RefreshProperties()
        {
            if (TrackSelection != null)
            {
                TrackSelection.OnSelectChange(null);
                TrackSelection.OnSelectChange(SelectionContainer);
            }
        }
        #endregion
        
        #region Document event handlers
        /// <summary>
        /// Handles DataLoaded event and calls virtual initialization routine.
        /// </summary>
        private void OnDocumentDataLoaded(object sender, EventArgs e)
        {
            InitializeView();
        }

        /// <summary>
        /// Handles DataLoaded event and calls virtual initialization routine.
        /// </summary>
        private void OnDocumentDataChanged(object sender, EventArgs e)
        {
            RefreshView();            
        }
        
        /// <summary>
        /// Handles Saving event and calls proper virtual method.
        /// </summary>
        private void OnDocumentSaving(object sender, CancelEventArgs e)
        {
            if( e != null && !e.Cancel )
                e.Cancel = !ValidateAndFlushChanges();
        }
        
        /// <summary>
        /// Handles SuccessfullySaved event and calls proper virtual method.
        /// </summary>
        private void OnDocumentSuccessfullySaved(object sender, EventArgs e)
        {
            AcceptChanges();
            RefreshCaption();
            RefreshView();
            RefreshProperties();
        }

        /// <summary>
        /// Handles failures on saving and reflect possible changes into view.
        /// </summary>
        void OnDocumentSaveAttemptFailed(object sender, EventArgs e)
        {
            RefreshCaption();
            RefreshView();
            RefreshProperties();
        }
        #endregion

        #region Overridings
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
        #endregion

        #region Private properties
        /// <summary>
        /// Track selection service.
        /// </summary>
        private ITrackSelection TrackSelection
        {
            get
            {
                if (trackSelectionRef == null)
                    trackSelectionRef = GetVsService(typeof(STrackSelection)) as ITrackSelection;
                Debug.Assert(trackSelectionRef != null, "Failed to get track selection interface!");
                return trackSelectionRef;
            }
        }

        /// <summary>
        /// Internal selection container
        /// </summary>
        private SelectionContainer SelectionContainer
        {
            get
            {
                return selectionContainerRef;
            }
        }
        #endregion

        #region Private variables to store properties
        private readonly IDocument documentRef;
        private IVsWindowFrame ownerFrameRef;
        private ITrackSelection trackSelectionRef;
        private readonly SelectionContainer selectionContainerRef = new SelectionContainer();
        private readonly ArrayList selectableObjectsArray = new ArrayList();
        private readonly ArrayList selectedObjectsArray = new ArrayList();
        #endregion

        // TODO: Review, cleanup and implement methods bellow.

        #region Fields
        /// <summary>
        /// Reference to service provider in which this object is sited.
        /// </summary>
        private ServiceProvider vsServiceProvider;

        // Find and replace related variables
        private object findState = null;
        /*private bool passedEndOfFile;
        private bool forceNewSearch;
        private int currentSearchStart;
        private int searchCursorLocation;
        private bool forceCursorWrapAround;
        private bool replaced;*/

        private const char EndLine = (char)10;
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
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, System.IntPtr pCmdText)
        {
            Debug.Assert(cCmds == 1, "Multiple commands");
            Debug.Assert(prgCmds!=null, "NULL argument");

            if ((prgCmds == null))
                return VSConstants.E_INVALIDARG;

            OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
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
                    /*case (uint)VSStd97CmdID.Cut:
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
                    }*/
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (pguidCmdGroup == GuidList.guidTableEditorTestCmdSet)
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
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, System.IntPtr pvaIn, System.IntPtr pvaOut)
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Exec() of: {0}", this.ToString()));

            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                // Process standard Visual Studio Commands
                switch (nCmdID)
                {
                    /*case (uint)VSStd97CmdID.Copy:
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
                    }*/
                    default:
                        return (int)(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED);
                }
            }
            else if (pguidCmdGroup == GuidList.guidTableEditorTestCmdSet)
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

            //return VSConstants.S_OK;
        }
        #endregion

        #region IVsWindowPane Members

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        int IVsWindowPane.TranslateAccelerator(MSG[] lpmsg)
        {
            return VSConstants.S_FALSE;
        }

        int IVsWindowPane.SaveViewState(IStream pStream)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowPane.LoadViewState(IStream pStream)
        {
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

        /// <summary>
        /// Creates a window pane
        /// </summary>
        /// <returns>Return code</returns>
        int IVsWindowPane.CreatePaneWindow(System.IntPtr hwndParent, int x, int y, int cx, int cy, out System.IntPtr hwnd)
        {
            Win32Methods.SetParent(Handle, hwndParent);
            hwnd = Handle;

            Debug.Assert(TrackSelection != null, "Unable to register in property window!");
            if (TrackSelection != null)
                TrackSelection.OnSelectChange(SelectionContainer);

            Size = new System.Drawing.Size(cx - x, cy - y);

            // Loading configuration settings
            LoadSettings();

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Closes a window pane
        /// </summary>
        /// <returns>Return code</returns>
        public int ClosePane()
        {
            // Saving configuration settings
            SaveSettings();

            this.Dispose(true);
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
                /*case (uint)__VSFTPROPID.VSFTPROPID_DocName:
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
                }*/
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
        public int GetFindState(out object ppunk)
        {
            ppunk = findState;
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
            /*int result = 0;
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
            searchCursorLocation = textBox1.SelectionStart;*/
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
            /*if (ErrorHandler.Succeeded(hr) && (pts.Length > 0))
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
            }*/
            return hr;
        }

        /// <summary>
        /// Get current cursor location
        /// </summary>
        /// <param name="pts">Current location</param>
        /// <returns>Hresult</returns>
        public int GetCurrentSpan(Microsoft.VisualStudio.TextManager.Interop.TextSpan[] pts)
        {
            /*Debug.Assert(pts.Length >0, "Array should not be empty");
            pts[0].iStartIndex = textBox1.SelectionStart;
            pts[0].iEndIndex = textBox1.SelectionStart + textBox1.SelectionLength;
            pts[0].iStartLine = 0;
            pts[0].iEndLine = 0;*/


            return VSConstants.S_OK;
        }

        public int MarkSpan(Microsoft.VisualStudio.TextManager.Interop.TextSpan[] pts)
        {
            return VSConstants.E_NOTIMPL; //"Only IVsTextImage providers implement this method"
        }

        public int Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced)
        {
            pfReplaced = 0;

            /*// If the selection is empty, exit
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
            }*/

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
                /*// Only support selection if something is selected
                if (textBox1.SelectionLength == 0)
                    pgrfOptions[0] &= ~((uint)__VSFINDOPTIONS.FR_Selection);*/
            }

            return VSConstants.S_OK;
        }

        int Microsoft.VisualStudio.TextManager.Interop.IVsFindTarget.GetMatchRect(RECT[] prc)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseEditor
            // 
            this.Name = "BaseEditor";
            this.ResumeLayout(false);

        }
    }
}
