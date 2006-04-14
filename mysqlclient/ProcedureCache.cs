using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System.Collections;

namespace MySql.Data.MySqlClient
{
    class ProcedureCache
    {
        private Hashtable procHash;
        private Queue<int> hashQueue;
        private int maxSize;

        public ProcedureCache(int size)
        {
            maxSize = size;
            hashQueue = new Queue<int>(maxSize);
            procHash = new Hashtable(maxSize);
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
            int oldestHash = hashQueue.Dequeue();
            procHash.Remove(oldestHash);
        }

        private DataSet GetProcData(MySqlConnection connection, string spName)
        {
            int dotIndex = spName.IndexOf(".");
            string schema = spName.Substring(0, dotIndex);
            string name = spName.Substring(dotIndex+1, spName.Length-dotIndex-1);

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
