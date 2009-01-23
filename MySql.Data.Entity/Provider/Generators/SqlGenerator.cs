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
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using MySql.Data.MySqlClient;

namespace MySql.Data.Entity
{
    abstract class SqlGenerator : DbExpressionVisitor<SqlFragment>
    {
        protected string tabs = String.Empty;
        private int parameterCount = 1;
        protected Stack<string> scope = new Stack<string>();

        public SqlGenerator()
        {
            Parameters = new List<MySqlParameter>();
            Symbols = new SymbolTable();
        }

        #region Properties

        public List<MySqlParameter> Parameters { get; private set; }
        protected SymbolTable Symbols { get; private set; }

        #endregion

        protected void Push()
        {
            tabs += "-";
        }

        protected void Pop()
        {
            tabs = tabs.Substring(1);
        }

        public virtual string GenerateSQL(DbCommandTree commandTree)
        {
            throw new NotImplementedException();
        }

        protected string CreateUniqueParameterName()
        {
            return String.Format("@p{0}", parameterCount++);
        }

        #region DbExpressionVisitor Base Implementations

        public override SqlFragment Visit(DbVariableReferenceExpression expression)
        {
            return Symbols.Lookup(expression.VariableName);
        }

        public override SqlFragment Visit(DbPropertyExpression expression)
        {
            InputFragment parent = (InputFragment)expression.Instance.Accept(this);
            InputFragment f = (InputFragment)parent.GetProperty(expression.Property.Name);
            if (f != null) return f;

            SymbolFragment sym = new SymbolFragment();
            sym.Fragment = parent;
            sym.Property = expression.Property.Name;
            return sym;
        }

        public override SqlFragment Visit(DbScanExpression expression)
        {
            EntitySetBase target = expression.Target;
            InputFragment fragment = new InputFragment();

            MetadataProperty property;
            bool propExists = target.MetadataProperties.TryGetValue("DefiningQuery", true, out property);
            if (propExists && property.Value != null)
                fragment.Text = String.Format("({0})", property.Value);
            else
            {
                string schema = target.EntityContainer.Name;
                string table = target.Name;

                propExists = target.MetadataProperties.TryGetValue("Schema", true, out property);
                if (propExists && property.Value != null)
                    schema = property.Value as string;
                propExists = target.MetadataProperties.TryGetValue("Table", true, out property);
                if (propExists && property.Value != null)
                    table = property.Value as string;
                fragment.Text = String.Format("`{0}`.`{1}`", schema, table);
            }
            //fragment.Name = scope.Pop();
            return fragment;
        }

        public override SqlFragment Visit(DbParameterReferenceExpression expression)
        {
            return new SqlFragment("@" + expression.ParameterName);
        }

        public override SqlFragment Visit(DbNotExpression expression)
        {
            SqlFragment f = expression.Argument.Accept(this);
            return f;
        }

        public override SqlFragment Visit(DbIsEmptyExpression expression)
        {
            SqlFragment f = expression.Argument.Accept(this);
            return f;
        }

        public override SqlFragment Visit(DbFunctionExpression expression)
        {
            FunctionGenerator gen = new FunctionGenerator();
            return gen.Generate(expression, this);
        }

        public override SqlFragment Visit(DbConstantExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}", tabs, "ConstantExpress"));

            SqlFragment f = new SqlFragment();
            if (Metadata.IsNumericType(expression.ResultType))
                f.Text = expression.Value.ToString();
            else
            {
                // use a parameter for non-numeric types so we get proper
                // quoting
                MySqlParameter p = new MySqlParameter();
                p.ParameterName = CreateUniqueParameterName();
                p.DbType = Metadata.GetDbType(expression.ResultType);
                p.Value = expression.Value;
                Parameters.Add(p);
                f.Text = p.ParameterName;
            }
            return f;
        }

        public override SqlFragment Visit(DbComparisonExpression expression)
        {
            SqlFragment left = expression.Left.Accept(this);
            SqlFragment right = expression.Right.Accept(this);

            ListFragment l = new ListFragment(" ");
            l.Items.Add(left);
            l.Items.Add(new SqlFragment(Metadata.GetOperator(expression.ExpressionKind)));
            l.Items.Add(right);
            return l;
        }

        public override SqlFragment Visit(DbAndExpression expression)
        {
            return VisitBinaryExpression(expression.Left, expression.Right, "AND");
        }

        public override SqlFragment Visit(DbOrExpression expression)
        {
            return VisitBinaryExpression(expression.Left, expression.Right, "OR");
        }

        public override SqlFragment Visit(DbCastExpression expression)
        {
            //TODO: handle casting
            return expression.Argument.Accept(this);
        }

        public override SqlFragment Visit(DbUnionAllExpression expression)
        {
            InputFragment input = new InputFragment();
            input.Name = scope.Pop();

            scope.Push(null);
            SqlFragment left = expression.Left.Accept(this);
            Debug.Assert(left is SelectStatement);
            (left as SelectStatement).Parent = null;
            input.Inputs.Add(left);

            input.Inputs.Add(new SqlFragment("UNION ALL"));

            scope.Push(null);
            SqlFragment right = expression.Right.Accept(this);
            Debug.Assert(right is SelectStatement);
            (right as SelectStatement).Parent = null;
            input.Inputs.Add(right);

            return input;
        }

        public override SqlFragment Visit(DbLikeExpression expression)
        {
            ListFragment list = new ListFragment(" ");
            list.Items.Add(expression.Argument.Accept(this));
            list.Items.Add(new SqlFragment(" LIKE "));
            list.Items.Add(expression.Pattern.Accept(this));

            if (expression.Escape.ExpressionKind != DbExpressionKind.Null)
            {
                list.Items.Add(new SqlFragment(" ESCAPE "));
                list.Items.Add(expression.Escape.Accept(this));
            }

            return list;
        }

        public override SqlFragment Visit(DbCaseExpression expression)
        {
            ListFragment list = new ListFragment("");

            Debug.Assert(expression.When.Count == expression.Then.Count);

            list.Append("CASE");
            for (int i = 0; i < expression.When.Count; ++i)
            {
                list.Append(" WHEN (");
                list.Append(expression.When[i].Accept(this));
                list.Append(") THEN ");
                list.Append(expression.Then[i].Accept(this));
            }
            if (expression.Else != null && !(expression.Else is DbNullExpression))
            {
                list.Append(" ELSE ");
                list.Append(expression.Else.Accept(this));
            }

            list.Append(" END");
            return list;
        }

        public override SqlFragment Visit(DbIsNullExpression expression)
        {
            ListFragment list = new ListFragment("");
            list.Append(expression.Argument.Accept(this));
            list.Append(" IS NULL");
            return list;
        }

        #endregion

        #region DBExpressionVisitor methods normally overridden


        public override SqlFragment Visit(DbTreatExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbSortExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbSkipExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbRelationshipNavigationExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbQuantifierExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbProjectExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbOfTypeExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbNullExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbNewInstanceExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbLimitExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbJoinExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbIsOfExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbIntersectExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbGroupByExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbRefKeyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbEntityRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbFilterExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbExceptExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbElementExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbDistinctExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbDerefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbCrossJoinExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbArithmeticExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbApplyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbExpression expression)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private ListFragment VisitBinaryExpression(DbExpression left, DbExpression right, string op)
        {
            ListFragment list = new ListFragment(" ");
            list.Items.Add(left.Accept(this));
            list.Items.Add(new SqlFragment(op));
            list.Items.Add(right.Accept(this));
            return list;
        }

        #endregion
    }
}
