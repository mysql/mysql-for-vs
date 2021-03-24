// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Antlr.Runtime;
using Xunit;

namespace MySql.Parser.Tests
{
  
  public class AlterEvent
  {
    [Fact]
    public void Simple1()
    {
      Utility.ParseSql(@"ALTER EVENT no_such_event 
          ON SCHEDULE 
            EVERY '2:3' DAY_HOUR;", false);
    }

    [Fact]
    public void Simple2()
    {
      Utility.ParseSql(@"CREATE EVENT myevent
    ON SCHEDULE
      EVERY 6 HOUR
    COMMENT 'A sample comment.'
    DO
      UPDATE myschema.mytable SET mycol = mycol + 1;", false);
    }

    [Fact]
    public void Simple3()
    {
      Utility.ParseSql(@"ALTER EVENT myevent
    ON SCHEDULE
      EVERY 12 HOUR
    STARTS CURRENT_TIMESTAMP + INTERVAL 4 HOUR;", false);
    }

    [Fact]
    public void Simple4()
    {
      Utility.ParseSql(@"ALTER EVENT myevent
    ON SCHEDULE
      AT CURRENT_TIMESTAMP + INTERVAL 1 DAY
    DO
      TRUNCATE TABLE myschema.mytable;", false);
    }

    [Fact]
    public void Simple5()
    {
      Utility.ParseSql(@"ALTER EVENT myevent
    DISABLE;", false);
    }

    [Fact]
    public void Simple6()
    {
      Utility.ParseSql(@"ALTER EVENT myevent
    RENAME TO yourevent;", false);
    }

    [Fact]
    public void Simple7()
    {
      Utility.ParseSql(@"ALTER EVENT olddb.myevent
    RENAME TO newdb.myevent;", false);
    }
  }
}
