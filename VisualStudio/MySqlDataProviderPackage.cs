// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of root package class.
 */

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using MySql.Data.VisualStudio;
using MySql.Data.VisualStudio.Utils;
using System.Reflection;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Diagnostics;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Properties;
using System.Data.Common;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// Implements necessary for root VS package class functionality. 
    /// Registers factory object for DDEX support entities.
    /// </summary>
    [ComVisible(true)]
    [Guid(GuidList.PackageGUIDString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
#if DEBUG
    [DefaultRegistryRoot(@"Microsoft\VisualStudio\8.0Exp")]
#else
    [DefaultRegistryRoot(@"Microsoft\VisualStudio\8.0")]
#endif
    [ProvideService(typeof(MySqlProviderObjectFactory), ServiceName = "MySQL Provider Object Factory")]
    [ProvideMenuResource(1000, 1)]
    [ProvideLoadKey("standard", "1.1", "MySQL Tools for Visual Studio", "MySQL AB c/o MySQL, Inc.", 100)]
    public class MySqlDataProviderPackage : Package, IVsInstalledProduct
    {
        /// <summary>
        /// Returns instance of the package.
        /// </summary>
        public static Package Instance
        {
            get
            {
                Debug.Assert(instanceRef != null, "Package is not initialized!");
                return instanceRef;
            }
        }

        /// <summary>
        /// Registers factory object for DDEX support entities.
        /// </summary>
        protected override void Initialize()
        {
            ((IServiceContainer)this).AddService(typeof(MySqlProviderObjectFactory), new ServiceCreatorCallback(CreateService), true);
            base.Initialize();

            try
            {
                DbProviderFactory f = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.MySqlClientNotRegistered);
            }

            instanceRef = this;

            // Initialize package
            InitializePackage();
        }

        public object GetVsService(Type serviceType)
        {
            return GetService(serviceType);
        }
        
        /// <summary>
        /// Enumerate all types and their custom attributes. It is necessary 
        /// to get registration attributes to work. They won’t be created 
        /// otherwise.
        /// </summary>
        public static void InitializePackage()
        {
            Assembly current = Assembly.GetExecutingAssembly();
            foreach (Type type in current.GetTypes())
                type.GetCustomAttributes(false);
        }

        /// <summary>
        /// Creates factory object for DDEX support entities.
        /// </summary>
        /// <param name="container">Not used.</param>
        /// <param name="serviceType">Must be typeof(MySqlProviderObjectFactory).</param>
        /// <returns>Reference to created factory.</returns>
        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(MySqlProviderObjectFactory))
            {
                return new MySqlProviderObjectFactory();
            }
            return null;
        }

        /// <summary>
        /// Used to store reference to the package instance.
        /// </summary>
        private static Package instanceRef = null;

        #region IVsInstalledProduct Members

        int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return VSConstants.E_NOTIMPL;
        }

        int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 101;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.OfficialName(out string pbstrName)
        {
            pbstrName = Resources.ProductName;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = Resources.ProductDetails;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductID(out string pbstrPID)
        {
            pbstrPID = "1.1";
            return VSConstants.S_OK;
        }

        #endregion
    }
}
