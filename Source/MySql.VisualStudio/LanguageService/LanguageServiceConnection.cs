using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
  public sealed class LanguageServiceConnection
  {
    private static readonly LanguageServiceConnection current = new LanguageServiceConnection();

    private LanguageServiceConnection() { }

    public static LanguageServiceConnection Current
    {
      get
      {
        return current;
      }
    }

    public DbConnection Connection { get; set; }
  }
}
