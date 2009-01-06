using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data;
using MySql.Data.MySqlClient.SQLGeneration;

namespace MySql.Data.MySqlClient
{
    internal class MySqlProviderServices : DbProviderServices
    {
        internal static readonly MySqlProviderServices Instance;

        static MySqlProviderServices()
        {
            Instance = new MySqlProviderServices();
        }


        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
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
        }

/*        protected override DbCommandDefinition CreateDbCommandDefinition(
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
            return CreateCommandDefinition(cmd);
        }
        */

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

            if (version.StartsWith("5")) return "5";
            return "6";
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            return new MySqlProviderManifest(manifestToken);
        }
    }
}
