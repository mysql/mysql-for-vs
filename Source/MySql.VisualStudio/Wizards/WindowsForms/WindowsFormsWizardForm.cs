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
using MySql.Data.MySqlClient;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class WindowsFormsWizardForm : BaseWizardForm
  {

    #region "Properties exposed"

    internal GuiType GuiType { get { return dataAccessConfig1.GuiType; } }

    internal MySqlConnection Connection { get { return dataAccessConfig1.Connection; } }

    internal string TableName { get { return dataAccessConfig1.TableName; } }

    internal DataAccessTechnology DataAccessTechnology { get { return dataAccessConfig1.DataAccessTechnology; } }

    internal string ConstraintName { get { return dataAccessConfig1.ConstraintName; } }

    internal List<ColumnValidation> ValidationColumns { get { return validationConfig1.ValidationColumns; } }

    #endregion

    internal protected WindowsFormsWizard Wizard = null;

    public WindowsFormsWizardForm(WindowsFormsWizard Wizard)
      : base()
    {
      this.Wizard = Wizard;
      InitializeComponent();
    }

    private void WizardForm_Load(object sender, EventArgs e)
    {
      // Create linked list of wizard pages.
      Pages.Add(dataAccessConfig1);
      Pages.Add(validationConfig1);
      CurPage = dataAccessConfig1;
      Current = 0;

      BaseWizardForm_Load(sender, e);
    }
  }
}
