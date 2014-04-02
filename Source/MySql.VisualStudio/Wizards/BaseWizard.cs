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


namespace MySql.Data.VisualStudio.Wizards
{
  // TODO: Make this class capable of generating both C# & VB.NET projects.

  /// <summary>
  ///  Base class for all the Project Wizards.
  /// </summary>
  public class BaseWizard<TWizardForm> : IWizard where TWizardForm : BaseWizardForm
  {
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

    /// <summary>
    /// The column metadata.
    /// </summary>
    internal Dictionary<string, Column> Columns;

    // Some constants of Entity Framework versions as supposed to be feed to this class's methods.
    protected const string ENTITY_FRAMEWORK_VERSION_5 = "5.0.0";
    protected const string ENTITY_FRAMEWORK_VERSION_6 = "6.0.0";

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

    protected void GenerateEntityFrameworkModel(VSProject VsProj, string Version, MySqlConnection con, string ModelName, string TableName )
    {
      // Run the generator
      string ns = GetCanonicalIdentifier(ProjectNamespace);
      EntityFrameworkGenerator gen = new EntityFrameworkGenerator(con, ModelName, TableName, ProjectPath, ns);
      gen.Generate();
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
      else
      {
        // Add the Edmx artifacts to the project.
        string artifactPath = Path.Combine(ProjectPath, string.Format("{0}.edmx", ModelName));
        VsProj.Project.ProjectItems.AddFromFile( artifactPath );
        string dstFile = Path.Combine( ProjectPath, string.Format("{0}.Designer.cs", ModelName) );
        if (File.Exists(dstFile))
          File.Delete(dstFile);
        File.Move( Path.Combine( ProjectPath, string.Format("{0}.Designer.cs.bak", ModelName) ), dstFile );
        //artifactPath = Path.Combine(ProjectPath, string.Format("{0}.Designer.cs", ModelName));
        //VsProj.Project.ProjectItems.AddFromFile(artifactPath);
      }
    }

    /// <summary>
    /// Downloads from Nuget the EntityFramework version required and adds the assembly reference to the project.
    /// </summary>
    /// <param name="vsProj"></param>
    protected void AddEntityFrameworkNugetPackage(VSProject VsProj, string Version)
    {
      // Installs the Entity Framework given version thru Nuget using reflection, which is a bit messy, but 
      // we avoid shipping Nuget dll.
      Assembly nugetAssembly = Assembly.Load("nuget.core"); // TODO: request an specific nuget version here (v 1.6 has bugs).
      Type packageRepositoryFactoryType = nugetAssembly.GetType("NuGet.PackageRepositoryFactory");
      PropertyInfo piDefault = packageRepositoryFactoryType.GetProperty("Default");
      MethodInfo miCreateRepository = packageRepositoryFactoryType.GetMethod("CreateRepository");
      object repo = miCreateRepository.Invoke(piDefault.GetValue(null, null), new object[] { "https://packages.nuget.org/api/v2" });
      Type packageManagerType = nugetAssembly.GetType("NuGet.PackageManager");
      ConstructorInfo ciPackageManger = packageManagerType.GetConstructor( new Type[] { System.Reflection.Assembly.Load( "nuget.core" ).GetType( "NuGet.IPackageRepository" ), typeof( string ) } );
      DirectoryInfo di = new DirectoryInfo(ProjectPath);
      string solPath = di.Parent.FullName;
      string installPath = di.Parent.CreateSubdirectory("packages").FullName;
      object packageManager = ciPackageManger.Invoke( new object[] { repo, installPath } );
      MethodInfo miInstallPackage = packageManagerType.GetMethod("InstallPackage",
        new Type[] { typeof(string), System.Reflection.Assembly.Load("nuget.core").GetType("NuGet.SemanticVersion") });
      string packageID = "EntityFramework";
      MethodInfo miParse = nugetAssembly.GetType("NuGet.SemanticVersion").GetMethod("Parse");
      object semanticVersion = miParse.Invoke(null, new object[] { Version });
      miInstallPackage.Invoke(packageManager, new object[] { packageID, semanticVersion });
      // Adds reference to project.
      AddEntityFrameworkReference(VsProj, solPath, Version);
    }

    /// <summary>
    /// Adds the reference for Entity Framework (already installed from Nuget) to the project, taking into account 
    /// the target .NET version and Entity Framework version.
    /// </summary>
    /// <param name="VsProj"></param>
    /// <param name="BasePath"></param>
    /// <param name="Version"></param>
    protected void AddEntityFrameworkReference( VSProject VsProj, string BasePath, string Version)
    {
      
      string efPath = "packages" + string.Format("EntityFramework.{0}\\lib", Version);
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
      packagePath = Path.Combine(packagePath, "EntityFramework.dll");
      VsProj.References.Add( packagePath );
    }

    protected void GenerateTypedDataSetModel(VSProject VsProj, MySqlConnection con, string TableName)
    {
      string canonicalNamespace = GetCanonicalIdentifier(ProjectNamespace);
      TypedDataSetGenerator gen = new TypedDataSetGenerator(con, "", TableName, ProjectPath, canonicalNamespace);
      string file = gen.Generate();
      string artifactPath = Path.Combine(ProjectPath, string.Format("{0}.cs", TypedDataSetGenerator.FILENAME_ARTIFACT));
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

    /// <summary>
    /// Transforms a user identifier like MySql table to ensure it is a valid identifier in C#/VB.NET.
    /// </summary>
    /// <returns></returns>
    protected string GetCanonicalIdentifier( string Identifier )
    {
      return Identifier.Replace(' ', '_').Replace('`', '_');
    }

    ///// <summary>
    ///// Gets the list of columns.
    ///// </summary>
    ///// <param name="TableName"></param>
    ///// <param name="con"></param>
    ///// <returns></returns>
    //protected List<string> GetColumnsFromTable(string TableName, MySqlConnection con)
    //{
    //  List<string> columns = new List<string>();
    //  DataTable t = con.GetSchema("COLUMNS", new string[] { "", con.Database, TableName });
    //  int idxCol = t.Columns["column_name"].Ordinal;
    //  foreach (DataRow row in t.Rows)
    //  {
    //    columns.Add((string)row[idxCol]);
    //  }
    //  return columns;
    //}

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
  }
}
