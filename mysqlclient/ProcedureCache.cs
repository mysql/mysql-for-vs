// Copyright (C) 2004-2006 MySQL AB
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

using MySql.Data.MySqlClient;
using System.Data;
using System.Collections;
using MySql.Data.Common;
using System.Diagnostics;
using System;
using System.Globalization;
#if NET20
using System.Collections.Generic;
#endif

namespace MySql.Data.MySqlClient
{
    class ProcedureCache
    {
        private Hashtable procHash;
#if NET20
        private Queue<int> hashQueue;
#else
        private Queue hashQueue;
#endif
        private uint maxSize;

        public ProcedureCache(uint size)
        {
            maxSize = size;
#if NET20
            hashQueue = new Queue<int>((int)maxSize);
#else
            hashQueue = new Queue(maxSize);
#endif
            procHash = new Hashtable((int)maxSize);
        }

        public DataSet GetProcedure(MySqlConnection conn, string spName)
        {
            int hash = spName.GetHashCode();
            DataSet ds = (DataSet)procHash[hash];
            if (ds == null)
            {
                ds = AddNew(conn, spName);
                conn.PerfMonitor.AddHardProcedureQuery();
            }
            else
                conn.PerfMonitor.AddSoftProcedureQuery();
            return ds;
        }

        private DataSet AddNew(MySqlConnection connection, string spName)
        {
            DataSet procData = GetProcData(connection, spName);
            if (maxSize > 0)
            {
                if (procHash.Keys.Count == maxSize)
                    TrimHash();
                int hash = spName.GetHashCode();
                procHash.Add(hash, procData);
                hashQueue.Enqueue(hash);
            }
            return procData;
        }

        private void TrimHash()
        {
#if NET20
            int oldestHash = hashQueue.Dequeue();
#else
            int oldestHash = (int)hashQueue.Dequeue();
#endif
            procHash.Remove(oldestHash);
        }

        private DataSet GetProcData(MySqlConnection connection, string spName)
        {
            int dotIndex = spName.IndexOf(".");
            string schema = spName.Substring(0, dotIndex);
            string name = spName.Substring(dotIndex + 1, spName.Length - dotIndex - 1);

            string[] restrictions = new string[4];
            restrictions[1] = schema;
            restrictions[2] = name;
            DataTable procTable = connection.GetSchema("procedures", restrictions);

            DataTable parametersTable = connection.GetSchema("procedure parameters",
                restrictions);

            DataSet ds = new DataSet();
            ds.Tables.Add(procTable);
            ds.Tables.Add(parametersTable);
            return ds;
        }
    }
}
