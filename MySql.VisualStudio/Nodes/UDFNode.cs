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
