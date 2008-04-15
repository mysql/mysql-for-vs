using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
//using MySql.Data.MySqlClient.Generator;
using MySql.Data.MySqlClient.SqlGeneration;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data;

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
            DbConnection connection, DbCommandTree commandTree)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            List<DbParameter> parameters;
            string sql = SqlGenerator.GenerateSql(commandTree, out parameters);

            MySqlCommand cmd = new MySqlCommand(sql, connection as MySql.Data.MySqlClient.MySqlConnection);

            // Now make sure we populate the command's parameters from the CQT's parameters:
            foreach (KeyValuePair<string, TypeUsage> queryParameter in commandTree.Parameters)
            {
                DbParameter parameter = cmd.CreateParameter();
                parameter.ParameterName = queryParameter.Key;
                parameter.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(parameter);
            }

            // Now add parameters added as part of SQL gen (note: this feature is only safe for DML SQL gen which
            // does not support user parameters, where there is no risk of name collision)
            if (null != parameters && 0 < parameters.Count)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }
            return CreateCommandDefinition(cmd);
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            return new MySqlProviderManifest();
        }

        protected override DbProviderManifest GetDbProviderManifest(DbConnection connection)
        {
            return new MySqlProviderManifest();
        }
    }
}
