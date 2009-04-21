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
using System.IO;

namespace MySql.Data.VisualStudio.Editors
{
    partial class GenerateChangeScriptDialog : Form
    {
        string sql;

        public GenerateChangeScriptDialog(TableNode node)
        {
            sql = node.GetSaveSql();
            InitializeComponent();
            sqlBox.Text = sql;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".sql";
            dlg.CheckPathExists = true;
            dlg.Filter = "SQL Files|*.sql|All Files|*.*";
            dlg.OverwritePrompt = true;
            dlg.Title = "Save Change Script";
            dlg.AutoUpgradeEnabled = false;
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DialogResult result = dlg.ShowDialog();
            if (DialogResult.OK == result)
                WriteOutChangeScript(dlg.FileName);
            Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WriteOutChangeScript(string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(sql);
            sw.Flush();
            sw.Close();
        }
    }
}
