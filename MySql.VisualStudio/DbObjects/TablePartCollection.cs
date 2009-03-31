using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.DbObjects
{
    internal interface ITablePart
    {
        string GetDropSql();
        string GetSql(bool isNew);
        bool HasChanges();
        bool IsNew();
        void Saved();
    }

    internal class TablePartCollection<T> : List<T>
    {
        public TablePartCollection()
        {
            Deleted = new List<T>();
        }

        public List<T> Deleted { get; private set; }

        public void Delete(T t)
        {
            Deleted.Add(t);
        }

        public void Delete(int index)
        {
            ITablePart part = (ITablePart)this[index];
            RemoveAt(index);
            if (!part.IsNew())
                Deleted.Add((T)part);
        }

        public bool HasChanges()
        {
            if (Deleted.Count > 0) return true;
            foreach (ITablePart part in this)
                if (part.HasChanges()) return true;
            return false;
        }

        public void Saved()
        {
            Deleted.Clear();
            foreach (ITablePart part in this)
                part.Saved();
        }

        public string GetSql(bool newTable)
        {
            List<string> parts = new List<string>();

            if (!newTable)
                foreach (ITablePart part in Deleted)
                    parts.Add(part.GetDropSql());

            // process new columns
            for (int i = 0; i < Count; i++)
            {
                ITablePart part = (ITablePart)this[i];
                string partSql = part.GetSql(newTable);
                if (!String.IsNullOrEmpty(partSql))
                {
                    if (part is Column && !newTable)
                    {
                        if (i == 0)
                            partSql += " FIRST";
                        else if (i < Count - 1)
                        {
                            Column c = this[i - 1] as Column;
                            partSql += String.Format(" AFTER {0}", c.ColumnName);
                        }
                    }
                    parts.Add(partSql);
                }
            }

            string delimiter = "";
            StringBuilder sql = new StringBuilder();
            foreach (string s in parts)
            {
                sql.AppendFormat("{0}{1}", delimiter, s);
                delimiter = ", ";
            }
            return sql.ToString();
        }
    }
}
