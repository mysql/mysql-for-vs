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
 * This file contains implementation of the hierarchy accessor proxy.
 */
using System;
using System.Text;
using Microsoft.VisualStudio.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DocumentView;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class is used as a proxy between DataViewHierarchyAccessor class 
    /// and internal entities of this provider. Widely used by command handlers, 
    /// documents and views. MySqlDataViewCommandHandler creates an instance of 
    /// this mediator and provides it to the command handlers, which will pass 
    /// it further to documents and views.
    /// </summary>
    public class ServerExplorerFacade
    {
        #region Initialization
        /// <summary>
        /// Constructor stores DataViewHierarchyAccessor reference in the private variable.
        /// </summary>
        /// <param name="hierarchy">DataViewHierarchyAccessor reference.</param>
        public ServerExplorerFacade(DataViewHierarchyAccessor hierarchy)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (hierarchy.Connection == null)
                throw new ArgumentException(Resources.Error_InvalidAccessorNoConnection, "hierarchy");
            
            // Store hierarchy accessor reference
            hierarchyAccessor = hierarchy;
            //Extract connection wrapper
            connectionWrapper = new DataConnectionWrapper(hierarchy.Connection);

            // Get UI shell object
            uiShell = Package.GetGlobalService(typeof(IVsUIShell)) as IVsUIShell;
            Debug.Assert(uiShell != null, "Unable to get UI shell");
            if (uiShell == null)
                throw new Exception(Resources.Error_UnableToGetUIShell);

            // Retrieve IVsUIShellOpenDocument interface
            shell = Package.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
            Debug.Assert(shell != null, "Unable to get IVsUIShellOpenDocument reference!");
            if (shell == null)
                throw new Exception(Resources.Error_UnableToGetOpenDocumentShell);

            // Get RDT interface
            rdt = Package.GetGlobalService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            Debug.Assert(rdt != null, "Unable to get Running Document Table interface reference!");
            if (rdt == null)
                throw new Exception(Resources.Error_UnableToGetRdt);

            // Get RDT2 interface
            rdt2 = rdt as IVsRunningDocumentTable2;
            Debug.Assert(rdt2 != null, "Unable to get Running Document Table extended interface reference!");
            if (rdt2 == null)
                throw new Exception(Resources.Error_UnableToGetRdt);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Returns data connection mediator
        /// </summary>
        public DataConnectionWrapper Connection
        {
            get
            {
                Debug.Assert(connectionWrapper != null, "Connection wrapper is not initialized!");
                return connectionWrapper;
            }
        }

        /// <summary>
        /// Returns data connection mediator
        /// </summary>
        public IVsUIHierarchy Hierarchy
        {
            get
            {
                Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");
                return hierarchyAccessor.Hierarchy;
            }
        }

        public DataViewHierarchyAccessor Accessor
        {
            get { return hierarchyAccessor; }
        }

        #endregion

        #region Public methods for work with hierarchy items
        /// <summary>
        /// Returns indentifier of the parent hierarchy item.
        /// </summary>
        /// <param name="item">Identifier of the child hierarchy item.</param>
        /// <returns>Returns indentifier of the parent hierarchy item.</returns>
        public int GetParent(int item)
        {
            /*if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");*/
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            object value;
            Hierarchy.GetProperty((uint)item, (int)__VSHPROPID.VSHPROPID_Parent, out value);
            if (value is int)
                return (int)value;
            
            // Return int.MinValue to indicate error.
            return int.MinValue;
        }

        /// <summary>
        /// Returns object identifier array for the hierarchy item.
        /// </summary>
        /// <param name="item">Hierarchy item identifier to process.</param>
        /// <returns>Returns object identifier array for the hierarchy item.</returns>
        public object[] GetObjectIdentifier(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            // Extract identifier
            object[] result = hierarchyAccessor.GetObjectIdentifier(item);

            // Replace DBNull's with nulls
            if (result != null)
                for (int i = 0; i < result.Length; i++)
                    if (result[i] is DBNull) result[i] = null;

            // Return result
            return result;
        }

        /// <summary>
        /// Returns object name for the hierarchy item.
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process.</param>
        /// <returns>Returns object name for the hierarchy item.</returns>
        public string GetName(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            return hierarchyAccessor.GetNodeName(item);
        }

        /// <summary>
        /// Set new object name for the hierarchy item.
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process.</param>
        /// <param name="newName">New name value for the hierarchy item.</param>        
        public void SetName(int item, string newName)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            if (newName == null)
                throw new ArgumentNullException("newName");
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            hierarchyAccessor.SetProperty(item, (int)__VSHPROPID.VSHPROPID_Name, newName);
        }

        /// <summary>
        /// Returns array with possible children object type names for the hierarchy item.
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process.</param>
        /// <returns>Returns array with posible children object type names for the hierarchy item.</returns>
        /// 
        /// This routine is a complete hack since GetChildSelectionTypes
        /// appears to crash under VS2008.  we are rewriting the DDEX provider
        /// for 5.3 so this code will disappear.
        public string[] GetChildTypes(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            //string[] typeNames = new string[1];
            int parent = (int)hierarchyAccessor.GetProperty(item, (int)__VSHPROPID.VSHPROPID_Parent);
            if (parent == VSConstants.VSITEMID_ROOT || parent < 0)
            {
                string name = hierarchyAccessor.GetNodeName(item);
                if (name == "Stored Procedures" || name == "Functions")
                    return new string[] { "StoredProcedure" };
                else if (name.EndsWith("s"))
                    return new string[] { name.Substring(0, name.Length - 1) };
            }

            string nodeId = hierarchyAccessor.GetNodeId(item);
            if (nodeId == "Table")
                return new string[] { "Trigger" };
            return null;
        }

        /// <summary>
        /// Returns object type name for the hierarchy item.
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process.</param>
        /// <returns>Returns object type name for the hierarchy item.</returns>
        public string GetObjectType(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            return hierarchyAccessor.GetObjectType(item);
        }

        /// <summary>
        /// Creates new contextless node in the hierarchy.
        /// </summary>
        /// <returns>Identifier of the created item.</returns>
        public int CreateObjectNode()
        {
            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            return hierarchyAccessor.CreateObjectNode();
        }

        /// <summary>
        /// Removes a given item node from the hierarchy
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process</param>
        /// <returns>True if the given node was actually dropped</returns>
        public bool DropObjectNode(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");

            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            if (IsContextFullNode(item))
            {
                hierarchyAccessor.DropObjectNode(item);
                return true;
            }

            return false;
        }
        #endregion

        #region RDT related support
        /// <summary>
        /// Registers given document and editor objects in the RDT under given 
        /// hierarchy item.
        /// </summary>
        /// <param name="item">Hierarchy item identifier for this object.</param>
        /// <param name="document">Refernce to the document object.</param>
        /// <param name="view">Reference to the view object (editor).</param>
        public void RegisterEditor(int item, IDocument document, IEditor view)
        {
            // Validate inputs
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            if (document == null)
                throw new ArgumentNullException("document");
            if (view == null)
                throw new ArgumentNullException("view");

            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");
            Debug.Assert(shell != null, "Hierarchy accessor is not initialized!");

            // Prepare data for shell call
            string moniker = BuildMoniker(document.TypeName, document.ObjectID);
            Guid editorID = Guid.Empty;
            Guid commandGroupID = view.CommandGroupID;
            IntPtr dataPunk = Marshal.GetIUnknownForObject(document);
            IntPtr viewPunk = Marshal.GetIUnknownForObject(view);

            // Validate prepared data
            Debug.Assert(!String.IsNullOrEmpty(moniker), "Failed to build moniker string!");
            Debug.Assert(dataPunk != null, "Failed to marshal document object!");
            Debug.Assert(viewPunk != null, "Failed to marshal view object!");

            // Variable to store result
            IVsWindowFrame winFrame = null;

            // Initialize IDE editor infrastracture
            int result = shell.InitializeEditorInstance(
                (uint)0,                // Initialization flags. We need default behavior here
                viewPunk,               // View object reference (should implement IVsWindowPane)
                dataPunk,               // Docuemnt object reference (should implement IVsPersistDocData)
                moniker,                // Document moniker
                ref editorID,           // GUID of the editor type
                null,                   // Name of the physical view. We use default
                ref editorID,           // GUID identifying the logical view.
                String.Empty,           // Initial caption defined by the document owner. Will be initialized by the editor later
                String.Empty,           // Initial caption defined by the document editor. Will be initialized by the editor later
                Hierarchy,              // Pointer to the IVsUIHierarchy interface of the project that contains the document
                (uint)item,             // UI hierarchy item identifier of the document in the project system
                IntPtr.Zero,            // Pointer to the IUnknown interface of the document data object if the document data object already exists in the running document table
                // Project-specific service provider.
                hierarchyAccessor.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider,
                ref commandGroupID,     // Command UI GUID of the commands to display for this editor.
                out winFrame            // The window frame that contains the editor
                );

            // Validate result code and frame object
            Debug.Assert(winFrame != null && ErrorHandler.Succeeded(result), "Failed to initialize editor!");
            ErrorHandler.ThrowOnFailure(result);

            // Pass frame reference to the editor for caption control
            view.OwnerFrame = winFrame;

            // Display editor if initialized
            winFrame.Show();
        }

        /// <summary>
        /// Activates document editor, if already opens. Searches RDT by moniker and 
        /// ensures that hierarchy item identifier for founded document is correct. 
        /// After that activates editor by hierarchy item ID.
        /// </summary>
        /// <param name="item">Hierarchy item identifier for this object.</param>
        /// <param name="typeName">Name of the type of the Database Object.</param>
        /// <param name="objectID">Multipart identifier of the Database Object.</param>        
        /// <returns>Returns true if document editor was found and activated, returns false otherwise.</returns>
        public bool FindAndActivateEditor(int item, string typeName, object[] objectID)
        {
            // Validate inputs
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");
            if (objectID == null || objectID.Length < 2 || objectID[1] == null)
                throw new ArgumentException(Resources.Error_EmptyIdentifierSlot, "objectID");

            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            string path = BuildMoniker(typeName, objectID);

            // Check if document is already registerd (by moniker, not by hierarchy ID!)
            // and rename it to new hierarchy ID
            Rename(path, path, item);
            return hierarchyAccessor.ActivateDocumentIfOpen(path);

        }

        /// <summary>
        /// Used to rename documents in RDT or change their hierarchy item identifier.
        /// </summary>
        /// <param name="oldMoniker">Old moniker of the document in the RDT</param>
        /// <param name="newMoniker">New moniker for the document in the RDT</param>
        /// <param name="newItemID">New hierarchy item for the document in RDT</param>
        public void Rename(string oldMoniker, string newMoniker, int newItemID)
        {
            if (String.IsNullOrEmpty(oldMoniker))
                throw new ArgumentException(Resources.Error_EmptyString, "oldMoniker");
            if (String.IsNullOrEmpty(newMoniker))
                throw new ArgumentException(Resources.Error_EmptyString, "newMoniker");
            if (newItemID < 0)
                throw new ArgumentOutOfRangeException("newItemID");

            if (HasDocument(oldMoniker))
            {
                int res = rdt.RenameDocument(oldMoniker, newMoniker, new IntPtr(-1), (uint)newItemID);
                Debug.Assert(res == VSConstants.S_OK, "Failed to rename document!");
            }
        }

        /// <summary>
        /// Returns true if document for given object is already registered.
        /// </summary>
        /// <param name="typeName">Object type name.</param>
        /// <param name="objectID">Object identifier.</param>
        /// <returns>Returns true if document for given object is already registered.</returns>
        public bool HasDocument(string typeName, object[] objectID)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (objectID == null)
                throw new ArgumentNullException("objectID");

            return HasDocument(BuildMoniker(typeName, objectID));
        }

        /// <summary>
        /// Returns true if document for given object is already registered.
        /// </summary>
        /// <param name="typeName">Object type name.</param>
        /// <param name="objectID">Object identifier.</param>
        /// <param name="cookie">Document cookie to check.</param>
        /// <returns>Returns true if document for given object is already registered.</returns>
        public bool HasDistinctDocument(string typeName, object[] objectID, uint cookie)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (objectID == null)
                throw new ArgumentNullException("objectID");

            return HasDistinctDocument(BuildMoniker(typeName, objectID), cookie);
        }

        /// <summary>
        /// Returns true if document with given moniker is already registered.
        /// </summary>
        /// <param name="moniker">Moniker to check.</param>
        /// <returns>Returns true if document with given moniker is already registered.</returns>
        public bool HasDocument(string moniker)
        {
            if (String.IsNullOrEmpty(moniker))
                throw new ArgumentException(Resources.Error_EmptyString, "moniker");

            return HasDistinctDocument(moniker, 0);
        }

        /// <summary>
        /// Returns true if document with given moniker is already registered and differs from 
        /// given document cookie.
        /// </summary>
        /// <param name="moniker">Moniker to check.</param>
        /// <param name="cookie">Document cookie to check.</param>
        /// <returns>Returns true if document with given moniker is already registered.</returns>
        public bool HasDistinctDocument(string moniker, uint cookie)
        {
            if (String.IsNullOrEmpty(moniker))
                throw new ArgumentException(Resources.Error_EmptyString, "moniker");

            // Temporary unused variables
            IVsHierarchy hier; uint uid, uiCok = 0; IntPtr pUnk;

            int res = rdt.FindAndLockDocument(
                        (uint)_VSRDTFLAGS.RDT_NoLock,   // Should not lock the document
                        moniker,                        // Moniker string to search for document
                        out hier,                       // Recieves document hierarchy reference
                        out uid,                        // Recieves document hierarchy item identifier
                        out pUnk,                       // Recieves reference to the document itself
                        out uiCok);                     // Recieves value of the document cookie

            return res == VSConstants.S_OK && pUnk != null && cookie != uiCok;
        }

        /// <summary>
        /// Builds moniker for database object object in format: 
        /// "mysql-{type name}://{server name}.{schema name}. ...".
        /// </summary>
        /// <param name="typeName">Name of the type of the Database Object.</param>
        /// <param name="objectID">Multipart identifier of the Database Object.</param>        
        /// <returns>Moniker for table object in format: 
        /// "mysql-{type name}://{server name}.{schema name}. ...".
        /// </returns>
        public string BuildMoniker(string typeName, object[] objectID)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (objectID == null)
                throw new ArgumentNullException("objectID");

            // Write moniker prefix
            StringBuilder result = new StringBuilder();
            result.AppendFormat("mysql-{0}://", typeName.ToLowerInvariant());

            // Write server name
            result.Append(Connection.ServerName);

            // Write each availabel ID part
            for (int i = 1; i < objectID.Length; i++)
            {
                if (objectID[i] == null)
                    continue;
                result.Append('.');
                result.Append(objectID[i]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Closes document windows for given document.
        /// </summary>
        /// <param name="typeName">Name of the type of the Database Object.</param>
        /// <param name="objectID">Multipart identifier of the Database Object.</param>   
        public void CloseDocument(string typeName, object[] objectID)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (objectID == null)
                throw new ArgumentNullException("objectID");

            // Get document cookie
            uint cookie = GetDocumentCookie(BuildMoniker(typeName, objectID));

            // Close document
            int res = rdt2.CloseDocuments((uint)__FRAMECLOSE.FRAMECLOSE_NoSave, null, cookie);
            Debug.Assert(ErrorHandler.Succeeded(res), "Failed to close document!");
        }
        #endregion

        #region Server Explorer refreshing
        /// <summary>
        /// Refreshs server exporer window using its refresh command.
        /// </summary>
        public void Refresh()
        {
            // Select connection node to have all content refreshed. 
            SelectConnectionNode();

            // Predefined IDs (constans are available only in H files)             
            Guid seToolWindow = new Guid("{74946827-37A0-11D2-A273-00C04F8EF4FF}");
            Guid cmdGroup = new Guid("{74D21311-2AEE-11d1-8BFB-00A0C90F26F7}");

            // Get server explorer window
            IVsWindowFrame frame;
            uiShell.FindToolWindow(0, ref seToolWindow, out frame);
            if (frame == null)
                return;

            // Determine if server explorer window is currently visible
            bool isVisible = frame.IsVisible() == 0;

            // Display server explorer window, if not visible
            if (!isVisible)
                frame.ShowNoActivate();

            // Get OLE command target
            IOleCommandTarget target = frame as IOleCommandTarget;
            if (target == null)
                return;

            // Executes command for selected node (connection node should be selected - only 
            // in this case all content will be refreshed)
            int result = target.Exec(
                ref cmdGroup,
                0x03004,
                0,
                IntPtr.Zero,
                IntPtr.Zero);

            // Hide server explorer if should be hidden
            if (!isVisible)
                frame.Hide();

            Debug.Assert(ErrorHandler.Succeeded(result), "Error while executing refresh command for server explorer!");           
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Checks, if given node is contextfull (it was created via enumeration 
        /// and not via CreateObjectNode).
        /// </summary>
        /// <param name="item">The hierarchy item identifier to process.</param>
        /// <returns>
        /// Returns false if node was created via CreateObjectNode, returns true 
        /// otherwise.
        /// </returns>
        private bool IsContextFullNode(int item)
        {
            if (item < 0)
                throw new ArgumentException(Resources.Error_InvalidHierarchyItemID, "item");

            Debug.Assert(hierarchyAccessor != null, "Hierarchy accessor is not initialized!");

            object parent;
            int result = hierarchyAccessor.Hierarchy.GetProperty((uint)item, (int)__VSHPROPID.VSHPROPID_Parent, out parent);

            return ErrorHandler.Succeeded(result) && parent is int && (int)parent > 0;
        }

        /// <summary>
        /// Returns cookie for registered document by its moniker.
        /// </summary>
        /// <param name="moniker">Moniker to check.</param>
        /// <returns>Returns cookie for registered document by its moniker.</returns>
        private uint GetDocumentCookie(string moniker)
        {
            if (String.IsNullOrEmpty(moniker))
                throw new ArgumentException(Resources.Error_EmptyString, "moniker");

            // Temporary unused variables
            IVsHierarchy hier; uint uid, uiCok; IntPtr pUnk;

            int res = rdt.FindAndLockDocument(
                        (uint)_VSRDTFLAGS.RDT_NoLock,   // Should not lock the document
                        moniker,                        // Moniker string to search for document
                        out hier,                       // Recieves document hierarchy reference
                        out uid,                        // Recieves document hierarchy item identifier
                        out pUnk,                       // Recieves reference to the document itself
                        out uiCok);                     // Recieves value of the document cookie

            Debug.Assert(ErrorHandler.Succeeded(res), "Failed to find document cookie!");

            return uiCok;
        }

        /// <summary>
        /// Selects connection node in the Server Explorer window.
        /// </summary>
        private void SelectConnectionNode()
        {
            // Extracts connection mamanger global service
            DataExplorerConnectionManager manager = Package.GetGlobalService(typeof(DataExplorerConnectionManager)) 
                as DataExplorerConnectionManager;
            if (manager == null)
            {
                Debug.Fail("Failed to get connection manager!");
                return;
            }

            // Searches for connection using connection string for current connection
            DataExplorerConnection connection = manager.FindConnection(
                GuidList.ProviderGUID, Connection.EncryptedConnectionString, true);
            if (connection == null)
            {
                Debug.Fail("Failed to find proper connection node!");
                return;
            }

            // Select connection node
            manager.SelectConnection(connection);
        }
        #endregion

        #region Underlying objects
        /// <summary>
        /// Used to store underlying hierarchy accessor.
        /// </summary>
        private readonly DataViewHierarchyAccessor hierarchyAccessor;
        /// <summary>
        /// Used to locate Server Explorer tool window and execute command on them.
        /// </summary>
        private readonly IVsUIShell uiShell;
        /// <summary>
        /// Used to register editor instance.
        /// </summary>
        private readonly IVsUIShellOpenDocument shell;
        /// <summary>
        /// Running document table interface
        /// </summary>
        private readonly IVsRunningDocumentTable rdt;
        /// <summary>
        /// Extended running document table interface
        /// </summary>
        private readonly IVsRunningDocumentTable2 rdt2;
        /// <summary>
        /// Connection wrapper.
        /// </summary>
        private readonly DataConnectionWrapper connectionWrapper;
        #endregion          
    }
}