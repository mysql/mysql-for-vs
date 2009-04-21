// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.DbObjects;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Editors;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.Data.VisualStudio
{
	class TableNode : DocumentNode
	{
        private Table table;

		public TableNode(DataViewHierarchyAccessor hierarchyAccessor, int id) : 
			base(hierarchyAccessor, id)
		{
            NodeId = "Table";
            //commandGroupGuid = GuidList.DavinciCommandSet;
        }

        #region Properties

        public Table Table
        {
            get { return table; }
        }

        public override bool Dirty
        {
            get
            {
                Debug.Assert(table != null);

                return table.HasChanges();
            }
        }

        #endregion

        protected override bool Save()
        {
            if (table.IsNew && table.Name == Name)
            {
                TableNamePromptDialog dlg = new TableNamePromptDialog();
                dlg.TableName = table.Name;
                if (DialogResult.Cancel == dlg.ShowDialog()) return false;
                table.Name = dlg.TableName;
                Name = table.Name;
                HierarchyAccessor.SetProperty(ItemId, (int)__VSHPROPID.VSHPROPID_Name, Name);
                //TODO: refresh open editor window
            }

            return base.Save();
        }

        public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor)
		{
            TableNode node = new TableNode(HierarchyAccessor, 0);
            node.Edit();
		}

		protected override void Load()
		{
            if (IsNew)
            {
                table = new Table(this, null, null);
                table.Name = Name;
            }
            else
            {
                DbConnection connection = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
                string[] restrictions = new string[4] { null, connection.Database, Name, null };
                DataTable columnsTable = connection.GetSchema("Columns", restrictions);

                DataTable dt = connection.GetSchema("Tables", restrictions);
                table = new Table(this, dt.Rows[0], columnsTable);

                HierarchyAccessor.Connection.UnlockProviderObject();
            }
		}

        public override string GetSaveSql()
        {
            return Table.GetSql();
        }

		public override object GetEditor()
		{
            return new TableEditorPane(this);
		}

    }
}
