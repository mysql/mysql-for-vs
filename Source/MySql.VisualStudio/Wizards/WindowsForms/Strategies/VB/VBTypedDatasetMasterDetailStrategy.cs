// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
  internal class VBTypedDatasetMasterDetailStrategy : VBMasterDetailStrategy
  {
    internal VBTypedDatasetMasterDetailStrategy(StrategyConfig config)
      : base(config)
    { 
    }

    protected override void WriteUsingUserCode()
    {
      Writer.WriteLine("Imports MySql.Data.MySqlClient");
    }

    protected override void WriteFormLoadCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Dim strConn As string = \"{0};\"", ConnectionString);
      Writer.WriteLine("ad = new MySqlDataAdapter(\"select * from `{0}`\", strConn)", TableName);
      Writer.WriteLine("Dim builder As MySqlCommandBuilder = New MySqlCommandBuilder(ad)");
      Writer.WriteLine("ad.Fill(Me.newDataSet.{0})", CanonicalTableName );
      Writer.WriteLine("ad.DeleteCommand = builder.GetDeleteCommand()");
      Writer.WriteLine("ad.UpdateCommand = builder.GetUpdateCommand()");
      Writer.WriteLine("ad.InsertCommand = builder.GetInsertCommand()");

      Writer.WriteLine("ad{0} = New MySqlDataAdapter(\"select * from `{1}`\", strConn)", CanonicalDetailTableName, DetailTableName );
      Writer.WriteLine("builder = New MySqlCommandBuilder(ad{0})", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.Fill(Me.newDataSet.{0})", CanonicalDetailTableName );
      Writer.WriteLine("ad{0}.DeleteCommand = builder.GetDeleteCommand()", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.UpdateCommand = builder.GetUpdateCommand()", CanonicalDetailTableName);
      Writer.WriteLine("ad{0}.InsertCommand = builder.GetInsertCommand()", CanonicalDetailTableName);
      
      RetrieveFkColumns();
      StringBuilder sbSrcCols = new StringBuilder("new DataColumn() { ");
      StringBuilder sbDstCols = new StringBuilder("new DataColumn() { ");
      for (int i = 0; i < FkColumnsSource.Count; i++)
      {
        // Both FkColumnsSource & FkColumnsDest have the item count
        sbSrcCols.AppendFormat(" newDataSet.{0}.Columns( \"{1}\" )", CanonicalTableName,
          FkColumnsSource[i]);
        sbDstCols.AppendFormat(" newDataSet.{0}.Columns( \"{1}\" ) ", CanonicalDetailTableName,
          FkColumnsDest[i]);
      }
      sbSrcCols.Append("}");
      sbDstCols.Append("}");
        
      Writer.WriteLine("newDataSet.Relations.Add( New DataRelation( \"{0}\", {1}, {2} ) )", 
        ConstraintName, sbSrcCols.ToString(), sbDstCols.ToString() );

      Writer.WriteLine("Dim ad3 As MySqlDataAdapter");

      for (int i = 0; i < ValidationColumns.Count; i++)
      {
        ColumnValidation cv = ValidationColumns[i];
        if (cv.HasLookup)
        {
          string colName = cv.Name;
          string idColumnCanonical = GetCanonicalIdentifier(colName);
          string canonicalReferencedTable = GetCanonicalIdentifier(cv.FkInfo.ReferencedTableName);

          Writer.WriteLine("ad3 = New MySqlDataAdapter(\"select * from `{0}`\", strConn)", cv.FkInfo.ReferencedTableName);
          Writer.WriteLine("ad3.Fill(Me.newDataSet.{0})", canonicalReferencedTable);
          Writer.WriteLine("Me.{0}_comboBox.DataSource = Me.newDataSet.{1}", idColumnCanonical, canonicalReferencedTable);
          Writer.WriteLine("Me.{0}_comboBox.DisplayMember = \"{1}\"", idColumnCanonical, cv.EfLookupColumnMapping);
          Writer.WriteLine("Me.{0}_comboBox.ValueMember = \"{1}\"", idColumnCanonical, cv.FkInfo.ReferencedColumnName);
          Writer.WriteLine("Me.{0}_comboBox.DataBindings.Add(New System.Windows.Forms.Binding(\"SelectedValue\", Me.{1}BindingSource, \"{2}\", True))",
            idColumnCanonical, CanonicalTableName, cv.EfColumnMapping);
          Writer.WriteLine("ad3.Dispose()");
        }
      }

      Writer.WriteLine("{0}BindingSource.DataSource = {1}BindingSource", CanonicalDetailTableName, CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataMember = \"{1}\"", CanonicalDetailTableName, ConstraintName);
      WriteDataGridColumnInitialization();
      Writer.WriteLine("dataGridView1.DataSource = {0}BindingSource", CanonicalDetailTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Private ad As MySqlDataAdapter");
      Writer.WriteLine("Private ad{0} As MySqlDataAdapter", CanonicalDetailTableName );

      Writer.PopIdentationLevel();
    }

    protected override void WriteSaveEventCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      for (int i = 0; i < ValidationColumns.Count; i++)
      {
        ColumnValidation cv = ValidationColumns[i];
        string colName = GetCanonicalIdentifier(cv.Name);
        if (cv.DataType == "timestamp")
        {
          Writer.WriteLine("If TypeOf( CType({0}BindingSource.Current, DataRowView )( \"{1}\" ) ) Is DBNull Then ", CanonicalTableName, colName);
          Writer.WriteLine(" CType({0}BindingSource.Current, DataRowView )(\"{1}\") = DateTime.Now", CanonicalTableName, colName);
          Writer.WriteLine("End If");
        }
      }
      
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalTableName );
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalDetailTableName );
      Writer.WriteLine("ad.Update(Me.newDataSet.{0})", CanonicalTableName );
      Writer.WriteLine("ad{0}.Update(Me.newDataSet.{0})", CanonicalDetailTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Friend WithEvents newDataSet As NewDataSet ");
      Writer.WriteLine("Friend WithEvents {0}BindingSource As System.Windows.Forms.BindingSource", CanonicalTableName);
      for (int i = 0; i < ValidationColumns.Count; i++)
      {
        ColumnValidation cv = ValidationColumns[i];
        string idColumnCanonical = GetCanonicalIdentifier(cv.Name);
        if (cv.HasLookup)
        {
          Writer.WriteLine("Friend WithEvents {0}_comboBox As System.Windows.Forms.ComboBox", idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("Friend WithEvents {0}_dateTimePicker As System.Windows.Forms.DateTimePicker", idColumnCanonical);
        }
        else if (cv.IsBooleanType())
        {
          Writer.WriteLine("Friend WithEvents {0}CheckBox As System.Windows.Forms.CheckBox", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("Friend WithEvents {0}TextBox As System.Windows.Forms.TextBox", idColumnCanonical);
        }
        Writer.WriteLine("Friend WithEvents {0}Label As System.Windows.Forms.Label", idColumnCanonical);
      }
      Writer.WriteLine("Friend WithEvents {0}BindingSource As System.Windows.Forms.BindingSource", CanonicalDetailTableName);
      Writer.WriteLine("Friend WithEvents dataGridView1 As System.Windows.Forms.DataGridView");
      Writer.WriteLine("Friend WithEvents panel2 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel3 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel4 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel5 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents lblDetails As System.Windows.Forms.Label");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.bindingNavigator1.BindingSource = Me.{0}BindingSource", CanonicalTableName);
      Writer.WriteLine("' ");
      Writer.WriteLine("'newDataSet");
      Writer.WriteLine("'");
      Writer.WriteLine("Me.newDataSet.DataSetName = \"NewDataSet\"");
      Writer.WriteLine("Me.newDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema");

      Writer.WriteLine("' ");
      Writer.WriteLine("'tableBindingSource");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.{0}BindingSource.DataMember = \"{0}\"", CanonicalTableName);
      Writer.WriteLine("Me.{0}BindingSource.DataSource = Me.newDataSet", CanonicalTableName);

      WriteControlInitialization(true);
      // Panel2
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel2");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel2.Controls.Add(Me.dataGridView1)");
      Writer.WriteLine("Me.panel2.Controls.Add(Me.lblDetails)");
      Writer.WriteLine("Me.panel2.Dock = System.Windows.Forms.DockStyle.Bottom");
      Writer.WriteLine("Me.panel2.Location = New System.Drawing.Point(0, 208)");
      Writer.WriteLine("Me.panel2.Name = \"panel2\"");
      Writer.WriteLine("Me.panel2.Padding = New System.Windows.Forms.Padding(10)");
      Writer.WriteLine("Me.panel2.Size = New System.Drawing.Size(666, 184)");
      Writer.WriteLine("Me.panel2.TabIndex = 4");
      // Label2
      Writer.WriteLine("' ");
      Writer.WriteLine("' lblDetails");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.lblDetails.AutoSize = True");
      Writer.WriteLine("Me.lblDetails.Location = New System.Drawing.Point(9, 10)");
      Writer.WriteLine("Me.lblDetails.Dock = System.Windows.Forms.DockStyle.Top");
      Writer.WriteLine("Me.lblDetails.Name = \"label2\"");
      Writer.WriteLine("Me.lblDetails.Size = New System.Drawing.Size(129, 13)");
      Writer.WriteLine("Me.lblDetails.TabIndex = 4");
      Writer.WriteLine("Me.lblDetails.Text = \"Details Records: {0}\"", DetailTableName);
      // DataGrid
      Writer.WriteLine("' ");
      Writer.WriteLine("'dataGridView1");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize");
      Writer.WriteLine("Me.dataGridView1.Location = New System.Drawing.Point(0, 35)");
      Writer.WriteLine("Me.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill");
      Writer.WriteLine("Me.dataGridView1.Name = \"dataGridView1\" ");
      Writer.WriteLine("Me.dataGridView1.Size = New System.Drawing.Size(666, 261)");
      Writer.WriteLine("Me.dataGridView1.TabIndex = 0");

      // Panel4
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel4");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel4.Dock = System.Windows.Forms.DockStyle.Right");
      Writer.WriteLine("Me.panel4.Location = New System.Drawing.Point(656, 0)");
      Writer.WriteLine("Me.panel4.Name = \"panel4\"");
      Writer.WriteLine("Me.panel4.Size = New System.Drawing.Size(10, 183)");
      Writer.WriteLine("Me.panel4.TabIndex = 3");
      // Panel3
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel3");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel3.Controls.Add(Me.Panel1)");
      Writer.WriteLine("Me.panel3.Controls.Add(Me.panel4)");
      Writer.WriteLine("Me.panel3.Controls.Add(Me.panel5)");
      Writer.WriteLine("Me.panel3.Dock = System.Windows.Forms.DockStyle.Fill");
      Writer.WriteLine("Me.panel3.Location = New System.Drawing.Point(0, 25)");
      Writer.WriteLine("Me.panel3.Name = \"panel3\"");
      Writer.WriteLine("Me.panel3.Size = New System.Drawing.Size(666, 183)");
      Writer.WriteLine("Me.panel3.TabIndex = 19");
      // Panel5
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel5");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel5.Dock = System.Windows.Forms.DockStyle.Left");
      Writer.WriteLine("Me.panel5.Location = New System.Drawing.Point(0, 0)");
      Writer.WriteLine("Me.panel5.Name = \"panel5\"");
      Writer.WriteLine("Me.panel5.Size = New System.Drawing.Size(10, 183)");
      Writer.WriteLine("Me.panel5.TabIndex = 5");

      if (ValidationsEnabled)
      {
		    Writer.WriteLine("AddHandler Me.dataGridView1.CellValidating, AddressOf Me.dataGridView1_CellValidating");
        Writer.WriteLine("AddHandler Me.dataGridView1.DataError, AddressOf Me.dataGridView1_DataError");
      }

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.newDataSet = New NewDataSet()");
      Writer.WriteLine("Me.dataGridView1 = New System.Windows.Forms.DataGridView()");
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalDetailTableName);
      Writer.WriteLine("Me.panel2 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.lblDetails = New System.Windows.Forms.Label()");
      Writer.WriteLine("Me.panel3 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.panel4 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.panel5 = New System.Windows.Forms.Panel()");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.panel3.SuspendLayout()");
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalDetailTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.Size = New System.Drawing.Size(682, 590)");
      Writer.WriteLine("Me.Text = \"{0}\"", CapitalizeString(TableName));
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalDetailTableName);
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
      Writer.WriteLine("Me.Controls.Add(Me.panel3)");
      Writer.WriteLine("Me.Controls.Add(Me.panel2)");
      Writer.WriteLine("Me.panel2.ResumeLayout(False)");
      Writer.WriteLine("Me.panel2.PerformLayout()");
      Writer.WriteLine("Me.panel3.ResumeLayout(False)");
      Writer.WriteLine("Me.panel3.PerformLayout()");

      Writer.PopIdentationLevel();
    }

    internal override string GetDataSourceForCombo(ColumnValidation cv)
    {
      string colName = cv.Name;
      string idColumnCanonical = GetCanonicalIdentifier(colName);
      string canonicalReferencedTableName = GetCanonicalIdentifier(cv.FkInfo.ReferencedTableName);
      Writer.WriteLine("If Not ( ad2 Is Nothing ) Then");
      Writer.WriteLine("ad2.Dispose()");
      Writer.WriteLine("End If");
      Writer.WriteLine("ad2 = New MySql.Data.MySqlClient.MySqlDataAdapter(\"select * from `{0}`\", strConn2)", cv.FkInfo.ReferencedTableName);
      Writer.WriteLine("ad2.Fill(Me.newDataSet.{0})", canonicalReferencedTableName);
      return string.Format("Me.newDataSet.{0}", canonicalReferencedTableName);
    }
  }
}
