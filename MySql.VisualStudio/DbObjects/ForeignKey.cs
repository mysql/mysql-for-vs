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
            SetName(String.Format("FK_{0}_{0}", t.Name), true);
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
        public bool NameSet { get; set; }

        public void SetName(string name, bool makeUnique)
        {
            string proposedName = name;
            int uniqueIndex = 0;

            if (makeUnique)
            {
                while (true)
                {
                    bool found = false;
                    foreach (ForeignKey k in Table.ForeignKeys)
                        if (k.Name == proposedName)
                        {
                            found = true;
                            break;
                        }
                    if (!found) break;
                    proposedName = String.Format("{0}_{1}", name, ++uniqueIndex);
                }
            }
            Name = proposedName;
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
