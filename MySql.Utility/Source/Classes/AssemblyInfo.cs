// Copyright (c) 2012-2014, Oracle and/or its affiliates. All rights reserved.
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

using System.Reflection;
using System.Runtime.InteropServices;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Gets information about the application.
  /// </summary>
  /// <remarks>
  /// This class is just for getting information about the application.
  /// Each assembly has a GUID which we will use with the Mutex.
  /// </remarks>
  public static class AssemblyInfo
  {
    /// <summary>
    /// Gets the GUID of the assembly of the running application.
    /// </summary>
    public static string AssemblyGuid
    {
      get
      {
        object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
        GuidAttribute guidAttribute = attributes.Length > 0 ? attributes[0] as GuidAttribute : null;
        return guidAttribute != null ? guidAttribute.Value : string.Empty;
      }
    }

    /// <summary>
    /// Gets the name of the assembly of the running application.
    /// </summary>
    public static string AssemblyTitle
    {
      get
      {
        object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        AssemblyTitleAttribute titleAttribute = null;

        if (attributes.Length > 0)
        {
          titleAttribute = (AssemblyTitleAttribute)attributes[0];
        }

        return titleAttribute != null && titleAttribute.Title != string.Empty ? titleAttribute.Title : Assembly.GetCallingAssembly().FullName;
      }
    }
  }
}