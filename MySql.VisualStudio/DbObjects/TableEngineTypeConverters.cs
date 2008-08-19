using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.VisualStudio
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
