// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
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

namespace MySql.Data.MySqlClient.Design
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
		private System.Windows.Forms.MenuItem dbMenuRefresh;
		private System.Windows.Forms.MenuItem dbMenuConnect;
		private System.Windows.Forms.MenuItem dbMenuCreate;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu dbsMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem dbMenuDelete;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.ContextMenu dbMenu;
		private System.Windows.Forms.MenuItem dbMenuGenScript;
		private System.Windows.Forms.MenuItem dbMenuModify;
		private System.Windows.Forms.MenuItem dbMenuClose;
		private System.Windows.Forms.MenuItem dbMenuRename;
		private System.Windows.Forms.ContextMenu serversMenu;
		private System.Windows.Forms.ContextMenu serverMenu;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem serversMenuRefresh;
		private System.Windows.Forms.MenuItem serversMenuAdd;
		private System.Windows.Forms.MenuItem serverMenuRefresh;
		private System.Windows.Forms.MenuItem serverMenuDelete;
		private System.Windows.Forms.ContextMenu tablesMenu;
		private System.Windows.Forms.MenuItem tablesMenuNewTable;
		private System.Windows.Forms.MenuItem tablesMenuRefresh;
		private System.Windows.Forms.ContextMenu tableMenu;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem queryTableMenuItem;
		private UserData	userData;
		#endregion


		public ServerExplorer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ReadDataFile();

			serverTree.Nodes[0].Tag = dbsMenu;
			serverTree.Nodes[1].Tag = serversMenu;
		}

/*		public Connect ConnectClass 
		{
			get { return connectClass; }
			set { connectClass = value; }
		}
*/
		private void ReadDataFile() 
		{
			string path = Environment.GetFolderPath( 
				Environment.SpecialFolder.LocalApplicationData );
			path += "/ConnectorNET.data";

			MessageBox.Show("checking " + path);
			if (File.Exists( path )) 
			{
				MessageBox.Show("it exists");
				XmlTextReader reader = new XmlTextReader( path );
				XmlSerializer serializer = new XmlSerializer( typeof(UserData) );
				try 
				{
					userData = (UserData)serializer.Deserialize( reader );
					FillTree();
				}
				catch (Exception ex) 
				{
					MessageBox.Show(ex.Message);
				}
				reader.Close();
			}
			else
				userData = new UserData();
		}

		private void FillTree() 
		{
			serverTree.Nodes[0].Nodes.Clear();
			serverTree.Nodes[1].Nodes.Clear();
			foreach (MySqlConnectionString s in userData.Servers) 
				AddServer(s);
			foreach (MySqlConnectionString conn in userData.DBConnections)
				AddDBConnection(conn);
			
		}

		internal UserData UserData 
		{
			get { return userData; }
		}

		private void WriteDataFile()
		{
			string path = Environment.GetFolderPath( 
				Environment.SpecialFolder.LocalApplicationData );
			path += "/ConnectorNET.data";

			XmlTextWriter writer = new XmlTextWriter( path, System.Text.Encoding.UTF8 );
			XmlSerializer serializer = new XmlSerializer( typeof(UserData) );
			try 
			{
				serializer.Serialize( writer, userData );
			}
			catch (Exception ex) 
			{
				MessageBox.Show(ex.Message);
			}
			writer.Close();
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
			this.dbsMenu = new System.Windows.Forms.ContextMenu();
			this.dbMenuRefresh = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.dbMenuConnect = new System.Windows.Forms.MenuItem();
			this.dbMenuCreate = new System.Windows.Forms.MenuItem();
			this.treeImages = new System.Windows.Forms.ImageList(this.components);
			this.serversMenu = new System.Windows.Forms.ContextMenu();
			this.serversMenuRefresh = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.serversMenuAdd = new System.Windows.Forms.MenuItem();
			this.dbMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.dbMenuDelete = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.dbMenuGenScript = new System.Windows.Forms.MenuItem();
			this.dbMenuModify = new System.Windows.Forms.MenuItem();
			this.dbMenuClose = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.dbMenuRename = new System.Windows.Forms.MenuItem();
			this.serverMenu = new System.Windows.Forms.ContextMenu();
			this.serverMenuRefresh = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.serverMenuDelete = new System.Windows.Forms.MenuItem();
			this.tablesMenu = new System.Windows.Forms.ContextMenu();
			this.tablesMenuNewTable = new System.Windows.Forms.MenuItem();
			this.tablesMenuRefresh = new System.Windows.Forms.MenuItem();
			this.tableMenu = new System.Windows.Forms.ContextMenu();
			this.queryTableMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// serverTree
			// 
			this.serverTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.serverTree.ContextMenu = this.dbsMenu;
			this.serverTree.HideSelection = false;
			this.serverTree.ImageList = this.treeImages;
			this.serverTree.LabelEdit = true;
			this.serverTree.Location = new System.Drawing.Point(4, 36);
			this.serverTree.Name = "serverTree";
			this.serverTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
																				   new System.Windows.Forms.TreeNode("Databases"),
																				   new System.Windows.Forms.TreeNode("Servers", 1, 1)});
			this.serverTree.Size = new System.Drawing.Size(216, 300);
			this.serverTree.TabIndex = 0;
			this.serverTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.serverTree_MouseDown);
			this.serverTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.serverTree_AfterLabelEdit);
			this.serverTree.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.serverTree_BeforeLabelEdit);
			// 
			// dbsMenu
			// 
			this.dbsMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.dbMenuRefresh,
																					this.menuItem6,
																					this.dbMenuConnect,
																					this.dbMenuCreate});
			// 
			// dbMenuRefresh
			// 
			this.dbMenuRefresh.Index = 0;
			this.dbMenuRefresh.Text = "Re&fresh";
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.Text = "-";
			// 
			// dbMenuConnect
			// 
			this.dbMenuConnect.Index = 2;
			this.dbMenuConnect.Text = "&Connect to Database...";
			this.dbMenuConnect.Click += new System.EventHandler(this.dbMenuConnect_Click);
			// 
			// dbMenuCreate
			// 
			this.dbMenuCreate.Index = 3;
			this.dbMenuCreate.Text = "Create &New MySQL Database...";
			this.dbMenuCreate.Click += new System.EventHandler(this.dbMenuCreate_Click);
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
			this.serversMenuAdd.Click += new System.EventHandler(this.serverMenuAdd_Click);
			// 
			// dbMenu
			// 
			this.dbMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.menuItem1,
																				   this.menuItem3,
																				   this.dbMenuDelete,
																				   this.menuItem5,
																				   this.dbMenuGenScript,
																				   this.dbMenuModify,
																				   this.dbMenuClose,
																				   this.menuItem10,
																				   this.dbMenuRename});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Re&fresh";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// dbMenuDelete
			// 
			this.dbMenuDelete.Index = 2;
			this.dbMenuDelete.Text = "&Delete";
			this.dbMenuDelete.Click += new System.EventHandler(this.dbMenuDelete_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 3;
			this.menuItem5.Text = "-";
			// 
			// dbMenuGenScript
			// 
			this.dbMenuGenScript.Index = 4;
			this.dbMenuGenScript.Text = "&Generate Create Script...";
			// 
			// dbMenuModify
			// 
			this.dbMenuModify.Index = 5;
			this.dbMenuModify.Text = "&Modify Connection...";
			// 
			// dbMenuClose
			// 
			this.dbMenuClose.Index = 6;
			this.dbMenuClose.Text = "&Close Connection";
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 7;
			this.menuItem10.Text = "-";
			// 
			// dbMenuRename
			// 
			this.dbMenuRename.Index = 8;
			this.dbMenuRename.Text = "Re&name";
			this.dbMenuRename.Click += new System.EventHandler(this.dbMenuRename_Click);
			// 
			// serverMenu
			// 
			this.serverMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.serverMenuRefresh,
																					   this.menuItem7,
																					   this.serverMenuDelete});
			// 
			// serverMenuRefresh
			// 
			this.serverMenuRefresh.Index = 0;
			this.serverMenuRefresh.Text = "Re&fresh";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 1;
			this.menuItem7.Text = "-";
			// 
			// serverMenuDelete
			// 
			this.serverMenuDelete.Index = 2;
			this.serverMenuDelete.Text = "&Delete";
			this.serverMenuDelete.Click += new System.EventHandler(this.serverMenuDelete_Click);
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
																					  this.menuItem11,
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
			// menuItem11
			// 
			this.menuItem11.Index = 3;
			this.menuItem11.Text = "&Delete";
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

		private void dbMenuConnect_Click(object sender, System.EventArgs e)
		{
			ConnectDatabaseDlg d = new ConnectDatabaseDlg("");
			DialogResult dr = d.ShowDialog(this);
			if (dr == DialogResult.Cancel) return;

			MySqlConnectionString cStr = d.GetConnectStringObject();

			userData.DBConnections.Add( cStr );
			AddDBConnection( cStr );
			WriteDataFile();
		}

		private void dbMenuCreate_Click(object sender, System.EventArgs e)
		{
			CreateDatabaseDlg d = new CreateDatabaseDlg();
			DialogResult dr = d.ShowDialog(this);
			if (dr == DialogResult.Cancel) return;

			
		}

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

		private void AddServer( MySqlConnectionString s ) 
		{
			TreeNode node = serverTree.Nodes[1].Nodes.Add( s.Server );
			node.ImageIndex = 2;
			node.SelectedImageIndex = 2;
			node.Tag = serverMenu;
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

		private void serverTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right) 
			{
				HitTestTreeNodes( serverTree.Nodes, e.X, e.Y );

				serverTree.ContextMenu = (ContextMenu)serverTree.SelectedNode.Tag;
			}
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

		private void queryTableMenuItem_Click(object sender, System.EventArgs e)
		{
/*			string dbConn = GetDBConnection( serverTree.SelectedNode );
			DBConnection conn = userData.FindDBConnection( dbConn );
			string tableName = serverTree.SelectedNode.Text;

			string caption = String.Format("{0}:{1}", dbConn, tableName );

//			TableViewer v = connectClass.CreateTableViewer( caption );		
			v.DBConnection = conn;
			v.Tablename = tableName;
			v.ShowEditor();*/
		}


	}
}
