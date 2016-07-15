// Copyright © 2013, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using System.Text;
using Xunit;

namespace MySql.Parser.Tests.DML
{
  public class Update
  {
    [Fact]
    public void UpdateSimpleTest()
    {
      Utility.ParseSql(
        @"update Table1
          set col1 = 20, col2 = a and b, col3 = col4, col4 = true, col5 = null, col6 = 'string'
          where Id = 30");
    }

    [Fact]
    public void UpdateMoreComplexText()
    {
      Utility.ParseSql(
        @"update low_priority ignore T set deleted = 1, ToArchive = false, DateStamp = '10-10-2000'
        where DateCreated < '09-10-2000' order by Id asc limit 1000");
      Utility.ParseSql(
        @"update low_priority ignore T set deleted = 1, ToArchive = false, DateStamp = '10-10-2000'
        where DateCreated < '09-10-2000' order by Id asc limit 1000 offset 113", true );
    }

    [Fact]
    public void UpdateMultiTable()
    {
      Utility.ParseSql(
        @"update T1, T2 inner join T3 on T2.KeyId = T3.ForeignKeyId
          set Col1 = 3.1416, T1.Col3 = T2.Col3, T1.Col2 = T3.Col2
          where ( T1.Id = T2.Id ) ");
    }

[Fact]
        public void Subquery()
        {
          Utility.ParseSql(@"UPDATE books SET author = ( SELECT author FROM volumes WHERE volumes.id = books.volume_id );");
        }

        [Fact]
        public void Subquery2()
        {
          Utility.ParseSql(
@"UPDATE people,
(SELECT count(*) as votecount, person_id
FROM votes GROUP BY person_id) as tally
SET people.votecount = tally.votecount
WHERE people.person_id = tally.person_id;");
        }

        [Fact]
        public void WithPartition_55()
        {
          string result = Utility.ParseSql(@"UPDATE employees PARTITION (p0) SET store_id = 2 WHERE fname = 'Jill';", true, new Version(5, 5, 50));
          Assert.True(result.IndexOf("'(' (opening parenthesis) is not valid input at this position", StringComparison.OrdinalIgnoreCase) != -1);
        }

        [Fact]
        public void WithPartition_56()
        {
          Utility.ParseSql(@"UPDATE employees PARTITION (p0) SET store_id = 2 WHERE fname = 'Jill';", false, new Version(5, 6, 31));
        }
  }
}
