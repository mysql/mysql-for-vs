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
