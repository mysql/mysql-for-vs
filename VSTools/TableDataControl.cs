using System;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Vsip.MyVSTools
{
    public partial class TableDataControl : UserControl
    {
        private string tableName;
        private DbConnection connection;

        public TableDataControl()
        {
            InitializeComponent();
        }

        public void SetData(DbConnection conn, string table)
        {
            tableName = table;
            connection = conn;

            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT * FROM " + table, (MySqlConnection)conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGrid.DataSource = dt;
        }
    }
}
