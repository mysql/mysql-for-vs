// Copyright (C) 2008-2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

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
                sql.Append(")");
            if (Name != null)
                sql.AppendFormat("AS {0}", QuoteIdentifier(Name));
            return sql.ToString();
        }
    }
}
