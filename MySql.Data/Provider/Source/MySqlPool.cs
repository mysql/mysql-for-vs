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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MySql.Data.MySqlClient.Properties;

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
        private bool beingCleared;
        private int available;
        private AutoResetEvent autoEvent;

		public MySqlPool(MySqlConnectionStringBuilder settings)
		{
			minSize = settings.MinimumPoolSize;
			maxSize = settings.MaximumPoolSize;

            available = (int)maxSize;
            autoEvent = new AutoResetEvent(false);

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
            for (int i = 0; i < minSize; i++)
                idlePool.Enqueue(CreateNewPooledConnection());

            procedureCache = new ProcedureCache((int)settings.ProcedureCacheSize);

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
        /// It is assumed that this method is only called from inside an active lock.
        /// </summary>
        private Driver GetPooledConnection()
		{
            Driver driver = null;

            // if we don't have an idle connection but we have room for a new
            // one, then create it here.
            lock ((idlePool as ICollection).SyncRoot)
            {
                if (HasIdleConnections)
                    driver = (Driver)idlePool.Dequeue();
            }

            if (driver != null)
            {
                // first check to see that the server is still alive
                if (!driver.Ping())
                {
                    driver.Close();
                    driver = null;
                }
                else if (settings.ConnectionReset)
                    // if the user asks us to ping/reset pooled connections
                    // do so now
                    driver.Reset();
            }
            if (driver == null)
                driver = CreateNewPooledConnection();

            Debug.Assert(driver != null);
            lock ((inUsePool as ICollection).SyncRoot)
            {
                inUsePool.Add(driver);
            }
            return driver;
        }

        /// <summary>
        /// It is assumed that this method is only called from inside an active lock.
        /// </summary>
		private Driver CreateNewPooledConnection()
		{
            Debug.Assert((maxSize - NumConnections) > 0, "Pool out of sync.");

            Driver driver = Driver.Create(settings);
            driver.Pool = this;
            return driver;
        }

		public void ReleaseConnection(Driver driver)
		{
            lock ((inUsePool as ICollection).SyncRoot)
            {
                if (inUsePool.Contains(driver))
                    inUsePool.Remove(driver);
            }

            if (driver.IsTooOld() || beingCleared)
            {
                driver.Close();
                Debug.Assert(!idlePool.Contains(driver));
            }
            else
            {
                lock ((idlePool as ICollection).SyncRoot)
                {
                    idlePool.Enqueue(driver);
                }
            }

            Interlocked.Increment(ref available);
            autoEvent.Set();
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
                if (inUsePool.Contains(driver))
                {
                    inUsePool.Remove(driver);
                    Interlocked.Increment(ref available);
                    autoEvent.Set();
                }
            }

            // if we are being cleared and we are out of connections then have
            // the manager destroy us.
            if (beingCleared && NumConnections == 0)
                MySqlPoolManager.RemoveClearedPool(this);
        }

        private Driver TryToGetDriver()
        {
            int count = Interlocked.Decrement(ref available);
            if (count < 0)
            {
                Interlocked.Increment(ref available);
                return null;
            }
            try
            {
                Driver driver = GetPooledConnection();
                return driver;
            }
            catch (Exception ex)
            {
                if (settings.Logging)
                    Logger.LogException(ex);
                Interlocked.Increment(ref available);
                throw;
            }
        }

		public Driver GetConnection() 
		{
			int fullTimeOut = (int)settings.ConnectionTimeout * 1000;
            int timeOut = fullTimeOut;

            DateTime start = DateTime.Now;

            while (timeOut > 0)
            {
                Driver driver = TryToGetDriver();
                if (driver != null) return driver;

                // We have no tickets right now, lets wait for one.
                if (!autoEvent.WaitOne(timeOut, false)) break;
                timeOut = fullTimeOut - (int)DateTime.Now.Subtract(start).TotalMilliseconds;
            }
            throw new MySqlException(Resources.TimeoutGettingConnection);
		}

        /// <summary>
        /// Clears this pool of all idle connections and marks this pool and being cleared
        /// so all other connections are closed when they are returned.
        /// </summary>
        internal void Clear()
        {
            lock ((idlePool as ICollection).SyncRoot)
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
