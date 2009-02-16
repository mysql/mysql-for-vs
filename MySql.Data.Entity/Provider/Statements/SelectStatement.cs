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
using System.Collections;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Data.Common.CommandTrees;

namespace MySql.Data.Entity
{
    class SelectStatement : InputFragment 
    {
        private Dictionary<string, ColumnFragment> columnHash;

        public SelectStatement() : base(null)
        {
            Columns = new List<ColumnFragment>();
        }

        public InputFragment From { get; set; }
        public List<ColumnFragment> Columns { get; private set;  }
        public SqlFragment Where { get; set; }
        public SqlFragment Limit { get; set; }
        public SqlFragment Skip { get; set; }
        public List<SqlFragment> GroupBy { get; private set; }
        public List<SortFragment> OrderBy { get; private set; }

        public void AddGroupBy(SqlFragment f)
        {
            if (GroupBy == null)
                GroupBy = new List<SqlFragment>();
            GroupBy.Add(f);
        }

        public void AddOrderBy(SortFragment f)
        {
            if (OrderBy == null)
                OrderBy = new List<SortFragment>();
            OrderBy.Add(f);
        }

        public override SqlFragment GetProperty(string propertyName)
        {
            if (From == null || From.Name != propertyName) return null;
            return From;
        }

        public override void WriteInnerSql(StringBuilder sql)
        {
            sql.Append("SELECT ");
            WriteList(Columns, sql);

            if (From != null)
            {
                sql.Append("\r\n FROM ");
                From.WriteSql(sql);
            }
            if (Where != null)
            {
                sql.Append("\r\n WHERE ");
                Where.WriteSql(sql);
            }
            if (GroupBy != null)
            {
                sql.Append("\r\n GROUP BY ");
                WriteList(GroupBy, sql);
            }
            if (OrderBy != null)
            {
                sql.Append("\r\n ORDER BY ");
                WriteList(OrderBy, sql);
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
        }

        public override void Wrap(Scope scope)
        {
            base.Wrap(scope);

            // next we need to remove child extents of the select from scope
            if (Name != null)
            {
                scope.Remove(this);
                scope.Add(Name, this);
            }

            // now we need to add default columns if necessary
            if (Columns.Count == 0)
                AddDefaultColumns();
        }

        void AddDefaultColumns()
        {
            AddDefaultColumnsForFragment(From);
        }

        void AddDefaultColumnsForFragment(InputFragment input)
        {
            if (input is TableFragment)
            {
                AddDefaultColumnsForTable(input as TableFragment);
            }
            else if (input is JoinFragment)
            {
                JoinFragment j = input as JoinFragment;
                AddDefaultColumnsForFragment(j.Left);
                AddDefaultColumnsForFragment(j.Right);
            }
            else
                throw new NotImplementedException();
        }

        void AddDefaultColumnsForTable(TableFragment table)
        {
            if (columnHash == null)
                columnHash = new Dictionary<string, ColumnFragment>();

            foreach (EdmProperty property in Metadata.GetProperties(table.Type.EdmType))
            {
                ColumnFragment col = new ColumnFragment(table.Name, property.Name);
                if (table.Columns == null)
                    table.Columns = new List<ColumnFragment>();
                table.Columns.Add(col);
                if (columnHash.ContainsKey(col.ColumnName))
                {
                    col.ColumnAlias = MakeColumnNameUnique(col.ColumnName);
                    columnHash.Add(col.ColumnAlias, col);
                }
                else
                    columnHash.Add(col.ColumnName, col);
                Columns.Add(col);
            }
        }

        private string MakeColumnNameUnique(string baseName)
        {
            int i = 1;
            while (true)
            {
                string name = String.Format("{0}{1}", baseName, i);
                if (!columnHash.ContainsKey(name)) return name;
                i++;
            }
        }

        public bool IsCompatible(DbExpressionKind expressionKind)
        {
            switch (expressionKind)
            {
                case DbExpressionKind.Filter:
                    return Where == null && Columns.Count == 0;
                case DbExpressionKind.Project:
                    return Columns.Count == 0;
                case DbExpressionKind.Limit:
                    return Limit == null;
                case DbExpressionKind.Skip:
                    return Skip == null;
                case DbExpressionKind.Sort:
                    return OrderBy == null;
            }
            throw new InvalidOperationException();
        }
    }
}
