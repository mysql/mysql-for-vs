// Copyright (c) 2021, Oracle and/or its affiliates.
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

using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MySql.Data.VisualStudio.Common
{
  internal static class ConnectionExtensions
  {
    /// <summary>
    /// DbConnection extension method used to ensure that the WAIT_TIMEOUT server variable is updated if required
    /// after opening a connection.
    /// </summary>
    /// <param name="connection">The connection object used to open a connection to the server.</param>
    public static void OpenWithDefaultTimeout(this DbConnection connection)
    {
      connection.Open();
      Utilities.UpdateWaitTimeout(connection as MySqlConnection);
    }

    /// <summary>
    /// MySqlConnection extension method used to ensure that the WAIT_TIMEOUT server variable is updated if required
    /// after opening a connection.
    /// </summary>
    /// <param name="connection">The connection object used to open a connection to the server.</param>
    public static void OpenWithDefaultTimeout(this MySqlConnection connection)
    {
      connection.Open();
      Utilities.UpdateWaitTimeout(connection);
    }

    /// <summary>
    /// DataConnection extension method used to ensure that the WAIT_TIMEOUT server variable is updated if required
    /// after opening a connection.
    /// </summary>
    /// <param name="connection">The connection object used to open a connection to the server.</param>
    public static void OpenWithDefaultTimeout(this DataConnection connection)
    {
      var mySqlConnection = connection.GetLockedProviderObject() as MySqlConnection;
      if (mySqlConnection == null)
      {
        return;
      }

      mySqlConnection.Open();
      Utilities.UpdateWaitTimeout(mySqlConnection);
      connection.UnlockProviderObject();
    }
  }
}
