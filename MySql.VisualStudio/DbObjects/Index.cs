using System.ComponentModel;
using System.Collections.Generic;
using MySql.Data.VisualStudio.Editors;

namespace MySql.Data.VisualStudio.DbObjects
{
    class Index
    {
        [Category("Identity")]
        [DisplayName("(Name)")]
        [Description("The name of this index/key")]
        public string Name { get; set; }

        [Category("Identity")]
        [Description("A description or comment about this index/key")]
        public string Comment { get; set; }

        [Category("(General)")]
        [Description("The columns of this index/key and their associated sort order")]
        public List<Column> Columns { get; set; }

        [Category("(General)")]
        [Description("Specifies if this object is an index or key")]
        public IndexType Type { get; set; }

        [Category("(General)")]
        [DisplayName("Is Unique")]
        [Description("Specifies if this index/key uniquely identifies every row")]
        [TypeConverter(typeof(YesNoTypeConverter))]
        public bool IsUnique { get; set; }

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
        public bool FullText { get; set; }

        [DisplayName("Is Spatial Index/Key")]
        [Description("Specifies if this is a spatial index or key.  This is only supported on MyISAM tables.")]
        [TypeConverter(typeof(YesNoTypeConverter))]
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
    }

    enum IndexType
    {
        Index, Key
    }

    enum IndexUsingType
    {
        BTREE, HASH, RTREE
    }
}
