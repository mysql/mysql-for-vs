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

                if (identifierParts.Length == 1) 
                    id = identifierParts[0];
                if (identifierParts.Length == 2 && identifierParts[0] == c.Database)
                    id = identifierParts[1];
                if (identifierParts.Length == 3 && identifierParts[1] == c.Database)
                    id = identifierParts[2];
            }
            if (id == String.Empty || forDisplay)
                id = base.BuildString(typeName, identifierParts, forDisplay);
            return id;
        }

        protected override string FormatPart(string typeName, object identifierPart, bool withQuotes)
        {
            DbConnection c = connection.ConnectionSupport.ProviderObject as DbConnection;
            if (typeName == "Table" && identifierPart.Equals(c.Database))
                return null;
            return base.FormatPart(typeName, identifierPart, withQuotes);
        }
    }
}
