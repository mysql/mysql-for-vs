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

using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Data.Services;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DBExport
{
    [Guid("4469031d-23e0-483c-8566-ce978f6c9a6f")]
  public class DbExportWindowPane : ToolWindowPane, IVsWindowFrameNotify2
    {
        public DbExportPanel DbExportPanelControl;
        public List<IVsDataExplorerConnection> Connections {get; set;}
        public string SelectedConnectionName { get; set; }
        public ToolWindowPane WindowHandler { get; set; }

        public DbExportWindowPane() : base(null)
        {
          DbExportPanelControl = new DbExportPanel();            
        }

        public void InitializeDbExportPanel()
        {
          DbExportPanelControl.LoadConnections(Connections, SelectedConnectionName, WindowHandler);        
        }

        public override IWin32Window Window
        {
          get { return DbExportPanelControl; }
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
          if (WindowHandler.Caption.Contains("*"))
          {
            if (MessageBox.Show(Resources.SaveSelectedSettingsText, Resources.MySqlDataProviderPackage_Information, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
              DbExportPanelControl.SaveSettings(true);
            }
          }
          return VSConstants.S_OK;
        }
    }
}
