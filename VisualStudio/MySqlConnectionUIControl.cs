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

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data connection UI control for entering
	/// connection information.
	/// </summary>
    internal partial class MySqlConnectionUIControl : DataConnectionUIStub
	{
        #region Constructor
        /// <summary>
        /// Simple constructor. Only calls InitializeComponent method.
        /// </summary>
        public MySqlConnectionUIControl()
        {
            InitializeComponent();
        } 
        #endregion

        #region Overridings
        /// <summary>
        /// Initializes GUI elements with properties values.
        /// </summary>
        public override void LoadProperties()
        {
            loadingInProcess = true;
            try
            {
                serverNameTextBox.Text = ConnectionProperties[MySqlConnectionProperties.Names.Server] as string;
                serverNameTextBox.Tag = MySqlConnectionProperties.Names.Server;
                userNameTextBox.Text = ConnectionProperties[MySqlConnectionProperties.Names.UserID] as string;
                userNameTextBox.Tag = MySqlConnectionProperties.Names.UserID;
                passwordTextBox.Text = ConnectionProperties[MySqlConnectionProperties.Names.Password] as string;
                passwordTextBox.Tag = MySqlConnectionProperties.Names.Password;
                databaseNameTextBox.Text = ConnectionProperties[MySqlConnectionProperties.Names.Database] as string;
                databaseNameTextBox.Tag = MySqlConnectionProperties.Names.Database;
                if (ConnectionProperties[MySqlConnectionProperties.Names.PersistSecurityInfo] is bool)
                    savePasswordCheckBox.Checked = (bool)ConnectionProperties[MySqlConnectionProperties.Names.PersistSecurityInfo];
                else
                    savePasswordCheckBox.Checked = false;
            }
            finally
            {
                loadingInProcess = false;
            }
        } 
        #endregion

        #region Event handlers
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
        #endregion

        #region Private fields
        /// <summary>
        /// Used to prevent OnChange event handling during initialization.
        /// </summary>
        private bool loadingInProcess = false; 
        #endregion

        private void SavePasswordChanged(object sender, EventArgs e)
        {
            // Only set properties if we are not currently loading them
            if (!loadingInProcess)
                ConnectionProperties[MySqlConnectionProperties.Names.PersistSecurityInfo] 
                    = savePasswordCheckBox.Checked;
        }
	}
}
