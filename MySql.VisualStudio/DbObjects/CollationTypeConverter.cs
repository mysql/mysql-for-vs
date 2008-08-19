using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.VisualStudio
{
    internal class CollationTypeConverter : StringConverter
    {
        private DataTable collationData;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            Table table = (context.Instance is Table) ?
                (context.Instance as Table) : (context.Instance as Column).OwningTable;

            if (collationData == null)
                PopulateList(table);
            StandardValuesCollection coll = 
                new StandardValuesCollection(GetRelevantCollations(context.Instance));
            return coll;
        }

        private List<string> GetRelevantCollations(object instance)
        {
            List<string> collations = new List<string>();
            string charset = String.Empty;
            if (instance is Table)
                charset = (instance as Table).CharacterSet;
            else
                charset = (instance as Column).CharacterSet;
            if (String.IsNullOrEmpty(charset)) return collations;

            foreach (DataRow row in collationData.Rows)
                if (row["charset"].Equals(charset))
                    collations.Add(row["collation"].ToString());
            return collations;
        }

        private void PopulateList(Table table)
        {
            collationData = table.OwningNode.GetDataTable("SHOW COLLATION");
        }
    }
}
