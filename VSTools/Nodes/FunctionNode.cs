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
        private string body;

        public FunctionNode(ExplorerNode parent, string name, DataRow row)
            : base(parent, name)
        {
            if (row != null)
                body = String.Format("DROP FUNCTION {0}.{1}; {2}",
                    Schema, Caption, row["ROUTINE_DEFINITION"].ToString());
            else
                body = String.Format("CREATE FUNCTION {0}.{1} RETURNS /*INT*/ AS\r\nBEGIN\r\nEND",
                    Schema, Caption);
        }

        #region Properties

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

        protected override string GetDeleteSql()
        {
            return String.Format("DROP FUNCTION {0}.{1}", Schema, Caption);
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
    }
}
