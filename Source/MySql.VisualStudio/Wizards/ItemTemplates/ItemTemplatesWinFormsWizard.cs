// Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE80;
using MySql.Data.VisualStudio.SchemaComparer;


namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  /// <summary>
  /// Base class for Wizard's Form.
  /// </summary>
  public partial class ItemTemplatesWinFormsWizard : Form
  {
    #region "Properties"
    internal Button BtnFinish { get { return btnFinish; } }
    internal string SelectedModel { get { return modelEntitySelection_WinForms1.ComboModelsList.Text; } }
    internal string SelectedEntity { get { return modelEntitySelection_WinForms1.ComboEntities.Text; } }
    internal DataAccessTechnology DataAccessTechnology { get { return modelEntitySelection_WinForms1.DataAccessTechnology; } }
    internal GuiType GuiType
    {
      get
      {
        if (modelEntitySelection_WinForms1.RadControls.Checked)
          return GuiType.IndividualControls;
        else if (modelEntitySelection_WinForms1.RadGrid.Checked)
          return GuiType.Grid;
        else if (modelEntitySelection_WinForms1.RadMasterDetail.Checked)
          return GuiType.MasterDetail;
        else
          return GuiType.None;
      }
    }
    internal string SelectedDetailEntity { get { return modelEntitySelection_WinForms1.DetailEntityName; } }
    internal string ConstraintName { get { return modelEntitySelection_WinForms1.ConstraintName; } }
    #endregion

    private LanguageGenerator _language;
    private DTE _dte;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemTemplatesWinFormsWizard"/> class.
    /// </summary>
    /// <param name="language">The language generator (C# or VB.NET).</param>
    /// <param name="dte">The DTE object.</param>
    /// <param name="projectType">Type of the project.</param>
    public ItemTemplatesWinFormsWizard(LanguageGenerator language, DTE dte, ItemTemplateUtilities.ProjectWizardType projectType)
    {
      _language = language;
      _dte = dte;
      InitializeComponent();
      modelEntitySelection_WinForms1.WinFormWizardForm = this;
      modelEntitySelection_WinForms1.Dte = dte;
      modelEntitySelection_WinForms1.projectType = projectType;
      modelEntitySelection_WinForms1.FillComboModels();
      btnFinish.Enabled = false;
    }

    /// <summary>
    /// Handles the Click event of the btnCancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    internal virtual void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Close();
    }


    /// <summary>
    /// Handles the Click event of the btnFinish control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void btnFinish_Click(object sender, EventArgs e)
    {
      if (!modelEntitySelection_WinForms1.Entities_IsValid())
      {
        return;
      }

      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }
  }
}
