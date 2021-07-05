// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

/*
 * This file contains class with data source specific information.
 */

using System;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data;
using System.Data.Common;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Represents a custom data source information class for MySQL
  /// </summary>
  public class MySqlDataSourceInformation : AdoDotNetDataSourceInformation
  {
    public const string DataSource = "DataSource";
    private DataTable values;

    /// <summary>
    /// Constructors fills available properties information
    /// </summary>
    /// <param name="connection">Reference to database connection object</param>
    public MySqlDataSourceInformation(DataConnection connection)
      : base(connection)
    {
      AddProperty(DataSource);
      AddProperty(DataSourceVersion);
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
      AddProperty(ParameterPrefix, "@");
      AddProperty(ParameterPrefixInName, true);
      AddProperty(DefaultCatalog, null);
    }

    internal void Refresh()
    {
      EnsureConnected();
      DbConnection c = (DbConnection)Connection.GetLockedProviderObject();
      values = c.GetSchema("DataSourceInformation");
      Connection.UnlockProviderObject();
    }

    #region Value retrieving

    public override object this[string propertyName]
    {
      get
      {
        // data source version can change so we need to 
        // refresh it here
        if (propertyName == "DataSourceVersion")
        {
          if (values == null)
            Refresh();
          return values.Rows[0]["DataSourceProductVersion"];
        }
        else
          return base[propertyName];
      }
    }

    /// <summary>
    /// Called to retrieve property value. Supports following custom properties:
    /// DataSource � MySQL server name.
    /// Database � default schema name.
    /// </summary>
    /// <param name="propertyName">Name of property to retrieve.</param>
    /// <returns>Property value</returns>
    protected override object RetrieveValue(string propertyName)
    {
      EnsureConnected();
      if (propertyName.Equals(DataSource, StringComparison.InvariantCultureIgnoreCase))
      {
        return (ProviderObject as DbConnection).DataSource;
      }
      else if (propertyName.Equals(DefaultSchema, StringComparison.InvariantCultureIgnoreCase))
      {
        return (ProviderObject as DbConnection).Database;
      }
      object value = base.RetrieveValue(propertyName);
      return value;
    }
    #endregion

    private void EnsureConnected()
    {
      if (Connection.State != DataConnectionState.Open)
      {
        Connection.OpenWithDefaultTimeout();
      }
    }
  }
}
