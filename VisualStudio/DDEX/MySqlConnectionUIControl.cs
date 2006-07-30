using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data connection UI control for entering
	/// connection information.
	/// </summary>
	internal partial class MySqlConnectionUIControl : DataConnectionUIControl
	{
		public MySqlConnectionUIControl()
		{
			InitializeComponent();
		}

		public override void LoadProperties()
		{
			this._loading = true;
			try
			{
				serverName.Text = ConnectionProperties["Server"] as string;
				userName.Text = ConnectionProperties["User ID"] as string;
				password.Text = ConnectionProperties["Password"] as string;
				savePassword.Checked = (bool)ConnectionProperties["Persist Security Info"];
				databaseName.Text = ConnectionProperties["Database"] as string;
			}
			finally
			{
				this._loading = false;
			}
		}

		private void SetProperty(object sender, EventArgs e)
		{
			// Only set properties if we are not currently loading them
			if (!this._loading)
			{
				if (sender == serverName)
				{
					ConnectionProperties["Data Source"] = serverName.Text.Trim();
				}
				if (sender == userName)
				{
					ConnectionProperties["User ID"] = userName.Text;
				}
				if (sender == password)
				{
					ConnectionProperties["Password"] = password.Text;
				}
				if (sender == savePassword)
				{
					ConnectionProperties["Persist Security Info"] = savePassword.Checked;
				}
				if (sender == databaseName)
				{
					ConnectionProperties["Database"] = databaseName.Text;
				}
			}

			// Update the UI to the correct state
			if (serverName.Text.Trim().Length == 0)
			{
				loginDetailsGroupBox.Enabled = false;
				databaseNameLabel.Enabled = false;
				databaseName.Enabled = false;
			}
			else
			{
				loginDetailsGroupBox.Enabled = true;
				loginTableLayoutPanel.Enabled = true;
				if (userName.Text.Trim().Length == 0)
				{
					databaseNameLabel.Enabled = false;
					databaseName.Enabled = false;
				}
				else
				{
					databaseNameLabel.Enabled = true;
					databaseName.Enabled = true;
				}
			}
		}

		private void TrimControlText(object sender, EventArgs e)
		{
			Control c = sender as Control;
			c.Text = c.Text.Trim();
		}

		/// <summary>
		/// It is necessary that we keep track of whether properties are
		/// currently being loaded or not.  This is because the events
		/// fired by each control that cause the SetProperty method to
		/// be called are typically called when the text changes or the
		/// value of the control is altered.  This happens when loading
		/// the properties and when a user sets them.  In the case of
		/// loading, we do not want to update the underlying connection
		/// properties instance with the value so we set this to true
		/// during load time so that SetProperty only causes UI state
		/// changes and does not write to the connection properties.
		/// </summary>
		private bool _loading = false;
	}
}
