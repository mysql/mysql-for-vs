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

        public InputFragment From;
        public List<ColumnFragment> Columns { get; private set;  }
        public SqlFragment Where;
        public SqlFragment Limit;
        public SqlFragment Skip;
        public List<SqlFragment> GroupBy { get; private set; }
        public List<SortFragment> OrderBy { get; private set; }
        public bool IsDistinct;

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

        public override void WriteSql(StringBuilder sql)
        {
            if (IsWrapped)
                sql.Append("(");
            sql.Append("SELECT");
            if (IsDistinct)
                sql.Append(" DISTINCT ");

            WriteList(Columns, sql);

            if (From != null)
            {
                sql.Append("\r\nFROM ");
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
            WriteOrderBy(sql);
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
            if (IsWrapped)
            {
                sql.Append(")");
                if (Name != null)
                    sql.AppendFormat(" AS {0}", QuoteIdentifier(Name));
            }
        }

        private void WriteOrderBy(StringBuilder sql)
        {
            if (OrderBy == null) return;
            sql.Append("\r\n ORDER BY ");
            WriteList(OrderBy, sql);
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
//            List<PropertyFragment> properties = GetColumnPropertiesFromInput(From);
            AddDefaultColumnsForFragment(From);
        }

        //private List<PropertyFragment> GetColumnPropertiesFromInput(InputFragment input)
        //{
        //    if (input is TableFragment)
        //}

        void AddDefaultColumnsForFragment(InputFragment input)
        {
            if (input is TableFragment)
            {
                AddDefaultColumnsForTable(input as TableFragment);
            }
            else if (input is JoinFragment || input is UnionFragment)
            {
                if (input.Left != null)
                    AddDefaultColumnsForFragment(input.Left);
                if (input.Right != null)
                AddDefaultColumnsForFragment(input.Right);

                // if this input is scoped, then it is the base tablename for the columns
                if (input.Scoped)
                    foreach (ColumnFragment col in Columns)
                        col.TableName = input.Name;
            }
            else if (input is SelectStatement)
            {
                SelectStatement select = input as SelectStatement;
                foreach (ColumnFragment cf in select.Columns)
                    Columns.Add(cf.Clone());
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
                    return Columns.Count == 0 &&
                        GroupBy == null &&
                        OrderBy == null;
                case DbExpressionKind.GroupBy:
                    return Columns.Count == 0 &&
                        GroupBy == null &&
                        OrderBy == null &&
                        Limit == null;
            }
            throw new InvalidOperationException();
        }
    }
}
