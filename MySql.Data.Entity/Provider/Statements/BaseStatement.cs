using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Data.Entity
{
    class BaseStatement : SqlFragment
    {
        private Dictionary<string, List<SqlFragment>> namespaces = new Dictionary<string, List<SqlFragment>>();

        public void IndexFragment(SqlFragment fragment, string name)
        {
            if (!namespaces.ContainsKey(name))
                namespaces.Add(name, new List<SqlFragment>());

            List<SqlFragment> list = namespaces[name];
            namespaces[name].Add(fragment);
        }
    }
}
