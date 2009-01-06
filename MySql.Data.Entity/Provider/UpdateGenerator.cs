using System;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;

namespace MySql.Data.MySqlClient.Generator
{
    class UpdateGenerator : SqlGenerator 
    {
        public override string GenerateSQL(DbCommandTree tree)
        {
            DbUpdateCommandTree commandTree = tree as DbUpdateCommandTree;

            StringBuilder setSql = new StringBuilder("UPDATE ");

            current = setSql;
            commandTree.Target.Expression.Accept(this);
            
            current.Append(" SET ");

            string separator = String.Empty;
            foreach (DbSetClause setClause in commandTree.SetClauses)
            {
                current.Append(separator);
                setClause.Property.Accept(this);
                current.Append("=");
                setClause.Value.Accept(this);
                separator = ",";
            }

            StringBuilder whereSql = new StringBuilder();
            current = whereSql;
            // now process the where clause
            commandTree.Predicate.Accept(this);

            if (current.Length > 0)
                setSql.AppendFormat(" WHERE {0}", current.ToString());

            return setSql.ToString();
        }

        public override void Visit(DbComparisonExpression expression)
        {
            expression.Left.Accept(this);
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Equals:
                    current.Append("=");
                    break;
                case DbExpressionKind.LessThan:
                    current.Append("<");
                    break;
                case DbExpressionKind.GreaterThan:
                    current.Append(">");
                    break;
            }
            expression.Right.Accept(this);
        }

        public override void Visit(DbConstantExpression expression)
        {
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

        public override void Visit(DbPropertyExpression expression)
        {
            current.Append(QuoteIdentifier(expression.Property.Name));
        }

    }
}
