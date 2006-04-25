using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Data;

namespace MySql.VSTools
{
    internal class ViewNode : ExplorerNode
    {
        private string body;

        public ViewNode(ExplorerNode parent, string name, DataRow row)
            : base(parent, name)
        {
            if (row != null)
                body = String.Format("ALTER VIEW {0}.{1} AS {2}",
                    Schema, Name, row["VIEW_DEFINITION"].ToString());
            else
                body = String.Format("CREATE VIEW {0}.{1} AS ",
                    Schema, Name);
            isNew = row == null ? true : false;
        }

        #region Properties

        public string Body
        {
            get { return body; }
        }

        #endregion

        public override bool Expandable
        {
            get { return true; }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ViewCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 6; }
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidAddNewView:
                    (Parent as ViewsNode).AddNewView();
                    break;
                case PkgCmdIDList.cmdidOpen:
                    OpenEditor();
                    break;
                case PkgCmdIDList.cmdidDelete:
                    Delete();
                    break;
            }
        }

        private void Delete()
        {
            // first make sure the user is sure
            if (MessageBox.Show(
                String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                Name),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            string sql = String.Format("DROP VIEW {0}.{1}", Schema, Name);
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
                    Name), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public override bool Save()
        {
            SqlTextEditor editor = (activeEditor as SqlTextEditor);
            body = editor.SqlText;
            ViewsNode parentTable = (Parent as ViewsNode);
            try
            {
                ExecuteNonQuery(body);
                if (isNew)
                    parentTable.AddChild(this);
                activeEditor.IsDirty = false;
                return true;
            }
            catch (Exception ex)
            {
                if (isNew)
                    MessageBox.Show("Error creating view: " + ex.Message);
                else
                    MessageBox.Show("Error updating view: " + ex.Message);
                return false;
            }
        }

        internal override BaseEditor GetEditor()
        {
            activeEditor = new SqlTextEditor(this, body);
            return activeEditor;
        }

        public override void Populate()
        {
            if (populated) return;
            DbConnection conn = GetOpenConnection();
            string[] restrictions = new string[4];
            restrictions[1] = Schema;
            restrictions[2] = Name;
            DataTable columns = conn.GetSchema("columns", restrictions);
            foreach (DataRow column in columns.Rows)
                AddChild(new ColumnNode(this, column));
            populated = true;
            try
            {
                restrictions[2] = null;
                restrictions[3] = Name;
                DataTable triggers = conn.GetSchema("triggers", restrictions);
                foreach (DataRow trigger in triggers.Rows)
                    AddChild(new TriggerNode(this, trigger));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
