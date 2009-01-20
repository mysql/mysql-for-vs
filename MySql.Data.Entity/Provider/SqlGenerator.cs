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
        private List<MySqlParameter> parameters;
        protected string tabs = String.Empty;
        private int parameterCount = 1;
        protected Stack<string> scope = new Stack<string>();
        private BaseStatement current;

        public SqlGenerator()
        {
            parameters = new List<MySqlParameter>();
        }

        #region Properties

        public List<MySqlParameter> Parameters
        {
            get { return parameters; }
        }

        protected virtual BaseStatement Current 
        { 
            get { return current; }
        }

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
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "VariableRefExpression", expression.VariableName));
            SymbolFragment f = new SymbolFragment();
            f.Variable = expression.VariableName;
            return f;
        }

        public override SqlFragment Visit(DbScanExpression expression)
        {
            EntitySetBase target = expression.Target;
            TableFragment fragment = new TableFragment();

            MetadataProperty property;
            if (target.MetadataProperties.TryGetValue("DefiningQuery", true, out property))
                fragment.Text = String.Format("({0})", property.Value);
            else
            {
                string schema = target.EntityContainer.Name;
                string table = target.Name;

                if (target.MetadataProperties.TryGetValue("Schema", true, out property))
                    schema = property.Value as string;
                if (target.MetadataProperties.TryGetValue("Table", true, out property))
                    table = property.Value as string;
                fragment.Text = String.Format("`{0}`.`{1}`", schema, table);
            }
            fragment.Name = scope.Pop();

            return fragment;
        }

        public override SqlFragment Visit(DbPropertyExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "PropertyExpression", expression.Property.Name));
            Push();

            SqlFragment f = expression.Instance.Accept(this);
            SymbolFragment symbolFragment = f as SymbolFragment;
            symbolFragment.Properties.Add(expression.Property.Name);
            Pop();
            return symbolFragment;
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
            SqlFragment left = expression.Left.Accept(this);
            SqlFragment right = expression.Right.Accept(this);
            return left;
        }

        #endregion

        #region DBExpressionVisitor methods normally overridden

        public override SqlFragment Visit(DbUnionAllExpression expression)
        {
            expression.
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

        public override SqlFragment Visit(DbOrExpression expression)
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

        public override SqlFragment Visit(DbLikeExpression expression)
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

        public override SqlFragment Visit(DbIsNullExpression expression)
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

        public override SqlFragment Visit(DbCastExpression expression)
        {
            throw new NotImplementedException();
        }

        public override SqlFragment Visit(DbCaseExpression expression)
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
    }
}
