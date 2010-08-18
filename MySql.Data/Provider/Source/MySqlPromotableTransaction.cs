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
using System.Transactions;
using System.Collections;
using System.Data;

namespace MySql.Data.MySqlClient
{
    internal sealed class MySqlPromotableTransaction : IPromotableSinglePhaseNotification, ITransactionPromoter
    {
        private MySqlConnection connection;
        private Transaction baseTransaction;
        private MySqlTransaction simpleTransaction;

        public MySqlPromotableTransaction(MySqlConnection connection, Transaction baseTransaction)
        {
            this.connection = connection;
            this.baseTransaction = baseTransaction;
        }

        public Transaction BaseTransaction
        {
            get { return baseTransaction; }
        }

        void IPromotableSinglePhaseNotification.Initialize()
        {
            string valueName = Enum.GetName(
                typeof(System.Transactions.IsolationLevel), baseTransaction.IsolationLevel);
            System.Data.IsolationLevel dataLevel = (System.Data.IsolationLevel)Enum.Parse(
                typeof(System.Data.IsolationLevel), valueName);
            simpleTransaction = connection.BeginTransaction(dataLevel);
        }

        void IPromotableSinglePhaseNotification.Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            // prevent commands in main thread to run concurrently
            Driver driver = connection.driver;
            lock (driver)
            {
                while (connection.Reader != null)
                {
                    // wait for reader to finish. Maybe we should not wait 
                    // forever and cancel it after some time?
                    System.Threading.Thread.Sleep(100);
                }
                simpleTransaction.Rollback();
                singlePhaseEnlistment.Aborted();
                DriverTransactionManager.RemoveDriverInTransaction(baseTransaction);

                driver.CurrentTransaction = null;

                if (connection.State == ConnectionState.Closed)
                    connection.CloseFully();
            }
        }

        void IPromotableSinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            simpleTransaction.Commit();
            singlePhaseEnlistment.Committed();
            DriverTransactionManager.RemoveDriverInTransaction(baseTransaction);

            connection.driver.CurrentTransaction = null;

            if (connection.State == ConnectionState.Closed)
                connection.CloseFully();
        }

        byte[] ITransactionPromoter.Promote()
        {
            throw new NotSupportedException();
        }
    }

    internal class DriverTransactionManager
    {
        private static Hashtable driversInUse = new Hashtable();

        public static Driver GetDriverInTransaction(Transaction transaction)
        {
            lock (driversInUse.SyncRoot)
            {
                Driver d = (Driver)driversInUse[transaction.GetHashCode()];
                return d;
            }
        }

        public static void SetDriverInTransaction(Driver driver)
        {
            lock (driversInUse.SyncRoot)
            {
                driversInUse[driver.CurrentTransaction.BaseTransaction.GetHashCode()] = driver;
            }
        }

        public static void RemoveDriverInTransaction(Transaction transaction)
        {
            lock (driversInUse.SyncRoot)
            {
                driversInUse.Remove(transaction.GetHashCode());
            }
        }
    }
}

