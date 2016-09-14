// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Utility.Enums;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  /// <summary>
  /// Defines a factory for <see cref="TableTestsProperties"/> based classes.
  /// </summary>
  public static class TableTestsPropertiesFactory
  {
    /// <summary>
    /// Gets a <see cref="TableTestsProperties"/> class instance corresponding to the given language.
    /// </summary>
    /// <param name="language">A <see cref="ScriptLanguageType"/> value.</param>
    /// <returns>A <see cref="TableTestsProperties"/> class instance corresponding to the given language.</returns>
    public static TableTestsProperties GetTableTestsProperties(ScriptLanguageType language)
    {
      switch (language)
      {
        case ScriptLanguageType.JavaScript:
          return new JsTableTestsProperties(language);

        case ScriptLanguageType.Python:
          return new PyTableTestsProperties(language);

        default:
          return null;
      }
    }
  }
}
