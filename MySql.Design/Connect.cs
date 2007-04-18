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
using Microsoft.Office.Core;
using Extensibility;
using System.Runtime.InteropServices;
using EnvDTE;
using System.Reflection;
using System.Windows.Forms;

namespace MySql.Design
{

	#region Read me for Add-in installation and setup information.
	// When run, the Add-in wizard prepared the registry for the Add-in.
	// At a later time, if the Add-in becomes unavailable for reasons such as:
	//   1) You moved this project to a computer other than which is was originally created on.
	//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
	//   3) Registry corruption.
	// you will need to re-register the Add-in by building the MyAddin21Setup project 
	// by right clicking the project in the Solution Explorer, then choosing install.
	#endregion
	
	/// <summary>
	///   The object for implementing an Add-in.
	/// </summary>
	/// <seealso class='IDTExtensibility2' />
	[GuidAttribute("7756F17D-640A-4DBA-A452-A38523DB2FBC"), ProgId("MySql.Design.Connect")]
	public class Connect : Object, Extensibility.IDTExtensibility2, IDTCommandTarget
	{
		private _DTE	applicationObject;
		private AddIn	addInInstance;
		private Window	windowToolWindow;

		/// <summary>
		///		Implements the constructor for the Add-in object.
		///		Place your initialization code within this method.
		/// </summary>
		public Connect()
		{
		}

		public static void SetForceNonIEContext()
		{
			Type tControl = typeof(Control);
			Type [] nested =
				tControl.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic);
			Type tActiveXImpl = null;
			foreach (Type t in nested)
			{
				Console.WriteLine("{0}", t.FullName);
				if (t.Name == "ActiveXImpl")
				{
					tActiveXImpl = t;
					break;
				}
			}
			FieldInfo checkedIE = tActiveXImpl.GetField("checkedIE",
				BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);
			FieldInfo isIE = tActiveXImpl.GetField("isIE",
				BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);
			Console.WriteLine("checkedIE={0}", checkedIE.GetValue(null));
			Console.WriteLine("isIE={0}", isIE.GetValue(null));
			checkedIE.SetValue(null, true);
			isIE.SetValue(null, false);
		}

		private void CreateToolbar() 
		{

		}

		/// <summary>
		///      Implements the OnConnection method of the IDTExtensibility2 interface.
		///      Receives notification that the Add-in is being loaded.
		/// </summary>
		/// <param term='application'>
		///      Root object of the host application.
		/// </param>
		/// <param term='connectMode'>
		///      Describes how the Add-in is being loaded.
		/// </param>
		/// <param term='addInInst'>
		///      Object representing this Add-in.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, Extensibility.ext_ConnectMode connectMode, object addInInst, ref System.Array custom)
		{
			applicationObject = (_DTE)application;
			addInInstance = (AddIn)addInInst;

			object objTemp = null;
			applicationObject = (_DTE)application;
//			addInInstance = (AddIn)addInInst;
			SetForceNonIEContext();

			if (connectMode == Extensibility.ext_ConnectMode.ext_cm_UISetup) 
				CreateToolbar();

			string mysqlPro = "{80BF7F79-0CBB-4be3-8AB2-2BF48BCB276A}";
			windowToolWindow = applicationObject.Windows.CreateToolWindow (addInInstance, "MySql.Design.ServerExplorer", "Connector/.NET", mysqlPro, ref objTemp);
			SetTabPicture();
			windowToolWindow.Visible = true;
			ServerExplorer se = (ServerExplorer)objTemp;
			se.ConnectClass = this;

			se.NodeDoubleClick += new NodeDoubleClickDelegate(se_NodeDoubleClick);

			if(connectMode == Extensibility.ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands commands = applicationObject.Commands;
				_CommandBars commandBars = applicationObject.CommandBars;

				// When run, the Add-in wizard prepared the registry for the Add-in.
				// At a later time, the Add-in or its commands may become unavailable for reasons such as:
				//   1) You moved this project to a computer other than which is was originally created on.
				//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
				//   3) You add new commands or modify commands already defined.
				// You will need to re-register the Add-in by building the MyAddin1Setup project,
				// right-clicking the project in the Solution Explorer, and then choosing install.
				// Alternatively, you could execute the ReCreateCommands.reg file the Add-in Wizard generated in
				// the project directory, or run 'devenv /setup' from a command prompt.
				try
				{
					Command command = commands.AddNamedCommand(addInInstance, "MySQLPro", "MySQLPro", "Executes the command for MySQLPro", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled);
					CommandBar commandBar = (CommandBar)commandBars["Tools"];
					CommandBarControl commandBarControl = command.AddControl(commandBar, 1);
				}
				catch(System.Exception)
				{
				}
			}
			
		}

		private void SetTabPicture() 
		{
			Assembly myAssembly = Assembly.GetExecutingAssembly();
			System.IO.Stream myStream = myAssembly.GetManifestResourceStream("MySql.Design.resources.tabPicture.bmp");
			System.Drawing.Bitmap image = new System.Drawing.Bitmap(myStream);

			Guid IPicture = new Guid("7BF80980-BF32-101A-8BBB-00AA00300CAB");
			PICTDESCBmp bmp = new PICTDESCBmp(image);
			stdole.IPictureDisp ipic = null;

			NativeMethods.OleCreatePictureIndirect(ref bmp, ref IPicture, true, out ipic);
			windowToolWindow.SetTabPicture(ipic);
		}

		/// <summary>
		///     Implements the OnDisconnection method of the IDTExtensibility2 interface.
		///     Receives notification that the Add-in is being unloaded.
		/// </summary>
		/// <param term='disconnectMode'>
		///      Describes how the Add-in is being unloaded.
		/// </param>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(Extensibility.ext_DisconnectMode disconnectMode, ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
		///      Receives notification that the collection of Add-ins has changed.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnAddInsUpdate(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnStartupComplete method of the IDTExtensibility2 interface.
		///      Receives notification that the host application has completed loading.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref System.Array custom)
		{
		}

		/// <summary>
		///      Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
		///      Receives notification that the host application is being unloaded.
		/// </summary>
		/// <param term='custom'>
		///      Array of parameters that are host application specific.
		/// </param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref System.Array custom)
		{
		}
		
		/// <summary>
		///      Implements the QueryStatus method of the IDTCommandTarget interface.
		///      This is called when the command's availability is updated
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to determine state for.
		/// </param>
		/// <param term='neededText'>
		///		Text that is needed for the command.
		/// </param>
		/// <param term='status'>
		///		The state of the command in the user interface.
		/// </param>
		/// <param term='commandText'>
		///		Text requested by the neededText parameter.
		/// </param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText)
		{
			if(neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "MyAddin1.Connect.MyAddin1")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
				}
			}
		}

		/// <summary>
		///      Implements the Exec method of the IDTCommandTarget interface.
		///      This is called when the command is invoked.
		/// </summary>
		/// <param term='commandName'>
		///		The name of the command to execute.
		/// </param>
		/// <param term='executeOption'>
		///		Describes how the command should be run.
		/// </param>
		/// <param term='varIn'>
		///		Parameters passed from the caller to the command handler.
		/// </param>
		/// <param term='varOut'>
		///		Parameters passed from the command handler to the caller.
		/// </param>
		/// <param term='handled'>
		///		Informs the caller if the command was handled or not.
		/// </param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "MyAddin1.Connect.MyAddin1")
				{
					handled = true;
					return;
				}
			}
		}

		public object CreateToolWindow( string className, string caption ) 
		{
			object temp = null;
			Window editor = applicationObject.Windows.CreateToolWindow (addInInstance, className, caption, Guid.NewGuid().ToString("B"), ref temp);
			editor.Linkable = false;
			(temp as UserControl).Tag = editor;
			return temp;
		}

		private void se_NodeDoubleClick(object sender, NodeEventArgs args)
		{
			string caption = args.nodeName;
			TableViewer v = (TableViewer)CreateToolWindow( "MySql.Design.TableViewer", caption );
//			object temp = null;
//			Window editor = applicationObject.Windows.CreateToolWindow (addInInstance, "MySql.Design.TableViewer", caption, Guid.NewGuid().ToString("B"), ref temp);
//			editor.Linkable = false;
//			TableViewer v = (temp as TableViewer);
			v.Populate( args.nodeName, args.config );
			(v.Tag as Window).Visible = true;
			//editor.Visible = true;
		}
	}

}