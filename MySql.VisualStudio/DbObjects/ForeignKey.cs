using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ForeignKey : Object
    {
        public ForeignKey(Table t)
        {
            Table = t;
        }

        private Table Table { get; set; }
        public string Name { get; set; }
        public string ReferencedTable { get; set; }
        public MatchOption Match { get; set; }
        public ReferenceOption UpdateAction { get; set; }
        public ReferenceOption DeleteAction { get; set; }
        List<FKColumnPair> Columns { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    enum MatchOption 
    {
        Full, Partial, Simple
    }

    enum ReferenceOption : int
    {
        NoAction, Cascade, Restrict, SetNull
    }

    struct FKColumnPair
    {
        public string parentTable;
        public string childTable;
    }
}
