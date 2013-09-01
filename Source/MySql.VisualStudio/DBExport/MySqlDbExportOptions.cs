// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
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
    
    public  bool add_drop_database { get; set; }
    public bool add_drop_table { get; set; }    
    public bool add_locks{ get; set; }
    public bool all_databases { get; set; }        
    public bool allow_keywords { get; set; }
       
    public bool comments { get; set; }
    public bool compact { get; set; }
    public bool complete_insert { get; set; }    
    public bool create_options { get; set; }

    public string database { get; set; }
    public bool databases { get; set; }
       
    public string default_character_set { get; set; }
    public bool delayed_insert { get; set; }    
    public bool disable_keys { get; set; }

    public bool events {get; set;}
    public bool extended_insert { get; set; }

    public bool flush_logs { get; set; }

    public bool hex_blob { get; set; }
    public string host { get; set; }

    public string ignore_table { get; set; }
    public bool insert_ignore { get; set; }

    public bool lock_tables { get; set; }
    public string log_error { get; set; }
    
    public bool no_data { get; set; }

    public bool no_create_info { get; set; }

    //Megabytes value    
    public int max_allowed_packet { get; set; }

    public bool order_by_primary { get; set; }

    public int port { get; set; }
    
    public bool quote_names { get; set; }

    public bool replace { get; set; }
    public string result_file { get; set; }
    public bool routines { get; set; }
    public bool single_transaction { get; set; }

    public string ssl_ca { get; set; }
    public string ssl_cert { get; set; }
    public string ssl_key { get; set; }    

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
