using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using MySql.Data.MySqlClient;

namespace MySql.Data.Entity
{
    class DeleteGenerator : SqlGenerator 
    {
        //private StringBuilder sql;

        public override string GenerateSQL(DbCommandTree tree)
        {
            DbDeleteCommandTree commandTree = tree as DbDeleteCommandTree;

       //     StringBuilder commandText = new StringBuilder(s_commandTextBuilderInitialCapacity);
            //ExpressionTranslator translator = new ExpressionTranslator(commandText, tree,
                //null != tree.Returning);

            DeleteStatement statement = new DeleteStatement();
            statement.Target = commandTree.Target.Expression.Accept(this);
            statement.Target.Name = commandTree.Target.VariableName;

            statement.Where = commandTree.Predicate.Accept(this);

            return statement.GenerateSQL();

/*            sql.Append("(");
            foreach (DbSetClause setClause in commandTree.SetClauses)
            {
                setClause.Property.Accept(this);
                sql.Append(",");
            }
            sql[sql.Length-1] = ')';

            // values c1, c2, ...
            //first = true;
            sql.Append(" VALUES (");
            foreach (DbSetClause setClause in commandTree.SetClauses)
            {
                setClause.Value.Accept(this);
                sql.Append(",");

                //translator.RegisterMemberValue(setClause.Property, setClause.Value);
            }
            sql[sql.Length - 1] = ')';

            // generate returning sql
/*            GenerateReturningSql(commandText, tree, translator, tree.Returning); 

            parameters = translator.Parameters;
            return commandText.ToString();*/
            //return sql.ToString();
        }
    }
}
