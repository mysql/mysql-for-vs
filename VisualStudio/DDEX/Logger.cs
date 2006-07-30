using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MySql.Data.VisualStudio
{
    class Logger
    {
        public static void WriteLine(string s)
        {
            Trace.WriteLine(String.Format("[{0}] - {1}",
                DateTime.Now, s));
        }
    }
}
