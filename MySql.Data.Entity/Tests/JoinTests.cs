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
using System.Linq;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class JoinTests : BaseEdmTest
    {
        //[Test]
/*        public void SimpleJoin()
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
                        on b.Author_id equals a.Id
                        select new
                        {
                            bookId = b.Id,
                            bookName = b.Name,
                            authorName = a.Name
                        };

                int i = 0;
                foreach (var o in q)
                    Assert.AreEqual(dt.Rows[i++][0], o.bookId);
                Assert.AreEqual(dt.Rows.Count, i);
            }
        }*/

        /*[Test]
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
                        on b.Author_id equals a.Id
                        where b.Pages > 300
                        select new
                        {
                            bookId = b.Id,
                            bookName = b.Name,
                            authorName = a.Name
                        };
                string s = q.ToTraceString();

                int i = 0;
                foreach (var o in q)
                    Assert.AreEqual(dt.Rows[i++][0], o.bookId);
                Assert.AreEqual(dt.Rows.Count, i);
            }
        }*/

//        [Test]
  //      public void UnionSelects()
    //    {
//            MySqlDataAdapter da = new MySqlDataAdapter(
//                @"SELECT b.id,b.name,a.name as author_name from books b JOIN
//                    authors a ON b.author_id=a.id WHERE b.pages > 300", conn);
//            DataTable dt = new DataTable();
//            da.Fill(dt);

            //using (testEntities context = new testEntities())
            //{
            //    var OldAuthors = from a in context.Authors
            //                     where a.Age > 50
            //                     select a;
            //    var BigBooks = from b in context.Books
            //                   where b.Pages > 300
            //                   select b;
            //    var q = from o in OldAuthors
            //            join p in BigBooks
            //            on o.Id equals p.Author_id
            //            where p.Name == "Insomnia"
            //            select o;
            //    string s = q.ToTraceString();

                //var q = from b in BigBooks
                //        join a in OldAuthors
                //        on b.author equals a.authorId
                //        select new
                //        {
                //            bookId = b.bookId,
                //            bookName = b.bookName,
                //            authorName = a.authorName
                //        };

                //int i = 0;
                //foreach (var o in q)
                //    Assert.AreEqual(dt.Rows[i++][0], o.bookId);
                //Assert.AreEqual(dt.Rows.Count, i);
//            }
  //      }
    }
}