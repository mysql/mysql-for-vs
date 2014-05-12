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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards.WindowsForms;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  internal class CSharpTypedDatasetDataGridStrategy : CSharpDataGridStrategy
  {
    internal CSharpTypedDatasetDataGridStrategy(StrategyConfig config)
      : base(config)
    {
    }

    protected override void WriteUsingUserCode()
    {
      // Nothing
    }

    protected override void WriteFormLoadCode()
    {
      Writer.WriteLine("string strConn = \"{0};\";", ConnectionString);
      Writer.WriteLine("ad = new MySqlDataAdapter(\"select * from `{0}`\", strConn);", TableName);
      Writer.WriteLine("MySqlCommandBuilder builder = new MySqlCommandBuilder(ad);");
      Writer.WriteLine("ad.Fill(this.newDataSet.{0});", CanonicalTableName );
      Writer.WriteLine("ad.DeleteCommand = builder.GetDeleteCommand();");
      Writer.WriteLine("ad.UpdateCommand = builder.GetUpdateCommand();");
      Writer.WriteLine("ad.InsertCommand = builder.GetInsertCommand();");

      Writer.WriteLine("bindingNavigatorAddNewItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorCountItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorDeleteItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveFirstItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMovePreviousItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorPositionItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveNextItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveLastItem.Enabled = false;");
      Writer.WriteLine("toolStripButton1.Enabled = true;");
    }

    protected override void WriteValidationCode()
    {
      bool validationsEnabled = ValidationsEnabled;
      List<ColumnValidation> validationColumns = ValidationColumns;
      if (validationsEnabled)
      {
        Writer.WriteLine("private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)");
        Writer.WriteLine("{");

        Writer.WriteLine("  int v;");
        Writer.WriteLine("  string s;");
        Writer.WriteLine("  DataGridViewRow row = dataGridView1.Rows[e.RowIndex];");
        Writer.WriteLine("  object value = e.FormattedValue;");
        Writer.WriteLine("  e.Cancel = false;");
        Writer.WriteLine("  row.ErrorText = \"\";");
        Writer.WriteLine("  if (row.IsNewRow) return;");
        for (int i = 0; i < validationColumns.Count; i++)
        {
          ColumnValidation cv = validationColumns[i];
          string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);

          Writer.WriteLine("  if (e.ColumnIndex == {0})", i);
          Writer.WriteLine("  {");

          if (cv.Required)
          {
            Writer.WriteLine("  if( (value is DBNull) || string.IsNullOrEmpty( value.ToString() ) ) { ");
            Writer.WriteLine("    e.Cancel = true;");
            Writer.WriteLine("    row.ErrorText = \"The field {0} is required\";", cv.Name);
            Writer.WriteLine("    return;");
            Writer.WriteLine("  }");
          }
          if (cv.IsNumericType())
          {
            Writer.WriteLine("  s = value.ToString();");
            Writer.WriteLine("  if( !int.TryParse( s, out v ) ) {");
            Writer.WriteLine("    e.Cancel = true;");
            Writer.WriteLine("    row.ErrorText = \"The field {0} must be numeric.\";", cv.Name);
            Writer.WriteLine("    return;");
            Writer.WriteLine("  }");
            if (cv.MinValue != null)
            {
              Writer.WriteLine(" else if( cv.MinValue > v ) { ");
              Writer.WriteLine("    e.Cancel = true;");
              Writer.WriteLine("    row.ErrorText = \"The field {0} must be greater or equal than {1}.\";", cv.Name, cv.MinValue);
              Writer.WriteLine("    return;");
              Writer.WriteLine(" } ");
            }
            if (cv.MaxValue != null)
            {
              Writer.WriteLine(" else if( cv.MaxValue < v ) { ");
              Writer.WriteLine("    e.Cancel = true;");
              Writer.WriteLine("    row.ErrorText = \"The field {0} must be lesser or equal than {1}\";", cv.Name, cv.MaxValue);
              Writer.WriteLine("    return;");
              Writer.WriteLine(" } ");
            }
          }

          Writer.WriteLine("  }");
        }
        Writer.WriteLine("}");

      }
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("private MySqlDataAdapter ad;");
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit();", CanonicalTableName);
      Writer.WriteLine("ad.Update(this.newDataSet.{0});", CanonicalTableName);
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.WriteLine("private System.Windows.Forms.DataGridView dataGridView1;");
      Writer.WriteLine("private NewDataSet newDataSet;");
      Writer.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", CanonicalTableName);
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.WriteLine("this.bindingNavigator1.BindingSource = this.{0}BindingSource;", CanonicalTableName);
      Writer.WriteLine("// ");
      Writer.WriteLine("// newDataSet");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.newDataSet.DataSetName = \"NewDataSet\";");
      Writer.WriteLine("this.newDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;");

      Writer.WriteLine("// ");
      Writer.WriteLine("// tableBindingSource");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.{0}BindingSource.DataMember = \"{0}\";", CanonicalTableName);
      Writer.WriteLine("this.{0}BindingSource.DataSource = this.newDataSet;", CanonicalTableName);

      Writer.WriteLine("this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;");
      Writer.WriteLine("this.dataGridView1.Location = new System.Drawing.Point(9, 37);");
      Writer.WriteLine("this.dataGridView1.Name = \"dataGridView1\"; ");
      Writer.WriteLine("this.dataGridView1.DataSource = this.{0}BindingSource;", CanonicalTableName);
      Writer.WriteLine("this.dataGridView1.Size = new System.Drawing.Size(339, 261);");
      Writer.WriteLine("this.dataGridView1.TabIndex = 0;");
      if (ValidationsEnabled)
      {
        Writer.WriteLine("this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);");
      }
      Writer.WriteLine("this.Controls.Add(this.dataGridView1);");
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("this.dataGridView1 = new System.Windows.Forms.DataGridView();");
      Writer.WriteLine("this.newDataSet = new NewDataSet();");
      Writer.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", CanonicalTableName);
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.newDataSet)).BeginInit();");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", CanonicalTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();");
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.newDataSet)).EndInit();");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", CanonicalTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();");
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      // Nothing
    }
  }
}
