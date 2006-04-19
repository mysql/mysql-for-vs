using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace MySql.VSTools
{
    internal class TriggerNode : ExplorerNode
    {
        private DataRow triggerDef;
        private string body;

        public TriggerNode(ExplorerNode parent, DataRow row)
            : base(parent, row["TRIGGER_NAME"].ToString())
        {
            triggerDef = row;
            body = triggerDef["ACTION_STATEMENT"].ToString();
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.TriggerCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 7; }
        }

        public override bool Expandable
        {
            get { return false; }
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidDelete:
                    Delete();
                    break;
                case PkgCmdIDList.cmdidOpen:
                    Open();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        private void Delete()
        {
        }

        internal void Open()
        {
            SqlTextEditor editor = new SqlTextEditor(
                Caption, GetDatabaseNode().Caption, body, GetOpenConnection());
            OpenEditor(editor);
        }

    }
}
