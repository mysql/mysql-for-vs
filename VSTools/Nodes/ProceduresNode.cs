using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace MySql.VSTools
{
    internal class ProceduresNode : ExplorerNode
    {
        public ProceduresNode(ExplorerNode parent)
            : base(parent, MyVSTools.GetResourceString("ProceduresNodeTitle"))
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
            get { return PkgCmdIDList.ProceduresCtxtMenu; }
        }

        #endregion

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidAddNewProcedure:
                    AddNewProcedure();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        private void AddNewProcedure()
        {
            string newName = GetDefaultName("Procedure");
            ProcedureNode node = new ProcedureNode(this, newName, null);
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
                restrictions[1] = GetDatabaseNode().Caption;
                restrictions[3] = "PROCEDURE";
                DataTable table = c.GetSchema("Procedures", restrictions);

                foreach (DataRow row in table.Rows)
                    AddChild(new ProcedureNode(this, row["ROUTINE_NAME"].ToString(), row));
                populated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
