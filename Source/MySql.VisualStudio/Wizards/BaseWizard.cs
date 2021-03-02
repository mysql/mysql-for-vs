// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
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
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using VSLangProj;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Utility.Classes;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  ///  Base class for all the Project Wizards.
  /// </summary>
  public class BaseWizard<TWizardForm,TCodeGeneratorStrategy> : IWizard 
    where TWizardForm : BaseWizardForm 
    //where TCodeGeneratorStrategy : ICodeGeneratorStrategy
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

    ///// <summary>
    ///// The code generation strategy.
    ///// </summary>
    //protected TCodeGeneratorStrategy Strategy;

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

    internal Dictionary<string, Dictionary<string, ColumnValidation>> ColumnMappings = new Dictionary<string, Dictionary<string, ColumnValidation>>();

    internal Dictionary<string, string> TablesIncludedInModel = new Dictionary<string, string>();

    public enum ProjectWizardType : int
    {
      AspNetMVC = 1,
      WindowsForms = 2
    };

    protected ProjectWizardType projectType
    {
      get;
      set;
    }

    // Some constants of Entity Framework versions as supposed to be feed to this class's methods for Nuget.
    internal protected readonly static string ENTITY_FRAMEWORK_VERSION_5 = "5.0.0";
    internal protected readonly static string ENTITY_FRAMEWORK_VERSION_6 = "6.0.0";
    internal protected const string ENTITY_FRAMEWORK_PCK_NAME = "EntityFramework";
    internal protected readonly static string JQUERY_VERSION = "2.1.1";
    internal protected const string JQUERY_PKG_NAME = "Jquery";

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
      try
      {
#if NET_461_OR_GREATER
        Dte = ((dynamic)automationObject).DTE;
#endif
        DialogResult result = WizardForm.ShowDialog();
        if (result == DialogResult.Cancel) throw new WizardCancelledException();
        ProjectPath = replacementsDictionary["$destinationdirectory$"];
        //NetFxVersion = replacementsDictionary["$clrversion$"];
        NetFxVersion = replacementsDictionary["$targetframeworkversion$"];
        ProjectNamespace = GetCanonicalIdentifier(replacementsDictionary["$safeprojectname$"]);
      }
      catch (WizardCancelledException wce)
      {
        throw wce;
      }
      catch (Exception e)
      {
        SendToGeneralOutputWindow(string.Format("An error occurred: {0}\n\n {1}", e.Message, e.StackTrace));
      }
    }

    public virtual bool ShouldAddProjectItem(string filePath)
    {
      return true;
    }

    protected void GenerateEntityFrameworkModel(
      Project project, VSProject vsProj, MySqlConnection con, string modelName, List<string> tables, string modelPath )
    {
      string ns = GetCanonicalIdentifier(ProjectNamespace);
      EntityFrameworkGenerator gen = new EntityFrameworkGenerator(
        con, modelName, tables, modelPath, ns, CurrentEntityFrameworkVersion, Language, vsProj, ColumnMappings);
      vsProj = project.Object as VSProject;

      AddDataEntityArtifactsToProject(gen, modelName, vsProj, con);
      if( projectType == ProjectWizardType.WindowsForms )
        SetupConfigFileEntityFramework(vsProj, GetConnectionString(), modelName);
      project.DTE.Solution.SolutionBuild.Build(true);
      gen.Generate();
        
      if (gen.TablesInModel.Count() > 0)
          TablesIncludedInModel = gen.TablesInModel.ToDictionary<string, string>(p => p);
      
      TryErrorsEntityFrameworkGenerator(gen);
    }

    protected virtual string GetConnectionString()
    {
      return "";
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

    protected void CopyPackageToProject(VSProject vsProj, string projPath, string packagesPath, string FolderName)
    {
      try
      {
        var destination = Path.Combine(projPath, FolderName);

        if (Directory.Exists(destination))
          Directory.Delete(destination);

        Directory.Move(packagesPath, destination);
        vsProj.Project.ProjectItems.AddFromDirectory(destination);
      }
      catch
      {
        Logger.LogError("An error occured when adding the jquery library to the project. Check your nuget version or your internet connection.", true);
      }
      
    }


    private void AddDataEntityArtifactsToProject(EntityFrameworkGenerator gen, string modelName, VSProject vsProj, MySqlConnection con)
    {
      try
      {
        string modelPath = ProjectPath;

        if (projectType == ProjectWizardType.AspNetMVC)
            modelPath = Path.Combine(ProjectPath, "Models");          
        
        // Adding references
        AddReferencesEntityFramework(vsProj);
      }
      catch( Exception e )
      {        
        throw new WizardException("Failed operation when adding model to project", e);
      }
    }

    protected void AddReferencesEntityFramework(VSProject vsProj)
    {
#if NET_461_OR_GREATER
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
#else
      string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
#endif

      if ( CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_5 )
      { 
        AddProjectReference( vsProj, Path.Combine( path, @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.Entity.dll" ));
        AddProjectReference( vsProj, Path.Combine( path, @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Runtime.Serialization.dll"));
      }
      else if (CurrentEntityFrameworkVersion == ENTITY_FRAMEWORK_VERSION_6)
      {
        AddProjectReference( vsProj, Path.Combine( path, @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Runtime.Serialization.dll"));
        AddProjectReference( vsProj, "MySql.Data.Entity.EF6");
        AddProjectReference( vsProj, "System.ComponentModel.DataAnnotations");
      }
    }

    protected void AddProjectReference(VSProject vsProj, string assembly)
    {
      try
      {
        vsProj.References.Add(assembly);
      }
      catch (COMException e)
      {
        // Gobble the exception if it is of the kind "A reference to the component '...' already exists in the project.
        // (This may happen in VB.NET projects).
        if (e.Message.IndexOf("A reference to the component") == -1 || 
            e.Message.IndexOf("already exists in the project") == -1) throw;
      }
    }

    protected void SetupConfigFileEntityFramework( VSProject vsProj, string connStr, string modelName)
    {
      string contents = "";
      
      if (String.IsNullOrEmpty(modelName))
      {
        modelName = "Model1";
      }

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
    <add name=""{0}Entities"" connectionString=""metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />
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
    <add name=""{0}Entities"" connectionString=""metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=MySql.Data.MySqlClient;provider connection string=&quot;{1}&quot;"" providerName=""System.Data.EntityClient"" />
  </connectionStrings>
</configuration>";
      }
      File.WriteAllText(Path.Combine(ProjectPath, "app.config"), string.Format(contents,modelName, connStr));
    }

    /// <summary>
    /// Downloads from Nuget the package version required and adds the assembly reference to the project.
    /// </summary>
    /// <param name="vsProj"></param>
    protected void AddNugetPackage(VSProject VsProj, string PackageName, string Version, bool addReference)
    {
      SendToGeneralOutputWindow(string.Format("Getting Nuget Package for {0}-{1}...", PackageName, Version));     
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
        
        if (addReference)
          AddPackageReference(VsProj, solPath, PackageName, Version);
      }
      catch(Exception ex)
      { 
        SendToGeneralOutputWindow(string.Format("{0} installation package failure." + Environment.NewLine + "Please check that you have the latest Nuget version and that you have an internet connection." + Environment.NewLine + "{1}", PackageName, ex.Message));
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
    protected void AddPackageReference(VSProject VsProj, string BasePath, string PackageName, string Version)
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
      VsProj.References.Add(packagePath);
    }

    protected void GenerateTypedDataSetModel(VSProject VsProj, MySqlConnection con, List<string> tables)
    {
      string canonicalNamespace = GetCanonicalIdentifier(ProjectNamespace);
      TypedDataSetGenerator gen = new TypedDataSetGenerator(con, "", tables, ProjectPath, canonicalNamespace, Language, VsProj );
      gen.Generate();
      if (gen.TablesInModel.Count() > 0)
        TablesIncludedInModel = gen.TablesInModel.ToDictionary<string, string>(p => p);
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
      return new string(chars);
    }

    internal string GetConnectionStringWithPassword(MySqlConnection con)
    {
      MySqlConnectionStringBuilder msb = GetConnectionSettings(con);
      return msb.ToString();
    }

    protected MySqlConnectionStringBuilder GetConnectionSettings(MySqlConnection con)
    {
      Type t = typeof(MySqlConnection);
      PropertyInfo p = t.GetProperty("Settings", BindingFlags.NonPublic | BindingFlags.Instance);
      return ( MySqlConnectionStringBuilder )p.GetValue(con, null);
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
        catch( System.Runtime.InteropServices.COMException ex)
        {
          System.Diagnostics.Debug.WriteLine(ex.Message);
        }
      }

      return dic;
    }

    protected List<string> GetColumnsFromTableVanilla(string TableName, MySqlConnection con)
    {
      string sql = string.Format(@"select c.column_name from information_schema.columns c 
where ( c.table_schema = '{0}' ) and ( c.table_name = '{1}' );", con.Database, TableName );
      List<string> columns = new List<string>();
      MySqlCommand cmd = new MySqlCommand(sql, con);
      using (MySqlDataReader r = cmd.ExecuteReader())
      {
        while (r.Read())
        {
          columns.Add(r.GetString(0));
        }
      }
      return columns;
    }

    internal static Dictionary<string, Column> GetColumnsFromTable(string TableName, MySqlConnection con)
    {
      string sqlFilter = string.Format(
        @"select t.table_name from information_schema.tables t 
          where ( t.table_schema = '{0}' ) and ( t.table_name = '{1}' )",
        con.Database, TableName );
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
            t.Connection = con;
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
#if NET_461_OR_GREATER
      return Dte.Version;
#else
      return "9.0";
#endif
    }

    internal void RetrieveAllFkInfo(MySqlConnection con, string tableName, out Dictionary<string,ForeignKeyColumnInfo> MyFKs)
    {
      string sql = string.Format(
@"select `constraint_name`, `table_name`, `column_name`, `referenced_table_name`, `referenced_column_name`  
from information_schema.key_column_usage where table_schema = '{0}' and `constraint_name` in ( 
select `constraint_name` from information_schema.referential_constraints where `constraint_schema` = '{0}' and `table_name` = '{1}' )
",
con.Database, tableName );
      if ((con.State & ConnectionState.Open) == 0)
        con.Open();
      Dictionary<string,ForeignKeyColumnInfo> FKs = new Dictionary<string,ForeignKeyColumnInfo>();
      // Gather FK info per column pair
      MySqlCommand cmd = new MySqlCommand(sql, con);
      using (MySqlDataReader r = cmd.ExecuteReader())
      {
        while (r.Read())
        {
          ForeignKeyColumnInfo fk = new ForeignKeyColumnInfo()
          {
            ConstraintName = r.GetString(0),
            TableName = r.GetString(1),
            ColumnName = r.GetString(2),
            ReferencedTableName = r.GetString(3),
            ReferencedColumnName = r.GetString(4)
          };
          FKs.Add(fk.ColumnName, fk);
        }
      }
      // Gather referenceable columns
      foreach (ForeignKeyColumnInfo fk in FKs.Values)
      {
        fk.ReferenceableColumns = GetColumnsFromTableVanilla(fk.ReferencedTableName, con);
      }
      MyFKs = FKs;
    }

    internal static string CapitalizeString(string s)
    {
      return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s);
    }    

    internal void AddColumnMappings(string table, List<ColumnValidation> columns)
    {
      Dictionary<string, ColumnValidation> dic = columns.ToDictionary(p => p.Name);
      if (ColumnMappings.ContainsKey(table))
      {
        ColumnMappings[table] = dic;
      }
      else 
      {
        ColumnMappings.Add(table, dic);
      }
    }

    internal void PopulateColumnMappingsForTypedDataSet()
    {
      foreach (KeyValuePair<string, Dictionary<string, ColumnValidation>> kvp in ColumnMappings)
      {
        foreach (KeyValuePair<string, ColumnValidation> kvp2 in kvp.Value)
        {
          kvp2.Value.EfColumnMapping = GetCanonicalIdentifier(kvp2.Value.Name);
        }
      }
      // Solve Lookup column mappings
      foreach (KeyValuePair<string, Dictionary<string, ColumnValidation>> kvp in ColumnMappings)
      {
        foreach (KeyValuePair<string, ColumnValidation> kvp2 in kvp.Value)
        {
          if (!kvp2.Value.HasLookup) continue;
          kvp2.Value.EfLookupColumnMapping = GetCanonicalIdentifier(kvp2.Value.LookupColumn);
        }
      }
    }
  }

  internal class ForeignKeyColumnInfo
  {
    internal string ConstraintName { get; set; }
    internal string TableName { get; set; }
    internal string ReferencedTableName { get; set; }
    internal string ColumnName { get; set; }
    internal string ReferencedColumnName { get; set; }
    internal List<string> ReferenceableColumns { get; set; }
  }
}
