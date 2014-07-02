// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Wizards.Web;
using MySql.Data.MySqlClient;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  internal class TablesSelectionWinForms : MySql.Data.VisualStudio.Wizards.Web.TablesSelection
  {

    internal WindowsFormsWizard Wizard { get; set; }

    internal TablesSelectionWinForms() : base()
    {     
    }

    internal Dictionary<string, AdvancedWizardForm> dicConfig;

    internal override void TablesFilled()
    {
      dicConfig = new Dictionary<string, AdvancedWizardForm>();
      MySqlConnection con = new MySqlConnection(base.ConnectionString);
      con.Open();
      Wizard.Connection = con;
      foreach(DbTables t in base._tables)
      {
        dicConfig.Add(t.Name, new AdvancedWizardForm(Wizard) { TableName = t.Name });
      }

      base.TablesFilled();
    }

    internal override void chkSelectAllTables_CheckedChanged(object sender, EventArgs e)
    {           
        _tables.Where(t => t.Selected == !chkSelectAllTables.Checked).ToList().ForEach(t =>
        {
          t.CheckObject(chkSelectAllTables.Checked);
          AdvancedWizardForm dlgAdvanced = null;
          dicConfig.TryGetValue(t.Name, out dlgAdvanced);
          if (dlgAdvanced != null)
          {
            if (chkSelectAllTables.Checked)
            {
              dlgAdvanced.GuiTypeForTable = GuiType.IndividualControls;
            }
            else
            {
              dlgAdvanced.GuiTypeForTable = GuiType.None;
            }
          }
        });

        // view type set to default value
        for(int i = 0; i < listTables.Rows.Count; i++)
        {
          if (chkSelectAllTables.Checked)
          {        
            if (String.IsNullOrEmpty(listTables.Rows[i].Cells[3].Value as String))
            {
              listTables.Rows[i].Cells[3].Value = "Single column layout";          
            }
          }
          else
            listTables.Rows[i].Cells[3].Value = "";          
        }
      
      listTables.Refresh();
    }

    internal override void listTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {      
      if (e.ColumnIndex == 2)
      {
        string tableName = (string)(listTables.CurrentRow.Cells[1].Value);
        ShowAdvancedForm(tableName, e.RowIndex);
      }
      else
      {
        base.listTables_CellContentClick(sender, e);
      }

      if (e.ColumnIndex == 0)
      {
        if (!_tables.Single(t => t.Name.Equals((string)listTables.Rows[e.RowIndex].Cells[1].Value)).Selected)
        {
          listTables.Rows[e.RowIndex].Cells[3].Value = String.Empty;
        }
        else
        {
          listTables.Rows[e.RowIndex].Cells[3].Value = "Single column layout";
          AdvancedWizardForm dlg = dicConfig[(string)listTables.Rows[e.RowIndex].Cells[1].Value];
          dlg.GuiTypeForTable = GuiType.IndividualControls;
        }
      }
    }

    private void ShowAdvancedForm(string tableName, int rowIndex)
    {
      AdvancedWizardForm dlg = dicConfig[tableName];
      dlg.TableName = tableName;
      dlg.ConnectionString = base.ConnectionString;
      if (dlg.ShowDialog() == DialogResult.Cancel) return;      
      _tables.Where(t => t.Name.Equals(tableName)).Single().CheckObject(true);

      switch (dlg.GuiTypeForTable)
	    {		    
          case GuiType.IndividualControls:
            listTables.Rows[rowIndex].Cells[3].Value = "Single column layout";
            break;
          case GuiType.Grid:
            listTables.Rows[rowIndex].Cells[3].Value = "Grid layout";
           break;
          case GuiType.MasterDetail:
           listTables.Rows[rowIndex].Cells[3].Value = String.Format("Master-Detail layout, {0}", dlg.DetailTableName);
           break;
        case GuiType.None:
           listTables.Rows[rowIndex].Cells[3].Value = string.Empty;
           break;
       }      
    }

    internal override void AddMoreColumns()
    {
      base.AddMoreColumns();

      listTables.Columns[1].Width = 120;

      DataGridViewButtonColumn colCustomize = new DataGridViewButtonColumn();
      colCustomize.UseColumnTextForButtonValue = false;
      colCustomize.ReadOnly = false;
      colCustomize.Text = "...";
      colCustomize.HeaderText = "Customize";
      colCustomize.Name = "colCustomize";
      colCustomize.Width = 90;
      listTables.Columns.Add(colCustomize);

      DataGridViewTextBoxColumn colSummary = new DataGridViewTextBoxColumn();
      colSummary.HeaderText = "Selected view";
      colSummary.Name = "colSummary";
      colSummary.Width = 180;
      listTables.Columns.Add(colSummary);
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      Wizard = ((WindowsFormsWizardForm)wizard).Wizard;
      base.OnStarting(wizard);
    }
  }
}
