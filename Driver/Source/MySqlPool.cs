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

using System;
using MySql.Data.Common;
using System.Collections;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for MySqlPool.
	/// </summary>
	internal sealed class MySqlPool
	{
#if NET20
        private List<Driver> inUsePool;
        private Queue<Driver> idlePool;
#else
		private ArrayList inUsePool;
		private Queue idlePool;
#endif
		private MySqlConnectionStringBuilder settings;
		private uint minSize;
		private uint maxSize;
        private ProcedureCache procedureCache;

		public MySqlPool(MySqlConnectionStringBuilder settings)
		{
			minSize = settings.MinimumPoolSize;
			maxSize = settings.MaximumPoolSize;
			this.settings = settings;
#if NET20
            inUsePool = new List<Driver>((int)maxSize);
            idlePool = new Queue<Driver>((int)maxSize);
#else
			inUsePool =new ArrayList(maxSize);
			idlePool = new Queue(maxSize);
#endif

			// prepopulate the idle pool to minSize
			for (int i=0; i < minSize; i++) 
				CreateNewPooledConnection();

            procedureCache = new ProcedureCache(settings.ProcedureCacheSize);
		}

		public MySqlConnectionStringBuilder	Settings 
		{
			get { return settings; }
			set { settings = value; }
		}

        public ProcedureCache ProcedureCache
        {
            get { return procedureCache; }
        }

/*		private int CheckConnections() 
		{
			int freed = 0;
			lock (inUsePool.SyncRoot) 
			{
				for (int i=inUsePool.Count-1; i >= 0; i--) 
				{
					Driver d = (inUsePool[i] as Driver);
					if (! d.Ping()) 
					{
						inUsePool.RemoveAt(i);
						freed++;
					}
				}
			}
			return freed;
		}
*/
		private Driver CheckoutConnection()
		{
			lock((idlePool as ICollection).SyncRoot)
			{
				if (idlePool.Count == 0) return null;
				Driver driver = (Driver)idlePool.Dequeue();

				// if the user asks us to ping/reset pooled connections
				// do so now
				if (settings.ConnectionReset)
				{
					if (!driver.Ping())
					{
						driver.Close();
						return null;
					}
					driver.Reset();
				}

                lock ((inUsePool as ICollection).SyncRoot)
                {
					inUsePool.Add(driver);
				}
				return driver;
			}
		}

		private Driver GetPooledConnection()
		{
			while (true)
			{
				if (idlePool.Count > 0)
					return CheckoutConnection();

				// if idlepool == 0 and inusepool == max, then we can't create a new one
				if (inUsePool.Count == maxSize)
					return null;

				CreateNewPooledConnection();
			}
		}

		private void CreateNewPooledConnection()
		{
            lock ((idlePool as ICollection).SyncRoot)
                lock ((inUsePool as ICollection).SyncRoot)
                {
					// first we check if we are allowed to create another
					if ((inUsePool.Count + idlePool.Count) == maxSize)
						return;

					Driver driver = Driver.Create(settings);

					idlePool.Enqueue(driver);
				}
		}

		public void ReleaseConnection(Driver driver)
		{
            lock ((idlePool as ICollection).SyncRoot)
                lock ((inUsePool as ICollection).SyncRoot)
                {
					inUsePool.Remove(driver);
					if (driver.IsTooOld())
						driver.Close();
					else
						idlePool.Enqueue(driver);
				}
		}

        /// <summary>
        /// Removes a connection from the in use pool.  The only situations where this method 
        /// would be called are when a connection that is in use gets some type of fatal exception
        /// or when the connection is being returned to the pool and it's too old to be 
        /// returned.
        /// </summary>
        /// <param name="driver"></param>
        public void RemoveConnection(Driver driver)
        {
            lock ((inUsePool as ICollection).SyncRoot)
            {
                inUsePool.Remove(driver);
            }
        }

		public Driver GetConnection() 
		{
			Driver driver = null;

			int start = Environment.TickCount;
			uint ticks = settings.ConnectionTimeout * 1000;

			// wait timeOut seconds at most to get a connection
			while (driver == null && (Environment.TickCount - start) < ticks)
				driver = GetPooledConnection();
					 
			// if pool size is at maximum, then we must have reached our timeout so we simply
			// throw our exception
			if (driver == null)
				throw new MySqlException("error connecting: Timeout expired.  The timeout period elapsed " + 
					"prior to obtaining a connection from the pool.  This may have occurred because all " +
					"pooled connections were in use and max pool size was reached.");

			return driver;
		}

	}
}
