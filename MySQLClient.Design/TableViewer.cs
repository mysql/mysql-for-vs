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
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Win32;
using System.Text;
//using EnvDTE;

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for TableViewer.
	/// </summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class TableViewer : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.DataGrid dataGrid;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

//		private DBConnection	dbconn;
		private string			tableName;
		private DataTable		data;
		private bool			populating;
		private bool			updating;

		public TableViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			data = new DataTable();
			data.RowChanged += new DataRowChangeEventHandler(data_RowChanged);
			data.RowDeleted += new DataRowChangeEventHandler(data_RowDeleted);
			populating = false;
			updating = false;
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

//		public DBConnection DBConnection 
//		{
//			get { return dbconn; }
//			set { dbconn = value; }
//		}

		public string Tablename
		{
			get { return tableName; }
			set { tableName = value; /*Populate();*/ }
		}

/*		private void Populate()
		{
			try 
			{
				if (dbconn == null || tableName == null) return; 
				populating = true;
				MySqlConnection conn = new MySqlConnection( dbconn.ConnectionString );
				MySqlDataAdapter da = new MySqlDataAdapter( "SELECT * FROM " + tableName, conn );
				data.Clear();
				da.Fill(data);
				dataGrid.DataSource = data;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error populating view: " + ex.Message);
			}
			finally 
			{
				populating = false;
			}
		}
*/
		internal void ShowEditor()
		{
//			Window w = (Window)this.Tag;
//			w.Visible = true;
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGrid = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGrid
			// 
			this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGrid.DataMember = "";
			this.dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid.Location = new System.Drawing.Point(8, 8);
			this.dataGrid.Name = "dataGrid";
			this.dataGrid.Size = new System.Drawing.Size(504, 344);
			this.dataGrid.TabIndex = 0;
			// 
			// TableViewer
			// 
			this.Controls.Add(this.dataGrid);
			this.Name = "TableViewer";
			this.Size = new System.Drawing.Size(520, 360);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
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

		private void UpdateDataTable(DataRow row)
		{
/*			if (populating) return;
			updating = true;
			MySqlConnection conn = new MySqlConnection( dbconn.ConnectionString );
			try 
			{
				conn.Open();
				MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM " + tableName, conn);
				MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			
				da.Update( new DataRow[] { row } );
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error updating view: " + ex.Message);
			}
			finally 
			{
				updating = false;
				conn.Close();
			}*/
		}

		private void data_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			if (updating) return;
			UpdateDataTable(e.Row);
		}

		private void data_RowDeleted(object sender, DataRowChangeEventArgs e)
		{
			if (updating) return;
			UpdateDataTable(e.Row);
		}
	}
}
