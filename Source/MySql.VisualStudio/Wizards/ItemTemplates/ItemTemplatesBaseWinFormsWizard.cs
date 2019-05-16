// Copyright (c) 2015, 2019, Oracle and/or its affiliates. All rights reserved.
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

using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards.WindowsForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSLangProj;

namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  public class ItemTemplatesBaseWinFormsWizard : IWizard
  {
    #region [ Private and internal fields ]
    private DTE dte;
    private bool wizardFinished;
    private string _projectPath;
    private string _projectNamespace;
    private string _itemTemplateTempPath;
    private string _NetFxVersion;
    private IVsOutputWindowPane _generalPane;
    private string _table;
    private string _detailTable;
    private Dictionary<string, Column> _columns;
    private Dictionary<string, Column> _detailColumns;
    internal List<ColumnValidation> _colValidations;
    private List<ColumnValidation> _colValidationsDetail;
    private Dictionary<string, ForeignKeyColumnInfo> ForeignKeys = new Dictionary<string, ForeignKeyColumnInfo>();
    internal Dictionary<string, ForeignKeyColumnInfo> DetailForeignKeys = new Dictionary<string, ForeignKeyColumnInfo>();
    private string _selectedModel;
    private string _selectedEntity;
    private string _detailEntity;
    private string _connectionString;
    private string _connectionName;
    private string _constraintName;
    private MySqlConnection _connection;
    private GuiType _GuiType;
    internal Dictionary<string, Dictionary<string, ColumnValidation>> ColumnMappings = new Dictionary<string, Dictionary<string, ColumnValidation>>();
    internal Dictionary<string, string> TablesIncludedInModel = new Dictionary<string, string>();
    private string _currentEntityFrameworkVersion;
    private bool _hasDataGridDateColumn;
    private IdentedStreamWriter sw;
    private DataAccessTechnology _dataAccessTechnology;
    private LanguageGenerator _language;
    private ItemTemplateUtilities.ProjectWizardType _projectType;
    #endregion

    #region [ IWIzard implementaion ]
    /// <summary>
    /// Runs custom wizard logic before opening an item in the template.
    /// </summary>
    /// <param name="projectItem">The project item that will be opened.</param>
    public void BeforeOpeningFile(ProjectItem projectItem)
    {
      return;
    }

    /// <summary>
    /// Runs custom wizard logic when a project has finished generating.
    /// </summary>
    /// <param name="project">The project that finished generating.</param>
    public void ProjectFinishedGenerating(Project project)
    {
      return;
    }

    /// <summary>
    /// Runs custom wizard logic when a project item has finished generating.
    /// </summary>
    /// <param name="projectItem">The project item that finished generating.</param>
    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
      return;
    }

    /// <summary>
    /// Runs custom wizard logic at the beginning of a template wizard run.
    /// </summary>
    /// <param name="automationObject">The automation object being used by the template wizard.</param>
    /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
    /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind" /> indicating the type of wizard run.</param>
    /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
    public void RunStarted(Object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, Object[] customParams)
    {
      try
      {
        dte = automationObject as DTE;
        Array activeProjects = (Array)dte.ActiveSolutionProjects;
        Project activeProj = (Project)activeProjects.GetValue(0);
        _projectPath = System.IO.Path.GetDirectoryName(activeProj.FullName);
        _projectNamespace = activeProj.Properties.Item("DefaultNamespace").Value.ToString();
        _NetFxVersion = replacementsDictionary["$targetframeworkversion$"];
        _itemTemplateTempPath = customParams[0].ToString().Substring(0, customParams[0].ToString().LastIndexOf("\\"));
        ItemTemplatesWinFormsWizard form = new ItemTemplatesWinFormsWizard(_language, dte, _projectType);
        form.StartPosition = FormStartPosition.CenterScreen;
        DialogResult result = form.ShowDialog();
        wizardFinished = result == System.Windows.Forms.DialogResult.OK ? true : false;
        if (wizardFinished)
        {
          GetFormValues(form);
          string providerConnectionString = ItemTemplateUtilities.GetProviderConnectionString(_selectedModel, dte, true);
          if (!string.IsNullOrEmpty(providerConnectionString))
          {
            _connectionString = providerConnectionString;
            _connectionName = ItemTemplateUtilities.GetConnectionStringName(_selectedModel, dte, true);
            _connection = new MySqlConnection(_connectionString);
          }
        }
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

    /// <summary>
    /// Runs custom wizard logic when the wizard has completed all tasks.
    /// </summary>
    public void RunFinished()
    {
      Array activeProjects = (Array)dte.ActiveSolutionProjects;
      Project project = (Project)activeProjects.GetValue(0);
      VSProject vsProj = null;
      if (wizardFinished)
      {
#if NET_46_OR_GREATER
        string frmName = string.Empty;
        SortedSet<string> tables = new SortedSet<string>();
        Dictionary<string, WindowsFormsCodeGeneratorStrategy> strategies = new Dictionary<string, WindowsFormsCodeGeneratorStrategy>();
        vsProj = project.Object as VSProject;
        ItemTemplateUtilities.CopyResourcesToProject(project, _itemTemplateTempPath);
        try
        {
          // Ensure all model exists, even if user didn't went through validation pages, so metadata for table used in FKs is already loaded.
          GenerateModelsForItemTemplates();
          string detailTableName = _detailEntity;
          string _canonicalTableName = ItemTemplateUtilities.GetCanonicalIdentifier(_selectedEntity);
          string canonicalDetailTableName = ItemTemplateUtilities.GetCanonicalIdentifier(detailTableName);
          // Gather all the tables
          tables.Add(_selectedEntity);
          if (!string.IsNullOrEmpty(detailTableName))
          {
            tables.Add(detailTableName);
          }

          foreach (KeyValuePair<string, MySql.Data.VisualStudio.Wizards.ForeignKeyColumnInfo> kvp2 in ForeignKeys)
          {
            tables.Add(kvp2.Value.ReferencedTableName);
          }

          foreach (KeyValuePair<string, MySql.Data.VisualStudio.Wizards.ForeignKeyColumnInfo> kvp2 in DetailForeignKeys)
          {
            tables.Add(kvp2.Value.ReferencedTableName);
          }

          AddColumnMappings(_canonicalTableName, _colValidations);
          if (!string.IsNullOrEmpty(detailTableName))
          {
            AddColumnMappings(canonicalDetailTableName, _colValidationsDetail);
          }

          InitializeColumnMappings(ForeignKeys);
          InitializeColumnMappings(DetailForeignKeys);
          // Generate the model using the proper technology
          ItemTemplateUtilities.GenerateEntityFrameworkModel(project, vsProj, _connection, _selectedModel, tables.ToList(), _projectPath,
            _currentEntityFrameworkVersion, _language, ColumnMappings, ref TablesIncludedInModel);

          // Generate screens for the selected table.
          _hasDataGridDateColumn = false;
          Dictionary<string, Column> Columns = _columns;
          Dictionary<string, Column> DetailColumns = _detailColumns;
          _canonicalTableName = ItemTemplateUtilities.GetCanonicalIdentifier(_selectedEntity);
          detailTableName = _detailEntity;
          canonicalDetailTableName = ItemTemplateUtilities.GetCanonicalIdentifier(detailTableName);
          if (!TablesIncludedInModel.ContainsKey(_selectedEntity))
          {
            SendToGeneralOutputWindow(string.Format("Skipping generation of screen for table '{0}' because it does not have primary key.", _selectedEntity));
            return;
          }

          if ((_GuiType == GuiType.MasterDetail) && !TablesIncludedInModel.ContainsKey(_detailEntity))
          {
            // If Detail table does not have PK, then you cannot edit details, "degrade" layout from Master Detail to Individual Controls.
            _GuiType = GuiType.IndividualControls;
            SendToGeneralOutputWindow(string.Format("Degrading layout for table '{0}' from master detail to single controls (because detail table '{1}' does not have primary key).",
                                                      _selectedEntity, _detailEntity));
          }

          // Create the strategy
          StrategyConfig config = new StrategyConfig(sw, _canonicalTableName, Columns, DetailColumns, _dataAccessTechnology, _GuiType, _language,
                                                    _colValidations != null, _colValidations, DetailValidationColumns, ItemTemplateUtilities.ConnectionStringWithIncludedPassword(_connection),
                                                    _connectionString, _selectedEntity, _detailEntity, _constraintName, ForeignKeys, DetailForeignKeys);
          WindowsFormsCodeGeneratorStrategy Strategy = WindowsFormsCodeGeneratorStrategy.GetInstance(config);
          strategies.Add(_selectedEntity, Strategy);
          if (!_hasDataGridDateColumn)
          {
            ItemTemplateUtilities.EnsureCodeForDateTimeGridColumn(vsProj, Columns, DetailColumns, _language, _projectPath, _projectNamespace);
          }

          // Add new form to project.
          frmName = string.Format("frm{0}", _canonicalTableName);
          string frmDesignerName = string.Format("frm{0}.designer", _canonicalTableName);
          ItemTemplateUtilities.AddNewForm(project, frmName, _projectPath, _projectNamespace, _language);

          // Now generated the bindings & custom code
          List<string> formNames = new List<string>();
          List<string> tableNames = new List<string>();
          if (!TablesIncludedInModel.ContainsKey(_selectedEntity))
          {
            return;
          }

          _canonicalTableName = ItemTemplateUtilities.GetCanonicalIdentifier(_selectedEntity);
          if (TablesIncludedInModel.ContainsKey(_selectedEntity))
          {
            frmName = string.Format("frm{0}", _canonicalTableName);
            formNames.Add(frmName);
            tableNames.Add(_selectedEntity);
            frmDesignerName = string.Format("frm{0}.designer", _canonicalTableName);
            WindowsFormsCodeGeneratorStrategy strategy = strategies[_selectedEntity];
            AddBindings(vsProj, strategy, frmName, frmDesignerName);
          }

          // This line is a hack to avoid "Project Unavailable" exceptions.
          project = (Project)((Array)(dte.ActiveSolutionProjects)).GetValue(0);
          vsProj = project.Object as VSProject;
          ItemTemplateUtilities.RemoveTemplateForm(vsProj, _projectPath, _language);
          ItemTemplateUtilities.FixNamespaces(_language, _projectPath, _projectNamespace, _dataAccessTechnology);
          // Update the model name with the Conn string name
          ItemTemplateUtilities.UpdateModelName(project, frmName, _projectPath, _projectNamespace, _connectionName, _language);
          if (_dataAccessTechnology == DataAccessTechnology.EntityFramework5)
          {
            string formFile = Path.Combine(_projectPath, _language == LanguageGenerator.CSharp ? string.Format("{0}.cs", frmName) : string.Format("{0}.vb", frmName));
            if (File.Exists(formFile))
            {
              string contents = "";
              contents = File.ReadAllText(formFile);
              string strToReplace = string.Format("ObjectResult<{0}> _entities = ctx.{0}.Execute(MergeOption.AppendOnly);", _selectedEntity);
              string strReplaceWith = string.Format("var _entities = ctx.{0}.ToList<{0}>();", _selectedEntity);
              contents = contents.Replace(strToReplace, strReplaceWith);
              File.WriteAllText(formFile, contents);
            }
          }

          SendToGeneralOutputWindow("Building Solution...");
          project.DTE.Solution.SolutionBuild.Build(true);
          Settings.Default.WinFormsWizardConnection = _connectionName;
          Settings.Default.Save();
          SendToGeneralOutputWindow("Finished item generation.");
        }
        catch (WizardException e)
        {
          SendToGeneralOutputWindow(string.Format("An error ocurred: {0}\n\n{1}", e.Message, e.StackTrace));
        }
#else
      throw new NotImplementedException();
#endif
      }
      else
      {
        vsProj = project.Object as VSProject;
        ItemTemplateUtilities.RemoveTemplateForm(vsProj, _projectPath, _language);
      }
    }

    /// <summary>
    /// Indicates whether the specified project item should be added to the project.
    /// </summary>
    /// <param name="filePath">The path to the project item.</param>
    /// <returns>
    /// true if the project item should be added to the project; otherwise, false.
    /// </returns>
    public bool ShouldAddProjectItem(string filePath)
    {
      return true;
    }
    #endregion

    /// <summary>
    /// Constructor. Initializes a new instance of the <see cref="ItemTemplatesBaseWinFormsWizard"/> class.
    /// </summary>
    /// <param name="language">The language generator (C# or VB .NET).</param>
    public ItemTemplatesBaseWinFormsWizard(LanguageGenerator language)
    {
      _currentEntityFrameworkVersion = null;
      _language = language;
      _projectType = ItemTemplateUtilities.ProjectWizardType.WindowsForms;
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

    /// <summary>
    /// Gets the detail validation columns.
    /// </summary>
    /// <value>
    /// The detail validation columns.
    /// </value>
    private List<ColumnValidation> DetailValidationColumns
    {
      get
      {
        if (_colValidationsDetail == null)
        {
          _colValidationsDetail = new List<ColumnValidation>();
        }

        return _colValidationsDetail;
      }
    }

    /// <summary>
    /// Gets the form values from the wizard, and set them into the base private fields.
    /// </summary>
    /// <param name="form">The wizard form.</param>
    private void GetFormValues(ItemTemplatesWinFormsWizard form)
    {
      _selectedModel = form.SelectedModel;
      _selectedEntity = form.SelectedEntity;
      _detailEntity = form.SelectedDetailEntity;
      _GuiType = form.GuiType;
      _constraintName = form.ConstraintName;
      _dataAccessTechnology = form.DataAccessTechnology;
    }

    /// <summary>
    /// Sends a message to the general output window.
    /// </summary>
    /// <param name="message">The message.</param>
    protected void SendToGeneralOutputWindow(string message)
    {
      if (_generalPane != null)
      {
        _generalPane.OutputString(Environment.NewLine + message);
      }
    }

    /// <summary>
    /// Generates the tables and columns based on the model information. Also, it gets the foreign keys data.
    /// </summary>
    private void GenerateModelsForItemTemplates()
    {
      if (_columns == null || _columns.Count == 0)
      {
        if (ForeignKeys != null)
        {
          ForeignKeys.Clear();
        }

        _table = _selectedEntity;
        _columns = BaseWizard<BaseWizardForm, WindowsFormsCodeGeneratorStrategy>.GetColumnsFromTable(_table, _connection);
        ItemTemplateUtilities.RetrieveAllFkInfo(_connection, _table, out ForeignKeys);
        _colValidations = ValidationsGrid.GetColumnValidationList(_table, _columns, ForeignKeys);
      }

      if ((_GuiType == Wizards.GuiType.MasterDetail) && ((_detailColumns == null) || (_detailColumns.Count == 0)))
      {
        GenerateDetailModelsForItemTemplates();
      }
    }

    /// <summary>
    /// Generates the detail tables definition based on the master-detail data of the model, getting the detail foreign keys as well.
    /// </summary>
    private void GenerateDetailModelsForItemTemplates()
    {
      if (DetailForeignKeys != null)
      {
        DetailForeignKeys.Clear();
      }

      _detailTable = _detailEntity;
      if (string.IsNullOrEmpty(_detailTable))
      {
        return;
      }

      _detailColumns = BaseWizard<BaseWizardForm, WindowsFormsCodeGeneratorStrategy>.GetColumnsFromTable(_detailTable, _connection);
      ItemTemplateUtilities.RetrieveAllFkInfo(_connection, _detailTable, out DetailForeignKeys);
      _colValidationsDetail = ValidationsGrid.GetColumnValidationList(_detailTable, _detailColumns, DetailForeignKeys);
    }

    /// <summary>
    /// Adds the column mappings data for the table specified.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="columns">The columns.</param>
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

    /// <summary>
    /// Initializes the column mappings.
    /// </summary>
    /// <param name="fks">The FKS.</param>
    internal void InitializeColumnMappings(Dictionary<string, MySql.Data.VisualStudio.Wizards.ForeignKeyColumnInfo> fks)
    {
      foreach (KeyValuePair<string, MySql.Data.VisualStudio.Wizards.ForeignKeyColumnInfo> kvp in fks)
      {
        string fkTableName = kvp.Value.ReferencedTableName;
        if (string.IsNullOrEmpty(fkTableName))
        {
          continue;
        }

        if (ColumnMappings.ContainsKey(fkTableName))
        {
          continue;
        }

        Dictionary<string, Column> dicCols = ItemTemplateUtilities.GetColumnsFromTable(fkTableName, _connection);
        List<ColumnValidation> myColValidations = ValidationsGrid.GetColumnValidationList(fkTableName, dicCols, null);
        ColumnMappings.Add(fkTableName, myColValidations.ToDictionary(p => { return p.Name; }));
      }
    }

    /// <summary>
    /// Adds the bindings.
    /// </summary>
    /// <param name="vsProj">The vs proj.</param>
    /// <param name="Strategy">The strategy.</param>
    /// <param name="frmName">Name of the form.</param>
    /// <param name="frmDesignerName">The name of the Form designer.</param>
    private void AddBindings(VSProject vsProj, WindowsFormsCodeGeneratorStrategy Strategy, string frmName, string frmDesignerName)
    {
      string ext = Strategy.GetExtension();
      SendToGeneralOutputWindow(string.Format("Customizing Form {0} Code...", frmName));
      // Get Form.cs
      ProjectItem item = ItemTemplateUtilities.FindProjectItem(vsProj.Project.ProjectItems, frmName + ext);
      // Get Form.Designer.cs
      ProjectItem itemDesigner = ItemTemplateUtilities.FindProjectItem(item.ProjectItems, frmDesignerName + ext);

      AddBindings((string)(item.Properties.Item("FullPath").Value), Strategy);
      AddBindings((string)(itemDesigner.Properties.Item("FullPath").Value), Strategy);
    }

    /// <summary>
    /// Adds the bindings.
    /// </summary>
    /// <param name="FormPath">The form path.</param>
    /// <param name="Strategy">The strategy.</param>
    private void AddBindings(string FormPath, WindowsFormsCodeGeneratorStrategy Strategy)
    {
      string originalContents = File.ReadAllText(FormPath);
      FileStream fs = new FileStream(FormPath, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);

      using (StringReader sr = new StringReader(originalContents))
      {
        using (sw = new IdentedStreamWriter(fs))
        {
          Strategy.Writer = sw;
          string line = null;

          while ((line = sr.ReadLine()) != null)
          {
            Strategy.Execute(line);
          }
        }
      }
    }
  }
}
