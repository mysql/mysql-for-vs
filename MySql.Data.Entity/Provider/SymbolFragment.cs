using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.Entity
{
    class SymbolFragment : SqlFragment
    {
        public SymbolFragment()
        {
            Properties = new List<string>();
        }

        public List<string> Properties { get; private set; }
        public string Variable { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder(QuoteIdentifier(Properties[0]));
            if (Properties.Count == 2)
                sb.AppendFormat(".{0}", QuoteIdentifier(Properties[1]));
            return sb.ToString();
        }
    }
}
