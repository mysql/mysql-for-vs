using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.DbObjects;
using System.Windows.Forms.Design;

namespace MySql.Data.VisualStudio.DbObjects
{
    class Column : Object
	{
        private Table owningTable;
        private string characterSet;
        private string name;
        private string dataType;

        public Column()
        {
        }

		public Column(DataRow row) : this()
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
        public string ColumnName
        {
            get { return name; }
            set { name = value; }
        }

        [Category("General")]
        [DisplayName("Data Type")]
        [TypeConverter(typeof(DataTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public string DataType { 
            get { return dataType; }
            set { dataType = value; }
        }

        [TypeConverter(typeof(YesNoTypeConverter))]
        [Category("Options")]
        [DisplayName("Allow Nulls")]
        public bool AllowNull { get; set; }

        [TypeConverter(typeof(YesNoTypeConverter))]
        [Category("Options")]
        [DisplayName("Is Unsigned")]
        public bool IsUnsigned { get; set; }

        [TypeConverter(typeof(YesNoTypeConverter))]
        [Category("Options")]
        [DisplayName("Is Zerofill")]
        public bool IsZerofill { get; set; }

		[Category("General")]
		[DisplayName("Default Value")]
		public string DefaultValue { get; set; }

        [TypeConverter(typeof(YesNoTypeConverter))]
        [Category("Options")]
        [DisplayName("Autoincrement")]
        public bool AutoIncrement { get; set; }

        [TypeConverter(typeof(YesNoTypeConverter))]
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

        #region Methods needed so PropertyGrid won't bold our values

        private bool ShouldSerializeColumnName() { return false; }
        private bool ShouldSerializeDataType() { return false; }
        private bool ShouldSerializeAllowNull() { return false; }
        private bool ShouldSerializeIsUnsigned() { return false; }
        private bool ShouldSerializeIsZerofill() { return false; }
        private bool ShouldSerializeDefaultValue() { return false; }
        private bool ShouldSerializeAutoIncrement() { return false; }
        private bool ShouldSerializePrimaryKey() { return false; }
        private bool ShouldSerializePrecision() { return false; }
        private bool ShouldSerializeScale() { return false; }
        private bool ShouldSerializeCharacterSet() { return false; }
        private bool ShouldSerializeCollation() { return false; }
        private bool ShouldSerializeComment() { return false; }

        #endregion
    }
}
