using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DbObjects
{
	internal class CharacterSetTypeConverter : StringConverter 
	{
        private List<string> charSets;

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
            if (charSets == null)
                PopulateList(context.Instance);
            StandardValuesCollection coll = new StandardValuesCollection(charSets);
            return coll;
        }

        private void PopulateList(object instance)
        {
            Table table = (instance is Table) ?
                (instance as Table) : (instance as Column).OwningTable;
            DataTable data = table.OwningNode.GetDataTable("SHOW CHARSET");
            charSets = new List<string>();
            charSets.Add(String.Empty);
            foreach (DataRow row in data.Rows)
                charSets.Add(row["charset"].ToString());
        }
	}
}
