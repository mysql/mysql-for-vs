using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySql.VSTools
{
    public partial class ConnectionDlg : System.Windows.Forms.Form
    {
        private MySqlConnectionString settings;

        public ConnectionDlg()
        {
            InitializeComponent();
        }

        public ConnectionDlg(string name, string connectString) : this()
        {
            connectionName.Text = name;
            settings = new MySqlConnectionString(connectString);

            // first page
            host.Text = settings.Server;
            connectionType.SelectedIndex = (int)settings.Protocol;
            timeout.Value = settings.ConnectionTimeout;
            port.Value = settings.Port;
            pipeName.Text = settings.PipeName;
            compress.Checked = settings.UseCompression;

            //second page
            user.Text = settings.UserId;
            password.Text = settings.Password;
            savePassword.Checked = settings.PersistSecurityInfo;
            database.SelectedIndex = FindDatabase(settings.Database);
            useSSL.Checked = settings.UseSSL;

            //third page
            enablePooling.Checked = settings.Pooling;
            minPoolSize.Value = settings.MinPoolSize;
            maxPoolSize.Value = settings.MaxPoolSize;
            connectLifetime.Value = settings.ConnectionLifetime;
            driverType.SelectedIndex = (int)settings.DriverType;

        }

        public string ConnectionName
        {
            get { return connectionName.Text; }
        }

        public string ConnectionString
        {
            get { return CreateConnectionString(); }
        }

        private int FindDatabase(string database)
        {
            return -1;
        }

        private void connectionType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (connectionType.SelectedIndex)
            {
                case 0: //tcp/ip
                    socketNameLabel.Text = "Port:";
                    break;
                case 1: //shred memory
                    socketNameLabel.Text = "Memory name:";
                    break;
                case 2: // named pipe
                    socketNameLabel.Text = "Pipe name:";
                    break;
                case 3: // unix socket
                    socketNameLabel.Text = "Socket name:";
                    break;
            }
            port.Visible = connectionType.SelectedIndex == 0;
            pipeName.Visible = !port.Visible;
        }

        private string CreateConnectionString()
        {
            System.Text.StringBuilder cs = new System.Text.StringBuilder();
            if (host.Text.Length > 0)
                cs = cs.AppendFormat("host={0};", host.Text);
            if (user.Text.Length > 0)
                cs = cs.AppendFormat("uid={0};", user.Text);
            if (password.Text.Length > 0)
                cs = cs.AppendFormat("pwd={0};", password.Text);
            cs = cs.AppendFormat("persist security info={0};compress={1};pooling={2};",
                savePassword.Checked, compress.Checked, enablePooling.Checked);
            if (database.SelectedIndex >= 0)
                cs = cs.AppendFormat("database={0};", database.SelectedValue);
            cs = cs.AppendFormat("connect timeout={0};", timeout.Value);
            if (enablePooling.Checked)
            {
                cs = cs.AppendFormat("min pool size={0}; max pool size={1};",
                    minPoolSize.Value, maxPoolSize.Value);
                cs = cs.AppendFormat("connection lifetime={0};",
                    connectLifetime.Value);
            }
            return cs.ToString();
        }

        private void testbtn_Click(object sender, EventArgs e)
        {            
            MySqlConnection c = new MySqlConnection(CreateConnectionString());
            try
            {
                c.Open();
                c.Close();
                MessageBox.Show("Connection was successful.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection failed.");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (connectionName.Text.Length == 0 && DialogResult == DialogResult.OK)
            {
                MessageBox.Show("Connection Name is a requird field");
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void database_DropDown(object sender, EventArgs e)
        {

        }

    }
}