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
using System.Reflection;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for MySqlDataAdapterDesigner.
	/// </summary>
	public class MySqlDataAdapterDesigner : ComponentDesigner
	{
		private DesignerVerbCollection	verbs;

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if ( verbs == null )
				{
					verbs = new DesignerVerbCollection();
					verbs.Add(new DesignerVerb( "Configure Data Adapter...", new EventHandler(OnConfigureAdapter)));
					verbs.Add(new DesignerVerb( "Generate Dataset...", new EventHandler(OnGenerateDataSet)));
					verbs.Add(new DesignerVerb( "Preview Data...", new EventHandler(OnPreview)));
 
				}
				return verbs;			
			}
		}

		private void OnConfigureAdapter(object sender, EventArgs e )
		{
		}

		private void OnGenerateDataSet(object sender, EventArgs e) 
		{
		}

		private void OnPreview(object sender, EventArgs e) 
		{
//			Type t = caller.GetType( "Microsoft.VSDesigner.Data.VS.DataDesignMenuService");
//			caller.Fin
//			caller.GetLoadedModules();
//			Module m;
//			m.

//			if (a == null)
//				System.Windows.Forms.MessageBox.Show("a is null");
//			else
//				System.Windows.Forms.MessageBox.Show("a is good");
//			return;

//			System.Windows.Forms.MessageBox.Show("type returned is " + t == null ? "null" : t.ToString());

			IDesignerHost host1;
//			DataDesignMenuService service1;
			
			host1 = ((IDesignerHost) base.GetService(typeof(IDesignerHost)));

			Assembly ahost = host1.Container.GetType().Assembly;
			MessageBox.Show( "assembly = " + ahost.FullName );
			return;


			Type t1 = host1.GetType("DataDesignMenuService");
			Type t2 = host1.GetType( "Microsoft.VSDesigner.Data.VS.DataDesignMenuService" );
			//host1.
			if (t1 == null)
				MessageBox.Show("t1 is null");
			if (t2 == null)
				MessageBox.Show("t2 is null");
//			service1 = null;
//			if (host1 != null)
//			{
//				service1 = ((DataDesignMenuService) host1.GetService(typeof(DataDesignMenuService)));
 //
//			}
//			if (service1 != null)
//			{
//				service1.OnPreview(sender, e, ((IDataAdapter) base.Component));
 //
//			}
		}
	}
}
