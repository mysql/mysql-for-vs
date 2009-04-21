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
            if (keyData["MATCH_OPTION"] != DBNull.Value)
                Match = (MatchOption)Enum.Parse(typeof(MatchOption), keyData["MATCH_OPTION"].ToString(), true);
            if (keyData["UPDATE_RULE"] != DBNull.Value)
                UpdateAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                    keyData["UPDATE_RULE"].ToString(), true);
            if (keyData["DELETE_RULE"] != DBNull.Value)
                DeleteAction = (ReferenceOption)Enum.Parse(typeof(ReferenceOption),
                    keyData["DELETE_RULE"].ToString(), true);

            string[] restrictions = new string[4] { null, Table.OwningNode.Database, Table.Name, Name };
            DataTable cols = Table.OwningNode.GetSchema("Foreign Key Columns", restrictions);
            foreach (DataRow row in cols.Rows)
            {
                FKColumnPair colPair = new FKColumnPair();
                colPair.Column = row["COLUMN_NAME"].ToString();
                colPair.ReferencedColumn = row["REFERENCED_COLUMN_NAME"].ToString();
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
            if (oldFk == null)
                oldFk = new ForeignKey(Table);
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
                old.ReferencedColumn = fc.ReferencedColumn;
                old.Column = fc.Column;
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
                    if (ofc.ReferencedColumn == fc.ReferencedColumn && 
                        ofc.Column == fc.Column) break;
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
                if (oldFk != null)
                    sql.AppendFormat("DROP FOREIGN KEY `{0}`, ", oldFk.Name);
                sql.Append("ADD ");
            } 
            sql.AppendFormat("FOREIGN KEY `{0}`", Name);

            sql.Append("(");
            string delimiter = "";
            foreach (FKColumnPair c in Columns)
            {
                sql.AppendFormat("{0}{1}", delimiter, c.Column);
                delimiter = ", ";
            }
            sql.Append(")");
            sql.AppendFormat(" REFERENCES `{0}`(", ReferencedTable);
            delimiter = "";
            foreach (FKColumnPair c in Columns)
            {
                sql.AppendFormat("{0}{1}", delimiter, c.ReferencedColumn);
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
        public string ReferencedColumn { get; set; }
        public string Column { get; set; }
    }
}
