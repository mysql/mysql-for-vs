
using System.Text;
namespace MySql.Data.Entity
{
    class DeleteStatement : SqlFragment
    {
        public SqlFragment Target { get; set; }
        public SqlFragment Where { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder("DELETE");
            sb.AppendFormat(" {0} FROM {1}", QuoteIdentifier(Target.Name), 
                Target.GenerateSQL());
            if (Where != null)
                sb.AppendFormat(" WHERE {0}", Where.GenerateSQL());
            return sb.ToString();
        }
    }
}
