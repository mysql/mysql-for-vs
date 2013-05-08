﻿// Copyright © 2013 Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MySql.Data.MySqlClient.Tests
{
  public class MySQLHelperTests : IUseFixture<SetUpClass>, IDisposable
  {
    private SetUpClass st;

    public void SetFixture(SetUpClass data)
    {
      st = data;      
    }

    public void Dispose()
    {
      st.execSQL("DROP TABLE IF EXISTS TEST");
    }

    /// <summary>
    /// Bug #62585	MySql Connector/NET 6.4.3+ Doesn't escape quotation mark (U+0022)
    /// </summary>
    [Fact]
    public void EscapeStringMethodCanEscapeQuotationMark()
    {
      st.execSQL("CREATE TABLE Test (id int NOT NULL, name VARCHAR(100))");

      MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES (1,\"firstname\")", st.conn);
      cmd.ExecuteNonQuery();

      cmd = new MySqlCommand("UPDATE test SET name = \"" + MySqlHelper.EscapeString("test\"name\"") + "\";", st.conn);
      cmd.ExecuteNonQuery();

      cmd.CommandText = "SELECT name FROM Test WHERE id=1";
      string name = (string)cmd.ExecuteScalar();

      Assert.True("test\"name\"" == name, "Update result with quotation mark");
    }
  }
}
