using System;
using System.ComponentModel;

namespace ByteFX.Data.MySqlClient.Design
{
	internal enum ConnectionProtocol 
	{
		Sockets, NamedPipe, UnixSocket
	}

	/// <summary>
	/// Summary description for ConnectionSettings.
	/// </summary>
	internal class ConnectionSettings
	{
		private string				server;
		private ConnectionProtocol	protocol = ConnectionProtocol.Sockets;
		private int					port = 3306;
		private string				pipeName;
		private bool				useCompression;
		private string				database;
		private int					connectTimeout = 15;

		private string				userId;
		private string				password;
		private bool				useSSL;

		private bool				pooling;
		private int					minPoolSize;
		private int					maxPoolSize=100;
		private int					connectionLifetime;
		private string				connectionName;

		[Browsable(false)]
		public string Name 
		{
			get { return connectionName; }
			set { connectionName = value; }
		}

		#region Server Properties
		[Category("Connection")]
		[Description("The name or IP address of the server to use")]
		public string Server 
		{
			get { return server; }
			set { server = value; }
		}

		[Category("Connection")]
		[Description("Protocol to use for connection to MySQL")]
		[DefaultValue(ConnectionProtocol.Sockets)]
		public ConnectionProtocol Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}

		[Category("Connection")]
		[Description("Port to use when connecting with sockets")]
		[DefaultValue(3306)]
		public int Port 
		{
			get { return port; }
			set { port = value; }
		}

		[Category("Connection")]
		[Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
		public string PipeName 
		{
			get { return pipeName; }
			set { pipeName = value; }
		}

		[Category("Connection")]
		[Description("Should the connection ues compression")]
		[DefaultValue(false)]
		public bool UseCompression 
		{
			get { return useCompression; }
			set { useCompression = value; }
		}

		[Category("Connection")]
		[Description("Database to use initially")]
		[Editor(typeof(DatabaseTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string Database
		{
			get { return database; }
			set { database = value; }
		}

		[Category("Connection")]
		[Description("Number of seconds to wait for the connection to succeed")]
		[DefaultValue(15)]
		public int ConnectionTimeout
		{
			get { return connectTimeout; }
			set { connectTimeout = value; }
		}
		#endregion

		#region Authentication Properties

		[Category("Authentication")]
		[Description("The username to connect as")]
		public string UserId 
		{
			get { return userId; }
			set { userId = value; }
		}

		[Category("Authentication")]
		[Description("The password to use for authentication")]
		public string Password 
		{
			get { return password; }
			set { password = value; }
		}

		[Category("Authentication")]
		[Description("Should the connection use SSL.  This currently has no effect.")]
		[DefaultValue(false)]
		public bool UseSSL
		{
			get { return useSSL; }
			set { useSSL = value; }
		}

		#endregion

		#region Pooling Properties

		[Category("Pooling")]
		[Description("Should the connection support pooling")]
		[DefaultValue(true)]
		public bool Pooling 
		{
			get { return pooling; }
			set { pooling = value; }
		}

		[Category("Pooling")]
		[Description("Minimum number of connections to have in this pool")]
		[DefaultValue(0)]
		public int MinPoolSize 
		{
			get { return minPoolSize; }
			set { minPoolSize = value; }
		}

		[Category("Pooling")]
		[Description("Maximum number of connections to have in this pool")]
		[DefaultValue(100)]
		public int MaxPoolSize 
		{
			get { return maxPoolSize; }
			set { maxPoolSize = value; }
		}

		[Category("Pooling")]
		[Description("Maximum number of seconds a connection should live.  This is checked when a connection is returned to the pool.")]
		[DefaultValue(0)]
		public int ConnectionLifetime 
		{
			get { return connectionLifetime; }
			set { connectionLifetime = value; }
		}

		#endregion

		public string GetConnectionString()
		{
			string connStr = String.Format("server={0};user id={1};password={2}", server, userId, password);

			if (protocol == ConnectionProtocol.NamedPipe)
				connStr += ";pipe=" + pipeName;
			if (useCompression == true)
				connStr += ";compress=true";
			if (pooling == false)
				connStr += ";pooling=false";
			if (minPoolSize != 0)
				connStr += ";MinPoolSize=" + minPoolSize;
			if (maxPoolSize != 100)
				connStr += ";MaxPoolSize=" + maxPoolSize;
			if (port != 3306)
				connStr += ";port=" + port;

			return connStr;
		}
	}
}
