using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE;
using EnvDTE80;

namespace MySql.Data.VisualStudio.Editors
{
	public class BaseEditor : UserControl, IOleCommandTarget
	{
        protected List<object> objectsForInspection = new List<object>();

        protected void SelectObject(object objectToInspect)
        {
            objectsForInspection.Clear();
            objectsForInspection.Add(objectToInspect);
        }
#if false
        protected void ShowToolBar(string name, bool show)
        {
            DTE dte = (DTE)vsServiceProvider.GetService(typeof(DTE));

            bool shouldUpdate = false;
            foreach (CommandBar bar in (CommandBars)dte.CommandBars)
            {
                if (bar.Name == name)
                {
                    bar.Enabled = show;
                    bar.Visible = show;
                    bar.Position = MsoBarPosition.msoBarTop;
                    shouldUpdate = true;
                }
            }
            if (shouldUpdate)
                (dte.Commands as Commands2).UpdateCommandUI(true);
        }
#endif
        public virtual OLECMDF GetCommandStatus(Guid group, uint cmdId)
        {
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        public virtual void ExecuteCommand(Guid group, uint cmdId)
        {
        }

        #region IOleCommandTarget Members

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, 
            IntPtr pvaIn, IntPtr pvaOut)
        {
            ExecuteCommand(pguidCmdGroup, nCmdID);
            return VSConstants.S_OK;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, 
            OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == GuidList.DavinciCommandSet)
            {
                Console.WriteLine("found it");
            }
            Console.WriteLine("{0}-{1}", pguidCmdGroup.ToString(), prgCmds[0]);
            for (int i = 0; i < cCmds; i++)
            {
                prgCmds[i].cmdf = (uint)GetCommandStatus(pguidCmdGroup, prgCmds[i].cmdID);
            }
            return VSConstants.S_OK;
        }

        #endregion
    }
}
