using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.DbObjects;
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio
{
	class TableNode : DocumentNode
	{
        private Table table;
        private Table originalTable;

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
            get { return !TablesAreEqual(); }
        }

        #endregion

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
                originalTable = new Table(this, null, null);
                table.Name = Name;
                originalTable.Name = Name;
            }
            else
            {
                DbConnection connection = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
                string[] restrictions = new string[4] { null, connection.Database, Name, null };
                DataTable columnsTable = connection.GetSchema("Columns", restrictions);

                DataTable dt = connection.GetSchema("Tables", restrictions);
                table = new Table(this, dt.Rows[0], columnsTable);
                originalTable = new Table(this, dt.Rows[0], columnsTable);

                HierarchyAccessor.Connection.UnlockProviderObject();
            }
		}

        public override string GetSaveSql()
        {
            return Table.GetSql(originalTable);
        }

		public override void ExecuteCommand(int command)
		{
    		Edit();
		}

		public override object GetEditor()
		{
			return new TableEditor(this);
		}

        private bool TablesAreEqual()
        {
            if (table.Columns.Count != originalTable.Columns.Count)
                return false;
            if (!ObjectComparer.AreEqual(table, originalTable))
                return false;
            for (int i = 0; i < table.Columns.Count; i++)
                if (!ObjectComparer.AreEqual(table.Columns[i],
                    originalTable.Columns[i])) return false;
            return true;
        }
    }
}
