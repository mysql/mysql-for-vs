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

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class WebWizardForm : BaseWizardForm
  {

    #region "Properties exposed"

    internal string connectionStringForModel
    {
      get {
        return providerConfiguration1.connectionString;
      }    
    }

    internal string connectionStringName
    {
      get {

        return providerConfiguration1.connectionStringName;
      }    
    }

    internal bool includeProfilesProvider
    {
      get {
        return providerConfiguration1.includeProfileProvider;      
      }
    }

    internal bool includeRoleProvider
    {
      get {
        return providerConfiguration1.includeRoleProvider;      
      }      
    }


    internal string modelName
    {
      get {
        return modelConfiguration1.modelName;
      }
    
    }

    internal DataEntityVersion dEVersion
    {
      get {
        return modelConfiguration1.dEVersion;
       }
    }

    internal List<DbTables> selectedTables
    {
      get {
        return modelConfiguration1.selectedTables;
      }
    }

    internal bool includeSensitiveInformation
    {
      get
      {
        return modelConfiguration1.includeSensitiveInformation;
      }    
    }


    internal bool writeExceptionsToLog
    {
      get {
        return providerConfiguration1.writeExceptionsToLog;
      }
    }

    internal int minimumPasswordLenght
    {
      get {
        return providerConfiguration1.minimumPasswordLenght;      
      }    
    }

    internal bool requireQuestionAndAnswer
    {
      get {
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

    internal string adminPassword
    {
      get
      {
        return providerConfiguration1.adminPassword;
      }
    }

    #endregion

    public WebWizardForm()
    {
      InitializeComponent();
    }

    private void WebWizardForm_Load(object sender, EventArgs e)
    {
      // Create linked list of wizard pages.
      Pages.Add(providerConfiguration1);
      Pages.Add(modelConfiguration1);
      CurPage = providerConfiguration1;
      Current = 0;

      BaseWizardForm_Load(sender, e);
    }
  }
}
