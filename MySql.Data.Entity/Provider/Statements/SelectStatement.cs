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
    class SelectStatement : InputFragment
    {
        private List<SqlFragment> output;

        public SelectStatement(SelectStatement parent)
        {
            Parent = parent;
            Output = new ListFragment("," + Environment.NewLine);
            Where = new List<SqlFragment>();
            OrderBy = new List<SqlFragment>();
        }

        public SelectStatement Parent { get; set; }
        public InputFragment Input { get; set; }
        public ListFragment Output { get; set; }

        public List<SqlFragment> Where { get; private set; }
        public SqlFragment Limit { get; set; }
        public SqlFragment Skip { get; set; }
        public List<SqlFragment> OrderBy { get; private set; }

        protected override string InnerText
        {
            get
            {
                return GenerateSql();
            }
        }

        private string GenerateSql()
        {
            StringBuilder sql = new StringBuilder();
            if (Parent != null)
                sql.Append("(");
            sql.AppendFormat("SELECT {0}", Output);
            if (Input != null)
                sql.AppendFormat(" FROM {0}", Input);
            if (Where.Count > 0)
            {
                sql.Append(" WHERE ");
                string seperator = "";
                foreach (SqlFragment f in Where)
                {
                    sql.AppendFormat("{0}{1}", seperator, f);
                    seperator = " AND ";
                }
            }

            // now do the sorting
            if (OrderBy.Count > 0)
            {
                string delimiter = "";
                sql.Append(" ORDER BY ");
                foreach (SqlFragment fragment in OrderBy)
                {
                    sql.AppendFormat("{0}{1}", delimiter, fragment);
                    delimiter = ",";
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
            return sql.ToString();
        }
    }
}
