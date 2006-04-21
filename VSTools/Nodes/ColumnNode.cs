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

        #region Properties

        public string Typename
        {
            get { return columnDef["DATA_TYPE"].ToString().ToUpper(); }
        }

        public string LengthAsString
        {
            get
            {
                string len = columnDef["CHARACTER_MAXIMUM_LENGTH"].ToString();
                if (len == "NULL") return String.Empty;
                return len;
            }
        }

        public bool CanBeNull
        {
            get { return columnDef["IS_NULLABLE"].Equals("YES"); }
        }

        public bool IsBinary
        {
            get { return columnDef["EXTRA"].ToString().IndexOf("binary") != -1; }
        }

        public bool ZeroFill
        {
            get { return columnDef["EXTRA"].ToString().IndexOf("zero") != -1; }
        }

        #endregion

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
