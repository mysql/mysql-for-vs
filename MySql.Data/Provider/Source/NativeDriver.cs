// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Diagnostics;
using System.IO;
using MySql.Data.Common;
using MySql.Data.Types;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient.Properties;
#if !CF
using System.Net.Security;
using System.Security.Authentication;
using System.Globalization;
#endif

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Summary description for Driver.
    /// </summary>
    internal class NativeDriver : Driver
    {
        protected int protocol;
        protected String encryptionSeed;
        protected ClientFlags connectionFlags;

        protected MySqlStream stream;
        protected Stream baseStream;
        private BitArray nullMap;

        private int warningCount;
        private MySqlPacket packet;

        public NativeDriver(MySqlConnectionStringBuilder settings)
            : base(settings)
        {
            isOpen = false;
        }

        ~NativeDriver()
        {
            Close();
        }

		public ClientFlags Flags
		{
			get { return connectionFlags; }
		}

        /// <summary>
        /// Returns true if this connection can handle batch SQL natively
        /// This means MySQL 4.1.1 or later.
        /// </summary>
        public override bool SupportsBatch
        {
            get
            {
                if ((Flags & ClientFlags.MULTI_STATEMENTS) != 0)
                {
                    if (version.isAtLeast(4, 1, 0) && !version.isAtLeast(4, 1, 10))
                    {
                        object qtType = serverProps["query_cache_type"];
                        object qtSize = serverProps["query_cache_size"];
                        if (qtType != null && qtType.Equals("ON") &&
                            (qtSize != null && !qtSize.Equals("0")))
                            return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public MySqlPacket GetPacket()
        { 
            return packet; 
        }

        /// <summary>
        /// ExecuteCommand does the work of writing the actual command bytes to the writer
        /// We break it out into a function since it is used in several places besides query
        /// </summary>
        /// <param name="cmd">The cmd that we are sending</param>
        /// <param name="bytes">The bytes of the command, can be null</param>
        /// <param name="length">The number of bytes to send</param>
/*        private void ExecuteCommand(DBCmd cmd, byte[] bytes, int length)
        {
            Debug.Assert(length == 0 || bytes != null);

            try
            {
                stream.StartOutput((ulong) length + 1, true);
                stream.WriteByte((byte) cmd);
                if (length > 0)
                    stream.Write(bytes, 0, length);
                stream.Flush();
            }
            catch (MySqlException ex)
            {
                if (ex.IsFatal)
                {
                    isOpen = false;
                    Close();
                }
                throw;
            }
        }*/

        private void ReadOk(bool read)
        {
            try
            {
                if (read)
                    packet = stream.ReadPacket();
                byte marker = (byte) packet.ReadByte();
                if (marker != 0)
                    throw new MySqlException("Out of sync with server", true, null);

                packet.ReadFieldLength(); /* affected rows */
                packet.ReadFieldLength(); /* last insert id */
                if (packet.HasMoreData)
                {
                    serverStatus = (ServerStatusFlags) packet.ReadInteger(2);
                    packet.ReadInteger(2);  /* warning count */
                    if (packet.HasMoreData)
                    {
                        packet.ReadLenString();  /* message */
                    }
                }
            }
            catch (MySqlException ex)
            {
                if (ex.IsFatal)
                {
                    isOpen = false;
                    Close();
                }
                throw;
            }
        }

        /// <summary>
        /// Sets the current database for the this connection
        /// </summary>
        /// <param name="dbName"></param>
        public override void SetDatabase(string dbName)
        {
            byte[] dbNameBytes = Encoding.GetBytes(dbName);

            packet.Clear();
            packet.WriteByte((byte)DBCmd.INIT_DB);
            packet.Write(dbNameBytes);
            ExecutePacket(packet);

            ReadOk(true);
        }

        public override void Configure(MySqlConnection conn)
        {
            base.Configure(conn);
            stream.MaxPacketSize = (ulong) maxPacketSize;
			stream.Encoding = Encoding;
        }

        public override void Open()
        {
            base.Open();

            // connect to one of our specified hosts
            try
            {
#if !CF
                if (Settings.ConnectionProtocol == MySqlConnectionProtocol.SharedMemory)
                {
                    SharedMemoryStream str = new SharedMemoryStream(Settings.SharedMemoryName);
                    str.Open(Settings.ConnectionTimeout);
                    baseStream = str;
                }
                else
                {
#endif
                    string pipeName = Settings.PipeName;
                    if (Settings.ConnectionProtocol != MySqlConnectionProtocol.NamedPipe)
                        pipeName = null;
                    StreamCreator sc = new StreamCreator(Settings.Server, Settings.Port, pipeName,
                        Settings.Keepalive);
                    baseStream = sc.GetStream(Settings.ConnectionTimeout);
#if !CF
                }
#endif
            }
            catch (Exception ex)
            {
                throw new MySqlException(Resources.UnableToConnectToHost, 
                    (int) MySqlErrorCode.UnableToConnectToHost, ex);
            }

            if (baseStream == null)
                throw new MySqlException(Resources.UnableToConnectToHost,
                    (int)MySqlErrorCode.UnableToConnectToHost);

            int maxSinglePacket = 255*255*255;
            stream = new MySqlStream(baseStream, encoding, false);

            stream.ResetTimeout((int)Settings.ConnectionTimeout*1000);

            // read off the welcome packet and parse out it's values
            packet = stream.ReadPacket();
            protocol = packet.ReadByte();
            string versionString = packet.ReadString();
            version = DBVersion.Parse(versionString);
            threadId = packet.ReadInteger(4);
            encryptionSeed = packet.ReadString();

            if (version.isAtLeast(4, 0, 8))
                maxSinglePacket = (256*256*256) - 1;

            // read in Server capabilities if they are provided
            serverCaps = 0;
            if (packet.HasMoreData)
                serverCaps = (ClientFlags) packet.ReadInteger(2);
            if (version.isAtLeast(4, 1, 1))
            {
                /* New protocol with 16 bytes to describe server characteristics */
                serverCharSetIndex = (int)packet.ReadByte();

                serverStatus = (ServerStatusFlags) packet.ReadInteger(2);
                packet.Position += 13;
                string seedPart2 = packet.ReadString();
                encryptionSeed += seedPart2;
            }

            // based on our settings, set our connection flags
            SetConnectionFlags();

            packet.Clear();
            packet.WriteInteger((int) connectionFlags,
                                version.isAtLeast(4, 1, 0) ? 4 : 2);

#if !CF
            if ((serverCaps & ClientFlags.SSL) ==0)
            {
                if ((connectionString.SslMode != MySqlSslMode.None)
                && (connectionString.SslMode != MySqlSslMode.Prefered))
                {
                    // Client requires SSL connections.
                    string message = String.Format(Resources.NoServerSSLSupport,
                        Settings.Server);
                    throw new MySqlException(message);
                }
            }
            else if (connectionString.SslMode != MySqlSslMode.None)
            {
                stream.SendPacket(packet);
                StartSSL();
                packet.Clear();
                packet.WriteInteger((int) connectionFlags,
                                    version.isAtLeast(4, 1, 0) ? 4 : 2);
            }
#endif

            packet.WriteInteger(maxSinglePacket,
                                version.isAtLeast(4, 1, 0) ? 4 : 3);

            if (version.isAtLeast(4, 1, 1))
            {
                packet.WriteByte(8);
                packet.Write(new byte[23]);
            }

            Authenticate();

            // if we are using compression, then we use our CompressedStream class
            // to hide the ugliness of managing the compression
            if ((connectionFlags & ClientFlags.COMPRESS) != 0)
                stream = new MySqlStream(baseStream, encoding, true);

            // give our stream the server version we are connected to.  
            // We may have some fields that are read differently based 
            // on the version of the server we are connected to.
            packet.Version = version;
            stream.MaxBlockSize = maxSinglePacket;

            isOpen = true;
        }

#if !CF

        #region SSL

        private void StartSSL()
        {
            RemoteCertificateValidationCallback sslValidateCallback =
                new RemoteCertificateValidationCallback(ServerCheckValidation);
            SslStream ss = new SslStream(baseStream, true, sslValidateCallback, null);
            X509CertificateCollection certs = new X509CertificateCollection();
            ss.AuthenticateAsClient(Settings.Server, certs, SslProtocols.Default, false);
            baseStream = ss;
            stream = new MySqlStream(ss, encoding, false);
            stream.SequenceByte = 2;

        }

        private bool ServerCheckValidation(object sender, X509Certificate certificate,
                                                  X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (Settings.SslMode == MySqlSslMode.Prefered ||
                Settings.SslMode == MySqlSslMode.Required)
            {
                //Tolerate all certificate errors.
                return true;
            }

            if (Settings.SslMode == MySqlSslMode.VerifyCA && 
                sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                // Tolerate name mismatch in certificate, if full validation is not requested.
                return true;
            }

            return false;
        }


        #endregion

#endif

        #region Authentication

        /// <summary>
        /// Return the appropriate set of connection flags for our
        /// server capabilities and our user requested options.
        /// </summary>
        private void SetConnectionFlags()
        {
            // allow load data local infile
            ClientFlags flags = ClientFlags.LOCAL_FILES;

            if (!Settings.UseAffectedRows)
                flags |= ClientFlags.FOUND_ROWS;

            if (version.isAtLeast(4, 1, 1))
            {
                flags |= ClientFlags.PROTOCOL_41;
                // Need this to get server status values
                flags |= ClientFlags.TRANSACTIONS;

                // user allows/disallows batch statements
                if (connectionString.AllowBatch)
                    flags |= ClientFlags.MULTI_STATEMENTS;

                // We always allow multiple result sets
                flags |= ClientFlags.MULTI_RESULTS;
            }
            else if (version.isAtLeast(4, 1, 0))
                flags |= ClientFlags.RESERVED;

            // if the server allows it, tell it that we want long column info
            if ((serverCaps & ClientFlags.LONG_FLAG) != 0)
                flags |= ClientFlags.LONG_FLAG;

            // if the server supports it and it was requested, then turn on compression
            if ((serverCaps & ClientFlags.COMPRESS) != 0 && connectionString.UseCompression)
                flags |= ClientFlags.COMPRESS;

            if (protocol > 9)
                flags |= ClientFlags.LONG_PASSWORD; // for long passwords
            else
                flags &= ~ClientFlags.LONG_PASSWORD;

            // did the user request an interactive session?
            if (Settings.InteractiveSession)
                flags |= ClientFlags.INTERACTIVE;

            // if the server allows it and a database was specified, then indicate
            // that we will connect with a database name
            if ((serverCaps & ClientFlags.CONNECT_WITH_DB) != 0 &&
                connectionString.Database != null && connectionString.Database.Length > 0)
                flags |= ClientFlags.CONNECT_WITH_DB;

            // if the server is requesting a secure connection, then we oblige
            if ((serverCaps & ClientFlags.SECURE_CONNECTION) != 0)
                flags |= ClientFlags.SECURE_CONNECTION;

            // if the server is capable of SSL and the user is requesting SSL
            if ((serverCaps & ClientFlags.SSL) != 0 && connectionString.SslMode != MySqlSslMode.None)
                flags |= ClientFlags.SSL;

            // if the server supports output parameters, then we do too
            //if ((serverCaps & ClientFlags.PS_MULTI_RESULTS) != 0)
                flags |= ClientFlags.PS_MULTI_RESULTS;

            connectionFlags = flags;
        }

        /// <summary>
        /// Perform an authentication against a 4.1.1 server
        /// </summary>
        private void Authenticate411()
        {
            if ((connectionFlags & ClientFlags.SECURE_CONNECTION) == 0)
                AuthenticateOld();

            packet.Write(Crypt.Get411Password(connectionString.Password, encryptionSeed));
            if ((connectionFlags & ClientFlags.CONNECT_WITH_DB) != 0 && connectionString.Database != null)
                packet.WriteString(connectionString.Database);

            stream.SendPacket(packet);

            // this result means the server wants us to send the password using
            // old encryption
            packet = stream.ReadPacket();
            if (packet.IsLastPacket)
            {
                packet.Clear();
                packet.WriteString(Crypt.EncryptPassword(
                                       connectionString.Password, encryptionSeed.Substring(0, 8), true));
                stream.SendPacket(packet);
                ReadOk(true);
            }
            else
                ReadOk(false);
        }

        private void AuthenticateOld()
        {
            packet.WriteString(Crypt.EncryptPassword(
                                   connectionString.Password, encryptionSeed, protocol > 9));
            if ((connectionFlags & ClientFlags.CONNECT_WITH_DB) != 0 && connectionString.Database != null)
                packet.WriteString(connectionString.Database);

            stream.SendPacket(packet);
            ReadOk(true);
        }

        public void Authenticate()
        {
            // write the user id to the auth packet
            packet.WriteString(connectionString.UserID);

            if (version.isAtLeast(4, 1, 1))
                Authenticate411();
            else
                AuthenticateOld();
        }

        #endregion

        public override void Reset()
        {
            stream.SequenceByte = 0;
            packet.Clear();
            packet.WriteByte((byte)DBCmd.CHANGE_USER);
            Authenticate();
        }

        /// <summary>
        /// Query is the method that is called to send all queries to the server
        /// </summary>
        public override void SendQuery(MySqlPacket queryPacket)
        {
            lastInsertId = -1;
            affectedRows = 0;
            if (Settings.Logging)
                Logger.LogCommand(DBCmd.QUERY, encoding.GetString(
                    queryPacket.Buffer, 5, queryPacket.Length-5));

            queryPacket.Buffer[4] = (byte)DBCmd.QUERY;
            ExecutePacket(queryPacket);

            // the server will respond in one of several ways with the first byte indicating
            // the type of response.
            // 0 == ok packet.  This indicates non-select queries
            // 0xff == error packet.  This is handled in stream.OpenPacket
            // > 0 = number of columns in select query
            // We don't actually read the result here since a single query can generate
            // multiple resultsets and we don't want to duplicate code.  See ReadResult
            // Instead we set our internal server status flag to indicate that we have a query waiting.
            // This flag will be maintained by ReadResult
            serverStatus |= ServerStatusFlags.AnotherQuery;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (isOpen)
                    {
                        packet.Clear();
                        packet.WriteByte((byte)DBCmd.QUIT);
                        ExecutePacket(packet);
                    }

                    if (stream != null)
                        stream.Close();
                    stream = null;
                }
                catch (Exception)
                {
                    // we are just going to eat any exceptions
                    // generated here
                }
            }

            base.Dispose(disposing);
        }

        public override bool Ping()
        {
            try
            {
                packet.Clear();
                packet.WriteByte((byte)DBCmd.PING);
                ExecutePacket(packet);
                ReadOk(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void ExecuteDirect(string sql)
        {
            MySqlPacket p = new MySqlPacket();
            p.WriteString(sql);
            SendQuery(p);
            ReadResult();
        }

        /// <summary>
        /// ReadResult will attempt to read a single result from the server.  Note that it is not 
        /// reading all the rows of the result set but simple determining what type of result it is
        /// and returning values appropriately.
        /// </summary>
        /// <returns>Number of columns in the resultset, 0 for non-selects, -1 for no more resultsets</returns>
        public override long ReadResult()
        {
            // if there is not another query or resultset, then return -1
            if ((serverStatus & (ServerStatusFlags.AnotherQuery | ServerStatusFlags.MoreResults)) == 0)
                return -1;

            try
            {
                packet = stream.ReadPacket();
            }
            catch (Exception)
            {
                serverStatus = 0;
                throw;
            }

            long fieldCount = packet.ReadFieldLength();
            if (fieldCount > 0)
                return fieldCount;

            if (-1 == fieldCount)
            {
                string filename = packet.ReadString();
                SendFileToServer(filename);

                return ReadResult();
            }

            // the code to read last packet will set these server status vars 
            // again if necessary.
            serverStatus &= ~(ServerStatusFlags.AnotherQuery |
                              ServerStatusFlags.MoreResults);
            affectedRows += (long)packet.ReadFieldLength();
            lastInsertId = packet.ReadFieldLength();
            if (version.isAtLeast(4, 1, 0))
            {
                serverStatus = (ServerStatusFlags) packet.ReadInteger(2);
                warningCount = packet.ReadInteger(2);
                if (packet.HasMoreData)
                {
                    packet.ReadLenString(); //TODO: server message
                }
            }
            return fieldCount;
        }

        /// <summary>
        /// Sends the specified file to the server. 
        /// This supports the LOAD DATA LOCAL INFILE
        /// </summary>
        /// <param name="filename"></param>
        private void SendFileToServer(string filename)
        {
            byte[] buffer = new byte[8196];

            long len = 0;
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    len = fs.Length;
                    while (len > 0)
                    {
                        int count = fs.Read(buffer, 4, (int)(len > 8192 ? 8192 : len));
                        stream.SendEntirePacketDirectly(buffer, count);
                        len -= count;
                    }
                    stream.SendEntirePacketDirectly(buffer, 0);
                }
            }
            catch (Exception ex)
            {
                throw new MySqlException("Error during LOAD DATA LOCAL INFILE", ex);
            }
        }

        public override bool SkipDataRow()
        {
            return FetchDataRow(-1, 0, 0);
        }

        private void ReadNullMap(int fieldCount)
        {
            // if we are binary, then we need to load in our null bitmap
            nullMap = null;
            byte[] nullMapBytes = new byte[(fieldCount + 9)/8];
            packet.ReadByte();
            packet.Read(nullMapBytes, 0, nullMapBytes.Length);
            nullMap = new BitArray(nullMapBytes);
        }

        public override IMySqlValue ReadColumnValue(int index, MySqlField field, IMySqlValue valObject)
        {
            long length = -1;
            bool isNull;

            if (nullMap != null)
                isNull = nullMap[index + 2];
            else
            {
                length = packet.ReadFieldLength();
                isNull = length == -1;
            }

            packet.Encoding = field.Encoding;
            packet.Version = Version;
            return valObject.ReadValue(packet, length, isNull);
        }

        public override void SkipColumnValue(IMySqlValue valObject)
        {
            int length = -1;
            if (nullMap == null)
            {
                length = packet.ReadFieldLength();
                if (length == -1) return;
            }
            if (length > -1)
                packet.Position += length;
            else
                valObject.SkipValue(packet);
        }

        public override MySqlField[] ReadColumnMetadata(int count)
        {
            MySqlField[] fields = new MySqlField[count];

            for (int i = 0; i < count; i++)
                fields[i] = GetFieldMetaData();

            ReadEOF();
            return fields;
        }


        private MySqlField GetFieldMetaData()
        {
            MySqlField field;

            stream.Encoding = encoding;
            if (version.isAtLeast(4, 1, 0))
                field = GetFieldMetaData41();
            else
            {
                packet = stream.ReadPacket();
                field = new MySqlField(connection);

                field.Encoding = encoding;
                field.TableName = packet.ReadLenString();
                field.ColumnName = packet.ReadLenString();
                field.ColumnLength = packet.ReadNBytes();
                MySqlDbType type = (MySqlDbType)packet.ReadNBytes();
                packet.ReadByte();
                ColumnFlags colFlags;
                if ((Flags & ClientFlags.LONG_FLAG) != 0)
                    colFlags = (ColumnFlags)packet.ReadInteger(2);
                else
                    colFlags = (ColumnFlags)packet.ReadByte();
                field.SetTypeAndFlags(type, colFlags);

                field.Scale = (byte)packet.ReadByte();
                if (!version.isAtLeast(3, 23, 15) && version.isAtLeast(3, 23, 0))
                    field.Scale++;
            }

            return field;
        }

        private MySqlField GetFieldMetaData41()
        {
            MySqlField field = new MySqlField(connection);

            packet = stream.ReadPacket();
            field.Encoding = encoding;
            field.CatalogName = packet.ReadLenString();
            field.DatabaseName = packet.ReadLenString();
            field.TableName = packet.ReadLenString();
            field.RealTableName = packet.ReadLenString();
            field.ColumnName = packet.ReadLenString();
            field.OriginalColumnName = packet.ReadLenString();
            packet.ReadByte();
            field.CharacterSetIndex = packet.ReadInteger(2);
            field.ColumnLength = packet.ReadInteger(4);
            MySqlDbType type = (MySqlDbType)packet.ReadByte();
            ColumnFlags colFlags;
            if ((Flags & ClientFlags.LONG_FLAG) != 0)
                colFlags = (ColumnFlags)packet.ReadInteger(2);
            else
                colFlags = (ColumnFlags)packet.ReadByte();

            field.SetTypeAndFlags(type, colFlags);

            field.Scale = (byte)packet.ReadByte();


            if (packet.HasMoreData)
            {
                packet.ReadInteger(2); // reserved
            }

            if (charSets != null && field.CharacterSetIndex != -1)
            {
                CharacterSet cs = CharSetMap.GetCharacterSet(Version, (string) charSets[field.CharacterSetIndex]);
                // starting with 6.0.4 utf8 has a maxlen of 4 instead of 3.  The old
                // 3 byte utf8 is utf8mb3
                if (cs.name.ToLower(System.Globalization.CultureInfo.InvariantCulture) == "utf-8" &&
                    Version.Major >= 6)
                    field.MaxLength = 4;
                else
                    field.MaxLength = cs.byteCount;
                field.Encoding = CharSetMap.GetEncoding(version, (string) charSets[field.CharacterSetIndex]);
            }

            return field;
        }

        private void ExecutePacket(MySqlPacket packetToExecute)
        {
            try
            {
                stream.SequenceByte = 0;
                stream.SendPacket(packetToExecute);
            }
            catch (MySqlException ex)
            {
                if (ex.IsFatal)
                {
                    isOpen = false;
                    Close();
                }
                throw;
            }
        }

        public void ExecuteStatement(MySqlPacket packetToExecute)
        {
            lastInsertId = -1;
            affectedRows = 0;
            packetToExecute.Buffer[4] = (byte)DBCmd.EXECUTE;
            ExecutePacket(packetToExecute);
            serverStatus |= ServerStatusFlags.AnotherQuery;
        }

        public override void ExecuteStatement(byte[] bytes)
        {
        }

        private void CheckEOF()
        {
            if (!packet.IsLastPacket)
                throw new MySqlException("Expected end of data packet");

            packet.ReadByte(); // read off the 254

            if (version.isAtLeast(3, 0, 0) && !version.isAtLeast(4, 1, 0))
                serverStatus = 0;
            if (packet.HasMoreData && version.isAtLeast(4, 1, 0))
            {
                warningCount = packet.ReadInteger(2);
                serverStatus = (ServerStatusFlags)packet.ReadInteger(2);

                // if we are at the end of this cursor based resultset, then we remove
                // the last row sent status flag so our next fetch doesn't abort early
                // and we remove this command result from our list of active CommandResult objects.
                //                if ((serverStatus & ServerStatusFlags.LastRowSent) != 0)
                //              {
                //                serverStatus &= ~ServerStatusFlags.LastRowSent;
                //              commandResults.Remove(lastCommandResult);
                //        }
            }
        }

        private void ReadEOF()
        {
            packet = stream.ReadPacket();
            CheckEOF();
        }

        public override int PrepareStatement(string sql, ref MySqlField[] parameters)
        {
            //TODO: check this
            //ClearFetchedRow();

            packet.Length = sql.Length*4 + 5;
            byte[] buffer = packet.Buffer;
            int len = encoding.GetBytes(sql, 0, sql.Length, packet.Buffer, 5);
            packet.Position = len + 5;
            buffer[4] = (byte)DBCmd.PREPARE;
            ExecutePacket(packet);

            packet = stream.ReadPacket();

            int marker = packet.ReadByte();
            if (marker != 0)
                throw new MySqlException("Expected prepared statement marker");

            int statementId = packet.ReadInteger(4);
            int numCols = packet.ReadInteger(2);
            int numParams = packet.ReadInteger(2);
            //TODO: find out what this is needed for
            packet.ReadInteger(3);
            if (numParams > 0)
            {
                parameters = ReadColumnMetadata(numParams);

                // we set the encoding for each parameter back to our connection encoding
                // since we can't trust what is coming back from the server
                for (int i = 0; i < parameters.Length; i++)
                    parameters[i].Encoding = encoding;
            }

            if (numCols > 0)
            {
                while (numCols-- > 0)
                {
                    packet = stream.ReadPacket();
                    //TODO: handle streaming packets
                }

                ReadEOF();
            }

            return statementId;
        }

        //		private void ClearFetchedRow() 
        //		{
        //			if (lastCommandResult == 0) return;

        //TODO
        /*			CommandResult result = (CommandResult)commandResults[lastCommandResult];
					result.ReadRemainingColumns();

					stream.OpenPacket();
					if (! stream.IsLastPacket)
						throw new MySqlException("Cursor reading out of sync");

					ReadEOF(false);
					lastCommandResult = 0;*/
        //		}

        /// <summary>
        /// FetchDataRow is the method that the data reader calls to see if there is another 
        /// row to fetch.  In the non-prepared mode, it will simply read the next data packet.
        /// In the prepared mode (statementId > 0), it will 
        /// </summary>
        public override bool FetchDataRow(int statementId, int pageSize, int columns)
        {
            /*			ClearFetchedRow();

						if (!commandResults.ContainsKey(statementId)) return false;

						if ( (serverStatus & ServerStatusFlags.LastRowSent) != 0)
							return false;

						stream.StartPacket(9, true);
						stream.WriteByte((byte)DBCmd.FETCH);
						stream.WriteInteger(statementId, 4);
						stream.WriteInteger(1, 4);
						stream.Flush();

						lastCommandResult = statementId;
							*/
            packet = stream.ReadPacket();
            if (packet.IsLastPacket)
            {
                CheckEOF();
                return false;
            }
            nullMap = null;
            if (statementId > 0)
                ReadNullMap(columns);

			return true;
		}

        public override void CloseStatement(int id)
        {
            packet.Clear();
            packet.WriteByte((byte)DBCmd.CLOSE_STMT);
            packet.WriteInteger((long)id, 4);
            stream.SequenceByte = 0;
            stream.SendPacket(packet);
        }

        /// <summary>
        /// Execution timeout, in milliseconds. When the accumulated time for network IO exceeds this value
        /// TimeoutException is thrown. This timeout needs to be reset for every new command
        /// </summary>
        /// 
        public override void ResetTimeout(int timeout)
        {
            stream.ResetTimeout(timeout);
        }

    }
 
}
