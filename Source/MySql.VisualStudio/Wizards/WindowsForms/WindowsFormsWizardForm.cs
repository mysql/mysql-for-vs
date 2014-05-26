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

    internal GuiType GuiType { get { return dataAccessTechnologyConfig1.GuiType; } }    

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

    internal string TableName { get { return dataAccessConfig1.TableName; } }

    internal DataAccessTechnology DataAccessTechnology { get { return dataAccessTechnologyConfig1.DataAccessTechnology; } }

    internal string ConstraintName { get { return dataAccessTechnologyConfig1.ConstraintName; } }

    internal string DetailTableName { get { return dataAccessTechnologyConfig1.DetailTableName; } }

    internal List<ColumnValidation> ValidationColumns { get { return validationConfig1.ValidationColumns; } }

    internal List<ColumnValidation> ValidationColumnsDetail { get { return detailValidationConfig1.DetailValidationColumns; } }

    internal Dictionary<string, Column> Columns { get { return validationConfig1.Columns; } }

    internal Dictionary<string, Column> DetailColumns { get { return detailValidationConfig1.DetailColumns; } }

    #endregion

    internal const int DATA_ACCESS_CONFIG_PAGE_IDX = 0;
    internal const int DATA_ACCESS_TECHNOLOGY_CONFIG_PAGE_IDX = 1;
    internal const int VALIDATION_CONFIG_PAGE_IDX = 2;
    internal const int DETAIL_VALIDATION_CONFIG_PAGE_IDX = 3;

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
      Descriptions.Add("Data Source Configuration,This wizard will create a full Windows Forms project connected to an existing MySQL database using either Entity Framework or ADO.NET Typed Datasets as data access and one of several layouts (Individual Controls, Data Grid, Master Detail).");
      Descriptions.Add("Data Access Technology Configuration,This step will set up the data access technology that will be used in the generation of the Form");      
      Descriptions.Add("Columns Validation,This page allows you to customize input validations for each column in the selected table.");
      Descriptions.Add("Detail Columns Validation, Within this step validations can be added on the columns for the child related table.");
      WizardName = "Windows Forms Project";

      // Create linked list of wizard pages.
      Pages.Add(dataAccessConfig1);
      Pages.Add(dataAccessTechnologyConfig1);
      Pages.Add(validationConfig1);
      Pages.Add(detailValidationConfig1);
      CurPage = dataAccessConfig1;
      Current = 0;
      BaseWizardForm_Load(sender, e);
    }
  }
}
