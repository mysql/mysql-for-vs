﻿// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Data.VisualStudio.Wizards;
using MySql.Web.Security;
using System.Collections.Specialized;
using System.Web.Security;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Reflection;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Data.Services;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.VisualStudio.Properties;
#if CLR4 || NET_40_OR_GREATER
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
#endif
using Microsoft.VisualStudio.Shell;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public class WebWizard : BaseWizard<WebWizardForm, BaseCodeGeneratorStrategy>
  {

    public WebWizard(LanguageGenerator language): base(language)
    {
      WizardForm = new WebWizardForm(this);
      projectType = ProjectWizardType.AspNetMVC;
    }

    public override void ProjectFinishedGenerating(Project project)
    {
      if (project != null)
      {
        VSProject vsProj = project.Object as VSProject;
        var tables = new List<string>();

        Settings.Default.MVCWizardConnection = WizardForm.serverExplorerConnectionSelected;
        Settings.Default.Save();

        if (_generalPane != null)
            _generalPane.Activate();

        SendToGeneralOutputWindow("Starting project generation...");

        //Updating project references
        vsProj.References.Add("MySql.Data");

        double version = double.Parse(WizardForm.Wizard.GetVisualStudioVersion());
        if (version >= 12.0)
        {
          References refs = vsProj.References;
          var i = 0;
          foreach (Reference item in refs)
          {
            switch (item.Name)
            {            
              case "System.Web.Razor":
                if (item.Version.Equals("1.0.0.0"))
                  vsProj.References.Item(i).Remove();
                break;            
              case "System.Web.WebPages":               
                vsProj.References.Item(i).Remove();
              break;
            }
            i++;
          }
          
          vsProj.References.Add("System.Web.Mvc");          
          vsProj.References.Add("System.Web.Helpers");
          vsProj.References.Add("System.Web.Razor");          
        }


        if (WizardForm.selectedTables != null && WizardForm.dEVersion != DataEntityVersion.None)
        {
          WizardForm.selectedTables.ForEach(t => tables.Add(t.Name));

          SendToGeneralOutputWindow("Generating Entity Framework model...");
          if (tables.Count > 0)
          {
                         
            if (WizardForm.dEVersion == DataEntityVersion.EntityFramework5)
              CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_5;
            else if (WizardForm.dEVersion == DataEntityVersion.EntityFramework6)
             CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_6;                        

            AddNugetPackage(vsProj, ENTITY_FRAMEWORK_PCK_NAME, CurrentEntityFrameworkVersion);
            GenerateEntityFrameworkModel(vsProj, new MySqlConnection(WizardForm.connectionStringForModel), WizardForm.modelName, tables);         
            GenerateMVCItems(vsProj);

            if (WizardForm.dEVersion == DataEntityVersion.EntityFramework6)
            {
              project.DTE.SuppressUI = true;
              project.Properties.Item("TargetFrameworkMoniker").Value = ".NETFramework,Version=v4.5";
            }

          }
        }
        var webConfig = new MySql.Data.VisualStudio.WebConfig.WebConfig(ProjectPath + @"\web.config");
        SendToGeneralOutputWindow("Starting provider configuration...");
        try
        {
          // add creation of providers tables
          if (WizardForm.includeProfilesProvider)
          {
            var profileConfig = new ProfileConfig();            
            profileConfig.Initialize(webConfig);
            profileConfig.Enabled = true;
            profileConfig.DefaultProvider = "MySQLProfileProvider";

            var options = new Options();
            options.AppName = @"\";
            options.AutoGenSchema = true;
            options.ConnectionStringName = WizardForm.connectionStringNameForAspNetTables;
            options.ConnectionString = WizardForm.connectionStringForAspNetTables;
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
            options.ConnectionStringName = WizardForm.connectionStringNameForAspNetTables;
            options.ConnectionString = WizardForm.connectionStringForAspNetTables;
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
              SendToGeneralOutputWindow("Creating administrator user...");
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
                  provider.CreateUser("administrator", WizardForm.adminPassword, "temporary@email.com", WizardForm.userQuestion, WizardForm.userAnswer, true, null, out status);
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
      SendToGeneralOutputWindow("Finished project generation.");
      WizardForm.Dispose();
    }


    public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
    {
      Dte = automationObject as DTE;

      connections = MySqlServerExplorerConnections.LoadMySqlConnectionsFromServerExplorer(Dte);
      WizardForm.connections = this.connections;
      WizardForm.dte = this.Dte;

      DialogResult result = WizardForm.ShowDialog();

      if (result == DialogResult.Cancel) throw new WizardCancelledException();

      var connectionstringForModel = string.Empty;

      if (!WizardForm.includeSensitiveInformation)
      {
        // connectionstringformodel
        var csb = new MySqlConnectionStringBuilder(WizardForm.connectionStringForModel);        
        csb.Password = null;        
        connectionstringForModel = string.Format(@"<add name=""{0}Entities"" connectionString=""metadata=res://*/Models.{0}.csdl|res://*/Models.{0}.ssdl|res://*/Models.{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />", WizardForm.connectionStringNameForModel, csb.ConnectionString);        
        // connectionstringforaspnet        
        csb = new MySqlConnectionStringBuilder(WizardForm.connectionStringForAspNetTables);
        csb.Password = null;
        replacementsDictionary.Add("$connectionstringforaspnettables$", csb.ConnectionString);        
      }
      else
      {
        connectionstringForModel = string.Format(@"<add name=""{0}Entities"" connectionString=""metadata=res://*/Models.{0}.csdl|res://*/Models.{0}.ssdl|res://*/Models.{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />", WizardForm.connectionStringNameForModel, WizardForm.connectionStringForModel);        
        replacementsDictionary.Add("$connectionstringforaspnettables$", WizardForm.connectionStringForAspNetTables);
      }

      replacementsDictionary.Add("$connectionstringnameformodel$", WizardForm.dEVersion != DataEntityVersion.None ? connectionstringForModel : string.Empty);
      replacementsDictionary.Add("$connectionstringnameforaspnettables$", WizardForm.connectionStringNameForAspNetTables);
      replacementsDictionary.Add("$EntityFrameworkReference$", WizardForm.dEVersion != DataEntityVersion.None ? @"<add assembly=""System.Data.Entity, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089""/>" : string.Empty);
      replacementsDictionary.Add("$requirequestionandanswer$", WizardForm.requireQuestionAndAnswer ? "True" : "False");
      replacementsDictionary.Add("$minimumrequiredlength$", WizardForm.minimumPasswordLenght.ToString());
      replacementsDictionary.Add("$writeExceptionstoeventlog$", WizardForm.writeExceptionsToLog ? "True" : "False");      
      replacementsDictionary.Add("$providerReference$", WizardForm.dEVersion == DataEntityVersion.EntityFramework6 ? @"<entityFramework> <providers> <provider invariantName=""MySql.Data.MySqlClient"" type=""MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF6"" /></providers> </entityFramework>" : string.Empty);

      switch (WizardForm.dEVersion)
      {
        case DataEntityVersion.None:
          replacementsDictionary.Add("$EntityFrameworkVersion$", string.Empty);
          break;
        case DataEntityVersion.EntityFramework5:
          replacementsDictionary.Add("$EntityFrameworkVersion$", @"<section name=""entityFramework"" type=""System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=4.4.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false""/>");
          break;
        case DataEntityVersion.EntityFramework6:
          replacementsDictionary.Add("$EntityFrameworkVersion$", @"<section name=""entityFramework"" type=""System.Data.Entinotepadty.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false""/>");
          break;
        default:
          break;
      }
      
      StringBuilder catalogs = new StringBuilder();

      if (WizardForm.dEVersion != DataEntityVersion.None)
      {
        catalogs = new StringBuilder("<h3> Catalog list</h3>");
        catalogs.AppendLine();

        foreach (var table in WizardForm.selectedTables)
        {           
          catalogs.AppendLine(string.Format(@"<div> @Html.ActionLink(""{0}"",""Index"", ""{0}"")</div>", table.Name[0].ToString().ToUpperInvariant() + table.Name.Substring(1)));
        }                
      }

      replacementsDictionary.Add("$catalogList$", catalogs.ToString());      
      ProjectPath = replacementsDictionary["$destinationdirectory$"];
      ProjectNamespace = GetCanonicalIdentifier(replacementsDictionary["$safeprojectname$"]);
      NetFxVersion = replacementsDictionary["$targetframeworkversion$"];

      double version = double.Parse(WizardForm.Wizard.GetVisualStudioVersion());
      replacementsDictionary.Add("$webpages$", version >= 12.0 ? "2.0.0.0" : "1.0.0.0");
      replacementsDictionary.Add("$SystemWebHelpers$", version >= 12.0 ? "2.0.0.0" : "1.0.0.0");
      replacementsDictionary.Add("$SystemWebMvc$", version >= 12.0 ? "4.0.0.0" : "3.0.0.0");
      replacementsDictionary.Add("$mvcbindingRedirect$", version >= 12.0 ? "4.0.0.0" : "3.0.0.0");      
      
      var requiredquestionandanswer = string.Empty;
      if (WizardForm.requireQuestionAndAnswer)
        requiredquestionandanswer = WizardForm.Wizard.Language == LanguageGenerator.CSharp ?  "[Required]" : "<Required()> _";          
      replacementsDictionary.Add("$requiredquestionandanswer$", requiredquestionandanswer);
    }

    private void GenerateMVCItems(VSProject vsProj)
    { 
      if (string.IsNullOrEmpty(WizardForm.connectionStringForModel))
       return;

      if (WizardForm.selectedTables == null || WizardForm.selectedTables.Count == 0)
        return;

#if CLR4 || NET_40_OR_GREATER
      IServiceProvider serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)Dte);
      ITextTemplating t4 = serviceProvider.GetService(typeof(STextTemplating)) as ITextTemplating;
      ITextTemplatingSessionHost sessionHost = t4 as ITextTemplatingSessionHost;
      
      var controllerClassPath = string.Empty;        
      var IndexFilePath = string.Empty;
      var fileExtension = string.Empty;
      Version productVersion = Assembly.GetExecutingAssembly().GetName().Version;
      var version = String.Format("{0}.{1}.{2}", productVersion.Major, productVersion.Minor, productVersion.Build);

      if (Language == LanguageGenerator.CSharp)
      {
        controllerClassPath = Path.GetFullPath(@"..\IDE\Extensions\Oracle\MySQL for Visual Studio\" + version + @"\T4Templates\CSharp\CSharpControllerClass.tt");
        IndexFilePath = Path.GetFullPath(@"..\IDE\Extensions\Oracle\MySQL for Visual Studio\" + version + @"\T4Templates\CSharp\CSharpIndexFile.tt");
        fileExtension = "cs";
      }
      else
      {
        controllerClassPath = Path.GetFullPath(@"..\IDE\Extensions\Oracle\MySQL for Visual Studio\" + version + @"\T4Templates\VisualBasic\VisualBasicControllerClass.tt");
        IndexFilePath = Path.GetFullPath(@"..\IDE\Extensions\Oracle\MySQL for Visual Studio\" + version + @"\T4Templates\VisualBasic\VisualBasicIndexFile.tt");
        fileExtension = "vb";
      }

      foreach (var table in WizardForm.selectedTables)
      {
         // creating controller file
          sessionHost.Session = sessionHost.CreateSession();
          sessionHost.Session["namespaceParameter"] = string.Format("{0}.Controllers", ProjectNamespace);
          sessionHost.Session["applicationNamespaceParameter"] = ProjectNamespace;
          sessionHost.Session["controllerClassParameter"] = string.Format("{0}Controller", table.Name[0].ToString().ToUpperInvariant() + table.Name.Substring(1));
          sessionHost.Session["modelNameParameter"] = string.Format("{0}Entities", WizardForm.connectionStringNameForModel);
          sessionHost.Session["classNameParameter"] = table.Name;
          sessionHost.Session["entityNameParameter"] = table.Name[0].ToString().ToUpperInvariant() + table.Name.Substring(1);
          sessionHost.Session["entityClassNameParameter"] = string.Format("{0}.{1}", ProjectNamespace, table.Name);

          T4Callback cb = new T4Callback();          
          string resultControllerFile = t4.ProcessTemplate(controllerClassPath, File.ReadAllText(controllerClassPath), cb);
          string controllerFilePath = ProjectPath + string.Format(@"\Controllers\{0}Controller.{1}", table.Name[0].ToString().ToUpperInvariant() + table.Name.Substring(1), fileExtension);
          File.WriteAllText(controllerFilePath, resultControllerFile);
          if (cb.errorMessages.Count > 0)
          {
            File.AppendAllLines(controllerFilePath, cb.errorMessages);
          }

          vsProj.Project.ProjectItems.AddFromFile(controllerFilePath);         

          var viewPath = Path.GetFullPath(ProjectPath + string.Format(@"\Views\{0}", table.Name[0].ToString().ToUpperInvariant() + table.Name.Substring(1)));  
          Directory.CreateDirectory(viewPath);          
          string resultViewFile = t4.ProcessTemplate(IndexFilePath, File.ReadAllText(IndexFilePath), cb);
          File.WriteAllText(string.Format(viewPath + @"\Index.{0}html",fileExtension), resultViewFile);
          if (cb.errorMessages.Count > 0)
          {
            File.AppendAllLines(controllerFilePath, cb.errorMessages);
          }
          vsProj.Project.ProjectItems.AddFromFile(string.Format(viewPath + @"\Index.{0}html", fileExtension));                
      }
#endif
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

#if CLR4 || NET_40_OR_GREATER
  public class T4Callback : ITextTemplatingCallback
  {
    public List<string> errorMessages = new List<string>();
    public string fileExtension = ".txt";
    public Encoding outputEncoding = Encoding.UTF8;

    public void ErrorCallback(bool warning, string message, int line, int column)
    { errorMessages.Add(message); }

    public void SetFileExtension(string extension)
    { fileExtension = extension; }

    public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
    { outputEncoding = encoding; }
  }
#endif
}
