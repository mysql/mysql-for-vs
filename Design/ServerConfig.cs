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
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MySql.Design
{
	public class ServerConfig : ICloneable
	{
		public string	host;
		public string	userId;
		public string	password;
		public int		port;
		public bool		savePassword;
		public string	name;
		public string	database;

		public string GetConnectString(bool includeDatabase) 
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append( "host=" + host );
			sb.Append( ";user id=" + userId );
			sb.Append( ";password=" + password );
			if (database != null && database.Length > 0 && includeDatabase)
				sb.Append( ";database=" + database );
			return sb.ToString();
		}

		#region ICloneable Members

		object ICloneable.Clone() 
		{
			return Clone();
		}

		public ServerConfig Clone()
		{
			ServerConfig sc = new ServerConfig();
			sc.host = host;
			sc.userId = userId;
			sc.password = password;
			sc.port = port;
			sc.savePassword = savePassword;
			sc.name = name;
			sc.database = database;
			return sc;
		}

		#endregion
	}

	public class ServerCollection 
	{
		private ArrayList	servers;

		public ServerCollection() 
		{
			servers = new ArrayList();
		}

		[XmlElement("Server", typeof(ServerConfig))]
		public ArrayList Servers 
		{
			get { return servers; }
			set { servers = value; }
		}

		public void Add( ServerConfig sc ) 
		{
			servers.Add( sc );
		}

		public void Remove( ServerConfig sc ) 
		{
			servers.Remove( sc );
		}

		public static ServerCollection Load() 
		{
			string path = Environment.GetFolderPath( 
				Environment.SpecialFolder.ApplicationData );
			path += "/MySQL/VSConfig.xml";

			FileInfo fi = new FileInfo( path );
			if (File.Exists( path ) && fi.Length > 0) 
			{
				XmlTextReader reader = new XmlTextReader( path );
				XmlSerializer serializer = new XmlSerializer( typeof(ServerCollection) );
				try 
				{
					ServerCollection c = (ServerCollection)serializer.Deserialize( reader );
					return c;
				}
				catch (Exception) 
				{
					throw;
				}
				finally 
				{
					reader.Close();
				}
			}
			else
				return new ServerCollection();
		}

		public void Save() 
		{
			string path = Environment.GetFolderPath( 
				Environment.SpecialFolder.ApplicationData );
			path += "/MySQL/VSConfig.xml";

			XmlTextWriter writer = new XmlTextWriter( path, System.Text.Encoding.UTF8 );
			XmlSerializer serializer = new XmlSerializer( typeof(ServerCollection) );
			try 
			{
				serializer.Serialize( writer, this );
			}
			catch (Exception) 			
			{
				throw;
			}
			finally 
			{
				writer.Close();
			}
		}

/*		[XmlElement("Server", typeof(MySqlConnectionString))]
		public ArrayList	Servers;

		[XmlElement("DBConnection", typeof(MySqlConnectionString))]
		public ArrayList	DBConnections;

		public UserData() 
		{
			Servers = new ArrayList();
			DBConnections = new ArrayList();
		}

		public void RemoveDBConnection( string name ) 
		{
			MySqlConnectionString conn = FindDBConnection(name);
			if (conn != null)
				DBConnections.Remove( conn );
		}

		internal MySqlConnectionString FindDBConnection( string name )
		{
			foreach (MySqlConnectionString conn in DBConnections)
				if (conn.Name == name) return conn;
			return null;
		}

		public void RemoveServerConnection( string name ) 
		{
			foreach (MySqlConnectionString s in Servers)
				if (s.Server == name)
				{
					Servers.Remove( s );
					return;
				}
		}
*/

	}

}
