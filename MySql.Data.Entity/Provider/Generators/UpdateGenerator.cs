// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MySql.Data.Entity
{
    class UpdateGenerator : SqlGenerator 
    {
        public override string GenerateSQL(DbCommandTree tree)
        {
            DbUpdateCommandTree commandTree = tree as DbUpdateCommandTree;

            UpdateStatement statement = new UpdateStatement();

            //scope.Push(commandTree.Target.VariableName);
            statement.Target = commandTree.Target.Expression.Accept(this);

            foreach (DbSetClause setClause in commandTree.SetClauses)
            {
                statement.Properties.Add(setClause.Property.Accept(this));
                DbExpression value = setClause.Value;
                SqlFragment valueFragment = value.Accept(this);
                statement.Values.Add(valueFragment);

                if (values == null)
                    values = new Dictionary<EdmMember, SqlFragment>();

                if (value.ExpressionKind != DbExpressionKind.Null)
                {
                    EdmMember property = ((DbPropertyExpression)setClause.Property).Property;
                    values.Add(property, valueFragment);
                }
            }

            statement.Where = commandTree.Predicate.Accept(this);

            if (commandTree.Returning != null)
                statement.ReturningSelect = GenerateReturningSql(commandTree, commandTree.Returning);

            return statement.ToString();
        }
    }
}
