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
using System.IO;
using System.Linq;
using System.Text;
using VSLangProj;
using EnvDTE;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public class VBWindowsFormsWizard : WindowsFormsWizard
  {
    public VBWindowsFormsWizard()
      : base( LanguageGenerator.VBNET )
    {
    }

    protected override string MenuEventHandlerMarker { get { return "' <WizardGeneratedCode>Menu EventHandlers</WizardGeneratedCode>"; } }

    protected override string MenuDesignerControlDeclMarker { get { return "' <WizardGeneratedCode>MenuDesigner ControlDecl</WizardGeneratedCode>"; } }

    protected override string MenuDesignerControlInitMarker { get { return "' <WizardGeneratedCode>MenuDesigner ControlInit</WizardGeneratedCode>"; } }

    protected override string MenuDesignerBeforeSuspendLayout { get { return "' <WizardGeneratedCode>MenuDesigner BeforeSuspendLayout</WizardGeneratedCode>"; } }

    protected override void AddMenuEntries(VSProject vsProj, List<string> formNames, List<string> tableNames)
    {
      ProjectItem item = FindProjectItem(vsProj.Project.ProjectItems, "frmMain.vb");
      ProjectItem itemDesigner = FindProjectItem(item.ProjectItems, "frmMain.Designer.vb");

      string path = (string)(item.Properties.Item("FullPath").Value);
      WriteMenuEntries(path, formNames);

      path = (string)(itemDesigner.Properties.Item("FullPath").Value);
      WriteMenuDesignerEntries(path, formNames, tableNames);
    }

    protected override void WriteMenuHandler(StreamWriter sw, string formName)
    {
      sw.WriteLine("");
      sw.WriteLine("    Private Sub {0}ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles {0}ToolStripMenuItem.Click", formName);
      sw.WriteLine("      Dim f As {0} = New {0}()", formName);
      sw.WriteLine("      f.Show()");
      sw.WriteLine("    End Sub");
    }

    protected override void WriteMenuStripConstruction(StreamWriter sw, string formName)
    {
      sw.WriteLine("      Me.{0}ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()", formName);
    }

    protected override void WriteMenuAddRange(StreamWriter sw, string formName)
    {
      sw.Write("      Me.{0}ToolStripMenuItem ", formName);
    }

    protected override void WriteMenuControlInit(StreamWriter sw, string formName, string tableName)
    {
      sw.WriteLine("      ' ");
      sw.WriteLine("      ' {0}ToolStripMenuItem", formName);
      sw.WriteLine("      ' ");
      sw.WriteLine("      Me.{0}ToolStripMenuItem.Name = \"{0}ToolStripMenuItem\"", formName);
      sw.WriteLine("      Me.{0}ToolStripMenuItem.Size = New System.Drawing.Size(152, 22)", formName);
      sw.WriteLine("      Me.{0}ToolStripMenuItem.Text = \"{1}\"", formName, tableName);
    }

    protected override void WriteAddRangeBegin(StreamWriter sw)
    {
      sw.WriteLine("      Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.formsToolStripMenuItem })");
      sw.WriteLine("      ' ");
      sw.WriteLine("      ' formsToolStripMenuItem");
      sw.WriteLine("      ' ");
      sw.WriteLine("      Me.formsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() { ");
    }

    protected override void WriteAddRangeEnd(StreamWriter sw)
    {
      sw.WriteLine("      })");
      sw.WriteLine("      Me.formsToolStripMenuItem.Name = \"formsToolStripMenuItem\"");
      sw.WriteLine("      Me.formsToolStripMenuItem.Size = New System.Drawing.Size(52, 20)");
      sw.WriteLine("      Me.formsToolStripMenuItem.Text = \"Forms\"");
    }

    protected override void WriteMenuDeclaration(StreamWriter sw, string formName)
    {
      sw.WriteLine("    Friend WithEvents {0}ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem", formName);
    }

    internal override void RemoveTemplateForm(VSProject proj)
    {
      ProjectItem pi = proj.Project.ProjectItems.Item("Form1.vb");
      pi.Remove();
      File.Delete(Path.Combine(ProjectPath, "Form1.vb"));
      File.Delete(Path.Combine(ProjectPath, "Form1.Designer.vb"));
      File.Delete(Path.Combine(ProjectPath, "Form1.resx"));
    }
  }
}
