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
using System.Text;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using System.Windows.Forms;
using VSLangProj;
using MySql.Data.VisualStudio.SchemaComparer;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  /// <summary>
  ///  Wizard for generation of a Windows Forms based project.
  /// </summary>
  public class WindowsFormsWizard : BaseWizard<WindowsFormsWizardForm>
  { 
    private bool ValidationsEnabled
    {
      get
      {
        return WizardForm.ValidationColumns != null;
      } 
    }

    public WindowsFormsWizard()
      : base()
    {
      WizardForm = new WindowsFormsWizardForm();
    }

    public override void ProjectFinishedGenerating(Project project)
    {
      VSProject vsProj = project.Object as VSProject;
      /*
       * TODO:
       * - Generate EF or TypedDataSet as per user selection
       * - Add items to project.
       * - Customize generated code for form (add bindings).
       * */
      try
      {
        Columns = GetColumnsFromTable(WizardForm.TableName, WizardForm.Connection);
        if (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5)
        {
          AddEntityFrameworkNugetPackage(vsProj, ENTITY_FRAMEWORK_VERSION_5);
          GenerateEntityFrameworkModel(vsProj, ENTITY_FRAMEWORK_VERSION_5, WizardForm.Connection, "Model1", WizardForm.TableName);
        }
        else if (WizardForm.DataAccessTechnology == DataAccessTechnology.TypedDataSet)
        {
          GenerateTypedDataSetModel(vsProj, WizardForm.Connection, WizardForm.TableName);
        }
        else if( WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6 )
        {
          throw new NotImplementedException("Entity Framework 6 is not supported in this version.");
        }
        AddBindings(vsProj);
      }
      catch (WizardException e)
      {
        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      WizardForm.Dispose();
    }

    private void AddBindings(VSProject vsProj)
    {
      // Get Form.cs
      ProjectItem item = FindProjectItem(vsProj.Project.ProjectItems, "Form1.cs");
      // Get Form.Designer.cs
      ProjectItem itemDesigner = FindProjectItem(item.ProjectItems, "Form1.Designer.cs");
      
      _canonicalTableName = GetCanonicalIdentifier(WizardForm.TableName);
      if ((WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework6) ||
          (WizardForm.DataAccessTechnology == DataAccessTechnology.EntityFramework5))
      {
        AddBindings((string)(item.Properties.Item("FullPath").Value), AddBindingToFormEntityFramework);
        AddBindings((string)(itemDesigner.Properties.Item("FullPath").Value), AddBindingToFormDesignerEntityFramework);
      }
      else if (WizardForm.DataAccessTechnology == DataAccessTechnology.TypedDataSet)
      {
        AddBindings((string)(item.Properties.Item("FullPath").Value), AddBindingToFormTypedDataSet );
        AddBindings((string)(itemDesigner.Properties.Item("FullPath").Value), AddBindingToFormDesignerTypedDataSet);
      }
    }

    private string _canonicalTableName;
    private string _bindingSourceName;
    private StreamWriter sw;
    private delegate void BindingAdder(string line);

    private void AddBindingToFormEntityFramework( string line )
    {
      // TODO: this messy code generation may be better with CodeDom.
      if (line.Trim() == "// <WizardGeneratedCode>Namespace_UserCode</WizardGeneratedCode>")
      {
        // nothing
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Form_Load</WizardGeneratedCode>")
      {
        // Write Form_Load code.
        sw.WriteLine("Model1Entities ctx = new Model1Entities();");
        sw.WriteLine("ctx.{0}.Load();", _canonicalTableName);
        sw.WriteLine("{0}.DataSource = ctx.{1}.Local.ToBindingList();", _bindingSourceName, _canonicalTableName);
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Validation Events</WizardGeneratedCode>")
      {
        bool validationsEnabled = ValidationsEnabled;
        List<ColumnValidation> validationColumns = WizardForm.ValidationColumns;
        if (validationsEnabled)
        {
          for (int i = 0; i < validationColumns.Count; i++)
          {
            ColumnValidation cv = validationColumns[i];
            string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
            sw.WriteLine("private void {0}TextBox_Validating(object sender, CancelEventArgs e)", idColumnCanonical);
            sw.WriteLine("{");
            sw.WriteLine("  e.Cancel = false;");
            if (cv.Required)
            {
              sw.WriteLine("  if( string.IsNullOrEmpty( {0}TextBox.Text ) ) {{ ", idColumnCanonical);
              sw.WriteLine("    e.Cancel = true;");
              sw.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ); ", idColumnCanonical, cv.Name);
              sw.WriteLine("  }");
            }
            if (cv.IsNumericType())
            {
              sw.WriteLine("  int v;");
              sw.WriteLine("  string s = {0}TextBox.Text;", idColumnCanonical);
              sw.WriteLine("  if( !int.TryParse( s, out v ) ) {");
              sw.WriteLine("    e.Cancel = true;");
              sw.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} must be numeric.\" );", idColumnCanonical, cv.Name);
              sw.WriteLine("  }");
              if (cv.MinValue != null)
              {
                sw.WriteLine(" else if( cv.MinValue > v ) { ");
                sw.WriteLine("   e.Cancel = true;");
                sw.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" );", idColumnCanonical, cv.Name, cv.MinValue);
                sw.WriteLine(" } ");
              }
              if (cv.MaxValue != null)
              {
                sw.WriteLine(" else if( cv.MaxValue < v ) { ");
                sw.WriteLine("   e.Cancel = true;");
                sw.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" );", idColumnCanonical, cv.Name, cv.MaxValue);
                sw.WriteLine(" } ");
              }
            }
            sw.WriteLine("  if( !e.Cancel ) {{ errorProvider1.SetError( {0}TextBox, \"\" ); }} ", idColumnCanonical);
            sw.WriteLine("}");
            sw.WriteLine();
          }
        }
      }
      else
      {
        // just write same line
        sw.WriteLine(line);
      }
    }

    private void AddBindingToFormTypedDataSet(string line)
    {
      // TODO: this messy code generation may be better with CodeDom.
      if (line.Trim() == "// <WizardGeneratedCode>Private Variables Frontend</WizardGeneratedCode>")
      {
        sw.WriteLine("private MySqlDataAdapter ad;");
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Namespace_UserCode</WizardGeneratedCode>")
      {
        // nothing
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Form_Load</WizardGeneratedCode>")
      {
        // Write Form_Load code.
        sw.WriteLine( "string strConn = \"{0};\";", WizardForm.Connection.ConnectionString );
        sw.WriteLine( "ad = new MySqlDataAdapter(\"select * from `{0}`\", strConn);", WizardForm.TableName );
        sw.WriteLine("MySqlCommandBuilder builder = new MySqlCommandBuilder(ad);");
        sw.WriteLine("ad.Fill(this.newDataSet._Table);");
        sw.WriteLine("ad.DeleteCommand = builder.GetDeleteCommand();");
        sw.WriteLine("ad.UpdateCommand = builder.GetUpdateCommand();");
        sw.WriteLine("ad.InsertCommand = builder.GetInsertCommand();");
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Save Event</WizardGeneratedCode>")
      {
        foreach( KeyValuePair<string,Column> kvp in Columns )
        {
          if (kvp.Value.IsDateType())
          {
            string idColumnCanonical = GetCanonicalIdentifier(kvp.Key);
            sw.WriteLine("((DataRowView){2}BindingSource.Current)[\"{0}\"] = {1}TextBox.Text;", 
              kvp.Value.ColumnName, idColumnCanonical, _canonicalTableName);
          }
        }
        sw.WriteLine("{0}BindingSource.EndEdit();", _canonicalTableName );
        sw.WriteLine("ad.Update(this.newDataSet._Table);");
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Validation Events</WizardGeneratedCode>")
      {
        bool validationsEnabled = ValidationsEnabled;
        List<ColumnValidation> validationColumns = WizardForm.ValidationColumns;
        if (validationsEnabled)
        { 
          for (int i = 0; i < validationColumns.Count; i++)
          {
            ColumnValidation cv = validationColumns[ i ];
            string idColumnCanonical = GetCanonicalIdentifier(cv.Column.ColumnName);
            sw.WriteLine("private void {0}TextBox_Validating(object sender, CancelEventArgs e)", idColumnCanonical );
            sw.WriteLine("{");
            sw.WriteLine("  e.Cancel = false;");
            if (cv.Required)
            {
              sw.WriteLine("  if( string.IsNullOrEmpty( {0}TextBox.Text ) ) {{ ", idColumnCanonical);
              sw.WriteLine("    e.Cancel = true;");
              sw.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} is required\" ); ", idColumnCanonical, cv.Name);
              sw.WriteLine("  }");
            }
            if (cv.IsNumericType() )
            {
              sw.WriteLine("  int v;");
              sw.WriteLine("  string s = {0}TextBox.Text;", idColumnCanonical);
              sw.WriteLine("  if( !int.TryParse( s, out v ) ) {");
              sw.WriteLine("    e.Cancel = true;");
              sw.WriteLine("    errorProvider1.SetError( {0}TextBox, \"The field {1} must be numeric.\" );", idColumnCanonical, cv.Name );
              sw.WriteLine("  }");
              if (cv.MinValue != null )
              {
                sw.WriteLine(" else if( {0} > v ) {{ ", cv.MinValue);
                sw.WriteLine("   e.Cancel = true;");
                sw.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be greater or equal than {2}.\" );", idColumnCanonical, cv.Name, cv.MinValue );
                sw.WriteLine(" } ");
              }
              if (cv.MaxValue != null)
              {
                sw.WriteLine(" else if( {0} < v ) {{ ", cv.MaxValue);
                sw.WriteLine("   e.Cancel = true;");
                sw.WriteLine("   errorProvider1.SetError( {0}TextBox, \"The field {1} must be lesser or equal than {2}\" );", idColumnCanonical, cv.Name, cv.MaxValue);
                sw.WriteLine(" } ");
              }
            }
            sw.WriteLine("  if( !e.Cancel ) {{ errorProvider1.SetError( {0}TextBox, \"\" ); }} ", idColumnCanonical);
            sw.WriteLine("}");
            sw.WriteLine();
          }
        }
      }
      else
      {
        // just write same line
        sw.WriteLine(line);
      }
    }

    private void AddBindingToFormDesignerEntityFramework(string line)
    {
      // TODO: this messy code generation may be better with CodeDom.
      if (line.Trim() == "// <WizardGeneratedCode>Control Declaration</WizardGeneratedCode>")
      {
        // Generate the declaration of all control variables.
        sw.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", _canonicalTableName);
        foreach( KeyValuePair<string,Column> kvp in Columns )
        {
          string idColumnCanonical = GetCanonicalIdentifier( kvp.Key );
          sw.WriteLine("private System.Windows.Forms.TextBox {0}TextBox;", idColumnCanonical );
          sw.WriteLine("private System.Windows.Forms.Label {0}Label;", idColumnCanonical);
        }
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Control Initialization</WizardGeneratedCode>")
      {
        // Generate the InitializeComponent code to configure everything (including custom coordinates for the controls).
        // TODO: The type may not always be the table name.
        sw.WriteLine("this.{0}BindingSource.DataSource = typeof({1});", _canonicalTableName, _canonicalTableName);
        WriteControlInitialization();
      }
      else
      {
        // just write same line
        sw.WriteLine(line);
      }
    }

    private void AddBindingToFormDesignerTypedDataSet(string line)
    {
      // TODO: this messy code generation may be better with CodeDom.
      if (line.Trim() == "// <WizardGeneratedCode>Control Declaration</WizardGeneratedCode>")
      {
        // Generate the declaration of all control variables.
        sw.WriteLine("private NewDataSet newDataSet;");
        sw.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", _canonicalTableName);
        foreach (KeyValuePair<string, Column> kvp in Columns)
        {
          string idColumnCanonical = GetCanonicalIdentifier(kvp.Key);
          sw.WriteLine("private System.Windows.Forms.TextBox {0}TextBox;", idColumnCanonical);
          sw.WriteLine("private System.Windows.Forms.Label {0}Label;", idColumnCanonical);
        }
      }
      else if (line.Trim() == "// <WizardGeneratedCode>Control Initialization</WizardGeneratedCode>")
      {
        // Generate the InitializeComponent code to configure everything (including custom coordinates for the controls).
        sw.WriteLine("this.bindingNavigator1.BindingSource = this.{0}BindingSource;", _canonicalTableName);
        sw.WriteLine("// ");
        sw.WriteLine("// newDataSet");
        sw.WriteLine("// ");
        sw.WriteLine("this.newDataSet.DataSetName = \"NewDataSet\";");
        sw.WriteLine("this.newDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;");

        sw.WriteLine("// ");
        sw.WriteLine("// tableBindingSource");
        sw.WriteLine("// ");
        sw.WriteLine("this.{0}BindingSource.DataMember = \"Table\";", _canonicalTableName);
        sw.WriteLine("this.{0}BindingSource.DataSource = this.newDataSet;", _canonicalTableName);

        WriteControlInitialization();
      }
      else if (line.Trim() == "// <WizardGeneratedCode>BeforeSuspendLayout</WizardGeneratedCode>")
      {
        sw.WriteLine("this.newDataSet = new NewDataSet();");
        sw.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", _canonicalTableName);
      }
      else if (line.Trim() == "// <WizardGeneratedCode>AfterSuspendLayout</WizardGeneratedCode>")
      {
        sw.WriteLine("((System.ComponentModel.ISupportInitialize)(this.newDataSet)).BeginInit();");
        sw.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", _canonicalTableName);
      }
      else if (line.Trim() == "// <WizardGeneratedCode>BeforeResumeSuspendLayout</WizardGeneratedCode>")
      {
        sw.WriteLine("((System.ComponentModel.ISupportInitialize)(this.newDataSet)).EndInit();");
        sw.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", _canonicalTableName);
      }
      else
      {
        // just write same line
        sw.WriteLine(line);
      }
    }

    private void WriteControlInitialization()
    {
      Label l = new Label();
      Size szText = TextRenderer.MeasureText(GetMaxWidthString(Columns), l.Font);
      Point initLoc = new Point( szText.Width + 10, 50);
      Point xy = new Point( initLoc.X, initLoc.Y);
      int tabIdx = 1;
      bool validationsEnabled = ValidationsEnabled;

      foreach( KeyValuePair<string,Column> kvp in Columns )
      {
        string colName = kvp.Key;
        string idColumnCanonical = GetCanonicalIdentifier(colName);
        sw.WriteLine("//");
        sw.WriteLine("// {0}Label", idColumnCanonical);
        sw.WriteLine("//");
        sw.WriteLine("this.{0}Label = new System.Windows.Forms.Label();", idColumnCanonical);

        sw.WriteLine("this.{0}Label.AutoSize = true;", idColumnCanonical);
        Size szLabel = TextRenderer.MeasureText(colName, l.Font);
        sw.WriteLine("this.{0}Label.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical,
          xy.X - 10 - szLabel.Width, xy.Y);
        sw.WriteLine("this.{0}Label.Name = \"{1}\";", idColumnCanonical, colName);
        sw.WriteLine("this.{0}Label.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical, 
          szLabel.Width, szLabel.Height );
        sw.WriteLine("this.{0}Label.TabIndex = {1};", idColumnCanonical, tabIdx++);
        sw.WriteLine("this.{0}Label.Text = \"{1}\";", idColumnCanonical, colName);
        sw.WriteLine("this.Controls.Add( this.{0}Label );", idColumnCanonical);

        sw.WriteLine("//");
        sw.WriteLine("// {0}TextBox", idColumnCanonical);
        sw.WriteLine("//");
        sw.WriteLine("this.{0}TextBox = new System.Windows.Forms.TextBox();", idColumnCanonical);
        sw.WriteLine("this.{0}TextBox.DataBindings.Add(new System.Windows.Forms.Binding(\"Text\", this.{2}BindingSource, \"{1}\", true ));",
          idColumnCanonical, colName, _canonicalTableName);
        sw.WriteLine("this.{0}TextBox.Location = new System.Drawing.Point( {1}, {2} );", idColumnCanonical, xy.X, xy.Y);
        sw.WriteLine("this.{0}TextBox.Name = \"{1}\";", idColumnCanonical, colName);
        sw.WriteLine("this.{0}TextBox.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical, 100, 20 );
        sw.WriteLine("this.{0}TextBox.TabIndex = {1};", idColumnCanonical, tabIdx++);

        if (validationsEnabled)
        {
          sw.WriteLine("this.{0}TextBox.Validating += new System.ComponentModel.CancelEventHandler( this.{0}TextBox_Validating );",
            idColumnCanonical);
        }
        sw.WriteLine("this.Controls.Add( this.{0}TextBox);", idColumnCanonical);
        xy.Y += szText.Height * 2;
      }
    }

    private string GetMaxWidthString(Dictionary<string, Column> l)
    { 
      KeyValuePair<string, Column> maxWidthItem = new KeyValuePair<string,Column>("", null );
      foreach( KeyValuePair<string, Column> kvp in l )
      {
        if (kvp.Key.Length > maxWidthItem.Key.Length) maxWidthItem = kvp;
      }
      return maxWidthItem.Key;
    }

    private void AddBindings(string FormPath, BindingAdder bindingAdder )
    {
      _bindingSourceName = string.Format("{0}BindingSource", _canonicalTableName);
      string originalContents = File.ReadAllText(FormPath);
      FileStream fs = new FileStream(FormPath, FileMode.Truncate, FileAccess.Write, FileShare.Read, 16284);
      using( StringReader sr = new StringReader(originalContents) )
      {
        using( sw = new StreamWriter( fs ) )
        {
          string line = null;
          while ((line = sr.ReadLine()) != null)
          {
            bindingAdder( line );
          }
        } // using StreamWriter
      } // using StreamReader
    }
  }
}
