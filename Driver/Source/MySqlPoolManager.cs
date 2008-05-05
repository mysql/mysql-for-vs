// Copyright (C) 2004-2007 MySQL AB
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

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Summary description for MySqlPoolManager.
    /// </summary>
    internal class MySqlPoolManager
    {
        private static Hashtable pools;

        static MySqlPoolManager()
        {
            pools = new Hashtable();
        }

        public static MySqlPool GetPool(MySqlConnectionStringBuilder settings)
        {
            string text = settings.GetConnectionString(true);

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
            string key = driver.Settings.GetConnectionString(true);
            MySqlPool pool = (MySqlPool) pools[key];

            // if we can't find the pool but we did get a thread id then we assume
            // something is bad wrong.  If we didn't get a thread id then we assume that
            // the driver connection info was bogus and that led to the pool failing
            // to create
            if (pool == null)
            {
                if (driver.ThreadID != -1)
                    throw new MySqlException("Pooling exception: Unable to find original pool for connection");
            }
            else
                pool.RemoveConnection(driver);
        }

        public static void ReleaseConnection(Driver driver)
        {
            string key = driver.Settings.GetConnectionString(true);
            MySqlPool pool = (MySqlPool) pools[key];

            // if we can't find the pool but we did get a thread id then we assume
            // something is bad wrong.  If we didn't get a thread id then we assume that
            // the driver connection info was bogus and that led to the pool failing
            // to create
            if (pool == null)
            {
                if (driver.ThreadID != -1)
                    throw new MySqlException("Pooling exception: Unable to find original pool for connection");
            }
            else
                pool.ReleaseConnection(driver);
        }
    }
}