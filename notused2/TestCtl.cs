using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.Text;

namespace ByteFX.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for TestCtl.
	/// </summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class TestControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button button1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(44, 60);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			// 
			// TestCtl
			// 
			this.Controls.Add(this.button1);
			this.Name = "TestCtl";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
