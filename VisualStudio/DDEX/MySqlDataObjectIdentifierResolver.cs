using System;
using System.Data;
using System.Diagnostics;
using System.Collections;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data object identifier resolver that correctly
	/// expands an identifier to its complete form or contracts it to its
	/// minimal form.  This is required for certain built in data design
	/// scenarios that are initialized with only a partial identifier and
	/// then try to match this identifier with an object from the server.
	/// It is also used to provide "clean" forms of a full identifier,
	/// where only the necessary parts are needed.
	/// </summary>
	internal class MySqlDataObjectIdentifierResolver : DataObjectIdentifierResolver
	{
		public MySqlDataObjectIdentifierResolver(DataConnection connection)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierResolver::ctor");
            Debug.Assert(connection != null);
			this._connection = connection;
		}

		/// <summary>
		/// SQL Server connections are always within the context of a current
		/// database and default schema.  This method expands identifiers
		/// that are missing database or schema parts by adding the defaults
		/// appropriately.
		/// </summary>
		protected override object[] QuickExpandIdentifier(string typeName, object[] partialIdentifier)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierResolver::QuickExpandIdentifier");
            if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}

			// Create an identifier array of the correct full length based on
			// the object type
			object[] identifier = null;
			int length = GetIdentifierLength(typeName);
			if (length == -1)
			{
				throw new NotSupportedException();
			}
			identifier = new object[length];

			// If the input identifier is not null, copy it to the full
			// identifier array.  If the input identifier's length is less
			// than the full length we assume the more specific parts are
			// specified and thus copy into the rightmost portion of the
			// full identifier array.
			if (partialIdentifier != null)
			{
				if (partialIdentifier.Length > length)
				{
					throw new InvalidOperationException();
				}
				partialIdentifier.CopyTo(identifier, length - partialIdentifier.Length);
			}

			if (length > 0)
			{
				// Fill in the current database if not specified
				if (!(identifier[0] is string))
				{
					identifier[0] = _connection.SourceInformation[DataSourceInformation.DefaultCatalog] as string;
				}
			}

			if (length > 1)
			{
				// Fill in the default schema if not specified
				if (!(identifier[1] is string))
				{
					identifier[1] = _connection.SourceInformation[DataSourceInformation.DefaultSchema] as string;
				}
			}

			return identifier;
		}

		/// <summary>
		/// SQL Server connections are always within the context of a current
		/// database and default schema.  This method contracts identifiers
		/// that contain current database or default schema parts by removing
		/// them as appropriate.
		/// </summary>
		protected override object[] QuickContractIdentifier(string typeName, object[] fullIdentifier)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierResolver::QuickContractIdentifier");
            if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (typeName == MySqlDataObjectTypes.Root ||
				typeName == MySqlDataObjectTypes.User)
			{
				// There is no contraction available
				return base.QuickContractIdentifier(typeName, fullIdentifier);
			}

			object[] identifier = null;
			int length = GetIdentifierLength(typeName);
			if (length == -1)
			{
				throw new NotSupportedException();
			}
			identifier = new object[length];
			if (fullIdentifier != null)
			{
				fullIdentifier.CopyTo(identifier, length - fullIdentifier.Length);
			}
			if (identifier.Length > 0 && identifier[0] != null)
			{
				string database = _connection.SourceInformation[DataSourceInformation.DefaultCatalog] as string;
				if (_connection.ObjectItemComparer.Compare(MySqlDataObjectTypes.Root, identifier, 0, database) == 0)
				{
					identifier[0] = null;
				}
			}
			if (identifier.Length > 1 && identifier[1] != null)
			{
				string schema = _connection.SourceInformation[DataSourceInformation.DefaultSchema] as string;
				if (_connection.ObjectItemComparer.Compare(MySqlDataObjectTypes.Root, identifier, 1, schema) == 0)
				{
					identifier[1] = null;
				}
			}

			return identifier;
		}

		internal static int GetIdentifierLength(string typeName)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierResolver::GetIdentifierLength");
            switch (typeName)
			{
				case MySqlDataObjectTypes.Root:
					return 0;
				case MySqlDataObjectTypes.User:
					return 1;
				case MySqlDataObjectTypes.Table:
                case MySqlDataObjectTypes.View:
                case MySqlDataObjectTypes.StoredProcedure:
                case MySqlDataObjectTypes.Function:
					return 3;
                case MySqlDataObjectTypes.Column:
                case MySqlDataObjectTypes.Index:
                case MySqlDataObjectTypes.ForeignKey:
                case MySqlDataObjectTypes.ViewColumn:
                case MySqlDataObjectTypes.StoredProcedureParameter:
                case MySqlDataObjectTypes.StoredProcedureColumn:
                case MySqlDataObjectTypes.FunctionParameter:
                case MySqlDataObjectTypes.FunctionColumn:
					return 4;
                case MySqlDataObjectTypes.IndexColumn:
                case MySqlDataObjectTypes.ForeignKeyColumn:
					return 5;
				default:
					Debug.Fail("Unknown SQL data object type.");
					return -1;
			}
		}

		private DataConnection _connection;
	}
}
