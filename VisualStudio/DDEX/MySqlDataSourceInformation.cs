using System;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data source information class that is able to
	/// provide data source information values that require some form of
	/// computation, perhaps based on an active connection.
	/// </summary>
	internal class MySqlDataSourceInformation : AdoDotNetDataSourceInformation
	{
		public MySqlDataSourceInformation(DataConnection connection) : base(connection)
		{
            Logger.WriteLine("MysqlDataSourceInformation::ctor default schema=" + DefaultSchema);
            AddProperty(DefaultSchema);
			AddProperty(SupportsAnsi92Sql, true);
			AddProperty(SupportsQuotedIdentifierParts, true);
			AddProperty(IdentifierOpenQuote, "`");
			AddProperty(IdentifierCloseQuote, "`");
			AddProperty(ServerSeparator, ".");
			AddProperty(CatalogSupported, false);
			AddProperty(CatalogSupportedInDml, false);
			AddProperty(SchemaSupported, true);
			AddProperty(SchemaSupportedInDml, true);
			AddProperty(SchemaSeparator, ".");
			AddProperty(ParameterPrefix, "?");
			AddProperty(ParameterPrefixInName, true);
		}

		/// <summary>
		/// RetrieveValue is called once per property that was identified
		/// as existing but without a value (specified in the constructor).
		/// For the purposes of this sample, only one property needs to be
		/// computed - DefaultSchema.  To retrieve this value a SQL statement
		/// is executed.
		/// </summary>
		protected override object RetrieveValue(string propertyName)
		{
            Logger.WriteLine("MysqlDataSourceInformation::RetrieveValue propertyName=" + propertyName);
			if (propertyName.Equals(DefaultSchema, StringComparison.InvariantCultureIgnoreCase))
			{
				if (Connection.State != DataConnectionState.Open)
				{
					Connection.Open();
				}
                DbConnection conn = ConnectionSupport.ProviderObject as DbConnection;
                string type = conn.GetType().ToString();

				Debug.Assert(type == "MySql.Data.MySqlClient.MySqlConnection", 
                    "The provider object is not the correct type.");

				if (conn != null)
				{
                    DbCommand cmd = conn.CreateCommand();
					try
					{
						cmd.CommandText = "SELECT database()";
						return cmd.ExecuteScalar() as string;
					}
					catch (Exception)
					{
						// We let the base class apply default behavior
					}
				}
			}
			return base.RetrieveValue(propertyName);
		}
	}
}
