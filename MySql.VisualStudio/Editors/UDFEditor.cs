// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
