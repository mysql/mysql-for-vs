using System;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents customization of the standard data connection properties
	/// implementation.  This class overrides those methods that cannot be
	/// provided by the base class due to limitations in the ADO .NET
	/// connection string builder.
	/// </summary>
	internal class MySqlConnectionProperties : AdoDotNetConnectionProperties
	{
		public MySqlConnectionProperties() : base("MySql.Data.MySqlClient")
		{
            Logger.WriteLine("MySqlConnectionProperties::ctor1");
        }

		public MySqlConnectionProperties(string connectionString) : base("MySql.Data.MySqlClient", connectionString)
		{
            Logger.WriteLine("MySqlConnectionProperties::ctor2 with connstr = " + connectionString);
        }

		/// <summary>
		/// Specifies those properties that are considered fundamental for
		/// creating a connection.
		/// </summary>
		public override string[] GetBasicProperties()
		{
            Logger.WriteLine("MySqlConnectionProperties::GetBasicProperties");
            return new string[] { "Server", "User ID", "Password", "Database" };
		}

		/// <summary>
		/// Indicates when enough properties are set that a connection could
		/// potentially be opened using the resulting connection string.
		/// </summary>
		public override bool IsComplete
		{
			get
			{
                Logger.WriteLine("MySqlConnectionProperties::get_IsComplete");

				if (!(this["Server"] is string) ||
					(this["Server"] as string).Length == 0)
				{
					return false;
				}
				if (!(this["User ID"] is string) ||
					(this["User ID"] as string).Length == 0)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Correctly implements the EquivalentTo method to only examine the
		/// important properties in the string rather than all of them (the
		/// default behavior).
		/// </summary>
		public override bool EquivalentTo(DataConnectionProperties connectionProperties)
		{
            Logger.WriteLine("MySqlConnectionProperties::EquivalentTo");
            if (connectionProperties == null || !(connectionProperties is MySqlConnectionProperties))
			{
				return false;
			}

			string dataSourceA = this["Server"] as string;
			string dataSourceB = connectionProperties["Server"] as string;
			if (String.Compare(dataSourceA, dataSourceB, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				if (dataSourceA != null)
				{
					dataSourceA = dataSourceA.ToUpperInvariant();
					if (dataSourceA.Equals(".", StringComparison.InvariantCulture) ||
						dataSourceA.StartsWith(".\\"))
					{
						dataSourceA = Environment.MachineName.ToUpperInvariant() + dataSourceA.Substring(".".Length);
					}
					else if (dataSourceA.Equals("(LOCAL)", StringComparison.InvariantCulture) ||
						dataSourceA.StartsWith("(LOCAL)\\"))
					{
						dataSourceA = Environment.MachineName.ToUpperInvariant() + dataSourceA.Substring("(LOCAL)".Length);
					}
				}
				if (dataSourceB != null)
				{
					dataSourceB = dataSourceB.ToUpperInvariant();
					if (dataSourceB.Equals(".", StringComparison.InvariantCulture) ||
						dataSourceB.StartsWith(".\\"))
					{
						dataSourceB = Environment.MachineName.ToUpperInvariant() + dataSourceB.Substring(".".Length);
					}
					else if (dataSourceB.Equals("(LOCAL)", StringComparison.InvariantCulture) ||
						dataSourceB.StartsWith("(LOCAL)\\"))
					{
						dataSourceB = Environment.MachineName.ToUpperInvariant() + dataSourceB.Substring("(LOCAL)".Length);
					}
				}
				if (String.Compare(dataSourceA, dataSourceB, StringComparison.InvariantCulture) != 0)
				{
					return false;
				}
			}

			string userIdA = this["User ID"] as string;
			string userIdB = connectionProperties["User ID"] as string;
			if (String.Compare(userIdA, userIdB, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}

			string initialCatalogA = this["Database"] as string;
			string initialCatalogB = connectionProperties["Database"] as string;
			if (String.Compare(initialCatalogA, initialCatalogB, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}

			return true;
		}
	}
}
