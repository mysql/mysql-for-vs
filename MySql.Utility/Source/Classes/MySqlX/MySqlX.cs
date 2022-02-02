// Copyright (c) 2016, 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Logging;

namespace MySql.Utility.Classes.MySqlX
{
  public static class MySqlX
  {
    /// <summary>
    /// Dictionary containing a key composed of host + classsicPort and its corresponding xPort.
    /// </summary>
    private static Dictionary<string, int> _xPortsDictionary;

    /// <summary>
    /// Fetches from a server in the given connection builder the port for the X Protocol.
    /// </summary>
    /// <param name="connectionStringBuilder">A <see cref="MySqlConnectionStringBuilder"/> instance.</param>
    /// <returns>The port for the X Protocol, or <c>-1</c> if it can't be fetched.</returns>
    public static int FetchXProtocolPort(this MySqlConnectionStringBuilder connectionStringBuilder)
    {
      if (connectionStringBuilder == null)
      {
        return -1;
      }

      string serverKey = $"{connectionStringBuilder.Server}:{connectionStringBuilder.Port}";

      // If we already have the key in the dictionary, fetch it from there
      if (_xPortsDictionary != null && _xPortsDictionary.ContainsKey(serverKey))
      {
        return _xPortsDictionary[serverKey];
      }

      // Fetch the key from the server
      try
      {
        const string SQL = "SELECT @@mysqlx_port";
        object xPortObj = MySqlHelper.ExecuteScalar(connectionStringBuilder.ToString(), SQL);
        if (xPortObj == null)
        {
          return -1;
        }

        var xPort = Convert.ToInt32(xPortObj);
        if (_xPortsDictionary == null)
        {
          _xPortsDictionary = new Dictionary<string, int>();
        }

        _xPortsDictionary.Add(serverKey, xPort);
        return xPort;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, false, string.Format(Resources.FetchXProtocolError, serverKey));
      }

      return -1;
    }

    /// <summary>
    /// Returns the connection string in a <see cref="DbConnection"/> converted to X Protocol format "user:pass@server:port".
    /// </summary>
    /// <param name="connection">A <see cref="DbConnection"/> instance.</param>
    /// <returns>The connection string of a <see cref="DbConnection"/> converted to X Protocol format: "user:pass@server:port"</returns>
    public static string GetXConnectionString(this DbConnection connection)
    {
      var mySqlConnection = connection as MySqlConnection;
      if (mySqlConnection == null)
      {
        return null;
      }

      var connStrBuilder = new MySqlConnectionStringBuilder(mySqlConnection.ConnectionString);
      return connStrBuilder.GetXConnectionString();
    }

    /// <summary>
    /// Returns the connection string in a <see cref="MySqlConnectionStringBuilder"/> converted to X Protocol format "user:pass@server:port".
    /// </summary>
    /// <param name="connectionStringBuilder">A <see cref="MySqlConnectionStringBuilder"/> instance.</param>
    /// <returns>The connection string of a <see cref="MySqlConnectionStringBuilder"/> converted to X Protocol format: "user:pass@server:port"</returns>
    public static string GetXConnectionString(this MySqlConnectionStringBuilder connectionStringBuilder)
    {
      if (connectionStringBuilder == null)
      {
        return null;
      }

      // Create the connection string builder
      string user = connectionStringBuilder.UserID;
      string pass = connectionStringBuilder.Password;
      string server = connectionStringBuilder.Server;

      var xPort = connectionStringBuilder.FetchXProtocolPort();
      if (xPort == -1)
      {
        throw new Exception(string.Format(Resources.FetchXProtocolError, connectionStringBuilder.Server));
      }

      if (connectionStringBuilder.SslMode == MySqlSslMode.None)
      {
        return $"{user}:{pass}@{server}:{xPort}";
      }

      StringBuilder xConecction = new StringBuilder();
      bool sslPameterAdded = false;
      xConecction.Append($"{user}:{pass}@{server}:{xPort}");
      if (!string.IsNullOrEmpty(connectionStringBuilder.SslCa))
      {
        xConecction.AppendFormat("?sslCa=({0})", connectionStringBuilder.SslCa.Replace(" ","%20"));
        sslPameterAdded = true;
      }

      if (!string.IsNullOrEmpty(connectionStringBuilder.SslCert))
      {
        var sslCert = !sslPameterAdded
          ? $"?sslCert=({connectionStringBuilder.SslCert.Replace(" ", "%20")})"
          : $"&sslCert=({connectionStringBuilder.SslCert.Replace(" ", "%20")})";
        xConecction.Append(sslCert);
        sslPameterAdded = true;
      }

      if (!string.IsNullOrEmpty(connectionStringBuilder.SslKey))
      {
        var sslKey = !sslPameterAdded
          ? $"?sslKey=({connectionStringBuilder.SslKey.Replace(" ", "%20")})"
          : $"&sslKey=({connectionStringBuilder.SslKey.Replace(" ", "%20")})";
        xConecction.Append(sslKey);
      }

      return xConecction.ToString();
    }
  }
}
