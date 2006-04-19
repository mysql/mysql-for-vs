using System;
using System.Data;
using System.Text;

namespace MySql.Data.MySqlClient
{
    class NonISSchemaProvider : SchemaProvider
    {
        public NonISSchemaProvider(MySqlConnection connection) : base(connection)
        {
        }

        public override DataTable GetDatabases()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable GetTables(string[] restrictions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable GetColumns(string[] restrictions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable HelpCollection()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataTable GetProcedures(string[] restrictions)
        {
            throw new MySqlException("The PROCEDURES collection is only supported on MySQL version 5.0 and later");
        }

        public override DataTable GetProcedureParameters(string[] restrictions)
        {
            throw new MySqlException("The PROCEDURE PARAMETERS collection is only supported on MySQL version 5.0 and later");
        }

        public override DataTable GetViews(string[] restrictions)
        {
            throw new MySqlException("The VIEWS collection is only supported on MySQL version 5.0 and later");
        }

        public override DataTable GetTriggers(string[] restrictions)
        {
            throw new MySqlException("The TRIGGERS collection is only supported on MySQL version 5.0 and later");
        }
    }
}
