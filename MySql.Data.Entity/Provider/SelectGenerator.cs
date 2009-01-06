using System;
using System.Text;
using System.Diagnostics;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient.Generator
{
    class SelectGenerator : SqlGenerator 
    {
        private StringBuilder input;
        private StringBuilder projection;

        public override string GenerateSQL(DbCommandTree tree)
        {
            DbQueryCommandTree commandTree = tree as DbQueryCommandTree;

            input = new StringBuilder();
            projection = new StringBuilder();

            DbExpression e = commandTree.Query;
            switch (commandTree.Query.ExpressionKind)
            {
                case DbExpressionKind.Project:
                    e.Accept(this);
                    break;
            }

            return String.Format("SELECT {0} FROM {1}", projection, input);
        }

        public override void Visit(DbConstantExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}", tabs, "ConstantExpress"));
            //            current.Append(expression.Value);

            // create a parameter and save it for later when we are 
            // making our command that will be returned.
            MySqlParameter p = new MySqlParameter();
            p.ParameterName = CreateUniqueParameterName();
            p.DbType = GetDbType(expression.ResultType);
            p.Value = expression.Value;
            Parameters.Add(p);

            // now add the parameter name to our SQL stream
            current.Append(p.ParameterName);
        }

        public override void Visit(DbFilterExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "FilterExpression", ""));
            tabs += "-";
            expression.Input.Expression.Accept(this);
            //current.AppendFormat(" AS {0}", expression.Input.VariableName);

            current.Append(" WHERE ");
            expression.Predicate.Accept(this);
            tabs = tabs.Substring(1);
        }

        public override void Visit(DbGroupByExpression expression)
        {
            StringBuilder i = new StringBuilder();
            current = i;
            expression.Input.Expression.Accept(this);

            CollectionType ct = (CollectionType)expression.ResultType.EdmType;
            RowType rt = (RowType)ct.TypeUsage.EdmType;
//            RowType groupByType = MetadataHelpers.GetEdmType<RowType>(MetadataHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);


            using (IEnumerator<EdmProperty> members = rt.Properties.GetEnumerator())
            {
                members.MoveNext();
            }


            foreach (DbExpression key in expression.Keys)
            {
//                key.
            }

            Trace.WriteLine(String.Format("{0}{1}", tabs, "GroupByExpression"));
            foreach (DbAggregate a in expression.Aggregates)
            {
                DbFunctionAggregate fa = a as DbFunctionAggregate;
                if (fa == null) throw new NotSupportedException();

                current.Append(fa.Function.Name);
                current.Append("(");
                if (fa.Distinct)
                    current.Append("DISTINCT ");
                Push();
                fa.Arguments[0].Accept(this);
                Pop();
                current.Append(")");
            }
        }

        public override void Visit(DbJoinExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "JoinExpression", ""));
            Push();
            expression.Left.Expression.Accept(this);
            current.AppendFormat(" AS `{0}`", expression.Left.VariableName);

            current.Append(" INNER JOIN ");
            expression.Right.Expression.Accept(this);
            current.AppendFormat(" AS `{0}`", expression.Right.VariableName);

            // now handle the ON case
            current.Append(" ON ");
            expression.JoinCondition.Accept(this);
            Pop();
        }

        public override void Visit(DbNewInstanceExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "NewInstanceExpression", ""));
            Push();
            RowType row = expression.ResultType.EdmType as RowType;

            string separator = "";
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                // let the expression do its thing
                current.Append(separator);
                expression.Arguments[i].Accept(this);
                current.AppendFormat(" AS {0}", QuoteIdentifier(row.Properties[i].Name));
                separator = ", ";
            }
            Pop();
        }

        public override void Visit(DbParameterReferenceExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "ParameterReferenceExpression", expression.ParameterName));
            current.AppendFormat("@{0}", expression.ParameterName);
        }

        public override void Visit(DbProjectExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "ProjectExpression", ""));
            Push();

            // handle from clause
            current = input;
            expression.Input.Expression.Accept(this);
            current.AppendFormat(" AS {0}", QuoteIdentifier(expression.Input.VariableName));

            // now handle projection
            current = projection;
            expression.Projection.Accept(this);
            Pop();
        }

        public override void Visit(DbPropertyExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "PropertyExpression", expression.Property.Name));
            Push();
            expression.Instance.Accept(this);
            current.Append(QuoteIdentifier(expression.Property.Name));
            Pop();
        }

        public override void Visit(DbScanExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "ScanExpression", expression.Target.Name));
            EntitySetBase target = expression.Target;
            current.AppendFormat("`{0}`.`{1}`", target.EntityContainer, target.Name);
        }

        public override void Visit(DbVariableReferenceExpression expression)
        {
            Trace.WriteLine(String.Format("{0}{1}-{2}", tabs, "VariableRefExpression", expression.VariableName));
            current.AppendFormat("{0}.",
                QuoteIdentifier(expression.VariableName));
        }
    }
}
