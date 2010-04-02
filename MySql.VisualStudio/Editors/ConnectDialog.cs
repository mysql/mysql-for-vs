using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
    public partial class ConnectDialog : Form
    {
        private int bigSize = 522;
        private int smallSize = 312;
        private DbProviderFactory factory;
        private DbConnectionStringBuilder connectionStringBuilder;
        private bool populated = false;

        public ConnectDialog()
        {
            InitializeComponent();
            factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            if (factory == null)
                throw new Exception("MySql Data Provider is not correctly registered");
            connectionStringBuilder = factory.CreateConnectionStringBuilder();
            connectionProperties.SelectedObject = connectionStringBuilder;
        }

        public DbConnection Connection
        {
            get
            {
                DbConnection c = factory.CreateConnection();
                c.ConnectionString = connectionStringBuilder.ConnectionString;
                c.Open();
                return c;
            }
            set
            {
                if (value != null)
                {
                    connectionStringBuilder.ConnectionString = value.ConnectionString;
                    Rebind();
                }
            }
        }

        private void advancedButton_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();
            if (this.Size.Height > 400)
            {
                advancedButton.Text = "Advanced >>";
                Height = smallSize;
                simplePanel.Visible = true;
                connectionProperties.Visible = false;
                Rebind();
            }
            else
            {
                advancedButton.Text = "Simple <<";
                Height = bigSize;
                simplePanel.Visible = false;
                connectionProperties.Visible = true;
            }
            this.ResumeLayout();
        }

        private void Rebind()
        {
            serverName.Text = connectionStringBuilder["server"] as string;
            userId.Text = connectionStringBuilder["userid"] as string;
            password.Text = connectionStringBuilder["password"] as string;
            database.Text = connectionStringBuilder["database"] as string;
        }

        private void database_DropDown(object sender, EventArgs e)
        {
            if (populated) return;
            populated = true;
            try
            {
                using (DbConnection c = factory.CreateConnection())
                {
                    c.ConnectionString = connectionStringBuilder.ConnectionString;
                    c.Open();
                    DbCommand cmd = c.CreateCommand();
                    cmd.CommandText = "SHOW DATABASES";
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            database.Items.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void serverName_Leave(object sender, EventArgs e)
        {
            connectionStringBuilder["server"] = serverName.Text.Trim();
        }

        private void userId_Leave(object sender, EventArgs e)
        {
            connectionStringBuilder["userid"] = userId.Text.Trim();
        }

        private void password_Leave(object sender, EventArgs e)
        {
            connectionStringBuilder["password"] = password.Text.Trim();
        }

        private void database_Leave(object sender, EventArgs e)
        {
            connectionStringBuilder["database"] = database.Text.Trim();
        }

    }
}
