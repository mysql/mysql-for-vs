using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

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

        #region Properties

        public string Schema
        {
            get { return tableDef["TABLE_SCHEMA"].ToString(); }
        }

        public string TypeName
        {
            get { return tableDef["ENGINE"].ToString(); }
        }

        public string DataDirectory
        {
            get { return String.Empty; }
        }

        public string IndexDirectory
        {
            get { return String.Empty; }
        }

        public string RowFormat
        {
            get { return tableDef["ROW_FORMAT"].ToString(); }
        }

        public bool UseChecksum
        {
            get { return false; }
        }

        public int MinimumRowCount
        {
            get { return 0; }
        }

        public int MaximumRowCount
        {
            get { return 0; }
        }

        [Category("Row Options")]
        [Description("Defines how the rows in MyISAM tables should be stored.  The option " +
                      "ValueType can be FIXED or DYNAMIC for static or variable-length row " +
                      "format.  The utility myisampack can be used to set the type to " +
                      "COMPRESSED.")]
        public string Password
        {
            get { return "mypass"; }
        }

        [Category("Row Options")]
        [Description("An approximation of the average row length for your table.  You " +
                     "need to set this only for large tables with variable-sized records.")]
        public int AverageRowLength
        {
            get { return Int32.Parse(tableDef["AVG_ROW_LENGTH"].ToString()); }
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

        #endregion

        public ArrayList GetColumns()
        {
            if (!populated)
                Populate();

            ArrayList cols = new ArrayList();
            ExplorerNode col = FirstChild;
            while (col != null)
            {
                if (col is ColumnNode)
                    cols.Add(col);
                col = col.NextSibling;
            }
            return cols;
        }


        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidOpenTableDef:
                    OpenEditor();
                    break;
                case PkgCmdIDList.cmdidShowTableData:
                    ShowTableData();
                    break;
                case PkgCmdIDList.cmdidAddNewTrigger:
                    AddNewTrigger();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        internal void AddNewTrigger()
        {
            // first determine a default name
            int num = 1;
            string name = String.Format("trigger{0}", num);
            ExplorerNode node = FirstChild;
            while (node != null)
            {
                if (node.Caption.ToLower(CultureInfo.InvariantCulture) == name)
                    name = String.Format("trigger{0}", ++num);
                node = node.NextSibling;
            }
            TriggerNode newTrigger = new TriggerNode(this, name);
            IndexChild(newTrigger);
            newTrigger.Open();
        }

        internal override BaseEditor GetEditor()
        {
            TableEditor editor = new TableEditor(this);
            return editor;
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
