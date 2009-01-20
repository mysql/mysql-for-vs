using System.Text;

namespace MySql.Data.Entity 
{
    class InsertStatement : SqlFragment 
    {
        public SqlFragment Target { get; set; }

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder("INSERT ");
            sb.Append(Target.GenerateSQL());

            return sb.ToString();
        }
    }
}
