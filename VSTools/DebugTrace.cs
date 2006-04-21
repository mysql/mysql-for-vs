using System;
using System.Diagnostics;

namespace MySql.VSTools
{
    class DebugTrace
    {
        public static void Trace(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
