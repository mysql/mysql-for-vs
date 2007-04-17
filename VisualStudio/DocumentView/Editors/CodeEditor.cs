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
 * This file contains astub for future smart SQL editor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Descriptors;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using Microsoft.VisualStudio.OLE.Interop;

namespace MySql.Data.VisualStudio.DocumentView.Editors
{
    /// <summary>
    /// This is a stub for future smart code editor with syntax highlighting.
    /// </summary>
    //[ViewObject(ViewDescriptor.TypeName, typeof(CodeEditor))]
    class CodeEditor : IVsWindowPane, IEditor
    {
        private IVsWindowPane codeWindow;

        public CodeEditor(IDocument document)
        {
            Package package = MySqlDataProviderPackage.Instance;
            Type codeWindowType = typeof(IVsCodeWindow);
            Guid riid = codeWindowType.GUID;
            Guid clsid = typeof(VsCodeWindowClass).GUID;
            IVsCodeWindow window = (IVsCodeWindow)package.CreateInstance(ref clsid, ref riid, codeWindowType);
            codeWindow = (IVsWindowPane)window;

            IVsTextLines textLines;
            // Create a new IVsTextLines buffer.
            Type textLinesType = typeof(IVsTextLines);
            riid = textLinesType.GUID;
            clsid = typeof(VsTextBufferClass).GUID;
            textLines = package.CreateInstance(ref clsid, ref riid, textLinesType) as IVsTextLines;

            String test = "            Package package = MySqlDataProviderPackage.Instance;";
            textLines.InitializeContent(test, test.Length);

            String editorCaption;

            ErrorHandler.ThrowOnFailure(window.SetBuffer(textLines));
            ErrorHandler.ThrowOnFailure(window.SetBaseEditorCaption(null));
            ErrorHandler.ThrowOnFailure(window.GetEditorCaption(READONLYSTATUS.ROSTATUS_Unknown, out editorCaption));
        }

        #region IVsWindowPane Members

        public int ClosePane()
        {
            return codeWindow.ClosePane();
        }

        public int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
        {
            return codeWindow.CreatePaneWindow(hwndParent, x, y, cx, cy, out hwnd);
        }

        public int GetDefaultSize(Microsoft.VisualStudio.OLE.Interop.SIZE[] pSize)
        {
            return codeWindow.GetDefaultSize(pSize);
        }

        public int LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
        {
            return codeWindow.LoadViewState(pStream);
        }

        public int SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
        {
            return codeWindow.SaveViewState(pStream);
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return codeWindow.SetSite(psp);
        }

        public int TranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
        {
            foreach (MSG message in lpmsg)
            {
            }
            return codeWindow.TranslateAccelerator(lpmsg);
        }

        #endregion

        #region IEditor Members

        public IVsWindowFrame OwnerFrame
        {
            get
            {
                return null;
                
            }
            set
            {
                
            }
        }

        public Guid CommandGroupID
        {
            get { return GuidList.guidMySqlProviderCmdSet; }
        }

        public IDocument Document
        {
            get { return null; }
        }

        #endregion
    }
}
