// Copyright (c) 2008, 2016, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;

namespace MySql.Data.VisualStudio.DBExport
{
  public class MySqlDbExportOptions : INotifyPropertyChanged
  {

    private bool _add_drop_database;
    private bool _add_drop_table;
    private bool _add_locks;
    private bool _all_databases;
    private bool _allow_keywords;
    private bool _comments;
    private bool _compact;
    private bool _complete_insert;
    private bool _create_options;
    private string _database;
    private bool _databases;
    private string _default_character_set;
    private bool _delayed_insert;
    private bool _disable_keys;
    private bool _events;
    private bool _extended_insert;
    private bool _flush_logs;
    private bool _hex_blob;
    private string _host;
    private string _ignore_table;
    private bool _insert_ignore;
    private bool _lock_tables;
    private string _log_error;
    private bool _no_data;
    private bool _no_create_info;
    private int _max_allowed_packet;
    private bool _order_by_primary;
    private int _port;
    private bool _quote_names;
    private bool _replace;
    private string _result_file;
    private bool _routines;
    private bool _single_transaction;
    private string _ssl_ca;
    private string _ssl_cert;
    private string _ssl_key;

    public  bool add_drop_database 
    {
      get {
        return _add_drop_database;
      }

      set {
        _add_drop_database = value;
        NotifyPropertyChanged("add_drop_database");
      }
    }

    public bool add_drop_table
    {
      get
      {
        return _add_drop_table;
      }
      set
      {
        _add_drop_table = value;
        NotifyPropertyChanged("add_drop_table");
      }
    }
    public bool add_locks
    {
      get
      {
        return _add_locks;
      }
      set
      {
        _add_locks = value;
        NotifyPropertyChanged("add_locks");
      }
    }

    public bool all_databases
    {
      get
      {
        return _all_databases;      
      }
      set
      {
        _all_databases = value;
        NotifyPropertyChanged("all_databases");
      }
    }
    public bool allow_keywords
    {
      get
      {
        return _allow_keywords;
      }

      set
      {
        _allow_keywords = value;
        NotifyPropertyChanged("allow_keywords");
      }
    }

    public bool comments
    {
      get
      {
        return _comments;      
      }
      set
      {
        _comments = value;
        NotifyPropertyChanged("comments");
      }
    }
    public bool compact
    {
      get
      {
        return _compact;
      }
      set
      {
        _compact = value;
        NotifyPropertyChanged("compact");
      }
    }

    public bool complete_insert {
     get    
      {
        return _complete_insert;
      }
    
      set
      {
        _complete_insert = value;
        NotifyPropertyChanged("complete_insert");
      }
    }
    public bool create_options
    {
      get {
        return _create_options;      
      }
      set
      {
        _create_options = value;
        NotifyPropertyChanged("create_options");
      }
    }

    public string database
    {
      get
      {
        return _database;
      }
      set
      {
        _database = value;
        NotifyPropertyChanged("database");
      }
    }
    public bool databases
    {
      get
      {
        return _databases;
      }
      set
      {
        _databases = value;
        NotifyPropertyChanged("databases");
      }
    }

    public string default_character_set
    {
      get
      {
        return _default_character_set;
      }
      set
      {
        _default_character_set = value;
        NotifyPropertyChanged("default_character_set");
      }
    }

    public bool delayed_insert
    {
      get
      {
        return _delayed_insert;
      }
      set
      {
        _delayed_insert = value;
        NotifyPropertyChanged("delayed_insert");
      }
    }
    public bool disable_keys
    {
      get
      {
        return _disable_keys;
      }
      set
      {
        _disable_keys = value;
        NotifyPropertyChanged("disable_keys");
      }
    }

    public bool events
    {
      get {
        return _events;
      }

      set
      {
        _events = value;
        NotifyPropertyChanged("events");
      }
    }

    public bool extended_insert
    {
      get
      {
        return _extended_insert;
      }
      set
      {
        _extended_insert = value;
        NotifyPropertyChanged("extended_insert");
      }
    }

    public bool flush_logs
    {
      get
      {
        return _flush_logs;      
      }
      set
      {
        _flush_logs = value;
        NotifyPropertyChanged("flush_logs");
      }
    }

    public bool hex_blob
    {
      get
      {        
        return _hex_blob;
      }
      set
      {
        _hex_blob = value;
        NotifyPropertyChanged("hex_blob");
      }
    }
    public string host
    {
      get
      {
        return _host;
      }

      set
      {
        _host = value;
        NotifyPropertyChanged("host");
      }
    }

    public string ignore_table
    {
      get {
        return _ignore_table;      
      }
      set
      {
        _ignore_table = value;
        NotifyPropertyChanged("ignore_table");
      }
    }
    public bool insert_ignore
    {
      get
      {
        return _insert_ignore;
      }
      set
      {
        _insert_ignore = value;
        NotifyPropertyChanged("insert_ignore");
      }
    }

    public bool lock_tables
    {
      get {
        return _lock_tables;      
      }
      set
      {
        _lock_tables = value;
        NotifyPropertyChanged("lock_tables");
      }
    }
    public string log_error
    {
      get {
        return _log_error;
      }
      set
      {
        _log_error = value;
        NotifyPropertyChanged("log_error");
      }
    }

    public bool no_data
    {
      get
      {
        return _no_data;
      }
      set
      {
        _no_data = value;
        NotifyPropertyChanged("no_data");
      }
    }

    public bool no_create_info
    {
      get {
        return _no_create_info;      
      }

      set
      {
        _no_create_info = value;
        NotifyPropertyChanged("no_create_info");
      }
    }

    //Megabytes value    
    public int max_allowed_packet
    {
      get
      {
        return _max_allowed_packet;
      }
      set
      {
        _max_allowed_packet = value;
        NotifyPropertyChanged("max_allowed_packet");
      }
    }

    public bool order_by_primary
    {
      get
      {
        return _order_by_primary;
      }
      set
      {
        _order_by_primary = value;
        NotifyPropertyChanged("order_by_primary");
      }
    }

    public int port
    {
      get
      {
        return _port;
      }
      set
      {
        _port = value;
        NotifyPropertyChanged("add_drop_table");
      }
    }

    public bool quote_names
    {
      get
      {
        return _quote_names;
      }
      set
      {
        _quote_names = value;
        NotifyPropertyChanged("quote_names");
      }
    }

    public bool replace
    {
      get {
        return _replace;
      }
      set
      {
        _replace = value;
        NotifyPropertyChanged("replace");
      }
    }
    public string result_file
    {
      get
      {
        return _result_file;
      }
      set
      {
        _result_file = value;
        NotifyPropertyChanged("result_file");
      }
    }
    public bool routines
    {
      get
      {
        return _routines;
      }
      set
      {
        _routines = value;
        NotifyPropertyChanged("routines");
      }
    }
    public bool single_transaction
    {
      get
      {
        return _single_transaction;
      }
      set
      {
        _single_transaction = value;
        NotifyPropertyChanged("single_transaction");
      }
    }

    public string ssl_ca
    {
      get
      {
        return _ssl_ca;
      }
      set
      {
        _ssl_ca = value;
        NotifyPropertyChanged("ssl_ca");
      }
    }
    public string ssl_cert
    {
      get
      {
        return _ssl_cert;
      }
      set
      {
        _ssl_cert = value;
        NotifyPropertyChanged("ssl_cert");
      }
    }
    public string ssl_key
    {
      get
      {
        return _ssl_key;
      }
      set
      {
        _ssl_key = value;
        NotifyPropertyChanged("ssl_key");
      }
    }    

    internal Dictionary<PropertyInfo, string> dictionary = new Dictionary<PropertyInfo, string>();

    public event PropertyChangedEventHandler PropertyChanged;

    public MySqlDbExportOptions()
    {
      InitializeDictionary();
      add_drop_database = true;
      quote_names = true;
      default_character_set = "utf8";
      create_options = true;
      disable_keys = true;
      extended_insert = true;
      max_allowed_packet = 1024;
      port = 3306;
      comments = true;
      databases = true;

    }

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private void InitializeDictionary()
    {
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.add_drop_database), "--add-drop-database");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.add_drop_table), "--add-drop-table");      
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.add_locks), "--add-locks");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.all_databases), "--all-databases");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.allow_keywords), "--allow-keywords");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.comments), "--comments");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.compact), "--compact");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.complete_insert), "--complete-insert");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.create_options), "--create-options");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.database), "");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.databases), "--databases");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.default_character_set), "--default-character-set");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.delayed_insert), "--delayed-insert");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.disable_keys), "--disable-keys");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.events), "--events");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.extended_insert), "--extended-insert");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.flush_logs), "--flush-logs");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.hex_blob), "--hex-blob");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.host), "--host");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.ignore_table), "--ignore-table");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.insert_ignore), "--insert-ignore");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.lock_tables), "--lock-tables");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.log_error), "--log-error");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.max_allowed_packet), "--max_allowed_packet");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.no_data), "--no-data");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.no_create_info), "--no-create-info");      
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.order_by_primary), "--order-by-primary");                 
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.port), "--port");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.quote_names), "--quote-names");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.replace), "--replace");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.result_file), "--result-file");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.routines), "--routines");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.single_transaction), "--single-transaction");      
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.ssl_ca), "--ssl-ca");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.ssl_cert), "--ssl-cert");
      dictionary.Add(TypedReflection<MySqlDbExportOptions>.GetPropertyInfo(p => p.ssl_key), "--ssl-key");      
    }

  }

  public static class TypedReflection<TSource>
  {
    public static PropertyInfo GetPropertyInfo<TProperty>(
        Expression<Func<TSource, TProperty>> propertyLambda)
    {
      MemberExpression member = propertyLambda.Body as MemberExpression;
      if (member == null)
        throw new ArgumentException(string.Format(
            "Expression '{0}' refers to a method, not a property.",
            propertyLambda.ToString()));

      PropertyInfo propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
        throw new ArgumentException(string.Format(
            "Expression '{0}' refers to a field, not a property.",
            propertyLambda.ToString()));

      return propInfo;
    }
  }
}
