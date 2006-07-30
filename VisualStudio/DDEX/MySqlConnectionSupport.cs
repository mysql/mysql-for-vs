using System;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data connection support implementation.  This
	/// class provides all the provider support entities related to a
	/// connection.
	/// </summary>
	internal class MySqlConnectionSupport : AdoDotNetConnectionSupport
	{
		public MySqlConnectionSupport() : base("MySql.Data.MySqlClient")
		{
            Logger.WriteLine("MySqlConnectionSupport::ctor");
        }

		/// <summary>
		/// This method shows how to easily override the default connection
		/// behavior and customize for design time purposes.  In this case,
		/// we restrict the provider so it will only allow connections to
		/// Microsoft SQL Server 2005.
		/// </summary>
		public override bool Open(bool doPromptCheck)
		{
            Logger.WriteLine("MySqlConnectionSupport::open");
            bool result = base.Open(doPromptCheck);
			if (result)
			{
                DbConnection conn = ProviderObject as DbConnection;
                Debug.Assert(ProviderObject.GetType().ToString() == "MySql.Data.MySqlClient.MySqlConnection",
                    "The provider object is not of the correct type");
				Debug.Assert(conn.State == ConnectionState.Open, "The connection is not open.");
			}

			return result;
		}

/*		protected override DataObjectIdentifierConverter CreateObjectIdentifierConverter()
		{
            Logger.WriteLine("MySqlConnectionSupport::CreateObjectIdentifierConverter");
            return new MySqlDataObjectIdentifierConverter(Site as DataConnection);
		}
*/
/*		protected override DataObjectItemComparer CreateObjectItemComparer()
		{
            Logger.WriteLine("MySqlConnectionSupport::CreateObjectItemComparer");
            return new MySqlDataObjectItemComparer(Site as DataConnection);
		}
*/
		protected override DataSourceInformation CreateDataSourceInformation()
		{
            Logger.WriteLine("MySqlConnectionSupport::CreateDataSourceInformation");
            return new MySqlDataSourceInformation(Site as DataConnection);
		}

		protected override object GetServiceImpl(Type serviceType)
		{
            Logger.WriteLine("MySqlConnectionSupport::GetServiceImpl");
            if (serviceType == typeof(DataViewSupport))
			{
				if (viewSupport == null)
				{
					viewSupport = new MySqlDataViewSupport();
				}
				return viewSupport;
			}
			if (serviceType == typeof(DataObjectSupport))
			{
				if (objectSupport == null)
				{
					objectSupport = new MySqlDataObjectSupport();
				}
				return objectSupport;
			}
/*			if (serviceType == typeof(DataObjectIdentifierResolver))
			{
				if (objectIdentifierResolver == null)
				{
					objectIdentifierResolver = new MySqlDataObjectIdentifierResolver(Site as DataConnection);
				}
				return objectIdentifierResolver;
			}*/
			return base.GetServiceImpl(serviceType);
		}

		private DataViewSupport viewSupport = null;
		private DataObjectSupport objectSupport = null;
		private DataObjectIdentifierResolver objectIdentifierResolver = null;
	}
}
