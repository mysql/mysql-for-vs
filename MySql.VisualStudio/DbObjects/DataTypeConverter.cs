using MySql.Data.VisualStudio.DbObjects;
using System.ComponentModel;
using System.Collections.Generic;

namespace MySql.Data.VisualStudio
{
	internal class DataTypeConverter : StringConverter 
	{
        private List<string> dataTypes = new List<string>();

        public DataTypeConverter()
        {
            dataTypes.AddRange(Metadata.GetDataTypes(false));
        }

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
            StandardValuesCollection coll = new StandardValuesCollection(dataTypes);
            return coll;
        }
	}
}
