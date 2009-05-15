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
using System.Data;
#if NET20
using System.Collections.Generic;
using MySql.Data.MySqlClient.Properties;
#endif

namespace MySql.Data.MySqlClient
{
    internal class ProcedureCache
    {
        private Hashtable procHash;
#if NET20
		private Queue<int> hashQueue;
#else
        private Queue hashQueue;
#endif
        private int maxSize;

        public ProcedureCache(int size)
        {
            maxSize = size;
#if NET20
			hashQueue = new Queue<int>(maxSize);
#else
            hashQueue = new Queue(maxSize);
#endif
            procHash = new Hashtable(maxSize);
        }

        public DataSet GetProcedure(MySqlConnection conn, string spName)
        {
            int hash = spName.GetHashCode();

            DataSet ds = null;
            lock (procHash.SyncRoot)
            {
                ds = (DataSet)procHash[hash];
            }
            if (ds == null)
            {
                ds = AddNew(conn, spName);
#if !CF
                conn.PerfMonitor.AddHardProcedureQuery();
#endif
                if (conn.Settings.Logging)
                    Logger.LogInformation(String.Format(
                                              Resources.HardProcQuery, spName));
            }
            else
            {
#if !CF
                conn.PerfMonitor.AddSoftProcedureQuery();
#endif
                if (conn.Settings.Logging)
                    Logger.LogInformation(String.Format(
                                              Resources.SoftProcQuery, spName));
            }
            return ds;
        }

        private DataSet AddNew(MySqlConnection connection, string spName)
        {
            DataSet procData = GetProcData(connection, spName);
            if (maxSize > 0)
            {
                int hash = spName.GetHashCode();
                lock (procHash.SyncRoot)
                {
                    if (procHash.Keys.Count >= maxSize)
                        TrimHash();
                    if (!procHash.ContainsKey(hash))
                    {
                        procHash[hash] = procData;
                        hashQueue.Enqueue(hash);
                    }
                }
            }
            return procData;
        }

        private void TrimHash()
        {
#if NET20
			int oldestHash = hashQueue.Dequeue();
#else
            int oldestHash = (int) hashQueue.Dequeue();
#endif
            procHash.Remove(oldestHash);
        }

        private static DataSet GetProcData(MySqlConnection connection, string spName)
        {
            int dotIndex = spName.IndexOf(".");
            string schema = spName.Substring(0, dotIndex);
            string name = spName.Substring(dotIndex + 1, spName.Length - dotIndex - 1);

            string[] restrictions = new string[4];
            restrictions[1] = schema.Length > 0 ? schema : connection.CurrentDatabase();
            restrictions[2] = name;
            DataTable procTable = connection.GetSchema("procedures", restrictions);
            if (procTable.Rows.Count > 1)
                throw new MySqlException(Resources.ProcAndFuncSameName);
            if (procTable.Rows.Count == 0)
                throw new MySqlException(String.Format(Resources.InvalidProcName, name, schema));

            DataSet ds = new DataSet();
            ds.Tables.Add(procTable);

            // we don't use GetSchema here because that would cause another
            // query of procedures and we don't need that since we already
            // know the procedure we care about.
            ISSchemaProvider isp = new ISSchemaProvider(connection);
            string[] rest = isp.CleanRestrictions(restrictions);
            if (isp.CanRetrieveProcedureParameters())
            {
                DataTable parametersTable = isp.GetProcedureParameters(rest, procTable);
                ds.Tables.Add(parametersTable);
            }

            return ds;
        }
    }
}