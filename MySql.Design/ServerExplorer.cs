// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace MySql.Design
{
	/// <summary>
	/// Summary description for ServerExplorer.
	/// </summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class ServerExplorer : System.Windows.Forms.UserControl
	{
		#region Fields
		private System.Windows.Forms.TreeView serverTree;
		private System.Windows.Forms.ImageList treeImages;
		private System.Windows.Forms.MenuItem dbMenuConnect;
		private System.Windows.Forms.MenuItem dbMenuCreate;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu dbMenu;
		private System.Windows.Forms.MenuItem dbMenuGenScript;
		private System.Windows.Forms.ContextMenu serversMenu;
		private System.Windows.Forms.MenuItem serversMenuRefresh;
		private System.Windows.Forms.MenuItem serversMenuAdd;
		private System.Windows.Forms.ContextMenu tablesMenu;
		private System.Windows.Forms.MenuItem tablesMenuNewTable;
		private System.Windows.Forms.MenuItem tablesMenuRefresh;
		private System.Windows.Forms.ContextMenu tableMenu;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem queryTableMenuItem;
		private System.Windows.Forms.ContextMenu serverMenu;
		private System.Windows.Forms.MenuItem dbMenuExportData;
		private System.Windows.Forms.MenuItem dbMenuDelete;
		private System.Windows.Forms.MenuItem tableMenuDelete;
		private System.Windows.Forms.MenuItem serverMenuDelete;
		private ServerCollection	serverCollection;
		#endregion
		private System.Windows.Forms.ContextMenu sprocsMenu;
		private System.Windows.Forms.ContextMenu sprocMenu;
		private System.Windows.Forms.MenuItem sprocsNewProcMenu;
		private System.Windows.Forms.MenuItem sprocsRefresh;
		private System.Windows.Forms.MenuItem sprocEdit;
		private System.Windows.Forms.MenuItem sprocNew;
		private System.Windows.Forms.MenuItem sprocRefresh;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem sprocCopy;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem sprocDelete;
		private System.Windows.Forms.MenuItem sprocGenerate;

		internal event NodeDoubleClickDelegate NodeDoubleClick;
		private Connect connectClass;


		public ServerExplorer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			serverCollection = ServerCollection.Load();
			FillServerTree();

			serverTree.Nodes[0].Tag = serversMenu;
		}

		public Connect ConnectClass 
		{
			get { return connectClass; }
			set { connectClass = value; }
		}


		private void FillServerTree() 
		{
			serverTree.Nodes[0].Nodes.Clear();
			foreach ( ServerConfig sc in serverCollection.Servers) 
				AddServer(sc);
		}

		private void PopulateServerNode(TreeNode node) 
		{
			ServerConfig sc = (ServerConfig)node.Tag;
			string connString = sc.GetConnectString(true);
			MySqlConnection conn = new MySqlConnection(connString);
			try 
			{
				conn.Open();

				PopulateDatabases(conn, node);
				PopulateUDF(conn, node);
//				PopulateUsers(conn, node);

				conn.Close();
			}
			catch (Exception ex) 
			{
				MessageBox.Show("Server population failed: " + ex.Message);
			}
			finally 
			{
				if (conn != null) conn.Close();
			}
		}

		private void PopulateUDF(MySqlConnection conn, TreeNode node) 
		{
			TreeNode udfNode = node.Nodes.Add("User-defined Functions");

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM mysql.func", conn);
			using (MySqlDataReader reader = cmd.ExecuteReader()) 
			{
				while (reader.Read()) 
				{
					udfNode.Nodes.Add(reader.GetString(0));
				}
			}
		}

		private void AddServer( ServerConfig sc )
		{
			TreeNode node = serverTree.Nodes[0].Nodes.Add( sc.name );
			node.ImageIndex = 2;
			node.SelectedImageIndex = 2;
			node.Tag = sc;

			PopulateServerNode(node);

   			// if we have a database selected, then we go ahead and add that node
/*			if (sc.database != null && sc.database != String.Empty)
			{
				TreeNode dbNode = node.Nodes.Add( sc.database );
				dbNode.ImageIndex = 4;
				dbNode.SelectedImageIndex = 4;
				dbNode.Tag = NodeType.Database;
				node = dbNode;
			}

			// we add a dummy node so we get the turndown.
			// we populate when the user opens us up
			node.Nodes.Add( "dummy" ); */
			node.Collapse();
		}


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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ServerExplorer));
			this.serverTree = new System.Windows.Forms.TreeView();
			this.serverMenu = new System.Windows.Forms.ContextMenu();
			this.dbMenuConnect = new System.Windows.Forms.MenuItem();
			this.dbMenuCreate = new System.Windows.Forms.MenuItem();
			this.serverMenuDelete = new System.Windows.Forms.MenuItem();
			this.treeImages = new System.Windows.Forms.ImageList(this.components);
			this.serversMenu = new System.Windows.Forms.ContextMenu();
			this.serversMenuRefresh = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.serversMenuAdd = new System.Windows.Forms.MenuItem();
			this.dbMenu = new System.Windows.Forms.ContextMenu();
			this.dbMenuExportData = new System.Windows.Forms.MenuItem();
			this.dbMenuGenScript = new System.Windows.Forms.MenuItem();
			this.dbMenuDelete = new System.Windows.Forms.MenuItem();
			this.tablesMenu = new System.Windows.Forms.ContextMenu();
			this.tablesMenuNewTable = new System.Windows.Forms.MenuItem();
			this.tablesMenuRefresh = new System.Windows.Forms.MenuItem();
			this.tableMenu = new System.Windows.Forms.ContextMenu();
			this.queryTableMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.tableMenuDelete = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.sprocsMenu = new System.Windows.Forms.ContextMenu();
			this.sprocsNewProcMenu = new System.Windows.Forms.MenuItem();
			this.sprocsRefresh = new System.Windows.Forms.MenuItem();
			this.sprocMenu = new System.Windows.Forms.ContextMenu();
			this.sprocEdit = new System.Windows.Forms.MenuItem();
			this.sprocNew = new System.Windows.Forms.MenuItem();
			this.sprocRefresh = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.sprocCopy = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.sprocDelete = new System.Windows.Forms.MenuItem();
			this.sprocGenerate = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// serverTree
			// 
			this.serverTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.serverTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.serverTree.ContextMenu = this.serverMenu;
			this.serverTree.HideSelection = false;
			this.serverTree.ImageList = this.treeImages;
			this.serverTree.LabelEdit = true;
			this.serverTree.Location = new System.Drawing.Point(2, 34);
			this.serverTree.Name = "serverTree";
			this.serverTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
																				   new System.Windows.Forms.TreeNode("MySQL Servers", 1, 1)});
			this.serverTree.Size = new System.Drawing.Size(220, 304);
			this.serverTree.TabIndex = 0;
			this.serverTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.serverTree_MouseDown);
			this.serverTree.DoubleClick += new System.EventHandler(this.serverTree_DoubleClick);
			this.serverTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.serverTree_BeforeExpand);
			// 
			// serverMenu
			// 
			this.serverMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.dbMenuConnect,
																					   this.dbMenuCreate,
																					   this.serverMenuDelete});
			// 
			// dbMenuConnect
			// 
			this.dbMenuConnect.Index = 0;
			this.dbMenuConnect.Text = "&Edit...";
			// 
			// dbMenuCreate
			// 
			this.dbMenuCreate.Index = 1;
			this.dbMenuCreate.Text = "&Create New Database...";
			this.dbMenuCreate.Click += new System.EventHandler(this.dbMenuCreate_Click);
			// 
			// serverMenuDelete
			// 
			this.serverMenuDelete.Index = 2;
			this.serverMenuDelete.Text = "&Delete";
			this.serverMenuDelete.Click += new System.EventHandler(this.serverMenuDelete_Click);
			// 
			// treeImages
			// 
			this.treeImages.ImageSize = new System.Drawing.Size(16, 16);
			this.treeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImages.ImageStream")));
			this.treeImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// serversMenu
			// 
			this.serversMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.serversMenuRefresh,
																						this.menuItem2,
																						this.serversMenuAdd});
			// 
			// serversMenuRefresh
			// 
			this.serversMenuRefresh.Index = 0;
			this.serversMenuRefresh.Text = "Re&fresh";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// serversMenuAdd
			// 
			this.serversMenuAdd.Index = 2;
			this.serversMenuAdd.Text = "&Add Server...";
			this.serversMenuAdd.Click += new System.EventHandler(this.serversMenuAdd_Click);
			// 
			// dbMenu
			// 
			this.dbMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.dbMenuExportData,
																				   this.dbMenuGenScript,
																				   this.dbMenuDelete});
			// 
			// dbMenuExportData
			// 
			this.dbMenuExportData.Index = 0;
			this.dbMenuExportData.Text = "&Export Data...";
			// 
			// dbMenuGenScript
			// 
			this.dbMenuGenScript.Index = 1;
			this.dbMenuGenScript.Text = "&Generate Create Script...";
			// 
			// dbMenuDelete
			// 
			this.dbMenuDelete.Index = 2;
			this.dbMenuDelete.Text = "&Drop Database";
			this.dbMenuDelete.Click += new System.EventHandler(this.dbMenuDelete_Click);
			// 
			// tablesMenu
			// 
			this.tablesMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.tablesMenuNewTable,
																					   this.tablesMenuRefresh});
			// 
			// tablesMenuNewTable
			// 
			this.tablesMenuNewTable.Index = 0;
			this.tablesMenuNewTable.Text = "New &Table";
			this.tablesMenuNewTable.Click += new System.EventHandler(this.tablesMenuNewTable_Click);
			// 
			// tablesMenuRefresh
			// 
			this.tablesMenuRefresh.Index = 1;
			this.tablesMenuRefresh.Text = "Re&fresh";
			// 
			// tableMenu
			// 
			this.tableMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.queryTableMenuItem,
																					  this.menuItem8,
																					  this.menuItem9,
																					  this.tableMenuDelete,
																					  this.menuItem12,
																					  this.menuItem13,
																					  this.menuItem14});
			// 
			// queryTableMenuItem
			// 
			this.queryTableMenuItem.Index = 0;
			this.queryTableMenuItem.Text = "Retrie&ve Data From Table";
			this.queryTableMenuItem.Click += new System.EventHandler(this.queryTableMenuItem_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 1;
			this.menuItem8.Text = "New &Table";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 2;
			this.menuItem9.Text = "-";
			// 
			// tableMenuDelete
			// 
			this.tableMenuDelete.Index = 3;
			this.tableMenuDelete.Text = "&Drop Table";
			this.tableMenuDelete.Click += new System.EventHandler(this.tableMenuDelete_Click);
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 4;
			this.menuItem12.Text = "-";
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 5;
			this.menuItem13.Text = "&Generate Create Script...";
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 6;
			this.menuItem14.Text = "E&xport Data...";
			// 
			// sprocsMenu
			// 
			this.sprocsMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.sprocsNewProcMenu,
																					   this.sprocsRefresh});
			// 
			// sprocsNewProcMenu
			// 
			this.sprocsNewProcMenu.Index = 0;
			this.sprocsNewProcMenu.Text = "New Stored &Procedure";
			// 
			// sprocsRefresh
			// 
			this.sprocsRefresh.Index = 1;
			this.sprocsRefresh.Text = "Refres&h";
			// 
			// sprocMenu
			// 
			this.sprocMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.sprocEdit,
																					  this.sprocNew,
																					  this.sprocRefresh,
																					  this.menuItem5,
																					  this.sprocCopy,
																					  this.menuItem7,
																					  this.sprocDelete,
																					  this.sprocGenerate});
			// 
			// sprocEdit
			// 
			this.sprocEdit.Index = 0;
			this.sprocEdit.Text = "&Edit Stored Procedure";
			this.sprocEdit.Click += new System.EventHandler(this.sprocEdit_Click);
			// 
			// sprocNew
			// 
			this.sprocNew.Index = 1;
			this.sprocNew.Text = "New Stored &Procedure";
			// 
			// sprocRefresh
			// 
			this.sprocRefresh.Index = 2;
			this.sprocRefresh.Text = "Refres&h";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.Text = "-";
			// 
			// sprocCopy
			// 
			this.sprocCopy.Enabled = false;
			this.sprocCopy.Index = 4;
			this.sprocCopy.Text = "Cop&y";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 5;
			this.menuItem7.Text = "-";
			// 
			// sprocDelete
			// 
			this.sprocDelete.Index = 6;
			this.sprocDelete.Text = "&Delete";
			// 
			// sprocGenerate
			// 
			this.sprocGenerate.Index = 7;
			this.sprocGenerate.Text = "&Generate Create Script...";
			// 
			// ServerExplorer
			// 
			this.Controls.Add(this.serverTree);
			this.Name = "ServerExplorer";
			this.Size = new System.Drawing.Size(224, 340);
			this.ResumeLayout(false);

		}
		#endregion

		#region COM Code
		[ComRegisterFunction()]
		public static void RegisterClass ( string key )
		{ 
			// Strip off HKEY_CLASSES_ROOT\ from the passed key as I don't need it
			StringBuilder sb = new StringBuilder ( key ) ;
			sb.Replace(@"HKEY_CLASSES_ROOT\","") ;

			// Open the CLSID\{guid} key for write access
			RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(),true);

			// And create the 'Control' key - this allows it to show up in 
			// the ActiveX control container 
			RegistryKey ctrl = k.CreateSubKey ( "Control" ) ; 
			ctrl.Close ( ) ;

			// Next create the CodeBase entry - needed if not string named and GACced.
			RegistryKey inprocServer32 = k.OpenSubKey ( "InprocServer32" , true ) ; 
			inprocServer32.SetValue ( "CodeBase" , Assembly.GetExecutingAssembly().CodeBase ) ; 
			inprocServer32.Close ( ) ;

			// Finally close the main key
			k.Close ( ) ;
		}

		[ComUnregisterFunction()]
		public static void UnregisterClass ( string key )
		{
			StringBuilder sb = new StringBuilder ( key ) ;
			sb.Replace(@"HKEY_CLASSES_ROOT\","") ;

			// Open HKCR\CLSID\{guid} for write access
			RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(),true);

			// Delete the 'Control' key, but don't throw an exception if it does not exist
			k.DeleteSubKey ( "Control" , false ) ;

			// Next open up InprocServer32
			RegistryKey inprocServer32 = k.OpenSubKey ( "InprocServer32" , true ) ;

			// And delete the CodeBase key, again not throwing if missing 
			k.DeleteSubKey ( "CodeBase" , false ) ;

			// Finally close the main key 
			k.Close ( ) ;
		}
		#endregion

		private void serverTree_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
		{
/*			TreeNode node = e.Node;

			// if we have already populated this node, then return
			if (node.Nodes.Count != 1 || node.Nodes[0].Text != "dummy") return;

			// ok, so now we need to populate our node
			// first we find out if we are the database node or the server node
			try 
			{
				if (node.Tag is ServerConfig)
					PopulateServerNode( node );
				else if ((NodeType)node.Tag == NodeType.DatabaseCollection)
					PopulateDatabases( node, null );
			}
			catch (Exception) 
			{
				e.Cancel = true;
			}*/
		}

		private void PopulateDatabases(MySqlConnection conn, TreeNode node) 
		{
			TreeNode dbNode = node.Nodes.Add("Databases");
			dbNode.Tag = NodeType.DatabaseCollection;

			MySqlCommand cmd = new MySqlCommand("SHOW DATABASES", conn);
			using (MySqlDataReader reader = cmd.ExecuteReader()) 
			{
				while (reader.Read()) 
				{	
					string name = reader.GetString(0);
					if (name.ToLower() == "information_schema") continue;
					TreeNode newNode = dbNode.Nodes.Add(name);
					newNode.Tag = NodeType.Database;
				}
			}

			foreach (TreeNode n in dbNode.Nodes) 
				PopulateDatabaseNode(conn, n);
		}

		private void PopulateDatabaseNode(MySqlConnection conn, TreeNode dbNode) 
		{
			conn.ChangeDatabase(dbNode.Text);

			// first do tables
			TreeNode tablesNode = dbNode.Nodes.Add("Tables");
			tablesNode.Tag = NodeType.TableCollection;
			tablesNode.ImageIndex = 7;
			tablesNode.SelectedImageIndex = 7;
			PopulateTables(conn, tablesNode);

			if (!conn.ServerVersion.StartsWith("5")) return;

			TreeNode sprocsNode = dbNode.Nodes.Add("Stored Procedures");
			sprocsNode.Tag = NodeType.StoredProcedureCollection;
			sprocsNode.ImageIndex = 9;
			sprocsNode.SelectedImageIndex = 9;
			PopulateStoredProcedures(conn, sprocsNode);

			TreeNode viewsNode = dbNode.Nodes.Add("Views");
			viewsNode.Tag = NodeType.ViewCollection;
			PopulateViews(conn, viewsNode);
		}

		private void PopulateTables(MySqlConnection conn, TreeNode tablesNode)
		{
			MySqlCommand cmd = new MySqlCommand("SHOW TABLES", conn);
			using (MySqlDataReader reader = cmd.ExecuteReader()) 
			{
				while (reader.Read()) 
				{
					TreeNode tableNode = tablesNode.Nodes.Add(reader.GetString(0));
					tableNode.Tag = NodeType.Table;
					tableNode.ImageIndex = 5;
					tableNode.SelectedImageIndex = 5;
				}
			}
		}

		private void PopulateStoredProcedures(MySqlConnection conn, TreeNode node)
		{
			string dbName = node.Parent.Text;
			MySqlCommand cmd = new MySqlCommand("SELECT name FROM mysql.proc WHERE db ='" + dbName + "'", conn);
			using (MySqlDataReader reader = cmd.ExecuteReader()) 
			{
				while (reader.Read()) 
				{
					TreeNode sprocNode = node.Nodes.Add(reader.GetString(0));
					sprocNode.Tag = NodeType.StoredProcedure;
					sprocNode.ImageIndex = 9;
					sprocNode.SelectedImageIndex = 9;
				}
			}
		}

		private void PopulateViews(MySqlConnection conn, TreeNode node)
		{
/*			string dbName = node.Parent.Text;
			MySqlCommand cmd = new MySqlCommand("SELECT name FROM mysql.proc WHERE db ='" + dbName + "'"", conn);
				using (MySqlDataReader reader = cmd.ExecuteReader()) 
				{
					while (reader.Read()) 
					{
						TreeNode tableNode = tablesNode.Nodes.Add(reader.GetString(0));
						tableNode.Tag = NodeType.StoredProcedure;
					}
				}*/
		}

		private void PopulateNode( TreeNode node, MySqlConnection conn, NodeType nodeType )
		{
			string[] nodeNames = new string[5] { "", "Tables", "UDF", "Views", "Stored Procedures" };
			int[] images = new int[5] { 0, 7, 0, 8, 9 };
			int[] itemNodeImages = new int[5] { 0, 5, 0, 8, 9 };

			string oldDb = conn.Database;

			string sql = "SHOW TABLES";
			if (nodeType == NodeType.UDF && oldDb.ToLower() == "mysql")
				sql = "SELECT * FROM func";
			else if (nodeType == NodeType.View)
				sql = "";
			else if (nodeType == NodeType.StoredProcedure) 
				sql = "SELECT name FROM mysql.proc WHERE db ='" + oldDb + "'";

			// add the node to hold the tables
			TreeNode newNode = node.Nodes.Add( nodeNames[ (int)nodeType ] );
			newNode.ImageIndex = images[ (int)nodeType ];
			newNode.SelectedImageIndex = images[ (int)nodeType ];

			MySqlDataReader reader = null;
			try 
			{
				MySqlCommand cmd = new MySqlCommand( sql, conn );
				reader = cmd.ExecuteReader();
				while (reader.Read()) 
				{
					TreeNode itemNode = newNode.Nodes.Add( reader.GetString(0) );
					itemNode.ImageIndex = itemNodeImages[(int)nodeType];
					itemNode.SelectedImageIndex = itemNodeImages[(int)nodeType];
					itemNode.Tag = nodeType;
				}
				reader.Close();
				if (node.Nodes[0].Text == "dummy")
					node.Nodes.RemoveAt(0);
			}
			catch (Exception) 
			{
				throw;
			}
			finally 
			{
				if (reader != null) reader.Close();
				conn.ChangeDatabase(oldDb);
			}
		}

		private ServerConfig GetNodeConfig( TreeNode node ) 
		{
			if (node.Parent == serverTree.Nodes[0])
				return (ServerConfig)node.Tag;

			if (! (node.Tag is NodeType)) return null;

			NodeType type = (NodeType)node.Tag;
			ServerConfig sc = null;
			if (type == NodeType.Table)
			{
				sc = ((ServerConfig)node.Parent.Parent.Parent.Parent.Tag).Clone();
				sc.database = node.Parent.Parent.Text;
			}
			if (type == NodeType.Database)
			{
				sc = ((ServerConfig)node.Parent.Parent.Tag).Clone();
				sc.database = node.Text;
			}
			return sc;
		}

		private void serverTree_DoubleClick(object sender, System.EventArgs e)
		{
			TreeNode selNode = serverTree.SelectedNode;

			if (! (selNode.Tag is NodeType)) return;

			if ((NodeType)selNode.Tag == NodeType.Table)
				queryTableMenuItem_Click(null,null);
			else if ((NodeType)selNode.Tag == NodeType.StoredProcedure)
				sprocEdit_Click(null,null);

//			NodeEventArgs args = new NodeEventArgs();
//			args.nodeName = selNode.Text;
//			args.nodeType = (NodeType)selNode.Tag;
//			args.config = GetNodeConfig( selNode );

//			if (NodeDoubleClick != null)
//				NodeDoubleClick( this, args );
		}

		private void serverTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Right) return;

			HitTestTreeNodes( serverTree.Nodes, e.X, e.Y );

			TreeNode node = serverTree.SelectedNode;
			if (node == serverTree.Nodes[0])
				serverTree.ContextMenu = serversMenu;
			else if (node.Tag is ServerConfig)
				serverTree.ContextMenu = serverMenu;
			else if ((NodeType)node.Tag == NodeType.DatabaseCollection)
				serverTree.ContextMenu = dbMenu;
			else if ((NodeType)node.Tag == NodeType.Database)
				serverTree.ContextMenu = dbMenu;
			else if ((NodeType)node.Tag == NodeType.StoredProcedureCollection)
				serverTree.ContextMenu = sprocsMenu;
			else if ((NodeType)node.Tag == NodeType.StoredProcedure)
				serverTree.ContextMenu = sprocMenu;
			else if ((NodeType)node.Tag == NodeType.TableCollection)
				serverTree.ContextMenu = tablesMenu;
			else if ((NodeType)node.Tag == NodeType.Table)
				serverTree.ContextMenu = tableMenu;
		}

		private void HitTestTreeNodes( TreeNodeCollection nodes, int x, int y )
		{
			if (nodes == null) return;
			foreach (TreeNode node in nodes) 
			{
				if (node.Bounds.Contains(x,y)) 
				{
					serverTree.SelectedNode = node;
					return;
				}
				HitTestTreeNodes( node.Nodes, x, y );
			}
		}

		private void queryTableMenuItem_Click(object sender, System.EventArgs e)
		{
			TreeNode tableNode = serverTree.SelectedNode;
			TableViewer v = (TableViewer)connectClass.CreateToolWindow("MySql.Design.TableViewer", tableNode.Text);
			ServerConfig sc = GetNodeConfig(tableNode);
			v.Populate(tableNode.Text, sc);
			(v.Tag as EnvDTE.Window).Visible = true;
		}

		private void tableMenuDelete_Click(object sender, System.EventArgs e)
		{
			TreeNode node = serverTree.SelectedNode;
			DialogResult result = MessageBox.Show("Drop table " + node.Text + "?", 
				"Confirm Drop Table", MessageBoxButtons.YesNo );
			if (result == DialogResult.No) return;

			//TODO: finish this
		
		}

		private void dbMenuCreate_Click(object sender, System.EventArgs e)
		{
			ServerConfig sc = GetNodeConfig( serverTree.SelectedNode );
			CreateDatabaseDlg d = new CreateDatabaseDlg();
			DialogResult dr = d.ShowDialog(this, sc);
			if (dr == DialogResult.Cancel) return;

			PopulateServerNode( serverTree.SelectedNode );
		}

		private void serverMenuDelete_Click(object sender, System.EventArgs e)
		{
			TreeNode node = serverTree.SelectedNode;
			DialogResult result = MessageBox.Show("Delete server " + 
				node.Text + "?", "Confirm delete server", MessageBoxButtons.YesNo );
			if (result == DialogResult.No) return;

			// first get a reference to the server config
			ServerConfig config = (ServerConfig)serverTree.SelectedNode.Tag;

			// remove the config from our list of configs
			serverCollection.Remove( config );

			// then delete the node
			serverTree.SelectedNode.Remove();

			// then update the config file
			serverCollection.Save();
		}

		private void serversMenuAdd_Click(object sender, System.EventArgs e)
		{
			ConnectDatabaseDlg d = new ConnectDatabaseDlg("");
			DialogResult dr = d.ShowDialog(this);
			if (dr == DialogResult.Cancel) return;

			ServerConfig sc = d.GetConfig();
			serverCollection.Add( sc );
			AddServer( sc );
			serverCollection.Save();
		}

		private void dbMenuDelete_Click(object sender, System.EventArgs e)
		{
			TreeNode node = serverTree.SelectedNode;
			//TODO: we have to make sure there are not table editors open

			// then ask the user if they really want to drop the database
			DialogResult result = MessageBox.Show("Drop database " + node.Text + "?", 
				"Confirm Drop Database", MessageBoxButtons.YesNo );
			if (result == DialogResult.No) return;

			MySqlConnection conn = new MySqlConnection( GetNodeConfig( node ).GetConnectString(false) );
			try 
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("DROP DATABASE " + node.Text, conn );
				cmd.ExecuteNonQuery();
				node.Remove();
			}
			catch (MySqlException mex) 
			{
				MessageBox.Show("MySQL error during drop database: " + mex.Message );
			}
			catch (Exception ex) 
			{
				MessageBox.Show("System error during drop database: " + ex.Message );
			}
			finally 
			{
				conn.Close();
			}

		}

		private void tablesMenuNewTable_Click(object sender, System.EventArgs e)
		{
			TableDesigner td = (TableDesigner)connectClass.CreateToolWindow( "MySql.Design.TableDesigner", "New Table" );
			(td.Tag as EnvDTE.Window).Visible = true;
		}

		private void sprocEdit_Click(object sender, System.EventArgs e)
		{
			TreeNode node = serverTree.SelectedNode;
			StoredProcedureEditor editor = (StoredProcedureEditor)
				connectClass.CreateToolWindow("MySql.Design.StoredProcedureEditor", node.Text);
			string db = node.Parent.Parent.Text;
			ServerConfig sc = (node.Parent.Parent.Parent.Parent.Tag as ServerConfig);
			editor.Edit(node.Text, db, sc);
			(editor.Tag as EnvDTE.Window).Visible = true;
		}


		/*
 * 
 * 
		private void serverMenuAdd_Click(object sender, System.EventArgs e)
		{
			ConnectServerDlg d = new ConnectServerDlg();
			DialogResult dr = d.ShowDialog(this);
			if (dr == DialogResult.Cancel) return;

//			MySqlConnectionString s = d.Server;
//			userData.Servers.Add( s );
//			AddServer( s );
//			WriteDataFile();
		}

		private void AddDBConnection( MySqlConnectionString conn )
		{
			// add dbconnection node
			TreeNode node = serverTree.Nodes[0].Nodes.Add( conn.Name );
			node.ImageIndex = 4;
			node.SelectedImageIndex = 4;
			node.Tag = dbMenu;

			// add tables node
			TreeNode tablesNode = node.Nodes.Add( "Tables" );
			tablesNode.ImageIndex = 3;
			tablesNode.SelectedImageIndex = 3;
			tablesNode.Tag = tablesMenu;

			DataSet ds = PopulateTablesAndFields(conn);
			foreach (DataRow row in ds.Tables["Tables"].Rows) 
			{
				TreeNode tableNode = tablesNode.Nodes.Add( (string)row[0] );
				tableNode.ImageIndex = 5;
				tableNode.SelectedImageIndex = 5;
				tableNode.Tag = tableMenu;
				foreach (DataRow fieldRow in ds.Tables[ (string)row[0] ].Rows)
				{
					TreeNode fieldNode = tableNode.Nodes.Add( (string)fieldRow[0] );
					fieldNode.ImageIndex = 6;
					fieldNode.SelectedImageIndex = 6;
				}
			}
		}

		private DataSet PopulateTablesAndFields( MySqlConnectionString cs )
		{
//			string connStr = String.Format("server={0};database={1};user id={2};password={3};port={4}",
//				Host, Database, UserId, Password, Port);
			MySqlConnection conn = new MySqlConnection( cs.CreateConnectionString() );
			MySqlDataAdapter da = new MySqlDataAdapter("SHOW TABLES", conn);
			DataSet ds = new DataSet();
			da.Fill(ds, "Tables");

			foreach (DataRow row in ds.Tables["Tables"].Rows) 
			{
				da.SelectCommand.CommandText = "DESCRIBE " + (string)row[0];
				da.Fill( ds, (string)row[0] );
			}

			return ds;
	}


		private void dbMenuDelete_Click(object sender, System.EventArgs e)
		{
			string name = serverTree.SelectedNode.Text;
			serverTree.SelectedNode.Remove();
			userData.RemoveDBConnection( name );
			WriteDataFile();
		}

		private void serverMenuDelete_Click(object sender, System.EventArgs e)
		{
			string name = serverTree.SelectedNode.Text;
			serverTree.SelectedNode.Remove();
			userData.RemoveServerConnection( name );
			WriteDataFile();
		}

		private void serverTree_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			if (e.Node.Parent == null ||
				(e.Node.Parent == serverTree.Nodes[0] &&
				 e.Node.Parent == serverTree.Nodes[1]))
				e.CancelEdit = true;
		}

		private void dbMenuRename_Click(object sender, System.EventArgs e)
		{
			serverTree.SelectedNode.BeginEdit();
		}

		private void serverTree_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{
			MySqlConnectionString conn = userData.FindDBConnection( e.Node.Text );
			conn.Name = e.Label;
			e.Node.Text = e.Label;
			WriteDataFile();
		}

		private string GetDBConnection( TreeNode node )
		{
			TreeNode n = node.Parent.Parent;
			return n.Text;
		}
*/
	}

	internal delegate void NodeDoubleClickDelegate( object sender, NodeEventArgs args );

	internal enum NodeType { 
		DatabaseCollection, 
		Database, 
		TableCollection, 
		Table, 
		UDFCollection, 
		UDF, 
		ViewCollection, 
		View,
		StoredProcedureCollection,
		StoredProcedure 
	};

	internal class NodeEventArgs
	{
		public ServerConfig config;
		public NodeType		nodeType;
		public string		nodeName;
	}


}
