using System.Text;

namespace MySql.Data.Entity
{
    class JoinFragment : TableFragment
    {
        public SqlFragment Left;
        public SqlFragment Right;
        public SqlFragment Condition;
        public string JoinType;

        public override string GenerateSQL()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2} ON {3}", Left.GenerateSQL(),
                JoinType, Right.GenerateSQL(), Condition.GenerateSQL());
            return sb.ToString();
        }
    }
}
