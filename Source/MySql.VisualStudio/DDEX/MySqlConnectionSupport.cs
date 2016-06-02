// Copyright © 2008, 2016 Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

/*
 * This file contains implementation of custom connection support for MySQL.
 */

using System;
using System.Data;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DDEX
{
  public class MySqlConnectionSupport : AdoDotNetConnectionSupport
  {
    private MySqlDataSourceInformation _sourceInformation = null;

    public MySqlConnectionSupport()
      : base(MySqlConnectionProperties.InvariantName)
    {
    }

    public override void Initialize(object providerObj)
    {
      if (providerObj == null)
      {
        providerObj = new MySqlConnection();
      }

      base.Initialize(providerObj);
    }

    /// <summary>
    /// Retrieves a service of the specified type. Following services are 
    /// supported:
    /// DataViewSupport � information about view schema.
    /// DataObjectSupport � information about object model.
    /// </summary>
    /// <param name="serviceType">A service type.</param>
    /// <returns>
    /// Returns the service of the specified type, or returns a null reference 
    /// if no service was found. 
    /// </returns>
    protected override object GetServiceImpl(Type serviceType)
    {
      if (serviceType == typeof(DataViewSupport))
      {
        return new MySqlDataViewSupport();
      }

      if (serviceType == typeof(DataObjectSupport))
      {
        return new MySqlDataObjectSupport();
      }

      if (serviceType == typeof(DataSourceInformation))
      {
        return new MySqlDataSourceInformation(Site as DataConnection);
      }

      if (serviceType == typeof(DataObjectIdentifierConverter))
      {
        return new MySqlDataObjectIdentifierConverter(Site as DataConnection);
      }

      return base.GetServiceImpl(serviceType);
    }

    public override bool Open(bool doPromptCheck)
    {
      // Open connection
      try
      {
        // Call base method first
        if (!base.Open(doPromptCheck))
          return false;

        // Validates expired password
        var connection = Connection as MySqlConnection;
        if (connection != null && connection.IsPasswordExpired)
        {
          using (var newPasswordDialog = new MySqlNewPasswordDialog(connection))
          {
            newPasswordDialog.ShowDialog();
          }
        }
      }
      catch (MySqlException ex)
      {
        // If can prompt user data and is error 1045: Access denied for user
        // Connector/NET with authentication plugin returns the original MySqlException as InnerException
        var exInner = ex.InnerException as MySqlException;
        if (doPromptCheck && ((ex.Number == 1045) || (exInner != null && exInner.Number == 1045)))
        {
          var packInstance = MySqlDataProviderPackage.Instance;
          if (packInstance != null)
          {
            var nodeSelected = packInstance.GetCurrentConnectionName();
            string connString = packInstance.GetConnectionStringBasedOnNode(nodeSelected);
            if (connString != null)
            {
              Connection.ConnectionString = connString;
              Connection.Open();
              return true;
            }
          }
        }

        // If can't prompt user for new authentication data, re-throw exception
        if (string.IsNullOrEmpty(Connection.ConnectionString))
          // If missing server & user, throw a more friendly error message
          throw new Exception( Resources.MissingServerAndUser, ex );
        throw;
      }

      // TODO: check server version compatibility

      // Return true if everything is ok
      if (_sourceInformation != null)
      {
        _sourceInformation.Refresh();
      }

      return true;
    }

    public override DataParameter CreateParameter()
    {
      return new AdoDotNetParameter("MySql.Data.MySqlClient", MySqlClientFactory.Instance.CreateParameter());
    }

    public override DataReader Execute(string command, int commandType, DataParameter[] parameters, int commandTimeout)
    {
      MySqlCommand cmd = DoExecute(command, commandType, parameters, commandTimeout);
      MySqlDataReader r = cmd.ExecuteReader();
      AdoDotNetDataReader reader = new AdoDotNetDataReader(r, cmd, parameters);
      return reader;
    }

    public override int ExecuteWithoutResults(string command, int commandType, DataParameter[] parameters, int commandTimeout)
    {
      MySqlCommand cmd = DoExecute(command, commandType, parameters, commandTimeout);
      return cmd.ExecuteNonQuery();
    }

    private MySqlCommand DoExecute(string command, int commandType, DataParameter[] parameters, int commandTimeout)
    {
      MySqlConnection con = (MySqlConnection)Connection;
      MySqlCommand cmd = new MySqlCommand(command, con)
      {
        Transaction = (MySqlTransaction) Transaction,
        CommandType = (CommandType) commandType,
        CommandTimeout = commandTimeout
      };

      if( parameters == null ) return cmd;
      foreach (DataParameter p in parameters)
      {
        var par = new MySqlParameter(p.Name, p.Value);
        switch (p.Direction)
        {
          case DataParameterDirection.In: par.Direction = ParameterDirection.Input; break;
          case DataParameterDirection.InOut: par.Direction = ParameterDirection.InputOutput; break;
          case DataParameterDirection.Out: par.Direction = ParameterDirection.Output; break;
          case DataParameterDirection.ReturnValue: par.Direction = ParameterDirection.ReturnValue; break;
        }

        par.IsNullable = p.IsNullable;
        par.Precision = p.Precision;
        par.Scale = p.Scale;
        par.Size = p.Size;
        cmd.Parameters.Add(par);
      }

      return cmd;
    }
  }
}
