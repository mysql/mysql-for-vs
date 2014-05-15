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

      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string colName = kvp.Key;
        string idColumnCanonical = GetCanonicalIdentifier(colName);
        Writer.WriteLine("Me.{0}TextBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
          idColumnCanonical, colName, CanonicalTableName);
      }
    }

    protected override void WriteValidationCode()
    {
      bool validationsEnabled = ValidationsEnabled;
      List<ColumnValidation> validationColumns = ValidationColumns;
      if (validationsEnabled)
      {
        for (int i = 0; i < validationColumns.Count; i++)
        {
          ColumnValidation cv = validationColumns[i];
          string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
          Writer.WriteLine("Private Sub {0}TextBox_Validating(sender As Object, e As CancelEventArgs)", idColumnCanonical);
          Writer.WriteLine("");
          Writer.WriteLine("  e.Cancel = False");
          if (cv.Required)
          {
            Writer.WriteLine("  If String.IsNullOrEmpty( {0}TextBox.Text ) Then ", idColumnCanonical);
            Writer.WriteLine("    e.Cancel = True");
            Writer.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ) ", idColumnCanonical, cv.Name);
            Writer.WriteLine("  End If");
          }
          if (cv.IsNumericType())
          {
            Writer.WriteLine("  Dim v As Integer");
            Writer.WriteLine("  Dim s As String = {0}TextBox.Text", idColumnCanonical);
            Writer.WriteLine("  If Not Integer.TryParse( s, v ) Then");
            Writer.WriteLine("    e.Cancel = True");
            Writer.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} must be numeric.\" )", idColumnCanonical, cv.Name);
            if (cv.MinValue != null)
            {
              Writer.WriteLine(" ElseIf cv.MinValue > v Then ");
              Writer.WriteLine("   e.Cancel = True");
              Writer.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" )", idColumnCanonical, cv.Name, cv.MinValue);
            }
            if (cv.MaxValue != null)
            {
              Writer.WriteLine(" ElseIf cv.MaxValue < v Then");
              Writer.WriteLine("   e.Cancel = True");
              Writer.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" )", idColumnCanonical, cv.Name, cv.MaxValue);
            }
            Writer.WriteLine( "End If" );
          }
          Writer.WriteLine("  If Not e.Cancel Then ");
		      Writer.WriteLine("    errorProvider1.SetError( {0}TextBox, \"\" )", idColumnCanonical);
          Writer.WriteLine("  End If" );
          Writer.WriteLine("End Sub");
          Writer.WriteLine();
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
      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string idColumnCanonical = GetCanonicalIdentifier(kvp.Key);
        Writer.WriteLine("Friend WithEvents {0}TextBox As System.Windows.Forms.TextBox", idColumnCanonical);
        Writer.WriteLine("Friend WithEvents {0}Label As System.Windows.Forms.Label", idColumnCanonical);
      }
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.WriteLine("Me.bindingNavigator1.BindingSource = Me.{0}BindingSource", CanonicalTableName);
      WriteControlInitialization(false);
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("Me.{0}BindingSource = New System.Windows.Forms.BindingSource(Me.components)", CanonicalTableName);
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).BeginInit()", CanonicalTableName);
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("CType(Me.{0}BindingSource, System.ComponentModel.ISupportInitialize).EndInit()", CanonicalTableName);
    }
  }
}
