using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace MySql.VSTools
{
    internal class FunctionsNode : ExplorerNode
    {
        public FunctionsNode(ExplorerNode parent)
            : base(parent, MyVSTools.GetResourceString("FunctionsNodeTitle"))
        {
        }

        #region Properties

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

        #endregion

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidAddNewFunction:
                    AddNewFunction();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        private void AddNewFunction()
        {
            string newName = GetDefaultName("Function");
            FunctionNode node = new FunctionNode(this, newName, null);
            IndexChild(node);
            node.OpenEditor();
        }

        public override void Populate()
        {
            if (populated) return;

            DbConnection c = GetOpenConnection();
            if (c == null) return;

            try
            {
                string[] restrictions = new string[4];
                restrictions[1] = Schema;
                restrictions[3] = "FUNCTION";
                DataTable table = c.GetSchema("Procedures", restrictions);

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
