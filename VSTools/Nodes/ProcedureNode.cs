using System;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.VSTools
{
    internal class ProcedureNode : ExplorerNode
    {
        private string body;
        private string schema;
        private SqlTextEditor editor;

        public ProcedureNode(ExplorerNode parent, string caption, DataRow row)
            : base(parent, caption)
        {
            schema = row["ROUTINE_SCHEMA"].ToString();
            body = row["ROUTINE_DEFINITION"].ToString();
        }

        public ProcedureNode(ExplorerNode parent, string caption, string body)
            : base(parent, caption)
        {
            this.body = body;
            schema = GetDatabaseNode().Caption;
            ItemId = VSConstants.VSITEMID_NIL;
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
                case PkgCmdIDList.cmdidDelete:
                    Delete();
                    break;
                case PkgCmdIDList.cmdidOpen:
                    OpenEditor();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        public override bool Save()
        {
            try
            {
                ExecuteNonQuery(editor.SqlText);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Delete()
        {
            // first make sure the user is sure
            if (MessageBox.Show(
                String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                Caption),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            string sql = String.Format("DROP PROCEDURE {0}.{1}", schema, Caption);
            try
            {
                ExecuteNonQuery(sql);
                //delete was successful, remove this node
                Parent.RemoveChild(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, 
                    String.Format(MyVSTools.GetResourceString("UnableToDeleteTitle"),
                    Caption), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal override BaseEditor GetEditor()
        {
            editor = new SqlTextEditor(
                Caption, GetDatabaseNode().Caption, body, GetOpenConnection());
            return editor;
        }

    }
}
