using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace MySql.VSTools
{
    [Guid("C1BEFA59-9F84-443d-ABF0-035AE4CEE549")]
    class QueryToolWindow : ToolWindowPane, IOleCommandTarget
    {
        private QueryControl queryControl;

        public QueryToolWindow() : base(null)
        {
            this.ToolBar = new CommandID(GuidList.guidMyVSToolsCmdSet, 
                (int)PkgCmdIDList.QueryToolbar);
            queryControl = new QueryControl();

            // Set the window title reading it from the resources.
            Caption = MyVSTools.GetResourceString("QueryWindowTitle");

            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            BitmapResourceID = 300;
            BitmapIndex = 14;
        }

        public override System.Windows.Forms.IWin32Window Window
        {
            get { return queryControl; }
        }

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            DebugTrace.Trace("group = " + pguidCmdGroup + ":cmdid=" + nCmdID);
            if (pguidCmdGroup != GuidList.guidMyVSToolsCmdSet)
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;

            switch (nCmdID)
            {
                case PkgCmdIDList.cmdidExecuteQuery:
                    queryControl.Execute();
                    break;
                case PkgCmdIDList.cmdidShowSQL:
                    queryControl.ShowingSql = !queryControl.ShowingSql;
                    break;
                case PkgCmdIDList.cmdidShowGrid:
                    queryControl.ShowingGrid = !queryControl.ShowingGrid;
                    break;
                case PkgCmdIDList.cmdidShowText:
                    queryControl.ShowingText = !queryControl.ShowingText;
                    break;
            }
            return VSConstants.S_OK;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup != GuidList.guidMyVSToolsCmdSet)
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;

            for (int i = 0; i < cCmds; i++)
            {
                if (prgCmds[i].cmdID == PkgCmdIDList.cmdidShowGrid)
                    prgCmds[i].cmdf = (uint)(queryControl.ShowingGrid ? OLECMDF.OLECMDF_LATCHED :
                        OLECMDF.OLECMDF_ENABLED);
                if (prgCmds[i].cmdID == PkgCmdIDList.cmdidShowText)
                    prgCmds[i].cmdf = (uint)(queryControl.ShowingText ? OLECMDF.OLECMDF_LATCHED :
                        OLECMDF.OLECMDF_ENABLED);
                if (prgCmds[i].cmdID == PkgCmdIDList.cmdidShowSQL)
                    prgCmds[i].cmdf = (uint)(queryControl.ShowingSql ? OLECMDF.OLECMDF_LATCHED :
                        OLECMDF.OLECMDF_ENABLED);
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}
