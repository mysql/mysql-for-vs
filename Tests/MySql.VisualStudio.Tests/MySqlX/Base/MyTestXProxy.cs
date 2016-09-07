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

using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.VisualStudio.Editors;
using MySql.Utility.Classes.MySqlX;
using MySql.Utility.Enums;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  public class MyTestXProxy : MySqlXProxy
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MyTestXProxy"/> class.
    /// </summary>
    /// <param name="connectionString">Connection string that will be used when a script is executed. Format: "user:pass@server:port".</param>
    /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    /// <param name="scriptType">The language type used.</param>
    public MyTestXProxy(string connectionString, bool keepXSession, ScriptLanguageType scriptType) : base(connectionString, keepXSession, scriptType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTestXProxy"/> class.
    /// </summary>
    /// <param name="connection">Connection object that will be used."</param>
    /// /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    /// <param name="scriptType">The language type used.</param>
    public MyTestXProxy(DbConnection connection, bool keepXSession, ScriptLanguageType scriptType) : base(connection, keepXSession, scriptType)
    {
    }

    /// <summary>
    /// Executes a JavaScript or Python query using the X Shell
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <param name="scriptType">The type of language used.</param>
    /// <returns>Returns an empty list of dictionary objects if the result returned from the server doesnt belong to the BaseResult hierarchy</returns>
    public new List<Dictionary<string, object>> ExecuteScript(string script, ScriptLanguageType scriptType)
    {
      if (scriptType == ScriptLanguageType.JavaScript)
      {
        script = script.ToJavaScript();
      }

      return base.ExecuteScript(script, scriptType);
    }
  }
}
