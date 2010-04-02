using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
    class SqlEditorPane : WindowPane//, IOleCommandTarget
    {
        private SqlEditor editor;

        public SqlEditorPane(ServiceProvider sp)
            : base(sp)
        {
            editor = new SqlEditor(sp);
        }

        public override IWin32Window Window
        {
            get { return editor; }
        }

        //#region IOleCommandTarget Members

        //int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
        //    IntPtr pvaIn, IntPtr pvaOut)
        //{
        //    return base.Ex
        //    return VSConstants.S_OK;
        //}

        //int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds,
        //    OLECMD[] prgCmds, IntPtr pCmdText)
        //{
        //    return VSConstants.S_OK;
        //}

        //#endregion
    }
}
