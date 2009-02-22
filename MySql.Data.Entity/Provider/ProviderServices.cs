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
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data;
//using MySql.Data.MySqlClient.SQLGeneration;
using MySql.Data.Entity;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
    internal class MySqlProviderServices : DbProviderServices
    {
        internal static readonly MySqlProviderServices Instance;

        static MySqlProviderServices()
        {
            Instance = new MySqlProviderServices();
        }


/*        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            List<DbParameter> parameters;
            CommandType commandType;

            string sql = SqlGenerator.GenerateSql(commandTree, out parameters, out commandType);
            MySqlCommand cmd = new MySqlCommand(sql);

            // Now make sure we populate the command's parameters from the CQT's parameters:
            foreach (KeyValuePair<string, TypeUsage> queryParameter in commandTree.Parameters)
            {
                DbParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Key;
                parameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(parameter);
            }

            // Now add parameters added as part of SQL gen 
            if (parameters != null)
                foreach (DbParameter p in parameters)
                    cmd.Parameters.Add(p);

            return CreateCommandDefinition(cmd);
        }*/

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

            string sql = generator.GenerateSQL(commandTree);

            MySqlCommand cmd = new MySqlCommand(sql);

            FieldInfo fi = cmd.GetType().GetField("EFCrap", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(cmd, true);

            // Now make sure we populate the command's parameters from the CQT's parameters:
            foreach (KeyValuePair<string, TypeUsage> queryParameter in commandTree.Parameters)
            {
                DbParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Key;
                parameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(parameter);
            }

            // Now add parameters added as part of SQL gen 
            foreach (DbParameter p in generator.Parameters)
                cmd.Parameters.Add(p);

            if (commandTree is DbInsertCommandTree)
            {
                MySqlConnection c = new MySqlConnection("server=localhost;uid=root;database=test");
                c.Open();
                cmd.Connection = c;
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                reader.Close();
                int i = reader.RecordsAffected;
            }

            return CreateCommandDefinition(cmd);
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
            string version = connection.ServerVersion;
            if (shouldClose)
                connection.Close();
            if (version.StartsWith("6")) return "6.0";
            if (version.StartsWith("5")) return "5.0";
            throw new NotSupportedException("Versions of MySQL prior to 5.0 are not currently supported");
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            return new MySqlProviderManifest(manifestToken);
        }
    }
}
