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
  /// BaseShellConsoleCommandEventArgs class used to handle the events fired by the command event, in the BaseShell console.
  /// </summary>
  /// <seealso cref="System.EventArgs" />
  public class BaseShellConsoleCommandEventArgs : EventArgs
  {
    /// <summary>
    /// The _cancel flag.
    /// </summary>
    private bool _cancel;

    /// <summary>
    /// The command to be executed.
    /// </summary>
    private string _command;

    /// <summary>
    /// The message to be stored.
    /// </summary>
    private string _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseShellConsoleCommandEventArgs"/> class.
    /// </summary>
    /// <param name="cmd">The command.</param>
    public BaseShellConsoleCommandEventArgs(string cmd)
    {
      _command = cmd;
      _message = "";
    }

    /// <summary>
    /// Flag to determine if Cancel event should be made.
    /// </summary>
    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    /// <summary>
    /// The command entered by the user.
    /// </summary>
    /// <value>
    /// The command.
    /// </value>
    public string Command
    {
      get { return _command; }
      set { _command = value; }
    }

    /// <summary>
    /// Message to be displayed to user in response to a command.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }
  }
}