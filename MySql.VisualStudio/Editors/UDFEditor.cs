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
