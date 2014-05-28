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
  internal class CSharpEntityFrameworkDataGridStrategy : CSharpDataGridStrategy
  {
    internal CSharpEntityFrameworkDataGridStrategy(StrategyConfig config)
      : base(config)
    {
    }

    protected override void WriteUsingUserCode()
    {
      if (DataAccessTech == DataAccessTechnology.EntityFramework5)
      {
        Writer.WriteLine("using System.Data.Objects;");
      }
      else if (DataAccessTech == DataAccessTechnology.EntityFramework6)
      {
        Writer.WriteLine("using System.Data.Entity.Core.Objects;");
      }
    }

    protected override void WriteFormLoadCode()
    {
      Writer.WriteLine("ctx = new Model1Entities();");
      Writer.WriteLine("ObjectResult<{0}> _entities = ctx.{0}.Execute(MergeOption.AppendOnly);", CanonicalTableName);
      Writer.WriteLine("{0}BindingSource.DataSource = _entities;", CanonicalTableName);

      Writer.WriteLine("bindingNavigatorAddNewItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorCountItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorDeleteItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveFirstItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMovePreviousItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorPositionItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveNextItem.Enabled = false;");
      Writer.WriteLine("bindingNavigatorMoveLastItem.Enabled = false;");
      Writer.WriteLine("toolStripButton1.Enabled = true;");

      WriteDataGridColumnInitialization();
      Writer.WriteLine("dataGridView1.DataSource = _entities;");
    }

    protected override void WriteVariablesUserCode()
    {
      Writer.WriteLine("private Model1Entities ctx;");
    }

    protected override void WriteSaveEventCode()
    {
      Writer.WriteLine("{0}BindingSource.EndEdit();", CanonicalTableName);
      Writer.WriteLine("ctx.SaveChanges();");
    }

    protected override void WriteDesignerControlDeclCode()
    {
      Writer.WriteLine("private System.Windows.Forms.BindingSource {0}BindingSource;", CanonicalTableName);
      Writer.WriteLine("private System.Windows.Forms.DataGridView dataGridView1;");
    }

    protected override void WriteDesignerControlInitCode()
    {
      Writer.WriteLine("this.bindingNavigator1.BindingSource = this.{0}BindingSource;", CanonicalTableName);
      Writer.WriteLine("this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;");
      Writer.WriteLine("this.dataGridView1.Location = new System.Drawing.Point(9, 37);");
      Writer.WriteLine("this.dataGridView1.Name = \"dataGridView1\"; ");
      Writer.WriteLine("this.dataGridView1.Size = new System.Drawing.Size(339, 261);");
      Writer.WriteLine("this.dataGridView1.TabIndex = 0;");
      if (ValidationsEnabled)
      {
        Writer.WriteLine("this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);");
      }
      Writer.WriteLine("this.Controls.Add(this.dataGridView1);");
    }

    protected override void WriteDesignerBeforeSuspendCode()
    {
      Writer.WriteLine("this.dataGridView1 = new System.Windows.Forms.DataGridView();");
      Writer.WriteLine("this.{0}BindingSource = new System.Windows.Forms.BindingSource(this.components);", CanonicalTableName);
    }

    protected override void WriteDesignerAfterSuspendCode()
    {
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).BeginInit();", CanonicalTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();");
    }

    protected override void WriteBeforeResumeSuspendCode()
    {
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.{0}BindingSource)).EndInit();", CanonicalTableName);
      Writer.WriteLine("((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();");
    }

    protected override void WriteControlInitialization(bool addBindings)
    {
      // Nothing
    }
  }
}
