using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace MySql.VSTools
{
    internal class ColumnNode : ExplorerNode
    {
        private DataRow columnDef;

        public ColumnNode(ExplorerNode parent, DataRow row)
            : base(parent, row["COLUMN_NAME"].ToString())
        {
            columnDef = row;
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ColumnCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 8; }
        }

        public override bool Expandable
        {
            get { return false; }
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

    }
}
