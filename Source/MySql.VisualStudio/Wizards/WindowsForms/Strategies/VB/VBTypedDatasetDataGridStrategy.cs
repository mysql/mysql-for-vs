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
  internal class VBTypedDatasetDataGridStrategy : VBDataGridStrategy
  {
    internal VBTypedDatasetDataGridStrategy(StrategyConfig config)
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

      Writer.WriteLine("Dim strConn As String = \"{0};\"", ConnectionString);
      Writer.WriteLine("ad = New MySqlDataAdapter(\"select * from `{0}`\", strConn)", TableName);
      Writer.WriteLine("Dim builder As MySqlCommandBuilder = New MySqlCommandBuilder(ad)");
      Writer.WriteLine("ad.Fill(Me.newDataSet.{0})", CanonicalTableName);
      Writer.WriteLine("ad.DeleteCommand = builder.GetDeleteCommand()");
      Writer.WriteLine("ad.UpdateCommand = builder.GetUpdateCommand()");
      Writer.WriteLine("ad.InsertCommand = builder.GetInsertCommand()");

      Writer.WriteLine("bindingNavigatorAddNewItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorCountItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorDeleteItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorMoveFirstItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorMovePreviousItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorPositionItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorMoveNextItem.Enabled = False");
      Writer.WriteLine("bindingNavigatorMoveLastItem.Enabled = False");
      Writer.WriteLine("toolStripButton1.Enabled = True");

      WriteDataGridColumnInitialization();
      Writer.WriteLine("Me.dataGridView1.DataSource = Me.{0}BindingSource", CanonicalTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Private ad As MySqlDataAdapter");

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
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalTableName);
      Writer.WriteLine("ad.Update(Me.newDataSet.{0})", CanonicalTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Friend WithEvents dataGridView1 As System.Windows.Forms.DataGridView");
      Writer.WriteLine("Friend WithEvents newDataSet As NewDataSet");
      Writer.WriteLine("Friend WithEvents {0}BindingSource As System.Windows.Forms.BindingSource", CanonicalTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.bindingNavigator1.BindingSource = Me.{0}BindingSource", CanonicalTableName);
      Writer.WriteLine("'");
      Writer.WriteLine("'newDataSet");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.newDataSet.DataSetName = \"NewDataSet\"");
      Writer.WriteLine("Me.newDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema");

      Writer.WriteLine("'");
      Writer.WriteLine("'tableBindingSource");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.{0}BindingSource.DataMember = \"{0}\"", CanonicalTableName);
      Writer.WriteLine("Me.{0}BindingSource.DataSource = Me.newDataSet", CanonicalTableName);

      Writer.WriteLine("Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize");
      Writer.WriteLine("Me.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill");
      Writer.WriteLine("Me.dataGridView1.Location = New System.Drawing.Point(9, 37)");
      Writer.WriteLine("Me.dataGridView1.Name = \"dataGridView1\" ");
      Writer.WriteLine("Me.dataGridView1.Size = New System.Drawing.Size(339, 261)");
      Writer.WriteLine("Me.dataGridView1.TabIndex = 0");
      if (ValidationsEnabled)
      {
        Writer.WriteLine("AddHandler Me.dataGridView1.CellValidating, AddressOf Me.dataGridView1_CellValidating");
        Writer.WriteLine("AddHandler Me.dataGridView1.DataError, AddressOf Me.dataGridView1_DataError");
      }
      Writer.WriteLine("Me.Panel1.Controls.Add(Me.dataGridView1)");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.dataGridView1 = New System.Windows.Forms.DataGridView()");
      Writer.WriteLine("Me.newDataSet = New NewDataSet()");
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);

      Writer.PopIdentationLevel();
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("CType(Me.newDataSet, System.ComponentModel.ISupportInitialize).BeginInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()");

      Writer.PopIdentationLevel();
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Me.Text = \"{0}\"", CapitalizeString(TableName));
      Writer.WriteLine("Me.Panel1.Padding = New System.Windows.Forms.Padding(10)");
      Writer.WriteLine("Me.Controls.Add(Me.Panel1)");
      Writer.WriteLine("CType(Me.newDataSet, System.ComponentModel.ISupportInitialize).EndInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()");

      Writer.PopIdentationLevel();
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      // Nothing
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
