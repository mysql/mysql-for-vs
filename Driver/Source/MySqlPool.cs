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

using System;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
#if CF
using MySql.Data.Common;
#endif

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
        private Object lockObject;
        private Semaphore poolGate;
        private bool beingCleared;

		public MySqlPool(MySqlConnectionStringBuilder settings)
		{
			minSize = settings.MinimumPoolSize;
			maxSize = settings.MaximumPoolSize;
            if (minSize > maxSize)
                minSize = maxSize;
			this.settings = settings;
#if NET20
            inUsePool = new List<Driver>((int)maxSize);
            idlePool = new Queue<Driver>((int)maxSize);
#else
			inUsePool =new ArrayList((int)maxSize);
			idlePool = new Queue((int)maxSize);
#endif

			// prepopulate the idle pool to minSize
			for (int i=0; i < minSize; i++) 
				CreateNewPooledConnection();

            procedureCache = new ProcedureCache((int)settings.ProcedureCacheSize);
            poolGate = new Semaphore((int)maxSize, (int)maxSize);

            // we don't really need to create this but it makes the code a bit cleaner
            lockObject = new Object();

            beingCleared = false;
        }

        #region Properties

        public MySqlConnectionStringBuilder	Settings 
		{
			get { return settings; }
			set { settings = value; }
		}

        public ProcedureCache ProcedureCache
        {
            get { return procedureCache; }
        }

        /// <summary>
        /// It is assumed that this property will only be used from inside an active
        /// lock.
        /// </summary>
        private bool HasIdleConnections
        {
            get { return idlePool.Count > 0; }
        }

        private int NumConnections
        {
            get { return idlePool.Count + inUsePool.Count; }
        }

        /// <summary>
        /// Indicates whether this pool is being cleared.
        /// </summary>
        public bool BeingCleared
        {
            get { return beingCleared; }
        }

        #endregion

        /// <summary>
        /// CheckoutConnection handles the process of pulling a driver
        /// from the idle pool, possibly resetting its state,
        /// and adding it to the in use pool.  We assume that this method is only
        /// called inside an active lock so there is no need to acquire a new lock.
        /// </summary>
        /// <returns>An idle driver object</returns>
		private Driver CheckoutConnection()
		{
			Driver driver = (Driver)idlePool.Dequeue();

            // first check to see that the server is still alive
            if (!driver.Ping())
            {
                driver.Close();
                return null;
            }

            // if the user asks us to ping/reset pooled connections
			// do so now
			if (settings.ConnectionReset)
				driver.Reset();

			inUsePool.Add(driver);

			return driver;
		}

        /// <summary>
        /// It is assumed that this method is only called from inside an active lock.
        /// </summary>
        private Driver GetPooledConnection()
		{
            while (true)
            {
                // if we don't have an idle connection then we must have room
                // for a new connection since poolGate let us in
                if (!HasIdleConnections)
                    CreateNewPooledConnection();

                Driver d = CheckoutConnection();
                if (d != null)
                    return d;
            }
		}

        /// <summary>
        /// It is assumed that this method is only called from inside an active lock.
        /// </summary>
		private void CreateNewPooledConnection()
		{
            Driver driver = Driver.Create(settings);
            driver.Pool = this;
            idlePool.Enqueue(driver);
        }

		public void ReleaseConnection(Driver driver)
		{
            lock (lockObject)
            {
                if (inUsePool.Contains(driver))
                    inUsePool.Remove(driver);

                if (driver.IsTooOld() || beingCleared)
                {
                    driver.Close();
                    Debug.Assert(!idlePool.Contains(driver));
                }
                else
                    idlePool.Enqueue(driver);

                // we now either have a connection available or have room to make
                // one so we release one slot in our semaphore
                poolGate.Release();
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
            lock (lockObject)
            {
                if (inUsePool.Contains(driver))
                {
                    inUsePool.Remove(driver);
                    poolGate.Release();
                }

                // if we are being cleared and we are out of connections then have
                // the manager destroy us.
                if (beingCleared && NumConnections == 0)
                    MySqlPoolManager.RemoveClearedPool(this);
            }
        }

		public Driver GetConnection() 
		{
			int ticks = (int)settings.ConnectionTimeout * 1000;

            // wait till we are allowed in
            bool allowed = poolGate.WaitOne(ticks, false);
            if (! allowed)
                throw new MySqlException(Resources.TimeoutGettingConnection);

            // if we get here, then it means that we either have an idle connection
            // or room to make a new connection
            lock (lockObject)
            {
                try
                {
                    Driver d = GetPooledConnection();
                    return d;
                }
                catch (Exception ex)
                {
                    if (settings.Logging)
                        Logger.LogException(ex);
                    poolGate.Release();
                    throw;
                }
            }
		}

        /// <summary>
        /// Clears this pool of all idle connections and marks this pool and being cleared
        /// so all other connections are closed when they are returned.
        /// </summary>
        internal void Clear()
        {
            lock (lockObject)
            {
                // first, mark ourselves as being cleared
                beingCleared = true;

                // then we remove all connections sitting in the idle pool
                while (idlePool.Count > 0)
                {
                    Driver d = idlePool.Dequeue();
                    d.Close();
                }

                // there is nothing left to do here.  Now we just wait for all
                // in use connections to be returned to the pool.  When they are
                // they will be closed.  When the last one is closed, the pool will
                // be destroyed.
            }
        }
	}
}
