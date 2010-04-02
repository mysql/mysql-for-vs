// Copyright © 2010, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell;

namespace MySql.Data.VisualStudio.Editors
{
    internal class VSCodeEditorUserControl : UserControl
    {
        private VSCodeEditorWindow nativeWindow;

        public void Init(ServiceProvider serviceProvider)
        {
            ServiceBroker sb = new ServiceBroker(serviceProvider);
            nativeWindow = new VSCodeEditorWindow(sb, this);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing) return;
                if (nativeWindow == null) return;
                nativeWindow.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public string Text
        {
            get { return nativeWindow.CoreEditor.Text; }
            set { nativeWindow.CoreEditor.Text = value; }
        }

        public bool IsDirty
        {
            get { return nativeWindow.CoreEditor.Dirty; }
            set { nativeWindow.CoreEditor.Dirty = value; }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // Since we process each pressed keystroke, the return value is always true.
            return true;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (nativeWindow == null) return;
            nativeWindow.SetFocus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (nativeWindow == null) return;
            nativeWindow.SetWindowPos(ClientRectangle);
        }
    }
}
