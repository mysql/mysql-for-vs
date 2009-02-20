using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
	[Guid("D949EA95-EDA1-4b65-8A9E-266949A99360")]
	class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
	{
        private static DbProviderFactory factory;

        internal static DbProviderFactory Factory
        {
            get 
            { 
                if (factory== null)
                    factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                return factory; 
            }
        }

		public override object CreateObject(Type objType)
		{
            if (objType == typeof(DataConnectionUIControl))
                return new MySqlDataConnectionUI();
            else if (objType == typeof(DataConnectionProperties))
                return new MySqlConnectionProperties();
            else if (objType == typeof(DataConnectionSupport))
                return new MySqlConnectionSupport();
//            else if (objType == typeof(DataConnectionPromptDialog))
  //              return new MySqlConnectionPromptDialog();
            else
                return base.CreateObject(objType);
		}
	}
}
