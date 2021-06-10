// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Data.Services;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Windows;
using System.Windows.Interop;

namespace MySql.Data.VisualStudio.DBExport
{
  [Guid("4469031d-23e0-483c-8566-ce978f6c9a6f")]
  public class DbExportWindowPane : ToolWindowPane, IVsWindowFrameNotify2
  {
    /// <summary>
    /// The Export Panel control that will be displayed to export MySQL data.
    /// </summary>
#if NET_472_OR_GREATER
    public DbExportPanelWPF DbExportPanelControl;
#else
    public dbExportPanel DbExportPanelControl;
#endif
    public List<IVsDataExplorerConnection> Connections {get; set;}
    public string SelectedConnectionName { get; set; }
    public ToolWindowPane WindowHandler { get; set; }

    public DbExportWindowPane() : base(null)
    {
#if NET_472_OR_GREATER
      DbExportPanelControl = new DbExportPanelWPF();
#else
      DbExportPanelControl = new dbExportPanel();
#endif
    }

    public void InitializeDbExportPanel()
    {
#if NET_472_OR_GREATER
      DbExportPanelControl.DbExportPanel.LoadConnections(Connections, SelectedConnectionName, WindowHandler);
#else
      DbExportPanelControl.LoadConnections(Connections, SelectedConnectionName, WindowHandler);
#endif
    }

    override public System.Windows.Forms.IWin32Window Window
    {
#if NET_472_OR_GREATER
      get
      {
        var helper = new WindowInteropHelper(DbExportPanelControl);
        if (helper.Handle == IntPtr.Zero)
        {
          DbExportPanelControl.Show();
        }

        var win32Window = new WindowWrapper(helper.Handle);
        return win32Window;
      }
#else
      get { return (System.Windows.Forms.IWin32Window)DbExportPanelControl; }
#endif
    }

    public int OnClose(ref uint pgrfSaveOptions)
    {
      if (WindowHandler.Caption.Contains("*"))
      {
        using (var yesNoDialog = Common.Utilities.GetYesNoInfoDialog(
                                   Utility.Forms.InfoDialog.InfoType.Info,
                                   false,
                                   "Save Settings",
                                   "Do you want to save the selected settings?"

        ))
        {
          if (yesNoDialog.ShowDialog() == DialogResult.Yes)
          {
#if NET_472_OR_GREATER
            DbExportPanelControl.DbExportPanel.SaveSettings(true);
#else
            DbExportPanelControl.SaveSettings(true);
#endif
          }
        }
      }

      return VSConstants.S_OK;
    }
  }
}
