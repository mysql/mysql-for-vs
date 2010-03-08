// Copyright (c) 2008 MySQL AB, 2008-2010 Sun Microsystems, Inc.
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
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.Entity
{
    internal abstract class SqlFragment
    {
        protected string QuoteIdentifier(string id)
        {
            return String.Format("`{0}`", id);
        }

        public abstract void WriteSql(StringBuilder sql);

        public override string ToString()
        {
            StringBuilder sqlText = new StringBuilder();
            WriteSql(sqlText);
            return sqlText.ToString();
        }

        protected void WriteList(IEnumerable list, StringBuilder sql)
        {
            string sep = "";
            foreach (SqlFragment s in list)
            {
                sql.Append(sep);
                sql.Append("\r\n");
                s.WriteSql(sql);
                sep = ", ";
            }
        }
    }

    internal class BinaryFragment : NegatableFragment
    {
        public SqlFragment Left;
        public SqlFragment Right;
        public string Operator;
        public bool WrapLeft;
        public bool WrapRight;

        public override void WriteSql(StringBuilder sql)
        {
            if (IsNegated)
                sql.Append("NOT (");

            // do left arg
            if (WrapLeft)
                sql.Append("(");
            Left.WriteSql(sql);
            if (WrapLeft)
                sql.Append(")");

            sql.AppendFormat(" {0} ", Operator);

            // now right arg
            if (WrapRight)
                sql.Append("(");
            Right.WriteSql(sql);
            if (WrapRight)
                sql.Append(")");
            if (IsNegated)
                sql.Append(")");
        }
    }

    internal class CaseFragment : SqlFragment
    {
        public List<SqlFragment> When = new List<SqlFragment>();
        public List<SqlFragment> Then = new List<SqlFragment>();
        public SqlFragment Else = null;

        public override void WriteSql(StringBuilder sql)
        {
            sql.Append("CASE");
            for (int i = 0; i < When.Count; i++)
            {
                sql.Append(" WHEN (");
                When[i].WriteSql(sql);
                sql.Append(") THEN (");
                Then[i].WriteSql(sql);
                sql.Append(") ");
            }
            if (Else != null)
            {
                sql.Append(" ELSE (");
                Else.WriteSql(sql);
                sql.Append(") ");
            }
            sql.Append("END");
        }
    }

    internal class ColumnFragment : SqlFragment
    {
        public ColumnFragment(string table, string name)
        {
            TableAlias = TableName = table;
            ColumnName = name;
        }

        public SqlFragment Literal { get; set; }
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public string ColumnName { get; set; }
        public string ColumnAlias { get; set; }

        public override void WriteSql(StringBuilder sql)
        {
            if (Literal != null)
            {
                Debug.Assert(ColumnAlias != null);
                Literal.WriteSql(sql);
            }
            else
            {
                if (TableAlias != null)
                    sql.AppendFormat("{0}.", QuoteIdentifier(TableAlias));
                sql.AppendFormat("{0}", QuoteIdentifier(ColumnName));
            }
            if (ColumnAlias != null && ColumnAlias != ColumnName)
                sql.AppendFormat(" AS {0}", QuoteIdentifier(ColumnAlias));
        }
    }

    internal class ExistsFragment : NegatableFragment
    {
        public SqlFragment Argument;

        public ExistsFragment(SqlFragment f)
        {
            Argument = f;
        }

        public override void WriteSql(StringBuilder sql)
        {
            sql.Append(IsNegated ? "NOT " : "");
            sql.Append("EXISTS(");
            Argument.WriteSql(sql);
            sql.Append(")");
        }

    }

    internal class FunctionFragment : SqlFragment
    {
        public bool Distinct;
        public SqlFragment Argmument;
        public string Name;
        public bool Quoted;

        public override void WriteSql(StringBuilder sql)
        {
            string name = Quoted ? QuoteIdentifier(Name) : Name;
            sql.AppendFormat("{0}({1}", name, Distinct ? "DISTINCT " : "");
            Argmument.WriteSql(sql);
            sql.Append(")");
        }
    }

    internal class IsNullFragment : NegatableFragment
    {
        public SqlFragment Argument;

        public override void WriteSql(StringBuilder sql)
        {
            Argument.WriteSql(sql);
            sql.AppendFormat(" IS {0} NULL", IsNegated ? "NOT" : "");
        }
    }

    internal class LikeFragment : NegatableFragment
    {
        public SqlFragment Argument;
        public SqlFragment Pattern;
        public SqlFragment Escape;

        public override void WriteSql(StringBuilder sql)
        {
            Argument.WriteSql(sql);
            if (IsNegated)
                sql.Append(" NOT ");
            sql.Append(" LIKE ");
            Pattern.WriteSql(sql);
            if (Escape != null)
            {
                sql.Append(" ESCAPE ");
                Escape.WriteSql(sql);
            }
        }
    }

    internal class ListFragment : SqlFragment
    {
        public List<SqlFragment> Fragments = new List<SqlFragment>();

        public void Append(string s)
        {
            Fragments.Add(new LiteralFragment(s));
        }

        public void Append(SqlFragment s)
        {
            Fragments.Add(s);
        }

        public override void WriteSql(StringBuilder sql)
        {
            foreach (SqlFragment f in Fragments)
                f.WriteSql(sql);
        }
    }

    internal class NegatableFragment : SqlFragment
    {
        public bool IsNegated;

        public void Negate()
        {
            IsNegated = !IsNegated;
        }

        public override void  WriteSql(StringBuilder sql)
        {
            Debug.Fail("This method should be overridden");
        }   
    }

    internal class LiteralFragment : SqlFragment
    {
        public string Literal;

        public LiteralFragment(string literal)
        {
            Literal = literal;
        }

        public override void WriteSql(StringBuilder sql)
        {
            sql.Append(Literal);
        }
    }

    internal class PropertyFragment : SqlFragment
    {
        public PropertyFragment()
        {
            Properties = new List<string>();
        }

        public List<string> Properties { get; private set; }

        public override void WriteSql(StringBuilder sql)
        {
            throw new NotImplementedException();
        }
    }

    internal class SortFragment : SqlFragment
    {
        public SortFragment(SqlFragment column, bool ascending)
        {
            Column = column;
            Ascending = ascending;
        }

        public SqlFragment Column { get; set; }
        public bool Ascending { get; set; }

        public override void WriteSql(StringBuilder sql)
        {
            Debug.Assert(Column is ColumnFragment);
            ColumnFragment f = Column as ColumnFragment;
            sql.AppendFormat("{0} {1}", QuoteIdentifier(f.ColumnName), Ascending ? "ASC" : "DESC");
        }
    }

    internal class UnionFragment : InputFragment
    {
        public bool Distinct = false;

        public override void WriteInnerSql(StringBuilder sql)
        {
            Left.WriteSql(sql);
            sql.Append(Distinct ? " UNION DISTINCT " : " UNION ALL ");
            Right.WriteSql(sql);
        }
    }
}
