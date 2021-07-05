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
 * This file contains implementation of custom connection support for MySQL.
 */

using System;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio
{
  public class MySqlConnectionSupport : AdoDotNetConnectionSupport
  {
    private MySqlDataSourceInformation sourceInformation = null;

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
      else if (serviceType == typeof(DataObjectSupport))
      {
        return new MySqlDataObjectSupport();
      }
      else if (serviceType == typeof(DataSourceInformation))
      {
        return new MySqlDataSourceInformation(Site as DataConnection);
      }
      else if (serviceType == typeof(DataObjectIdentifierConverter))
      {
        return new MySqlDataObjectIdentifierConverter(Site as DataConnection);
      }
      else
      {
        return base.GetServiceImpl(serviceType);
      }
    }

    public override bool Open(bool doPromptCheck)
    {
      // Open connection.
      try
      {
        // Call base method first.
        if (!base.Open(doPromptCheck))
        {
          return false;
        }

        // Validates expired password.
        var connection = base.Connection as MySqlConnection ?? new MySqlConnection(base.Connection.ConnectionString);
        if (connection.State != ConnectionState.Open)
        {
          connection.Open();
        }

        Utilities.UpdateWaitTimeout(connection);
        if (connection.IsPasswordExpired)
        {
          MySqlNewPasswordDialog newPasswordDialog = new MySqlNewPasswordDialog(connection);
          newPasswordDialog.ShowDialog();
        }
      }
      catch (MySqlException ex)
      {
        // If can prompt user data and is error 1045: Access denied for user
        // Connector/NET with authentication plugin returns the original MySqlException as InnerException
        MySqlException exInner = ex.InnerException as MySqlException;
        if (doPromptCheck && ((ex.Number == 1045) || (exInner != null && exInner.Number == 1045)))
        {
          var packInstance = MySqlDataProviderPackage.Instance;
          if (packInstance != null)
          {
            var nodeSelected = packInstance.GetCurrentConnectionName();
            string connString = packInstance.GetConnectionStringBasedOnNode(nodeSelected);
            if (connString != null)
            {
              base.Connection.ConnectionString = connString;
              var connection = base.Connection as DbConnection;
              if (connection != null)
              {
                connection.OpenWithDefaultTimeout();
              }

              return true;
            }
          }
        }

        // If can't prompt user for new authentication data, re-throw exception.
        if (string.IsNullOrEmpty(base.Connection.ConnectionString))
        {
          // If missing server & user, throw a more friendly error message.
          throw new Exception(Properties.Resources.MissingServerAndUser, ex );
        }

        throw;
      }

      // TODO: check server version compatibility.

      // Return true if everything is ok.
      if (sourceInformation != null)
      {
        sourceInformation.Refresh();
      }

      return true;
    }

    public override DataParameter CreateParameter()
    {
      return new AdoDotNetParameter("MySql.Data.MySqlClient", MySqlClientFactory.Instance.CreateParameter());
    }

    public override DataReader Execute(string command, int commandType, Microsoft.VisualStudio.Data.DataParameter[] parameters, int commandTimeout)
    {
      MySqlCommand cmd = DoExecute(command, commandType, parameters, commandTimeout);
      MySqlDataReader r = cmd.ExecuteReader();
      AdoDotNetDataReader reader = new AdoDotNetDataReader(r, cmd, parameters);
      return reader;
    }

    public override int ExecuteWithoutResults(string command, int commandType, Microsoft.VisualStudio.Data.DataParameter[] parameters, int commandTimeout)
    {
      MySqlCommand cmd = DoExecute(command, commandType, parameters, commandTimeout);
      return cmd.ExecuteNonQuery();
    }

    private MySqlCommand DoExecute(string command, int commandType, Microsoft.VisualStudio.Data.DataParameter[] parameters, int commandTimeout)
    {
      MySqlConnection con = (MySqlConnection)this.Connection;
      MySqlCommand cmd = new MySqlCommand(command, con);
      cmd.Transaction = (MySqlTransaction)this.Transaction;
      cmd.CommandType = (CommandType)commandType;
      cmd.CommandTimeout = commandTimeout;
      if (parameters == null)
      {
        return cmd;
      }

      for (int i = 0; i < parameters.Length; i++)
      {
        DataParameter p = parameters[i];
        MySqlParameter par = new MySqlParameter(p.Name, p.Value);
        switch (p.Direction)
        {
          case Microsoft.VisualStudio.Data.DataParameterDirection.In: par.Direction = ParameterDirection.Input; break;
          case Microsoft.VisualStudio.Data.DataParameterDirection.InOut: par.Direction = ParameterDirection.InputOutput; break;
          case Microsoft.VisualStudio.Data.DataParameterDirection.Out: par.Direction = ParameterDirection.Output; break;
          case Microsoft.VisualStudio.Data.DataParameterDirection.ReturnValue: par.Direction = ParameterDirection.ReturnValue; break;
          default: break; /* nothing */
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
