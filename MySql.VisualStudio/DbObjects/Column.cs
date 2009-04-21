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
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.DbObjects;
using System.Windows.Forms.Design;

namespace MySql.Data.VisualStudio.DbObjects
{
    internal class Column : Object, ITablePart
	{
        private string characterSet;
        internal Column OldColumn;
        private bool isNew;

        private Column()
        {
            AllowNull = true;
        }

		public Column(DataRow row) : this()
		{
            isNew = row == null;
            if (row != null)
                ParseColumnInfo(row);
            OldColumn = new Column();
            ObjectHelper.Copy(this, OldColumn);
		}

        #region Properties

        [Browsable(false)]
        internal Table OwningTable { get; set; }

        [Category("General")]
        [Description("The name of this column")]
        public string ColumnName { get; set; }

        [Category("General")]
        [DisplayName("Data Type")]
        [TypeConverter(typeof(DataTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public string DataType { get; set; }

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

        //[TypeConverter(typeof(YesNoTypeConverter))]
        //[Category("Options")]
        //[DisplayName("Primary Key")]
        //[RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
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

        private void ParseColumnInfo(DataRow row)
        {
            ColumnName = row["COLUMN_NAME"].ToString();
            AllowNull = row["IS_NULLABLE"] != DBNull.Value && row["IS_NULLABLE"].ToString() == "YES";
            Comment = row["COLUMN_COMMENT"].ToString();
            Collation = row["COLLATION_NAME"].ToString();
            CharacterSet = row["CHARACTER_SET_NAME"].ToString();
            DefaultValue = row["COLUMN_DEFAULT"].ToString();

            string columnType = row["COLUMN_TYPE"].ToString().ToLowerInvariant();
            int index = columnType.IndexOf(' ');
            if (index == -1)
                index = columnType.Length;
            DataType = columnType.Substring(0, index);
            CleanDataType();

            columnType = columnType.Substring(index);
            IsUnsigned = columnType.IndexOf("unsigned") != -1;
            IsZerofill = columnType.IndexOf("zerofill") != -1;

            PrimaryKey = row["COLUMN_KEY"].ToString() == "PRI";
            Precision = DataRowHelpers.GetValueAsInt32(row, "NUMERIC_PRECISION");
            Scale = DataRowHelpers.GetValueAsInt32(row, "NUMERIC_SCALE");

            string extra = row["EXTRA"].ToString().ToLowerInvariant();
            if (extra != null)
                AutoIncrement = extra.IndexOf("auto_increment") != -1;
        }

        private void CleanDataType()
        {
            if (DataType.Contains("char") || DataType.Contains("binary")) return;
            int index = DataType.IndexOf("(");
            if (index == -1) return;
            DataType = DataType.Substring(0, index);
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

        #region ITablePart Members

        void ITablePart.Saved()
        {
            ObjectHelper.Copy(this, OldColumn);
        }

        bool ITablePart.HasChanges()
        {
            return !ObjectHelper.AreEqual(this, OldColumn);
        }

        bool ITablePart.IsNew()
        {
            return isNew;
        }

        string ITablePart.GetDropSql()
        {
            return String.Format("DROP `{0}`", ColumnName);
        }

        string ITablePart.GetSql(bool newTable)
        {
            if (OldColumn != null &&
                OldColumn.ColumnName != null &&
                ObjectHelper.AreEqual(this, OldColumn))
                return null;

            if (String.IsNullOrEmpty(ColumnName)) return null;

            StringBuilder props = new StringBuilder();
            int changes = 0;

            if (DataType != OldColumn.DataType)
                props.AppendFormat(" {0}", DataType);
            if (CharacterSet != OldColumn.CharacterSet)
                props.AppendFormat(" CHARACTER SET '{0}'", CharacterSet);
            if (Collation != OldColumn.Collation)
                props.AppendFormat(" COLLATE '{0}'", Collation);
            if (AllowNull != OldColumn.AllowNull) 
                props.Append(AllowNull ? " NULL" : " NOT NULL");
            if (IsUnsigned != OldColumn.IsUnsigned)
            {
                changes++;
                if (IsUnsigned) props.Append(" UNSIGNED");
            }
            if (IsZerofill != OldColumn.IsZerofill)
            {
                changes++;
                if (IsZerofill) props.Append(" ZEROFILL");
            }
            if (AutoIncrement != OldColumn.AutoIncrement)
            {
                changes++;
                if (AutoIncrement) props.Append(" AUTO_INCREMENT");
            }
            if (DefaultValue != OldColumn.DefaultValue) 
                props.AppendFormat(" DEFAULT '{0}'", DefaultValue);
            if (Comment != OldColumn.Comment) 
                props.AppendFormat(" COMMENT '{0}'", Comment);

            if (props.Length == 0 && 
                changes == 0 && 
                ColumnName.ToLowerInvariant()  == OldColumn.ColumnName.ToLowerInvariant())
                return null;

            if (newTable)
                return String.Format("`{0}`{1}", ColumnName, props.ToString());
            if (isNew)
                return String.Format("ADD `{0}`{1}", ColumnName, props.ToString());
            return String.Format("CHANGE `{0}` `{1}` {2}{3}", 
                OldColumn.ColumnName, ColumnName, DataType, props.ToString());
        }

        #endregion
    }
}
