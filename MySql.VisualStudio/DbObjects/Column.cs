using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.DbObjects;
using System.Windows.Forms.Design;

namespace MySql.Data.VisualStudio
{
    internal class Column : ICustomTypeDescriptor
	{
        private Table owningTable;
        private string characterSet;

        public Column()
        {
        }

		public Column(DataRow row)
		{
            if (row != null)
                ParseColumnInfo(row);
		}

        #region Properties

        [Browsable(false)]
        internal Table OwningTable
        {
            get { return owningTable; }
            set { owningTable = value; }
        }

		[Category("General")]
		public string ColumnName { get; set; }

        [Category("General")]
        [DisplayName("Data Type")]
        [TypeConverter(typeof(DataTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public string DataType { get; set; }

        [Category("Options")]
        [DisplayName("Allow Nulls")]
        public bool AllowNull { get; set; }

        [Category("Options")]
        [DisplayName("Is Unsigned")]
        public bool IsUnsigned { get; set; }

        [Category("Options")]
        [DisplayName("Is Zerofill")]
        public bool IsZerofill { get; set; }

		[Category("General")]
		[DisplayName("Default Value")]
		public string DefaultValue { get; set; }

        [Category("Options")]
        [DisplayName("Autoincrement")]
        public bool AutoIncrement { get; set; }

        [Category("Options")]
        [DisplayName("Primary Key")]
        public bool PrimaryKey { get; set; }

        public int Precision { get; set; }
        public int Scale { get; set; }

        [Category("Encoding")]
        [DisplayName("Character Set")]
        [TypeConverter(typeof(CharacterSetTypeConverter))]
        public string CharacterSet
        {
            get { return characterSet; }
            set
            {
                if (value != characterSet)
                    Collation = String.Empty;
                characterSet = value;
            }
        }

        [Category("Encoding")]
        [TypeConverter(typeof(CollationTypeConverter))]
        public string Collation { get; set; }

        [Category("Miscellaneous")]
        public string Comment { get; set; }

		#endregion

/*            dt.Columns.Add("ORDINAL_POSITION", typeof (long));
            dt.Columns.Add("DATA_TYPE", typeof (string));
            dt.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof (long));
            dt.Columns.Add("CHARACTER_OCTET_LENGTH", typeof (long));
            dt.Columns.Add("NUMERIC_PRECISION", typeof (long));
            dt.Columns.Add("NUMERIC_SCALE", typeof (long));
            dt.Columns.Add("COLUMN_TYPE", typeof (string));
            dt.Columns.Add("COLUMN_KEY", typeof (string));
            dt.Columns.Add("EXTRA", typeof (string));
            dt.Columns.Add("PRIVILEGES", typeof (string));*/

        private void ParseColumnInfo(DataRow row)
        {
            ColumnName = row["COLUMN_NAME"].ToString();
            AllowNull = row["IS_NULLABLE"] != DBNull.Value && row["IS_NULLABLE"].ToString() == "YES";
            Comment = row["COLUMN_COMMENT"].ToString();
            Collation = row["COLLATION_NAME"].ToString();
            CharacterSet = row["CHARACTER_SET_NAME"].ToString();
            DefaultValue = row["COLUMN_DEFAULT"].ToString();

/*            string extra = (string)dataRow["EXTRA"];
            if (String.IsNullOrEmpty(extra)) return;

            string columnType = (string)dataRow["COLUMN_TYPE"];

            autoInc = extra.IndexOf("auto_increment") != -1;
            // the following works because zero fill always appears last in
            // the column type
            zeroFill = columnType.EndsWith("zerofill");
            if (zeroFill)
                columnType = columnType.Substring(0, columnType.Length - "zeroFill".Length - 1);
            unsigned = columnType.EndsWith("unsigned"); */
        }

        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(this, attributes, true);

            List<PropertyDescriptor> props = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor pd in coll)
            {
                if (!pd.IsBrowsable) continue;

                if (pd.Name == "Precision" || pd.Name == "Scale")
                {
                    if (DataType != null && DataType.ToLowerInvariant() == "decimal")
                        props.Add(pd);
                }
                else if (pd.Name == "CharacterSet" || pd.Name == "Collation")
                {
                    CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
                    newPd.SetReadOnly(DataType == null || !Metadata.IsStringType(DataType));
                    props.Add(newPd);
                }
                else
                    props.Add(pd);
            }
            return new PropertyDescriptorCollection(props.ToArray());
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return TypeDescriptor.GetProperties(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        #endregion

    }
}
