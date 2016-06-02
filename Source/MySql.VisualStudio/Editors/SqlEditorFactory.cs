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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  [Guid(GuidStrings.SQL_EDITOR_FACTORY)]
  public sealed class SqlEditorFactory : IVsEditorFactory, IDisposable
  {
    private ServiceProvider _serviceProvider;

    #region IVsEditorFactory Members

    internal string LastDocumentPath { get; private set; }

    int IVsEditorFactory.Close()
    {
      return VSConstants.S_OK;
    }

    int IVsEditorFactory.CreateEditorInstance(uint grfCreateDoc, string pszMkDocument,
        string pszPhysicalView, IVsHierarchy pvHier, uint itemid,
        IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData,
        out string pbstrEditorCaption, out Guid pguidCmdUi, out int pgrfCdw)
    {
      string s;
      pvHier.GetCanonicalName(itemid, out s);
      pgrfCdw = 0;
      LastDocumentPath = pszMkDocument;
      pguidCmdUi = VSConstants.GUID_TextEditorFactory;

      ppunkDocData = IntPtr.Zero;
      ppunkDocView = IntPtr.Zero;
      WindowPane editor;

      FileInfo fileInfo = new FileInfo(LastDocumentPath);
      if (fileInfo.Extension.ToLower().Equals(".mysql"))
      {
        editor = new SqlEditorPane(_serviceProvider, this);
      }
      else
      {
        ScriptType scriptType = ScriptType.JavaScript;
        if (fileInfo.Extension.ToLower().Equals(".mypy"))
        {
          scriptType = ScriptType.Python;
        }


        editor = new MySqlHybridScriptEditorPane(_serviceProvider, this, scriptType);
      }
      if (editor.Window != null)
      {
        ppunkDocData = Marshal.GetIUnknownForObject(editor.Window);
        ppunkDocView = Marshal.GetIUnknownForObject(editor);
      }

      pbstrEditorCaption = "";
      return VSConstants.S_OK;
    }

    int IVsEditorFactory.MapLogicalView(ref Guid logicalView, out string physicalView)
    {
      physicalView = null;
      if (VSConstants.LOGVIEWID_Primary == logicalView)
      {
        // --- Primary view uses null as physicalView
        return VSConstants.S_OK;
      }
      else
      {
        // --- You must return E_NOTIMPL for any unrecognized logicalView values
        return VSConstants.E_NOTIMPL;
      }
    }

    int IVsEditorFactory.SetSite(IOleServiceProvider psp)
    {
      _serviceProvider = new ServiceProvider(psp);
      return VSConstants.S_OK;
    }

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        // --- Here we dispose all managed and unmanaged resources
        if (_serviceProvider != null)
        {
          _serviceProvider.Dispose();
          _serviceProvider = null;
        }
      }
    }

    #endregion
  }
}
