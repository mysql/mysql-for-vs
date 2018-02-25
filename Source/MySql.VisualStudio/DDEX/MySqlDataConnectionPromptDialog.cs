// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
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

/*
 * This file contains an implemetation of prompt dialog.
 */
using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Prompt dialog for user information retrieving in case of connection failure.
  /// </summary>
  public partial class MySqlDataConnectionPromptDialog : DataConnectionPromptDialog
  {
    /// <summary>
    /// Simple constructor to calls InitializeComponent
    /// </summary>
    public MySqlDataConnectionPromptDialog()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Extract connection options in this handler.
    /// </summary>
    /// <param name="e">Not used</param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      // Check connection support
      if (ConnectionSupport == null)
      {
        Debug.Fail("No connection support!");
        return;
      }

      // Create connection properties to parse connection string
      MySqlConnectionProperties prop = new MySqlConnectionProperties();
      prop.ConnectionStringBuilder.ConnectionString = ConnectionSupport.ConnectionString;

      // Extract server name and port to build connection string
      string server = prop["Server"] as string;
      if (String.IsNullOrEmpty(server))
        server = "localhost"; // Empty server name means local host
      Int64 port = 3306; // By default port is 3306
      object givenPort = prop["Port"];
      if (givenPort != null && typeof(Int64).IsAssignableFrom(givenPort.GetType()))
        port = (Int64)givenPort;

      // Format caption
      Text = String.Format(CultureInfo.CurrentCulture, Text, server, port);

      // Extract options
      login.Text = prop["User Id"] as string;
      password.Text = prop["Password"] as string;
      if (prop["Persist Security Info"] is bool)
        savePassword.Checked = (bool)prop["Persist Security Info"];
      else
        savePassword.Checked = false;
    }

    /// <summary>
    /// Creates new connection string depending on user inpur.
    /// </summary>
    /// <param name="sender">Not used.</param>
    /// <param name="e">Not used.</param>
    private void OkClick(object sender, EventArgs e)
    {
      // Check connection support
      if (ConnectionSupport == null)
      {
        Debug.Fail("No connection support!");
        return;
      }

      // Create connection properties to parse connection string
      MySqlConnectionProperties prop = new MySqlConnectionProperties();
      prop.ConnectionStringBuilder.ConnectionString = ConnectionSupport.ConnectionString;

      // Apply changed options
      prop["User Id"] = login.Text;
      prop["Password"] = password.Text;
      prop["Persist Security Info"] = savePassword.Checked;

      // Change connection string for connection support
      ConnectionSupport.ConnectionString = prop.ToFullString();
    }
  }
}