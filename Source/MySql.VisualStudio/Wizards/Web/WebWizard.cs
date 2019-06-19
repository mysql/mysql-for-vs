// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Utility.Classes;
using System.Diagnostics;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public class WebWizard : BaseWizard<WebWizardForm, BaseCodeGeneratorStrategy>
  {

    private string _fullconnectionstring = string.Empty;

    public WebWizard(LanguageGenerator language)
      : base(language)
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
        try
        {
          vsProj.References.Add("MySql.Data");
        }
        catch
        {
          if (MessageBox.Show("The MySQL .NET driver could not be found." + Environment.NewLine
                        + @"To use it you must download and install the MySQL Connector/Net package from http://dev.mysql.com/downloads/connector/net/" +
                         Environment.NewLine + "Click OK to go to the page or Cancel to continue", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
          {
            ProcessStartInfo browserInfo = new ProcessStartInfo("http://dev.mysql.com/downloads/connector/net/");
            System.Diagnostics.Process.Start(browserInfo);
          }
        }

        double version = double.Parse(WizardForm.Wizard.GetVisualStudioVersion());
        if (version >= 12.0)
        {
          References refs = vsProj.References;
          foreach (Reference item in refs)
          {
            switch (item.Name)
            {
              case "System.Web.Razor":
                if (item.Version.Equals("1.0.0.0"))
                  vsProj.References.Find("System.Web.Razor").Remove();
                break;
              case "System.Web.WebPages":
                vsProj.References.Find("System.Web.WebPages").Remove();
                break;
              case "System.Web.Mvc":
                vsProj.References.Find("System.Web.Mvc").Remove();
                break;
              case "System.Web.Helpers":
                vsProj.References.Find("System.Web.Helpers").Remove();
                break;
            }
          }

          vsProj.References.Add("System.Web.WebPages, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL");
          vsProj.References.Add("System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL");
          vsProj.References.Add("System.Web.Helpers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL");
          vsProj.References.Add("System.Web.Razor");

#if NET_40_OR_GREATER
          vsProj.Project.Save();
#endif

        }
        AddNugetPackage(vsProj, JQUERY_PKG_NAME, JQUERY_VERSION, false);
        var packagesPath = Path.Combine(Path.GetDirectoryName(ProjectPath), @"Packages\jQuery." + JQUERY_VERSION + @"\Content\Scripts");
        CopyPackageToProject(vsProj, ProjectPath, packagesPath, "Scripts");

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

            AddNugetPackage(vsProj, ENTITY_FRAMEWORK_PCK_NAME, CurrentEntityFrameworkVersion, true);
            string modelPath = Path.Combine(ProjectPath, "Models");
            GenerateEntityFrameworkModel(project, vsProj, new MySqlConnection(WizardForm.connectionStringForModel), WizardForm.modelName, tables, modelPath);
            GenerateMVCItems(vsProj);

            if (WizardForm.dEVersion == DataEntityVersion.EntityFramework6)
            {
              project.DTE.SuppressUI = true;
              project.Properties.Item("TargetFrameworkMoniker").Value = ".NETFramework,Version=v4.5";
            }
          }
        }

        else
        {
          string indexPath = Language == LanguageGenerator.CSharp ? (string)(FindProjectItem(FindProjectItem(FindProjectItem(vsProj.Project.ProjectItems, "Views").ProjectItems,
     "Home").ProjectItems, "Index.cshtml").Properties.Item("FullPath").Value) :
      (string)(FindProjectItem(FindProjectItem(FindProjectItem(vsProj.Project.ProjectItems, "Views").ProjectItems,
     "Home").ProjectItems, "Index.vbhtml").Properties.Item("FullPath").Value);

          string contents = File.ReadAllText(indexPath);
          contents = contents.Replace("$catalogList$", String.Empty);
          File.WriteAllText(indexPath, contents);
        }

        var webConfig = new MySql.Data.VisualStudio.WebConfig.WebConfig(ProjectPath + @"\web.config");
        SendToGeneralOutputWindow("Starting provider configuration...");
        try
        {
          try
          {
            string configPath = ProjectPath + @"\web.config";

            if (WizardForm.createAdministratorUser)
            {
              SendToGeneralOutputWindow("Creating administrator user...");
              using (AppConfig.Load(configPath))
              {
                var configFile = new FileInfo(configPath);
                var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
                var wcfm = new WebConfigurationFileMap();
                wcfm.VirtualDirectories.Add("/", vdm);
                System.Configuration.Configuration config = WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/");
                try
                {
                  if (!WizardForm.includeSensitiveInformation)
                  {
                    ConnectionStringsSection connectionStringsection = config.GetSection("connectionStrings") as ConnectionStringsSection;
                    if (connectionStringsection != null)
                    {
                      connectionStringsection.ConnectionStrings[WizardForm.connectionStringNameForAspNetTables].ConnectionString = _fullconnectionstring;
                      config.Save();
                    }
                  }
                }
                catch
                { }

                MembershipSection section = (MembershipSection)config.GetSection("system.web/membership");
                ProviderSettingsCollection settings = section.Providers;
                NameValueCollection membershipParams = settings[section.DefaultProvider].Parameters;
                var provider = new MySQLMembershipProvider();

                provider.Initialize(section.DefaultProvider, membershipParams);

                //create the user
                MembershipCreateStatus status;
                if (!WizardForm.requireQuestionAndAnswer)
                {
                  provider.CreateUser(WizardForm.adminName, WizardForm.adminPassword, "temporary@email.com", null, null, true, null, out status);
                }
                else
                {
                  provider.CreateUser(WizardForm.adminName, WizardForm.adminPassword, "temporary@email.com", WizardForm.userQuestion, WizardForm.userAnswer, true, null, out status);
                }
              }
            }

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
          }
          catch (Exception ex)
          {
            Logger.LogError($"An error occured when creating user. {ex.Message}", true);
          }

        }
        catch (Exception ex)
        {
          Logger.LogError(ex.Message, true);
        }
      }
      SendToGeneralOutputWindow("Finished project generation.");
      WizardForm.Dispose();
    }


    public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
      Dte = automationObject as DTE;

      connections = MySqlServerExplorerConnections.LoadMySqlConnectionsFromServerExplorer(Dte);
      WizardForm.connections = this.connections;
      WizardForm.dte = this.Dte;

      DialogResult result = WizardForm.ShowDialog();

      if (result == DialogResult.Cancel) throw new WizardCancelledException();

      var connectionstringForModel = string.Empty;

      var path = Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");
      Version mysqlDataVersion = null;


      if (!string.IsNullOrEmpty(path))
      {
        mysqlDataVersion = new Version(Utilities.GetProductVersion(path + @"\Assemblies\v4.0\MySql.Data.dll"));
      }

      _fullconnectionstring = WizardForm.connectionStringForAspNetTables;

      if (!WizardForm.includeSensitiveInformation)
      {
        // connectionstringformodel
        var csb = new MySqlConnectionStringBuilder(WizardForm.connectionStringForModel);
        csb.Password = null;
        connectionstringForModel = string.Format(@"<add name=""{0}Entities"" connectionString=""metadata=res://*/Models.{0}.csdl|res://*/Models.{0}.ssdl|res://*/Models.{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />", WizardForm.connectionStringNameForModel, csb.ConnectionString);
        // connectionstringforaspnet                
        csb = new MySqlConnectionStringBuilder(WizardForm.connectionStringForAspNetTables);
        csb.Password = null;
        WizardForm.connectionStringForAspNetTables = csb.ConnectionString;
      }
      else
      {
        connectionstringForModel = string.Format(@"<add name=""{0}Entities"" connectionString=""metadata=res://*/Models.{0}.csdl|res://*/Models.{0}.ssdl|res://*/Models.{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />", WizardForm.connectionStringNameForModel, WizardForm.connectionStringForModel);
      }

      replacementsDictionary.Add("$connectionstringforaspnettables$", WizardForm.connectionStringForAspNetTables);
      replacementsDictionary.Add("$connectionstringnameformodel$", WizardForm.dEVersion != DataEntityVersion.None ? connectionstringForModel : string.Empty);
      replacementsDictionary.Add("$connectionstringnameforaspnettables$", WizardForm.connectionStringNameForAspNetTables);
      replacementsDictionary.Add("$EntityFrameworkReference$", WizardForm.dEVersion != DataEntityVersion.None ? @"<add assembly=""System.Data.Entity, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089""/>" : string.Empty);
      replacementsDictionary.Add("$requirequestionandanswer$", WizardForm.requireQuestionAndAnswer ? "True" : "False");
      replacementsDictionary.Add("$minimumrequiredlength$", WizardForm.minimumPasswordLenght.ToString());
      replacementsDictionary.Add("$writeExceptionstoeventlog$", WizardForm.writeExceptionsToLog ? "True" : "False");
      replacementsDictionary.Add("$providerReference$", WizardForm.dEVersion == DataEntityVersion.EntityFramework6 ? @"<entityFramework> <providers> <provider invariantName=""MySql.Data.MySqlClient"" type=""MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF6"" /></providers> </entityFramework>" : string.Empty);
      replacementsDictionary.Add("$mySqlProviderVersion$", mysqlDataVersion != null ? string.Format("{0}.{1}.{2}.{3}", mysqlDataVersion.Major, mysqlDataVersion.Minor, mysqlDataVersion.Build, "0") : "6.8.3.0");
      replacementsDictionary.Add("$jqueryversion$", JQUERY_VERSION);

      switch (WizardForm.dEVersion)
      {
        case DataEntityVersion.None:
          replacementsDictionary.Add("$EntityFrameworkVersion$", string.Empty);
          break;
        case DataEntityVersion.EntityFramework5:
          replacementsDictionary.Add("$EntityFrameworkVersion$", @"<section name=""entityFramework"" type=""System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=4.4.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false""/>");
          break;
        case DataEntityVersion.EntityFramework6:
          replacementsDictionary.Add("$EntityFrameworkVersion$", @"<section name=""entityFramework"" type=""System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false""/>");
          break;
        default:
          break;
      }

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
        requiredquestionandanswer = WizardForm.Wizard.Language == LanguageGenerator.CSharp ? "[Required]" : "<Required()> _";
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

      double visualStudioVersion = double.Parse(WizardForm.Wizard.GetVisualStudioVersion());

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

      StringBuilder catalogs = new StringBuilder();
      catalogs = new StringBuilder("<h3> Catalog list</h3>");
      catalogs.AppendLine();

      foreach (var table in TablesIncludedInModel)
      {
        catalogs.AppendLine(string.Format(@"<div> @Html.ActionLink(""{0}"",""Index"", ""{0}"")</div>", table.Key[0].ToString().ToUpperInvariant() + table.Key.Substring(1)));
      }


      string indexPath = Language == LanguageGenerator.CSharp ? (string)(FindProjectItem(FindProjectItem(FindProjectItem(vsProj.Project.ProjectItems, "Views").ProjectItems,
        "Home").ProjectItems, "Index.cshtml").Properties.Item("FullPath").Value) :
         (string)(FindProjectItem(FindProjectItem(FindProjectItem(vsProj.Project.ProjectItems, "Views").ProjectItems,
        "Home").ProjectItems, "Index.vbhtml").Properties.Item("FullPath").Value);

      string contents = File.ReadAllText(indexPath);
      contents = contents.Replace("$catalogList$", catalogs.ToString());
      File.WriteAllText(indexPath, contents);

      try
      {
        foreach (var table in TablesIncludedInModel)
        {
          // creating controller file
          sessionHost.Session = sessionHost.CreateSession();
          sessionHost.Session["namespaceParameter"] = string.Format("{0}.Controllers", ProjectNamespace);
          sessionHost.Session["applicationNamespaceParameter"] = string.Format("{0}.Models", ProjectNamespace);
          sessionHost.Session["controllerClassParameter"] = string.Format("{0}Controller", table.Key[0].ToString().ToUpperInvariant() + table.Key.Substring(1));
          if ((WizardForm.dEVersion == DataEntityVersion.EntityFramework6 && Language == LanguageGenerator.VBNET) ||
            Language == LanguageGenerator.CSharp)
          {
            sessionHost.Session["modelNameParameter"] = string.Format("{0}Entities", WizardForm.connectionStringNameForModel);
          }
          else if (WizardForm.dEVersion == DataEntityVersion.EntityFramework5 && Language == LanguageGenerator.VBNET)
          {
            sessionHost.Session["modelNameParameter"] = string.Format("{1}.{0}Entities", WizardForm.connectionStringNameForModel, ProjectNamespace);
          }
          sessionHost.Session["classNameParameter"] = table.Key;
          sessionHost.Session["entityNameParameter"] = table.Key[0].ToString().ToUpperInvariant() + table.Key.Substring(1);
          sessionHost.Session["entityClassNameParameter"] = table.Key;
          if ((visualStudioVersion < 12.0 && Language == LanguageGenerator.VBNET) ||
              Language == LanguageGenerator.CSharp)
          {
            sessionHost.Session["entityClassNameParameterWithNamespace"] =
              string.Format("{0}.{1}", ProjectNamespace, table.Key);
          }
          else if (Language == LanguageGenerator.VBNET && visualStudioVersion >= 12.0)
          {
            if (WizardForm.dEVersion == DataEntityVersion.EntityFramework5)
            {
              sessionHost.Session["entityClassNameParameterWithNamespace"] = string.Format("{0}.{0}.{1}", ProjectNamespace, table.Key);
            }
            else if (WizardForm.dEVersion == DataEntityVersion.EntityFramework6)
            {
              sessionHost.Session["entityClassNameParameterWithNamespace"] = string.Format("{0}.{1}", ProjectNamespace, table.Key);
            }
          }
          T4Callback cb = new T4Callback();
          StringBuilder resultControllerFile = new StringBuilder(t4.ProcessTemplate(controllerClassPath, File.ReadAllText(controllerClassPath), cb));
          string controllerFilePath = ProjectPath + string.Format(@"\Controllers\{0}Controller.{1}", table.Key[0].ToString().ToUpperInvariant() + table.Key.Substring(1), fileExtension);
          File.WriteAllText(controllerFilePath, resultControllerFile.ToString());
          if (cb.errorMessages.Count > 0)
          {
            File.AppendAllLines(controllerFilePath, cb.errorMessages);
          }

          vsProj.Project.ProjectItems.AddFromFile(controllerFilePath);

          var viewPath = Path.GetFullPath(ProjectPath + string.Format(@"\Views\{0}", table.Key[0].ToString().ToUpperInvariant() + table.Key.Substring(1)));
          Directory.CreateDirectory(viewPath);
          string resultViewFile = t4.ProcessTemplate(IndexFilePath, File.ReadAllText(IndexFilePath), cb);
          File.WriteAllText(string.Format(viewPath + @"\Index.{0}html", fileExtension), resultViewFile);
          if (cb.errorMessages.Count > 0)
          {
            File.AppendAllLines(controllerFilePath, cb.errorMessages);
          }
          vsProj.Project.ProjectItems.AddFromFile(string.Format(viewPath + @"\Index.{0}html", fileExtension));
        }
      }
      catch
      {
        Logger.LogError("An error occured when generating MVC items. The application is not completed.", true);
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
