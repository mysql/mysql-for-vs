// Copyright © 2014, 2019, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Utility.Classes;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// A settings provider customized for MySQL for Visual Studio.
  /// </summary>
  public class MySqlForVisualStudioSettings : CustomSettingsProvider
  {
    /// <summary>
    /// The text from <see cref="AssemblyInfo.AssemblyTitle"/> stripped of spaces.
    /// </summary>
    private string _assemblyTitleWithoutSpaces;

    /// <summary>
    /// Gets the fle path for the settings file.
    /// </summary>
    public static string SettingsFilePath
    {
      get
      {
        return MySqlDataProviderPackage.Instance.AppDataPath + "settings.config";
      }
    }

    /// <summary>
    /// Gets the name of this application.
    /// </summary>
    public override string ApplicationName
    {
      get
      {
        return AssemblyInfo.AssemblyTitle;
      }
      set
      {
      }
    }

    /// <summary>
    /// Gets or sets the name used for the root XML element of the settings file.
    /// </summary>
    public override string RootElementApplicationName
    {
      get
      {
        if (string.IsNullOrEmpty(_assemblyTitleWithoutSpaces))
        {
          _assemblyTitleWithoutSpaces = string.IsNullOrEmpty(AssemblyInfo.AssemblyTitle)
            ? "settings"
            : AssemblyInfo.AssemblyTitle.Replace(" ", string.Empty);
        }

        return string.IsNullOrEmpty(RootElementName)
          ? _assemblyTitleWithoutSpaces
          : RootElementName;
      }
    }

    /// <summary>
    /// Gets or sets the name used for the root XML element of the settings file.
    /// </summary>
    public static string RootElementName { get; set; }

    /// <summary>
    /// Gets the custom path where the settings file is saved.
    /// </summary>
    public override string SettingsPath
    {
      get
      {
        return SettingsFilePath;
      }
    }
  }
}