using System;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.VSTools
{
    internal class ProcedureNode : ExplorerNode
    {
        private string body;

        public ProcedureNode(ExplorerNode parent, string caption, DataRow row)
            : base(parent, caption)
        {
            if (row != null)
                body = String.Format("DROP PROCEDURE {0}.{1}; {2}",
                    Schema, Caption, row["ROUTINE_DEFINITION"].ToString());
            else
                body = String.Format("CREATE PROCEDURE {0}.{1} AS\r\nBEGIN\r\nEND",
                    Schema, Caption);
        }

        #region Properties

        public override uint MenuId
        {
            get { return PkgCmdIDList.ProcedureCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 4; }
        }

        public override bool Expandable
        {
            get { return false; }
        }

        #endregion

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidOpen:
                    OpenEditor();
                    break;
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

        public override bool Save()
        {
            try
            {
                ExecuteNonQuery((activeEditor as SqlTextEditor).SqlText);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        internal override BaseEditor GetEditor()
        {
            activeEditor = new SqlTextEditor(this, body);
            return activeEditor;
        }

        protected override string GetDeleteSql()
        {
            return String.Format("DROP PROCEDURE {0}.{1}", Schema, Caption);
        }
    }
}
