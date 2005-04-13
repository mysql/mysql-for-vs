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
using System.Collections;
using System.ComponentModel;
using System.Text;
using MySql.Data.Common;

namespace MySql.Data.MySqlClient
{
	internal enum ConnectionProtocol 
	{
		Sockets, NamedPipe, UnixSocket, SharedMemory
	}

	/// <summary>
	/// Specifies the connection types supported
	/// </summary>
	public enum DriverType 
	{
		/// <summary>Use TCP/IP sockets</summary>
		Native, 
		/// <summary>Use client library</summary>
		Client, 
		/// <summary>Use MySQL embedded server</summary>
		Emebedded
	}

	/// <summary>
	/// Summary description for MySqlConnectionString.
	/// </summary>
	internal sealed class MySqlConnectionString : DBConnectionString
	{
		private Hashtable	defaults;

		public MySqlConnectionString() : base()
		{
		}

		public MySqlConnectionString(string connectString) : this()
		{
			SetConnectionString( connectString );
		}

		#region Server Properties
		public string Name 
		{
			get { return connectionName; }
			set { connectionName = value; }
		}


#if DESIGN
		[Category("Connection")]
		[Description("The name or IP address of the server to use")]
#endif
		public string Server 
		{
			get { return GetString("host"); }
			set { keyValues["host"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Port to use when connecting with sockets")]
		[DefaultValue(3306)]
#endif
		public uint Port 
		{
			get { return (uint)GetInt("port"); }
			set { keyValues["port"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Protocol to use for connection to MySQL")]
		[DefaultValue(ConnectionProtocol.Sockets)]
#endif
		public ConnectionProtocol Protocol
		{
			get { return (ConnectionProtocol)keyValues["protocol"]; }
			set { keyValues["protocol"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
#endif
		public string PipeName 
		{
			get { return GetString("pipeName"); }
			set { keyValues["pipeName"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Should the connection ues compression")]
		[DefaultValue(false)]
#endif
		public bool UseCompression 
		{
			get { return GetBool("compress"); }
			set { keyValues["compress"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Database to use initially")]
		[Editor("MySql.Data.MySqlClient.Design.DatabaseTypeEditor,MySqlClient.Design", typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public string Database
		{
			get { return GetString("database"); }
			set { keyValues["database"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Number of seconds to wait for the connection to succeed")]
		[DefaultValue(15)]
#endif
		public int ConnectionTimeout
		{
			get { return GetInt("connect timeout"); }
			set { keyValues["connect timeout"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Allows execution of multiple SQL commands in a single statement")]
		[DefaultValue(true)]
#endif
		public bool AllowBatch 
		{
			get { return GetBool("allow batch"); }
			set { keyValues["allow batch"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Enables output of diagnostic messages")]
		[DefaultValue(false)]
#endif
		public bool Logging
		{
			get { return GetBool("logging"); }
			set { keyValues["logging"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Name of the shared memory object to use")]
		[DefaultValue("MYSQL")]
#endif
		public string SharedMemoryName 
		{
			get { return GetString("memname"); }
			set { keyValues["memname"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Allows the use of old style @ syntax for parameters")]
		[DefaultValue(false)]
#endif
		public bool UseOldSyntax 
		{
			get { return GetBool("oldsyntax"); }
			set { keyValues["oldsyntax"] = value; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Specifies the type of driver to use for this connection")]
		[DefaultValue(DriverType.Native)]
#endif
		public DriverType DriverType
		{
			get { return (DriverType)keyValues["driver"]; }
			set { keyValues["driver"] = value; }
		}

		#endregion

		#region Authentication Properties

#if DESIGN
		[Category("Authentication")]
		[Description("The username to connect as")]
#endif
		public string UserId 
		{
			get { return GetString("user id"); }
			set { keyValues["user id"] = value; }
		}

#if DESIGN
		[Category("Authentication")]
		[Description("The password to use for authentication")]
#endif
		public string Password 
		{
			get { return GetString("password"); }
			set { keyValues["password"] = value; }
		}

#if DESIGN
		[Category("Authentication")]
		[Description("Should the connection use SSL.  This currently has no effect.")]
		[DefaultValue(false)]
#endif
		public bool UseSSL
		{
			get { return GetBool("useSSL"); }
			set { keyValues["useSSL"] = value; }
		}

#if DESIGN
		[Category("Authentication")]
		[Description("Show user password in connection string")]
		[DefaultValue(false)]
#endif
		public bool PersistSecurityInfo 
		{
			get { return GetBool("persist security info"); }
			set { keyValues["persist security info"] = value; }
		}
		#endregion

		#region Pooling Properties

#if DESIGN
		[Category("Pooling")]
		[Description("Should the connection support pooling")]
		[DefaultValue(true)]
#endif
		public bool Pooling 
		{
			get { return GetBool("pooling"); }
			set { keyValues["pooling"] = value; }
		}

#if DESIGN
		[Category("Pooling")]
		[Description("Minimum number of connections to have in this pool")]
		[DefaultValue(0)]
#endif
		public int MinPoolSize 
		{
			get { return GetInt("min pool size"); }
			set { keyValues["min pool size"] = value; }
		}

#if DESIGN
		[Category("Pooling")]
		[Description("Maximum number of connections to have in this pool")]
		[DefaultValue(100)]
#endif
		public int MaxPoolSize 
		{
			get { return GetInt("max pool size"); }
			set { keyValues["max pool size"] = value; }
		}

#if DESIGN
		[Category("Pooling")]
		[Description("Maximum number of seconds a connection should live.  This is checked when a connection is returned to the pool.")]
		[DefaultValue(0)]
#endif
		public int ConnectionLifetime 
		{
			get { return GetInt("connect lifetime"); }
			set { keyValues["connect lifetime"] = value; }
		}

		#endregion

		#region Other Properties

#if DESIGN
		[Category("Other")]
		[Description("Should zero datetimes be supported")]
		[DefaultValue(false)]
#endif
		public bool AllowZeroDateTime 
		{
			get { return GetBool("allowzerodatetime"); }
			set { keyValues["alllowzerodatetime"] = value; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Character set this connection should use")]
		[DefaultValue(null)]
#endif
		public string CharacterSet 
		{
			get { return GetString("charset"); }
			set { keyValues["charset"] = value; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Logs inefficient database operations")]
		[DefaultValue(false)]
#endif
		public bool UseUsageAdvisor 
		{
			get { return GetBool("usageAdvisor"); }
			set { keyValues["usageAdvisor"] = value; }
		}

		#endregion

		/// <summary>
		/// Takes a given connection string and returns it, possible
		/// stripping out the password info
		/// </summary>
		/// <returns></returns>
		public string GetConnectionString()
		{
			if (connectString == null) return CreateConnectionString();

			StringBuilder str = new StringBuilder();
			Hashtable ht = ParseKeyValuePairs( connectString );

			if (! PersistSecurityInfo) 
				ht.Remove("password");

			foreach( string key in ht.Keys)
				str.AppendFormat(null, "{0}={1};", key, ht[key]);

			if (str.Length > 0)
				str.Remove( str.Length-1, 1 );

			return str.ToString();
		}

		/// <summary>
		/// Uses the values in the keyValues hash to create a
		/// connection string
		/// </summary>
		/// <returns></returns>
		public string CreateConnectionString()
		{
			string cStr = String.Empty;

			Hashtable values = (Hashtable)keyValues.Clone();
			Hashtable defaultValues = GetDefaultValues();

			if (!PersistSecurityInfo && values.Contains("password") )
				values.Remove( "password" );

			// we always return the server key.  It's not needed but 
			// seems weird for it not to be there.
			cStr = "server=" + values["host"] + ";";
			values.Remove("server");

			foreach (string key in values.Keys)
			{
				if (!values[key].Equals( defaultValues[key]))
					cStr += key + "=" + values[key] + ";";
			}

			return cStr;
		}

		protected override Hashtable GetDefaultValues()
		{
			defaults = base.GetDefaultValues();
			if (defaults == null)
			{
				defaults = new Hashtable();
				defaults["host"] = String.Empty;
				defaults["connect lifetime"] = 0;
				defaults["user id"] = String.Empty;
				defaults["password"] = String.Empty;
				defaults["database"] = null;
				defaults["charset"] = null;
				defaults["pooling"] = true;
				defaults["min pool size"] = 0;
				defaults["protocol"] = ConnectionProtocol.Sockets;
				defaults["max pool size"] = 100;
				defaults["connect timeout"] = 15;
				defaults["port"] = 3306;
				defaults["useSSL"] = false;
				defaults["compress"] = false;
				defaults["persist security info"] = false;
				defaults["allow batch"] = true;
				defaults["logging"] = false;
				defaults["oldsyntax"] = false;
				defaults["pipeName"] = "MySQL";
				defaults["memname"] = "MYSQL";
				defaults["allowzerodatetime"] = false;
				defaults["usageAdvisor"] = false;
				defaults["driver"] = DriverType.Native;
			}
			return (Hashtable)defaults.Clone();
		}

		protected override bool ConnectionParameterParsed(Hashtable hash, string key, string value)
		{
			switch (key.ToLower()) 
			{
				case "driver":
					string d = value.ToLower();
					if (d == "native")
						hash["driver"] = DriverType.Native;
					else if (d == "client")
						hash["driver"] = DriverType.Client;
					else if (d == "embedded")
						hash["driver"] = DriverType.Emebedded;
					else
						throw new MySqlException("Unknown driver type: " + value);
					return true;

				case "usage advisor":
				case "useusageadvisor":
					hash["usageAdvisor"] = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "character set":
				case "charset":
					hash["charset"] = value;
					return true;

				case "use compression":
				case "compress":
					hash["compress"] = 
						value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "protocol":
					if (value == "socket" || value == "tcp")
						hash["protocol"] = ConnectionProtocol.Sockets;
					else if (value == "pipe")
						hash["protocol"] = ConnectionProtocol.NamedPipe;
					else if (value == "unix")
						hash["protocol"] = ConnectionProtocol.UnixSocket;
					else if (value == "memory")
						hash["protocol"] = ConnectionProtocol.SharedMemory;
					return true;

				case "pipe name":
				case "pipe":
					hash["pipeName"] = value;
					return true;

				case "allow batch":
					hash["allow batch"] = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "logging":
					hash["logging"] = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "shared memory name":
					hash["memname"] = value;
					return true;

				case "old syntax":
				case "oldsyntax":
					hash["oldsyntax"] = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "allow zero datetime":
				case "allowzerodatetime":
					hash["allowzerodatetime"] = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;
			}

			if (! base.ConnectionParameterParsed(hash, key, value))
				throw new ArgumentException("Keyword not supported: '" + key + "'");
			return true;
		}

	}
}
