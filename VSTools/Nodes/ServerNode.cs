using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio;

namespace Vsip.MyVSTools
{
    internal class ServerNode : HierNode
    {
        private MySqlConnection conn;

        public ServerNode(string name, string connectString)
            : base(null, name)
        {
            conn = new MySqlConnection(connectString);
            ItemId = VSConstants.VSITEMID_ROOT;
        }

        public MySqlConnection Connection
        {
            get { return conn; }
        }

        public override uint IconIndex 
        {
           get{ return 0; }
        }

        public override bool Expandable
        {
            get { return true; }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ServerCtxtMenu; }
        }
        /*
        public override bool DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidModifyConnection:
                    Modify();
                    return true;
            }
            return base.DoCommand(commandId);
        }
*/
        public override void Populate()
        {
            if (populated) return;

            try
            {
                DbConnection connection = GetOpenConnection();
                if (connection == null) return;

                DataTable databases = conn.GetSchema("databases");

                foreach (DataRow row in databases.Rows)
                    AddChild(new DatabaseNode(this, row[0].ToString()));
                populated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /*
        public void Modify()
        {
            ConnectionDlg dlg = new ConnectionDlg(this.Text, conn.ConnectionString);
            if (DialogResult.Cancel == dlg.ShowDialog())
                return;
            this.Text = dlg.ConnectionName;
            if (conn != null)
                conn.Close();
            string s = dlg.ConnectionString;
            conn = new MySqlConnection(s); //dlg.ConnectionString);
            (this.TreeView.Parent as ExplorerControl).SaveServers();
        }
*/

    }
}
