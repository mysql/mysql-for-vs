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
using System.Transactions;

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
            simpleTransaction = connection.BeginTransaction();
        }

        void IPromotableSinglePhaseNotification.Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            simpleTransaction.Rollback();
            singlePhaseEnlistment.Aborted();
            connection.CurrentTransaction = null;
        }

        void IPromotableSinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            simpleTransaction.Commit();
            singlePhaseEnlistment.Committed();
            connection.CurrentTransaction = null;
        }

//TODO:  remove this once Mono corrects this bug
#if MONO
	    Transaction IPromotableSinglePhaseNotification.Promote()
	    {
			throw new NotSupportedException();
		}
#endif		

        byte[] ITransactionPromoter.Promote()
        {
            throw new NotSupportedException();
        }
    }
}

