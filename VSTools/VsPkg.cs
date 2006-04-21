// VsPkg.cs : Implementation of MyVSTools
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;

[assembly:ComVisible(true)]

namespace MySql.VSTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\8.0")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#100", "#102", "1.0", IconResourceID = 400)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "MyVSTools", "MySQL, Inc.", 1)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(MyToolWindow))]
    [ProvideToolWindow(typeof(QueryToolWindow), Style = Microsoft.VisualStudio.Shell.VsDockStyle.MDI)]
    [ProvideToolWindow(typeof(TableDataWindow), Style = Microsoft.VisualStudio.Shell.VsDockStyle.MDI)]
    [ProvideEditorLogicalView(typeof(EditorFactory), "{7651a703-06e5-11d1-8ebd-00a0c90f26ea}")]
    [Guid("5ceb61c4-7111-44f8-b7f2-ac049b81ad32")]
    public sealed class MyVSTools : Package
    {
        const int bitmapResourceID = 300;
        private EditorFactory editorFactory;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public MyVSTools()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            PackageSingleton.Package = this;
        }

        public EditorFactory EditorFactory
        {
            get { return editorFactory; }
        }

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(MyToolWindow), 
                PackageSingleton.ToolWindowId, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new COMException(GetResourceString("CanNotCreateWindow"));
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }


        /// <summary>
        /// Helper function that will load a resource string using the standard Visual Studio Resource Manager
        /// Service (SVsResourceManager). Because of the fact that it is using a service, this method can be
        /// called only after the package is sited.
        /// </summary>
        internal static string GetResourceString(string resourceName)
        {
            string resourceValue;
            IVsResourceManager resourceManager = (IVsResourceManager)GetGlobalService(typeof(SVsResourceManager));
            if (resourceManager == null)
            {
                throw new InvalidOperationException("Could not get SVsResourceManager service. Make sure the package is Sited before calling this method.");
            }
            Guid packageGuid = typeof(MyVSTools).GUID;
            int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            return resourceValue;
        }
        internal static string GetResourceString(int resourceID)
        {
            return GetResourceString(string.Format("@{0}", resourceID));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            //Create Editor Factory
            editorFactory = new EditorFactory(this);
            base.RegisterEditorFactory(editorFactory);

            // Add our command handlers for menu (commands must exist in the .ctc file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidMyVSToolsCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand( new EventHandler(MenuItemCallback), menuCommandID );
                mcs.AddCommand(menuItem);

                CommandID refreshCommandID = new CommandID(GuidList.guidMyVSToolsCmdSet, 
                    (int)PkgCmdIDList.cmdidRefresh);
                MenuCommand refreshMenuItem = new MenuCommand(
                    new EventHandler(RefreshCallback), refreshCommandID);
                mcs.AddCommand(refreshMenuItem);

                
                // Create the command for the tool window
                CommandID toolwndCommandID = new CommandID(GuidList.guidMyVSToolsCmdSet, (int)PkgCmdIDList.cmdidMyExplorerWindow);
                MenuCommand menuToolWin = new MenuCommand( new EventHandler(ShowToolWindow), toolwndCommandID);
                mcs.AddCommand( menuToolWin );
            }
        }

        #endregion

        private void SaveConnection(string name, string connectString)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                System.IO.Path.DirectorySeparatorChar +
                "MySQL";
            System.IO.Directory.CreateDirectory(path);
            path += System.IO.Path.DirectorySeparatorChar +
                "MyVSTools.conf"; 

            System.IO.StreamWriter writer = System.IO.File.AppendText(path);
            writer.WriteLine(String.Format("{0}:{1}", name, connectString));
            writer.Flush();
            writer.Close();
        }

        private void RefreshCallback(object sender, EventArgs e)
        {
            MyToolWindow window = this.FindToolWindow(typeof(MyToolWindow), 0, true) as MyToolWindow;
            window.RefreshServerList();
        }

        internal object GetMyService(Type serviceType)
        {
            return GetService(serviceType);
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;

            ConnectionDlg dlg = new ConnectionDlg();
            if (dlg.ShowDialog() == DialogResult.Cancel) return;

            //save connect string to save file
            SaveConnection(dlg.ConnectionName, dlg.ConnectionString);

            // if explorer window is open, ask it to refresh
            MyToolWindow window = (MyToolWindow)this.FindToolWindow(typeof(MyToolWindow), 0, true);
            if (window != null)
                window.AddServer(dlg.ConnectionName, dlg.ConnectionString);


/*            uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "MyVSTools",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,        // false
                       out result);*/
        }

        public ToolWindowPane CreateAndShowToolWindow(Type toolWindowType, string caption, object data)
        {
            Microsoft.VisualStudio.Shell.ToolWindowPane pane =
                PackageSingleton.Package.FindToolWindow(toolWindowType,
                PackageSingleton.ToolWindowId, true);
            if ((null == pane) || (null == pane.Frame))
            {
                throw new System.Runtime.InteropServices.COMException(
                    MyVSTools.GetResourceString("CanNotCreateWindow"));
            }
            pane.Caption = caption;
            IVsWindowFrame windowFrame = (IVsWindowFrame)pane.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return pane;
        }

    }
}