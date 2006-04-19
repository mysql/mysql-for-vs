using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data;

namespace MySql.VSTools
{
    internal class FunctionNode : ExplorerNode
    {
        DataRow funcDef;

        public FunctionNode(ExplorerNode parent, string name, DataRow row)
            : base(parent, name)
        {
            funcDef = row;
        }

        public override bool Expandable
        {
            get { return false; }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.FunctionCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 5; }
        }
        
        public override void Populate()
        {
        }

        private void Open()
        {
            SqlTextEditor editor = new SqlTextEditor(
                Caption, funcDef["ROUTINE_SCHEMA"].ToString(),
                funcDef["ROUTINE_DEFINITION"].ToString(),
                GetOpenConnection());
            OpenEditor(editor);
        }

    }
}
