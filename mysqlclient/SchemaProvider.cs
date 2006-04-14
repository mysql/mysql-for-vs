using System.Data;
using System;

namespace MySql.Data.MySqlClient
{
    internal abstract class SchemaProvider
    {
        protected MySqlConnection connection;

        public SchemaProvider(MySqlConnection connectionToUse)
        {
            connection = connectionToUse;
        }

        public virtual DataTable GetSchema(string collection, String[] restrictions)
        {
            if (connection.State != ConnectionState.Open)
                throw new MySqlException("GetSchema can only be called on an open connection.");

            collection = collection.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            switch (collection)
            {
                case "databases":
                    return GetDatabases();
                case "tables":
                    return GetTables(restrictions);
                case "views":
                    return GetViews(restrictions);
                case "procedures":
                    return GetProcedures(restrictions);
                case "procedure parameters":
                    return GetProcedureParameters(restrictions);
            }
            return HelpCollection();
        }

        public abstract DataTable GetDatabases();
        public abstract DataTable GetTables(string[] restrictions);
        public abstract DataTable GetProcedures(string[] restrictions);
        public abstract DataTable GetProcedureParameters(string[] restrictions);
        public abstract DataTable GetViews(string[] restrictions);
        public abstract DataTable HelpCollection();
    }
}
