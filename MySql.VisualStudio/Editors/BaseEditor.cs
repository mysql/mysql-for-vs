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

namespace MySql.Data.VisualStudio.Editors
{
	public class BaseEditor : UserControl, IVsWindowPane, IOleCommandTarget, IVsWindowFrameNotify
	{
        protected List<object> objectsForInspection = new List<object>();
        private ServiceProvider vsServiceProvider;

        protected void SelectObject(object objectToInspect)
        {
            objectsForInspection.Clear();
            objectsForInspection.Add(objectToInspect);
        }

        protected void ShowToolBar(string name, bool show)
        {
//            DTE dte = (DTE)vsServiceProvider.GetService(typeof(DTE));

  //          bool shouldUpdate = false;
  //          foreach (CommandBar bar in (CommandBars)dte.CommandBars)
    //        {
      //          if (bar.Name == name)
        //        {
          //          bar.Enabled = show;
            //        bar.Visible = show;
              //      bar.Position = MsoBarPosition.msoBarTop;
//                    shouldUpdate = true;
                //}
            //}
    //        if (shouldUpdate)
      //          (dte.Commands as Commands2).UpdateCommandUI(true);
        }

        public virtual OLECMDF GetCommandStatus(Guid group, uint cmdId)
        {
            return OLECMDF.OLECMDF_INVISIBLE;
        }

        public virtual void ExecuteCommand(Guid group, uint cmdId)
        {
        }

        public virtual void Showing(int showing)
        {
        }

		#region IVsWindowPane Members

		public int ClosePane()
		{
			//SaveSettings();
			Dispose(true);
			return VSConstants.S_OK;
		}

		public int CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr hwnd)
		{
			Win32Methods.SetParent(Handle, hwndParent);
			hwnd = Handle;

            // set up our property window tracking
            Microsoft.VisualStudio.Shell.SelectionContainer selContainer = 
                new Microsoft.VisualStudio.Shell.SelectionContainer();
            selContainer.SelectableObjects = objectsForInspection;
            selContainer.SelectedObjects = objectsForInspection;
            ITrackSelection trackSelectionRef = vsServiceProvider.GetService(
                typeof(STrackSelection)) as ITrackSelection;
            trackSelectionRef.OnSelectChange(selContainer);


//			Size = new System.Drawing.Size(cx - x, cy - y);

			// Loading configuration settings
//			LoadSettings();

			return VSConstants.S_OK;
		}

		public int GetDefaultSize(Microsoft.VisualStudio.OLE.Interop.SIZE[] pSize)
		{
/*			if (pSize.Length >= 1)
			{
				pSize[0].cx = Size.Width;
				pSize[0].cy = Size.Height;
			}
*/
			return VSConstants.S_OK;
		}

		public int LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
		{
			return VSConstants.S_OK;
		}

		public int SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
		{
			return VSConstants.S_OK;
		}

		public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
		{
			vsServiceProvider = new ServiceProvider(psp);
			return VSConstants.S_OK;
		}

		public int TranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
		{
			return VSConstants.S_FALSE;
		}

		#endregion

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
            for (int i = 0; i < cCmds; i++)
            {
                prgCmds[i].cmdf = (uint)GetCommandStatus(pguidCmdGroup, prgCmds[i].cmdID);
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsWindowFrameNotify Members

        int IVsWindowFrameNotify.OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        int IVsWindowFrameNotify.OnMove()
        {
            return VSConstants.S_OK;
        }

        int IVsWindowFrameNotify.OnShow(int fShow)
        {
            Showing(fShow);
            return VSConstants.S_OK;
        }

        int IVsWindowFrameNotify.OnSize()
        {
            return VSConstants.S_OK;
        }

        #endregion

    }
}
