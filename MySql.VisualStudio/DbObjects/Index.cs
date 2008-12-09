using System;
using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.VisualStudio.Editors;

namespace MySql.Data.VisualStudio.DbObjects
{
    class Index : Object, ICustomTypeDescriptor
    {
        Table table;
        List<IndexColumn> indexColumns = new List<IndexColumn>();

        public Index(Table t)
        {
            table = t;
        }

        [Category("Identity")]
        [DisplayName("(Name)")]
        [Description("The name of this index/key")]
        public string Name { get; set; }

        [Category("Identity")]
        [Description("A description or comment about this index/key")]
        public string Comment { get; set; }

        [Category("(General)")]
        [Description("The columns of this index/key and their associated sort order")]
        [TypeConverter(typeof(IndexColumnTypeConverter))]
        public List<IndexColumn> Columns
        {
            get { return indexColumns; }
        }

        [Category("(General)")]
        [Description("Specifies if this object is an index or key")]
        public IndexType Type { get; set; }

        [Category("(General)")]
        [DisplayName("Is Unique")]
        [Description("Specifies if this index/key uniquely identifies every row")]
        [TypeConverter(typeof(YesNoTypeConverter))]
        public bool IsUnique { get; set; }

        [Browsable(false)]
        public bool IsPrimary { get; set; }

        [Category("Storage")]
        [DisplayName("Index Algorithm")]
        [Description("Specifies the algorithm that should be used for storing the index/key")]
        public IndexUsingType IndexUsing { get; set; }

        [Category("Storage")]
        [DisplayName("Key Block Size")]
        [Description("Suggested size in bytes to use for index key blocks.  A zero value means to use the storage engine default.")]
        public int KeyBlockSize { get; set; }

        [Description("Specifies a parser plugin to be used for this index/key.  This is only valid for full-text indexes or keys.")]
        public string Parser { get; set; }

        [DisplayName("Is Full-text Index/Key")]
        [Description("Specifies if this is a full-text index or key.  This is only supported on MyISAM tables.")]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public bool FullText { get; set; }

        [DisplayName("Is Spatial Index/Key")]
        [Description("Specifies if this is a spatial index or key.  This is only supported on MyISAM tables.")]
        [TypeConverter(typeof(YesNoTypeConverter))]
        [RefreshProperties(RefreshProperties.All)]
        public bool Spatial { get; set; }

        #region ShouldSerialize

        bool ShouldSerializeName() { return false; }
        bool ShouldSerializeComment() { return false; }
        bool ShouldSerializeColumns() { return false; }
        bool ShouldSerializeType() { return false; }
        bool ShouldSerializeIsUnique() { return false; }
        bool ShouldSerializeIndexUsing() { return false; }
        bool ShouldSerializeKeyBlockSize() { return false; }
        bool ShouldSerializeParser() { return false; }
        bool ShouldSerializeFullText() { return false; }
        bool ShouldSerializeSpatial() { return false; }

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
            PropertyDescriptorCollection coll =
                TypeDescriptor.GetProperties(this, attributes, true);

            List<PropertyDescriptor> props = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor pd in coll)
            {
                if (!pd.IsBrowsable) continue;

                if (pd.Name == "IsUnique")
                {
                    if (IsPrimary)
                    {
                        CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
                        newPd.SetValue(this, true);
                        newPd.SetReadOnly(true);
                        props.Add(newPd);
                    }
                }
                else if (pd.Name == "FullText" && (Spatial ||
                         String.Compare(table.Engine, "myisam", true) != 0))
                {
                    CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
                    newPd.SetReadOnly(true);
                    props.Add(newPd);
                }
                else if (pd.Name == "Spatial" && (FullText ||
                         String.Compare(table.Engine, "myisam", true) != 0))
                {
                    CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
                    newPd.SetReadOnly(true);
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

    enum IndexType
    {
        Index, Key
    }

    enum IndexUsingType
    {
        BTREE, HASH, RTREE
    }

    struct IndexColumn
    {
        public string ColumnName;
        public bool Ascending;
    }
}
