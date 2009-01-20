
using System.Text;
using System.Collections.Generic;
namespace MySql.Data.Entity
{
    class UpdateStatement : SqlFragment 
    {
        public UpdateStatement()
        {
            Properties = new List<SqlFragment>();
            Values = new List<SqlFragment>();
        }

        public SqlFragment Target { get; set; }
        public List<SqlFragment> Properties { get; private set; }
        public List<SqlFragment> Values { get; private set; }
        public SqlFragment Where { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE {0} SET ", Target.GenerateSQL());
            string seperator = "";
            for (int i = 0; i < Properties.Count; i++)
            {
                sb.AppendFormat("{0}{1}={2}", seperator, 
                    Properties[i].GenerateSQL(), Values[i].GenerateSQL());
                seperator = ", ";
            }
            if (Where != null)
                sb.AppendFormat(" WHERE {0}", Where.GenerateSQL());

            return sb.ToString();
        }
    }
}
