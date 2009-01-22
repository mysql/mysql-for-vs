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

using System.Text;
using System.Collections.Generic;
namespace MySql.Data.Entity
{
    class UpdateStatement : SqlFragment 
    {
        public UpdateStatement()
        {
            Properties = new List<SqlFragment>();
            Values = new List<SqlFragment>();
        }

        public SqlFragment Target { get; set; }
        public List<SqlFragment> Properties { get; private set; }
        public List<SqlFragment> Values { get; private set; }
        public SqlFragment Where { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE {0} SET ", Target);
            string seperator = "";
            for (int i = 0; i < Properties.Count; i++)
            {
                sb.AppendFormat("{0}{1}={2}", seperator, Properties[i], Values[i]);
                seperator = ", ";
            }
            if (Where != null)
                sb.AppendFormat(" WHERE {0}", Where);

            return sb.ToString();
        }
    }
}
