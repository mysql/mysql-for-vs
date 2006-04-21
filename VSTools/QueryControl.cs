using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace MySql.VSTools
{
    public partial class QueryControl : UserControl
    {
        private MySqlConnection connection;
        private bool showingSql;
        private bool showingGrid;
        private bool showingText;

        public QueryControl()
        {
            InitializeComponent();
            showingSql = true;
            showingGrid = true;
            showingText = false;
        }

        public DbConnection Connection
        {
            set { this.connection = (MySqlConnection)value; }
        }

        public bool ShowingSql
        {
            get { return showingSql; }
            set { showingSql = value; ToggleWindows();  }
        }

        public bool ShowingGrid
        {
            get { return showingGrid; }
            set { showingGrid = value; ToggleWindows();  }
        }

        public bool ShowingText
        {
            get { return showingText; }
            set { showingText = value; ToggleWindows();  }
        }

        private void ToggleWindows()
        {
            inputBox.Visible = ShowingSql;
            resultsGrid.Visible = ShowingGrid;
            resultsText.Visible = ShowingText;
        }

        public void Execute()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(inputBox.Text, connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            this.resultsGrid.DataSource = dt;
       }
    }
}
