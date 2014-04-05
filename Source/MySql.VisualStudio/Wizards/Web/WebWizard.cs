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
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using MySql.Data.MySqlClient;
using VSLangProj;
using MySql.Data.VisualStudio.WebConfig;
using MySql.Web.Security;
using System.Collections.Specialized;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Reflection;

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
              string configPath = ProjectPath + @"\web.config";

              using (AppConfig.Load(configPath))
              {
                var configFile = new FileInfo(configPath);
                var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
                var wcfm = new WebConfigurationFileMap();
                wcfm.VirtualDirectories.Add("/", vdm);
                System.Configuration.Configuration config = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
                MembershipSection section = (MembershipSection)config.GetSection("system.web/membership");

                ProviderSettingsCollection settings = section.Providers;
                NameValueCollection membershipParams = settings[section.DefaultProvider].Parameters;
                var provider = new MySQLMembershipProvider();
                provider.Initialize(section.DefaultProvider, membershipParams);
              
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


  /// <summary>
  /// Util class to change the AppDomain when loading the
  /// web.config file of the new application.
  /// </summary>
  public abstract class AppConfig : IDisposable
  {
    public static AppConfig Load(string path)
    {
      return new LoadAppConfig(path);
    }

    public abstract void Dispose();

    private class LoadAppConfig : AppConfig
    {
      private readonly string oldConfig =
          AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

      private bool disposedValue;

      public LoadAppConfig(string path)
      {
        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
        ResetConfig();
      }

      public override void Dispose()
      {
        if (!disposedValue)
        {
          AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", oldConfig);
          ResetConfig();
          disposedValue = true;
        }
        GC.SuppressFinalize(this);
      }

      private static void ResetConfig()
      {
        typeof(System.Configuration.ConfigurationManager)
            .GetField("s_initState", BindingFlags.NonPublic |
                                     BindingFlags.Static)
            .SetValue(null, 0);

        typeof(System.Configuration.ConfigurationManager)
            .GetField("s_configSystem", BindingFlags.NonPublic |
                                        BindingFlags.Static)
            .SetValue(null, null);

        typeof(System.Configuration.ConfigurationManager)
            .Assembly.GetTypes()
            .Where(x => x.FullName ==
                        "System.Configuration.ClientConfigPaths")
            .First()
            .GetField("s_current", BindingFlags.NonPublic |
                                   BindingFlags.Static)
            .SetValue(null, null);
      }
    }
  }



}
