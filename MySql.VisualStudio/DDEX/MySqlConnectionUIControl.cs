// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of custom connection dialog. 
 */
using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.VisualStudio.Properties;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data connection UI control for entering
	/// connection information.
	/// </summary>
    internal partial class MySqlDataConnectionUI : DataConnectionUIStub
	{
        public MySqlDataConnectionUI()
        {
            InitializeComponent();
        } 

        /// <summary>
        /// Initializes GUI elements with properties values.
        /// </summary>
        public override void LoadProperties()
        {
            if (ConnectionProperties == null)
                throw new Exception(Resources.ConnectionPropertiesNull);

			AdoDotNetConnectionProperties prop = 
                (ConnectionProperties as AdoDotNetConnectionProperties);
			DbConnectionStringBuilder cb = prop.ConnectionStringBuilder;

            loadingInProcess = true;
            try
            {
                serverNameTextBox.Text = (string)cb["Server"];
                userNameTextBox.Text = (string)cb["User Id"];
                passwordTextBox.Text = (string)cb["Password"];
                databaseNameTextBox.Text = (string)cb["Database"];
                savePasswordCheckBox.Checked = (bool)cb["Persist Security Info"];
            }
            finally
            {
                loadingInProcess = false;
            }
        } 

        /// <summary>
        /// Handles TextChanged event from all textboxes.
        /// </summary>
        /// <param name="sender">Reference to sender object.</param>
        /// <param name="e">Additional event data. Not used.</param>
        private void SetProperty(object sender, EventArgs e)
        {
            // Only set properties if we are not currently loading them
            if (!loadingInProcess)
            {
                Control source = sender as Control;
                // Tag is used to determine property name
                if (source != null && source.Tag is string)
                  ConnectionProperties[source.Tag as string] = source.Text;
            }
        }

        /// <summary>
        /// Handles Leave event and trims text.
        /// </summary>
        /// <param name="sender">Reference to sender object.</param>
        /// <param name="e">Additional event data. Not used.</param>
        private void TrimControlText(object sender, EventArgs e)
        {
            Control c = sender as Control;
            c.Text = c.Text.Trim();
        } 

        /// <summary>
        /// Used to prevent OnChange event handling during initialization.
        /// </summary>
        private bool loadingInProcess = false; 

        private void SavePasswordChanged(object sender, EventArgs e)
        {
            // Only set properties if we are not currently loading them
            if (!loadingInProcess)
                ConnectionProperties[savePasswordCheckBox.Tag as string] 
                    = savePasswordCheckBox.Checked;
        }
	}
}
