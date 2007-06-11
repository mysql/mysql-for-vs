// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

using System;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data;

namespace MySql.Data.VisualStudio
{
    class StoredProcedureColumnEnumerator : DataObjectEnumerator
    {
        public override DataReader EnumerateObjects(string typeName, object[] items, 
            object[] restrictions, string sort, object[] parameters)
        {
            DataConnectionWrapper connectionWrapper = new DataConnectionWrapper(Connection);
            string spName = String.Format("{0}.{1}", restrictions[1], restrictions[2]);
            IDataReader reader = connectionWrapper.ExecuteReader(spName, true, CommandBehavior.SchemaOnly);
            DataTable dt = reader.GetSchemaTable();
            reader.Close();

            dt.Columns.Add(new DataColumn("RoutineName", typeof(string)));
            foreach (DataRow row in dt.Rows)
                row["RoutineName"] = restrictions[2];

            return new AdoDotNetDataTableReader(dt);
        }
    }
}
