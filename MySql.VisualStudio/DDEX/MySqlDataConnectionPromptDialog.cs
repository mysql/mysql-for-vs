// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
                savePassword.Checked = (bool) prop["Persist Security Info"];
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