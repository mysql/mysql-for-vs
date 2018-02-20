// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using Microsoft.VisualStudio.Data.Services;
using EnvDTE;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class WebWizardForm : BaseWizardForm
  {    

    #region "Properties exposed"

    string _connectionStringSafeForAspNet = string.Empty;
     
     internal string serverExplorerConnectionSelected
     {
       get {
          return dataSourceConfiguration1.selectedConnection;
        }
      }

    internal string connectionStringForAspNetTables
    {
      get
      {
        if (string.IsNullOrEmpty(_connectionStringSafeForAspNet))        
          _connectionStringSafeForAspNet = dataSourceConfiguration1.connectionString;
          return _connectionStringSafeForAspNet;        
      }
      set
      {
        _connectionStringSafeForAspNet = value;
      }
    }

    internal string connectionStringNameForAspNetTables
    {
      get
      {
        return dataSourceConfiguration1.connectionStringName;
      }    
    }
    
    
    internal string connectionStringForModel
    {
      get
      {
        return modelConfiguration1.connectionString;
      }
    }

    internal override string ConnectionString
    {
      get { return connectionStringForModel; }
    }

    internal string connectionStringNameForModel
    {
      get
      {

        return modelConfiguration1.connectionStringName;
      }
    }

    internal bool includeProfilesProvider
    {
      get
      {
        return dataSourceConfiguration1.includeProfileProvider;
      }
    }

    internal bool includeRoleProvider
    {
      get
      {
        return dataSourceConfiguration1.includeRoleProvider;
      }
    }


    internal string modelName
    {
      get
      {
        return modelConfiguration1.modelName;
      }

    }

    internal DataEntityVersion dEVersion
    {
      get
      {
        return modelConfiguration1.dEVersion;
      }
    }

    internal List<DbTables> selectedTables
    {
      get
      {
        return tablesSelection1.selectedTables;        
      }
    }

    internal bool includeSensitiveInformation
    {
      get
      {
        return dataSourceConfiguration1.includeSensitiveInformation;
      }
    }

    internal bool writeExceptionsToLog
    {
      get
      {
        return providerConfiguration1.writeExceptionsToLog;
      }
    }

    internal int minimumPasswordLenght
    {
      get
      {
        return providerConfiguration1.minimumPasswordLenght;
      }
    }

    internal bool requireQuestionAndAnswer
    {
      get
      {
        return providerConfiguration1.requireQuestionAndAnswer;
      }
    }

    internal bool createAdministratorUser
    {
      get
      {
        return providerConfiguration1.createAdministratorUser;
      }
    }

    internal string adminName
    {
      get 
      {
        return providerConfiguration1.adminName;
      }
    }

    internal string adminPassword
    {
      get
      {
        return providerConfiguration1.adminPassword;
      }
    }

    internal string userQuestion
    {
      get {
        return providerConfiguration1.userQuestion;
      }    
    }

    internal string userAnswer
    {
      get {
        return providerConfiguration1.userAnswer;
      }
    }


    #endregion

    internal protected WebWizard Wizard = null;


    public WebWizardForm(WebWizard Wizard)
      : base()
    {
      this.Wizard = Wizard;
      InitializeComponent();
    }


    private void WebWizardForm_Load(object sender, EventArgs e)
    {
      // set up descriptions and title
      Descriptions.Add("Data Source Configuration,This wizard will create a full MVC project connected to a MySQL database existing or will create a new one with a web site that includes user authentication with the ASP.NET MySQL Membership provider.");
      Descriptions.Add("Provider Settings Configuration,Please select the correspondant settings that will be applied for the ASP.NET MySQL Membership provider.");
      Descriptions.Add("Model Connection String Settings,Please select the correspondant settings to use in the Connection String for your Data Entity Model.");
      Descriptions.Add("Database objects selection,Please select the tables that you want to include in the generation of your model");
      WizardName = "ASP.net MVC Project";


      // Create linked list of wizard pages.      
      Pages.Add(dataSourceConfiguration1);
      Pages.Add(providerConfiguration1);
      Pages.Add(modelConfiguration1);
      Pages.Add(tablesSelection1);
      CurPage = dataSourceConfiguration1;
      Current = 0;      
      BaseWizardForm_Load(sender, e);      
    }
  }
}
