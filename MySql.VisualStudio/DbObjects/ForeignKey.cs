using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ForeignKey : Object
    {
        public ForeignKey(Table t)
        {
            Table = t;
            SetName(String.Format("FK_{0}_{0}", t.Name), true);
            Columns = new List<FKColumnPair>();
        }

        public ForeignKey(Table t, DataRow keyData) : this (t)
        {
            Name = keyData["CONSTRAINT_NAME"].ToString();
            ReferencedTable = keyData["REFERENCED_TABLE_NAME"].ToString();
            Match = (MatchOption)Enum.Parse(typeof(MatchOption), keyData["MATCH_OPTION"].ToString());
            UpdateAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                keyData["UPDATE_RULE"].ToString());
            DeleteAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                keyData["DELETE_RULE"].ToString());

            string[] restrictions = new string[4] { null, Table.OwningNode.Database, Table.Name, Name };
            DataTable cols = Table.OwningNode.GetSchema("Foreign Key Columns", restrictions);
            foreach (DataRow row in cols.Rows)
            {
                FKColumnPair colPair = new FKColumnPair();
                colPair.ParentTable = row["COLUMN_NAME"].ToString();
                colPair.ChildTable = row["REFERENCED_COLUMN_NAME"].ToString();
                Columns.Add(colPair);
            }
        }

        private Table Table { get; set; }
        public string Name { get; set; }
        public string ReferencedTable { get; set; }
        public MatchOption Match { get; set; }
        public ReferenceOption UpdateAction { get; set; }
        public ReferenceOption DeleteAction { get; set; }
        public List<FKColumnPair> Columns { get; set; }

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

    class FKColumnPair
    {
        public string ParentTable { get; set; }
        public string ChildTable { get; set; }
    }
}
