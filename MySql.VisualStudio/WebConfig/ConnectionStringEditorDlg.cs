// Copyright (c) 2009 Sun Microsystems, Inc.
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

using System;
using System.Data.Common;
using System.Windows.Forms;
using System.Reflection;

namespace MySql.Data.VisualStudio.WebConfig
{
    public partial class ConnectionStringEditorDlg : Form
    {
        private DbConnectionStringBuilder builder;

        public ConnectionStringEditorDlg()
        {
            InitializeComponent();

            DbProviderFactory factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            builder = factory.CreateConnectionStringBuilder ();
            connStrProps.SelectedObject = builder;
        }

        public string ConnectionString
        {
            get { return builder.ConnectionString; }
            set { builder.ConnectionString = value; connectionString.Text = value;  }
        }

        private void connStrProps_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            connectionString.Text = builder.ConnectionString;
        }
    }
}
