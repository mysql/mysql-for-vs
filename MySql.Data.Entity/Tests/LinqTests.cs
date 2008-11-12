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
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Linq;

namespace MySql.Data.Entity.Tests
{
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
	[TestFixture]
	public class LinqTests : BaseEdmTest
	{
        public override void Setup()
        {
            base.Setup();
        }

        private EntityConnection GetEntityConnection()
        {
            string connectionString = String.Format(
                "metadata=TestDB.csdl|TestDB.msl|TestDB.ssdl;provider=MySql.Data.MySqlClient; provider connection string=\"{0}\"", GetConnectionString(true));
            EntityConnection connection = new EntityConnection(connectionString);
            return connection;
        }

        [Test]
        public void SelectSingleColumnNoParameter()
        {
/*            using (TestModel ctxt = new TestModel())
            {
                var query = from c in db.Employee where c.LastName == "Flintstone" select c;
                foreach (Employee c in query)
                    Console.WriteLine("    {0}", c.LastName);
            }*/
        }

    }
}