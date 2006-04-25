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

        [Category("Storage")]
        [Description("MySQL engine this table is using")]
        public string Engine
        {
            get { return tableDef["ENGINE"].ToString(); }
        }

        [Category("Storage")]
        [Description("Where the data files are stored")]
        [DisplayName("Data Directory")]
        public string DataDirectory
        {
            get { return String.Empty; }
        }

        [Category("Storage")]
        [Description("Where the index files are stored")]
        [DisplayName("Index Directory")]
        public string IndexDirectory
        {
            get { return String.Empty; }
        }

        [Browsable(false)]
        public string RowFormat
        {
            get { return tableDef["ROW_FORMAT"].ToString(); }
        }

        [Browsable(false)]
        public bool UseChecksum
        {
            get { return false; }
        }

        [Browsable(false)]
        public int MinimumRowCount
        {
            get { return 0; }
        }

        [Browsable(false)]
        public int MaximumRowCount
        {
            get { return 0; }
        }

        [Browsable(false)]
        public string Password
        {
            get { return "mypass"; }
        }

        [Browsable(false)]
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
                if (node.Name.ToLower(CultureInfo.InvariantCulture) == name)
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

        protected override string GetDeleteSql()
        {
            return String.Format("DROP TABLE {0}.{1}", Schema, Name);
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
