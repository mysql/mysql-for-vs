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
    #region Fields
    /// <summary>
    /// The VSCodeEditorWindow native window
    /// </summary>
    private VSCodeEditorWindow _nativeWindow;

    /// <summary>
    /// The base editor
    /// </summary>
    internal BaseEditorControl Editor;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the Text field of the core editor
    /// </summary>
    public override string Text
    {
      get { return _nativeWindow != null && _nativeWindow.CoreEditor != null ? _nativeWindow.CoreEditor.Text : string.Empty; }
      set
      {
        if (_nativeWindow != null && _nativeWindow.CoreEditor != null)
        {
          _nativeWindow.CoreEditor.Text = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
    /// </value>
    public bool IsDirty
    {
      get { return _nativeWindow != null && _nativeWindow.CoreEditor != null ? _nativeWindow.CoreEditor.Dirty : false; }
      set
      {
        if (_nativeWindow != null && _nativeWindow.CoreEditor != null)
        {
          _nativeWindow.CoreEditor.Dirty = value;
        }
      }
    }
    #endregion

    /// <summary>
    /// Initializes the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="editor">The generic editor.</param>
    public void Init(ServiceProvider serviceProvider, BaseEditorControl editor)
    {
      Editor = editor;
      ServiceBroker sb = new ServiceBroker(serviceProvider);
      _nativeWindow = new VSCodeEditorWindow(sb, this);
    }

    /// <summary>
    /// Dispose override to clean up the native window.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!disposing) return;
        if (_nativeWindow == null) return;
        _nativeWindow.Dispose();
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    /// <summary>
    /// Registers the editor to re-add handles and events to the window.
    /// </summary>
    public void RegisterEditor()
    {
      if (_nativeWindow != null)
      {
        EditorBroker.RegisterEditor(_nativeWindow);
      }
    }

    /// <summary>
    /// Unregister the editor to delete the handles and events to the window.
    /// </summary>
    public void UnregisterEditor()
    {
      if (_nativeWindow != null)
      {
        EditorBroker.UnregisterEditor(_nativeWindow);
      }
    }

    /// <summary>
    /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
    /// </summary>
    /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values.</param>
    /// <returns>
    /// true if the specified key is a regular input key; otherwise, false.
    /// </returns>
    protected override bool IsInputKey(Keys keyData)
    {
      // Since we process each pressed keystroke, the return value is always true.
      return true;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnGotFocus(EventArgs e)
    {
      if (_nativeWindow == null)
      {
        return;
      }

      _nativeWindow.SetFocus();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected override void OnSizeChanged(EventArgs e)
    {
      if (_nativeWindow == null)
      {
        return;
      }

      _nativeWindow.SetWindowPos(ClientRectangle);
    }
  }
}
