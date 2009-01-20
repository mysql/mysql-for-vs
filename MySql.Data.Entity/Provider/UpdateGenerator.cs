using System;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using MySql.Data.MySqlClient;

namespace MySql.Data.Entity
{
    class UpdateGenerator : SqlGenerator 
    {
        public override string GenerateSQL(DbCommandTree tree)
        {
            DbUpdateCommandTree commandTree = tree as DbUpdateCommandTree;

            UpdateStatement statement = new UpdateStatement();

            statement.Target = commandTree.Target.Expression.Accept(this);
            statement.Target.Name = commandTree.Target.VariableName;

            foreach (DbSetClause setClause in commandTree.SetClauses)
            {
                statement.Properties.Add(setClause.Property.Accept(this));
                statement.Values.Add(setClause.Value.Accept(this));
            }

            statement.Where = commandTree.Predicate.Accept(this);

            return statement.GenerateSQL();
        }
    }
}
