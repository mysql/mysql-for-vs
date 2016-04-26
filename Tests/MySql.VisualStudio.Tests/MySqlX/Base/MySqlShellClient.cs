// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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

using System;
using MySql.Data.VisualStudio.Editors;
using MySqlX.Shell;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  /// <summary>
  /// Custom direct XShellWrapper Implementation for the current tests
  /// </summary>
  public class MySqlShellClient : ShellClient
  {
    /// <summary>
    /// Executes a base query converting it first to JavaScript format.
    /// </summary>
    /// <param name="baseQuery"></param>
    public object ExecuteToJavaScript(string baseQuery)
    {
      return Execute(baseQuery.ToJavaScript());
    }

    /// <summary>
    /// Overrides the Input Method to write and return data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    /// <param name="ret">Value to return</param>
    /// <returns></returns>
    public override bool Input(string text, ref string ret)
    {
      Console.WriteLine(text);
      ret = Console.ReadLine();
      return ret != null;
    }

    /// <summary>
    /// Overrides the Password Method to return data in a custom way
    /// </summary>
    /// <param name="text">Data information</param>
    /// <param name="ret">Data to Return</param>
    /// <returns></returns>
    public override bool Password(string text, ref string ret)
    {
      return Input(text, ref ret);
    }

    /// <summary>
    /// Overrides the Print Method to write data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    public override void Print(string text)
    {
      Console.WriteLine(text);
    }

    /// <summary>
    /// Overrides the PrintError Method to write data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    public override void PrintError(string text)
    {
      Console.WriteLine(@"***ERROR***{0}", text);
    }
  }
}
