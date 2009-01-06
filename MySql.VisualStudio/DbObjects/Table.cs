using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using MySql.Data.VisualStudio.Properties;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using MySql.Data.VisualStudio.Editors;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.DbObjects
{
    public enum InsertMethod
    {
        No, First, Last
    }

    public enum PackKeysMethod
    {
        Default, None, Full
    }

	internal class Table : ICustomTypeDescriptor
	{
        private TableNode owningNode;
        private string schema;
        private List<Column> columns = new List<Column>();
        private List<Index> indexes = new List<Index>();
        private List<ForeignKey> fkeys = new List<ForeignKey>();
        private bool isNew;

		public Table(TableNode node, DataRow row, DataTable columns)
		{
            owningNode = node;

            // set some defaults that may be overridden with actual table data
            Engine = node.DefaultStorageEngine;
            PackKeys = PackKeysMethod.Default;

            if (row != null)
              ParseTableData(row);
            if (columns != null)
              ParseColumns(columns);
            LoadIndexes();
            LoadForeignKeys();

            schema = node.Database;
            isNew = row == null;
        }

        internal TableNode OwningNode
        {
            get { return owningNode; }
        }

        internal bool SupportsFK
        {
            get
            {
                string engine = Engine.ToLowerInvariant();
                return engine == "innodb" || engine == "falcon";
            }
        }

        #region Table options

        [Category("(Identity)")]
        [MyDescription("TableNameDesc")]
        public string Name { get; set; }

        [Category("(Identity)")]
        [MyDescription("TableSchemaDesc")]
        public string Schema
        {
            get { return schema; }
        }

        [MyDescription("TableCommentDesc")]
        public string Comment { get; set; }

        [Category("Table Options")]
        [DisplayName("Character Set")]
        [TypeConverter(typeof(CharacterSetTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [MyDescription("TableCharSetDesc")]
        public string CharacterSet { get; set; }

        [Category("Table Options")]
        [DisplayName("Collation")]
        [TypeConverter(typeof(CollationTypeConverter))]
        [MyDescription("TableCollationDesc")]
        public string Collation { get; set; }

        [Category("Table")]
        [DisplayName("Auto Increment")]
        [MyDescription("TableAutoIncStartDesc")]
        public ulong AutoInc { get; set; }

        [Browsable(false)]
        public List<Column> Columns
        {
            get { return columns; }
        }

        [Browsable(false)]
        public List<Index> Indexes
        {
            get { return indexes; }
        }

        [Browsable(false)]
        public List<ForeignKey> ForeignKeys
        {
            get { return fkeys; }
        }

        #endregion

        #region Storage options

        [Category("Storage")]
        [DisplayName("Storage Engine")]
        [MyDescription("TableEngineDescription")]
        [TypeConverter(typeof(TableEngineTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public string Engine { get; set; }

        [Category("Storage")]
        [DisplayName("Data Directory")]
        [MyDescription("TableDataDirDesc")]
        public string DataDirectory { get; set; }

        [Category("Storage")]
        [DisplayName("Index Directory")]
        [MyDescription("TableIndexDirDesc")]
        public string IndexDirectory { get; set; }

        #endregion

        #region Row options

        [Category("Row")]
        [DisplayName("Row Format")]
        [DefaultValue(RowFormat.Default)]
        [MyDescription("TableRowFormatDesc")]
        public RowFormat RowFormat { get; set; }

        [Category("Row")]
        [DisplayName("Compute Checksum")]
        [MyDescription("TableCheckSumDesc")]
        [DefaultValue(false)]
        [TypeConverter(typeof(YesNoTypeConverter))]
        public bool CheckSum { get; set; }

        [Category("Row")]
        [DisplayName("Average Row Length")]
        [MyDescription("TableAvgRowLengthDesc")]
        [TypeConverter(typeof(NumericTypeConverter))]
        public ulong AvgRowLength { get; set; }

        [Category("Row")]
        [DisplayName("Minimum Rows")]
        [MyDescription("TableMinRowsDesc")]
        [TypeConverter(typeof(NumericTypeConverter))]
        public ulong MinRows { get; set; }

        [Category("Row")]
        [DisplayName("Maximum Rows")]
        [MyDescription("TableMaxRowsDesc")]
        [TypeConverter(typeof(NumericTypeConverter))]
        public UInt64 MaxRows { get; set; }

        [Category("Row")]
        [DisplayName("Pack Keys")]
        [MyDescription("TablePackKeysDesc")]
        [DefaultValue(PackKeysMethod.Default)]
        public PackKeysMethod PackKeys { get; set; }

        [Category("Row")]
        [DisplayName("Insert method")]
        [MyDescription("TableInsertMethodDesc")]
        [DefaultValue(InsertMethod.First)]
        public InsertMethod InsertMethod { get; set; }

        [Category("Row")]
        [DisplayName("Delay Key Write")]
        [MyDescription("DelayKeyWriteDesc")]
        public bool DelayKeyWrite { get; set; }

        #endregion

        #region ShouldSerializeMethods

        bool ShouldSerializeName() { return false; }
        bool ShouldSerializeSchema() { return false; }
        bool ShouldSerializeComment() { return false; }
        bool ShouldSerializeCharacterSet() { return false; }
        bool ShouldSerializeCollation() { return false; }
        bool ShouldSerializeAutoInc() { return false; }
        bool ShouldSerializeEngine() { return false; }
        bool ShouldSerializeDataDirectory() { return false; }
        bool ShouldSerializeIndexDirectory() { return false; }
        bool ShouldSerializeRowFormat() { return false; }
        bool ShouldSerializeCheckSum() { return false; }
        bool ShouldSerializeAvgRowLength() { return false; }
        bool ShouldSerializeMinRows() { return false; }
        bool ShouldSerializeMaxRows() { return false; }
        bool ShouldSerializePackKeys() { return false; }
        bool ShouldSerializeInsertMethod() { return false; }

        #endregion

        public void DeleteKey(string keyName)
        {
            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                if ((keyName != null && indexes[i].Name == keyName) ||
                    (keyName == null && indexes[i].IsPrimary))
                {
                    indexes.RemoveAt(i);
                    break;
                }
            }
        }

        public Index CreateIndexWithUniqueName(bool primary)
        {
            Index newIndex = new Index(this);
            newIndex.IsPrimary = primary;
            string baseName = String.Format("{0}_{1}", primary ? "PK" : "IX",
                Name);
            string name = baseName;
            int uniqueIndex = 0;
            while (KeyExists(name))
                name = String.Format("{0}_{1}", baseName, ++uniqueIndex);
            newIndex.Name = name;
            return newIndex;
        }

        public List<string> GetColumnNames()
        {
            string sql = @"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                TABLE_SCHEMA='{0}' AND TABLE_NAME='{1}'";
            DataTable dt = owningNode.GetDataTable(String.Format(sql, owningNode.Database, Name));
            List<string> cols =new List<string>();
            foreach (DataRow row in dt.Rows)
                cols.Add(row[0].ToString());
            return cols;
        }

        public string GetSql(Table fromTable)
        {

            StringBuilder sql = new StringBuilder();
            if (isNew)
                sql.AppendFormat("CREATE TABLE `{0}` (", Name);
            else
                sql.AppendFormat("ALTER TABLE `{0}` ", fromTable.Name);

            //            foreach (Column c in Columns)
            //              sql.Append(c.GetSql(), IsNew);

            if (isNew) sql.Append(") ");
            sql.Append(GetTableOptionSql(fromTable));
            return sql.ToString();
        }

        #region Private methods

        private bool KeyExists(string keyName)
        {
            foreach (Index i in indexes)
                if (String.Compare(i.Name, keyName, true) == 0) return true;
            return false;
        }

        private void ParseTableData(DataRow tableRow)
        {
            schema = tableRow["TABLE_SCHEMA"].ToString();
            Name = tableRow["TABLE_NAME"].ToString();
            Engine = tableRow["ENGINE"].ToString();
            RowFormat = (RowFormat)Enum.Parse(typeof(RowFormat), tableRow["ROW_FORMAT"].ToString());
            AvgRowLength = (ulong)tableRow["AVG_ROW_LENGTH"];
            AutoInc = (ulong)tableRow["AUTO_INCREMENT"];   
            Comment = tableRow["TABLE_COMMENT"].ToString();
            Collation = tableRow["TABLE_COLLATION"].ToString();
            if (Collation != null)
            {
                int index = Collation.IndexOf("_");
                if (index != -1)
                    CharacterSet = Collation.Substring(0, index);
            }

            string createOpt = (string)tableRow["CREATE_OPTIONS"];
            if (String.IsNullOrEmpty(createOpt))
                ParseCreateOptions(createOpt.ToLowerInvariant());
        }

        private void ParseCreateOptions(string createOptions)
        {
            string[] options = createOptions.Split(' ');
            foreach (string option in options)
            {
                string[] parts = option.Split('=');
                if (parts.Length != 2) continue;
                switch (parts[0])
                {
                    case "min_rows":
                        MinRows = UInt64.Parse(parts[1]);
                        break;
                    case "max_rows":
                        MaxRows = UInt64.Parse(parts[1]);
                        break;
                    case "checksum":
                        CheckSum = Boolean.Parse(parts[1]);
                        break;
                    case "pack_keys":
                        PackKeys = parts[1] == "1" ? PackKeysMethod.Full : PackKeysMethod.None;
                        break;
                    case "delay_key_write":
                        DelayKeyWrite = parts[1] == "1";
                        break;
                    // data directory
                    // index directory
                    // insert method
                }
            }
        }

        private void ParseColumns(DataTable columnData)
        {
            foreach (DataRow row in columnData.Rows)
            {
                Column c = new Column(row);
                c.OwningTable = this;
                columns.Add(c);
            }
        }

        private void LoadIndexes()
        {
            string[] restrictions = new string[4] { null, owningNode.Database, Name, null };
            DataTable dt = owningNode.GetSchema("Indexes", restrictions);
            foreach (DataRow row in dt.Rows)
            {
                Index i = new Index(this, row);
                indexes.Add(i);
            }
        }

        private void LoadForeignKeys()
        {
            string[] restrictions = new string[4] { null, owningNode.Database, Name, null };
            DataTable dt = owningNode.GetSchema("Foreign Keys", restrictions);
            foreach (DataRow row in dt.Rows)
            {
                ForeignKey key = new ForeignKey(this, row);
                ForeignKeys.Add(key);
            }
        }

        private string GetTableOptionSql(Table fromTable)
        {
            StringBuilder sql = new StringBuilder();
            if (Name != fromTable.Name)
                sql.AppendFormat("RENAME TO `{0}` ", Name);
            if (AvgRowLength != fromTable.AvgRowLength)
                sql.AppendFormat("AVG_ROW_LENGTH={0} ", AvgRowLength);
            if (Comment != fromTable.Comment)
                sql.AppendFormat("COMMENT='{0}' ", Comment);
            if (Engine != fromTable.Engine)
                sql.AppendFormat("ENGINE={0} ", Engine);

            if (MaxRows != fromTable.MaxRows)
                sql.AppendFormat("MAX_ROWS={0} ", MaxRows);
            if (MinRows != fromTable.MinRows)
                sql.AppendFormat("MIN_ROWS={0} ", MinRows);
            if (DataDirectory != fromTable.DataDirectory)
                sql.AppendFormat("DATA DIRECTORY='{0}' ", DataDirectory);
            if (IndexDirectory != fromTable.IndexDirectory)
                sql.AppendFormat("INDEX DIRECTORY='{0}' ", IndexDirectory);

            return sql.ToString();
        }

        #endregion

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

            bool engineIsMyIsam = Engine.ToLowerInvariant() == "myisam";

            foreach (PropertyDescriptor pd in coll)
            {
                if (!pd.IsBrowsable) continue;

                if (pd.Name == "DataDirectory" || pd.Name == "IndexDirectory")
                {
                    CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
                    newPd.SetReadOnly(!engineIsMyIsam);
                    props.Add(newPd);
                }
                else if (pd.Name == "DelayKeyWrite" && !engineIsMyIsam)
                {
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
