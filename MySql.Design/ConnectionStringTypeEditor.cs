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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing.Design;

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for ConnectionStringTypeEditor.
	/// </summary>
	public class ConnectionStringTypeEditor : UITypeEditor
	{
		// fEdSrc is created and nulled in EditValue. It is here only to allow the value
		// to be shared with the List_Click event handler
		private IWindowsFormsEditorService	fEdSvc = null;
		private UserData					userData;
		private string						connStr;

		/// <summary>
		/// GetEditStyle must be overridden for any UITypeEditor.
		/// In this case, we are using a DropDown style.
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext pContext)
		{
			if (pContext != null && pContext.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(pContext);
		} 

		/// <summary>
		/// EditValue must be overridden from UITypeEditor to install a control that appears in the drop down.
		/// For UITypeEditorEditStyle.DropDown, here is the general procedure:
		/// 1. Get the Edit Service from pProvider.GetService(typeof(IWindowsFormsEditorService)).
		///    It contains methods to run the DropDown and Forms interfaces.
		/// 2. Get a Window control instance that reflects the UI you want. In this case, its a ListBox.
		///    If you wanted multiple controls, consider something like the Panel class and add controls to its Controls list.
		/// 3. We want the ListBox to close on a Click event much like boolean and enum dropdowns do.
		///    So we set up a Click event handler that calls CloseDropDown on the Edit Service.
		/// 4. Add data to the ListBox.
		/// 5. Set the initial value of the list. pValue contains that value.
		/// 6. Let the Edit Service open and manage the DropDown interface.
		/// 7. Return the value from the list.
		/// </summary>
		public override object EditValue(ITypeDescriptorContext pContext, IServiceProvider pProvider, object pValue)
		{
			if (pContext != null && pContext.Instance != null && pProvider != null) 
				try
				{
					connStr = (string)pValue;

					// get the editor service
					fEdSvc = (IWindowsFormsEditorService)
						pProvider.GetService(typeof(IWindowsFormsEditorService));

					ConnectDatabaseDlg dlg = new ConnectDatabaseDlg( connStr );
					if (DialogResult.Cancel == dlg.ShowDialog())
						return connStr;

					return dlg.GetConnectionString();

					// create the control(s) we want for the UI
//					list = new ListBox();
//					list.Click += new EventHandler(List_Click);
        
					// modify the list's properties including the Item list
//					FillInList();

					// let the editor service place the list on screen and manage its events
//					fEdSvc.DropDownControl(list);

					// return the updated value;
//					return connStr;
				}  // try
				finally
				{
					fEdSvc = null;
				}
			else
				return pValue;

		}  

		/// <summary>
		/// List_Click is a click event handler for the ListBox. We want the have the list
		/// close when the user clicks, just like the Enum and Bool types do in their UITypeEditors.
		/// </summary>
/*		protected void List_Click(object pSender, EventArgs pArgs)
		{
			fEdSvc.CloseDropDown();

			DBConnection conn;
			if (list.SelectedIndex == list.Items.Count-1)
			{
				ConnectDatabaseDlg dlg = new ConnectDatabaseDlg();
				DialogResult result = dlg.ShowDialog();
				if (result == DialogResult.Cancel) return;

				conn = dlg.DBConnection;
				userData.DBConnections.Add( conn );
				UserData.Save( userData );
			}
			else 
				conn = (DBConnection)userData.DBConnections[ list.SelectedIndex ];

			connStr = conn.ConnectionString;
		}  

		protected void FillInList()
		{
			list.Items.Clear();
			foreach (DBConnection dbc in userData.DBConnections)
				list.Items.Add( dbc.Name );
			list.Items.Add("<New Connection...>");
		}
*/
	}
}
