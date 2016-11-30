// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Constants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class serves as
  /// a) Command broker (subscribing once to the mappable keys of Visual Studio, instead of many times
  /// Solving a bug of backspace affecting the wrong mysql editorWindow window when more than one is open).
  /// b) A repository to serve the connections of each Editor for the Intellisense classifiers.
  /// </summary>
  internal class EditorBroker : IOleCommandTarget
  {
    private Dictionary<string, VSCodeEditorWindow> dic = new Dictionary<string, VSCodeEditorWindow>();
    private DTE Dte;
    private uint CmdTargetCookie;
    internal static EditorBroker Broker { get; private set; }

    private EditorBroker(ServiceBroker sb)
    {
      // Register priority command target, this dispatches mappable keys like Enter, Backspace, Arrows, etc.
      int hr = sb.VsRegisterPriorityCommandTarget.RegisterPriorityCommandTarget(0, (IOleCommandTarget)this, out this.CmdTargetCookie);
      if (hr != VSConstants.S_OK)
      {
        Marshal.ThrowExceptionForHR(hr);
      }

      Dte = (DTE)sb.Site.GetService(typeof(DTE));
    }

    // this method must be externally synchronized
    internal static void CreateSingleton(ServiceBroker sb)
    {
      if (Broker != null)
      {
        throw new InvalidOperationException("The singleton broker has alreaby been created.");
      }

      Broker = new EditorBroker(sb);
    }

    internal static void RegisterEditor(VSCodeEditorWindow editorWindow)
    {
      Broker.dic.Add(editorWindow.Parent.Editor.GetDocumentPath(), editorWindow);
    }

    internal static void UnregisterEditor(VSCodeEditorWindow editor)
    {
      Broker.dic.Remove(editor.Parent.Editor.GetDocumentPath());
    }

    /// <summary>
    /// Returns the full name of the DTE's active document.
    /// </summary>
    /// <returns></returns>
    internal string GetActiveDocumentFullName()
    {
      if (Dte.ActiveDocument == null)
      {
        return null;
      }

      return Dte.ActiveDocument.FullName;
    }

    /// <summary>
    /// Returns the DbConnection associated with the current mysql editorWindow.
    /// </summary>
    /// <returns></returns>
    internal DbConnection GetCurrentConnection()
    {
      VSCodeEditorWindow editor;
      if (Dte.ActiveDocument == null)
      {
        return null;
      }

      dic.TryGetValue(Dte.ActiveDocument.FullName, out editor);
      // Null here means No connection opened for the current mysql editorWindow, or current active window not a mysql editorWindow.
      return editor == null ? null : editor.Parent.Editor.Connection;
    }

    /// <summary>
    /// Returns the current database, as per the last query executed.
    /// </summary>
    /// <returns></returns>
    internal string GetCurrentDatabase()
    {
      VSCodeEditorWindow editor;
      if (Dte.ActiveDocument == null)
      {
        return null;
      }

      dic.TryGetValue(Dte.ActiveDocument.FullName, out editor);
      // Null here means No connection opened for the current mysql editorWindow, or current active window not a mysql editorWindow.
      return editor == null ? null : editor.Parent.Editor.CurrentDatabase;
    }

    public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      VSCodeEditorWindow editor;
      if (Dte.ActiveDocument == null)
      {
        return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
      }

      if (dic.TryGetValue(Dte.ActiveDocument.FullName, out editor))
      {
        return ((IOleCommandTarget)editor).Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }

      return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
    }

    public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      VSCodeEditorWindow editor;
      if (Dte.ActiveDocument == null)
      {
        return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
      }

      if (dic.TryGetValue(Dte.ActiveDocument.FullName, out editor))
      {
        return ((IOleCommandTarget)editor).QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
      }

      return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
    }

    /// <summary>
    /// Unregisters the editor with the old document path and re-registers the same editor but with an updated document path.
    /// </summary>
    /// <param name="oldDocumentPath">Old document path set when the script was first created.</param>
    /// <param name="newDocumentPath">New document path set when the script was saved for the first time.</param>
    internal static void UpdateEditorDocumentPath(string oldDocumentPath, string newDocumentPath)
    {
      VSCodeEditorWindow editor;

      if (oldDocumentPath!=null && Broker.dic.TryGetValue(oldDocumentPath, out editor))
      {
        UnregisterEditor(editor);
        editor.Parent.Editor.SetDocumentPath(newDocumentPath);
        RegisterEditor(editor);
      }
    }
  }
}
