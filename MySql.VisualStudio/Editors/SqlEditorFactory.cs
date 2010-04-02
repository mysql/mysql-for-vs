using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
    [Guid(GuidStrings.SqlEditorFactory)]
    public sealed class SqlEditorFactory : IVsEditorFactory, IDisposable 
    {
        private MySqlDataProviderPackage package;
        private ServiceProvider serviceProvider;

        #region IVsEditorFactory Members

        int IVsEditorFactory.Close()
        {
            return VSConstants.S_OK;
        }

        int IVsEditorFactory.CreateEditorInstance(uint grfCreateDoc, string pszMkDocument, 
            string pszPhysicalView, IVsHierarchy pvHier, uint itemid, 
            IntPtr punkDocDataExisting, out IntPtr ppunkDocView, out IntPtr ppunkDocData, 
            out string pbstrEditorCaption, out Guid pguidCmdUI, out int pgrfCDW)
        {
            pgrfCDW = 0;
            pguidCmdUI = VSConstants.GUID_TextEditorFactory;
            SqlEditorPane editor = new SqlEditorPane(serviceProvider);
            ppunkDocData = Marshal.GetIUnknownForObject(editor.Window);
            ppunkDocView = Marshal.GetIUnknownForObject(editor);
            pbstrEditorCaption = "";
            return VSConstants.S_OK;
        }

        int IVsEditorFactory.MapLogicalView(ref Guid logicalView, out string physicalView)
        {
            physicalView = null;
            if (VSConstants.LOGVIEWID_Primary == logicalView)
            {
                // --- Primary view uses null as physicalView
                return VSConstants.S_OK;
            }
            else
            {
                // --- You must return E_NOTIMPL for any unrecognized logicalView values
                return VSConstants.E_NOTIMPL;
            }
        }

        int IVsEditorFactory.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            serviceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // --- Here we dispose all managed and unmanaged resources
                if (serviceProvider != null)
                {
                    serviceProvider.Dispose();
                    serviceProvider = null;
                }
            }
        }

        #endregion
    }
}
