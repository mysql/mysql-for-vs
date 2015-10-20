﻿// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio;
using MySql.Data.MySqlClient;
using EnvDTE;
using MySql.Data.VisualStudio.DBExport;
using VSLangProj;
using System.Xml.Linq;

namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  public partial class ModelEntitySelection : UserControl
  {
    internal List<string> models = null;
    protected BindingList<DbTables> _tables = new BindingList<DbTables>();
    internal BindingSource _sourceTables = new BindingSource();
    private BackgroundWorker _worker;
    private AutoCompleteStringCollection _modelsAutoCompleteCollection, _entitiesAutoCompleteCollection;
    private const string _runWorkerCompletedErrorMessage = "The following error ocurred while exporting:";

    public ModelEntitySelection()
    {
      InitializeComponent();
      comboModelsList.KeyDown += comboModelsList_KeyDown;
      comboModelsList.TextChanged += comboModelsList_TextChanged;
      comboEntities.KeyDown += comboEntities_KeyDown;
      comboEntities.TextChanged += comboEntities_TextChanged;
    }

    internal ItemTemplateUtilities.ProjectWizardType projectType { get; set; }
    internal ItemTemplatesWinFormsWizard WinFormWizardForm { get; set; }
    internal ItemTemplatesWebWizard WebWizardForm { get; set; }
    internal BackgroundWorker Worker { get { return _worker; } set { _worker = value; } }
    internal string ConnectionString { get; set; }
    internal string ConnectionName { get; set; }
    internal string SelectedEntity { get { return comboEntities.Text; } }
    internal List<DbTables> SelectedTables
    {
      get
      {
        return _tables.Where(t => t.Name == comboEntities.Text).ToList();
      }
    }
    internal System.Windows.Forms.ComboBox ComboModelsList { get { return comboModelsList; } }
    internal System.Windows.Forms.ComboBox ComboEntities { get { return comboEntities; } }
    internal DataAccessTechnology DataAccessTechnology
    {
      get
      {
        if (Models_IsValid())
          return ItemTemplateUtilities.GetEntityFrameworkVersion(Dte, comboModelsList.Text, projectType == ItemTemplateUtilities.ProjectWizardType.WindowsForms ? true : false);
        return DataAccessTechnology.None;
      }
    }
    internal DTE Dte { get; set; }
    internal string SelectedModel
    {
      get
      {
        if (!string.IsNullOrEmpty(comboModelsList.SelectedItem.ToString()))
        {
          return comboModelsList.SelectedItem.ToString();
        }

        return string.Empty;
      }
    }

    internal void FillComboModels()
    {
      try
      {
        LockUI();
        Array activeProjects = (Array)Dte.ActiveSolutionProjects;
        Project project = (Project)activeProjects.GetValue(0);
        models = new List<string>();
        models = ItemTemplateUtilities.GetModels(project.ProjectItems, ref models);

        foreach (var model in models)
        {
          comboModelsList.Items.Add(model);
        }

        SetModelsAutoCompleteCollection();
      }
      finally
      {
        UnlockUI();
      }
    }

    internal void SetModelsAutoCompleteCollection()
    {
      _modelsAutoCompleteCollection = new AutoCompleteStringCollection();
      comboModelsList.AutoCompleteCustomSource = _modelsAutoCompleteCollection;
      comboModelsList.AutoCompleteMode = AutoCompleteMode.Suggest;
      comboModelsList.AutoCompleteSource = AutoCompleteSource.CustomSource;

      if (models.Count > 0)
        _modelsAutoCompleteCollection.AddRange(models.ToArray());
    }

    internal void SetEntitiesAutoCompleteCollection()
    {
      _entitiesAutoCompleteCollection = new AutoCompleteStringCollection();
      comboEntities.AutoCompleteCustomSource = _entitiesAutoCompleteCollection;
      comboEntities.AutoCompleteMode = AutoCompleteMode.Suggest;
      comboEntities.AutoCompleteSource = AutoCompleteSource.CustomSource;

      if (_tables.Count > 0)
        _entitiesAutoCompleteCollection.AddRange(_tables.ToList().Select(t => t.Name).ToArray());
    }

    internal void comboModelsList_KeyDown(object sender, KeyEventArgs e)
    {
      comboModelsList_Toggle();
    }

    internal void comboModelsList_TextChanged(object sender, EventArgs e)
    {
      comboModelsList_Toggle();
    }

    internal void comboModelsList_Toggle()
    {
      ClearEntitiesOptions();

      if (Models_IsValid())
      {
        pnlEntityContainer.Enabled = true;
        FillTables(SelectedModel, Dte, projectType == ItemTemplateUtilities.ProjectWizardType.WindowsForms ? true : false);
      }
      else
      {
        pnlEntityContainer.Enabled = false;
      }
    }

    internal bool Models_IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      ModelSelection_Validating(this, args);

      return !args.Cancel;      
    }

    internal void ModelSelection_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = string.IsNullOrEmpty(comboModelsList.Text) || comboModelsList.Items.Cast<string>().FirstOrDefault(i => i == comboModelsList.Text) == null
                  || comboModelsList.Items.Count == 0;
    }

    internal void LockUI()
    {
      EnableControls(false);
    }

    internal void UnlockUI()
    {
      if (!(_worker != null && _worker.IsBusy))
      {
        EnableControls(true);
      }
    }

    internal void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.Invoke((Action)(() =>
        {
          MessageBox.Show(
            string.Format("{0} {1}", _runWorkerCompletedErrorMessage, e.Error.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }));
      }

      UnlockUI();
    }

    internal virtual void EnableControls(bool enabling)
    {
      comboEntities.Enabled = enabling;
    }

    internal void FillTables(string modelName, DTE dte, bool checkForAppConfig)
    {
      string edmxFileName = string.Format("{0}.edmx", modelName);
      string providerConnectionString = ItemTemplateUtilities.GetProviderConnectionString(edmxFileName, dte, checkForAppConfig);

      if (string.IsNullOrEmpty(providerConnectionString))
      {
        return;
      }

      this.ConnectionString = providerConnectionString;
      this.ConnectionName = ItemTemplateUtilities.GetConnectionStringName(edmxFileName, dte, checkForAppConfig);
      LockUI();

      try
      {
        DoWorkEventHandler doWorker = (worker, doWorkerArgs) =>
        {
          Application.DoEvents();
          var cnn = new MySqlConnection(providerConnectionString);
          cnn.Open();
          var dtTables = cnn.GetSchema("Tables", new string[] { null, cnn.Database });
          cnn.Close();
          _tables = new BindingList<DbTables>();

          this.Invoke((Action)(() =>
          {
            ComboEntities.Items.Clear();

            for (int i = 0; i < dtTables.Rows.Count; i++)
              _tables.Add(new DbTables(false, dtTables.Rows[i][2].ToString()));

            _sourceTables.DataSource = _tables;

            foreach (string table in _tables.Select(t => t.Name))
            {
              ComboEntities.Items.Add(table);
            }

            SetEntitiesAutoCompleteCollection();
          }));
        };

        if (Worker != null)
        {
          Worker.DoWork -= doWorker;
          Worker.RunWorkerCompleted -= _worker_RunWorkerCompleted;
          Worker.Dispose();
        }

        Worker = new BackgroundWorker();
        Worker.WorkerSupportsCancellation = true;
        Worker.DoWork += doWorker;
        Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
        Worker.RunWorkerAsync();
      }
      finally
      {
        UnlockUI();
      }
    }

    private void ClearEntitiesOptions()
    {
      ComboEntities.Items.Clear();
      ComboEntities.Text = string.Empty;
    }

    private void comboEntities_KeyDown(object sender, KeyEventArgs e)
    {
      comboEntities_Toggle();
    }

    private void comboEntities_TextChanged(object sender, EventArgs e)
    {
      comboEntities_Toggle();
    }

    internal virtual void comboEntities_Toggle()
    {
      ToggleWizardBtnFinish(Entities_IsValid());
    }

    internal bool Entities_IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      EntitySelection_Validating(this, args);

      if (args.Cancel)
        return false;
      else
        return true;
    }

    internal virtual void EntitySelection_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = comboEntities.Items.Count == 0 || string.IsNullOrEmpty(comboEntities.Text) || 
        comboEntities.Items.Cast<string>().FirstOrDefault(i => i == comboEntities.Text) == null;
    }

    internal void ToggleWizardBtnFinish(bool enabling)
    {
      if (WinFormWizardForm != null)
      {
        WinFormWizardForm.BtnFinish.Enabled = enabling;
      }
      if (WebWizardForm != null)
      {
        WebWizardForm.BtnFinish.Enabled = enabling;
      }
    }
  }
}
