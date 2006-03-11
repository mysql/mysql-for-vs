using System;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Vsip.MyVSTools
{
    internal class ProcedureNode : ExplorerNode
    {
        private DataRow procDef;

        public ProcedureNode(ExplorerNode parent, string caption, DataRow row)
            : base(parent, caption)
        {
            procDef = row;
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ProcedureCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 4; }
        }

        public override bool Expandable
        {
            get { return false; }
        }

        public override void Populate()
        {
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
//                case PkgCmdIDList.cmdidDelete:
  //                  Delete();
    //                break;
                case PkgCmdIDList.cmdidOpen:
                    Open();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

/*        private void Delete()
        {
            // first make sure the user is sure
            if (MessageBox.Show(TreeView.Parent,
                String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                procDef["ROUTINE_NAME"]),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            System.Data.Common.DbConnection conn;
            conn = GetOpenConnection();
            System.Data.Common.DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DROP PROCEDURE " + procDef["ROUTINE_SCHEMA"] + "." +
                procDef["ROUTINE_NAME"];
            try
            {
                cmd.ExecuteNonQuery();
                //delete was successful, remove this node
                this.Remove();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, 
                    String.Format(MyVSTools.GetResourceString("UnableToDeleteTitle"),
                    procDef["ROUTINE_NAME"]),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */
        private void Open()
        {
            StoredProcedureEditor editor = new StoredProcedureEditor(
                Caption, procDef["ROUTINE_SCHEMA"].ToString(), 
                procDef["ROUTINE_DEFINITION"].ToString(), 
                GetOpenConnection());
            OpenEditor(editor);
        }
    }
}
