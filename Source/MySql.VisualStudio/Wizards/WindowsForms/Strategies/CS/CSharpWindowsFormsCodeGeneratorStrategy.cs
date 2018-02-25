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
      return BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.GetCanonicalIdentifier(Identifier);    
    }

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

    internal protected override string GetApplicationFileName()
    {
      return "Application.Designer.cs";
    }

    internal protected override string GetExtension()
    {
      return ".cs";
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

    internal override string GetDataSourceForCombo( ColumnValidation cv )
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
      
      for( int j = 0; j < ValidationColumns.Count; j++ )
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

        if (cv.HasLookup)
        {
          Writer.WriteLine("this.{0}_comboBox = new System.Windows.Forms.ComboBox();", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical, xy.X, xy.Y);
          //Writer.WriteLine("this.{0}_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.FormattingEnabled = true;", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.Name = \"{0}_comboBox\";", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.Size = new System.Drawing.Size(206, 21);", idColumnCanonical);
          Writer.WriteLine("this.{0}_comboBox.TabIndex = {1};", idColumnCanonical, tabIdx++ );
          if (validationsEnabled)
          {
            Writer.WriteLine("this.{0}_comboBox.Validating += new System.ComponentModel.CancelEventHandler( this.{0}_comboBox_Validating );",
              idColumnCanonical);
          }
          if (addBindings)
          {
            // nothing
          }
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}_comboBox );", idColumnCanonical);
        }
        else if (cv.IsDateType())
        {
          Writer.WriteLine("//");
          Writer.WriteLine("// {0}_dateTimePicker", idColumnCanonical);
          Writer.WriteLine("//");
          Writer.WriteLine("this.{0}_dateTimePicker = new System.Windows.Forms.DateTimePicker();", idColumnCanonical);
          if (cv.IsDateTimeType())
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
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}_dateTimePicker );", idColumnCanonical);
        }
        else if (cv.IsBooleanType())
        {
          Writer.WriteLine("//");
          Writer.WriteLine("//{0}CheckBox", idColumnCanonical);
          Writer.WriteLine("//");
          Writer.WriteLine("this.{0}CheckBox = new System.Windows.Forms.CheckBox();", idColumnCanonical);
          Writer.WriteLine("this.{0}CheckBox.AutoSize = true;", idColumnCanonical);
          Writer.WriteLine("this.{0}CheckBox.Location = new System.Drawing.Point({1}, {2});", idColumnCanonical, xy.X, xy.Y + 3);
          Writer.WriteLine("this.{0}CheckBox.Name = \"{0}CheckBox\";", idColumnCanonical);
          Writer.WriteLine("this.{0}CheckBox.Size = new System.Drawing.Size(15, 14);", idColumnCanonical);
          Writer.WriteLine("this.{0}CheckBox.TabIndex = {1};", idColumnCanonical, tabIdx++);
          Writer.WriteLine("this.{0}CheckBox.UseVisualStyleBackColor = true;", idColumnCanonical);
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}CheckBox );", idColumnCanonical);
          if (addBindings)
          {
            Writer.WriteLine("this.{0}CheckBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Checked\", this.{2}BindingSource, \"{1}\", true));",
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }
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
              idColumnCanonical, cv.EfColumnMapping, CanonicalTableName);
          }

          Writer.WriteLine("this.{0}TextBox.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical, xy.X, xy.Y);
          Writer.WriteLine("this.{0}TextBox.Name = \"{0}TextBox\";", idColumnCanonical);
          Writer.WriteLine("this.{0}TextBox.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical, 100, 20);
          Writer.WriteLine("this.{0}TextBox.TabIndex = {1};", idColumnCanonical, tabIdx++);

          if (cv.MaxLength.HasValue)
          {
            Writer.WriteLine("this.{0}TextBox.MaxLength = {1};", idColumnCanonical, cv.MaxLength.Value );
          }

          if (cv.IsReadOnly())
          {
            Writer.WriteLine("this.{0}TextBox.Enabled = false;", idColumnCanonical);
          }
          else if (validationsEnabled)
          {
            Writer.WriteLine("this.{0}TextBox.Validating += new System.ComponentModel.CancelEventHandler( this.{0}TextBox_Validating );",
              idColumnCanonical);
          }
          Writer.WriteLine("this.Panel1.Controls.Add( this.{0}TextBox);", idColumnCanonical);
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
            Writer.WriteLine( "private void {0}_comboBox_Validating(object sender, CancelEventArgs e)", idColumnCanonical);
            Writer.WriteLine( "{");
            Writer.WriteLine( "int i = {0}_comboBox.SelectedIndex;", idColumnCanonical );
            Writer.WriteLine( "e.Cancel = false;" );
            Writer.WriteLine( "if( i == -1 )" );
            Writer.WriteLine( "{" );
            Writer.WriteLine( "e.Cancel = true; ");
            Writer.WriteLine( "errorProvider1.SetError({0}_comboBox, \"Must select a {0}\");", idColumnCanonical);
            Writer.WriteLine( "}" );
            Writer.WriteLine( "if( !e.Cancel )" );
            Writer.WriteLine( "{");
            Writer.WriteLine( "errorProvider1.SetError({0}_comboBox, \"\");", idColumnCanonical );
            Writer.WriteLine( "}" );
            Writer.WriteLine( "}");
            continue;
          }
          
          Writer.WriteLine("private void {0}TextBox_Validating(object sender, CancelEventArgs e)", idColumnCanonical);
          Writer.WriteLine("{");
          Writer.WriteLine("e.Cancel = false;");
          if (cv.Required)
          {
            Writer.WriteLine("if( string.IsNullOrEmpty( {0}TextBox.Text ) )", idColumnCanonical);
            Writer.WriteLine("{");
            Writer.WriteLine("e.Cancel = true;");
            Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ); ", idColumnCanonical, cv.Name);
            Writer.WriteLine("}");
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
            Writer.WriteLine("{0} v;", numericType);
            Writer.WriteLine("string s = {0}TextBox.Text;", idColumnCanonical);
            if (!string.IsNullOrEmpty(numericType))
            {
              Writer.WriteLine("if( !{0}.TryParse( s, out v ) )", numericType);
              Writer.WriteLine("{");
            }
            else
            {
              // just assume is good
              Writer.WriteLine("if( true )");
              Writer.WriteLine("{");
            }
            Writer.WriteLine("e.Cancel = true;");
            Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be {2}.\" );", idColumnCanonical, cv.Name, numericType);
            Writer.WriteLine("}");
            if (cv.MinValue != null)
            {
              Writer.WriteLine("else if( {0} > v )", cv.MinValue);
              Writer.WriteLine("{");
              Writer.WriteLine("e.Cancel = true;");
              Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" );", idColumnCanonical, cv.Name, cv.MinValue);
              Writer.WriteLine("}");
            }
            if (cv.MaxValue != null)
            {
              Writer.WriteLine("else if( {0} < v )", cv.MaxValue);
              Writer.WriteLine("{");
              Writer.WriteLine("e.Cancel = true;");
              Writer.WriteLine("errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" );", idColumnCanonical, cv.Name, cv.MaxValue);
              Writer.WriteLine("} ");
            }
          }
          Writer.WriteLine("if( !e.Cancel ) {{ errorProvider1.SetError( {0}TextBox, \"\" ); }} ", idColumnCanonical);
          Writer.WriteLine("}");
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

      Writer.WriteLine("private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)");
      Writer.WriteLine("{");
      
      Writer.WriteLine("string s;");
      Writer.WriteLine("DataGridViewRow row = dataGridView1.Rows[e.RowIndex];");
      Writer.WriteLine("object value = e.FormattedValue;");
      Writer.WriteLine("e.Cancel = false;");
      Writer.WriteLine("row.ErrorText = \"\";");
      Writer.WriteLine("if (row.IsNewRow) return;");
      for (int i = 0; i < validationColumns.Count; i++)
      {
        ColumnValidation cv = validationColumns[i];
        if (cv.IsBooleanType() || cv.HasLookup ) continue;
        string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);

        Writer.WriteLine("if (e.ColumnIndex == {0})", i);
        Writer.WriteLine("{");

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
          Writer.WriteLine("{0} v;", numericType);
        }

        if (cv.Required)
        {
          Writer.WriteLine("if( (value is DBNull) || string.IsNullOrEmpty( value.ToString() ) )");
          Writer.WriteLine("{");
          Writer.WriteLine("e.Cancel = true;");
          Writer.WriteLine("row.ErrorText = \"The field {0} is required\";", cv.Name);
          Writer.WriteLine("return;");
          Writer.WriteLine("}");
        }
        if (cv.IsNumericType())
        {
          Writer.WriteLine("s = value.ToString();");
          if (!string.IsNullOrEmpty(numericType))
          {
            Writer.WriteLine("if( !{0}.TryParse( s, out v ) )", numericType);
            Writer.WriteLine("{");
          }
          else
          {
            // just assume is good
            Writer.WriteLine("if( true )");
            Writer.WriteLine("{");
          }
          Writer.WriteLine("e.Cancel = true;");
          Writer.WriteLine("row.ErrorText = \"The field {0} must be {1}.\";", cv.Name, numericType);
          Writer.WriteLine("return;");
          Writer.WriteLine("}");
          if (cv.MinValue != null)
          {
            Writer.WriteLine("else if( {0} > v )", cv.MinValue);
            Writer.WriteLine("{");
            Writer.WriteLine("e.Cancel = true;");
            Writer.WriteLine("row.ErrorText = \"The field {0} must be greater or equal than {1}.\";", cv.Name, cv.MinValue);
            Writer.WriteLine("return;");
            Writer.WriteLine("} ");
          }
          if (cv.MaxValue != null)
          {
            Writer.WriteLine("else if( {0} < v )", cv.MaxValue);
            Writer.WriteLine("{");
            Writer.WriteLine("e.Cancel = true;");
            Writer.WriteLine("row.ErrorText = \"The field {0} must be lesser or equal than {1}\";", cv.Name, cv.MaxValue);
            Writer.WriteLine("return;");
            Writer.WriteLine("} ");
          }
        }

        Writer.WriteLine("}");
      }
      Writer.WriteLine("}");

      // DataError event
      Writer.WriteLine("");
      Writer.WriteLine("private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)");
      Writer.WriteLine("{");
      Writer.WriteLine("dataGridView1.Rows[e.RowIndex].ErrorText = e.Exception.Message;");
      Writer.WriteLine("e.Cancel = true;");
      Writer.WriteLine("}");

      Writer.PopIdentationLevel();
    }

    protected override void WriteDataGridColumnInitialization()
    {
      List<ColumnValidation> validationColumns = GetValidationColumns();

      Writer.PushIdentationLevel();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();
      Writer.IncreaseIdentation();

      Writer.WriteLine("dataGridView1.AutoGenerateColumns = false;");
      Writer.WriteLine("string strConn2 = \"{0};\";", ConnectionString);
      Writer.WriteLine("MySql.Data.MySqlClient.MySqlDataAdapter ad2 = null;");
      for (int i = 0; i < validationColumns.Count; i++)
      {
        ColumnValidation cv = validationColumns[i];
        string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
        if (cv.IsBooleanType())
        {
          Writer.WriteLine("System.Windows.Forms.DataGridViewCheckBoxColumn col{0} = new System.Windows.Forms.DataGridViewCheckBoxColumn();", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\";", idColumnCanonical, cv.EfColumnMapping );
          Writer.WriteLine("col{0}.HeaderText = \"{1}\";", idColumnCanonical, cv.Name );
          Writer.WriteLine("col{0}.Name = \"col{0}\";", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0});", idColumnCanonical);
        }
        else if (cv.HasLookup)
        {
          Writer.WriteLine("System.Windows.Forms.DataGridViewComboBoxColumn col{0} = new System.Windows.Forms.DataGridViewComboBoxColumn();", idColumnCanonical);
          Writer.WriteLine("col{0}.DataSource = {1};", idColumnCanonical, GetDataSourceForCombo( cv ));
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\";",
            idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.DisplayMember = \"{1}\";", idColumnCanonical, cv.EfLookupColumnMapping);
          Writer.WriteLine("col{0}.ValueMember = \"{1}\";", idColumnCanonical, cv.FkInfo.ReferencedColumnName);
          Writer.WriteLine("col{0}.HeaderText = \"{0}\";", idColumnCanonical);
          Writer.WriteLine("col{0}.Name = \"col{0}\";", idColumnCanonical );
          Writer.WriteLine("col{0}.ToolTipText = \"Pick the column from the foreign table to use as friendly value for this lookup.\";", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0});", idColumnCanonical);
        }
        else if( cv.IsDateType() )
        {
          Writer.WriteLine("MyDateTimePickerColumn col{0} = new MyDateTimePickerColumn();", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\";", idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.HeaderText = \"{1}\";", idColumnCanonical, cv.Name);
          Writer.WriteLine("col{0}.Name = \"col{0}\";", idColumnCanonical);
          Writer.WriteLine("dataGridView1.Columns.Add(col{0});", idColumnCanonical);
        }
        else
        {
          Writer.WriteLine("System.Windows.Forms.DataGridViewTextBoxColumn col{0} = new System.Windows.Forms.DataGridViewTextBoxColumn();", idColumnCanonical);
          Writer.WriteLine("col{0}.DataPropertyName = \"{1}\";", idColumnCanonical, cv.EfColumnMapping);
          Writer.WriteLine("col{0}.HeaderText = \"{1}\";", idColumnCanonical, cv.Name);
          Writer.WriteLine("col{0}.Name = \"col{0}\";", idColumnCanonical);
          if (cv.MaxLength.HasValue)
          {
            Writer.WriteLine("col{0}.MaxInputLength = {1};", idColumnCanonical, cv.MaxLength.Value);
          }
          if (cv.IsReadOnly())
          {
            Writer.WriteLine("col{0}.ReadOnly = true;", idColumnCanonical);
            Writer.WriteLine("col{0}.DefaultCellStyle.BackColor = Color.LightGray;", idColumnCanonical);
          }
          Writer.WriteLine("dataGridView1.Columns.Add(col{0});", idColumnCanonical);
        }
      }

      Writer.PopIdentationLevel();
    }
  }
}
