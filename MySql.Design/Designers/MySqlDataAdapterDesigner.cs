using System;
using System.ComponentModel.Design;

namespace ByteFX.Data.MySqlClient.Designers
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
					//					verbs.Add(new DesignerVerb( "Configure Data Adapter...", new EventHandler(OnConfigureAdapter)));
					verbs.Add(new DesignerVerb( "Generate Dataset...", new EventHandler(OnGenerateDataSet)));
					verbs.Add(new DesignerVerb( "Preview Data...", new EventHandler(OnPreview)));
 
				}
				return verbs;			
			}
		}

		private void OnGenerateDataSet(object sender, EventArgs e) 
		{
		}

		private void OnPreview(object sender, EventArgs e) 
		{

		}
	}
}
