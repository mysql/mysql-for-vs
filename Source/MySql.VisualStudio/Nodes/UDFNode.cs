// Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
      DocumentNode.RegisterNode(this);
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
