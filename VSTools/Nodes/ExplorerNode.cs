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
using System.Collections;

namespace MySql.VSTools
{
    internal abstract class ExplorerNode
    {
        private HierNode hierNode;
        private ExplorerNode parent;
        protected string caption;
        private uint itemId;
        private bool isExpanded;
        private ExplorerNode nextSibling;
        private ExplorerNode firstChild;
        protected bool populated;
        protected BaseEditor activeEditor;
        protected bool isNew;
        protected ArrayList newNodes;

        public ExplorerNode(ExplorerNode parent, string caption)
        {
            this.parent = parent;
            this.caption = caption;
            isExpanded = false;
            populated = false;
            newNodes = new ArrayList();
        }

        public virtual string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public virtual string Schema
        {
            get { return GetDatabaseNode().Caption; }
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

        protected virtual string GetDeleteSql()
        {
            return String.Empty;
        }

        internal virtual BaseEditor GetEditor()
        {
            return null;
        }

        public virtual void Populate()
        {
        }

        public virtual bool Save()
        {
            return false;
        }

        public virtual uint MenuId
        {
            get { return 0; }
        }

        protected void IndexChild(ExplorerNode node)
        {
            HierNode hierNode = GetHierNode();
            node.ItemId = hierNode.IndexNode(node);
        }

        protected void LinkChild(ExplorerNode node)
        {
            ExplorerNode nodeIter = null;
            if (firstChild == null)
                firstChild = node;
            else
            {
                nodeIter = firstChild;
                while (nodeIter.NextSibling != null)
                    nodeIter = nodeIter.NextSibling;
                nodeIter.NextSibling = node;
            }
            // notify our hierarchy node that we have linked in an item
            GetHierNode().ItemAdded(ItemId, 
                nodeIter == null ? VSConstants.VSITEMID_NIL : nodeIter.ItemId, 
                node.ItemId);
        }

        public void AddChild(ExplorerNode node)
        {
            for (int i=0; i < newNodes.Count; i++)
            {
                if (newNodes[i] != node) continue;
                newNodes.RemoveAt(i);
                break;
            }
            IndexChild(node);
            LinkChild(node);
        }

        public void RemoveChild(ExplorerNode node)
        {
            HierNode hierNode = GetHierNode();
            // first remove it from the item id index
            hierNode.UnindexNode(node);

            // now we unlink it
            ExplorerNode prevNode = null;
            ExplorerNode nodeIter = firstChild;
            while (nodeIter != node)
            {
                prevNode = nodeIter;
                nodeIter = nodeIter.NextSibling;
            }
            if (prevNode == null)
                firstChild = nodeIter.NextSibling;
            else
                prevNode.NextSibling = nodeIter.NextSibling;

            hierNode.ItemDeleted(itemId);
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

        protected void Delete()
        {
            // first make sure the user is sure
            if (MessageBox.Show(
                String.Format(MyVSTools.GetResourceString("DeleteConfirm"),
                Caption),
                MyVSTools.GetResourceString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.No)
                return;

            string sql = GetDeleteSql();
            try
            {
                ExecuteNonQuery(sql);
                //delete was successful, remove this node
                Parent.RemoveChild(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    String.Format(MyVSTools.GetResourceString("UnableToDeleteTitle"),
                    Caption), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected DatabaseNode GetDatabaseNode()
        {
            ExplorerNode node = this;
            while (node != null && !(node is DatabaseNode))
            {
                node = node.Parent;
            }
            return (node as DatabaseNode);
        }


        public void OpenEditor()
        {
            IVsUIShellOpenDocument openDoc = (IVsUIShellOpenDocument)
                PackageSingleton.Package.GetMyService(typeof(SVsUIShellOpenDocument));


            IVsUIShell uiShell = (IVsUIShell)
                PackageSingleton.Package.GetMyService(typeof(SVsUIShell));

//            System.Diagnostics.Trace.WriteLine("------------------- launching editor");
            IVsWindowFrame winFrame;
            Guid editor = Guid.Empty;
            Guid cmdGui = Guid.Empty;

            Guid ed = GuidList.guidEditorFactory;
  //          Guid view = VSConstants.LOGVIEWID_Primary;
    //        uint rdtflags = (uint)__VSOSPEFLAGS.OSPE_RDTFLAGS_MASK | 
        //        (uint)_VSRDTFLAGS.RDT_NonCreatable | 
          //      (uint)_VSRDTFLAGS.RDT_VirtualDocument;
            //int result = openDoc.OpenSpecificEditor(0, //rdtflags, 
              //  "path", ref ed,
                //null, ref view, "caption", GetHierNode(), ItemId,
                //IntPtr.Zero, PackageSingleton.Package, out winFrame);*/

/*            IntPtr viewAndDataPunk = Marshal.GetIUnknownForObject(editorObj);
            int result = openDoc.InitializeEditorInstance(0,
                viewAndDactaPunk, viewAndDataPunk,
                editorObj.Filename, ref editor, null, ref editor, editorObj.Filename,
                null, GetHierNode(), ItemId, IntPtr.Zero,
                PackageSingleton.Package, ref cmdGui, out winFrame);
            ErrorHandler.ThrowOnFailure(result);

            result = uiShell.CreateDocumentWindow(0, editorObj.Filename,
                GetHierNode(), ItemId, viewAndDataPunk, 
                viewAndDataPunk, ref editor, null, ref cmdGui, PackageSingleton.Package, 
                editorObj.Filename, null, null, out winFrame);
            */
            DebugTrace.Trace("starting editor on item = " + this.ItemId);

            string filename = GetDatabaseNode().Caption + "." + Caption;
            //editor = GuidList.guidProcedureEditor;
               Guid logicalView = VSConstants.LOGVIEWID_Primary;
                int result = openDoc.OpenSpecificEditor(0,
                    filename, ref ed, null, ref logicalView,
                    Caption, GetHierNode(), ItemId, IntPtr.Zero,
                    PackageSingleton.Package, out winFrame);
     
                if (winFrame != null)
                    winFrame.Show();
        }

        protected string GetDefaultName(string baseName)
        {
            // first determine a default name
            int num = 1;
            string name = String.Format("{0}{1}", baseName, num).ToLower(CultureInfo.InvariantCulture);
            ExplorerNode node = FirstChild;
            int i = 0;
            while (node != null)
            {
                if (node.Caption.ToLower(CultureInfo.InvariantCulture) == name)
                    name = String.Format("{0}{1}", baseName, ++num).ToLower(CultureInfo.InvariantCulture);
                node = node.NextSibling;
                if (node == null && i < newNodes.Count)
                    node = (ExplorerNode)newNodes[i++];
            }
            return name;
        }

        protected void ExecuteNonQuery(string sql)
        {
            DbConnection connection = GetOpenConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual void DoCommand(int commandId)
        {
            switch (commandId)
            {
                case PkgCmdIDList.cmdidRefresh:
                    Refresh();
                    break;
                case PkgCmdIDList.cmdidDelete:
                    Delete();
                    break;
            }
        }

        protected virtual void Refresh()
        {
            populated = false;
            while (firstChild != null)
                RemoveChild(firstChild);
            Populate();
        }

        public void CloseEditor()
        {
            activeEditor = null;
        }
    }
}
