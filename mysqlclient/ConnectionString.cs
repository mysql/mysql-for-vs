// Copyright (C) 2004-2005 MySQL AB
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
        private bool useUsageAdvisor;
        private int procedureCacheSize;
        private string charSet;
        private ConnectionProtocol protocol;
        private bool compress;
        private string pipeName;
        private bool allowBatch;
        private bool logging;
        private string sharedMemoryName;
        private bool supportOldSyntax;
        private string optionFile;
        private bool useSSL;
        private DriverType driverType;
        private bool allowZeroDateTime;
        private bool convertZeroDateTime;

		public MySqlConnectionString() : base()
		{
            useUsageAdvisor = false;
            procedureCacheSize = 25;
            protocol = ConnectionProtocol.Sockets;
            compress = false;
            allowBatch = true;
            pipeName = "MYSQL";
            logging = false;
            sharedMemoryName = "MYSQL";
            supportOldSyntax = false;
            useSSL = false;
            driverType = DriverType.Native;
            allowZeroDateTime = true;
            convertZeroDateTime = true;
		}

		public MySqlConnectionString(string connectString) : base(connectString)
		{
            Parse();
		}

		#region Server Properties

#if DESIGN
		[Category("Connection")]
		[Description("Protocol to use for connection to MySQL")]
		[DefaultValue(ConnectionProtocol.Sockets)]
#endif
		public ConnectionProtocol Protocol
		{
			get { return protocol; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
#endif
		public string PipeName 
		{
			get { return pipeName; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Should the connection ues compression")]
		[DefaultValue(false)]
#endif
		public bool UseCompression 
		{
			get { return compress; }
		}



#if DESIGN
		[Category("Connection")]
		[Description("Allows execution of multiple SQL commands in a single statement")]
		[DefaultValue(true)]
#endif
		public bool AllowBatch 
		{
			get { return allowBatch; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Enables output of diagnostic messages")]
		[DefaultValue(false)]
#endif
		public bool Logging
		{
			get { return logging; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Name of the shared memory object to use")]
		[DefaultValue("MYSQL")]
#endif
		public string SharedMemoryName 
		{
			get { return sharedMemoryName; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Allows the use of old style @ syntax for parameters")]
		[DefaultValue(false)]
#endif
		public bool UseOldSyntax 
		{
			get { return supportOldSyntax; }
		}

#if DESIGN
		[Category("Connection")]
		[Description("Specifies the type of driver to use for this connection")]
		[DefaultValue(DriverType.Native)]
#endif
		public DriverType DriverType
		{
			get { return driverType; }
		}

		public string OptionFile 
		{
			get { return optionFile; }
		}

		#endregion

		#region Authentication Properties

#if DESIGN
		[Category("Authentication")]
		[Description("Should the connection use SSL.  This currently has no effect.")]
		[DefaultValue(false)]
#endif
		public bool UseSSL
		{
			get { return useSSL; }
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
			get { return allowZeroDateTime; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Should illegal datetime values be converted to DateTime.MinValue")]
		[DefaultValue(false)]
#endif
		public bool ConvertZeroDateTime 
		{
			get { return convertZeroDateTime; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Character set this connection should use")]
		[DefaultValue(null)]
#endif
		public string CharacterSet 
		{
            get { return charSet; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Logs inefficient database operations")]
		[DefaultValue(false)]
#endif
		public bool UseUsageAdvisor 
		{
			get { return useUsageAdvisor; }
		}

#if DESIGN
		[Category("Other")]
		[Description("Number of stored procedures to cache.  0 to disable.")]
		[DefaultValue(25)]
#endif
        public int ProcedureCacheSize
        {
            get { return procedureCacheSize; }
        }

		#endregion

		/// <summary>
		/// Takes a given connection string and returns it, possible
		/// stripping out the password info
		/// </summary>
		/// <returns></returns>
		public string GetConnectionString(bool includePass)
		{
			if (connectString == null) return String.Empty;

			string connStr = connectString;
			if (! PersistSecurityInfo && !includePass)
				connStr = RemovePassword(connStr);

			return connStr;
		}

		private string RemovePassword(string connStr)
		{
			return RemoveKeys(connStr, new string[2] { "password", "pwd" });
		}

		protected override bool ConnectionParameterParsed(string key, string value)
		{
			string lowerCaseKey = key.ToLower();
			string lowerCaseValue = value.Trim().ToLower();
			bool boolVal = lowerCaseValue == "yes" || lowerCaseValue == "true";

			switch (lowerCaseKey)
			{
				case "option file":
                    optionFile = value;
					return true;

				case "driver":
					string d = value.ToLower();
					if (d == "native")
						driverType = DriverType.Native;
					else if (d == "client")
						driverType = DriverType.Client;
					else if (d == "embedded")
						driverType = DriverType.Emebedded;
					else
						throw new ArgumentException("Unknown driver type: " + value);
					return true;

				case "usage advisor":
				case "useusageadvisor":
                    useUsageAdvisor = boolVal;
					return true;

				case "character set":
				case "charset":
					charSet = value;
					return true;

				case "use compression":
				case "compress":
					compress = boolVal;
					return true;

				case "protocol":
					if (value == "socket" || value == "tcp")
						protocol = ConnectionProtocol.Sockets;
					else if (value == "pipe")
						protocol = ConnectionProtocol.NamedPipe;
					else if (value == "unix")
						protocol = ConnectionProtocol.UnixSocket;
					else if (value == "memory")
						protocol = ConnectionProtocol.SharedMemory;
					return true;

				case "pipe name":
				case "pipe":
					pipeName = value;
					return true;

				case "allow batch":
					allowBatch = boolVal;
					return true;

				case "logging":
					logging = boolVal;
					return true;

				case "shared memory name":
					sharedMemoryName = value;
					return true;

				case "old syntax":
				case "oldsyntax":
					supportOldSyntax = boolVal;
					return true;

				case "convert zero datetime":
				case "convertzerodatetime":
					convertZeroDateTime = boolVal;
					return true;

				case "allow zero datetime":
				case "allowzerodatetime":
					allowZeroDateTime = boolVal;
					return true;

                case "procedure cache size":
                case "procedurecachesize":
                case "procedure cache":
                case "procedurecache":
                    procedureCacheSize = Int32.Parse(value);
                    return true;
			}

			if (! base.ConnectionParameterParsed(key, value))
				throw new ArgumentException("Keyword not supported: '" + key + "'");
			return true;
		}

	}
}
