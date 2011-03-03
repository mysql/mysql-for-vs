// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using MySql.Data.Entity.Tests.Properties;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class JoinTests : BaseEdmTest
    {
        [Test]
        public void SimpleJoin()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                @"SELECT b.id,b.name,a.name as author_name from books b JOIN
                    authors a ON b.author_id=a.id", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                var q = from b in context.Books
                        join a in context.Authors
                        on b.Author.Id equals a.Id
                        select new
                        {
                            bookId = b.Id,
                            bookName = b.Name,
                            authorName = a.Name
                        };

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.SimpleJoin);

                int i = 0;
                foreach (var o in q)
                    Assert.AreEqual(dt.Rows[i++][0], o.bookId);
                Assert.AreEqual(dt.Rows.Count, i);
            }
        }

        [Test]
        public void SimpleJoinWithPredicate()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                @"SELECT b.id,b.name,a.name as author_name from books b JOIN
                    authors a ON b.author_id=a.id WHERE b.pages > 300", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                var q = from b in context.Books
                        join a in context.Authors
                        on b.Author.Id equals a.Id
                        where b.Pages > 300
                        select new
                        {
                            bookId = b.Id,
                            bookName = b.Name,
                            authorName = a.Name
                        };

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.SimpleJoinWithPredicate);

                int i = 0;
                foreach (var o in q)
                    Assert.AreEqual(dt.Rows[i++][0], o.bookId);
                Assert.AreEqual(dt.Rows.Count, i);
            }
        }
    }
}