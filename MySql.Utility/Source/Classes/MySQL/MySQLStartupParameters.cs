// Copyright (c) 2013, 2016, Oracle and/or its affiliates. All rights reserved.
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

using System.Globalization;
using System.ServiceProcess;
using MySql.Utility.Classes.MySqlWorkbench;

namespace MySql.Utility.Classes.MySql
{
  /// <summary>
  /// Contains connection parameters extracted from a Windows service of a MySQL Server instance.
  /// </summary>
  public class MySqlStartupParameters
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlStartupParameters"/> class.
    /// </summary>
    public MySqlStartupParameters()
    {
      DefaultsFile = null;
      HostIPv4 = null;
      HostName = null;
      IsRealMySqlService = false;
      NamedPipesEnabled = false;
      PipeName = null;
      Port = 0;
    }

    #region Properties

    /// <summary>
    /// Gets the default INI file used by a MySQL Server instance containing initialization parameters and values.
    /// </summary>
    public string DefaultsFile { get; private set; }

    /// <summary>
    /// Gets the connection IP of the MySQL Server instance.
    /// </summary>
    public string HostIPv4 { get; private set; }

    /// <summary>
    /// Gets the connection host name of the MySQL Server instance.
    /// </summary>
    public string HostName { get; private set; }

    /// <summary>
    /// Gets a value indicating if this service is bound to a MySQL server service.
    /// </summary>
    public bool IsRealMySqlService { get; private set; }

    /// <summary>
    /// Gets a value indicating if names pipes are enabled for the MySQL Server connection.
    /// </summary>
    public bool NamedPipesEnabled { get; private set; }

    /// <summary>
    /// Gets the name of the pipe used by the connection.
    /// </summary>
    public string PipeName { get; private set; }

    /// <summary>
    /// Gets the connection port of the MySQL Server instance.
    /// </summary>
    public uint Port { get; private set; }

    #endregion Properties

    /// <summary>
    /// Gets the connection properties for a existing MySQL Servers instance installed as a Windows service in the local computer.
    /// </summary>
    /// <param name="winService">Windows service for a MySQL Server instance.</param>
    /// <returns>A <see cref="MySqlStartupParameters"/> object with the connection properties.</returns>
    public static MySqlStartupParameters GetStartupParameters(ServiceController winService)
    {
      var parameters = new MySqlStartupParameters
      {
        HostName = winService.MachineName == "." ? MySqlWorkbenchConnection.DEFAULT_HOSTNAME : winService.MachineName
      };

      // Get our host information
      parameters.HostIPv4 = Utilities.GetIPv4ForHostName(parameters.HostName);
      string imagepath;
      parameters.IsRealMySqlService = Service.IsRealMySqlService(winService.ServiceName, out imagepath);
      if (!parameters.IsRealMySqlService)
      {
        return parameters;
      }

      var args = Utilities.SplitArgs(imagepath);
      parameters.PipeName = "mysql";

      // Parse our command line args
      uint port = 0;
      var p = new Mono.Options.OptionSet()
        .Add("defaults-file=", "", v => parameters.DefaultsFile = v)
        .Add("port=|P=", "", v => uint.TryParse(v, out port))
        .Add("enable-named-pipe", v => parameters.NamedPipesEnabled = true)
        .Add("socket=", "", v => parameters.PipeName = v);
      parameters.Port = port;

      p.Parse(args);
      if (parameters.DefaultsFile == null)
      {
        return parameters;
      }

      // We have a valid defaults file
      var f = new IniFile(parameters.DefaultsFile);
      uint.TryParse(f.ReadValue("mysqld", "port", parameters.Port.ToString(CultureInfo.InvariantCulture)), out port);
      parameters.PipeName = f.ReadValue("mysqld", "socket", parameters.PipeName);
      parameters.Port = port;

      // Now see if named pipes are enabled
      parameters.NamedPipesEnabled = parameters.NamedPipesEnabled || f.HasKey("mysqld", "enable-named-pipe");
      return parameters;
    }
  }
}