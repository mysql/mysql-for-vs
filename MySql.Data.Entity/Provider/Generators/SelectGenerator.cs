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

using System;
using System.Text;
using System.Diagnostics;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace MySql.Data.Entity
{
    class SelectGenerator : SqlGenerator 
    {
        Stack<SelectStatement> selectStatements = new Stack<SelectStatement>();

        #region Properties 

        private SelectStatement CurrentSelect
        {
            get { return selectStatements.Count == 0 ? null : selectStatements.Peek(); }
        }

        #endregion

        public override string GenerateSQL(DbCommandTree tree)
        {
            DbQueryCommandTree commandTree = tree as DbQueryCommandTree;

            SqlFragment fragment = null;

            DbExpression e = commandTree.Query;
            switch (commandTree.Query.ExpressionKind)
            {
                case DbExpressionKind.Project:
                    fragment = e.Accept(this);
                    Debug.Assert(fragment is SelectStatement);
                    break;
            }

            return fragment.ToString();
        }

        public override SqlFragment Visit(DbFilterExpression expression)
        {
            SelectStatement select = VisitInputExpressionEnsureSelect(expression.Input.Expression, 
                expression.Input.VariableName, expression.Input.VariableType);
            select = WrapIfNotCompatible(select, expression.ExpressionKind);
            select.Where = expression.Predicate.Accept(this);
            return select;
        }

        public override SqlFragment Visit(DbGroupByExpression expression)
        {
            // first process the input
            DbGroupExpressionBinding e = expression.Input;
            SelectStatement select = VisitInputExpressionEnsureSelect(e.Expression, e.VariableName, e.VariableType);

            CollectionType ct = (CollectionType)expression.ResultType.EdmType;
            RowType rt = (RowType)ct.TypeUsage.EdmType;

            int propIndex = 0;
            foreach (DbExpression key in expression.Keys)
            {
                select.AddGroupBy(key.Accept(this));
                propIndex++;
            }

            for (int agg = 0; agg < expression.Aggregates.Count; agg++)
            {
                DbAggregate a = expression.Aggregates[agg];
                DbFunctionAggregate fa = a as DbFunctionAggregate;
                if (fa == null) throw new NotSupportedException();

                ColumnFragment col = new ColumnFragment(null, null);
                col.Literal = HandleFunction(fa);
                col.ColumnAlias = rt.Properties[propIndex++].Name;
                select.Columns.Add(col);
            }
            return select;
        }

        private SqlFragment HandleFunction(DbFunctionAggregate fa)
        {
            Debug.Assert(fa.Arguments.Count == 1);

            if (fa.Function.NamespaceName != "Edm")
                throw new NotSupportedException();

            FunctionFragment fragment = new FunctionFragment();
            fragment.Name = fa.Function.Name;
            if (fa.Function.Name == "BigCount")
                fragment.Name = "Count";

            fragment.Distinct = fa.Distinct;
            fragment.Argmument = fa.Arguments[0].Accept(this);
            return fragment;
            //return new CastExpression(aggregate, GetDbType(functionAggregate.ResultType.EdmType));
        }

        public override SqlFragment Visit(DbJoinExpression expression)
        {
            JoinFragment join = new JoinFragment();
            join.JoinType = Metadata.GetOperator(expression.ExpressionKind);

            join.Left = VisitInputExpression(expression.Left.Expression,
                expression.Left.VariableName, expression.Left.VariableType);
            WrapJoinInputIfNecessary(join.Left);
            join.Right = VisitInputExpression(expression.Right.Expression,
                expression.Right.VariableName, expression.Right.VariableType);
            WrapJoinInputIfNecessary(join.Right);

            // now handle the ON case
            join.Condition = expression.JoinCondition.Accept(this);
            return join;
        }

        public SelectStatement WrapIfNotCompatible(SelectStatement select, DbExpressionKind expressionKind)
        {
            if (select.IsCompatible(expressionKind)) return select;
            SelectStatement newSelect = new SelectStatement();
            select.Wrap(scope);
            newSelect.From = select;
            return newSelect;
        }

        private void WrapJoinInputIfNecessary(InputFragment fragment)
        {
            if (fragment is SelectStatement ||
                fragment is UnionFragment ||
                fragment is JoinFragment)
                fragment.Wrap(scope);
        }

        public override SqlFragment Visit(DbNewInstanceExpression expression)
        {
            Debug.Assert(expression.ResultType.EdmType is CollectionType);

            SelectStatement s = new SelectStatement();

            ColumnFragment c = new ColumnFragment(null, null);
            if (expression.Arguments.Count != 0)
                c.Literal = (LiteralFragment)expression.Arguments[0].Accept(this);
            else
                c.Literal = new LiteralFragment("NULL");
            c.ColumnAlias = "X";
            s.Columns.Add(c);
            return s;
        }

        private void VisitNewInstanceExpression(SelectStatement select, 
            DbNewInstanceExpression expression)
        {
            Debug.Assert(expression.ResultType.EdmType is RowType);

            RowType row = expression.ResultType.EdmType as RowType;

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                ColumnFragment col = null;

                SqlFragment fragment = expression.Arguments[i].Accept(this);
                if (fragment is ColumnFragment)
                    col = fragment as ColumnFragment;
                else
                {
                    col = new ColumnFragment(null, null);
                    col.Literal = fragment;
                }

                col.ColumnAlias = row.Properties[i].Name;
                select.Columns.Add(col);
            }
        }

        public override SqlFragment Visit(DbProjectExpression expression)
        {
            SelectStatement select = VisitInputExpressionEnsureSelect(expression.Input.Expression, 
                expression.Input.VariableName, expression.Input.VariableType);

            // see if we need to wrap this select inside a new select
            select = WrapIfNotCompatible(select, expression.ExpressionKind);

            Debug.Assert(expression.Projection is DbNewInstanceExpression);
            VisitNewInstanceExpression(select, expression.Projection as DbNewInstanceExpression);

            return select;
        }

        private SelectStatement VisitInputExpressionEnsureSelect(DbExpression e, string name, TypeUsage type)
        {
            InputFragment fragment = VisitInputExpression(e, name, type);
            if (fragment is SelectStatement) return (fragment as SelectStatement);

            SelectStatement s = new SelectStatement();
            
            // if the fragment is a union then it needs to be wrapped
            if (fragment is UnionFragment)
                (fragment as UnionFragment).Wrap(scope);

            s.From = fragment;
            return s;
        }

        public override SqlFragment Visit(DbSortExpression expression)
        {
            SelectStatement select = VisitInputExpressionEnsureSelect(expression.Input.Expression, 
                expression.Input.VariableName, expression.Input.VariableType);

            foreach (DbSortClause sortClause in expression.SortOrder)
            {
                select.AddOrderBy(new SortFragment(
                    sortClause.Expression.Accept(this), sortClause.Ascending));
            }
            return select;
        }

        public override SqlFragment Visit(DbLimitExpression expression)
        {
            SelectStatement statement = (SelectStatement)VisitInputExpressionEnsureSelect(
                expression.Argument, null, null);
            statement.Limit = expression.Limit.Accept(this);
            return statement;
        }

        public override SqlFragment Visit(DbSkipExpression expression)
        {
            SelectStatement select = VisitInputExpressionEnsureSelect(expression.Input.Expression, expression.Input.VariableName,
                expression.Input.VariableType);

            select = WrapIfNotCompatible(select, expression.ExpressionKind);
            select.Skip = expression.Count.Accept(this);
            return select;
        }

        public override SqlFragment Visit(DbUnionAllExpression expression)
        {
            UnionFragment f = new UnionFragment();
            Debug.Assert(expression.Left is DbProjectExpression);
            Debug.Assert(expression.Right is DbProjectExpression);

            SelectStatement left = VisitInputExpressionEnsureSelect(expression.Left, null, null);
            Debug.Assert(left.Name == null);
            left.Wrap(scope);

            SelectStatement right = VisitInputExpressionEnsureSelect(expression.Right, null, null);
            Debug.Assert(right.Name == null);
            right.Wrap(scope);

            f.Left = left;
            f.Right = right;
            return f;
        }
    }
}
