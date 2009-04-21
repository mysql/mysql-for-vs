// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using System.Text;
using System.Collections.Generic;

namespace MySql.Data.Entity 
{
    class InsertStatement : SqlFragment 
    {
        public InsertStatement()
        {
            Sets = new List<SqlFragment>();
            Values = new List<SqlFragment>();
        }

        public InputFragment Target { get; set; }
        public List<SqlFragment> Sets { get; private set; }
        public List<SqlFragment> Values { get; private set; }
        public SelectStatement ReturningSelect;

        public override void WriteSql(StringBuilder sql)
        {
            sql.Append("INSERT INTO ");
            Target.WriteSql(sql);
            if (Sets.Count > 0)
            {
                sql.Append("(");
                WriteList(Sets, sql);
                sql.Append(")");
            }
            sql.Append(" VALUES ");
            sql.Append("(");
            WriteList(Values, sql);
            sql.Append(")");

            if (ReturningSelect != null)
            {
                sql.Append(";\r\n");
                ReturningSelect.WriteSql(sql);
            }
        }
    }
}
