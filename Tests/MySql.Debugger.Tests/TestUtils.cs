using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger.Tests
{
  public class TestUtils
  {
    public static readonly string CONNECTION_STRING = "server=localhost;User Id=root;database=test6;Allow User Variables=true;Pooling=false;";
    public static readonly string CONNECTION_STRING_DBG = "server=localhost;User Id=root;database=ServerSideDebugger;Allow User Variables=true;Pooling=false;";
    public static readonly string CONNECTION_STRING_WITHOUT_DB = "server=localhost;User Id=root;Allow User Variables=true;Pooling=false;";
  }
}
