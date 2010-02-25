using System.Reflection;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient.Tests
{
    class SqlTokenizer
    {
        object tokenizer;

        public SqlTokenizer(string sql)
        {
            tokenizer = typeof(MySqlConnection).Assembly.CreateInstance("MySql.Data.MySqlClient.MySqlTokenizer",
                false, System.Reflection.BindingFlags.CreateInstance, null,
                    new object[] { sql }, null, null);
        }

        public bool ReturnComments
        {
            set
            {
                PropertyInfo pi = tokenizer.GetType().GetProperty("ReturnComments");
                pi.SetValue(tokenizer, value, null);
            }
        }

        public bool AnsiQuotes
        {
            set
            {
                PropertyInfo pi = tokenizer.GetType().GetProperty("AnsiQuotes");
                pi.SetValue(tokenizer, value, null);
            }
        }

        public bool SqlServerMode
        {
            set
            {
                PropertyInfo pi = tokenizer.GetType().GetProperty("SqlServerMode");
                pi.SetValue(tokenizer, value, null);
            }
        }

        public bool Quoted
        {
            get
            {
                PropertyInfo pi = tokenizer.GetType().GetProperty("Quoted");
                return (bool)pi.GetValue(tokenizer, null);
            }
        }

        public string NextToken()
        {
            return (string)tokenizer.GetType().InvokeMember("NextToken",
                System.Reflection.BindingFlags.InvokeMethod,
                null, tokenizer, null);
        }

        public string NextParameter()
        {
            return (string)tokenizer.GetType().InvokeMember("NextParameter",
                System.Reflection.BindingFlags.InvokeMethod,
                null, tokenizer, null);
        }
    }
}
