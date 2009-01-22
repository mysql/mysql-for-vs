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
            scope.Push(expression.Input.VariableName);
            SqlFragment input = expression.Input.Expression.Accept(this);

            CurrentSelect.Where.Add(expression.Predicate.Accept(this));

            return input;
        }

        public override SqlFragment Visit(DbGroupByExpression expression)
        {
            scope.Push(expression.Input.VariableName);
            SqlFragment input = expression.Input.Expression.Accept(this);

            CollectionType ct = (CollectionType)expression.ResultType.EdmType;
            RowType rt = (RowType)ct.TypeUsage.EdmType;
//            RowType groupByType = MetadataHelpers.GetEdmType<RowType>(MetadataHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);

            List<string> names = new List<string>();
            using (IEnumerator<EdmProperty> members = rt.Properties.GetEnumerator())
            {
                members.MoveNext();
                names.Add(members.Current.Name);
            }

//            foreach (DbExpression key in expression.Keys)
  //          {
//                key.
    //        }

            Trace.WriteLine(String.Format("{0}{1}", tabs, "GroupByExpression"));

            for (int agg = 0; agg < expression.Aggregates.Count; agg++)
            {
                DbAggregate a = expression.Aggregates[agg];
                DbFunctionAggregate fa = a as DbFunctionAggregate;
                if (fa == null) throw new NotSupportedException();

                ListFragment lf = new ListFragment("");
                string sql = fa.Function.Name + "(";
                lf.Items.Add(new SqlFragment(fa.Function.Name + "("));
                if (fa.Distinct)
                    sql += " DISTINCT ";
                lf.Items.Add(new SqlFragment(sql));
                lf.Items.Add(fa.Arguments[0].Accept(this));
                lf.Items.Add(new SqlFragment(")"));
            }
            return input;
        }

        public override SqlFragment Visit(DbJoinExpression expression)
        {
            JoinFragment join = new JoinFragment();
            join.JoinType = Metadata.GetOperator(expression.ExpressionKind);
            join.Name = scope.Pop();

            scope.Push(expression.Left.VariableName);
            join.Left = (InputFragment)expression.Left.Expression.Accept(this);

            scope.Push(expression.Right.VariableName);
            join.Right = (InputFragment)expression.Right.Expression.Accept(this);

            // now handle the ON case
            join.Condition = expression.JoinCondition.Accept(this);
            return join;
        }

        public override SqlFragment Visit(DbLimitExpression expression)
        {
            CurrentSelect.Limit = expression.Limit.Accept(this);
            return expression.Argument.Accept(this);
        }

        public override SqlFragment Visit(DbNewInstanceExpression expression)
        {
            if (expression.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
                return HandleNewInstanceAsInput(expression);

            RowType row = expression.ResultType.EdmType as RowType;

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                SqlFragment fragment = expression.Arguments[i].Accept(this);
                if (row != null)
                    fragment.Name = row.Properties[i].Name;
                CurrentSelect.Output.Items.Add(fragment);
            }
            return null;
        }

        private SqlFragment HandleNewInstanceAsInput(DbNewInstanceExpression expression)
        {
            SelectStatement statement = new SelectStatement(CurrentSelect);
            statement.Name = scope.Pop();

            SqlFragment output = new SqlFragment("NULL");
            if (expression.Arguments.Count != 0)
                output = expression.Arguments[0].Accept(this);
            output.Name = "X";
            statement.Output.Items.Add(output);
            return statement;
        }

        public override SqlFragment Visit(DbProjectExpression expression)
        {
            SelectStatement statement = new SelectStatement(CurrentSelect);
            if (scope.Count > 0)
                statement.Name = scope.Pop();
            selectStatements.Push(statement);

            // handle from clause
            scope.Push(expression.Input.VariableName);
            statement.Input = (InputFragment)expression.Input.Expression.Accept(this);
            
            // now handle projection
            expression.Projection.Accept(this);

            selectStatements.Pop();
            return statement;
        }

        public override SqlFragment Visit(DbSortExpression expression)
        {
            scope.Push(expression.Input.VariableName);
            SqlFragment input = expression.Input.Expression.Accept(this);

            foreach (DbSortClause sortClause in expression.SortOrder)
            {
                ListFragment clause = new ListFragment(" ");
                clause.Items.Add(sortClause.Expression.Accept(this));
                clause.Items.Add(new SqlFragment(sortClause.Ascending ? "ASC" : "DESC"));
                CurrentSelect.OrderBy.Add(clause);
            }
            return input;
        }

        public override SqlFragment Visit(DbSkipExpression expression)
        {
            CurrentSelect.Skip = expression.Count.Accept(this);
            return expression.Input.Expression.Accept(this);
        }
    }
}
