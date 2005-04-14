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
using System.Text;
using System.Collections;
using MySql.Data.Common;
using MySql.Data.Types;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for BaseDriver.
	/// </summary>
	internal abstract class Driver
	{
		protected int					threadId;
		protected DBVersion				version;
		protected Encoding				encoding;
		protected ServerStatusFlags		serverStatus;
		protected MySqlConnectionString	connectionString;
		protected ClientFlags			serverCaps;
		protected bool					isOpen;
		protected DateTime				creationTime;
		protected string				serverCharSet;
		protected int					serverCharSetIndex;
		protected Hashtable				serverProps;
		protected MySqlConnection		connection;
		protected bool					processing;
		protected Hashtable				charSets;

		public Driver(MySqlConnectionString settings)
		{
			encoding = System.Text.Encoding.GetEncoding(1252);
			connectionString = settings;
			processing = false;
			threadId = -1;
			serverCharSetIndex = -1;
			serverCharSet = null;
		}

		#region Properties

		public MySqlConnection Connection 
		{
			get { return connection; }
		}

		public int ThreadID 
		{
			get { return threadId; }
		}

		public DBVersion Version 
		{
			get { return version; }
		}

		public MySqlConnectionString Settings 
		{
			get { return connectionString; }
			set { connectionString = value; }
		}

		public Encoding Encoding 
		{
			get { return encoding; 	}
			set { encoding = value; }
		}

		public bool IsProcessing 
		{ 
			get { return processing; }
			set { processing = value; }
		}

		public ServerStatusFlags ServerStatus 
		{
			get { return serverStatus; }
		}

		public abstract bool HasMoreResults { get; }

		#endregion

		public bool IsTooOld() 
		{
			TimeSpan ts = DateTime.Now.Subtract( creationTime );
			if (ts.Seconds > Settings.ConnectionLifetime)
				return true;
			return false;
		}

		public static Driver Create( MySqlConnectionString settings ) 
		{
			Driver d = null;
			if (settings.DriverType == DriverType.Native)
				d = new NativeDriver( settings );
#if !CF
			else
				d = new ClientDriver( settings );
#endif

			d.Open();
			return d;
		}
		
		public virtual void Open() 
		{
			creationTime = DateTime.Now;
		}

		public virtual void Close() 
		{
			isOpen = false;
		}

		/// <summary>
		/// I don't like this setup but can't think of a better way of doing
		/// right now.
		/// </summary>
		/// <param name="connection"></param>
		public virtual void Configure(MySqlConnection connection)
		{
			this.connection = connection;
			
			// load server properties
			serverProps = new Hashtable();
			MySqlCommand cmd = new MySqlCommand("SHOW VARIABLES", connection);

			try 
			{
				MySqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read()) 
					serverProps[ reader.GetValue(0) ] = reader.GetValue(1);
				reader.Close();
			}
			catch (Exception ex)
			{
				Logger.LogException( ex );
				throw;
			}

#if AUTHENTICATED
			string licenseType = props["license"];
			if (licenseType == null || licenseType.Length == 0 || 
				licenseType != "commercial") 
				throw new MySqlException( "This client library licensed only for use with commercially-licensed MySQL servers." );
#endif
			LoadCharacterSets();

			string charSet = connectionString.CharacterSet;
			if (charSet == null || charSet == String.Empty) 
			{
				if (! version.isAtLeast(4,1,0))
				{
					if (serverProps.Contains( "character_set" ))
						charSet = serverProps["character_set"].ToString();
				}
				else 
				{
					if (serverCharSetIndex >= 0)
						charSet = (string)charSets[serverCharSetIndex];
					else
						charSet = serverCharSet;
				}
			}

			// now tell the server which character set we will send queries in and which charset we
			// want results in
			if (version.isAtLeast(4,1,0)) 
			{
				cmd.CommandText = "SET NAMES " + charSet + "; SET character_set_results=NULL";
				cmd.ExecuteNonQuery();
			}

			if (charSet != null)
				Encoding = CharSetMap.GetEncoding( version, charSet );
			else
				Encoding = CharSetMap.GetEncoding( version, "latin1" );
		}

		/// <summary>
		/// Loads all the current character set names and ids for this server 
		/// into the charSets hashtable
		/// </summary>
		private void LoadCharacterSets() 
		{
			if (! version.isAtLeast(4,1,0)) return;

			MySqlCommand cmd = new MySqlCommand("SHOW COLLATION", connection);
			MySqlDataReader reader = null;

			// now we load all the currently active collations
			try 
			{
				reader = cmd.ExecuteReader();
				charSets = new Hashtable();
				while (reader.Read()) 
				{
					charSets[ Convert.ToInt32(reader["id"]) ] = reader["charset"];
				}
			}
			catch (Exception ex) 
			{
				Logger.LogException(ex);
				throw;
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		public void ShowWarnings(int count) 
		{
			if (count == 0 || 
				(serverStatus & (ServerStatusFlags.MoreResults | ServerStatusFlags.AnotherQuery )) != 0 ) return;

			MySqlError[] errors = new MySqlError[count];

			MySqlCommand cmd = new MySqlCommand("SHOW WARNINGS", connection);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				int i = 0;
				while (reader.Read()) 
				{
					errors[i++] = new MySqlError( reader.GetString(0), reader.GetUInt32(1), reader.GetString(2) );
				}
				reader.Close();
				if (i == 0) return;  // MySQL resets warnings before each statement, so a batch could indicate
									// warnings when there aren't any

				MySqlInfoMessageEventArgs args = new MySqlInfoMessageEventArgs();
				args.errors = errors;
				if (connection != null)
					connection.OnInfoMessage( args );
			
			}
			catch (Exception) 
			{
				throw;
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		#region Abstract Methods

		public abstract bool SupportsBatch { get; }
		public abstract void SetDatabase( string dbName );
		public abstract PreparedStatement Prepare( string sql, string[] names ); 
		public abstract void Reset();
		public abstract CommandResult SendQuery( byte[] bytes, int length, bool consume );
		public abstract bool ReadResult( ref long fieldCount, ref ulong affectedRows, ref long lastInsertId );
		public abstract bool OpenDataRow(int fieldCount, bool isBinary, int statementId);
		public abstract IMySqlValue ReadFieldValue( int index, MySqlField field, IMySqlValue value ); 
		public abstract CommandResult ExecuteStatement( byte[] bytes, int statementId, int cursorPageSize );
		public abstract void SkipField(IMySqlValue valObject );

		public abstract void ReadFieldMetadata( int count, ref MySqlField[] fields );
		public abstract bool Ping();
		#endregion

	}
}
