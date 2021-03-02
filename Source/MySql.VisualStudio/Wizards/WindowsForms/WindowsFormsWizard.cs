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
using System.Diagnostics;
using System.Collections;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  /// <summary>
  ///  Wizard for generation of a Windows Forms based project.
  /// </summary>
  public class WindowsFormsWizard : BaseWizard<WindowsFormsWizardForm, WindowsFormsCodeGeneratorStrategy>
  {    

    internal MySqlConnection Connection { get; set; }

    private bool _hasDataGridDateColumn;
    public WindowsFormsWizard(LanguageGenerator Language)
      : base(Language)
    {
      WizardForm = new WindowsFormsWizardForm(this);
      projectType = ProjectWizardType.WindowsForms;
    }

    /// <summary>
    /// If there is a DateTimePicker column and a grid layout, add the support code for custom DateTimePicker for Grids.
    /// </summary>
    /// <param name="vsProj"></param>
    private void EnsureCodeForDateTimeGridColumn(
      VSProject vsProj,
      Dictionary<string,Column> Columns,
      Dictionary<string,Column> DetailColumns )
    {
      bool hasDateColumn = false;

      foreach( KeyValuePair<string, Column> kvp in Columns )
      {
        if( kvp.Value.IsDateType() ) {
          hasDateColumn = true;
          break;
        }
      }
      if( !hasDateColumn && DetailColumns != null )
      {
        foreach (KeyValuePair<string, Column> kvp in DetailColumns)
        {
          if (kvp.Value.IsDateType())
          {
            hasDateColumn = true;
            break;
          }
        }
      }

      // If is the case, then add support code.
      if (hasDateColumn)
      {
        string outFilePath = "";
        Stream stream = null;
        if( Language == LanguageGenerator.CSharp )
        {
          stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.Data.VisualStudio.Wizards.WindowsForms.Templates.CS.MyDateTimePickerColumn.cs");
          outFilePath = Path.Combine(ProjectPath, "MyDateTimePickerColumn.cs");
        }
        else if (Language == LanguageGenerator.VBNET)
        {
          stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.Data.VisualStudio.Wizards.WindowsForms.Templates.VB.MyDateTimePickerColumn.vb");
          outFilePath = Path.Combine(ProjectPath, "MyDateTimePickerColumn.vb");
        }
        StreamReader sr = new StreamReader(stream);
        string contents = sr.ReadToEnd();
        File.WriteAllText(outFilePath, contents.Replace("$ProjectNamespace$", ProjectNamespace));
        vsProj.Project.ProjectItems.AddFromFile( outFilePath );
      }
      _hasDataGridDateColumn = hasDateColumn;
    }

    public override void ProjectFinishedGenerating(Project project)
    {
#if NET_461_OR_GREATER
      //Dictionary<string,object> dic = GetAllProperties(project.Properties);      
      
      VSProject vsProj = project.Object as VSProject;
      SortedSet<string> tables = new SortedSet<string>();
      Dictionary<string, WindowsFormsCodeGeneratorStrategy> strategies = new Dictionary<string, WindowsFormsCodeGeneratorStrategy>();

      vsProj.References.Add("MySql.Data");
      project.DTE.SuppressUI = true;
      vsProj.Project.Save();

      bool found = false;
      foreach (Reference reference in vsProj.References)
      {
        if (((Reference)reference).Name.IndexOf("MySql.Data",StringComparison.CurrentCultureIgnoreCase) >=0 && !String.IsNullOrEmpty(reference.Path))
        {
          found = true;
          break;
        }
      }

      using (var okCancelDialog = Common.Utilities.GetOkCancelInfoDialog(
                                    Utility.Forms.InfoDialog.InfoType.Warning,
                                    "The MySQL .NET driver could not be found",
                                    @"To use it you must download and install the MySQL Connector/Net package from http://dev.mysql.com/downloads/connector/net/",
                                    "Click OK to go to the page or Cancel to continue."
      ))
      {
        if (!found && okCancelDialog.ShowDialog() == DialogResult.OK)
        {
          ProcessStartInfo browserInfo = new ProcessStartInfo("http://dev.mysql.com/downloads/connector/net/");
          System.Diagnostics.Process.Start(browserInfo);
        }
      }
       
      try
      {

        for (int i = 0; i < WizardForm.SelectedTables.Count; i++)
        {
          AdvancedWizardForm crud = WizardForm.CrudConfiguration[WizardForm.SelectedTables[i].Name];
          // Ensure all model exists, even if user didn't went through validation pages.
          // So metadata for table used in FKs is already loaded.
          crud.GenerateModels();
          string detailTableName = crud.DetailTableName;
          string _canonicalTableName = GetCanonicalIdentifier(crud.TableName);
          string canonicalDetailTableName = GetCanonicalIdentifier(detailTableName);
          // Gather all the tables
          tables.Add(crud.TableName);
          if (!string.IsNullOrEmpty(detailTableName))
              tables.Add(detailTableName);
          foreach (KeyValuePair<string, ForeignKeyColumnInfo> kvp2 in crud.ForeignKeys)
          {
            tables.Add(kvp2.Value.ReferencedTableName);
          }
          foreach (KeyValuePair<string, ForeignKeyColumnInfo> kvp2 in crud.DetailForeignKeys)
          {
            tables.Add(kvp2.Value.ReferencedTableName);
          }

          AddColumnMappings(_canonicalTableName, crud.ValidationColumns);
          if (!string.IsNullOrEmpty(detailTableName))
          {
            AddColumnMappings(canonicalDetailTableName, crud.ValidationColumnsDetail);
          }

          InitializeColumnMappings(crud.ForeignKeys);
          InitializeColumnMappings(crud.DetailForeignKeys);
        }

        // Generate the model using the proper technology
        if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5 ||
          WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6)
        {
          if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5)
            CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_5;
          else
            CurrentEntityFrameworkVersion = ENTITY_FRAMEWORK_VERSION_6;

          AddNugetPackage(vsProj, ENTITY_FRAMEWORK_PCK_NAME, CurrentEntityFrameworkVersion, true);
          GenerateEntityFrameworkModel(project, vsProj, WizardForm.Connection, "Model1", tables.ToList(), ProjectPath);
        }
        else if (WizardForm.DataAccessTechnology == DataAccessTechnology.TypedDataSet)
        {
          PopulateColumnMappingsForTypedDataSet();
          GenerateTypedDataSetModel(vsProj, WizardForm.Connection, tables.ToList());
        }

        try
        {
          _hasDataGridDateColumn = false;
          // Start a loop here, to generate screens for all the selected tables.
          for (int i = 0; i < WizardForm.SelectedTables.Count; i++)
          {
            AdvancedWizardForm crud = WizardForm.CrudConfiguration[WizardForm.SelectedTables[i].Name];
            Dictionary<string, Column> Columns = crud.Columns;
            Dictionary<string, Column> DetailColumns = crud.DetailColumns;
            string _canonicalTableName = GetCanonicalIdentifier(crud.TableName);
            string detailTableName = crud.DetailTableName;
            string canonicalDetailTableName = GetCanonicalIdentifier(detailTableName);

            if (!TablesIncludedInModel.ContainsKey(crud.TableName))
            {
              SendToGeneralOutputWindow(string.Format("Skipping generation of screen for table '{0}' because it does not have primary key.", crud.TableName));
              continue;
            }

            if ( (crud.GuiType == GuiType.MasterDetail) && !TablesIncludedInModel.ContainsKey(crud.DetailTableName))
            {
              // If Detail table does not have PK, then you cannot edit details, "degrade" layout from Master Detail to Individual Controls.
              crud.GuiType = GuiType.IndividualControls;
              SendToGeneralOutputWindow( string.Format(
                "Degrading layout for table '{0}' from master detail to single controls (because detail table '{1}' does not have primary key).",
                crud.TableName, crud.DetailTableName ) );
            }

            // Create the strategy
            StrategyConfig config = new StrategyConfig(sw, _canonicalTableName, Columns, DetailColumns,
              WizardForm.DataAccessTechnology, crud.GuiType, Language,
              crud.ValidationsEnabled, crud.ValidationColumns, crud.ValidationColumnsDetail,
              WizardForm.ConnectionStringWithIncludedPassword, WizardForm.ConnectionString, crud.TableName,
              detailTableName, crud.ConstraintName, crud.ForeignKeys, crud.DetailForeignKeys);
            WindowsFormsCodeGeneratorStrategy Strategy = WindowsFormsCodeGeneratorStrategy.GetInstance(config);
            strategies.Add(WizardForm.SelectedTables[i].Name, Strategy);

            if (!_hasDataGridDateColumn)
            {
              EnsureCodeForDateTimeGridColumn(vsProj, Columns, DetailColumns);
            }

            string frmName = string.Format("frm{0}", _canonicalTableName);
            string frmDesignerName = string.Format("frm{0}.designer", _canonicalTableName);
            // Add new form to project.
            AddNewForm(project, frmName, Strategy);
          }
        }
        catch (WizardException e)
        {
          SendToGeneralOutputWindow(string.Format("An error ocurred: {0}\n\n{1}", e.Message, e.StackTrace));
        }
        
        // Now generated the bindings & custom code
        List<string> formNames = new List<string>();
        List<string> tableNames = new List<string>();
        for (int i = 0; i < WizardForm.SelectedTables.Count; i++)
        {
          AdvancedWizardForm crud = WizardForm.CrudConfiguration[WizardForm.SelectedTables[i].Name];
          string _canonicalTableName = GetCanonicalIdentifier(crud.TableName);
          if (!TablesIncludedInModel.ContainsKey(crud.TableName))
              continue;

          string frmName = string.Format("frm{0}", _canonicalTableName);
          formNames.Add(frmName);
          tableNames.Add(crud.TableName);
          string frmDesignerName = string.Format("frm{0}.designer", _canonicalTableName);
          WindowsFormsCodeGeneratorStrategy strategy = strategies[WizardForm.SelectedTables[i].Name];
          AddBindings(vsProj, strategy, frmName, frmDesignerName);
        }
        // Add menu entries for each form
        AddMenuEntries(vsProj, formNames, tableNames);

        if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6)
        {
          // Change target version to 4.5 (only version currently supported for EF6).
          project.Properties.Item("TargetFrameworkMoniker").Value = ".NETFramework,Version=v4.5";
          // This line is a hack to avoid "Project Unavailable" exceptions.
          project = (Project)((Array)(Dte.ActiveSolutionProjects)).GetValue(0);
          vsProj = project.Object as VSProject;
        }

        RemoveTemplateForm(vsProj);

        FixNamespaces();
        
        SendToGeneralOutputWindow("Building Solution...");
        project.DTE.Solution.SolutionBuild.Build(true);

        Settings.Default.WinFormsWizardConnection = WizardForm.ConnectionName;
        Settings.Default.Save();
        
        SendToGeneralOutputWindow("Finished project generation.");

        if (project.DTE.Solution.SolutionBuild.LastBuildInfo > 0)
        {
          Logger.LogError("Solution build failed. Please check that all project references have been resolved.", true);
        }

        WizardForm.Dispose();
      }
      catch (WizardException e)
      {
        SendToGeneralOutputWindow(string.Format("An error ocurred: {0}\n\n{1}", e.Message, e.StackTrace));
      }
#else
      throw new NotImplementedException();
#endif

    }

    protected override string GetConnectionString()
    {
      return WizardForm.ConnectionStringWithIncludedPassword;
    }

    internal virtual void RemoveTemplateForm(VSProject proj)
    {
    }

    /// <summary>
    ///  Creates and adds a new Windows Forms to the project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="formName"></param>
    private void AddNewForm( Project project, string formName, WindowsFormsCodeGeneratorStrategy strategy )
    {
      //project.ProjectItems.Item(1).Remove();
      string formFile = Path.Combine( ProjectPath, strategy.GetFormFileName().Replace("Form1", formName) );
      string formFileDesigner = Path.Combine( ProjectPath,  strategy.GetFormDesignerFileName().Replace("Form1", formName));
      string formFileResx = Path.Combine( ProjectPath, strategy.GetFormResxFileName().Replace("Form1", formName));
      string contents = "";
      
      contents = File.ReadAllText(Path.Combine(ProjectPath, strategy.GetFormFileName()));
      contents = contents.Replace("Form1", formName);
      File.WriteAllText(formFile, contents);

      contents = File.ReadAllText(Path.Combine(ProjectPath, strategy.GetFormDesignerFileName()));
      contents = contents.Replace("Form1", formName);
      File.WriteAllText(formFileDesigner, contents);

      contents = File.ReadAllText(Path.Combine(ProjectPath, strategy.GetFormResxFileName()));
      contents = contents.Replace("Form1", formName);
      File.WriteAllText(formFileResx, contents);

      // Now add the form
      ProjectItem pi = project.ProjectItems.AddFromFile(formFile);
      ProjectItem pi2 = pi.ProjectItems.AddFromFile(formFileDesigner);
      ProjectItem pi3 = pi.ProjectItems.AddFromFile(formFileResx);
      pi3.Properties.Item("ItemType").Value = "EmbeddedResource";
      //pi.Properties.Item("ItemType").Value = "Compile";
      pi.Properties.Item("SubType").Value = "Form";
    }

    internal void InitializeColumnMappings(Dictionary<string, ForeignKeyColumnInfo> fks)
    {
      foreach (KeyValuePair<string, ForeignKeyColumnInfo> kvp in fks)
      { 
        string fkTableName = kvp.Value.ReferencedTableName;
        if( string.IsNullOrEmpty( fkTableName )) continue;
        if (ColumnMappings.ContainsKey(fkTableName)) continue;
        Dictionary<string,Column> dicCols = GetColumnsFromTable(fkTableName, WizardForm.Connection);
        List<ColumnValidation> myColValidations = ValidationsGrid.GetColumnValidationList(fkTableName, dicCols, null);
        ColumnMappings.Add(fkTableName, myColValidations.ToDictionary(p => { return p.Name; }));
      }
    }

    /// <summary>
    /// Fixes namespaces for issue in VB.NET with some VS versions like 2013.
    /// </summary>
    /// <param name="outputPath"></param>
    private void FixNamespaces()
    {
      if (Language != LanguageGenerator.VBNET) return;
      string outputPath = Path.Combine(Path.Combine(ProjectPath, "My Project"), "Application.Designer.vb");
      string contents = File.ReadAllText(outputPath);
      if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6)
      {
        contents = contents.Replace(string.Format("Me.MainForm = Global.{0}.frmMain", ProjectNamespace),
          string.Format("Me.MainForm = Global.{0}.{0}.frmMain", ProjectNamespace));
      }
      else { 
        contents = contents.Replace(string.Format("Me.MainForm = Global.{0}.frmMain", ProjectNamespace),
          string.Format("Me.MainForm = {0}.frmMain", ProjectNamespace));
      }
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

    protected virtual void AddMenuEntries(VSProject vsProj, List<string> formNames, List<string> tableNames)
    {
    }

    protected virtual void WriteMenuHandler(StreamWriter sw, string formName)
    {
    }
    
    protected virtual void WriteMenuStripConstruction(StreamWriter sw, string formName) 
    {
    }

    protected virtual void WriteMenuAddRange(StreamWriter sw, string formName)
    {
    }

    protected virtual void WriteMenuControlInit(StreamWriter sw, string formName, string tableName)
    {
    }

    protected virtual void WriteAddRangeBegin( StreamWriter sw )
    {
    }

    protected virtual void WriteAddRangeEnd( StreamWriter sw )
    {
    }

    protected virtual void WriteMenuDeclaration( StreamWriter sw, string formName)
    {
    }

    protected virtual string MenuEventHandlerMarker { get { return ""; } }

    protected virtual string MenuDesignerControlDeclMarker { get { return ""; } }

    protected virtual string MenuDesignerControlInitMarker { get { return ""; } }

    protected virtual string MenuDesignerBeforeSuspendLayout { get { return ""; } }

    protected void WriteMenuEntries(string path, List<string> formNames)
    {
      string originalContents = File.ReadAllText(path);
      FileStream fs = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);
      using (StringReader sr = new StringReader(originalContents))
      {
        using (  StreamWriter sw = new StreamWriter(fs))
        {
          string line = null;
          while ((line = sr.ReadLine()) != null)
          {
            if (line.Trim() == MenuEventHandlerMarker)
            {
              for (int i = 0; i < formNames.Count; i++)
              {
                string formName = formNames[i];
                WriteMenuHandler(sw, formName);
              }
            }
            else
            {
              sw.WriteLine(line);
            }
          }
        }
      }
    }

    protected void WriteMenuDesignerEntries(string path, List<string> formNames, List<string> tableNames)
    {
      string originalContents = File.ReadAllText(path);
      FileStream fs = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);
      using (StringReader sr = new StringReader(originalContents))
      {
        using (StreamWriter sw = new StreamWriter(fs))
        {
          string line = null;
          while ((line = sr.ReadLine()) != null)
          {
            if (line.Trim() == MenuDesignerBeforeSuspendLayout)
            {
              for (int i = 0; i < formNames.Count; i++)
              {
                string formName = formNames[i];
                WriteMenuStripConstruction(sw, formName);
              }
            }
            else if (line.Trim() == MenuDesignerControlInitMarker)
            {
              WriteAddRangeBegin(sw);
              for (int i = 0; i < formNames.Count; i++)
              {
                string formName = formNames[i];
                WriteMenuAddRange(sw, formName);
                if( i < formNames.Count - 1 )
                {
                  sw.Write(", ");
                }
              }
              WriteAddRangeEnd(sw);

              for (int i = 0; i < formNames.Count; i++)
              {
                string formName = formNames[i];
                string tableName = tableNames[i];
                WriteMenuControlInit(sw, formName, tableName);
              }
            }
            else if (line.Trim() == MenuDesignerControlDeclMarker)
            {
              for (int i = 0; i < formNames.Count; i++)
              {
                string formName = formNames[i];
                WriteMenuDeclaration(sw, formName);
              }
            }
            else
            {
              sw.WriteLine( line );
            }
          }
        }
      }
    }

    private void AddBindings(VSProject vsProj, WindowsFormsCodeGeneratorStrategy Strategy, 
      string frmName, string frmDesignerName )
    {
      string ext = Strategy.GetExtension();
      SendToGeneralOutputWindow( string.Format( "Customizing Form {0} Code...", frmName ));
      // Get Form.cs
      ProjectItem item = FindProjectItem(vsProj.Project.ProjectItems, frmName + ext );
      // Get Form.Designer.cs
      ProjectItem itemDesigner = FindProjectItem(item.ProjectItems, frmDesignerName + ext );
      
      AddBindings((string)(item.Properties.Item("FullPath").Value), Strategy);
      AddBindings((string)(itemDesigner.Properties.Item("FullPath").Value), Strategy);
    }
    
    private IdentedStreamWriter sw;

    private void AddBindings(string FormPath, WindowsFormsCodeGeneratorStrategy Strategy)
    {
      string originalContents = File.ReadAllText(FormPath);
      FileStream fs = new FileStream(FormPath, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);
      using( StringReader sr = new StringReader(originalContents) )
      {
        using( sw = new IdentedStreamWriter( fs ) )
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
