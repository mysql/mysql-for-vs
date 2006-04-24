using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace MySql.VSTools
{
    internal class TriggerNode : ExplorerNode
    {
        private string body;
        private string actionTiming;
        private string action;

        public TriggerNode(ExplorerNode parent, string name)
            : base(parent, name)
        {
            actionTiming = "BEFORE";
            action = "INSERT";
            body = String.Empty;
            isNew = true;
        }

        public TriggerNode(ExplorerNode parent, DataRow row)
            : base(parent, row["TRIGGER_NAME"].ToString())
        {
            body = row["ACTION_STATEMENT"].ToString();
            actionTiming = row["ACTION_TIMING"].ToString();
            action = row["EVENT_MANIPULATION"].ToString();
            isNew = false;
        }

        #region Properties

        public string Body
        {
            get { return body; }
        }

        public string Schema
        {
            get { return GetDatabaseNode().Caption; }
        }

        public string ActionTime
        {
            get { return actionTiming; }
        }

        public string Action
        {
            get { return action; }
        }

        #endregion

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

        public override bool Save()
        {
            TriggerEditor editor = (activeEditor as TriggerEditor);
            StringBuilder sql = new StringBuilder();
            TableNode parentTable = (Parent as TableNode);
            if (!isNew)
                sql.AppendFormat("DROP TRIGGER {0}.{1};", Schema, Caption);
            sql.AppendFormat("CREATE TRIGGER {0} {1} {2} ON {3}.{4} FOR EACH ROW {5}",
                editor.TriggerName, editor.ActionTiming, editor.Action,
                parentTable.Schema, parentTable.Caption, editor.Body);
            try
            {
                ExecuteNonQuery(sql.ToString());
                if (isNew)
                    parentTable.AddChild(this);
                activeEditor = null;
                return true;
            }
            catch (Exception ex)
            {
                if (isNew)
                    MessageBox.Show("Error creating trigger: " + ex.Message);
                else
                    MessageBox.Show("Error updating trigger: " + ex.Message);
                return false;
            }
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
            // first make sure the user is sure
            if (MessageBox.Show(
                String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                Caption),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            string sql = String.Format("DROP TRIGGER {0}.{1}", Schema, Caption);
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
            activeEditor = new TriggerEditor(this);
            return activeEditor;
        }

        internal void Open()
        {
            if (activeEditor != null) return;
            OpenEditor();
        }
    }
}
