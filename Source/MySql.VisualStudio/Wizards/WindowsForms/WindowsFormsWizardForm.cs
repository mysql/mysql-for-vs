// Copyright (c) 2008, 2015, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Data.MySqlClient;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class WindowsFormsWizardForm : BaseWizardForm
  {

    #region "Properties exposed"

    internal MySqlConnection Connection {
      get
      {
        return string.IsNullOrEmpty(dataAccessConfig1.connectionString)
          ? null
          : new MySqlConnection(dataAccessConfig1.connectionString);
      }
    }

    internal string ConnectionName
    { 
      get
      {
        return dataAccessConfig1.connectionName;
      }
    }

    internal DataAccessTechnology DataAccessTechnology
    {
      get
      {
        return dataAccessConfig1.DataAccessTechnology;
      }
    }

    internal override string ConnectionString
    {
      get
      {
        return Wizard.GetConnectionStringWithPassword(Connection);
      }
    }

    internal string ConnectionStringWithIncludedPassword
    {
      get
      {
        if (IncludePassword)
        {
          return Wizard.GetConnectionStringWithPassword(Connection);
        }

        var msb = new MySqlConnectionStringBuilder(Connection.ConnectionString) { Password = string.Empty };
        return msb.ToString();
      }
    }

    internal bool IncludePassword
    {
      get
      {
        return dataAccessConfig1.IncludePassword;
      }
    }

    internal Dictionary<string, AdvancedWizardForm> CrudConfiguration
    {
      get
      {
        return tablesSelection1.DicConfig;
      }
    }

    internal List<DbTables> SelectedTables
    {
      get
      {
        return tablesSelection1.selectedTables;
      }
    }

    #endregion

    //internal const int DATA_ACCESS_CONFIG_PAGE_IDX = 0;
    internal const int DATA_ACCESS_TECHNOLOGY_CONFIG_PAGE_IDX = 0;
    internal const int VALIDATION_CONFIG_PAGE_IDX = 1;
    internal const int DETAIL_VALIDATION_CONFIG_PAGE_IDX = 2;

    internal protected WindowsFormsWizard Wizard = null;

    public WindowsFormsWizardForm(WindowsFormsWizard wizard)
    {
      Wizard = wizard;
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
