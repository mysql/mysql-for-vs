// Copyright (c) 2016, 2017, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using MySql.Utility.Classes;
using MySql.Utility.Classes.Tokenizers;
using MySql.Utility.Enums;
using Xunit;

namespace MySql.Utility.Tests.Tokenizers
{
  public class PyTokenizerTests : TokenizerBaseTests
  {
    public PyTokenizerTests()
    {
      SchemaName1 = SqlTokenizerTests.X_TEST_SCHEMA_NAME;
      SchemaName2 = null;
      SchemaName3 = null;
      DropSchemasOnDispose = false;
      ScriptLanguage = ScriptLanguageType.Python;
      Script = ReplaceSchemasInScript(GetPythonSetupScript().NormalizeNewLineCharacters());
      ScriptStatementsCount = 26;
    }

    /// <summary>
    /// Tests the splitting of statements on the <see cref="TokenizerBaseTests.Script"/>.
    /// </summary>
    [Fact]
    public override void SplitStatements()
    {
      string tokenizerExceptionMessage = null;
      try
      {
        base.SplitStatements();
      }
      catch (TokenizerException tEx)
      {
        tokenizerExceptionMessage = "Position: " + tEx.AtPosition + " , Error: " + tEx.Message;
      }

      Assert.True(string.IsNullOrEmpty(tokenizerExceptionMessage), tokenizerExceptionMessage);
    }
  }
}
