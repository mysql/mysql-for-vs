using System.Collections.Generic;
using System.Text;

namespace MySql.Data.Entity
{
    class ListFragment : SqlFragment 
    {
        public ListFragment(string sep)
        {
            Items = new List<SqlFragment>();
            Seperator = sep;
        }

        public List<SqlFragment> Items { get; private set; }
        public string Seperator { get; set; }

        public override string GenerateSQL()
        {
            string seperator = "";
            StringBuilder sb = new StringBuilder();

            foreach (SqlFragment f in Items)
            {
                sb.AppendFormat("{0}{1}", seperator, f.GenerateSQL());
                seperator = Seperator;
            }
            return sb.ToString();
        }
    }
}
