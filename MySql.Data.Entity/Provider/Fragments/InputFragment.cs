// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Data.Entity
{
    abstract class InputFragment : SqlFragment 
    {
        // not all input classes will support two inputs but union and join do
        // in cases where only one input is used, Left is it
        public InputFragment Left;
        public InputFragment Right;

        public InputFragment()
        {
        }

        public InputFragment(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool IsWrapped { get; private set; }

        public virtual SqlFragment GetProperty(string propertyName)
        {
            if (Left != null && Left.Name == propertyName) return Left;
            if (Right != null && Right.Name == propertyName) return Right;
            return null;
        }

        public virtual void Wrap(Scope scope)
        {
            IsWrapped = true;

            if (scope == null) return;
            if (Left != null)
                scope.Remove(Left);
            if (Right != null)
                scope.Remove(Right);
        }

        public virtual void WriteInnerSql(StringBuilder sql)
        {
        }

        public override void WriteSql(StringBuilder sql)
        {
            if (IsWrapped)
                sql.Append("(");
            WriteInnerSql(sql);
            if (IsWrapped)
                sql.Append(")");
            if (Name == null) return;
            if (this is TableFragment ||
                (IsWrapped && !(this is JoinFragment)))
                sql.AppendFormat(" AS {0}", QuoteIdentifier(Name));
        }
    }
}
