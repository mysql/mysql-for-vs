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
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("Private ad As MySqlDataAdapter");
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalTableName);
      Writer.WriteLine("ad.Update(Me.newDataSet.{0})", CanonicalTableName);
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.WriteLine("Friend WithEvents dataGridView1 As System.Windows.Forms.DataGridView");
      Writer.WriteLine("Friend WithEvents newDataSet As NewDataSet");
      Writer.WriteLine("Friend WithEvents {0}BindingSource As System.Windows.Forms.BindingSource", CanonicalTableName);
    }

    protected override void WriteDesignerControlInitCode()
    {
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
      Writer.WriteLine("Me.dataGridView1.Location = New System.Drawing.Point(9, 37)");
      Writer.WriteLine("Me.dataGridView1.Name = \"dataGridView1\" ");
      WriteDataGridColumnInitialization();
      Writer.WriteLine("Me.dataGridView1.DataSource = Me.{0}BindingSource", CanonicalTableName);
      Writer.WriteLine("Me.dataGridView1.Size = New System.Drawing.Size(339, 261)");
      Writer.WriteLine("Me.dataGridView1.TabIndex = 0");
      if (ValidationsEnabled)
      {
        Writer.WriteLine("AddHandler Me.dataGridView1.CellValidating, AddressOf Me.dataGridView1_CellValidating");
      }
      Writer.WriteLine("Me.Panel1.Controls.Add(Me.dataGridView1)");
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("Me.dataGridView1 = New System.Windows.Forms.DataGridView()");
      Writer.WriteLine("Me.newDataSet = New NewDataSet()");
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("CType(Me.newDataSet, System.ComponentModel.ISupportInitialize).BeginInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()");
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("CType(Me.newDataSet, System.ComponentModel.ISupportInitialize).EndInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()");
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      // Nothing
    }
  }
}
