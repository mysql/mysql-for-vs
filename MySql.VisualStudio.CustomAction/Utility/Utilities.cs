// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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

using Microsoft.Win32;
using System;
using System.IO;
using System.Text;

namespace MySql.VisualStudio.CustomAction.Utility
{
  /// <summary>
  /// Provides utility methods used on the Custom Actions project.
  /// </summary>
  public static class Utilities
  {
    /// <summary>
    /// Gets the installation path for Visual Studio 2015.
    /// </summary>
    /// <returns>The installation path for Visual Studio 2015 if installed; otherwise, <c>null</c>.</returns>
    public static string GetVisualStudio2015InstallationPath()
    {
      const string visualStudio2015RegistryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0";

      try
      {
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(visualStudio2015RegistryKey))
        {
          if (key != null)
          {
            var value = key.GetValue("ShellFolder").ToString();
            if (!string.IsNullOrEmpty(value))
            {
              return value;
            }
          }
        }
      }
      catch (Exception)
      {
        return null;
      }

      return null;
    }

    /// <summary>
    /// Writes or appends text to the specified file.
    /// </summary>
    /// <param name="fileName">The name and path of the file.</param>
    /// <param name="append">Indicates if the text must be appended to an existing file.</param>
    /// <param name="text">The text to write or append.</param>
    /// <param name="encoding">The encoding to use for the append operation.</param>
    /// <returns><c>true</c> if writing to the file was successful; otherwise, <c>false</c>.</returns>
    public static bool WriteAllText(string fileName, bool append, string text, Encoding encoding = null)
    {
      if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text) || !File.Exists(fileName))
      {
        return false;
      }

      try
      {
        if (encoding == null)
        {
          if (!append)
          {
            File.WriteAllText(fileName, text);
          }
          else
          {
            File.AppendAllText(fileName, text);
          }
        }
        else
        {
          if (!append)
          {
            File.WriteAllText(fileName, text, encoding);
          }
          else
          {
            File.AppendAllText(fileName, text, encoding);
          }
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Gets the file path for the MySQL for Visual Studio PKGDEF file in the specified location.
    /// </summary>
    /// <param name="visualStudioInstallationPath">The Visual Studio installation path.</param>
    /// <param name="mySqlForVisualStudioVersion">The installed version of MySQL for Visual Studio.</param>
    /// <returns>The location of the PKGDEF file.</returns>
    public static string GetPkgdefFilePath(string visualStudioInstallationPath, Version mySqlForVisualStudioVersion)
    {
      if (string.IsNullOrEmpty(visualStudioInstallationPath))
      {
        return null;
      }

      const string commonPath = @"Common7\IDE\Extensions\Oracle\MySQL for Visual Studio\{0}\MySql.VisualStudio.pkgdef";
#if DEBUG
      return Path.Combine(visualStudioInstallationPath,
                          string.Format(commonPath, $"{mySqlForVisualStudioVersion.Major}.{mySqlForVisualStudioVersion.Minor}.{mySqlForVisualStudioVersion.Build}"));
#else
      return Path.Combine(visualStudioInstallationPath,
                          string.Format(commonPath, $"{mySqlForVisualStudioVersion.Major}.{mySqlForVisualStudioVersion.Minor}.{mySqlForVisualStudioVersion.Build}"));
#endif
    }

    /// <summary>
    /// Removes unnecesary text leaving only the version part as a string.
    /// </summary>
    /// <param name="entry">The entry to sanitize.</param>
    /// <returns>If the sanitize operation was successful it returns a version string; otherwise, <c>null</c>.</returns>
    public static string SanitizePkgdegFileVersionEntry(string entry)
    {
      const string oldVersionEntry = "\"OldVersion\" =";
      const string newVersionEntry = "\"NewVersion\" =";
      if (entry.StartsWith(oldVersionEntry))
      {
        entry = entry.Replace(oldVersionEntry, string.Empty);
      }
      else if (entry.StartsWith(newVersionEntry))
      {
        entry = entry.Replace(newVersionEntry, string.Empty);
      }
      else
      {
        return null;
      }

      // Remove the ending " char.
      return entry.Replace("\"", string.Empty).Replace(" ", "");
    }
  }
}
