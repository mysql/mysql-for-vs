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

namespace MySql.Data.VisualStudio
{
	class TableNode : DocumentNode
	{
        private Table table;
        private bool isDirty;

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

                return table.HasChanges(); // if (!isDirty)
                    //isDirty = table.HasChanges();
                //return isDirty;
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

		public override void ExecuteCommand(int command)
		{
    		Edit();
		}

		public override object GetEditor()
		{
            return new TableEditorPane(this);
		}

    }
}
