using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using System.Data;

namespace Vsip.MyVSTools
{
    internal class ViewNode : ExplorerNode
    {
        private DataRow viewDef;

        public ViewNode(ExplorerNode parent, string name, DataRow row)
            : base(parent, name)
        {
            viewDef = row;
        }

        public override bool Expandable
        {
            get { return false; }
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
        }

        public override void Populate()
        {
        }

    }
}
