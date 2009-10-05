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

using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Summary description for MySqlPoolManager.
    /// </summary>
    internal class MySqlPoolManager
    {
        private static Hashtable pools = new Hashtable();
        private static List<MySqlPool> clearingPools = new List<MySqlPool>();

        // Timeout in seconds, after which an unused (idle) connection 
        // should be closed.
        static internal int maxConnectionIdleTime = 180;


        private static Timer timer = new Timer(new TimerCallback(CleanIdleConnections),
            null, maxConnectionIdleTime*1000, maxConnectionIdleTime*1000);

        public static MySqlPool GetPool(MySqlConnectionStringBuilder settings)
        {
            string text = settings.ConnectionString;

            lock (pools.SyncRoot)
            {
                MySqlPool pool = (pools[text] as MySqlPool);

                if (pool == null)
                {
                    pool = new MySqlPool(settings);
                    pools.Add(text, pool);
                }
                else
                    pool.Settings = settings;

                return pool;
            }
        }

        public static void RemoveConnection(Driver driver)
        {
            Debug.Assert(driver != null);

			MySqlPool pool = driver.Pool;
			if (pool == null) return;

            pool.RemoveConnection(driver);
        }

        public static void ReleaseConnection(Driver driver)
        {
            Debug.Assert(driver != null);

			MySqlPool pool = driver.Pool;
			if (pool == null) return;
			
            pool.ReleaseConnection(driver);
        }

        public static void ClearPool(MySqlConnectionStringBuilder settings)
        {
            Debug.Assert(settings != null);

            string text = settings.ConnectionString;
            ClearPoolByText(text);
        }

        private static void ClearPoolByText(string key)
        {
            lock (pools.SyncRoot)
            {
                // if pools doesn't have it, then this pool must already have been cleared
                if (!pools.ContainsKey(key)) return;

                // add the pool to our list of pools being cleared
                MySqlPool pool = (pools[key] as MySqlPool);
                clearingPools.Add(pool);

                // now tell the pool to clear itself
                pool.Clear();

                // and then remove the pool from the active pools list
                pools.Remove(key);
            }
        }

		public static void ClearAllPools()
		{
			lock (pools.SyncRoot)
			{
				// Create separate keys list.
				List<string> keys = new	List<string>(pools.Count);

				foreach (string key in pools.Keys)
					keys.Add(key);

				// Remove all pools by key.
				foreach (string key in keys)
					ClearPoolByText(key);
			}
		}

        public static void RemoveClearedPool(MySqlPool pool)
        {
            Debug.Assert(clearingPools.Contains(pool));
            clearingPools.Remove(pool);
        }

        /// <summary>
        /// Remove drivers that have been idle for too long.
        /// </summary>
        public static void CleanIdleConnections(object obj)
        {
            List<Driver> oldDrivers = new List<Driver>();
            lock (pools.SyncRoot)
            {
                foreach (string key in pools.Keys)
                {
                    MySqlPool pool = (pools[key] as MySqlPool);
                    oldDrivers.AddRange(pool.RemoveOldIdleConnections());
                }
            }
            foreach(Driver driver in oldDrivers)
            {
                driver.Close();
            }
        }
    }
}