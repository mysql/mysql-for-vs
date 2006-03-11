using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace Vsip.MyVSTools
{
    internal class FunctionsNode : ExplorerNode
    {
        public FunctionsNode(ExplorerNode parent)
            : base(parent, MyVSTools.GetResourceString("FunctionsNodeTitle"))
        {
        }

        public override uint IconIndex
        {
            get { return 1; }
        }

        public override bool Expandable
        {
            get { return true; }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.FunctionsCtxtMenu; }
        }

        public override void Populate()
        {
            if (populated) return;

            DbConnection c = GetOpenConnection();
            if (c == null) return;

            try
            {
                DataTable table = c.GetSchema("Functions",
                    new string[] { null, Parent.Caption, null, null });

                foreach (DataRow row in table.Rows)
                    AddChild(new FunctionNode(this, row["ROUTINE_NAME"].ToString(), row));
                populated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
