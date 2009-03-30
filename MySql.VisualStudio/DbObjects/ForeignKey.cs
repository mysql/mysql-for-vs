using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ForeignKey : Object, ITablePart
    {
        bool isNew;
        ForeignKey oldFk;

        private ForeignKey(Table t)
        {
            Table = t;
            SetName(String.Format("FK_{0}_{0}", t.Name), true);
            Columns = new List<FKColumnPair>();
        }

        public ForeignKey(Table t, DataRow keyData) : this (t)
        {
            isNew = keyData == null;
            oldFk = new ForeignKey(t);
            if (!isNew)
            {
                ParseFKInfo(keyData);
                (this as ITablePart).Saved();
            }
        }

        private void ParseFKInfo(DataRow keyData)
        {
            Name = keyData["CONSTRAINT_NAME"].ToString();
            ReferencedTable = keyData["REFERENCED_TABLE_NAME"].ToString();
            Match = (MatchOption)Enum.Parse(typeof(MatchOption), keyData["MATCH_OPTION"].ToString(), true);
            UpdateAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                keyData["UPDATE_RULE"].ToString(), true);
            DeleteAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                keyData["DELETE_RULE"].ToString(), true);

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

        #region ITablePart Members

        void ITablePart.Saved()
        {
            // copy over the top level properties
            oldFk.DeleteAction = DeleteAction;
            oldFk.Match = Match;
            oldFk.Name = Name;
            oldFk.ReferencedTable = ReferencedTable;
            oldFk.Table = Table;
            oldFk.UpdateAction = UpdateAction;

            // now we need to copy the columns
            oldFk.Columns.Clear();
            foreach (FKColumnPair fc in Columns)
            {
                FKColumnPair old = new FKColumnPair();
                old.ParentTable = fc.ParentTable;
                old.ChildTable = fc.ChildTable;
                oldFk.Columns.Add(old);
            }
        }

        bool ITablePart.HasChanges()
        {
            if (!ObjectHelper.AreEqual(this, oldFk)) return true;

            if (Columns.Count != oldFk.Columns.Count) return true;
            foreach (FKColumnPair fc in Columns)
            {
                int i = 0;
                for (; i < oldFk.Columns.Count; i++)
                {
                    FKColumnPair ofc = oldFk.Columns[i];
                    if (ofc.ParentTable == fc.ParentTable && 
                        ofc.ChildTable == fc.ChildTable) break;
                }
                if (i == oldFk.Columns.Count) return true;
            }
            return false;
        }

        string ITablePart.GetDropSql()
        {
            return String.Format("DROP FOREIGN KEY `{0}`", Name);
        }

        string ITablePart.GetSql(bool newTable)
        {
            // if we don't have any changes then just return null
            if (!(this as ITablePart).HasChanges()) return null;

            StringBuilder sql = new StringBuilder();
            if (!newTable)
            {
                if (!String.IsNullOrEmpty(oldFk.Name))
                    sql.AppendFormat("DROP FOREIGN KEY `{0}`, ", oldFk.Name);
                sql.Append("ADD ");
            } 
            sql.AppendFormat("FOREIGN KEY '{0}'", Name);

            sql.Append("(");
            string delimiter = "";
            foreach (FKColumnPair c in Columns)
            {
                sql.AppendFormat("{0}{1}", delimiter, c.ChildTable);
                delimiter = ", ";
            }
            sql.Append(")");
            sql.Append(" REFERENCES (");
            delimiter = "";
            foreach (FKColumnPair c in Columns)
            {
                sql.AppendFormat("{0}{1}", delimiter, c.ParentTable);
                delimiter = ", ";
            }
            sql.Append(")");
            
            return sql.ToString();
        }

        bool ITablePart.IsNew()
        {
            return isNew;
        }

        #endregion
    }

    enum MatchOption 
    {
        Full, Partial, Simple, None
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
