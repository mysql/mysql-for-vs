// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using MySql.Data.Common;
using MySql.Data.Types;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for BaseDriver.
	/// </summary>
	internal abstract class Driver : IDisposable 
    {
        protected int threadId;
        protected DBVersion version;
        protected Encoding encoding;
        protected ServerStatusFlags serverStatus;
        protected MySqlConnectionStringBuilder connectionString;
        protected ClientFlags serverCaps;
        protected bool isOpen;
        protected DateTime creationTime;
        protected string serverCharSet;
        protected int serverCharSetIndex;
        protected Hashtable serverProps;
        protected MySqlConnection connection;
        protected Hashtable charSets;
        protected bool hasWarnings;
        protected long maxPacketSize;
        protected long lastInsertId;
        protected long affectedRows;
#if !CF
        protected MySqlPromotableTransaction currentTransaction;
        protected bool inActiveUse;
#endif
        protected MySqlPool pool;

        public Driver(MySqlConnectionStringBuilder settings)
        {
            encoding = Encoding.GetEncoding(1252);
            if (encoding == null)
                throw new MySqlException(Resources.DefaultEncodingNotFound);
            connectionString = settings;
            threadId = -1;
            serverCharSetIndex = -1;
            maxPacketSize = 1024;
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

        public MySqlConnectionStringBuilder Settings
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public ServerStatusFlags ServerStatus
        {
            get { return serverStatus; }
        }

        public bool HasWarnings
        {
            get { return hasWarnings; }
        }

#if !CF
        public MySqlPromotableTransaction CurrentTransaction
        {
            get { return currentTransaction; }
            set { currentTransaction = value; }
        }

        public bool IsInActiveUse
        {
            get { return inActiveUse; }
            set { inActiveUse = value; }
        }
#endif
        public bool IsOpen
        {
            get { return isOpen; }
        }

        public MySqlPool Pool
        {
            get { return pool; }
            set { pool = value; }
        }

        public long MaxPacketSize
        {
            get { return maxPacketSize; }
        }

        internal int ConnectionCharSetIndex
        {
            get { return serverCharSetIndex; }
        }

        internal Hashtable CharacterSets
        {
            get { return charSets; }
        }

        public long AffectedRows
        {
            get { return affectedRows; }
        }

        public long LastInsertedId
        {
            get { return lastInsertId; }
        }

        public bool MoreResults
        {
            get { return (serverStatus & (ServerStatusFlags.AnotherQuery | ServerStatusFlags.MoreResults)) != 0; }
        }

        public bool SupportsOutputParameters 
        {
            get { return Version.isAtLeast(6,0,8); }
        }

        #endregion

        public string Property(string key)
        {
            return (string) serverProps[key];
        }

        public bool IsTooOld()
        {
            TimeSpan ts = DateTime.Now.Subtract(creationTime);
            if (Settings.ConnectionLifeTime != 0 &&
                ts.TotalSeconds > Settings.ConnectionLifeTime)
                return true;
            return false;
        }

        public static Driver Create(MySqlConnectionStringBuilder settings)
        {
            Driver d = null;
            switch (settings.DriverType)
            {
                case MySqlDriverType.Native:
                    d = new NativeDriver(settings);
                    break;
            }
            d.Open();
            return d;
        }

        public virtual void Open()
        {
            creationTime = DateTime.Now;
        }

        public virtual void SafeClose()
        {
            try
            {
                Close();
            }
            catch (Exception)
            {
            }
        }

		public virtual void Close()
		{
            Dispose(true);
            GC.SuppressFinalize(this);
		}

        public virtual void Configure(MySqlConnection connection)
        {
            this.connection = connection;

            bool firstConfigure = false;
            // if we have not already configured our server variables
            // then do so now
            if (serverProps == null)
            {
                firstConfigure = true;
                // load server properties
                serverProps = new Hashtable();
                MySqlCommand cmd = new MySqlCommand("SHOW VARIABLES", connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    try
                    {
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string value = reader.GetString(1);
                            serverProps[key] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        throw;
                    }
                }

                if (serverProps.Contains("max_allowed_packet"))
                    maxPacketSize = Convert.ToInt64(serverProps["max_allowed_packet"]);

                LoadCharacterSets();
            }

#if AUTHENTICATED
			string licenseType = serverProps["license"];
			if (licenseType == null || licenseType.Length == 0 || 
				licenseType != "commercial") 
				throw new MySqlException( "This client library licensed only for use with commercially-licensed MySQL servers." );
#endif
            // if the user has indicated that we are not to reset
            // the connection and this is not our first time through,
            // then we are done.
            if (!Settings.ConnectionReset && !firstConfigure) return;

            string charSet = connectionString.CharacterSet;
            if (charSet == null || charSet.Length == 0)
            {
                if (!version.isAtLeast(4, 1, 0))
                {
                    if (serverProps.Contains("character_set"))
                        charSet = serverProps["character_set"].ToString();
                }
                else
                {
                    if (serverCharSetIndex >= 0)
                        charSet = (string) charSets[serverCharSetIndex];
                    else
                        charSet = serverCharSet;
                }
            }

            // now tell the server which character set we will send queries in and which charset we
            // want results in
            if (version.isAtLeast(4, 1, 0))
            {
                MySqlCommand cmd = new MySqlCommand("SET character_set_results=NULL",
                                                    connection);
                object clientCharSet = serverProps["character_set_client"];
                object connCharSet = serverProps["character_set_connection"];
                if ((clientCharSet != null && clientCharSet.ToString() != charSet) ||
                    (connCharSet != null && connCharSet.ToString() != charSet))
                {
                    MySqlCommand setNamesCmd = new MySqlCommand("SET NAMES " + charSet, connection);
                    setNamesCmd.ExecuteNonQuery();
                }
                cmd.ExecuteNonQuery();
            }

            if (charSet != null)
                Encoding = CharSetMap.GetEncoding(version, charSet);
            else
                Encoding = CharSetMap.GetEncoding(version, "latin1");
        }

        /// <summary>
        /// Loads all the current character set names and ids for this server 
        /// into the charSets hashtable
        /// </summary>
        private void LoadCharacterSets()
        {
            if (!version.isAtLeast(4, 1, 0)) return;

            MySqlCommand cmd = new MySqlCommand("SHOW COLLATION", connection);

            // now we load all the currently active collations
            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    charSets = new Hashtable();
                    while (reader.Read())
                    {
                        charSets[Convert.ToInt32(reader["id"], NumberFormatInfo.InvariantInfo)] =
                            reader.GetString(reader.GetOrdinal("charset"));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw;
            }
        }

        public void ReportWarnings()
        {
            ArrayList errors = new ArrayList();

            MySqlCommand cmd = new MySqlCommand("SHOW WARNINGS", connection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    errors.Add(new MySqlError(reader.GetString(0),
                                              reader.GetInt32(1), reader.GetString(2)));
                }

                hasWarnings = false;
                // MySQL resets warnings before each statement, so a batch could indicate
                // warnings when there aren't any
                if (errors.Count == 0) return;

                MySqlInfoMessageEventArgs args = new MySqlInfoMessageEventArgs();
                args.errors = (MySqlError[]) errors.ToArray(typeof (MySqlError));
                if (connection != null)
                    connection.OnInfoMessage(args);
            }
        }

        #region Abstract Methods

        public abstract bool SupportsBatch { get; }
        public abstract void SetDatabase(string dbName);
        public abstract int PrepareStatement(string sql, ref MySqlField[] parameters);
        public abstract void Reset();
        public abstract void SendQuery(MySqlPacket packet);
        public abstract long ReadResult();
        public abstract bool FetchDataRow(int statementId, int pageSize, int columns);
        public abstract bool SkipDataRow();
        public abstract IMySqlValue ReadColumnValue(int index, MySqlField field, IMySqlValue value);
        public abstract void ExecuteStatement(byte[] bytes);
        public abstract void SkipColumnValue(IMySqlValue valObject);
        public abstract MySqlField[] ReadColumnMetadata(int count);
        public abstract bool Ping();
        public abstract void CloseStatement(int id);
        public abstract void ExecuteDirect(string sql);

		#endregion


        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // if we are pooling, then release ourselves
            if (connectionString.Pooling)
                MySqlPoolManager.RemoveConnection(this);

            isOpen = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
