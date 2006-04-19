using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace MySql.VSTools
{
    internal class TableNode : ExplorerNode
    {
        private DataRow tableDef;

        public TableNode(ExplorerNode parent, string name, DataRow row)
            : base(parent, name)
        {
            tableDef = row;
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.TableCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 2; }
        }

        public override bool Expandable
        {
            get { return true; }
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidDelete:
                    Delete();
                    break;
                case PkgCmdIDList.cmdidOpenTableDef:
                    EditTable();
                    break;
                case PkgCmdIDList.cmdidShowTableData:
                    ShowTableData();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        private void EditTable()
        {
            TableEditor te = new TableEditor();
            OpenEditor(te);
        }

        private void Delete()
        {
            // first make sure the user is sure
            if (MessageBox.Show(String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                tableDef["TABLE_NAME"]),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            System.Data.Common.DbConnection conn;
            conn = GetOpenConnection();
            System.Data.Common.DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE " + tableDef["TABLE_SCHEMA"] + "." +
                tableDef["TABLE_NAME"];
            try
            {
                cmd.ExecuteNonQuery();
                //delete was successful, remove this node
            //    this.Remove();  
                //TODO: do the remove
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    String.Format(MyVSTools.GetResourceString("UnableToDeleteTitle"),
                    tableDef["TABLE_NAME"]),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowTableData()
        {
            Microsoft.VisualStudio.Shell.ToolWindowPane pane = 
                (PackageSingleton.Package as MyVSTools).CreateAndShowToolWindow(
                typeof(TableDataWindow), tableDef["TABLE_NAME"].ToString(),
                null);
            (pane.Window as TableDataControl).SetData(
                GetOpenConnection(), tableDef["TABLE_SCHEMA"] +
                "." + tableDef["TABLE_NAME"]);
        }


        public override void Populate()
        {
            if (populated) return;
            DbConnection conn = GetOpenConnection();
            string[] restrictions = new string[4];
            restrictions[1] = GetDatabaseNode().Caption;
            restrictions[2] = Caption;
            DataTable columns = conn.GetSchema("columns", restrictions);
            foreach (DataRow column in columns.Rows)
                AddChild(new ColumnNode(this, column));
            populated = true;
            try
            {
                restrictions[2] = null;
                restrictions[3] = Caption;
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
