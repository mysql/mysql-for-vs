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
//        protected Stack<string> inputVars = new Stack<string>();
  //      protected Dictionary<string, string> varMap = new Dictionary<string, string>();
        protected Scope scope = new Scope();
        protected int propertyLevel;
        public List<ColumnFragment> BoolOverrides = new List<ColumnFragment>();

        public SqlGenerator()
        {
            Parameters = new List<MySqlParameter>();
        }

        #region Properties

        public List<MySqlParameter> Parameters { get; private set; }
//        protected SymbolTable Symbols { get; private set; }

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
            return String.Format("@gp{0}", parameterCount++);
        }

        #region DbExpressionVisitor Base Implementations

        public override SqlFragment Visit(DbVariableReferenceExpression expression)
        {
            PropertyFragment fragment = new PropertyFragment();
            fragment.Properties.Add(expression.VariableName);
            return fragment;
        }

        public override SqlFragment Visit(DbPropertyExpression expression)
        {
            propertyLevel++;
            PropertyFragment fragment = expression.Instance.Accept(this) as PropertyFragment;
            fragment.Properties.Add(expression.Property.Name);
            propertyLevel--;

            // if we are not at the top level property then just return
            if (propertyLevel > 0) return fragment;

            // we are at the top level property so now we can do our work
            ColumnFragment column = GetColumnFromPropertyTree(fragment);

            for (int i = fragment.Properties.Count - 1; i >= 0; --i)
            {
                InputFragment inputFragment = scope.GetFragment(fragment.Properties[i]);
                if (inputFragment != null)
                {
                    column.TableAlias = inputFragment.Name;
                    break;
                }
            }
            return column;
        }

        public override SqlFragment Visit(DbScanExpression expression)
        {
            EntitySetBase target = expression.Target;
            TableFragment fragment = new TableFragment();

            MetadataProperty property;
            bool propExists = target.MetadataProperties.TryGetValue("DefiningQuery", true, out property);
            if (propExists && property.Value != null)
                fragment.DefiningQuery = new LiteralFragment(property.Value as string);
            else
            {
                fragment.Schema = target.EntityContainer.Name;
                fragment.Table = target.Name;

                propExists = target.MetadataProperties.TryGetValue("Schema", true, out property);
                if (propExists && property.Value != null)
                    fragment.Schema = property.Value as string;
                propExists = target.MetadataProperties.TryGetValue("Table", true, out property);
                if (propExists && property.Value != null)
                    fragment.Table = property.Value as string;
            }
            return fragment;
        }

        public override SqlFragment Visit(DbParameterReferenceExpression expression)
        {
            return new LiteralFragment("@" + expression.ParameterName);
        }

        public override SqlFragment Visit(DbNotExpression expression)
        {
            SqlFragment f = expression.Argument.Accept(this);
            Debug.Assert(f is NegatableFragment);
            NegatableFragment nf = f as NegatableFragment;
            nf.Negate();
            return nf;
        }

        public override SqlFragment Visit(DbIsEmptyExpression expression)
        {
            ExistsFragment f = new ExistsFragment(expression.Argument.Accept(this));
            f.Negate();
            return f;
        }

        public override SqlFragment Visit(DbFunctionExpression expression)
        {
            FunctionGenerator gen = new FunctionGenerator();
            return gen.Generate(expression, this);
        }

        public override SqlFragment Visit(DbConstantExpression expression)
        {
            PrimitiveTypeKind pt = ((PrimitiveType)expression.ResultType.EdmType).PrimitiveTypeKind;
            if (Metadata.IsNumericType(expression.ResultType))
                return new LiteralFragment(expression.Value.ToString());
            else if (pt == PrimitiveTypeKind.Boolean)
                return new LiteralFragment(String.Format("cast({0} as decimal(0,0))",
                    (bool)expression.Value ? 1 : 0));
            else
            {
                // use a parameter for non-numeric types so we get proper
                // quoting
                MySqlParameter p = new MySqlParameter();
                p.ParameterName = CreateUniqueParameterName();
                p.DbType = Metadata.GetDbType(expression.ResultType);
                p.Value = expression.Value;
                Parameters.Add(p);
                return new LiteralFragment(p.ParameterName);
            }
        }

        public override SqlFragment Visit(DbComparisonExpression expression)
        {
            return VisitBinaryExpression(expression.Left, expression.Right,
                Metadata.GetOperator(expression.ExpressionKind));
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

        public override SqlFragment Visit(DbLikeExpression expression)
        {
            LikeFragment f = new LikeFragment();

            f.Argument = expression.Argument.Accept(this);
            f.Pattern = expression.Pattern.Accept(this);

            if (expression.Escape.ExpressionKind != DbExpressionKind.Null)
                f.Escape = expression.Escape.Accept(this);

            return f;
        }

        public override SqlFragment Visit(DbCaseExpression expression)
        {
            CaseFragment c = new CaseFragment();

            Debug.Assert(expression.When.Count == expression.Then.Count);

            for (int i = 0; i < expression.When.Count; ++i)
            {
                c.When.Add(expression.When[i].Accept(this));
                c.Then.Add(expression.Then[i].Accept(this));
            }
            if (expression.Else != null && !(expression.Else is DbNullExpression))
                c.Else = expression.Else.Accept(this);
            return c;
        }

        public override SqlFragment Visit(DbIsNullExpression expression)
        {
            IsNullFragment f = new IsNullFragment();
            f.Argument = expression.Argument.Accept(this);
            return f;
        }

        public override SqlFragment Visit(DbNullExpression expression)
        {
            return new LiteralFragment("NULL");
        }

        public override SqlFragment Visit(DbArithmeticExpression expression)
        {
            if (expression.ExpressionKind == DbExpressionKind.UnaryMinus)
            {
                ListFragment f = new ListFragment();
                f.Append("-(");
                f.Append(expression.Arguments[0].Accept(this));
                f.Append(")");
                return f;
            }

            string op = String.Empty;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Divide:
                    op = "/"; break;
                case DbExpressionKind.Minus:
                    op = "-"; break;
                case DbExpressionKind.Modulo:
                    op = "%"; break;
                case DbExpressionKind.Multiply:
                    op = "*"; break;
                case DbExpressionKind.Plus:
                    op = "+"; break;
                default:
                    throw new NotSupportedException();
            }
            return VisitBinaryExpression(expression.Arguments[0], expression.Arguments[1], op);
        }


        #endregion

        #region DBExpressionVisitor methods normally overridden

        public override SqlFragment Visit(DbUnionAllExpression expression)
        {
            throw new NotImplementedException();
        }

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

        public override SqlFragment Visit(DbApplyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbExpression expression)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected InputFragment VisitInputExpression(DbExpression e, string name, TypeUsage type)
        {
            SqlFragment f = e.Accept(this);
            Debug.Assert(f is InputFragment);

            InputFragment inputFragment = f as InputFragment;
            inputFragment.Name = name;

            if (inputFragment is TableFragment && type != null)
                (inputFragment as TableFragment).Type = type;

            SelectStatement select = inputFragment as SelectStatement;
            if (name != null)
            {
                if (select != null &&  !select.IsWrapped)
                    scope.Add(name, select.From);
                else
                    scope.Add(name, inputFragment);
            }

            return inputFragment;
        }

        #region Private Methods

        SqlFragment VisitBinaryExpression(DbExpression left, DbExpression right, string op)
        {
            BinaryFragment f = new BinaryFragment();
            f.Operator = op;
            f.Left = left.Accept(this);
            f.WrapLeft = ShouldWrapExpression(left);
            f.Right = right.Accept(this);
            f.WrapRight = ShouldWrapExpression(right);
            return f;
        }

        private bool ShouldWrapExpression(DbExpression e)
        {
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.Property:
                case DbExpressionKind.ParameterReference:
                case DbExpressionKind.Constant:
                    return false;
            }
            return true;
        }

        ColumnFragment GetColumnFromPropertyTree(PropertyFragment fragment)
        {
            int lastIndex = fragment.Properties.Count-1;
            SqlFragment currentFragment = scope.GetFragment(fragment.Properties[0]);
            if (currentFragment != null)
            {
                for (int i = 1; i < fragment.Properties.Count; i++)
                {
                    SqlFragment f = (currentFragment as InputFragment).GetProperty(fragment.Properties[i]);
                    if (f == null) break;
                    currentFragment = f;
                }
                if (currentFragment is ColumnFragment)
                    return currentFragment as ColumnFragment;
            }
            ColumnFragment col = new ColumnFragment(null, fragment.Properties[lastIndex]);
            return col;
        }

        #endregion

    }
}
