using System;
using System.Reflection;
using System.Collections.Generic;

namespace MySql.Data.VisualStudio.LanguageService
{
    class SqlTokenizer
    {
        object tokenizer;

        public SqlTokenizer()
        {
            Assembly a = Assembly.LoadWithPartialName("mysql.data");
            if (a == null)
                throw new Exception("Unable to load mysql.data assembly");
            tokenizer = a.CreateInstance("MySql.Data.MySqlClient.MySqlTokenizer",
                false, System.Reflection.BindingFlags.CreateInstance, null, null,
                    null, null);
        }

        public SqlTokenizer(string sql)
        {
            Assembly a = Assembly.LoadWithPartialName("mysql.data");
            if (a == null)
                throw new Exception("Unable to load mysql.data assembly");
            tokenizer = a.CreateInstance("MySql.Data.MySqlClient.MySqlTokenizer",
                false, System.Reflection.BindingFlags.CreateInstance, null, 
                new object[] { sql }, null, null);
        }

        #region Properties

        public string Text
        {
            set
            {
                PropertyInfo pi = tokenizer.GetType().GetProperty("Text");
                pi.SetValue(tokenizer, value, null);
            }
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
