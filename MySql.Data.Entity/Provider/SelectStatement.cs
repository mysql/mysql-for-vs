using System.Collections.Generic;
using System.Text;
using System;

namespace MySql.Data.Entity
{
    class SelectStatement : BaseStatement 
    {
        public SelectStatement(SelectStatement parent)
        {
            Parent = parent;
            Where = new List<SqlFragment>();
        }

        private SelectStatement Parent { get; set; }
        public SqlFragment Input  { get; set; }
        public SqlFragment Output { get; set; }
        public List<SqlFragment> Where { get; private set; }
        public SqlFragment Limit { get; set; }
        public SqlFragment Skip { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sql = new StringBuilder();
            if (Parent != null)
                sql.Append("(");
            sql.AppendFormat("SELECT {0}", Output.GenerateSQL());
            if (Input != null)
                sql.AppendFormat(" FROM {0}", Input.GenerateSQL());
            if (Where.Count > 0)
            {
                sql.Append(" WHERE ");
                string seperator = "";
                foreach (SqlFragment f in Where)
                {
                    sql.AppendFormat("{0} {1}", seperator, f.GenerateSQL());
                    seperator = " AND";
                }
            }
            if (Limit != null || Skip != null)
            {
                sql.Append(" LIMIT ");
                if (Skip != null)
                    sql.AppendFormat("{0},", Skip);
                if (Limit == null)
                    sql.Append("18446744073709551615");
                else
                    sql.AppendFormat("{0}", Limit);
            }
            if (Parent != null)
                sql.AppendFormat(") AS {0}", QuoteIdentifier(Name));
            return sql.ToString();
        }
    }
}
