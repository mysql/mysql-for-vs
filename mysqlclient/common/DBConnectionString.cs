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

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	internal abstract class DBConnectionString
	{
		protected string connectString;
        protected bool persistSecurityInfo;
        protected string userId;
        protected string password;
        protected string host;
        protected string database;
        protected int connectionTimeout;
        protected uint port;
        protected bool pooling;
        protected int minPoolSize;
        protected int maxPoolSize;
        protected int poolLifeTime;

		public DBConnectionString()
		{	
            persistSecurityInfo = false;
            connectionTimeout = 15;
            pooling = true;
            minPoolSize = 0;
            maxPoolSize = 100;
            port = 3306;
        }

        public DBConnectionString(string connectString) : this()
        {
            this.connectString = connectString;
        }

        #region Base Connection Properties

#if DESIGN
		[Category("Connection")]
		[Description("The name or IP address of the server to use")]
#endif
        public string Server
        {
            get { return host; }
        }

#if DESIGN
		[Category("Connection")]
		[Description("Port to use when connecting with sockets")]
		[DefaultValue(3306)]
#endif
        public uint Port
        {
            get { return port; }
        }

#if DESIGN
		[Category("Connection")]
		[Description("Database to use initially")]
		[Editor("MySql.Data.MySqlClient.Design.DatabaseTypeEditor,MySqlClient.Design", typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public string Database
        {
            get { return database; }
            set { database = value; }
        }

#if DESIGN
		[Category("Connection")]
		[Description("Number of seconds to wait for the connection to succeed")]
		[DefaultValue(15)]
#endif
        public int ConnectionTimeout
        {
            get { return connectionTimeout; }
        }

#if DESIGN
		[Category("Authentication")]
		[Description("Show user password in connection string")]
		[DefaultValue(false)]
#endif
        public bool PersistSecurityInfo
        {
            get { return persistSecurityInfo; }
        }

#if DESIGN
		[Category("Authentication")]
		[Description("The username to connect as")]
#endif
        public string UserId
        {
            get { return userId; }
        }

#if DESIGN
		[Category("Authentication")]
		[Description("The password to use for authentication")]
#endif
        public string Password
        {
            get { return password; }
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
            get { return pooling; }
        }

#if DESIGN
		[Category("Pooling")]
		[Description("Minimum number of connections to have in this pool")]
		[DefaultValue(0)]
#endif
        public int MinPoolSize
        {
            get { return minPoolSize; }
        }

#if DESIGN
		[Category("Pooling")]
		[Description("Maximum number of connections to have in this pool")]
		[DefaultValue(100)]
#endif
        public int MaxPoolSize
        {
            get { return maxPoolSize; }
        }

#if DESIGN
		[Category("Pooling")]
		[Description("Maximum number of seconds a connection should live.  This is checked when a connection is returned to the pool.")]
		[DefaultValue(0)]
#endif
        public int ConnectionLifetime
        {
            get { return poolLifeTime; }
        }

        #endregion

		protected string RemoveKeys(string value, string[] keys)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string[] pairs = Utility.ContextSplit(value, ";", "\"'");
			foreach (string keyvalue in pairs)
			{
				string test = keyvalue.Trim().ToLower();
				if (test.StartsWith("pwd") || test.StartsWith("password"))
					continue;
				sb.Append(keyvalue);
				sb.Append(";");
			}
			sb.Remove(sb.Length-1, 1);  // remove the trailing ;
			return sb.ToString();
		}

		protected virtual bool ConnectionParameterParsed(string key, string value)
		{
			string lowerKey =  key.ToLower(System.Globalization.CultureInfo.InvariantCulture);

			switch (lowerKey)
			{
				case "persist security info":
                    persistSecurityInfo = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "uid":
				case "username":
				case "user id":
				case "user name": 
				case "userid":
                    userId = value;
					return true;

				case "password": 
				case "pwd":
                    password = value; 
					return true;

				case "host":
				case "server":
				case "data source":
				case "datasource":
				case "address":
				case "addr":
				case "network address":
                    host = value;
					return true;
				
				case "initial catalog":
				case "database":
                    database = value;
					return true;

				case "connection timeout":
				case "connect timeout":
                    connectionTimeout = Int32.Parse(value, 
                        System.Globalization.NumberFormatInfo.InvariantInfo);
					return true;

				case "port":
                    port = UInt32.Parse(value, 
                        System.Globalization.NumberFormatInfo.InvariantInfo);
					return true;

				case "pooling":
					pooling = value.ToLower() == "yes" || value.ToLower() == "true";
					return true;

				case "min pool size":
                    minPoolSize = Int32.Parse(value, 
                        System.Globalization.NumberFormatInfo.InvariantInfo);
					return true;

				case "max pool size":
					maxPoolSize = Int32.Parse(value, 
                        System.Globalization.NumberFormatInfo.InvariantInfo);
					return true;

				case "connection lifetime":
					poolLifeTime = Int32.Parse(value, 
                        System.Globalization.NumberFormatInfo.InvariantInfo);
					return true;
			}
			return false;
		}

		protected virtual void Parse() 
		{
            String[] keyvalues = connectString.Split(';');
            String[] newkeyvalues = new String[keyvalues.Length];
            int x = 0;

            // first run through the array and check for any keys that
            // have ; in their value
            foreach (String keyvalue in keyvalues)
            {
                // check for trailing ; at the end of the connection string
                if (keyvalue.Length == 0) continue;

                // this value has an '=' sign so we are ok
                if (keyvalue.IndexOf('=') >= 0)
                {
                    newkeyvalues[x++] = keyvalue;
                }
                else
                {
                    newkeyvalues[x - 1] += ";";
                    newkeyvalues[x - 1] += keyvalue;
                }
            }

            // now we run through our normalized key-values, splitting on equals
            for (int y = 0; y < x; y++)
            {
                String[] parts = newkeyvalues[y].Split('=');

                // first trim off any space and lowercase the key
                parts[0] = parts[0].Trim().ToLower();
                parts[1] = parts[1].Trim();

                // we also want to clear off any quotes
                parts[0] = parts[0].Trim('\'', '"');
                parts[1] = parts[1].Trim('\'', '"');

                ConnectionParameterParsed(parts[0], parts[1]);
            }
        }

	}
}
