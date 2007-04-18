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

#if WINDOWS
using System;
using System.Windows.Forms;
using System.Drawing.Design;

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for MySqlConnectionDesign.
	/// </summary>
	internal class SqlCommandTextEditor : UITypeEditor
	{
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}

		public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return false;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			System.Data.IDbCommand command = (System.Data.IDbCommand)context.Instance;

			if (command.Connection == null) 
			{
				MessageBox.Show("Connection property not set to a valid connection.\n"+
					"Please set and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return value;
			}

			SqlCommandEditorDlg dlg = new SqlCommandEditorDlg( command );

			dlg.SQL = (string)value;
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				return dlg.SQL;
			}
			else
				return value;
		}
	}
}
#endif
