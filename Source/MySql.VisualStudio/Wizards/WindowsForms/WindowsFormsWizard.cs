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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using System.Windows.Forms;
using VSLangProj;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.MySqlClient;
using System.Reflection;
using MySql.Data.VisualStudio.Wizards;
using MySql.Data.VisualStudio.Properties;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  /// <summary>
  ///  Wizard for generation of a Windows Forms based project.
  /// </summary>
  public class WindowsFormsWizard : BaseWizard<WindowsFormsWizardForm, WindowsFormsCodeGeneratorStrategy>
  { 
    private bool ValidationsEnabled
    {
      get
      {
        return WizardForm.ValidationColumns != null;
      }
    }

    public WindowsFormsWizard( LanguageGenerator Language )
      : base( Language )
    {
      WizardForm = new WindowsFormsWizardForm(this);
      projectType = ProjectWizardType.WindowsForms;
    }

    public override void ProjectFinishedGenerating(Project project)
    {
      //Dictionary<string,object> dic = GetAllProperties(project.Properties);
      VSProject vsProj = project.Object as VSProject;
      try
      {
        Columns = WizardForm.Columns;
        DetailColumns = WizardForm.DetailColumns;
        _canonicalTableName = GetCanonicalIdentifier(WizardForm.TableName);
        string detailTableName = WizardForm.DetailTableName;
        
        // Create the strategy
        StrategyConfig config = new StrategyConfig(sw, _canonicalTableName, Columns, DetailColumns,
          WizardForm.DataAccessTechnology, WizardForm.GuiType, Language,
          ValidationsEnabled, WizardForm.ValidationColumns, WizardForm.ValidationColumnsDetail,
          GetConnectionStringWithPassword(WizardForm.Connection), WizardForm.TableName,
          detailTableName, WizardForm.ConstraintName, ForeignKeys );
        Strategy = WindowsFormsCodeGeneratorStrategy.GetInstance(config);

        vsProj.References.Add("MySql.Data");

        // Gather all the tables
#if NET_40_OR_GREATER
        SortedSet<string> tables = new SortedSet<string>();
        tables.Add(WizardForm.TableName);
        if (!string.IsNullOrEmpty(detailTableName))
          tables.Add(detailTableName);
        foreach (KeyValuePair<string, ForeignKeyColumnInfo> kvp in ForeignKeys)
        {
          tables.Add(kvp.Value.ReferencedTableName);
        }


        // Generate the model using the proper technology
        if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5 ||
          WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6 )
        {
          if( WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5 )
            CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_5;
          else
            CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_6;

          AddNugetPackage(vsProj, ENTITY_FRAMEWORK_PCK_NAME, CurrentEntityFrameworkVersion);
          GenerateEntityFrameworkModel(vsProj, WizardForm.Connection, "Model1", tables.ToList());
        }
        else if (WizardForm.DataAccessTechnology == DataAccessTechnology.TypedDataSet)
        {
          GenerateTypedDataSetModel(vsProj, WizardForm.Connection, tables.ToList());
        }
        AddBindings(vsProj);
#endif
      }
      catch (WizardException e)
      {
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      if( WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6 )
      {
        // Change target version to 4.5 (only version currently supported for EF6).
        project.DTE.SuppressUI = true;
        project.Properties.Item("TargetFrameworkMoniker").Value = ".NETFramework,Version=v4.5";
      }

      FixNamespaces();
      SendToGeneralOutputWindow("Building Solution...");
      project.DTE.Solution.SolutionBuild.Build(true);

      Settings.Default.WinFormsWizardConnection = WizardForm.ConnectionName;
      Settings.Default.Save();

      WizardForm.Dispose();
    }

    /// <summary>
    /// Fixes namespaces for issue in VB.NET with some VS versions like 2013.
    /// </summary>
    /// <param name="outputPath"></param>
    private void FixNamespaces()
    {
      if (Language != LanguageGenerator.VBNET) return;
      string outputPath = Path.Combine(Path.Combine(ProjectPath, "My Project"), Strategy.GetApplicationFileName());
      string contents = File.ReadAllText(outputPath);

      contents = contents.Replace(string.Format("Me.MainForm = Global.{0}.Form1", ProjectNamespace),
          string.Format("Me.MainForm = {0}.Form1", ProjectNamespace));
      File.WriteAllText(outputPath, contents);
    }

    public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
    {
      Dte = automationObject as DTE;

      connections = MySqlServerExplorerConnections.LoadMySqlConnectionsFromServerExplorer(Dte);
      WizardForm.connections = this.connections;
      WizardForm.dte = Dte;
      base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
    }


    private void AddBindings(VSProject vsProj)
    {
      SendToGeneralOutputWindow("Customizing Form Code...");
      // Get Form.cs
      ProjectItem item = FindProjectItem(vsProj.Project.ProjectItems, Strategy.GetFormFileName() );
      // Get Form.Designer.cs
      ProjectItem itemDesigner = FindProjectItem(item.ProjectItems, Strategy.GetFormDesignerFileName() );
      
      AddBindings((string)(item.Properties.Item("FullPath").Value));
      AddBindings((string)(itemDesigner.Properties.Item("FullPath").Value));
    }

    private string _canonicalTableName;
    private string _bindingSourceName;
    private StreamWriter sw;

    private void AddBindings(string FormPath)
    {
      _bindingSourceName = string.Format("{0}BindingSource", _canonicalTableName);
      string originalContents = File.ReadAllText(FormPath);
      FileStream fs = new FileStream(FormPath, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);
      using( StringReader sr = new StringReader(originalContents) )
      {
        using( sw = new StreamWriter( fs ) )
        {
          Strategy.Writer = sw;
          string line = null;
          while ((line = sr.ReadLine()) != null)
          {
            Strategy.Execute(line);
          }
        } // using StreamWriter
      } // using StreamReader
    }
  }
}
