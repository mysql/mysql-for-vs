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
  internal class VBWindowsFormsCodeGeneratorStrategy : WindowsFormsCodeGeneratorStrategy
  {
    internal VBWindowsFormsCodeGeneratorStrategy(StrategyConfig config)
      : base(config)
    {
      ActionMappings = new Dictionary<string, WriterDelegate>();

      ActionMappings["'<WizardGeneratedCode>Namespace_UserCode</WizardGeneratedCode>"] = WriteUsingUserCode;
      ActionMappings["'<WizardGeneratedCode>Form_Load</WizardGeneratedCode>"] = WriteFormLoadCode;
      ActionMappings["'<WizardGeneratedCode>Validation Events</WizardGeneratedCode>"] = WriteValidationCode;
      ActionMappings["'<WizardGeneratedCode>Private Variables Frontend</WizardGeneratedCode>"] = WriteVariablesUserCode;
      ActionMappings["'<WizardGeneratedCode>Save Event</WizardGeneratedCode>"] = WriteSaveEventCode;
      ActionMappings["'<WizardGeneratedCode>Add Event</WizardGeneratedCode>"] = WriteAddEventCode;
      ActionMappings["'<WizardGeneratedCode>Designer Control Declaration</WizardGeneratedCode>"] = WriteDesignerControlDeclCode;
      ActionMappings["'<WizardGeneratedCode>Designer Control Initialization</WizardGeneratedCode>"] = WriteDesignerControlInitCode;
      ActionMappings["'<WizardGeneratedCode>Designer BeforeSuspendLayout</WizardGeneratedCode>"] = WriteDesignerBeforeSuspendCode;
      ActionMappings["'<WizardGeneratedCode>Designer AfterSuspendLayout</WizardGeneratedCode>"] = WriteDesignerAfterSuspendCode;
      ActionMappings["'<WizardGeneratedCode>Designer BeforeResumeSuspendLayout</WizardGeneratedCode>"] = WriteBeforeResumeSuspendCode;
    }

    /// <summary>
    /// Transforms a user identifier like MySql table to ensure it is a valid identifier in C#/VB.NET.
    /// </summary>
    /// <returns></returns>
    internal protected override string GetCanonicalIdentifier(string Identifier)
    {
      return BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.GetCanonicalIdentifier(Identifier);
    }

    internal protected override string GetEdmDesignerFileName()
    {
      return "{0}.Designer.vb";
    }

    internal protected override string GetFormDesignerFileName()
    {
      return "Form1.Designer.vb";
    }

    internal protected override string GetFormFileName()
    {
      return "Form1.vb";
    }

    internal protected override string GetApplicationFileName()
    {
      return "Application.Designer.vb";
    }

    internal protected override string GetExtension()
    {
      return ".vb";
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
      Writer.WriteLine("{0}BindingSource.AddNew()", CanonicalTableName);
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

    internal override string GetDataSourceForCombo(ColumnValidation cv)
    {
      throw new NotImplementedException();
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      Label l = new Label();
      Size szText = TextRenderer.MeasureText(GetMaxWidthString(Columns), l.Font);
      Point initLoc = new Point(szText.Width + 10 + 50, 50);
      Point xy = new Point(initLoc.X, initLoc.Y);
      bool passedHalve = false;
      int tabIdx = 1;
      bool validationsEnabled = ValidationsEnabled;
      int i = 0;

      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      for (int j = 0; j < ValidationColumns.Count; j++)
      {
        ColumnValidation cv = ValidationColumns[j];
        string colName = cv.Name;
        string idColumnCanonical = GetCanonicalIdentifier(colName);

        // Place half the column input in one column and the other in the second column.
        if (!passedHalve && ++i > (Columns.Count / 2))
        {
          passedHalve = true;
          xy.X += 200 + 20 + szText.Width;
          xy.Y = initLoc.Y;
        }

        Writer.WriteLine("'");
        Writer.WriteLine("' {0}Label", idColumnCanonical);
        Writer.WriteLine("'");
        Writer.WriteLine("Me.{0}Label = New System.Windows.Forms.Label()", idColumnCanonical);

        Writer.WriteLine("Me.{0}Label.AutoSize = True", idColumnCanonical);
        Size szLabel = TextRenderer.MeasureText(colName, l.Font);
        Writer.WriteLine("Me.{0}Label.Location = New System.Drawing.Point( {1}, {2} )", idColumnCanonical,
          xy.X - 10 - szLabel.Width, xy.Y);
        Writer.WriteLine("Me.{0}Label.Name = \"{0}Label\"", idColumnCanonical );
        Writer.WriteLine("Me.{0}Label.Size = New System.Drawing.Size( {1}, {2} )", idColumnCanonical,
          szLabel.Width, szLabel.Height);
        Writer.WriteLine("Me.{0}Label.TabIndex = {1}", idColumnCanonical, tabIdx++);
        Writer.WriteLine("Me.{0}Label.Text = \"{1}\"", idColumnCanonical, colName);
        Writer.WriteLine("Me.Panel1.Controls.Add( Me.{0}Label )", idColumnCanonical);


        if (cv.HasLookup)
        {
          Writer.WriteLine("Me.{0}_comboBox = New System.Windows.Forms.ComboBox()", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.Location = New System.Drawing.Point( {1}, {2} )", idColumnCanonical, xy.X, xy.Y);
          //Writer.WriteLine("Me.{0}_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.FormattingEnabled = True", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.Name = \"{0}_comboBox\"", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.Size = New System.Drawing.Size(206, 21)", idColumnCanonical);
          Writer.WriteLine("Me.{0}_comboBox.TabIndex = {1}", idColumnCanonical, tabIdx++);
          if (validationsEnabled)
          {
            Writer.WriteLine("AddHandler Me.{0}_comboBox.Validating, AddressOf Me.{0}_comboBox_Validating",
              idColumnCanonical);
          }
          if (addBindings)
          {
            // nothing
          }
          Writer.WriteLine("Me.Panel1.Controls.Add( Me.{0}_comboBox )", idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("'");
          Writer.WriteLine("'{0}_dateTimePicker", idColumnCanonical);
          Writer.WriteLine("'");
          Writer.WriteLine("Me.{0}_dateTimePicker = New System.Windows.Forms.DateTimePicker()", idColumnCanonical);
          if (cv.IsDateTimeType())
          {
            Writer.WriteLine("Me.{0}_dateTimePicker.CustomFormat = \"dd/MM/yyyy, hh:mm\" ", idColumnCanonical);
          }
          else
          {
            Writer.WriteLine("Me.{0}_dateTimePicker.CustomFormat = \"dd/MM/yyyy\" ", idColumnCanonical);
          }
          Writer.WriteLine("Me.{0}_dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom", idColumnCanonical);
          Writer.WriteLine("Me.{0}_dateTimePicker.Location = New System.Drawing.Point({1}, {2})", idColumnCanonical, xy.X, xy.Y);
          Writer.WriteLine("Me.{0}_dateTimePicker.Name = \"{0}_dateTimePicker\"", idColumnCanonical);
          Writer.WriteLine("Me.{0}_dateTimePicker.Size = New System.Drawing.Size(200, 20)", idColumnCanonical);
          Writer.WriteLine("Me.{0}_dateTimePicker.TabIndex = {1}", idColumnCanonical, tabIdx++);
          Writer.WriteLine("Me.{0}_dateTimePicker.Value = New System.DateTime(2014, 5, 26, 17, 35, 11, 0)", idColumnCanonical);
          if (addBindings)
          {
            Writer.WriteLine("Me.{0}_dateTimePicker.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", True ))",
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }
          Writer.WriteLine("Me.Panel1.Controls.Add( Me.{0}_dateTimePicker )", idColumnCanonical);
        }
        else if (cv.IsBooleanType())
        {
          Writer.WriteLine( "'" );
          Writer.WriteLine( "'{0}CheckBox", idColumnCanonical );
          Writer.WriteLine("'" );
          Writer.WriteLine("Me.{0}CheckBox = New System.Windows.Forms.CheckBox()", idColumnCanonical);
          Writer.WriteLine("Me.{0}CheckBox.AutoSize = True", idColumnCanonical );
          Writer.WriteLine("Me.{0}CheckBox.Location = New System.Drawing.Point({1}, {2})", idColumnCanonical, xy.X, xy.Y + 3);
          Writer.WriteLine("Me.{0}CheckBox.Name = \"{0}CheckBox\"", idColumnCanonical);
          Writer.WriteLine("Me.{0}CheckBox.Size = New System.Drawing.Size(15, 14)", idColumnCanonical);
          Writer.WriteLine("Me.{0}CheckBox.TabIndex = {1}", idColumnCanonical, tabIdx++);
          Writer.WriteLine("Me.{0}CheckBox.UseVisualStyleBackColor = True", idColumnCanonical);
          Writer.WriteLine("Me.Panel1.Controls.Add( Me.{0}CheckBox )", idColumnCanonical);
          if (addBindings)
          {
            Writer.WriteLine("Me.{0}CheckBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Checked\", Me.{2}BindingSource, \"{1}\", True))",
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }
        }
        else
        {
          Writer.WriteLine("'");
          Writer.WriteLine("' {0}TextBox", idColumnCanonical);
          Writer.WriteLine("'");
          Writer.WriteLine("Me.{0}TextBox = New System.Windows.Forms.TextBox()", idColumnCanonical);

          if (addBindings)
          {
            Writer.WriteLine("Me.{0}TextBox.DataBindings.Add(New System.Windows.Forms.Binding(\"Text\", Me.{2}BindingSource, \"{1}\", true ))",
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }

          Writer.WriteLine("Me.{0}TextBox.Location = New System.Drawing.Point( {1}, {2} )", idColumnCanonical, xy.X, xy.Y);
          Writer.WriteLine("Me.{0}TextBox.Name = \"{0}TextBox\"", idColumnCanonical);
          Writer.WriteLine("Me.{0}TextBox.Size = New System.Drawing.Size( {1}, {2} )", idColumnCanonical, 100, 20);
          Writer.WriteLine("Me.{0}TextBox.TabIndex = {1}", idColumnCanonical, tabIdx++);

          if (cv.MaxLength.HasValue)
          {
            Writer.WriteLine("Me.{0}TextBox.MaxLength = {1}", idColumnCanonical, cv.MaxLength.Value);
          }

          if (cv.IsReadOnly())
          {
            Writer.WriteLine("Me.{0}TextBox.Enabled = False", idColumnCanonical);
          }
          else if (validationsEnabled)
          {
            Writer.WriteLine("AddHandler Me.{0}TextBox.Validating, AddressOf Me.{0}TextBox_Validating",
              idColumnCanonical);
          }
          Writer.WriteLine("Me.Panel1.Controls.Add( Me.{0}TextBox)", idColumnCanonical);
        }
        xy.Y += szText.Height * 2;
      }

      Writer.PopIdentationLevel();
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

      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      if (validationsEnabled)
      {
        for (int i = 0; i < validationColumns.Count; i++)
        {
          ColumnValidation cv = validationColumns[i];
          if ( cv.IsDateType() || cv.IsReadOnly() || cv.IsBooleanType()) continue;

          string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);

          if (cv.HasLookup && cv.Required)
          {
            Writer.WriteLine("Private Sub {0}_comboBox_Validating(sender As System.Object, e As System.ComponentModel.CancelEventArgs)", idColumnCanonical);
            Writer.WriteLine("Dim i As Integer = {0}_comboBox.SelectedIndex", idColumnCanonical);
            Writer.WriteLine("e.Cancel = False");
            Writer.WriteLine("If i = -1 Then");
            Writer.WriteLine("e.Cancel = True" );
            Writer.WriteLine("errorProvider1.SetError({0}_comboBox, \"Must select a {0}\")", idColumnCanonical);
            Writer.WriteLine("End If");
            Writer.WriteLine("If Not e.Cancel Then" );
            Writer.WriteLine("errorProvider1.SetError({0}_comboBox, \"\")", idColumnCanonical);
            Writer.WriteLine("End If");
            Writer.WriteLine("End Sub");
            continue;
          }

          Writer.WriteLine("Private Sub {0}TextBox_Validating(sender As Object, e As CancelEventArgs)", idColumnCanonical);
          Writer.WriteLine("");
          Writer.WriteLine("e.Cancel = False");
          if (cv.Required)
          {
            Writer.WriteLine("If String.IsNullOrEmpty( {0}TextBox.Text ) Then ", idColumnCanonical);
            Writer.WriteLine("e.Cancel = True");
            Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ) ", idColumnCanonical, cv.Name);
            Writer.WriteLine("End If");
          }
          if (cv.IsNumericType())
          {
            string numericType = "";
            if (cv.IsIntegerType())
            {
              numericType = "Integer";
            }
            else if (cv.IsFloatingPointType())
            {
              numericType = "Double";
            }
            Writer.WriteLine("Dim v As {0}", numericType);
            Writer.WriteLine("Dim s As string = {0}TextBox.Text", idColumnCanonical);
            if (!string.IsNullOrEmpty(numericType))
            {
              Writer.WriteLine("If Not {0}.TryParse( s, v ) Then", numericType);
            }
            else
            {
              // just assume is good
              Writer.WriteLine("If True Then");
            }
            Writer.WriteLine("e.Cancel = True");
            Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be {2}.\" )", idColumnCanonical, cv.Name, numericType);
            if (cv.MinValue != null)
            {
              Writer.WriteLine("ElseIf {0} > v Then ", cv.MinValue);
              Writer.WriteLine("e.Cancel = True");
              Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" )", idColumnCanonical, cv.Name, cv.MinValue);
            }
            if (cv.MaxValue != null)
            {
              Writer.WriteLine("ElseIf {0} < v Then ", cv.MaxValue);
              Writer.WriteLine("e.Cancel = True");
              Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" )", idColumnCanonical, cv.Name, cv.MaxValue);
            }
            Writer.WriteLine("End If");
          }
          Writer.WriteLine("If Not e.Cancel Then");
          Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"\" )", idColumnCanonical);
          Writer.WriteLine("End If");
          Writer.WriteLine("End Sub");
          Writer.WriteLine();
        }
      }
      Writer.PopIdentationLevel();
    }

    protected void WriteValidationCodeDetailsGrid()
    {
      bool validationsEnabled = ValidationsEnabled;
      if (!validationsEnabled) return;
      List<ColumnValidation> validationColumns = GetValidationColumns();

      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("Private Sub dataGridView1_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs)");
      Writer.WriteLine("");
      
      Writer.WriteLine("Dim s As String");
      Writer.WriteLine("Dim row As DataGridViewRow= dataGridView1.Rows(e.RowIndex)");
      Writer.WriteLine("Dim value As Object = e.FormattedValue");
      Writer.WriteLine("e.Cancel = False");
      Writer.WriteLine("row.ErrorText = \"\"");
      Writer.WriteLine("If row.IsNewRow Then");
      Writer.WriteLine("Return");
      Writer.WriteLine("End If");
      for (int i = 0; i < validationColumns.Count; i++)
      {
        ColumnValidation cv = validationColumns[i];
        if (cv.IsBooleanType() || cv.HasLookup) continue;
        string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);

        Writer.WriteLine("If e.ColumnIndex = {0} Then", i);
        Writer.WriteLine("");

        string numericType = "";
        if (cv.IsNumericType())
        {
          if (cv.IsIntegerType())
          {
            numericType = "Integer";
          }
          if (cv.IsFloatingPointType())
          {
            numericType = "Double";
          }
          Writer.WriteLine("Dim v as {0}", numericType);
        }

        if (cv.Required)
        {
          Writer.WriteLine("If (TypeOf value is DBNull) OrElse String.IsNullOrEmpty( value.ToString() ) Then ");
          Writer.WriteLine("e.Cancel = True");
          Writer.WriteLine("row.ErrorText = \"The field {0} is required\"", cv.Name);
          Writer.WriteLine("Return");
          Writer.WriteLine("End If");
        }
        if (cv.IsNumericType())
        {
          Writer.WriteLine("s = value.ToString()");
          if (!string.IsNullOrEmpty(numericType))
          {
            Writer.WriteLine("If Not {0}.TryParse( s, v ) Then", numericType);
          }
          else
          {
            // just assume is good
            Writer.WriteLine("If True Then");
          }
          Writer.WriteLine("e.Cancel = True");          
          Writer.WriteLine("row.ErrorText = \"The field {0} must be {1}.\"", cv.Name, numericType );
          Writer.WriteLine("Return");
          if (cv.MinValue != null)
          {
            Writer.WriteLine("ElseIf {0} > v Then", cv.MinValue);
            Writer.WriteLine("e.Cancel = True");
            Writer.WriteLine("row.ErrorText = \"The field {0} must be greater or equal than {1}.\"", cv.Name, cv.MinValue);
            Writer.WriteLine("Return");
          }
          if (cv.MaxValue != null)
          {
            Writer.WriteLine("ElseIf {0} < v Then", cv.MaxValue);
            Writer.WriteLine("e.Cancel = True");
            Writer.WriteLine("row.ErrorText = \"The field {0} must be lesser or equal than {1}\"", cv.Name, cv.MaxValue);
            Writer.WriteLine("Return");
          }
          Writer.WriteLine("End If");
        }

        Writer.WriteLine("End If");
      }
      Writer.WriteLine("End Sub");
      // DataError
      Writer.WriteLine( "Private Sub dataGridView1_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)" );
      Writer.WriteLine( "dataGridView1.Rows(e.RowIndex).ErrorText = e.Exception.Message" );
      Writer.WriteLine( "e.Cancel = true" );
      Writer.WriteLine( "End Sub");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDataGridColumnInitialization()
    {
      List<ColumnValidation> validationColumns = GetValidationColumns();

      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("dataGridView1.AutoGenerateColumns = False");
      Writer.WriteLine("Dim strConn2 As string = \"{0};\"", ConnectionString);
      Writer.WriteLine("Dim ad2 As MySql.Data.MySqlClient.MySqlDataAdapter = Nothing");
      for (int i = 0; i < validationColumns.Count; i++)
      {
        ColumnValidation cv = validationColumns[i];
        string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
        if (cv.IsBooleanType())
        {
          Writer.WriteLine("Dim col{0} As System.Windows.Forms.DataGridViewCheckBoxColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\"", idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.HeaderText = \"{1}\"", idColumnCanonical, cv.Name);
          Writer.WriteLine("col{0}.Name = \"col{0}\"", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0})", idColumnCanonical);
        }
        else if (cv.HasLookup)
        {
          Writer.WriteLine("Dim col{0} As System.Windows.Forms.DataGridViewComboBoxColumn = New System.Windows.Forms.DataGridViewComboBoxColumn()", idColumnCanonical);
          Writer.WriteLine("col{0}.DataSource = {1}", idColumnCanonical, GetDataSourceForCombo(cv));
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\"",
            idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.DisplayMember = \"{1}\"", idColumnCanonical, cv.EfLookupColumnMapping);
          Writer.WriteLine("col{0}.ValueMember = \"{1}\"", idColumnCanonical, cv.FkInfo.ReferencedColumnName);
          Writer.WriteLine("col{0}.HeaderText = \"{0}\"", idColumnCanonical);
          Writer.WriteLine("col{0}.Name = \"col{0}\"", idColumnCanonical);
          Writer.WriteLine("col{0}.ToolTipText = \"Pick the column from the foreign table to use as friendly value for this lookup.\"", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0})", idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("Dim col{0} As MyDateTimePickerColumn = New MyDateTimePickerColumn()", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\"", idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.HeaderText = \"{1}\"", idColumnCanonical, cv.Name);
          Writer.WriteLine("col{0}.Name = \"col{0}\"", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0})", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("Dim col{0} As System.Windows.Forms.DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\"", idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.HeaderText = \"{1}\"", idColumnCanonical, cv.Name);
          Writer.WriteLine("col{0}.Name = \"col{0}\"", idColumnCanonical);
          if (cv.MaxLength.HasValue)
          {
            Writer.WriteLine("col{0}.MaxInputLength = {1}", idColumnCanonical, cv.MaxLength.Value);
          }
          if (cv.IsReadOnly())
          {
            Writer.WriteLine("col{0}.ReadOnly = True", idColumnCanonical);
            Writer.WriteLine("col{0}.DefaultCellStyle.BackColor = Color.LightGray", idColumnCanonical);
          }
          Writer.WriteLine("dataGridView1.Columns.Add(col{0})", idColumnCanonical);
        }
      }

      Writer.PopIdentationLevel();
    }
  }
}
