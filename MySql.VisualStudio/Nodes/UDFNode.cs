using System;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.Editors;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio
{
	class UDFNode : BaseNode
	{

		public UDFNode(DataViewHierarchyAccessor hierarchyAccessor, int itemId) : 
			base(hierarchyAccessor, itemId)
		{
            NodeId = "UDF";
		}

        public override string GetDropSQL()
        {
            return String.Format("DROP FUNCTION `{0}`.`{1}`", Database, Name);
        }

		public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor)
		{
            UDFNode node = new UDFNode(HierarchyAccessor, 0);
            node.Edit();
		}

        public override void Edit()
        {
            UDFEditor editor = new UDFEditor();
            DialogResult result = editor.ShowDialog();
            if (result == DialogResult.Cancel) return;

            string sql = "CREATE {0} FUNCTION {1} RETURNS {2} SONAME '{3}'";
            sql = String.Format(sql, editor.Aggregate ? "AGGREGATE" : "",
                editor.FunctionName, editor.ReturnTypeByName, editor.LibraryName);

            Name = editor.FunctionName;
            ExecuteSQL(sql);
            SaveNode();
        }
	}
}
