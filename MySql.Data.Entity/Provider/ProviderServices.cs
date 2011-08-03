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
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data;
using MySql.Data.Entity;
using System.Reflection;
using System.Diagnostics;
using MySql.Data.Entity.Properties;
using System.Globalization;

namespace MySql.Data.MySqlClient
{
    internal class MySqlProviderServices : DbProviderServices
    {
        internal static readonly MySqlProviderServices Instance;

        static MySqlProviderServices()
        {
            Instance = new MySqlProviderServices();
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(
            DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            SqlGenerator generator = null;
            if (commandTree is DbQueryCommandTree)
                generator = new SelectGenerator();
            else if (commandTree is DbInsertCommandTree)
                generator = new InsertGenerator();
            else if (commandTree is DbUpdateCommandTree)
                generator = new UpdateGenerator();
            else if (commandTree is DbDeleteCommandTree)
                generator = new DeleteGenerator();
            else if (commandTree is DbFunctionCommandTree)
                generator = new FunctionGenerator();

            string sql = generator.GenerateSQL(commandTree);

            EFMySqlCommand cmd = new EFMySqlCommand();
            cmd.CommandText = sql;
            if (generator is FunctionGenerator)
                cmd.CommandType = (generator as FunctionGenerator).CommandType;

            SetExpectedTypes(commandTree, cmd);

            EdmFunction function = null;
            if (commandTree is DbFunctionCommandTree)
                function = (commandTree as DbFunctionCommandTree).EdmFunction;

            // Now make sure we populate the command's parameters from the CQT's parameters:
            foreach (KeyValuePair<string, TypeUsage> queryParameter in commandTree.Parameters)
            {
                DbParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Key;
                parameter.Direction = ParameterDirection.Input;
                parameter.DbType = Metadata.GetDbType(queryParameter.Value);

                FunctionParameter funcParam;
                if (function != null &&
                    function.Parameters.TryGetValue(queryParameter.Key, false, out funcParam))
                {
                    parameter.ParameterName = funcParam.Name;
                    parameter.Direction = Metadata.ModeToDirection(funcParam.Mode);
                    parameter.DbType = Metadata.GetDbType(funcParam.TypeUsage);
                }
                cmd.Parameters.Add(parameter);
            }

            // Now add parameters added as part of SQL gen 
            foreach (DbParameter p in generator.Parameters)
                cmd.Parameters.Add(p);

            return CreateCommandDefinition(cmd);
        }

        /// <summary>
        /// Sets the expected column types
        /// </summary>
        private void SetExpectedTypes(DbCommandTree commandTree, EFMySqlCommand cmd)
        {
            if (commandTree is DbQueryCommandTree)
                SetQueryExpectedTypes(commandTree as DbQueryCommandTree, cmd);
            else if (commandTree is DbFunctionCommandTree)
                SetFunctionExpectedTypes(commandTree as DbFunctionCommandTree, cmd);
        }

        /// <summary>
        /// Sets the expected column types for a given query command tree
        /// </summary>
        private void SetQueryExpectedTypes(DbQueryCommandTree tree, EFMySqlCommand cmd)
        {
            DbProjectExpression projectExpression = tree.Query as DbProjectExpression;
            if (projectExpression != null)
            {
                EdmType resultsType = projectExpression.Projection.ResultType.EdmType;

                StructuralType resultsAsStructuralType = resultsType as StructuralType;
                if (resultsAsStructuralType != null)
                {
                    cmd.ColumnTypes = new PrimitiveType[resultsAsStructuralType.Members.Count];

                    for (int ordinal = 0; ordinal < resultsAsStructuralType.Members.Count; ordinal++)
                    {
                        EdmMember member = resultsAsStructuralType.Members[ordinal];
                        PrimitiveType primitiveType = member.TypeUsage.EdmType as PrimitiveType;
                        cmd.ColumnTypes[ordinal] = primitiveType;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the expected column types for a given function command tree
        /// </summary>
        private void SetFunctionExpectedTypes(DbFunctionCommandTree tree, EFMySqlCommand cmd)
        {
            if (tree.ResultType != null)
            {
                Debug.Assert(tree.ResultType.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType,
                    Resources.WrongFunctionResultType);

                CollectionType collectionType = (CollectionType)(tree.ResultType.EdmType);
                EdmType elementType = collectionType.TypeUsage.EdmType; 

                if (elementType.BuiltInTypeKind == BuiltInTypeKind.RowType) 
                {
                    ReadOnlyMetadataCollection<EdmMember> members = ((RowType)elementType).Members;
                    cmd.ColumnTypes = new PrimitiveType[members.Count];

                    for (int ordinal = 0; ordinal < members.Count; ordinal++)
                    {
                        EdmMember member = members[ordinal];
                        PrimitiveType primitiveType = (PrimitiveType)member.TypeUsage.EdmType;
                        cmd.ColumnTypes[ordinal] = primitiveType;
                    }

                }
                else if (elementType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType) 
                {
                    cmd.ColumnTypes = new PrimitiveType[1];
                    cmd.ColumnTypes[0] = (PrimitiveType)elementType;
                }
                else
                {
                    Debug.Fail(Resources.WrongFunctionResultType);
                }
            }
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            // we need the connection option to determine what version of the server
            // we are connected to
            bool shouldClose = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                shouldClose = true;
            }
            double version = double.Parse(connection.ServerVersion.Substring(0, 3), CultureInfo.InvariantCulture);
            
            if (shouldClose)
                connection.Close();

            if (version < 5.0) throw new NotSupportedException("Versions of MySQL prior to 5.0 are not currently supported");
            if (version < 5.1) return "5.0";
            if (version < 5.5) return "5.1";
            return "5.5";            
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            return new MySqlProviderManifest(manifestToken);
        }
    }
}
