using System;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace MySql.Data.VisualStudio
{
	internal class MySqlDataViewCommandHandler : DataViewCommandHandler
	{
		/// <summary>
		/// This method supplies information about a command's status.
		/// </summary>
		public override OleCommandStatus GetCommandStatus(int[] itemIds, OleCommand command, OleCommandTextType textType, OleCommandStatus status)
		{
            Logger.WriteLine("MySqlDataViewCommandHandler::GetCommandStatus");
            if (command.GroupGuid == PkgCmdIDList.guidStandardCmdSet97)
            {
                switch (command.CommandId)
                {
                    case PkgCmdIDList.cmdidDelete:
                    case PkgCmdIDList.cmdidCopy:
                        status.Supported = true;
                        status.Visible = true;
                        status.Enabled = true;
                        break;
                    default:
                        base.GetCommandStatus(itemIds, command, textType, status);
                        break;
                }
            }
            else if (command.GroupGuid == PkgCmdIDList.guidDataCmdSet)
			{
				switch (command.CommandId)
				{
                    case PkgCmdIDList.cmdidNewQuery:
						status.Supported = true;
						status.Visible = true;
						status.Enabled = true;
						break;
					default:
						base.GetCommandStatus(itemIds, command, textType, status);
						break;
				}
			}
            else if (command.GroupGuid == PkgCmdIDList.guidVSPackageBasedProviderCmdSet)
			{
				switch (command.CommandId)
				{
                    case PkgCmdIDList.cmdidCreateTable:
                    case PkgCmdIDList.cmdidOpenTableDef:
                    case PkgCmdIDList.cmdidShowTableData:
                    case PkgCmdIDList.cmdidAddNewTrigger:
                    case PkgCmdIDList.cmdidOpenProcedure:
                    case PkgCmdIDList.cmdidAddNewProcedure:
                    case PkgCmdIDList.cmdidAddNewView:
                    case PkgCmdIDList.cmdidOpenViewDefinition:
                    case PkgCmdIDList.cmdidShowViewResults:
                    case PkgCmdIDList.cmdidAddNewFunction:
                        status.Supported = true;
						status.Visible = true;
						status.Enabled = true;
						break;
					default:
						base.GetCommandStatus(itemIds, command, textType, status);
						break;
				}
			}
			else
			{
				base.GetCommandStatus(itemIds, command, textType, status);
			}
			return status;
		}

		/// <summary>
		/// This method executes a specified command, potentially based
		/// on parameters passed in from the data view support XML.
		/// </summary>
		public override object ExecuteCommand(int itemId, OleCommand command, OleCommandExecutionOption executionOption, object arguments)
		{
			object returnValue = null;
            Logger.WriteLine("MySqlDataViewCommandHandler::ExecuteCommand");
            if (command.GroupGuid == PkgCmdIDList.guidDataCmdSet)
			{
				switch (command.CommandId)
				{
//                    case MySqlDataViewCommands.cmdidNewQuery:
//						NewQuery();
//						break;
					default:
						returnValue = base.ExecuteCommand(itemId, command, executionOption, arguments);
						break;
				}
			}
            else if (command.GroupGuid == PkgCmdIDList.guidVSPackageBasedProviderCmdSet)
			{
				switch (command.CommandId)
				{
                    case PkgCmdIDList.cmdidOpenProcedure:
                        OpenObject("PROCEDURE", (uint)itemId);
                        break;
                    case PkgCmdIDList.cmdidCreateTable:
						CreateTable();
						break;
                    case PkgCmdIDList.cmdidOpenTableDef:
						OpenObject("TABLE", (uint)itemId);
						break;
//                    case PkgCmdIDList.cmdidDropTable:
//						DropTable();
//						break;
					default:
						returnValue = base.ExecuteCommand(itemId, command, executionOption, arguments);
						break;
				}
			}
			else
			{
				returnValue = base.ExecuteCommand(itemId, command, executionOption, arguments);
			}
			return returnValue;
		}

		private void NewQuery()
		{
			// For this sample, just show a message box
			IUIService uiService = DataViewHierarchyAccessor.ServiceProvider.GetService(typeof(IUIService)) as IUIService;
			if (uiService != null)
			{
				uiService.ShowMessage("This demonstrates overriding the new query command.");
			}
		}

        private void OpenObject(string type, uint itemId)
        {
            string name = ResolveName(itemId, 2);
            name = String.Format("{0}:{1}", type, name);
            OpenEditor(itemId, name);
        }

		private void CreateTable()
		{
                uint newNode = (uint)DataViewHierarchyAccessor.CreateObjectNode();
                OpenEditor(newNode, "Untitled");
		}

        private void DropTable()
		{
			// For this sample, just show a message box
			IUIService uiService = DataViewHierarchyAccessor.ServiceProvider.GetService(typeof(IUIService)) as IUIService;
			if (uiService != null)
			{
				uiService.ShowMessage("The table should be dropped here.");
			}
		}

        private string ResolveName(uint itemId, int numLevels)
        {
            IVsUIHierarchy hier = DataViewHierarchyAccessor.Hierarchy;
            object baseName, rootName;

            // first get the base name of this object
            int result = hier.GetProperty(
                itemId, (int)__VSHPROPID.VSHPROPID_Name, out baseName);
            for (int i = 0; i < numLevels; i++)
            {
                object parentId;
                result = hier.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_Parent,
                    out parentId);
                itemId = (uint)(int)parentId;
            }
            result = hier.GetProperty(
                itemId, (int)__VSHPROPID.VSHPROPID_Name, out rootName);

            return String.Format("{0}.{1}", rootName, baseName);
        }

        private void OpenEditor(uint nodeId, string name)
        {
            IVsUIShellOpenDocument openDoc;
            IVsWindowFrame winFrame;

            openDoc = (IVsUIShellOpenDocument)
                this.DataViewHierarchyAccessor.ServiceProvider.GetService(
                typeof(SVsUIShellOpenDocument));

            Guid editorGuid = GuidList.guidEditorFactory;
            Guid logicalView = VSConstants.LOGVIEWID_Primary;
            Guid cmdUIGuid = Guid.Empty;

            Guid editorFactory = GuidList.guidEditorFactory;
            uint flags = (uint)(_VSRDTFLAGS.RDT_DontAddToMRU |
                _VSRDTFLAGS.RDT_NonCreatable | _VSRDTFLAGS.RDT_DontSaveAs);
            PackageSingleton.Package.EditorFactory.Connection =
                DataViewHierarchyAccessor.Connection;
            int result = openDoc.OpenSpecificEditor(flags, name,
                ref editorFactory, null, ref logicalView,
                "", DataViewHierarchyAccessor.Hierarchy,
                (uint)nodeId, IntPtr.Zero,
                PackageSingleton.Package,
                out winFrame);
            if (winFrame != null)
                winFrame.Show();
        }

	}
}
