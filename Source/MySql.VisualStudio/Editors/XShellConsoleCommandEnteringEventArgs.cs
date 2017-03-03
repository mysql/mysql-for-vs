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

using System;
using System.ComponentModel;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// BaseShellConsoleCommandEnteringEventArgs class used to handle the events fired by the command entering event, in the BaseShell console.
  /// </summary>
  /// <seealso cref="System.EventArgs" />
  public class BaseShellConsoleCommandEnteringEventArgs : EventArgs
  {
    /// <summary>
    /// The command string.
    /// </summary>
    private string _command;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseShellConsoleCommandEnteringEventArgs"/> class.
    /// </summary>
    /// <param name="comm">The comm.</param>
    public BaseShellConsoleCommandEnteringEventArgs(string comm)
    {
      _command = comm;
    }

    /// <summary>
    /// Gets or sets the command in the input area.
    /// </summary>
    /// <value>
    /// The command.
    /// </value>
    public string Command
    {
      get { return _command; }
      set { _command = value; }
    }
  }
}
