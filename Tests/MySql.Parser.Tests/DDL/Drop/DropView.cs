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

using System;
using Antlr.Runtime;
using Xunit;


namespace MySql.Parser.Tests.DDL.Drop
{
  
  public class DropView
  {
    [Fact]
    public void SimpleNoSchema()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP VIEW `viewname`");
      /*
      Assert.Equal(1, statements.Count);
      Assert.True(statements[0] is DropViewStatement);
      DropViewStatement ds = statements[0] as DropViewStatement;
      Assert.Equal(1, ds.ToDrop.Count);
      Assert.Equal("`viewname`", ds.ToDrop[0].Name.Text);
       * */
    }

    [Fact]
    public void SimpleWithSchema()
    {			
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP VIEW `schema1`.`viewname`");
      /*
      Assert.Equal(1, statements.Count);
      Assert.True(statements[0] is DropViewStatement);
      DropViewStatement ds = statements[0] as DropViewStatement;
      Assert.Equal(1, ds.ToDrop.Count);
      Assert.Equal("`schema1`", ds.ToDrop[0].Database.Text);
      Assert.Equal("`viewname`", ds.ToDrop[0].Name.Text);
       * */
    }

    [Fact]
    public void MissingViewName()
    {
      try
      {
        Utility.ParseSql("DROP VIEW", true);
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.WriteLine(e.Message);
      }
    }

    [Fact]
    public void MultipleViews()
    {			
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP VIEW `view1`, schema2.view2, `schema3`.`view3`, view4");
      /*
      Assert.Equal(1, statements.Count);
      Assert.True(statements[0] is DropViewStatement);
      DropViewStatement ds = statements[0] as DropViewStatement;
      Assert.Equal(4, ds.ToDrop.Count);

      Assert.Null(ds.ToDrop[0].Database);
      Assert.Equal("`view1`", ds.ToDrop[0].Name.Text);

      Assert.Equal("schema2", ds.ToDrop[1].Database.Text);
      Assert.Equal("view2", ds.ToDrop[1].Name.Text);

      Assert.Equal("`schema3`", ds.ToDrop[2].Database.Text);
      Assert.Equal("`view3`", ds.ToDrop[2].Name.Text);

      Assert.Null(ds.ToDrop[3].Database);
      Assert.Equal("view4", ds.ToDrop[3].Name.Text);
       * */
    }

    [Fact]
    public void IfExists()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP VIEW IF EXISTS `viewname`");
      /*
      Assert.Equal(1, statements.Count);
      Assert.True(statements[0] is DropViewStatement);
      DropViewStatement ds = statements[0] as DropViewStatement;
      Assert.Equal(1, ds.ToDrop.Count);
      Assert.Equal("`viewname`", ds.ToDrop[0].Name.Text);
      Assert.True(ds.IfExists);
       * */
    }

    [Fact]
    public void CascadeOrRestrict()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP VIEW IF EXISTS `viewname` CASCADE");
      /*
      DropViewStatement ds = statements[0] as DropViewStatement;
      Assert.True(ds.Cascade);
      Assert.False(ds.Restrict);
      */
      r = Utility.ParseSql("DROP VIEW IF EXISTS `viewname` RESTRICT");
      /*
      ds = statements[0] as DropViewStatement;
      Assert.False(ds.Cascade);
      Assert.True(ds.Restrict);
      */

      r = Utility.ParseSql("DROP VIEW IF EXISTS `viewname` RESTRICT CASCADE", true);
    }
  }
}
