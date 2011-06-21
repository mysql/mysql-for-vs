using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MySql.Data.Entity
{
    class SymbolTable
    {
        public SymbolTable()
        {
            Symbols = new Dictionary<string, SqlFragment>();
        }

        private Dictionary<string, SqlFragment> Symbols { get; set; }

        public void Add(string name, SqlFragment fragment)
        {
            Debug.Assert(!Symbols.ContainsKey(name));
            if (fragment.Name == null)
                fragment.Name = name;
            Symbols.Add(name, fragment);
        }

        public SqlFragment Lookup(string name)
        {
            return Symbols[name];
        }
    }
}
