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
namespace MySql.Data.Entity
{
    class DeleteStatement : SqlFragment
    {
        public SqlFragment Target { get; set; }
        public SqlFragment Where { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder("DELETE");
            sb.AppendFormat(" {0} FROM {1}", QuoteIdentifier(Target.Name), 
                Target.GenerateSQL());
            if (Where != null)
                sb.AppendFormat(" WHERE {0}", Where.GenerateSQL());
            return sb.ToString();
        }
    }
}
