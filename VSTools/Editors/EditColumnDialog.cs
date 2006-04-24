using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MySql.VSTools
{
    internal partial class EditColumnDialog : Form
    {
        private ColumnNode node;

        public EditColumnDialog(ColumnNode nodeToEdit)
        {
            node = nodeToEdit;
            InitializeComponent();

            columnName.Text = node.Caption;
            columnType.SelectedItem = node.Typename;
            charSet.SelectedItem = node.CharacterSet;
            collation.SelectedItem = node.Collation;
            defaultValue.Text = node.DefaultValue;
            comment.Text = node.Comment;
            unsigned.Checked = node.IsUnsigned;
            zerofill.Checked = node.IsZeroFill;
            autoincrement.Checked = node.IsAutoIncrement;
            allowNull.Checked = node.AllowNulls;
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            node.Caption = columnName.Text;
            if (columnType.SelectedIndex != -1)
                node.Typename = columnType.SelectedItem.ToString();
            if (charSet.SelectedIndex != -1)
                node.CharacterSet = charSet.SelectedItem.ToString();
            if (collation.SelectedIndex != -1)
                node.Collation = collation.SelectedItem.ToString();
            node.DefaultValue = defaultValue.Text;
            node.Comment = comment.Text;
            node.IsUnsigned = unsigned.Checked;
            node.IsZeroFill = zerofill.Checked;
            node.IsAutoIncrement = autoincrement.Checked;
            node.AllowNulls = allowNull.Checked;
        }
    }
}