// Copyright (C) 2008-2009 Sun Microsystems, Inc.
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
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class UpdateTests : BaseEdmTest
	{
       [Test]
       public void UpdateAllRows()
       {
           using (testEntities context = new testEntities())
           {
               MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM toys", conn);
               object count = cmd.ExecuteScalar();

               foreach (Toy t in context.Toys)
                   t.Name = "Top";
               context.SaveChanges();

               cmd.CommandText = "SELECT COUNT(*) FROM Toys WHERE name='Top'";
               object newCount = cmd.ExecuteScalar();
               Assert.AreEqual(count, newCount);
           }
       }
    }
}