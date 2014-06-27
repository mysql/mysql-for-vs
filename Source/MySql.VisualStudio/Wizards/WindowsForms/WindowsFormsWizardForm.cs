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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class WindowsFormsWizardForm : BaseWizardForm
  {
   
    #region "Properties exposed"  
    
    internal MySqlConnection Connection {
      get {
        if (string.IsNullOrEmpty(dataAccessConfig1.connectionString))
          return null;
        return new MySqlConnection(dataAccessConfig1.connectionString); 
      }
    }

    internal string ConnectionName { 
        get{
          return dataAccessConfig1.connectionName;        
        }
    }

    internal DataAccessTechnology DataAccessTechnology
    {
      get {
        return dataAccessConfig1.DataAccessTechnology;
      }
    }

   internal Dictionary<string, AdvancedWizardForm> dicConfig
    {
      get {
        return tablesSelection1.dicConfig;
      }      
    }
  

    internal override string ConnectionString
    {
      get { return this.Wizard.GetConnectionStringWithPassword(Connection); }
    }

    #endregion

    //internal const int DATA_ACCESS_CONFIG_PAGE_IDX = 0;
    internal const int DATA_ACCESS_TECHNOLOGY_CONFIG_PAGE_IDX = 0;
    internal const int VALIDATION_CONFIG_PAGE_IDX = 1;
    internal const int DETAIL_VALIDATION_CONFIG_PAGE_IDX = 2;

    internal protected WindowsFormsWizard Wizard = null;

    public WindowsFormsWizardForm(WindowsFormsWizard Wizard)
      : base()
    {
      this.Wizard = Wizard;
      InitializeComponent();
    }

    private void WizardForm_Load(object sender, EventArgs e)
    {
      // set up descriptions and title
      Descriptions.Add("Data Source Configuration,This wizard will create a full Windows Forms project connected to an existing MySQL database using Entity Framework or ADO.NET Typed Datasets.");
      Descriptions.Add("Database objects selection,Please select the tables that you want to include in the generation of your model");
      WizardName = "Windows Forms Project";
            
      // Create linked list of wizard pages.
      Pages.Add(dataAccessConfig1);
      Pages.Add(tablesSelection1);
      
      CurPage = dataAccessConfig1;
      Current = 0;
      BaseWizardForm_Load(sender, e);
    }
  }
}
