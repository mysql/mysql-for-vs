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
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using VSLangProj;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySQL.Utility.Classes;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;


namespace MySql.Data.VisualStudio.Wizards
{
  // TODO: Make this class capable of generating both C# & VB.NET projects.

  /// <summary>
  ///  Base class for all the Project Wizards.
  /// </summary>
  public class BaseWizard<TWizardForm,TCodeGeneratorStrategy> : IWizard 
    where TWizardForm : BaseWizardForm 
    where TCodeGeneratorStrategy : ICodeGeneratorStrategy
  {
    /// <summary>
    /// The DTE instance.
    /// </summary>
    protected DTE Dte;

    /// <summary>
    /// The language used for the code generator.
    /// </summary>
    protected LanguageGenerator Language;

    /// <summary>
    /// The CodeDom Provider for the generated language (currently either C# or VB.NET).
    /// </summary>
    protected CodeDomProvider CodeProvider;

    /// <summary>
    /// The code generation strategy.
    /// </summary>
    protected TCodeGeneratorStrategy Strategy;

    /// <summary>
    /// The wizard form used with this Wizard.
    /// </summary>
    protected TWizardForm WizardForm;

    /// <summary>
    /// The project path chosen by user where the wizard will generate the project artifacts.
    /// </summary>
    protected string ProjectPath;

    /// <summary>
    /// The namespace for the code artifacts in the generated project.
    /// </summary>
    protected string ProjectNamespace;

    /// <summary>
    /// The .NET Framework version that the generated project will target (chosen by the user).
    /// </summary>
    protected string NetFxVersion;

    protected BindingSource connections
    {
      get;
      set;
    }

    protected IVsOutputWindowPane _generalPane;


    /// <summary>
    /// The column metadata.
    /// </summary>
    internal Dictionary<string, Column> Columns;

    /// <summary>
    /// The column metadata for the detail table.
    /// </summary>
    internal Dictionary<string, Column> DetailColumns;

    // Some constants of Entity Framework versions as supposed to be feed to this class's methods for Nuget.
    internal protected readonly static string ENTITY_FRAMEWORK_VERSION_5 = "5.0.0";
    internal protected readonly static string ENTITY_FRAMEWORK_VERSION_6 = "6.0.0";
    internal protected const string ENTITY_FRAMEWORK_PCK_NAME = "EntityFramework";

    protected string CurrentEntityFrameworkVersion;

    internal BaseWizard(LanguageGenerator Language)
    {
      this.Language = Language;
      if (Language == LanguageGenerator.CSharp)
      {
        CodeProvider = CodeDomProvider.CreateProvider("CSharp");
      }
      else if (Language == LanguageGenerator.VBNET)
      {
        CodeProvider = CodeDomProvider.CreateProvider("VisualBasic");
      }

      // get the general output window      
      IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
      Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;
      if (outWindow != null)
      {
        outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0);
        outWindow.GetPane(ref generalPaneGuid, out _generalPane);
        _generalPane.Activate();
      }

    }

    protected void SendToGeneralOutputWindow(string message)
    {
      if (_generalPane != null)
      {      
        _generalPane.OutputString(Environment.NewLine + message);       
      }
    }

    public virtual void BeforeOpeningFile(ProjectItem projectItem)
    {
      return;
    }

    /// <summary>
    /// This method needs to be overriden in derived classes to implement the project customization.
    /// </summary>
    /// <param name="projectItem"></param>
    public virtual void ProjectFinishedGenerating(Project project)
    {
      return;
    }
    
    public virtual void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
      return;
    }

    public virtual void RunFinished()
    {
      return;
    }

    /// <summary>
    /// Shows the Form wizard for collecting data, after user finishes with it caches some properties 
    /// from <paramref name="replacementsDictionary"/>.
    /// </summary>
    /// <param name="automationObject"></param>
    /// <param name="replacementsDictionary"></param>
    /// <param name="runKind"></param>
    /// <param name="customParams"></param>
    public virtual void RunStarted(object automationObject,
      Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
    {
#if NET_40_OR_GREATER
      Dte = ((dynamic)automationObject).DTE;
#endif
      DialogResult result = WizardForm.ShowDialog();
      if (result == DialogResult.Cancel) throw new WizardCancelledException();
      ProjectPath = replacementsDictionary["$destinationdirectory$"];
      //NetFxVersion = replacementsDictionary["$clrversion$"];
      NetFxVersion = replacementsDictionary["$targetframeworkversion$"];
      ProjectNamespace = GetCanonicalIdentifier(replacementsDictionary["$safeprojectname$"]);
    }

    public virtual bool ShouldAddProjectItem(string filePath)
    {
      return true;
    }

    protected void GenerateEntityFrameworkModel(VSProject vsProj, MySqlConnection con, string modelName, List<string> tables)
    {
      string ns = GetCanonicalIdentifier(ProjectNamespace);
      EntityFrameworkGenerator gen = new EntityFrameworkGenerator(
        con, modelName, tables, ProjectPath, ns, CurrentEntityFrameworkVersion, Language);
      gen.Generate();
      TryErrorsEntityFrameworkGenerator(gen);
      SetupConfigFileEntityFramework(vsProj, con.ConnectionString);
      AddDataEntityArtifactsToProject(gen, modelName, vsProj, con);
    }

    protected void GenerateEntityFrameworkModel(VSProject vsProj, MySqlConnection con, string modelName, string tableName )
    {
      string ns = GetCanonicalIdentifier(ProjectNamespace);
      EntityFrameworkGenerator gen = new EntityFrameworkGenerator(
        con, modelName, tableName, ProjectPath, ns, CurrentEntityFrameworkVersion, Language);
      gen.Generate();
      TryErrorsEntityFrameworkGenerator(gen);
      SetupConfigFileEntityFramework(vsProj, con.ConnectionString);
      AddDataEntityArtifactsToProject(gen, modelName, vsProj, con);
    }

    private void TryErrorsEntityFrameworkGenerator(EntityFrameworkGenerator gen)
    {
      List<string> errors = gen.Errors.ToList();
      if (errors.Count != 0)
      {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < errors.Count; i++)
        {
          sb.Append(" - ").AppendLine(errors[i]);
        }
        throw new WizardException(string.Format("The Entity Framework generation failed with the following errors:\n\n",
          sb.ToString()));
      }
    }

    private void AddDataEntityArtifactsToProject(EntityFrameworkGenerator gen, string modelName, VSProject vsProj, MySqlConnection con)
    {
      try
      {
        // Add the Edmx artifacts to the project.
        string _modelName = string.Format("{0}.edmx", modelName );
        string artifactPath = Path.Combine(ProjectPath, _modelName);
        string artifactPathExcluded = artifactPath + ".exclude";
        File.Move(artifactPath, artifactPathExcluded);
        ProjectItem pi = vsProj.Project.ProjectItems.AddFromFile(artifactPathExcluded);
        //Dictionary<string, object> props = GetAllProperties(pi.Properties);
        pi.DTE.SuppressUI = true;
        pi.Name = _modelName;
        pi.Properties.Item("ItemType").Value = "EntityDeploy";
        string dstFile = Path.Combine( ProjectPath, string.Format("{0}.Designer.{1}", modelName, 
          CodeProvider.FileExtension) );
        if (File.Exists(dstFile))
          File.Delete(dstFile);
        File.Move( Path.Combine( ProjectPath, string.Format("{0}.Designer.{1}.bak", modelName, 
          CodeProvider.FileExtension ) ), dstFile );
        vsProj.Project.ProjectItems.AddFromFile(dstFile);
        // Adding references
        AddReferencesEntityFramework(vsProj);
      }
      catch
      {        
        throw new WizardException("Failed operation when addin model to project");
      }      
    }

    protected void AddReferencesEntityFramework(VSProject vsProj)
    {
      if( CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_5 )
      { 
        vsProj.References.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.Entity.dll");
        vsProj.References.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Runtime.Serialization.dll");
      }
      else if (CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_6)
      {
        vsProj.References.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Runtime.Serialization.dll");
        vsProj.References.Add("MySql.Data.Entity.EF6");
        vsProj.References.Add("System.ComponentModel.DataAnnotations");
      }
    }

    protected void SetupConfigFileEntityFramework( VSProject vsProj, string connStr )
    {
      string contents = "";
      if (CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_5)
      {
        contents = 
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name=""entityFramework"" type=""System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false"" />
  </configSections>
  <startup>
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0"" />
  </startup>
  <connectionStrings>
    <add name=""Model1Entities"" connectionString=""metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{0}&quot;"" providerName=""System.Data.EntityClient"" />
  </connectionStrings>
</configuration>";
      }
      else if (CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_6)
      {
        contents =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name=""entityFramework"" type=""System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" requirePermission=""false"" />
  </configSections>
  <startup>
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5"" />
  </startup>
  <entityFramework>
    <defaultConnectionFactory type=""System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework"">
      <parameters>
        <parameter value=""v11.0"" />
      </parameters>
    </defaultConnectionFactory>
    <providers>
      <provider invariantName=""MySql.Data.MySqlClient"" type=""MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF6"" />
    </providers>
  </entityFramework>
  <connectionStrings>
    <add name=""Model1Entities"" connectionString=""metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{0}&quot;"" providerName=""System.Data.EntityClient"" />
  </connectionStrings>
</configuration>";
      }
      File.WriteAllText(Path.Combine(ProjectPath, "app.config"), string.Format(contents, connStr));
    }

    /// <summary>
    /// Downloads from Nuget the package version required and adds the assembly reference to the project.
    /// </summary>
    /// <param name="vsProj"></param>
    protected void AddNugetPackage(VSProject VsProj, string PackageName, string Version)
    {
      SendToGeneralOutputWindow(string.Format("Getting Nuget Package for {0}-{1}...", PackageName, Version));
      // Installs the Entity Framework given version thru Nuget using reflection, which is a bit messy, but 
      // we avoid shipping Nuget dll.
      try
      {
        Assembly nugetAssembly = Assembly.Load("nuget.core");
        Type packageRepositoryFactoryType = nugetAssembly.GetType("NuGet.PackageRepositoryFactory");
        PropertyInfo piDefault = packageRepositoryFactoryType.GetProperty("Default");
        MethodInfo miCreateRepository = packageRepositoryFactoryType.GetMethod("CreateRepository");
        object repo = miCreateRepository.Invoke(piDefault.GetValue(null, null), new object[] { "https://packages.nuget.org/api/v2" });
        Type packageManagerType = nugetAssembly.GetType("NuGet.PackageManager");
        ConstructorInfo ciPackageManger = packageManagerType.GetConstructor(new Type[] { System.Reflection.Assembly.Load("nuget.core").GetType("NuGet.IPackageRepository"), typeof(string) });
        DirectoryInfo di = new DirectoryInfo(ProjectPath);
        string solPath = di.Parent.FullName;
        string installPath = di.Parent.CreateSubdirectory("packages").FullName;
        object packageManager = ciPackageManger.Invoke(new object[] { repo, installPath });
        MethodInfo miInstallPackage = packageManagerType.GetMethod("InstallPackage",
          new Type[] { typeof(string), System.Reflection.Assembly.Load("nuget.core").GetType("NuGet.SemanticVersion") });
        string packageID = PackageName;
        MethodInfo miParse = nugetAssembly.GetType("NuGet.SemanticVersion").GetMethod("Parse");
        object semanticVersion = miParse.Invoke(null, new object[] { Version });
        miInstallPackage.Invoke(packageManager, new object[] { packageID, semanticVersion });
        // Adds reference to project.
        AddPackageReference(VsProj, solPath, PackageName, Version);
      }
      catch 
      {
        MessageBox.Show("EntityFramework installation package failure. Please check that you have the latest Nuget version.", "Error", MessageBoxButtons.OK);
        return;
      }      
    }

    /// <summary>
    /// Adds the reference for a nuget package (already installed from Nuget) to the project, taking into account 
    /// the target .NET version and nuget package version.
    /// </summary>
    /// <param name="VsProj"></param>
    /// <param name="BasePath"></param>
    /// <param name="Version"></param>
    protected void AddPackageReference( VSProject VsProj, string BasePath, string PackageName, string Version)
    {
      string efPath = Path.Combine("packages", string.Format("{0}.{1}\\lib", PackageName, Version));
      string packagePath = Path.Combine(BasePath, efPath);

      if (NetFxVersion.StartsWith("4.5"))
      {
        packagePath = Path.Combine(packagePath, "net45");
      }
      else if (NetFxVersion.StartsWith("4.0"))
      {
        packagePath = Path.Combine(packagePath, "net40");
      }
      else
      {
        throw new WizardException(string.Format("Only .NET versions 4.0/4.5/4.5.1 are supported (received version {0})", 
          NetFxVersion));
      }
      packagePath = Path.Combine(packagePath, PackageName + ".dll");
      VsProj.References.Add( packagePath );
    }

    protected void GenerateTypedDataSetModel(VSProject VsProj, MySqlConnection con, List<string> tables)
    {
      string canonicalNamespace = GetCanonicalIdentifier(ProjectNamespace);
      TypedDataSetGenerator gen = new TypedDataSetGenerator(con, "", tables, ProjectPath, canonicalNamespace, Language );
      string file = gen.Generate();
      string artifactPath = Path.Combine(ProjectPath, string.Format("{0}.{1}", 
        TypedDataSetGenerator.FILENAME_ARTIFACT, CodeProvider.FileExtension ));
      VsProj.Project.ProjectItems.AddFromFile(artifactPath);
    }

    protected void GenerateTypedDataSetModel(VSProject VsProj, MySqlConnection con, string TableName)
    {
      string canonicalNamespace = GetCanonicalIdentifier(ProjectNamespace);
      TypedDataSetGenerator gen = new TypedDataSetGenerator(con, "", TableName, ProjectPath, canonicalNamespace, Language);
      string file = gen.Generate();
      string artifactPath = Path.Combine(ProjectPath, string.Format("{0}.{1}", 
        TypedDataSetGenerator.FILENAME_ARTIFACT, CodeProvider.FileExtension ));
      VsProj.Project.ProjectItems.AddFromFile(artifactPath);
    }

    protected ProjectItem FindProjectItem(ProjectItems Items, string Name)
    {
      ProjectItem item = null;
      for (int i = 1; i <= Items.Count; i++)
      {
        ProjectItem item2 = Items.Item(i);
        if (item2.Name == Name)
        {
          item = item2;
          break;
        }
      }
      return item;
    }

    internal protected static string GetCanonicalIdentifier(string Identifier)
    {
      if (Identifier == null) return "";
      char[] chars = Identifier.ToCharArray();
      for (int i = 0; i < chars.Length; i++)
      {
        if (!char.IsLetterOrDigit(chars[i])) chars[i] = '_';
      }
      return new string(chars);    }

    protected string GetConnectionStringWithPassword(MySqlConnection con)
    {
      Type t = typeof(MySqlConnection);
      PropertyInfo p = t.GetProperty("Settings", BindingFlags.NonPublic | BindingFlags.Instance);
      object v = p.GetValue(con, null);
      return v.ToString();
    }

    protected Dictionary<string, object> GetAllProperties(EnvDTE.Properties props)
    {
      Dictionary<string, object> dic = new Dictionary<string, object>();
      for (int i = 1; i <= props.Count; i++)
      {
        try
        {
          dic.Add(props.Item(i).Name, props.Item(i).Value);
        } 
        catch( System.Runtime.InteropServices.COMException ex) { /* just ignore it */ }
      }

      return dic;
    }

    internal static Dictionary<string, Column> GetColumnsFromTable(string TableName, MySqlConnection con)
    {
      string sqlFilter = string.Format(
        @"select t.table_name from information_schema.tables t 
          where ( t.table_schema = '{0}' ) and ( t.table_name = '{1}' )",
        con.Database, TableName );
      // TODO: add validation to include datetime_precision when using 5.6
      string sqlData = string.Format(
        @"select c.table_schema, c.table_name, c.column_name, c.column_default, c.is_nullable, c.data_type, 
          c.character_maximum_length, c.numeric_precision, c.numeric_scale,  c.column_type 
          from information_schema.columns c where ( c.table_schema = '{0}' ) and ( c.table_name in {1} )",
          con.Database, "{0}");
      Dictionary<string, Column> dic = GetMetadata<Column>(con, sqlFilter, sqlData);
      return dic;
    }

    /// <summary>
    /// Gets a list of table columns for a given database.
    /// </summary>
    /// <param name="con"></param>
    /// <returns></returns>
    private static Dictionary<string, T> GetMetadata<T>(
      MySqlConnection con, string sqlFilter, string sqlData) where T : MetaObject, new()
    {
      Dictionary<string, T> dic = new Dictionary<string, T>();
      if ((con.State & ConnectionState.Open) == 0)
        con.Open();
      try
      {
        MySqlCommand cmd = new MySqlCommand("", con);
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(sqlFilter))
        {
          sb.Append("( ");
          cmd.CommandText = sqlFilter;
          using (MySqlDataReader r1 = cmd.ExecuteReader())
          {
            while (r1.Read())
            {
              sb.Append("'").Append(r1.GetString(0)).Append("',");
            }
          }
          sb.Length = sb.Length - 1;
          sb.Append(" ) ");
          cmd.CommandText = string.Format(sqlData, sb);
        }
        else
        {
          cmd.CommandText = sqlData;
        }
        // Get columns
        using (MySqlDataReader r = cmd.ExecuteReader())
        {
          while (r.Read())
          {
            T t = new T();
            t.Initialize(r);
            dic.Add(t.Name, t);
          }
        }
      }
      finally
      {
        //con.Close();
      }
      return dic;
    }

    internal protected string GetVisualStudioVersion()
    {
#if NET_40_OR_GREATER
      return Dte.Version;
#else
      return "9.0";
#endif
    }

  }
}
