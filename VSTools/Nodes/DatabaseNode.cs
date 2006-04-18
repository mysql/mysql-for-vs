using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MySql.VSTools
{
    internal class DatabaseNode : ExplorerNode
    {
        public DatabaseNode(ExplorerNode parent, string name)
            : base(parent, name)
        {
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.DatabaseCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 3; }
        }

        public override bool Expandable
        {
            get { return true; }
        }

        public override void Populate()
        {
            if (populated) return;

            AddChild(new TablesNode(this));
            AddChild(new ViewsNode(this));
            AddChild(new ProceduresNode(this));
            AddChild(new FunctionsNode(this));
            populated = true;
        }
    }
}
