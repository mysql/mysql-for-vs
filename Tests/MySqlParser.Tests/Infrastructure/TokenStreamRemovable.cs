using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using MySql.Parser;

namespace MySql.Parser.Tests
{
  [TestFixture]
  public class TokenStreamRemovable
  {
    [Test]
    public void TestTokenRemove()
    {
      string sql = "select *, a, c, d from table1 where a is null";
      MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQL51Lexer lexer = new MySQL51Lexer(input);
      MySql.Parser.TokenStreamRemovable tsr = new MySql.Parser.TokenStreamRemovable(lexer);
      tsr.Fill();
      List<IToken> tokens = tsr.GetTokens();
      IToken removed = null;
      foreach( IToken t in tokens )
      {
        if (t.Text == "d")
        {
          removed = t;
          break;
        }
      }
      tsr.Remove(removed);
      tokens = tsr.GetTokens();
    }
  }
}
