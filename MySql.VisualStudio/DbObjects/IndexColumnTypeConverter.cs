using System;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.DbObjects
{
    class IndexColumnTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
            object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                StringBuilder str = new StringBuilder();
                List<IndexColumn> cols = (value as List<IndexColumn>);
                string separator = String.Empty;
                foreach (IndexColumn ic in cols)
                {
                    str.AppendFormat("{2}{0} ({1})", ic.ColumnName,
                        ic.Ascending ? "ASC" : "DESC", separator);
                    separator = ",";
                }
                return str.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
