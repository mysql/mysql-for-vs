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

        protected override BaseStatement Current
        {
            get { return selectStatements.Count == 0 ? null : selectStatements.Peek(); }
        }

        private SelectStatement CurrentSelect
        {
            get { return Current as SelectStatement; }
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
                    scope.Push(null);
                    fragment = e.Accept(this);
                    break;
            }

            return fragment.GenerateSQL();
        }

        public override SqlFragment Visit(DbFilterExpression expression)
        {
            scope.Push(expression.Input.VariableName);
            SqlFragment input = expression.Input.Expression.Accept(this);

            (Current as SelectStatement).Where.Add(expression.Predicate.Accept(this));

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

            for (int agg = 0; agg <= expression.Aggregates.Count; agg++)
            {
                DbAggregate a = expression.Aggregates[agg];
                DbFunctionAggregate fa = a as DbFunctionAggregate;
                if (fa == null) throw new NotSupportedException();

                ListFragment lf = new ListFragment("");
                string sql = fa.Function.Name + "(";
                lf.Items.Add(new SqlFragment(fa.Function.Name + "("));
                if (fa.Distinct)
                    sql += " DISTINCT ";
                Push();
                lf.Items.Add(new SqlFragment(sql));
                lf.Items.Add(fa.Arguments[0].Accept(this));
                lf.Items.Add(new SqlFragment(")"));
                //Current.HashFragment(scope.Peek() + "." + names[agg], lf);
                Pop();
            }
            scope.Pop();
            return input;
        }

        public override SqlFragment Visit(DbJoinExpression expression)
        {
            JoinFragment join = new JoinFragment();
            join.Name = scope.Peek();
            join.JoinType = Metadata.GetOperator(expression.ExpressionKind);

            scope.Push(expression.Left.VariableName);
            join.Left = expression.Left.Expression.Accept(this);
            Current.IndexFragment(join.Left, scope.Peek());

            scope.Push(expression.Right.VariableName);
            join.Right = expression.Right.Expression.Accept(this);
            Current.IndexFragment(join.Right, scope.Pop());

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

            ListFragment list = new ListFragment(", ");

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                SqlFragment fragment = expression.Arguments[i].Accept(this);
                if (row != null)
                    fragment.Name = row.Properties[i].Name;
                list.Items.Add(fragment);
            }
            return list;
        }

        private SqlFragment HandleNewInstanceAsInput(DbNewInstanceExpression expression)
        {
            SelectStatement statement = new SelectStatement(CurrentSelect);
            statement.Name = scope.Pop();

            if (expression.Arguments.Count == 0)
            {
                statement.Output = new SqlFragment("NULL");
            }
            else
            {
                statement.Output = expression.Arguments[0].Accept(this);
            }
            statement.Output.Name = "X";
            return statement;
        }

        public override SqlFragment Visit(DbProjectExpression expression)
        {
            SelectStatement statement = new SelectStatement(Current as SelectStatement);
            statement.Name = scope.Pop();
            selectStatements.Push(statement);

            // handle from clause
            scope.Push(expression.Input.VariableName);
            statement.Input = expression.Input.Expression.Accept(this);
            
            // now handle projection
            statement.Output = expression.Projection.Accept(this);

            selectStatements.Pop();
            return statement;
        }

        public override SqlFragment Visit(DbSortExpression expression)
        {
            ListFragment list = new ListFragment(" ");

            scope.Push(expression.Input.VariableName);
            list.Items.Add(expression.Input.Expression.Accept(this));
            list.Items.Add(new SqlFragment("ORDER BY"));

            ListFragment clauses = new ListFragment(", ");
            foreach (DbSortClause sortClause in expression.SortOrder)
            {
                ListFragment clause = new ListFragment(" ");
                clause.Items.Add(sortClause.Expression.Accept(this));
                clause.Items.Add(new SqlFragment(sortClause.Ascending ? "ASC" : "DESC"));
                clauses.Items.Add(clause);
            }
            list.Items.Add(clauses);
            return list;
        }

        public override SqlFragment Visit(DbSkipExpression expression)
        {
            CurrentSelect.Skip = expression.Count.Accept(this);
            return expression.Input.Expression.Accept(this);
        }
    }
}
