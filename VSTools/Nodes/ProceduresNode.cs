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

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidAddNewProcedure:
                    AddNewProcedure();
                    break;
            }
        }

        private void AddNewProcedure()
        {

            int i = 1;
            while (true)
            {
                string nameToCheck = "StoredProdedure" + i;
                ExplorerNode proc = FirstChild;
                while (proc != null)
                {
                    if (proc.Caption == nameToCheck) break;
                    proc = proc.NextSibling;
                }
                if (proc == null) break;
                i++;
            }

            string name = "StoredProcedure" + i;
            string defaultBody = "CREATE PROCEDURE " + name +
                "()" + Environment.NewLine +
                "BEGIN" + Environment.NewLine + "END" +
                Environment.NewLine;
            ProcedureNode node = new ProcedureNode(this, name, defaultBody);
            IndexChild(node);
            node.Open();
       }



        public override void Populate()
        {
            if (populated) return;

            DbConnection c = GetOpenConnection();
            if (c == null) return;

            try
            {
                DataTable table = c.GetSchema("Procedures",
                    new string[] { null, Parent.Caption, null, null });

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
