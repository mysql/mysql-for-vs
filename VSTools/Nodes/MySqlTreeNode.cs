using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data;

namespace Vsip.MyVSTools
{
    internal class MySqlTreeNode : TreeNode
    {
        public MySqlTreeNode(string text, NodeImageIndex imageIndex)
            : base(text, (int)imageIndex, (int)imageIndex)
        {
        }

        protected DbConnection GetOpenConnection()
        {
            TreeNode node = this;
            while (!(node is ServerTreeNode))
                node = node.Parent;
            // we've found our server node, so we grab the connection object
            DbConnection conn = (node as ServerTreeNode).Connection;

            // if it's closed, we try to open it
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error opening connection");
                    conn = null;
                    node.Collapse();
                }
            }
            return conn;
        }

        public virtual uint MenuId
        {
            get { return 0; }
        }

        public virtual bool DoCommand(int commandId)
        {
            return false;
        }

        public virtual bool Populate()
        {
            return false;
        }

    }
}
