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
