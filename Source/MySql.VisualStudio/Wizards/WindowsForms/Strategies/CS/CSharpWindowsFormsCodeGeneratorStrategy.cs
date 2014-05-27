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
  internal class CSharpWindowsFormsCodeGeneratorStrategy : WindowsFormsCodeGeneratorStrategy
  {
    internal CSharpWindowsFormsCodeGeneratorStrategy(
      StrategyConfig config)
      : base(config)
    {
      ActionMappings = new Dictionary<string, WriterDelegate>();

      ActionMappings["// <WizardGeneratedCode>Namespace_UserCode</WizardGeneratedCode>"] = WriteUsingUserCode;
      ActionMappings["// <WizardGeneratedCode>Form_Load</WizardGeneratedCode>"] = WriteFormLoadCode;
      ActionMappings["// <WizardGeneratedCode>Validation Events</WizardGeneratedCode>"] = WriteValidationCode;
      ActionMappings["// <WizardGeneratedCode>Private Variables Frontend</WizardGeneratedCode>"] = WriteVariablesUserCode;
      ActionMappings["// <WizardGeneratedCode>Save Event</WizardGeneratedCode>"] = WriteSaveEventCode;
      ActionMappings["// <WizardGeneratedCode>Add Event</WizardGeneratedCode>"] = WriteAddEventCode;
      ActionMappings["// <WizardGeneratedCode>Designer Control Declaration</WizardGeneratedCode>"] = WriteDesignerControlDeclCode;
      ActionMappings["// <WizardGeneratedCode>Designer Control Initialization</WizardGeneratedCode>"] = WriteDesignerControlInitCode;
      ActionMappings["// <WizardGeneratedCode>Designer BeforeSuspendLayout</WizardGeneratedCode>"] = WriteDesignerBeforeSuspendCode;
      ActionMappings["// <WizardGeneratedCode>Designer AfterSuspendLayout</WizardGeneratedCode>"] = WriteDesignerAfterSuspendCode;
      ActionMappings["// <WizardGeneratedCode>Designer BeforeResumeSuspendLayout</WizardGeneratedCode>"] = WriteBeforeResumeSuspendCode;
    }

    /// <summary>
    /// Transforms a user identifier like MySql table to ensure it is a valid identifier in C#/VB.NET.
    /// </summary>
    /// <returns></returns>
    internal protected override string GetCanonicalIdentifier(string Identifier)
    {
      return BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.GetCanonicalIdentifier(Identifier);    }

    internal protected override string GetEdmDesignerFileName()
    {
      return "{0}.Designer.cs";
    }

    internal protected override string GetFormDesignerFileName()
    {
      return "Form1.Designer.cs";
    }

    internal protected override string GetFormFileName()
    {
      return "Form1.cs";
    }

    protected override void WriteUsingUserCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteFormLoadCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteVariablesUserCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteSaveEventCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteAddEventCode()
    {
      Writer.WriteLine("{0}BindingSource.AddNew();", CanonicalTableName );
    }

    protected override void WriteDesignerControlDeclCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteDesignerControlInitCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      throw new NotImplementedException();
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      Label l = new Label();
      Size szText = TextRenderer.MeasureText(GetMaxWidthString(Columns), l.Font);
      Point initLoc = new Point(szText.Width + 10, 50);
      Point xy = new Point(initLoc.X, initLoc.Y);
      int tabIdx = 1;
      bool validationsEnabled = ValidationsEnabled;

      foreach (KeyValuePair<string, Column> kvp in Columns)
      {
        string colName = kvp.Key;
        string idColumnCanonical = GetCanonicalIdentifier(colName);
        Writer.WriteLine("//");
        Writer.WriteLine("// {0}Label", idColumnCanonical);
        Writer.WriteLine("//");
        Writer.WriteLine("this.{0}Label = new System.Windows.Forms.Label();", idColumnCanonical);

        Writer.WriteLine("this.{0}Label.AutoSize = true;", idColumnCanonical);
        Size szLabel = TextRenderer.MeasureText(colName, l.Font);
        Writer.WriteLine("this.{0}Label.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical,
          xy.X - 10 - szLabel.Width, xy.Y);
        Writer.WriteLine("this.{0}Label.Name = \"{0}Label\";", idColumnCanonical );
        Writer.WriteLine("this.{0}Label.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical,
          szLabel.Width, szLabel.Height);
        Writer.WriteLine("this.{0}Label.TabIndex = {1};", idColumnCanonical, tabIdx++);
        Writer.WriteLine("this.{0}Label.Text = \"{1}\";", idColumnCanonical, colName);
        Writer.WriteLine("this.Panel1.Controls.Add( this.{0}Label );", idColumnCanonical);

        if (kvp.Value.IsDateType())
        {
          Writer.WriteLine("//");
          Writer.WriteLine("// {0}_dateTimePicker", idColumnCanonical);
          Writer.WriteLine("//");
          Writer.WriteLine("this.{0}_dateTimePicker = new System.Windows.Forms.DateTimePicker();", idColumnCanonical);
          if (kvp.Value.IsDateTimeType())
          {
            Writer.WriteLine("this.{0}_dateTimePicker.CustomFormat = \"dd/MM/yyyy, hh:mm\"; ", idColumnCanonical);
          }
          else
          {
            Writer.WriteLine("this.{0}_dateTimePicker.CustomFormat = \"dd/MM/yyyy\"; ", idColumnCanonical);
          }
          Writer.WriteLine("this.{0}_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;", idColumnCanonical);
          Writer.WriteLine("this.{0}_dateTimePicker.Location = new System.Drawing.Point({1}, {2});", idColumnCanonical, xy.X, xy.Y);
          Writer.WriteLine("this.{0}_dateTimePicker.Name = \"{0}_dateTimePicker\";", idColumnCanonical);
          Writer.WriteLine("this.{0}_dateTimePicker.Size = new System.Drawing.Size(200, 20);", idColumnCanonical);
          Writer.WriteLine("this.{0}_dateTimePicker.TabIndex = {1};", idColumnCanonical, tabIdx++);
          Writer.WriteLine("this.{0}_dateTimePicker.Value = new System.DateTime(2014, 5, 26, 17, 35, 11, 0);", idColumnCanonical);
          if (addBindings)
          {
            Writer.WriteLine("this.{0}_dateTimePicker.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", this.{2}BindingSource, \"{1}\", true ));",
              idColumnCanonical, colName, CanonicalTableName);
          }
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}_dateTimePicker );", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("//");
          Writer.WriteLine("// {0}TextBox", idColumnCanonical);
          Writer.WriteLine("//");
          Writer.WriteLine("this.{0}TextBox = new System.Windows.Forms.TextBox();", idColumnCanonical);

          if (addBindings)
          {
            Writer.WriteLine("this.{0}TextBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", this.{2}BindingSource, \"{1}\", true ));",
              idColumnCanonical, colName, CanonicalTableName);
          }

          Writer.WriteLine("this.{0}TextBox.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical, xy.X, xy.Y);
          Writer.WriteLine("this.{0}TextBox.Name = \"{0}TextBox\";", idColumnCanonical);
          Writer.WriteLine("this.{0}TextBox.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical, 100, 20);
          Writer.WriteLine("this.{0}TextBox.TabIndex = {1};", idColumnCanonical, tabIdx++);

          if (kvp.Value.IsReadOnly())
          {
            Writer.WriteLine("this.{0}TextBox.Enabled = false;", idColumnCanonical );
          } 
          else if(validationsEnabled)
          {
            Writer.WriteLine("this.{0}TextBox.Validating += new System.ComponentModel.CancelEventHandler( this.{0}TextBox_Validating );",
              idColumnCanonical);
          }
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}TextBox);", idColumnCanonical);
        }
        xy.Y += szText.Height * 2;
      }
    }

    protected string GetMaxWidthString(Dictionary<string, Column> l)
    {
      KeyValuePair<string, Column> maxWidthItem = new KeyValuePair<string, Column>("", null);
      foreach (KeyValuePair<string, Column> kvp in l)
      {
        if (kvp.Key.Length > maxWidthItem.Key.Length) maxWidthItem = kvp;
      }
      return maxWidthItem.Key;
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
          if (cv.IsDateType() || cv.IsReadOnly()) continue;

          string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
          Writer.WriteLine("private void {0}TextBox_Validating(object sender, CancelEventArgs e)", idColumnCanonical);
          Writer.WriteLine("{");
          Writer.WriteLine("  e.Cancel = false;");
          if (cv.Required)
          {
            Writer.WriteLine("  if( string.IsNullOrEmpty( {0}TextBox.Text ) ) {{ ", idColumnCanonical);
            Writer.WriteLine("    e.Cancel = true;");
            Writer.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ); ", idColumnCanonical, cv.Name);
            Writer.WriteLine("  }");
          }
          if (cv.IsNumericType())
          {
            string numericType = "";
            if (cv.IsIntegerType())
            {
              numericType = "int";
            }
            else if (cv.IsFloatingPointType())
            {
              numericType = "double";
            }
            Writer.WriteLine("  {0} v;", numericType);
            Writer.WriteLine("  string s = {0}TextBox.Text;", idColumnCanonical);
            if (!string.IsNullOrEmpty(numericType))
            {
              Writer.WriteLine("  if( !{0}.TryParse( s, out v ) ) {{", numericType);
            }
            else
            {
              // just assume is good
              Writer.WriteLine("  if( true ) {");
            }
            Writer.WriteLine("    e.Cancel = true;");
            Writer.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} must be {2}.\" );", idColumnCanonical, cv.Name, numericType);
            Writer.WriteLine("  }");
            if (cv.MinValue != null)
            {
              Writer.WriteLine(" else if( {0} > v ) {{ ", cv.MinValue);
              Writer.WriteLine("   e.Cancel = true;");
              Writer.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" );", idColumnCanonical, cv.Name, cv.MinValue);
              Writer.WriteLine(" } ");
            }
            if (cv.MaxValue != null)
            {
              Writer.WriteLine(" else if( {0} < v ) {{ ", cv.MaxValue);
              Writer.WriteLine("   e.Cancel = true;");
              Writer.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" );", idColumnCanonical, cv.Name, cv.MaxValue);
              Writer.WriteLine(" } ");
            }
          }
          Writer.WriteLine("  if( !e.Cancel ) {{ errorProvider1.SetError( {0}TextBox, \"\" ); }} ", idColumnCanonical);
          Writer.WriteLine("}");
          Writer.WriteLine();
        }
      }
    }

    protected void WriteValidationCodeDetailsGrid()
    {
      bool validationsEnabled = ValidationsEnabled;
      if (!validationsEnabled) return;
      List<ColumnValidation> validationColumns = GetValidationColumns();
      Writer.WriteLine("private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)");
      Writer.WriteLine("{");
      
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

        string numericType = "";
        if (cv.IsNumericType())
        {
          if (cv.IsIntegerType())
          {
            numericType = "int";
          }
          if (cv.IsFloatingPointType())
          {
            numericType = "double";
          }
          Writer.WriteLine(" {0} v;", numericType);
        }

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
          if (!string.IsNullOrEmpty(numericType))
          {
            Writer.WriteLine("  if( !{0}.TryParse( s, out v ) ) {{", numericType);
          }
          else
          {
            // just assume is good
            Writer.WriteLine("  if( true ) {");
          }
          Writer.WriteLine("    e.Cancel = true;");
          Writer.WriteLine("    row.ErrorText = \"The field {0} must be {1}.\";", cv.Name, numericType);
          Writer.WriteLine("    return;");
          Writer.WriteLine("  }");
          if (cv.MinValue != null)
          {
            Writer.WriteLine(" else if( {0} > v ) {{ ", cv.MinValue);
            Writer.WriteLine("    e.Cancel = true;");
            Writer.WriteLine("    row.ErrorText = \"The field {0} must be greater or equal than {1}.\";", cv.Name, cv.MinValue);
            Writer.WriteLine("    return;");
            Writer.WriteLine(" } ");
          }
          if (cv.MaxValue != null)
          {
            Writer.WriteLine(" else if( {0} < v ) {{ ", cv.MaxValue);
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
}
