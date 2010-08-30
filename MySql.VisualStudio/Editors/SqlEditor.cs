﻿// Copyright © 2008, 2010, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.Data.Common;
using MySql.Data.MySqlClient;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Globalization;
using System.IO;

namespace MySql.Data.VisualStudio.Editors
{
    public partial class SqlEditor : BaseEditorControl
    {
        private DbConnection connection;
        private DbProviderFactory factory;

        public SqlEditor()
        {
            InitializeComponent();
            factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            if (factory == null)
                throw new Exception("MySql Data Provider is not correctly registered");
            tabControl1.TabPages.Remove(resultsPage);
        }

        public SqlEditor(ServiceProvider sp) : this()
        {
            serviceProvider = sp;
            codeEditor.Init(sp);
        }

        #region Overrides

        protected override string GetFileFormatList()
        {
            return "MySQL Script Files (*.mysql)\n*.mysql\n\n";
        }

        protected override void SaveFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                writer.Write(codeEditor.Text);
            }
        }

        protected override void LoadFile(string fileName)
        {
            if (!File.Exists(fileName)) return;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string sql = reader.ReadToEnd();
                codeEditor.Text = sql;
            }
        }

        protected override bool IsDirty
        {
            get { return codeEditor.IsDirty; }
            set { codeEditor.IsDirty = value; }
        }

        #endregion

        private void connectButton_Click(object sender, EventArgs e)
        {
            resultsPage.Hide();
            ConnectDialog d = new ConnectDialog();
            d.Connection = connection;
            DialogResult r = d.ShowDialog();
            if (r == DialogResult.Cancel) return;
            connection = d.Connection;
            UpdateButtons();
        }

        private void runSqlButton_Click(object sender, EventArgs e)
        {
            string sql = codeEditor.Text.Trim();
            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
                ExecuteSelect(sql);
            else
                ExecuteScript(sql);
        }

        private void ExecuteSelect(string sql)
        {
            tabControl1.TabPages.Clear();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, (MySqlConnection)connection);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                tabControl1.TabPages.Add(resultsPage);
                resultsGrid.DataSource = dt;
            }
            catch (Exception ex)
            {
                messages.Text = ex.Message;
            }
            finally
            {
                tabControl1.TabPages.Add(messagesPage);
            }
        }

        private void ExecuteScript(string sql)
        {
            tabControl1.TabPages.Clear();
            MySqlScript script = new MySqlScript((MySqlConnection)connection, sql);
            try
            {
                int rows = script.Execute();
                messages.Text = String.Format("{0} row(s) affected", rows);
            }
            catch (Exception ex)
            {
                messages.Text = ex.Message;
            }
            finally
            {
                tabControl1.TabPages.Add(messagesPage);
            }
        }

        private void validateSqlButton_Click(object sender, EventArgs e)
        {

        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool connected = connection.State == ConnectionState.Open;
            runSqlButton.Enabled = connected;
//            validateSqlButton.Enabled = connected;
            disconnectButton.Enabled = connected;
            connectButton.Enabled = !connected;
            serverLabel.Text = String.Format("Server: {0}",
                connected ? connection.ServerVersion : "<none>");
            DbConnectionStringBuilder builder = factory.CreateConnectionStringBuilder();
            builder.ConnectionString = connection.ConnectionString;
            userLabel.Text = String.Format("User: {0}",
                connected ? builder["userid"] as string : "<none>");
            dbLabel.Text = String.Format("Database: {0}",
                connected ? connection.Database : "<none>");
        }

    }
}
