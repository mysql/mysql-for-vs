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
