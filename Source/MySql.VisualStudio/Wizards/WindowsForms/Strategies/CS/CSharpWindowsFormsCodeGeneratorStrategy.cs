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
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteFormLoadCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteValidationCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteVariablesUserCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteSaveEventCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteAddEventCode()
    {
      Writer.WriteLine("{0}BindingSource.AddNew();", CanonicalTableName );
    }

    protected override void WriteDesignerControlDeclCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteDesignerControlInitCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      // TODO:
      throw new NotImplementedException();
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      // TODO:
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
        Writer.WriteLine("this.{0}TextBox.Name = \"{0}TextBox\";", idColumnCanonical );
        Writer.WriteLine("this.{0}TextBox.Size = new System.Drawing.Size( {1}, {2} );", idColumnCanonical, 100, 20);
        Writer.WriteLine("this.{0}TextBox.TabIndex = {1};", idColumnCanonical, tabIdx++);

        if (validationsEnabled)
        {
          Writer.WriteLine("this.{0}TextBox.Validating += new System.ComponentModel.CancelEventHandler( this.{0}TextBox_Validating );",
            idColumnCanonical);
        }
        Writer.WriteLine("this.Panel1.Controls.Add( this.{0}TextBox);", idColumnCanonical);
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
  }
}
