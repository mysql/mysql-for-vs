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
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DbObjects
{
    internal class TableEngineTypeConverter : StringConverter
    {
        private List<string> engineList;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Debug.Assert(context.Instance is Table);
            Table table = context.Instance as Table;

            if (engineList == null)
                PopulateList(table);
            StandardValuesCollection coll = new StandardValuesCollection(engineList);
            return coll;
        }

        private void PopulateList(Table table)
        {
            engineList = new List<string>();
            DataTable data = table.OwningNode.GetDataTable("SHOW ENGINES");

            foreach (DataRow row in data.Rows)
            {
                if (!row[1].Equals("NO"))
                    engineList.Add(row[0].ToString());
            }
        }
    }
}
