using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	class ViewNode : BaseNode
	{
		public ViewNode(DataViewHierarchyAccessor hierarchyAccessor, int id) : 
			base(hierarchyAccessor, id)
		{
            NodeId = "View";
		}

		public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor)
		{
//			ViewNode node = new ViewNode();
//			node.HierarchyAccessor = HierarchyAccessor;
//			node.Create();
//			node.OpenEditor();
		}

//		protected void Load()
//		{
//			if (IsNew) return;

/*			DbConnection connection = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
			string[] restrictions = new string[4] { null, connection.Database, Name, null };
			columnsTable = connection.GetSchema("Columns", restrictions);
			HierarchyAccessor.Connection.UnlockProviderObject();

			columns.Clear();
			foreach (DataRow row in columnsTable.Rows)
			{
				Column c = new Column(row);
				columns.Add(c);
			}*/
//		}

/*		public override object GetEditor()
		{
			return new SqlEditor(this);
		}
*/

	}
}
