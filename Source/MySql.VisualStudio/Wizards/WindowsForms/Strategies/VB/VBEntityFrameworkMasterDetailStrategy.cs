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
  internal class VBEntityFrameworkMasterDetailStrategy : VBMasterDetailStrategy
  {
    internal VBEntityFrameworkMasterDetailStrategy(StrategyConfig config)
      : base(config)
    { 
    }

    protected override void WriteUsingUserCode()
    {
      if (DataAccessTech == DataAccessTechnology.EntityFramework5)
      {
        Writer.WriteLine("Imports System.Data.Objects");
      }
      else if (DataAccessTech == DataAccessTechnology.EntityFramework6)
      {
        Writer.WriteLine("Imports System.Data.Entity.Core.Objects");
      }
    }

    protected override void WriteFormLoadCode()
    {
      Writer.WriteLine("ctx = New Model1Entities()");
      Writer.WriteLine("Dim _entities As ObjectResult(Of {0}) = ctx.{0}.Execute(MergeOption.AppendOnly)", CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataSource = _entities", CanonicalTableName);

      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string colName = kvp.Key;
        string idColumnCanonical = GetCanonicalIdentifier(colName);
        if (kvp.Value.IsDateType())
        {
          Writer.WriteLine("Me.{0}_dateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
              idColumnCanonical, colName, CanonicalTableName);
        }
        else if (kvp.Value.IsBooleanType())
        {
          Writer.WriteLine("Me.{0}CheckBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Checked\", Me.{2}BindingSource, \"{1}\", True))", idColumnCanonical, colName, CanonicalTableName);
        }
        else
        {
          Writer.WriteLine("Me.{0}TextBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
            idColumnCanonical, colName, CanonicalTableName);
        }
      }

      Writer.WriteLine("{0}BindingSource.DataSource = {1}BindingSource", CanonicalDetailTableName, CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataMember = \"{0}\"", CanonicalDetailTableName );
      WriteDataGridColumnInitialization();
      Writer.WriteLine("dataGridView1.DataSource = {0}BindingSource", CanonicalDetailTableName);
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("Private ctx As Model1Entities");
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalDetailTableName);
      Writer.WriteLine("ctx.SaveChanges()");
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.WriteLine("Friend WithEvents {0}BindingSource As System.Windows.Forms.BindingSource", CanonicalTableName);
      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string idColumnCanonical = GetCanonicalIdentifier(kvp.Key);
        if (kvp.Value.IsDateType())
        {
          Writer.WriteLine("Friend WithEvents {0}_dateTimePicker As System.Windows.Forms.DateTimePicker", idColumnCanonical);
        }
        else if (kvp.Value.IsBooleanType())
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
      Writer.WriteLine("Friend WithEvents lblDetails As System.Windows.Forms.Label");
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.WriteLine("Me.bindingNavigator1.BindingSource = Me.{0}BindingSource", CanonicalTableName);
      WriteControlInitialization(false);
      // Panel2
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel2");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel2.Controls.Add(Me.dataGridView1)");
      Writer.WriteLine("Me.panel2.Controls.Add(Me.lblDetails)");
      Writer.WriteLine("Me.panel2.Dock = System.Windows.Forms.DockStyle.Bottom");
      Writer.WriteLine("Me.panel2.Location = New System.Drawing.Point(0, 283)");
      Writer.WriteLine("Me.panel2.Name = \"panel2\"");
      Writer.WriteLine("Me.panel2.Size = New System.Drawing.Size(666, 268)");
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
      if (ValidationsEnabled)
      {
        Writer.WriteLine("AddHandler Me.dataGridView1.CellValidating, AddressOf Me.dataGridView1_CellValidating");
      }
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("Me.dataGridView1 = New System.Windows.Forms.DataGridView()");
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalDetailTableName);
      Writer.WriteLine("Me.panel2 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.lblDetails = New System.Windows.Forms.Label()");
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalDetailTableName);
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("Me.Size = New System.Drawing.Size(682, 590)");
      Writer.WriteLine("CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalDetailTableName);
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
      Writer.WriteLine("Me.Controls.Add(Me.panel2)");
      Writer.WriteLine("Me.panel2.ResumeLayout(False)");
      Writer.WriteLine("Me.panel2.PerformLayout()");
    }
  }
}
