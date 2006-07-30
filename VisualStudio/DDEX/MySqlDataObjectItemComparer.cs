using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data object item comparer.  SQL Server is unique
	/// in that it has different case sensitivity depending on the scope of
	/// the identifier or object item.  The collation of the server also
	/// plays a part in comparing data for purposes of sorting, but this
	/// sample does not account for that.
	/// </summary>
	internal class MySqlDataObjectItemComparer : DataObjectItemComparer
	{
		public MySqlDataObjectItemComparer(DataConnection connection)
		{
            Logger.WriteLine("MySqlDataObjectItemComparer::ctor");
            Debug.Assert(connection != null);
			this._connection = connection;
		}

		/// <summary>
		/// This method compares two identifier parts.  It takes into account
		/// that the case sensitivity on the server as whole may be different
		/// to the case sensitivity of a specific database.
		/// </summary>
		public override int Compare(string typeName, object[] identifier, int identifierPart, object value)
		{
            Logger.WriteLine("MySqlDataObjectItemComparer::Compare");
            if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}

			string strValue = value as string;

			// Comparison of identifiers is based on server case sensitivity
			// and the specific database sensitivity.
            if (!typeName.Equals(MySqlDataObjectTypes.User, StringComparison.InvariantCultureIgnoreCase) &&
				identifierPart == 0)
			{
				// Use server case sensitivity for comparison
				return String.Compare(identifier[0] as string, strValue, !IsCaseSensitive, CultureInfo.CurrentCulture);
			}
			else
			{
				// Use specific database case sensitivity for comparison
				string database = null;
                if (!typeName.Equals(MySqlDataObjectTypes.User, StringComparison.InvariantCultureIgnoreCase))
				{
					database = identifier[0] as string;
				}
				bool ignoreCase = !IsCaseSensitiveIn(database);
				return String.Compare(identifier[identifierPart] as string, strValue, ignoreCase, CultureInfo.CurrentCulture);
			}
		}

		private bool IsCaseSensitive
		{
			get
			{
                Logger.WriteLine("MySqlDataObjectItemComparer::get_IsCaseSensitive");
                if (!_gotCaseSensitive)
				{
                    System.Windows.Forms.MessageBox.Show("IsCaseSenstivie");
                    return false;
/*					SqlConnection conn = _connection.ConnectionSupport.ProviderObject as SqlConnection;
					Debug.Assert(conn != null, "The provider object is not the correct type.");
					if (conn != null)
					{
						if (conn.State != ConnectionState.Open)
						{
							_connection.Open();
						}
						SqlCommand comm = conn.CreateCommand();
						comm.CommandText = "SELECT CONVERT(bit, CHARINDEX(N'_CS_', CAST(SERVERPROPERTY('Collation') AS nvarchar(255))))";
						_isCaseSensitive = (bool)comm.ExecuteScalar();
					}
					_gotCaseSensitive = true;*/
				}
				return _isCaseSensitive;
			}
		}

		private bool IsCaseSensitiveIn(string database)
		{
            Logger.WriteLine("MySqlDataObjectItemComparer::IsCaseSensitiveIn");
            string theDatabase = null;
			if (database == null)
			{
				theDatabase = _connection.SourceInformation[DataSourceInformation.DefaultCatalog] as string;
			}
			else
			{
				theDatabase = database;
			}
			if (!_caseSensitivities.ContainsKey(theDatabase))
			{
/*				SqlConnection conn = _connection.ConnectionSupport.ProviderObject as SqlConnection;
				Debug.Assert(conn != null, "The provider object is not the correct type.");
				if (conn != null)
				{
					if (conn.State != ConnectionState.Open)
					{
						_connection.Open();
					}
					SqlCommand comm = conn.CreateCommand();
					comm.CommandText = "SELECT CONVERT(bit, CHARINDEX(N'_CS_', CAST(DATABASEPROPERTYEX(N'" + theDatabase.Replace("'", "''") + "', 'Collation') AS nvarchar(255))))";
					_caseSensitivities[theDatabase] = (bool)comm.ExecuteScalar();
				}
				else
				{
					_caseSensitivities[theDatabase] = false;
				}*/
			}
			return _caseSensitivities[theDatabase];
		}

		private bool _isCaseSensitive = false;
		private bool _gotCaseSensitive = false;
		private Dictionary<string, bool> _caseSensitivities = new Dictionary<string, bool>();
		private DataConnection _connection;
	}
}
