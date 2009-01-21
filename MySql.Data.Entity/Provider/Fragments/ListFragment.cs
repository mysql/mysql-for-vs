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

namespace MySql.Data.Entity
{
    class ListFragment : SqlFragment 
    {
        public ListFragment(string sep)
        {
            Items = new List<SqlFragment>();
            Seperator = sep;
        }

        public List<SqlFragment> Items { get; private set; }
        public string Seperator { get; set; }

        public override string GenerateSQL()
        {
            string seperator = "";
            StringBuilder sb = new StringBuilder();

            foreach (SqlFragment f in Items)
            {
                sb.AppendFormat("{0}{1}", seperator, f.GenerateSQL());
                seperator = Seperator;
            }
            return sb.ToString();
        }
    }
}
