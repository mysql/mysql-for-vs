// Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.MySql
{
  /// <summary>
  /// Defines MySQL Server users that support authentication mechanisms and roles.
  /// </summary>
  [Serializable]
  public class MySqlServerUser : ICloneable
  {
    #region Constants

    /// <summary>
    /// The commonly used local host.
    /// </summary>
    public const string LOCALHOST = "localhost";

    /// <summary>
    /// The commonly used administrator username.
    /// </summary>
    public const string ROOT_USERNAME = "root";

    /// <summary>
    /// A temporary username that might be used by the MySQL Isntaller for configuration.
    /// </summary>
    public const string TEMPORARY_USERNAME = "mysqltempinternal";

    #endregion Constants

    #region Fields

    /// <summary>
    /// The password.
    /// </summary>
    private string _password;

    /// <summary>
    /// The user name.
    /// </summary>
    private string _username;

    /// <summary>
    /// The tokens used for Windows authentication.
    /// </summary>
    private string _windowsSecurityTokenList;

    #endregion Fields

    /// <summary>
    /// Initiliazes a new instance of the <see cref="MySqlServerUser"/> class.
    /// </summary>
    public MySqlServerUser()
      : this(string.Empty, string.Empty, MySqlAuthenticationPluginType.None)
    {
    }

    /// <summary>
    /// Initiliazes a new instance of the <see cref="MySqlServerUser"/> class.
    /// </summary>
    /// <param name="username">The user name for the account.</param>
    /// <param name="password">The password for the account.</param>
    /// <param name="authenticationPlugin">The <see cref="MySqlAuthenticationPluginType"/> used for the account.</param>
    /// <param name="host">The hostname for the account.</param>
    public MySqlServerUser(string username, string password, MySqlAuthenticationPluginType authenticationPlugin, string host = null)
    {
      _username = username;
      _password = password;
      _windowsSecurityTokenList = string.Empty;
      Host = string.IsNullOrEmpty(host) ? LOCALHOST : host;
      AuthenticationPlugin = authenticationPlugin;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="MySqlAuthenticationPluginType"/> used for the account.
    /// </summary>
    public MySqlAuthenticationPluginType AuthenticationPlugin { get; set; }

    /// <summary>
    /// Gets or sets the host the user account has access to.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <remarks>Password and WindowsSecurityTokenList are mutually exclusive.</remarks>
    public string Password
    {
      get
      {
        return _password.Sanitize();
      }

      set
      {
        _password = value;
      }
    }

    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string Username
    {
      get
      {
        return _username.Sanitize();
      }

      set
      {
        _username = value;
      }
    }

    /// <summary>
    /// Gets or sets the tokens used for Windows authentication.
    /// </summary>
    public string WindowsSecurityTokenList
    {
      get
      {
        return _windowsSecurityTokenList.Sanitize('\\');
      }

      set
      {
        _windowsSecurityTokenList = value;
      }
    }

    #endregion Properties

    /// <summary>
    /// Gets a new instance of the <see cref="MySqlServerUser"/> class for the local root user.
    /// </summary>
    /// <param name="password">The password for the account.</param>
    /// <param name="authenticationPlugin">The <see cref="MySqlAuthenticationPluginType"/> used for the account.</param>
    /// <returns>A new instance of the <see cref="MySqlServerUser"/> class for the local root user.</returns>
    public static MySqlServerUser GetLocalRootUser(string password, MySqlAuthenticationPluginType authenticationPlugin)
    {
      return new MySqlServerUser(ROOT_USERNAME, password, authenticationPlugin);
    }

    /// <summary>
    /// Gets a new instance of the <see cref="MySqlServerUser"/> class for a temporary user account used on configurations.
    /// </summary>
    /// <param name="authenticationPlugin">The <see cref="MySqlAuthenticationPluginType"/> used for the account.</param>
    /// <returns>A new instance of the <see cref="MySqlServerUser"/> class for a temporary user account used on configurations.</returns>
    public static MySqlServerUser GetLocalTemporaryUser(MySqlAuthenticationPluginType authenticationPlugin)
    {
      return new MySqlServerUser(TEMPORARY_USERNAME, new string(TEMPORARY_USERNAME.Reverse().ToArray()), authenticationPlugin);
    }

    /// <summary>
    /// Returns a shallow clone of an instance of this class.
    /// </summary>
    /// <returns>A shallow clone of an instance of this class.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

    public virtual bool IsValid(out string msg)
    {
      msg = string.Empty;
      if (AuthenticationPlugin == MySqlAuthenticationPluginType.None)
      {
        msg = Resources.ServerUserMissingAuthenticationPlugin;
        return false;
      }

      if (AuthenticationPlugin == MySqlAuthenticationPluginType.Windows && string.IsNullOrEmpty(WindowsSecurityTokenList))
      {
        msg = Resources.ServerUserMissingWindowsTokensList;
        return false;
      }

      if (string.IsNullOrWhiteSpace(Username))
      {
        msg = Resources.ServerUserMissingUserName;
        return false;
      }

      return true;
    }
  }
}
