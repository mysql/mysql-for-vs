// Copyright © 2010, Oracle and/or its affiliates. All rights reserved.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
    internal class ServiceBroker
    {
        private IOleServiceProvider psp;
        private ServiceProvider site;

        public ServiceBroker(ServiceProvider site)
        {
            this.site = site;
            psp = (IOleServiceProvider)site.GetService(typeof(IOleServiceProvider).GUID);
        }

        public ServiceBroker(IOleServiceProvider psp)
        {
            site = null;
            this.psp = psp;
        }

        #region Common Interfaces

        public IOleServiceProvider IOleServiceProvider
        {
            get { return psp; }
        }

        public ILocalRegistry LocalRegistry 
        {
            get { return OleService<ILocalRegistry>(typeof(SLocalRegistry)); }
        }

        public IVsRegisterPriorityCommandTarget VsRegisterPriorityCommandTarget
        {
            get { return OleService<IVsRegisterPriorityCommandTarget>(typeof(SVsRegisterPriorityCommandTarget)); }
        }

        public IVsFilterKeys2 VsFilterKeys2 
        {
            get { return OleService<IVsFilterKeys2>(typeof(SVsFilterKeys)); }
        }

        #endregion

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="localRegistry">Establishes a locally-registered COM object relative to the local Visual Studio registry hive</param>
        /// <param name="clsid">GUID if object to be created</param>
        /// <param name="iid">GUID assotiated with specified System.Type</param>
        /// <returns>An object</returns>
        public object CreateObject(ILocalRegistry localRegistry, Guid clsid, Guid iid)
        {
            object objectInstance;
            IntPtr unknown = IntPtr.Zero;

            int hr = localRegistry.CreateInstance(clsid, null, ref iid, (uint)CLSCTX.CLSCTX_INPROC_SERVER, out unknown);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            try
            {
                objectInstance = Marshal.GetObjectForIUnknown(unknown);
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                {
                    Marshal.Release(unknown);
                }
            }

            // Try to site object instance
            IObjectWithSite objectWithSite = objectInstance as IObjectWithSite;
            if (objectWithSite != null)
                objectWithSite.SetSite(psp);

            return objectInstance;
        }


        private InterfaceType OleService<InterfaceType>(Type serviceType)
            where InterfaceType : class
        {
            Guid serviceGuid = serviceType.GUID;
            Guid interfaceGuid = typeof(InterfaceType).GUID;
            IntPtr unknown = IntPtr.Zero;
            InterfaceType service = null;
            int hr = psp.QueryService(ref serviceGuid, ref interfaceGuid, out unknown);

            if (hr != VSConstants.S_OK)
                Marshal.ThrowExceptionForHR(hr);

            try
            {
                service = (InterfaceType)Marshal.GetObjectForIUnknown(unknown);
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                    Marshal.Release(unknown);
            }
            return service;
        }
    }
}
