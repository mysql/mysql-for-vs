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
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MySql.Data.MySqlClient.Design
{
	public class UserData
	{
		[XmlElement("Server", typeof(MySqlConnectionString))]
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


	}

}
