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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Base class for Wizard's Form.
  /// </summary>
  public partial class BaseWizardForm : Form
  {
    protected List<WizardPage> Pages = new List<WizardPage>();
    protected int Current = 0;
    protected WizardPage CurPage = null;

    public BaseWizardForm()
    {
      InitializeComponent();
      // this.Load += new System.EventHandler(this.BaseWizardForm_Load);
    }

    protected void BaseWizardForm_Load(object sender, EventArgs e)
    {
      for (int i = 0; i < Pages.Count; i++)
      {
        Pages[i].Visible = (Pages[i] == CurPage);
      }

      btnBack.Enabled = false;
      btnNext.Enabled = true;

      ShowFinishButton(false);
      CurPage.OnStarting(this);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnBack_Click(object sender, EventArgs e)
    {
      int prevCurrent = Current;
      if (Current == (Pages.Count - 1))
        ShowFinishButton(false);
      if (Current > 0)
      {
        CurPage = Pages[--Current];
        Pages[prevCurrent].Visible = false;
        Pages[Current].Visible = true;
        btnNext.Enabled = true;
      }
      if (Current == 0)
      {
        btnBack.Enabled = false;
      }
      CurPage.OnStarting(this);
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      int prevCurrent = Current;
      if (!CurPage.IsValid()) return;
      if (Current < (Pages.Count - 1))
      {
        CurPage = Pages[++Current];
        Pages[prevCurrent].Visible = false;
        Pages[Current].Visible = true;
        btnBack.Enabled = true;
      }
      if (Current == (Pages.Count - 1))
      {
        ShowFinishButton(true);
      }
      CurPage.OnStarting(this);
    }

    private void ShowFinishButton(bool showFinish)
    {
      btnNext.Enabled = !showFinish;
      btnFinish.Enabled = showFinish;
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      if (!CurPage.IsValid()) return;
      else
      {
        // this form keeps all the user selections handy so the IWizard can customize the project template.
        this.Close();
      }
    }
  }
}
