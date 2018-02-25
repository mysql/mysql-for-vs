// Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Data.VisualStudio.DBExport
{
    [Guid("4469031d-23e0-483c-8566-ce978f6c9a6f")]
  public class DbExportWindowPane : ToolWindowPane, IVsWindowFrameNotify2
    {
        public dbExportPanel DbExportPanelControl;
        public List<IVsDataExplorerConnection> Connections {get; set;}
        public string SelectedConnectionName { get; set; }
        public ToolWindowPane WindowHandler { get; set; }

        public DbExportWindowPane() : base(null)
        {
          DbExportPanelControl = new dbExportPanel();            
        }

        public void InitializeDbExportPanel()
        {
          DbExportPanelControl.LoadConnections(Connections, SelectedConnectionName, WindowHandler);        
        }

        override public IWin32Window Window
        {
          get { return (IWin32Window)DbExportPanelControl; }
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
          if (WindowHandler.Caption.Contains("*"))
          {
            if (MessageBox.Show("Do you want to save the selected settings?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
              DbExportPanelControl.SaveSettings(true);
            }
          }
          return VSConstants.S_OK;
        }
    }
}
