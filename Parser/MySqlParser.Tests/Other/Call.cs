using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Parser.Tests
{
  [TestFixture]
  public class Call
  {
    [Test]
    public void CallSimple()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp", false, out sb);
    }

    [Test]
    public void CallSimple2()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp()", false, out sb);
    }

    [Test]
    public void CallWithArgs()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call sp( 4 + 3, 'myname', (( 3.1416 * 20 ) + 7 ) / 2.71)", false, out sb);
    }

    [Test]
    public void CallWrong()
    {
      StringBuilder sb;
      MySQL51Parser.program_return r =
        Utility.ParseSql("call ", true, out sb);
    }
  }
}
