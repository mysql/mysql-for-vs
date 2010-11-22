using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
    public class MySqlDataObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        private DataConnection connection;

        public MySqlDataObjectIdentifierConverter(DataConnection c)
            : base(c)
        {
            connection = c;
        }

        protected override string BuildString(string typeName, string[] identifierParts, bool forDisplay)
        {
            string id = String.Empty;

            if (typeName == "Table")
            {
                DbConnection c = connection.ConnectionSupport.ProviderObject as DbConnection;
                string dbName = FormatPart("Table", c.Database, true);

                if (identifierParts.Length == 1) 
                    id = identifierParts[0];
                if (identifierParts.Length == 2 && identifierParts[0] == dbName)
                    id = identifierParts[1];
                if (identifierParts.Length == 3 && identifierParts[1] == dbName)
                    id = identifierParts[2];
            }
            if (id == String.Empty || forDisplay)
                id = base.BuildString(typeName, identifierParts, forDisplay);
            return id;
        }
    }
}
