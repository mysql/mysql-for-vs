using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using MySql.Data.MySqlClient;
using VSLangProj;
using MySql.Data.VisualStudio.WebConfig;
using MySql.Web.Security;
using System.Collections.Specialized;
using System.Web.Security;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public class WebWizard : BaseWizard<WebWizardForm>
  {    
    private VSProject _vsProj;    
    //private string _modelName;
    //private DataEntityVersion _dataEntityVersion;
    //private List<DbTables> _selectedTables;

    public WebWizard() : base()
    {
      WizardForm = new WebWizardForm();
    }

    public override void ProjectFinishedGenerating(Project project)
    {
      if (project != null)
      {
        _vsProj = project.Object as VSProject;
        var tables = new List<string>();

        if (WizardForm.selectedTables != null && WizardForm.dEVersion != DataEntityVersion.None)
        {
          WizardForm.selectedTables.ForEach(t => tables.Add(t.Name));

          if (tables.Count > 0)
            GenerateEntityFrameworkModel(_vsProj, "", new MySqlConnection(WizardForm.connectionStringForModel), WizardForm.modelName, tables);
        }
        var webConfig = new MySql.Data.VisualStudio.WebConfig.WebConfig(ProjectPath + @"\web.config");

        try
        {
          // add creation of providers tables
          if (WizardForm.includeProfilesProvider)
          {
            var profileConfig = new ProfileConfig();
            //Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
            profileConfig.Initialize(webConfig);
            profileConfig.Enabled = true;
            profileConfig.DefaultProvider = "MySQLProfileProvider";

            var options = new Options();
            options.AppName = @"\";
            options.AutoGenSchema = true;
            options.ConnectionStringName = WizardForm.connectionStringName;
            options.ConnectionString = WizardForm.connectionStringForModel;
            options.EnableExpireCallback = false;
            options.ProviderName = "MySQLProfileProvider";
            options.WriteExceptionToLog = WizardForm.writeExceptionsToLog;
            profileConfig.GenericOptions = options;
            profileConfig.DefaultProvider = "MySQLProfileProvider";
            profileConfig.Save(webConfig);
          }
          if (WizardForm.includeRoleProvider)
          {
            var roleConfig = new RoleConfig();
            roleConfig.Initialize(webConfig);
            roleConfig.Enabled = true;
            roleConfig.DefaultProvider = "MySQLRoleProvider";

            var options = new Options();
            options.AppName = @"\";
            options.AutoGenSchema = true;
            options.ConnectionStringName = WizardForm.connectionStringName;
            options.ConnectionString = WizardForm.connectionStringForModel;
            options.EnableExpireCallback = false;
            options.ProviderName = "MySQLRoleProvider";
            options.WriteExceptionToLog = WizardForm.writeExceptionsToLog;
            roleConfig.GenericOptions = options;
            roleConfig.DefaultProvider = "MySQLRoleProvider";
            roleConfig.Save(webConfig);
          }
          webConfig.Save();
          try
          {
            if (WizardForm.createAdministratorUser)
            {
              var provider = new MySQLMembershipProvider();
              NameValueCollection config = new NameValueCollection();
              config.Add("connectionStringName", WizardForm.connectionStringName);
              config.Add("connectionString", WizardForm.connectionStringForModel);
              config.Add("writeExceptionsToEventLog", "false");
              config.Add("autogenerateschema", "true");
              config.Add("applicationName", "/");
              provider.Initialize("MySQLMembershipProvider", config);

               //create the user
              MembershipCreateStatus status;
              if (!WizardForm.requireQuestionAndAnswer)
              {
                provider.CreateUser("administrator", WizardForm.adminPassword, "temporary@email.com", null, null, true, null, out status);
              }
              else
              {
                provider.CreateUser("administrator", WizardForm.adminPassword, "temporary@email.com", "Set question", "Set answer", true, null, out status);
              }
            }          
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.Message, "An error occured when creating user", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
          
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }      
      }
      WizardForm.Dispose();
    }


    public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
    {
      DialogResult result = WizardForm.ShowDialog();
     
      if (result == DialogResult.Cancel) throw new WizardCancelledException();

      if (!WizardForm.includeSensitiveInformation)
      {
        var csb = new MySqlConnectionStringBuilder(WizardForm.connectionStringForModel);
        csb.Password = "";
        replacementsDictionary.Add("$connectionstring$", csb.ConnectionString);        
      }
      else
      {
        replacementsDictionary.Add("$connectionstring$", WizardForm.connectionStringForModel);        
      }

      replacementsDictionary.Add("$requirequestionandanswer$", WizardForm.requireQuestionAndAnswer ? "True" : "False");
      replacementsDictionary.Add("$minimumrequiredlength$", WizardForm.minimumPasswordLenght.ToString());
      replacementsDictionary.Add("$writeExceptionstoeventlog$", WizardForm.writeExceptionsToLog ? "True" : "False");
      replacementsDictionary.Add("$connectionstringname$", WizardForm.connectionStringName);
      ProjectPath = replacementsDictionary["$destinationdirectory$"];      
      ProjectNamespace = GetCanonicalIdentifier(replacementsDictionary["$safeprojectname$"]);
      NetFxVersion = replacementsDictionary["$targetframeworkversion$"];
    }
  }
}
