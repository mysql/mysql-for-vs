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
  internal class VBEntityFrameworkIndividualControlsStrategy : VBIndividualControlsStrategy
  {
    internal VBEntityFrameworkIndividualControlsStrategy(StrategyConfig config)
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
      Writer.WriteLine("ctx = new Model1Entities()");
      Writer.WriteLine("Dim _entities As ObjectResult(Of {0}) = ctx.{0}.Execute(MergeOption.AppendOnly)", CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataSource = _entities", CanonicalTableName);

      for (int i = 0; i < ValidationColumns.Count; i++)
      {
        ColumnValidation cv = ValidationColumns[i];
        string colName = cv.Name;
        string idColumnCanonical = GetCanonicalIdentifier(colName);

        if (cv.HasLookup)
        {
          string canonicalReferencedTableName = GetCanonicalIdentifier(cv.FkInfo.ReferencedTableName);
          Writer.WriteLine("Me.{0}_comboBox.DataSource = ctx.{1}.ToList()", idColumnCanonical, canonicalReferencedTableName);
          Writer.WriteLine("Me.{0}_comboBox.DisplayMember = \"{1}\"", idColumnCanonical, cv.LookupColumn);
          Writer.WriteLine("Me.{0}_comboBox.ValueMember = \"{1}\"", idColumnCanonical, cv.FkInfo.ReferencedColumnName);
          Writer.WriteLine("Me.{0}_comboBox.DataBindings.Add(New System.Windows.Forms.Binding(\"SelectedValue\", Me.{1}BindingSource, \"{2}\", True))",
            idColumnCanonical, CanonicalTableName, idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("Me.{0}_dateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
            idColumnCanonical, colName, CanonicalTableName);
        }
        else if (cv.IsBooleanType())
        {
          Writer.WriteLine("Me.{0}CheckBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Checked\", Me.{2}BindingSource, \"{1}\", True))", idColumnCanonical, colName, CanonicalTableName);
        }
        else
        {
          Writer.WriteLine("Me.{0}TextBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
            idColumnCanonical, colName, CanonicalTableName);
        }
      }
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("Private ctx As Model1Entities");
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit()", CanonicalTableName);
      Writer.WriteLine("ctx.SaveChanges()");
    }

    protected override void WriteDesignerControlDeclCode()
    {
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
      Writer.WriteLine("Friend WithEvents panel3 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel4 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel5 As System.Windows.Forms.Panel");
      Writer.WriteLine("Friend WithEvents panel6 As System.Windows.Forms.Panel");
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.WriteLine("Me.bindingNavigator1.BindingSource = Me.{0}BindingSource", CanonicalTableName);
      WriteControlInitialization(false);

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
      Writer.WriteLine("Me.panel3.Controls.Add(Me.panel6)");
      Writer.WriteLine("Me.panel3.Dock = System.Windows.Forms.DockStyle.Fill");
      Writer.WriteLine("Me.panel3.Location = new System.Drawing.Point(0, 25)");
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
      // Panel6
      Writer.WriteLine("' ");
      Writer.WriteLine("' panel6");
      Writer.WriteLine("' ");
      Writer.WriteLine("Me.panel6.Dock = System.Windows.Forms.DockStyle.Bottom");
      Writer.WriteLine("Me.panel6.Location = New System.Drawing.Point(0, 324)");
      Writer.WriteLine("Me.panel6.Name = \"panel6\"");
      Writer.WriteLine("Me.panel6.Size = New System.Drawing.Size(400, 10)");
      Writer.WriteLine("Me.panel6.TabIndex = 6");

    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);
      Writer.WriteLine("Me.panel3 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.panel4 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.panel5 = New System.Windows.Forms.Panel()");
      Writer.WriteLine("Me.panel6 = New System.Windows.Forms.Panel()");
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("Me.panel3.SuspendLayout()");
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("Me.Text = \"{0}\"", CapitalizeString(TableName));
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
      Writer.WriteLine("Me.Controls.Add(Me.panel3)");
      Writer.WriteLine("Me.panel3.ResumeLayout(False)");
      Writer.WriteLine("Me.panel3.PerformLayout()");
    }
  }
}
