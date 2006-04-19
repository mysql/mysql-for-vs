using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Resources;
using System.Reflection;
using System.Drawing;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace MySql.VSTools
{
    internal abstract class HierNode : ExplorerNode, 
        IVsUIHierarchy, 
        IVsPersistHierarchyItem2
    {
        private static ImageList imageList;
        private EventSinkCollection nodes;
        private ExplorerNode activeNode;
        private EventSinkCollection sinks;

        public HierNode(HierNode parent, string name) : base(parent, name)
        {
            nodes = new EventSinkCollection();
            sinks = new EventSinkCollection();

            if (imageList == null)
            {
                ResourceManager rm = new ResourceManager("MySql.VSTools.VSPackage",
                    Assembly.GetExecutingAssembly());

                imageList = new ImageList();
                imageList.ImageSize = new Size(16, 16);
                imageList.TransparentColor = Color.Transparent;
                imageList.Images.Add((Image)rm.GetObject("server-node"));
                imageList.Images.Add((Image)rm.GetObject("folder"));
                imageList.Images.Add((Image)rm.GetObject("table-node"));
                imageList.Images.Add((Image)rm.GetObject("database"));
                imageList.Images.Add((Image)rm.GetObject("procedure"));
                imageList.Images.Add((Image)rm.GetObject("function"));
                imageList.Images.Add((Image)rm.GetObject("view"));
                imageList.Images.Add((Image)rm.GetObject("trigger"));
                imageList.Images.Add((Image)rm.GetObject("column"));
            }

/*                    System.ComponentModel.ComponentResourceManager resources = 
                new System.ComponentModel.ComponentResourceManager(typeof(ExplorerControl));
            imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "MysqlServer.16x16.bmp");
            imageList.Images.SetKeyName(1, "folder.16x16.bmp");
            imageList.Images.SetKeyName(2, "database.16x16.bmp");
            imageList.Images.SetKeyName(3, "Table.16x16.bmp");
            imageList.Images.SetKeyName(4, "Column.16x16.bmp");
            imageList.Images.SetKeyName(5, "View.16x16.bmp");
            imageList.Images.SetKeyName(6, "Procedure.16x16.bmp");
            imageList.Images.SetKeyName(7, "Function.16x16.bmp");
            imageList.Images.SetKeyName(8, "db.Procedure.16x16.png");
            imageList.Images.SetKeyName(9, "db.Procedure.many_16x16.png");
            imageList.Images.SetKeyName(10, "db.Schema.16x16.png");
            imageList.Images.SetKeyName(11, "db.Table.16x16.png");
            imageList.Images.SetKeyName(12, "db.View.16x16.png");
            imageList.Images.SetKeyName(13, "db.View.many_16x16.png");*/
        }


        public uint IndexNode(ExplorerNode node)
        {
            return nodes.Add(node);
        }

        public void UnindexNode(ExplorerNode node)
        {
            nodes.Remove(node);
        }

        public ExplorerNode ActiveNode
        {
            get { return activeNode; }
        }

        public void RefreshItem(uint itemId)
        {
            IEnumerator sinkEnum = (sinks as IEnumerable).GetEnumerator();
            while (sinkEnum.MoveNext())
                (sinkEnum.Current as IVsHierarchyEvents).OnInvalidateItems(itemId);
        }

        public void ItemDeleted(uint itemId)
        {
            IEnumerator sinkEnum = (sinks as IEnumerable).GetEnumerator();
            while (sinkEnum.MoveNext())
                (sinkEnum.Current as IVsHierarchyEvents).OnItemDeleted(itemId);
        }

        public ExplorerNode NodeFromId(uint itemId)
        {
            if (itemId == VSConstants.VSITEMID_ROOT)
                return this;
            return (ExplorerNode)nodes[itemId];
        }

        private void ShowContextMenu(uint menuId, IntPtr pointData)
        {
            object vData = Marshal.GetObjectForNativeVariant(pointData);
            UInt32 point = (UInt32)vData;
            short x = (short)(point & 0x0000ffff);
            short y = (short)((point & 0xffff0000) >> 16);

            // guidMyPackageCommandSet:MyContextMenu is the GUID:ID pair
            // for the context menu.
            CommandID contextMenuID = new CommandID(GuidList.guidMyVSToolsCmdSet,
                (int)menuId);

            OleMenuCommandService menuService;
            menuService = PackageSingleton.Package.GetMyService(
                typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != menuService)
            {
                try
                {
                    menuService.ShowContextMenu(contextMenuID, x, y);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #region IVsUIHierarchy Members

        public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            pdwCookie = sinks.Add(pEventSink);
            return VSConstants.S_OK;
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        public int ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, 
            uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            //MessageBox.Show("guid = " + pguidCmdGroup.ToString() + ";cmdid=" + nCmdID);
            if (pguidCmdGroup == VsMenus.guidVsUIHierarchyWindowCmds)
            {
                if (nCmdID == (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_RightClick)
                {
                    activeNode = NodeFromId(itemid);
                    if (activeNode.MenuId != 0)
                        ShowContextMenu(activeNode.MenuId, pvaIn);
                }
//#define UIHWCMDID_DoubleClick 2 

//#define UIHWCMDID_EnterKey 3 

//#define UIHWCMDID_StartLabelEdit 4 

//#define UIHWCMDID_CommitLabelEdit 5 

//#define UIHWCMDID_CancelLabelEdit 6 
                return VSConstants.S_OK;
            }
            return VSConstants.DISP_E_MEMBERNOTFOUND;
        }

        public int GetCanonicalName(uint itemid, out string pbstrName)
        {
            pbstrName = "Dummy name";
            return VSConstants.S_OK;
        }

        public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            //2016
            //2054
            pguid = Guid.NewGuid();
            return VSConstants.E_FAIL;
        }

        public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, 
                                      out IntPtr ppHierarchyNested, 
                                      out uint pitemidNested)
        {
            ppHierarchyNested = IntPtr.Zero;
            pitemidNested = 0;
            // If itemid is not a nested hierarchy we must return E_FAIL.
            return VSConstants.E_FAIL;
        }

        public int GetProperty(uint itemid, int propid, out object result)
        {
            result = null;
            ExplorerNode node = NodeFromId(itemid);

            string propname = Enum.GetName(typeof(__VSHPROPID),
                (__VSHPROPID)propid);
            System.Diagnostics.Trace.WriteLine("prop = " + propname + "; itemid=" + itemid);

            __VSHPROPID propVal = (__VSHPROPID)propid;
            switch (propVal)
            {
                case __VSHPROPID.VSHPROPID_ParentHierarchy:
                    result = null;
                    return VSConstants.S_OK;

                case __VSHPROPID.VSHPROPID_Caption:
                case __VSHPROPID.VSHPROPID_Name:
                    result = node.Caption;
                    break;

                case __VSHPROPID.VSHPROPID_OpenFolderIconIndex:
                    result =  node.IconIndex;
                    break;
                case __VSHPROPID.VSHPROPID_Expandable:
                    result = node.Expandable;
                    break;
                case __VSHPROPID.VSHPROPID_ExpandByDefault:
                    result = false;
                    break;
                case __VSHPROPID.VSHPROPID_Expanded:
                    result = node.IsExpanded;
                    break;

                case __VSHPROPID.VSHPROPID_IconImgList:
                    result = imageList.Handle;
                    break;
                case __VSHPROPID.VSHPROPID_IconIndex:
                    result = node.IconIndex;
                    break;

      //          case __VSHPROPID.VSHPROPID_OverlayIconIndex:
        //            pvar = 1;
          //          break;
//                case __VSHPROPID.VSHPROPID_SelContainer:
  //                  break;
    //            case __VSHPROPID.VSHPROPID_BrowseObject:
      //              break;
                case __VSHPROPID.VSHPROPID_ItemDocCookie:
                    result = ItemId;
                    break;
                    //2072
                    //2084

                case __VSHPROPID.VSHPROPID_NextSibling:
                    result = node.NextSibling != null ? node.NextSibling.ItemId : 
                        VSConstants.VSITEMID_NIL;
                    break;
                    
                case __VSHPROPID.VSHPROPID_FirstChild:
                    result = node.FirstChild != null ? (uint)node.FirstChild.ItemId : 
                        VSConstants.VSITEMID_NIL;
                    break;
                    
                case __VSHPROPID.VSHPROPID_Parent:
                    result = (IntPtr)(Parent != null ? Parent.ItemId : VSConstants.VSITEMID_NIL);
                    break;

                case __VSHPROPID.VSHPROPID_Root:
                    result = Marshal.GetIUnknownForObject(this);
                    break;


  //              default:
             //       MessageBox.Show("Unhandled property: " +
               //         Enum.GetName(typeof(__VSHPROPID), propVal));
//    				return VSConstants.DISP_E_MEMBERNOTFOUND;
            }
            if (result == null)
            {
                //MessageBox.Show("prop not found: " + propname);
                return VSConstants.DISP_E_MEMBERNOTFOUND;
            }
            return VSConstants.S_OK;
        }

        public int GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        {
            ppSP = PackageSingleton.Package.GetMyService(typeof(
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as
                Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            return VSConstants.S_OK;
        }

        public int ParseCanonicalName(string pszName, out uint pitemid)
        {
            pitemid = 1;
            return VSConstants.S_OK;
        }

        public int QueryClose(out int pfCanClose)
        {
            pfCanClose = 1;
            return VSConstants.S_OK;
        }

        public int QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, Microsoft.VisualStudio.OLE.Interop.OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return VSConstants.S_OK;
        }

        public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            return VSConstants.S_OK;
        }

        public int SetProperty(uint itemid, int propid, object value)
        {
            __VSHPROPID id = (__VSHPROPID)propid;
            ExplorerNode node = NodeFromId(itemid);

            switch (id)
            {
                case __VSHPROPID.VSHPROPID_Expanded:
                    node.IsExpanded = (bool)value;
                    break;

                case __VSHPROPID.VSHPROPID_EditLabel:
                    //return SetEditLabel((string)value);
                    string s = (string)value;
                    break;

                default:
                    MessageBox.Show("SetProperty for " + propid + " not handled");
                    break;
            }
            return VSConstants.S_OK;
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int UnadviseHierarchyEvents(uint dwCookie)
        {
            sinks.RemoveAt(dwCookie);
            return VSConstants.S_OK;
        }

        public int Unused0()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused1()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused2()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused3()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused4()
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsPersistHierarchyItem2 Members

        public int IgnoreItemFileChanges(uint itemid, int fIgnore)
        {
            return VSConstants.S_OK;
        }

        public int IsItemDirty(uint itemid, IntPtr punkDocData, out int pfDirty)
        {
            IVsPersistDocData docData = (IVsPersistDocData)
                Marshal.GetObjectForIUnknown(punkDocData);
            return ErrorHandler.ThrowOnFailure(docData.IsDocDataDirty(out pfDirty));
        }

        public int IsItemReloadable(uint itemid, out int pfReloadable)
        {
            pfReloadable = 0;
            return VSConstants.S_OK;
        }

        public int ReloadItem(uint itemid, uint dwReserved)
        {
            return VSConstants.S_OK;
        }

        public int SaveItem(VSSAVEFLAGS dwSave, string pszSilentSaveAsName, uint itemid, IntPtr punkDocData, out int pfCanceled)
        {
            pfCanceled = 0;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
