// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// VSCodeEditorUserControl class serves as a bridge between the BaseEditorControl object and the VSCodeEditorWindow.
  /// </summary>
  internal class VSCodeEditorUserControl : UserControl
  {
    private VSCodeEditorWindow nativeWindow;
    internal BaseEditorControl Editor;

    /// <summary>
    /// Initializes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="editor">The generic editor.</param>
    public void Init(ServiceProvider serviceProvider, BaseEditorControl editor)
    {
      Editor = editor;
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

    public override string Text
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
