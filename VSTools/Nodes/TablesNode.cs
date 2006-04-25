using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;

namespace MySql.VSTools
{
    internal class TablesNode : ExplorerNode
    {
        public TablesNode(ExplorerNode parent)
            : base(parent, MyVSTools.GetResourceString("TablesNodeTitle"))
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
            get { return PkgCmdIDList.TablesCtxtMenu; }
        }

        #endregion

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidAddNewTable:
                    AddNewTable();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        private void AddNewTable()
        {
            //OpenEditor(typeof(EditorPane));
        }

        public override void Populate()
        {
            if (populated) return;

            DbConnection c = GetOpenConnection();
            if (c == null) return;

            try
            {
                DataTable table = c.GetSchema("Tables", 
                    new string[] { null, Parent.Name, null, null });

                foreach (DataRow row in table.Rows)
                    AddChild(new TableNode(this, row["table_name"].ToString(), row));
                populated = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
