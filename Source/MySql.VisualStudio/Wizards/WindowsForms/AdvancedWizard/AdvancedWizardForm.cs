// Copyright (c) 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
  public partial class AdvancedWizardForm : BaseWizardForm
  {
    internal new string ConnectionString { get; set;}

    internal string TableName { get; set; }

    internal WindowsFormsWizard Wizard { get; set; }

    internal MySqlConnection Connection { get { return Wizard.Connection; } }
   
    internal string DetailTableName { get { return dataAccessTechnologyConfig1.DetailTableName; } }
    internal GuiType GuiType { 
      get { return dataAccessTechnologyConfig1.GuiType; }
      set { dataAccessTechnologyConfig1.GuiType = value; }
    }

    internal string ConstraintName { get { return dataAccessTechnologyConfig1.ConstraintName; } }
    
    internal List<ColumnValidation> ValidationColumns { get { return validationConfig1.ValidationColumns; } }
    
    internal List<ColumnValidation> ValidationColumnsDetail { get { return detailValidationConfig1.DetailValidationColumns; } }

    internal Dictionary<string, Column> Columns { get { return validationConfig1.Columns; } }

    internal Dictionary<string, Column> DetailColumns { get { return detailValidationConfig1.DetailColumns; } }

    internal Dictionary<string, ForeignKeyColumnInfo> ForeignKeys = new Dictionary<string, ForeignKeyColumnInfo>();

    internal Dictionary<string, ForeignKeyColumnInfo> DetailForeignKeys = new Dictionary<string, ForeignKeyColumnInfo>();
    
    internal bool ValidationsEnabled { get { return ValidationColumns != null; } }

    internal GuiType GuiTypeForTable
    {
      get
      {
        return dataAccessTechnologyConfig1.GuiType;
      }
      set
      {
        dataAccessTechnologyConfig1.GuiType = value;
      }
    }
        
    public AdvancedWizardForm(WindowsFormsWizard wizard)
    {
      this.Wizard = wizard;
      InitializeComponent();
    }

    private void AdvancedWizardForm_Load(object sender, EventArgs e)
    {
      Descriptions.Add("View type Selection,Select the type of view to use in the form generation.");
      Descriptions.Add("Columns Validation,This page allows you to customize input validations for each column in the selected table.");
      Descriptions.Add("Detail Columns Validation,Within this step validations can be added on the columns for the child related table.");
      WizardName = "Windows Forms Project";

      Pages = new List<WizardPage>();

      Pages.Add(dataAccessTechnologyConfig1);
      Pages.Add(validationConfig1);
      Pages.Add(detailValidationConfig1);

      CurPage = dataAccessTechnologyConfig1;
      Current = 0;
      BaseWizardForm_Load(sender, e);
      ShowFinishButton(true);    
    }

    internal void GenerateModels()
    {
      if( validationConfig1.Columns == null || validationConfig1.Columns.Count == 0)
        validationConfig1.GenerateModel(this);
      if( (this.GuiType == Wizards.GuiType.MasterDetail) && 
        (( detailValidationConfig1.DetailColumns == null) || (detailValidationConfig1.DetailColumns.Count == 0 )))
      {
        detailValidationConfig1.GenerateModel(this);
      }
    }

    internal override void btnCancel_Click(object sender, EventArgs e)
    {
      if (CurPage == dataAccessTechnologyConfig1)
      {
        if( GuiType == Wizards.GuiType.MasterDetail )
        {
          GuiType = Wizards.GuiType.IndividualControls;
        }
      }
      base.btnCancel_Click(sender, e);
    }
  }
}
