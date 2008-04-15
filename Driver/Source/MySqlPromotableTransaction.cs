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
            simpleTransaction.Rollback();
            singlePhaseEnlistment.Aborted();
            DriverTransactionManager.RemoveDriverInTransaction(baseTransaction);

            connection.driver.CurrentTransaction = null;

            if (connection.State == ConnectionState.Closed)
                connection.CloseFully();
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

