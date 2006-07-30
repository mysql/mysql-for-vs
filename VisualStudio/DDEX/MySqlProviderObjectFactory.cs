using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a data provider object factory.  This is registered as
	/// a VS service and is called by the DDEX engine to create data provider
	/// specific objects.
	/// </summary>
    [Guid("C7255288-CF3E-40be-9136-734DBB69A50B")]
	internal class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
	{
		public MySqlProviderObjectFactory()
		{
        }

		public override object CreateObject(Type objType)
		{
//            Logger.WriteLine("MySqlProviderObjectFactory::CreateObject for type=" + objType.ToString());
            if (objType == typeof(DataConnectionUIControl))
                return new MySqlConnectionUIControl();
            else  if (objType == typeof(DataConnectionProperties))
                return new MySqlConnectionProperties();
			else if (objType == typeof(DataConnectionSupport))
                return new MySqlConnectionSupport();
			return base.CreateObject(objType);
		}
	}
}
