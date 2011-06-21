// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace MySql.Data.Entity
{
    abstract class SqlGenerator : DbExpressionVisitor<SqlFragment>
    {
        protected string tabs = String.Empty;
        private int parameterCount = 1;
        protected Scope scope = new Scope();
        protected int propertyLevel;
        protected Dictionary<EdmMember, SqlFragment> values;

        public SqlGenerator()
        {
            Parameters = new List<MySqlParameter>();
        }

        #region Properties

        public List<MySqlParameter> Parameters { get; private set; }
//        protected SymbolTable Symbols { get; private set; }

        #endregion

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

            ColumnFragment column = new ColumnFragment(null, fragment.LastProperty);
            column.PropertyFragment = fragment;
            InputFragment input = scope.FindInputFromProperties(fragment);
            if (input != null)
                column.TableName = input.Name;

            // now we need to check if our column name was possibly renamed
            if (input is TableFragment) return column;

            SelectStatement select = input as SelectStatement;
            UnionFragment union = input as UnionFragment;

            if (select != null)
                select.HasDifferentNameForColumn(column);
            else if (union != null)
                union.HasDifferentNameForColumn(column);

            // input is a table, selectstatement, or unionstatement
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
            FunctionProcessor gen = new FunctionProcessor();
            return gen.Generate(expression, this);
        }

        public override SqlFragment Visit(DbConstantExpression expression)
        {
            PrimitiveTypeKind pt = ((PrimitiveType)expression.ResultType.EdmType).PrimitiveTypeKind;
            string literal = Metadata.GetNumericLiteral(pt, expression.Value);
            if (literal != null)
                return new LiteralFragment(literal);
            else if (pt == PrimitiveTypeKind.Boolean)
                return new LiteralFragment((bool)expression.Value ? "1" : "0");
            else
            {
                // use a parameter for non-numeric types so we get proper
                // quoting
                MySqlParameter p = new MySqlParameter();
                p.ParameterName = CreateUniqueParameterName();
                p.DbType = Metadata.GetDbType(expression.ResultType);
                p.Value = Metadata.NormalizeValue(expression.ResultType, expression.Value);
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

        public override SqlFragment Visit(DbIntersectExpression expression)
        {
            throw new NotSupportedException();
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

        protected void VisitNewInstanceExpression(SelectStatement select,
            DbNewInstanceExpression expression)
        {
            Debug.Assert(expression.ResultType.EdmType is RowType);

            RowType row = expression.ResultType.EdmType as RowType;

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                ColumnFragment col; 

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

        public override SqlFragment Visit(DbTreatExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbRelationshipNavigationExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbRefExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbOfTypeExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbIsOfExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbRefKeyExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbEntityRefExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbExceptExpression expression)
        {
            throw new NotSupportedException();
        }

        public override SqlFragment Visit(DbExpression expression)
        {
            throw new InvalidOperationException();
        }

        public override SqlFragment Visit(DbDerefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbApplyExpression expression)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region DBExpressionVisitor methods normally overridden

        public override SqlFragment Visit(DbUnionAllExpression expression)
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

        public override SqlFragment Visit(DbQuantifierExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbProjectExpression expression)
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

        public override SqlFragment Visit(DbGroupByExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbFilterExpression expression)
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

        public override SqlFragment Visit(DbCrossJoinExpression expression)
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

            if (name != null)
                scope.Add(name, inputFragment);

            return inputFragment;
        }

        protected SelectStatement GenerateReturningSql(DbModificationCommandTree tree, DbExpression returning)
        {
            SelectStatement select = new SelectStatement();

            Debug.Assert(returning is DbNewInstanceExpression);
            VisitNewInstanceExpression(select, returning as DbNewInstanceExpression);

            select.From = (InputFragment)tree.Target.Expression.Accept(this);

            ListFragment where = new ListFragment();
            where.Append(" row_count() > 0");

            EntitySetBase table = ((DbScanExpression)tree.Target.Expression).Target;
            bool foundIdentity = false;
            foreach (EdmMember keyMember in table.ElementType.KeyMembers)
            {
                SqlFragment value;
                if (!values.TryGetValue(keyMember, out value))
                {
                    if (foundIdentity)
                        throw new NotSupportedException();
                    foundIdentity = true;
                    value = new LiteralFragment("last_insert_id()");
                }
                where.Append(String.Format(" AND `{0}`=", keyMember));
                where.Append(value);
            }
            select.Where = where;
            return select;
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

        #endregion

    }
}
