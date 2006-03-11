using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using System.Security.Policy;

namespace Vsip.MyVSTools
{
    internal abstract class ExplorerNode
    {
        private HierNode hierNode;
        private ExplorerNode parent;
        private string caption;
        private uint itemId;
        private bool isExpanded;
        private ExplorerNode nextSibling;
        private ExplorerNode firstChild;
        protected bool populated;

        public ExplorerNode(ExplorerNode parent, string caption)
        {
            this.parent = parent;
            this.caption = caption;
            isExpanded = false;
            populated = false;
        }

        public virtual string Caption
        {
            get { return caption; }
        }

        public ExplorerNode Parent
        {
            get { return parent; }
        }

        public uint ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; }
        }

        public ExplorerNode FirstChild
        {
            get { Populate();  return firstChild; }
            set { firstChild = value; }
        }

        public ExplorerNode NextSibling
        {
            get { return nextSibling; }
            set { nextSibling = value; }
        }

        public abstract uint IconIndex { get; }
        public abstract bool Expandable { get; }
        public abstract void Populate();

        public virtual uint MenuId
        {
            get { return 0; }
        }

        protected void AddChild(ExplorerNode node)
        {
            HierNode hierNode = GetHierNode();
            node.ItemId = hierNode.IndexNode(node);

            if (firstChild == null)
                firstChild = node;
            else
            {
                ExplorerNode nodeIter = firstChild;
                while (nodeIter.NextSibling != null)
                    nodeIter = nodeIter.NextSibling;
                nodeIter.NextSibling = node;
            }

            //children.Add(node);


/*            node.ItemId = (uint)childId++;
            if (lastChild != null)
                lastChild.NextSibling = node;
            node.NextSibling = null;
            lastChild = node;
            if (firstChild == null)
                firstChild = lastChild;*/
        }

        protected DbConnection GetOpenConnection()
        {
            ExplorerNode node = this;
            while (!(node is ServerNode))
                node = node.Parent;
            // we've found our server node, so we grab the connection object
            DbConnection conn = (node as ServerNode).Connection;

            // if it's closed, we try to open it
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error opening connection");
                    conn = null;
                }
            }
            return conn;
        }

        protected HierNode GetHierNode()
        {
            if (hierNode == null)
            {
                ExplorerNode node = this;
                while (!(node is HierNode))
                    node = node.parent;
                hierNode = (HierNode)node;
            }
            return hierNode;
        }

        protected void OpenEditor(BaseEditor editorObj)
        {
            IVsUIShellOpenDocument openDoc = (IVsUIShellOpenDocument)
                PackageSingleton.Package.GetMyService(typeof(SVsUIShellOpenDocument));


            IVsUIShell uiShell = (IVsUIShell)
                PackageSingleton.Package.GetMyService(typeof(SVsUIShell));

//            System.Diagnostics.Trace.WriteLine("------------------- launching editor");
            IVsWindowFrame winFrame;
            Guid editor = Guid.Empty;
            Guid cmdGui = Guid.Empty;

      //      Guid ed = GuidList.guidEditorFactory;
  //          Guid view = VSConstants.LOGVIEWID_Primary;
    //        uint rdtflags = (uint)__VSOSPEFLAGS.OSPE_RDTFLAGS_MASK | 
        //        (uint)_VSRDTFLAGS.RDT_NonCreatable | 
          //      (uint)_VSRDTFLAGS.RDT_VirtualDocument;
            //int result = openDoc.OpenSpecificEditor(0, //rdtflags, 
              //  "path", ref ed,
                //null, ref view, "caption", GetHierNode(), ItemId,
                //IntPtr.Zero, PackageSingleton.Package, out winFrame);*/

            IntPtr viewAndDataPunk = Marshal.GetIUnknownForObject(editorObj);
            int result = openDoc.InitializeEditorInstance(0, viewAndDataPunk, viewAndDataPunk,
                editorObj.Filename, ref editor, null, ref editor, editorObj.Filename,
                null, GetHierNode(), ItemId, IntPtr.Zero,
                PackageSingleton.Package, ref cmdGui, out winFrame);
            ErrorHandler.ThrowOnFailure(result);

            result = uiShell.CreateDocumentWindow(0, editorObj.Filename,
                GetHierNode(), this.ItemId, viewAndDataPunk, 
                viewAndDataPunk, ref editor, null, ref cmdGui, PackageSingleton.Package, 
                editorObj.Filename, null, null, out winFrame);

            if (winFrame != null)
                winFrame.Show();
            
/*            System.Diagnostics.Trace.WriteLine("starting editor on item = " + this.ItemId);
            Guid logicalView = VSConstants.LOGVIEWID_Primary;
            result = openDoc.OpenSpecificEditor((uint)__VSOSPEFLAGS.OSPE_OpenAsNewFile,
                ".cpp", ref editorGuid, null, ref logicalView,
                Caption, GetHierNode(), this.ItemId, IntPtr.Zero,
                PackageSingleton.Package, out winFrame);
            MessageBox.Show("result = " + result);*/
        }

        public virtual void DoCommand(int commandId)
        {
        }
    }
}
