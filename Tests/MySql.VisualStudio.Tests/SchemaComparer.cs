// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.MySqlClient;


namespace MySql.VisualStudio.Tests
{
  public class SchemaComparerTests
  {
    [Fact]
    public void Comparison1()
    {
      MySqlConnection src = new MySqlConnection( "server=localhost; userid=root; database=DbCmp1; port=3305;" );
      MySqlConnection dst = new MySqlConnection("server=localhost; userid=root; database=DbCmp2; port=3305;");

      MySqlConnection mon = new MySqlConnection("server=localhost; userid=root; database=mysql; port=3305;");
      try
      {
        mon.Open();
        MySqlScript scr = new MySqlScript(mon);
        scr.Query = @"delimiter //
          drop database if exists DbCmp1 //
          drop database if exists DbCmp2 //
          create database DbCmp1 //
          create database DbCmp2 //";
        scr.Execute();
      }
      finally
      {
        mon.Close();
      }
      try
      {
        src.Open();
        dst.Open();
        MySqlScript scrSrc = new MySqlScript( src,"delimiter // create table t1( a int, b int, d int ) //" );
        MySqlScript scrDst = new MySqlScript(dst, "delimiter // create table t1( a int, c int, d bit ) //");
        scrSrc.Execute();
        scrDst.Execute();
        scrSrc.Query = @"delimiter // CREATE PROCEDURE simpleproc (OUT param1 INT)
     BEGIN
       SELECT 1 INTO param1;
     END //";
        scrDst.Query = @"delimiter // CREATE PROCEDURE simpleproc (OUT param1 INT)
     BEGIN
       SELECT 1 + 1 INTO param1;
     END //";
        scrSrc.Execute();
        scrDst.Execute();
      }
      finally
      {
        src.Close();
        dst.Close();
      }
       
      Comparer cmp = new Comparer(src, dst);
      ComparerResult result = cmp.Compare();
      
      // Now check results
      ComparerResultItem[] validResultDst = new ComparerResultItem[] {
        new ComparerResultItem( ComparerResultItemType.Equal, null, "t1.a", ObjectType.Column, "t1" ),          
        new ComparerResultItem( ComparerResultItemType.Missing, null, "t1.c", ObjectType.Column, "t1" ),
        new ComparerResultItem( ComparerResultItemType.Different, null, "t1.d", ObjectType.Column, "t1" ),
        new ComparerResultItem( ComparerResultItemType.Different, null, "simpleproc", ObjectType.StoredProcedure, "dbcmp1" )
      };
      ComparerResultItem[] validResultSrc = new ComparerResultItem[] {
        new ComparerResultItem( ComparerResultItemType.Equal, null, "t1.a", ObjectType.Column, "t1" ),
        new ComparerResultItem( ComparerResultItemType.Missing, null, "t1.b", ObjectType.Column, "t1" ),
        new ComparerResultItem( ComparerResultItemType.Different, null, "t1.d", ObjectType.Column, "t1" ),
        new ComparerResultItem( ComparerResultItemType.Different, null, "simpleproc", ObjectType.StoredProcedure, "dbcmp2" )
      };

      Assert.True(validResultDst.Length == result.DiffsInDst.Count);
      Assert.True(validResultSrc.Length == result.DiffsInSrc.Count);

      string scriptSrc;
      string scriptDst;
      cmp.GetScript(result, true, out scriptSrc, out scriptDst);

      cmp.GetScript(result, false, out scriptSrc, out scriptDst);
    }
  }
}
