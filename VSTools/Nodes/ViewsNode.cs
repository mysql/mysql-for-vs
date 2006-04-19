using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace MySql.VSTools
{
    internal class ViewsNode : ExplorerNode
    {
        public ViewsNode(ExplorerNode parent)
            : base(parent, MyVSTools.GetResourceString("ViewsNodeTitle"))
        {
        }

        public override bool Expandable
        {
            get { return true; }
        }

        public override uint IconIndex
        {
            get { return 1; }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ViewsCtxtMenu; }
        }

        public override void Populate()
        {
            if (populated) return;


            DbConnection c = GetOpenConnection();
            if (c == null) return;

            try
            {
                DataTable table = c.GetSchema("Views",
                    new string[] { null, Parent.Caption, null, null }); 

                foreach (DataRow row in table.Rows)
                    AddChild(new ViewNode(this, row["TABLE_NAME"].ToString(), row));
                populated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
