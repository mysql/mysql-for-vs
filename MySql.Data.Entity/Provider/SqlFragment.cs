using System;

namespace MySql.Data.Entity
{
    public class SqlFragment
    {
        public SqlFragment()
        {
        }

        public SqlFragment(string text)
        {
            Text = text;
        }

        #region Properties

        public string Text { get; set; }
        public string Name { get; set; }

        #endregion

        public virtual string GenerateSQL()
        {
            if (String.IsNullOrEmpty(Name))
                return Text;
            return String.Format("{0} AS {1}",
                Text, QuoteIdentifier(Name));
        }

        protected string QuoteIdentifier(string id)
        {
            return String.Format("`{0}`", id);
        }

        public override string ToString()
        {
            return GenerateSQL();
        }
    }
}
