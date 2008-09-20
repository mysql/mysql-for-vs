using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.Data.Common;
using System.Resources;
using System.Globalization;
using System.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using MySql.Data.VisualStudio.Editors;

namespace MySql.Data.VisualStudio
{
	abstract class BaseNode
	{
		protected Guid editorGuid;
		protected Guid commandGroupGuid;
        protected string name;

		public BaseNode(DataViewHierarchyAccessor hierarchyAccessor, int id)
		{
			HierarchyAccessor = hierarchyAccessor;
			ItemId = id;
            IsNew = ItemId == 0;

            // set the server and database from our active connection
            DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
            Server = conn.DataSource;
            Database = conn.Database;
            HierarchyAccessor.Connection.UnlockProviderObject();

            if (!IsNew)
                Name = hierarchyAccessor.GetNodeName(id);
            else
                ItemId = hierarchyAccessor.CreateObjectNode();

            NameIndex = 2;
            commandGroupGuid = VSConstants.GUID_TextEditorFactory;
			editorGuid = Guid.Empty;
		}

		#region Properties

        public string Name 
        {
            get
            {
                if (name == null)
                    Name = GenerateUniqueName();
                return name;
            }
            protected set { name = value; }
        }

        public bool IsNew { get; set; }
        public int ItemId { get; protected set; }
        public string NodeId { get; protected set; }
		public DataViewHierarchyAccessor HierarchyAccessor { get; set; }
        public string Server { get; private set; }
        public string Database { get; private set; }
        public virtual bool Dirty { get; protected set; }

        protected string Moniker 
        { 
            get { return String.Format("mysql://{0}/{1}/{2}", Server, Database, Name); }
        }

        public int NameIndex { get; protected set; }

        public virtual string SchemaCollection
        {
            get { return NodeId + "s"; }
        }

        public virtual string LocalizedTypeString
        {
            get
            {
                return Resources.ResourceManager.GetString("Type_" + NodeId);
            }
        }


    	#endregion

        #region Virtuals

        public virtual void ExecuteCommand(int command)
		{
			switch ((uint)command)
			{
				case PkgCmdIDList.cmdAlterTable:
				case PkgCmdIDList.cmdAlterTrigger:
				case PkgCmdIDList.cmdAlterProcedure:
				case PkgCmdIDList.cmdAlterView:
					Alter();
					break;

                case PkgCmdIDList.cmdDelete:
					Drop();
					break;

				case PkgCmdIDList.cmdCloneTable:
				case PkgCmdIDList.cmdCloneProcedure:
				case PkgCmdIDList.cmdCloneView:
					Clone();
					break;
			}
		}


        public virtual void Edit()
        {
            if (!HierarchyAccessor.ActivateDocumentIfOpen(Moniker))
                OpenEditor();
        }

        public virtual object GetEditor()
        {
            throw new NotImplementedException();
        }

        public virtual void Alter()
        {
            Edit();
        }

        public virtual string GetDropSQL()
        {
            return String.Format("DROP {0} `{1}`.`{2}`", NodeId, Database, Name);
        }

        private void Drop()
        {
            string typeString = LocalizedTypeString.ToLower(CultureInfo.CurrentCulture);

            DialogResult result = MessageBox.Show(String.Format(
                Resources.DropConfirmation, typeString, Name),
                String.Format(Resources.DropConfirmationCaption, typeString),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            string sql = GetDropSQL();
            try
            {
                ExecuteSQL(sql);

                // now we drop the node from the hierarchy
                HierarchyAccessor.DropObjectNode(ItemId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format(Resources.ErrorAttemptingToDrop,
                    LocalizedTypeString, Name, ex.Message), Resources.ErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public virtual void Clone() 
        { 
        }

        protected virtual string GenerateUniqueName()
        {
            DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
            string[] restrictions = new string[2] { null, Database };
            try
            {
                DataTable dt = conn.GetSchema(SchemaCollection, restrictions);

                int uniqueIndex = 1;
                string objectName = String.Empty;

                foreach (DataRow row in dt.Rows)
                {
                    objectName = String.Format("{0}{1}", NodeId, uniqueIndex).Replace(" ", "");
                    if (row[NameIndex].ToString().ToLowerInvariant() == objectName.ToLowerInvariant())
                        uniqueIndex++;
                }
                Name = String.Format("{0}{1}", NodeId, uniqueIndex).Replace(" ", "");
                return Name;
            }
            finally
            {
                HierarchyAccessor.Connection.UnlockProviderObject();
            }
        }

        #endregion

        #region Private Methods

		private void OpenEditor()
		{
			IVsUIShellOpenDocument shell =
				MySqlDataProviderPackage.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

			IVsWindowFrame winFrame = null;

			object editor = GetEditor();
            object coreEditor = (editor is TextBufferEditor) ?
                (editor as TextBufferEditor).CodeWindow : editor;

			IntPtr viewPunk = Marshal.GetIUnknownForObject(coreEditor);
			IntPtr dataPunk = Marshal.GetIUnknownForObject(this);
            Guid viewGuid = VSConstants.LOGVIEWID_TextView;

			// Initialize IDE editor infrastracture
			int result = shell.InitializeEditorInstance(
				(uint)0,                // Initialization flags. We need default behavior here
				viewPunk,               // View object reference (should implement IVsWindowPane)
				dataPunk,               // Docuemnt object reference (should implement IVsPersistDocData)
				Moniker,                // Document moniker
				ref editorGuid,         // GUID of the editor type
				null,                   // Name of the physical view. We use default
				ref viewGuid,           // GUID identifying the logical view.
				null,                   // Initial caption defined by the document owner. Will be initialized by the editor later
				null,                   // Initial caption defined by the document editor. Will be initialized by the editor later
				// Pointer to the IVsUIHierarchy interface of the project that contains the document
				HierarchyAccessor.Hierarchy,
				(uint)ItemId,           // UI hierarchy item identifier of the document in the project system
				IntPtr.Zero,            // Pointer to the IUnknown interface of the document data object if the document data object already exists in the running document table
				// Project-specific service provider.
				HierarchyAccessor.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider,
				ref commandGroupGuid,   // Command UI GUID of the commands to display for this editor.
				out winFrame            // The window frame that contains the editor
				);

			Debug.Assert(winFrame != null &&
						 ErrorHandler.Succeeded(result), "Failed to initialize editor");

            // if our editor is a text buffer then hook up our language service
            if (editor is TextBufferEditor)
            {
                // now we tell our text buffer what language service to use
                Guid langSvcGuid = typeof(MySqlLanguageService).GUID;
                (editor as TextBufferEditor).TextBuffer.SetLanguageServiceID(ref langSvcGuid);
            }

			winFrame.Show();
		}

        protected void SaveNode()
        {
            string nodePath = String.Format("/Connection/{0}s/{1}", NodeId, Name);
            HierarchyAccessor.SetNodePath(ItemId, nodePath);
        }

		protected void ExecuteSQL(string sql)
		{
			DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
			try
			{
				DbCommand cmd = conn.CreateCommand();
				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				HierarchyAccessor.Connection.UnlockProviderObject();
			}
		}

        public DataTable GetDataTable(string sql)
        {
            DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
            try
            {
                DbDataAdapter da = MySqlProviderObjectFactory.Factory.CreateDataAdapter();
                DbCommand cmd = MySqlProviderObjectFactory.Factory.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                HierarchyAccessor.Connection.UnlockProviderObject();
            }
        }

        #endregion



	}
}
