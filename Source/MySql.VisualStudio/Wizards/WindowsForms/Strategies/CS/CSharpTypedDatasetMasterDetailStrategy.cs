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
  internal class CSharpTypedDatasetMasterDetailStrategy : CSharpMasterDetailStrategy
  {
    internal CSharpTypedDatasetMasterDetailStrategy(StrategyConfig config)
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

      Writer.WriteLine("ad{0} = new MySqlDataAdapter(\"select * from `{1}`\", strConn);", CanonicalDetailTableName, DetailTableName );
      Writer.WriteLine("builder = new MySqlCommandBuilder(ad{0});", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.Fill(this.newDataSet.{0});", CanonicalDetailTableName );
      Writer.WriteLine("ad{0}.DeleteCommand = builder.GetDeleteCommand();", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.UpdateCommand = builder.GetUpdateCommand();", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.InsertCommand = builder.GetInsertCommand();", CanonicalDetailTableName);
      
      RetrieveFkColumns();
      StringBuilder sbSrcCols = new StringBuilder("new DataColumn[] { ");
      StringBuilder sbDstCols = new StringBuilder("new DataColumn[] { ");
      for (int i = 0; i < FkColumnsSource.Count; i++)
      {
        // Both FkColumnsSource & FkColumnsDest have the item count
        sbSrcCols.AppendFormat(" newDataSet.{0}.Columns[ \"{1}\" ] ", CanonicalTableName,
          FkColumnsSource[i]);
        sbDstCols.AppendFormat(" newDataSet.{0}.Columns[ \"{1}\" ] ", CanonicalDetailTableName,
          FkColumnsDest[i]);
      }
      sbSrcCols.Append("}");
      sbDstCols.Append("}");

      Writer.WriteLine("newDataSet.Relations.Add( new DataRelation( \"{0}\", {1}, {2} ) );", 
        ConstraintName, sbSrcCols.ToString(), sbDstCols.ToString() );

      Writer.WriteLine("{0}BindingSource.DataSource = {1}BindingSource;", CanonicalDetailTableName, CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataMember = \"{1}\";", CanonicalDetailTableName, ConstraintName);
      WriteDataGridColumnInitialization();
      Writer.WriteLine("dataGridView1.DataSource = {0}BindingSource;", CanonicalDetailTableName);
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("private MySqlDataAdapter ad;");
      Writer.WriteLine("private MySqlDataAdapter ad{0};", CanonicalDetailTableName );
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit();", CanonicalTableName );
      Writer.WriteLine("{0}BindingSource.EndEdit();", CanonicalDetailTableName );
      Writer.WriteLine("ad.Update(this.newDataSet.{0});", CanonicalTableName );
      Writer.WriteLine("ad{0}.Update(this.newDataSet.{0});", CanonicalDetailTableName);
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.WriteLine("private NewDataSet newDataSet;");
      Writer.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", CanonicalTableName);
      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string idColumnCanonical = GetCanonicalIdentifier(kvp.Key);
        if (kvp.Value.IsDateType())
        {
          Writer.WriteLine("private System.Windows.Forms.DateTimePicker {0}_dateTimePicker;", idColumnCanonical);
        }
        else if (kvp.Value.IsBooleanType())
        {
          Writer.WriteLine("private System.Windows.Forms.CheckBox {0}CheckBox;", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("private System.Windows.Forms.TextBox {0}TextBox;", idColumnCanonical);
        }
        Writer.WriteLine("private System.Windows.Forms.Label {0}Label;", idColumnCanonical);
      }
      Writer.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", CanonicalDetailTableName);
      Writer.WriteLine("private System.Windows.Forms.DataGridView dataGridView1;");
      Writer.WriteLine("private System.Windows.Forms.Panel panel2;");
      Writer.WriteLine("private System.Windows.Forms.Label lblDetails;");
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

      WriteControlInitialization(true);
      // Panel2
      Writer.WriteLine("// ");
      Writer.WriteLine("// panel2");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.panel2.Controls.Add(this.dataGridView1);");
      Writer.WriteLine("this.panel2.Controls.Add(this.lblDetails);");
      Writer.WriteLine("this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;");
      Writer.WriteLine("this.panel2.Location = new System.Drawing.Point(0, 283);");
      Writer.WriteLine("this.panel2.Name = \"panel2\";");
      Writer.WriteLine("this.panel2.Size = new System.Drawing.Size(666, 268);");
      Writer.WriteLine("this.panel2.TabIndex = 4;");
      // Label2
      Writer.WriteLine("// ");
      Writer.WriteLine("// lblDetails");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.lblDetails.AutoSize = true;");
      Writer.WriteLine("this.lblDetails.Location = new System.Drawing.Point(9, 10);");
      Writer.WriteLine("this.lblDetails.Dock = System.Windows.Forms.DockStyle.Top;");
      Writer.WriteLine("this.lblDetails.Name = \"label2\";");
      Writer.WriteLine("this.lblDetails.Size = new System.Drawing.Size(129, 13);");
      Writer.WriteLine("this.lblDetails.TabIndex = 4;");
      Writer.WriteLine("this.lblDetails.Text = \"Details Records: {0}\";", DetailTableName);
      // DataGrid
      Writer.WriteLine("// ");
      Writer.WriteLine("//dataGridView1");
      Writer.WriteLine("// ");
      Writer.WriteLine("this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;");
      Writer.WriteLine("this.dataGridView1.Location = new System.Drawing.Point(0, 35);");
      Writer.WriteLine("this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;");
      Writer.WriteLine("this.dataGridView1.Name = \"dataGridView1\"; ");
      Writer.WriteLine("this.dataGridView1.Size = new System.Drawing.Size(666, 233);");
      Writer.WriteLine("this.dataGridView1.TabIndex = 0;");
      if (ValidationsEnabled)
      {
        Writer.WriteLine("this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);");
      }
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("this.newDataSet = new NewDataSet();");
      Writer.WriteLine("this.dataGridView1 = new System.Windows.Forms.DataGridView();");
      Writer.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", CanonicalTableName);
      Writer.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", CanonicalDetailTableName);
      Writer.WriteLine("this.panel2 = new System.Windows.Forms.Panel();");
      Writer.WriteLine("this.lblDetails = new System.Windows.Forms.Label();");
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", CanonicalTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", CanonicalDetailTableName);
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("this.Size = new System.Drawing.Size(682, 590);");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();");
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", CanonicalDetailTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", CanonicalTableName);
      Writer.WriteLine("this.Controls.Add(this.panel2);");
      Writer.WriteLine("this.panel2.ResumeLayout(false);");
      Writer.WriteLine("this.panel2.PerformLayout();");
    }
  }
}
