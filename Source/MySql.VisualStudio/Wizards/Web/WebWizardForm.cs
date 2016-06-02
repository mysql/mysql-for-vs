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
using System.Collections.Generic;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class WebWizardForm : BaseWizardForm
  {
    #region "Properties exposed"

    string _connectionStringSafeForAspNet = string.Empty;
     
     internal string ServerExplorerConnectionSelected
     {
       get {
          return dataSourceConfiguration1.SelectedConnection;
        }
      }

    internal string ConnectionStringForAspNetTables
    {
      get
      {
        if (string.IsNullOrEmpty(_connectionStringSafeForAspNet))        
          _connectionStringSafeForAspNet = dataSourceConfiguration1.ConnectionString;
          return _connectionStringSafeForAspNet;        
      }
      set
      {
        _connectionStringSafeForAspNet = value;
      }
    }

    internal string ConnectionStringNameForAspNetTables
    {
      get
      {
        return dataSourceConfiguration1.ConnectionStringName;
      }    
    }
    
    
    internal string ConnectionStringForModel
    {
      get
      {
        return modelConfiguration1.ConnectionString;
      }
    }

    internal override string ConnectionString
    {
      get { return ConnectionStringForModel; }
    }

    internal string ConnectionStringNameForModel
    {
      get
      {

        return modelConfiguration1.ConnectionStringName;
      }
    }

    internal bool IncludeProfilesProvider
    {
      get
      {
        return dataSourceConfiguration1.IncludeProfileProvider;
      }
    }

    internal bool IncludeRoleProvider
    {
      get
      {
        return dataSourceConfiguration1.IncludeRoleProvider;
      }
    }

    internal string ModelName
    {
      get
      {
        return modelConfiguration1.ModelName;
      }

    }

    internal DataEntityVersion DEVersion
    {
      get
      {
        return modelConfiguration1.DEVersion;
      }
    }

    internal List<DbTables> SelectedTables
    {
      get
      {
        return tablesSelection1.selectedTables;        
      }
    }

    internal bool IncludeSensitiveInformation
    {
      get
      {
        return dataSourceConfiguration1.IncludeSensitiveInformation;
      }
    }

    internal bool WriteExceptionsToLog
    {
      get
      {
        return providerConfiguration1.writeExceptionsToLog;
      }
    }

    internal int MinimumPasswordLenght
    {
      get
      {
        return providerConfiguration1.minimumPasswordLenght;
      }
    }

    internal bool RequireQuestionAndAnswer
    {
      get
      {
        return providerConfiguration1.requireQuestionAndAnswer;
      }
    }

    internal bool CreateAdministratorUser
    {
      get
      {
        return providerConfiguration1.createAdministratorUser;
      }
    }

    internal string AdminName
    {
      get 
      {
        return providerConfiguration1.adminName;
      }
    }

    internal string AdminPassword
    {
      get
      {
        return providerConfiguration1.adminPassword;
      }
    }

    internal string UserQuestion
    {
      get {
        return providerConfiguration1.userQuestion;
      }    
    }

    internal string UserAnswer
    {
      get {
        return providerConfiguration1.userAnswer;
      }
    }

    #endregion

    protected internal WebWizard Wizard;

    public WebWizardForm(WebWizard wizard)
    {
      Wizard = wizard;
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
