// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  public partial class UDFEditor : Form
  {
    public UDFEditor()
    {
      InitializeComponent();
      returnType.SelectedIndex = 0;
    }

    public bool Aggregate
    {
      get { return aggregate.Checked; }
    }

    public string FunctionName
    {
      get { return functionName.Text.Trim(); }
    }

    public string LibraryName
    {
      get { return libraryName.Text.Trim(); }
    }

    public int ReturnType
    {
      get { return returnType.SelectedIndex; }
    }

    public string ReturnTypeByName
    {
      get { return returnType.SelectedItem as string; }
    }

    private void functionName_TextChanged(object sender, EventArgs e)
    {
      UpdateOkButton();
    }

    private void libraryName_TextChanged(object sender, EventArgs e)
    {
      UpdateOkButton();
    }

    private void UpdateOkButton()
    {
      okButton.Enabled = functionName.Text.Trim().Length > 0 &&
          libraryName.Text.Trim().Length > 0;
    }
  }
}
