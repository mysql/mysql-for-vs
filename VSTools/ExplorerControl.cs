using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Vsip.MyVSTools
{
    internal enum NodeImageIndex
    {
        Server = 0,
        Tables = 1,
        Views = 1,
        Procedures = 1,
        Functions = 1,
        Triggers = 1,
        UDFS = 1,
        Database = 2,
        Table = 3,
        Column = 4,
        View = 5,
        Procedure = 6,
        Function = 7
    }

    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public class ExplorerControl : System.Windows.Forms.UserControl
    {
        private TreeView serverTree;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addServerMenuItem;
        private ImageList treeImages;
        private ContextMenuStrip serverMenu;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem modifyConnectionToolStripMenuItem;
        private ToolStripMenuItem closeConnectionToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private IContainer components;
        private ToolWindowPane container;
        //private MySqlTreeNode nodeCausingCommand;

        public ExplorerControl(ToolWindowPane container) : base()
        {
            this.container = container;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

/*        internal MySqlTreeNode NodeCausingCommand
        {
            get { return nodeCausingCommand; }
        }
        */
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        protected override bool ProcessDialogChar(char charCode)
        {
              // If we're the top-level form or control, we need to do the mnemonic handling
              if (charCode != ' ' && ProcessMnemonic(charCode))
              {
                    return true;
              }
              return base.ProcessDialogChar(charCode);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplorerControl));
            this.serverTree = new System.Windows.Forms.TreeView();
            this.treeImages = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addServerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.modifyConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.serverMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverTree
            // 
            this.serverTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverTree.ImageIndex = 0;
            this.serverTree.ImageList = this.treeImages;
            this.serverTree.Location = new System.Drawing.Point(0, 0);
            this.serverTree.Name = "serverTree";
            this.serverTree.SelectedImageIndex = 0;
            this.serverTree.Size = new System.Drawing.Size(150, 150);
            this.serverTree.TabIndex = 0;
            this.serverTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.serverTree_BeforeExpand);
            this.serverTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.serverTree_NodeMouseClick);
            // 
            // treeImages
            // 
            this.treeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
            this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImages.Images.SetKeyName(0, "MysqlServer.16x16.bmp");
            this.treeImages.Images.SetKeyName(1, "folder.16x16.bmp");
            this.treeImages.Images.SetKeyName(2, "database.16x16.bmp");
            this.treeImages.Images.SetKeyName(3, "Table.16x16.bmp");
            this.treeImages.Images.SetKeyName(4, "Column.16x16.bmp");
            this.treeImages.Images.SetKeyName(5, "View.16x16.bmp");
            this.treeImages.Images.SetKeyName(6, "Procedure.16x16.bmp");
            this.treeImages.Images.SetKeyName(7, "Function.16x16.bmp");
            this.treeImages.Images.SetKeyName(8, "db.Procedure.16x16.png");
            this.treeImages.Images.SetKeyName(9, "db.Procedure.many_16x16.png");
            this.treeImages.Images.SetKeyName(10, "db.Schema.16x16.png");
            this.treeImages.Images.SetKeyName(11, "db.Table.16x16.png");
            this.treeImages.Images.SetKeyName(12, "db.View.16x16.png");
            this.treeImages.Images.SetKeyName(13, "db.View.many_16x16.png");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addServerMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
            // 
            // addServerMenuItem
            // 
            this.addServerMenuItem.Name = "addServerMenuItem";
            this.addServerMenuItem.Size = new System.Drawing.Size(140, 22);
            this.addServerMenuItem.Text = "&Add Server...";
            // 
            // serverMenu
            // 
            this.serverMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.modifyConnectionToolStripMenuItem,
            this.closeConnectionToolStripMenuItem,
            this.toolStripMenuItem2,
            this.renameToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.serverMenu.Name = "serverMenu";
            this.serverMenu.Size = new System.Drawing.Size(176, 126);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(172, 6);
            // 
            // modifyConnectionToolStripMenuItem
            // 
            this.modifyConnectionToolStripMenuItem.Name = "modifyConnectionToolStripMenuItem";
            this.modifyConnectionToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.modifyConnectionToolStripMenuItem.Text = "Modify Connection...";
            // 
            // closeConnectionToolStripMenuItem
            // 
            this.closeConnectionToolStripMenuItem.Name = "closeConnectionToolStripMenuItem";
            this.closeConnectionToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.closeConnectionToolStripMenuItem.Text = "Close Connection";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(172, 6);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.renameToolStripMenuItem.Text = "Re&name";
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.propertiesToolStripMenuItem.Text = "P&roperties";
            // 
            // ExplorerControl
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.serverTree);
            this.Name = "ExplorerControl";
            this.contextMenuStrip1.ResumeLayout(false);
            this.serverMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void button1_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show(this,
                            string.Format("We are inside {0}.button1_Click()", this.ToString()),
                            "MyExplorerWindow");
        }

        internal void AddServer(string name, string connectString)
        {
            serverTree.Nodes.Add(new ServerNode(name, connectString));
        }

        internal void SaveServers()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                System.IO.Path.DirectorySeparatorChar +
                "MySQL" + System.IO.Path.DirectorySeparatorChar;
            string newpath = basePath + "MyVSTools.new";
            string oldpath = basePath + "MyVSTools.conf";

            // Write out a new config file
            System.IO.StreamWriter writer = new System.IO.StreamWriter(newpath);
            foreach (ServerTreeNode node in serverTree.Nodes)
                writer.WriteLine(String.Format("{0}:{1}", node.Text, 
                    node.Connection.ConnectionString));
            writer.Flush();
            writer.Close();

            System.IO.File.Copy(newpath, oldpath, true);
            System.IO.File.Delete(newpath);
        }

/*        private void CreateDatabaseNode(TreeNode parent, string name)
        {
            // if the parent node already has a node for this data
            // we remove it and readd it
            TreeNode[] nodes = parent.Nodes.Find(name, true);
            if (nodes.Length != 0)
                nodes[0].Remove();

            TreeNode node = parent.Nodes.Add(name, name,
                (int)ImageIndex.Database, (int)ImageIndex.Database);

            string tablesName = MyVSTools.GetResourceString("TablesNodeName");
            TreeNode tablesNode = node.Nodes.Add(tablesName, tablesName, 
                (int)ImageIndex.Tables, (int)ImageIndex.Tables);
            tablesNode.Nodes.Add("dummy", "dummy");
        }
        */


        private void serverTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            MySqlTreeNode node = (e.Node as MySqlTreeNode);
            e.Cancel = !node.Populate();
        }

        private void serverTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            MySqlTreeNode mysqlNode = (e.Node as MySqlTreeNode);
            nodeCausingCommand = mysqlNode;

            // guidMyPackageCommandSet:MyContextMenu is the GUID:ID pair
            // for the context menu.
            CommandID contextMenuID = new CommandID(GuidList.guidMyVSToolsCmdSet,
                (int)mysqlNode.MenuId);

            OleMenuCommandService menuService;
            menuService = (container.Package as MyVSTools).GetMyService(
                typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != menuService)
            {
                try
                {
                    Point pt = PointToScreen(e.Location);
                     // Note: point must be in screen coordinates!
                     menuService.ShowContextMenu(contextMenuID, pt.X, pt.Y);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }
    }
}
