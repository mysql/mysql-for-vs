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
using System.IO;
using System.Linq;
using System.Text;
using VSLangProj;
using EnvDTE;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public class CSharpWindowsFormsWizard : WindowsFormsWizard
  {
    public CSharpWindowsFormsWizard() 
      : base( LanguageGenerator.CSharp )
    {
    }

    protected override string MenuEventHandlerMarker { get { return "// <WizardGeneratedCode>Menu EventHandlers</WizardGeneratedCode>"; } }

    protected override string MenuDesignerControlDeclMarker { get { return "// <WizardGeneratedCode>MenuDesigner ControlDecl</WizardGeneratedCode>"; } }

    protected override string MenuDesignerControlInitMarker { get { return "// <WizardGeneratedCode>MenuDesigner ControlInit</WizardGeneratedCode>"; } }

    protected override string MenuDesignerBeforeSuspendLayout { get { return "// <WizardGeneratedCode>MenuDesigner BeforeSuspendLayout</WizardGeneratedCode>"; } }

    protected override void AddMenuEntries(VSProject vsProj, List<string> formNames, List<string> tableNames)
    {
      ProjectItem item = FindProjectItem(vsProj.Project.ProjectItems, "frmMain.cs");
      ProjectItem itemDesigner = FindProjectItem(item.ProjectItems, "frmMain.Designer.cs");

      string path = (string)(item.Properties.Item("FullPath").Value);
      WriteMenuEntries(path, formNames);

      path = (string)(itemDesigner.Properties.Item("FullPath").Value);
      WriteMenuDesignerEntries(path, formNames, tableNames);
    }

    protected override void WriteMenuHandler(StreamWriter sw, string formName)
    {
      sw.WriteLine("");
      sw.WriteLine("    private void {0}ToolStripMenuItem_Click(object sender, EventArgs e)", formName);
      sw.WriteLine("    {");
      sw.WriteLine("      {0} f = new {0}();", formName);
      sw.WriteLine("      f.Show();");
      sw.WriteLine("    }");
    }

    protected override void WriteMenuStripConstruction(StreamWriter sw, string formName)
    {
      sw.WriteLine("      this.{0}ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();", formName);
    }

    protected override void WriteMenuAddRange(StreamWriter sw, string formName)
    {
      sw.Write("      this.{0}ToolStripMenuItem ", formName);
    }

    protected override void WriteMenuControlInit(StreamWriter sw, string formName, string tableName)
    {
      sw.WriteLine("      // ");
      sw.WriteLine("      // {0}ToolStripMenuItem", formName);
      sw.WriteLine("      // " );
      sw.WriteLine("      this.{0}ToolStripMenuItem.Name = \"{0}ToolStripMenuItem\";", formName);
      sw.WriteLine("      this.{0}ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);", formName);
      sw.WriteLine("      this.{0}ToolStripMenuItem.Text = \"{1}\";", formName, tableName);
      sw.WriteLine("      this.{0}ToolStripMenuItem.Click += new System.EventHandler(this.{0}ToolStripMenuItem_Click);", formName);
    }

    protected override void WriteAddRangeBegin(StreamWriter sw)
    {
      sw.WriteLine("      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.formsToolStripMenuItem });");
      sw.WriteLine("      // ");
      sw.WriteLine("      // formsToolStripMenuItem");
      sw.WriteLine("      // ");
      sw.WriteLine("      this.formsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ");
    }

    protected override void WriteAddRangeEnd(StreamWriter sw)
    {
      sw.WriteLine("      });");
      sw.WriteLine("      this.formsToolStripMenuItem.Name = \"formsToolStripMenuItem\";");
      sw.WriteLine("      this.formsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);");
      sw.WriteLine("      this.formsToolStripMenuItem.Text = \"Forms\"; ");
    }

    protected override void WriteMenuDeclaration( StreamWriter sw, string formName)
    {
      sw.WriteLine("    private System.Windows.Forms.ToolStripMenuItem {0}ToolStripMenuItem;", formName);
    }

    internal override void RemoveTemplateForm(VSProject proj)
    {
      ProjectItem pi = proj.Project.ProjectItems.Item("Form1.cs");
      pi.Remove();
      File.Delete(Path.Combine(ProjectPath, "Form1.cs"));
      File.Delete(Path.Combine(ProjectPath, "Form1.Designer.cs"));
      File.Delete(Path.Combine(ProjectPath, "Form1.resx"));
    }
  }
}
