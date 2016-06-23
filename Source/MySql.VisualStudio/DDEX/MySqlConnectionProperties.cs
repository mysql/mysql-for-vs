// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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

/*
 * This file contains implementation of customized connection properties. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using Microsoft.VisualStudio.Data;
using MySql.Data.MySqlClient;

namespace MySql.Data.VisualStudio.DDEX
{
  /// <summary>
  /// This class customize standard connection properties for 
  /// MySql data base connection.
  /// </summary>
  public class MySqlConnectionProperties : DataConnectionProperties, IDictionary
  {
    public static string InvariantName = "MySql.Data.MySqlClient";

    /// <summary>
    /// Constructor fills base object with list of custom options and their description.
    /// </summary>
    public MySqlConnectionProperties()
    {
      ConnectionStringBuilder = new MySqlConnectionStringBuilder();
    }

    public DbConnectionStringBuilder ConnectionStringBuilder { get; set; }

    public override object this[string propertyName]
    {
      get
      {
        if (propertyName == null)
        {
          throw new ArgumentNullException("propertyName");
        }

        object obj;
        if (!ConnectionStringBuilder.TryGetValue(propertyName, out obj))
        {
          return null;
        }

        if (ConnectionStringBuilder.ShouldSerialize(propertyName))
        {
          return ConnectionStringBuilder[propertyName];
        }

        return ConnectionStringBuilder[propertyName] ?? DBNull.Value;
      }

      set
      {
        if (propertyName == null)
        {
          throw new ArgumentNullException("propertyName");
        }

        ConnectionStringBuilder.Remove(propertyName);
        if (value == DBNull.Value)
        {
          OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(propertyName));
        }
        else
        {
          object objA;
          ConnectionStringBuilder.TryGetValue(propertyName, out objA);
          ConnectionStringBuilder[propertyName] = value;
          if (Equals(objA, value))
          {
            ConnectionStringBuilder.Remove(propertyName);
          }

          OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(propertyName));
        }
      }
    }

    protected override void InitializeProperties()
    {
      var props = TypeDescriptor.GetProperties(ConnectionStringBuilder, true);
      foreach (PropertyDescriptor prop in props)
      {
        var nameAttribute = prop.Attributes[typeof(NameAttribute)] as NameAttribute;
        if (nameAttribute != null)
        {
          var attributeArray = new Attribute[prop.Attributes.Count];
          prop.Attributes.CopyTo(attributeArray, 0);
          AddProperty(nameAttribute.Name, prop.PropertyType, attributeArray);
        }
        else
        {
          AddProperty(prop, new Attribute[0]);
        }
      }
    }

    /// <summary>
    /// Test connection for these properties. Uses MySqlConnection support for version validation.
    /// </summary>
    public override void Test()
    {
      // Create connection support
      MySqlConnectionSupport conn = new MySqlConnectionSupport();
      try
      {
        // Initializes it with empty provider
        conn.Initialize(null);
        // Set connection string
        conn.ConnectionString = ConnectionStringBuilder.ConnectionString;
        // Try to open
        conn.Open(false);
        // Close after open
        conn.Close();
      }
      finally
      {
        // In any case dispose connection support
        conn.Dispose();
      }
    }

    /// <summary>
    /// Connection properties are complete if server and database specified.
    /// </summary>
    public override bool IsComplete
    {
      get
      {
        DbConnectionStringBuilder cb = ConnectionStringBuilder;
        return !string.IsNullOrEmpty((string)cb["Server"]) && !string.IsNullOrEmpty((string)cb["Database"]);
      }
    }

    public override string ToDisplayString()
    {
      string str2;
      var properties = ((ICustomTypeDescriptor)this).GetProperties(new Attribute[] { PasswordPropertyTextAttribute.Yes });
      var list = new List<KeyValuePair<string, object>>();
      foreach (PropertyDescriptor descriptor in properties)
      {
        string str = descriptor.DisplayName;
        if (!ConnectionStringBuilder.ShouldSerialize(str))
        {
          continue;
        }

        list.Add(new KeyValuePair<string, object>(str, ConnectionStringBuilder[str]));
        ConnectionStringBuilder.Remove(str);
      }
      try
      {
        str2 = ConnectionStringBuilder.ToString();
      }
      finally
      {
        foreach (KeyValuePair<string, object> pair in list)
        {
          if (pair.Value != null)
          {
            ConnectionStringBuilder[pair.Key] = pair.Value;
          }
        }
      }

      return str2;
    }

    public override string ToFullString()
    {
      return ToString();
    }
        
    public override string ToString()
    {
      return ConnectionStringBuilder.ConnectionString;
    }

    public override bool EquivalentTo(DataConnectionProperties connectionProperties)
    {
      return ToString().Equals(((MySqlConnectionProperties)connectionProperties).ToString());
    }

    object this[object key]
    {
      get
      {
        object valueKey;
        ConnectionStringBuilder.TryGetValue(key.ToString(), out valueKey);
        return valueKey;
      }
      set
      {
        ConnectionStringBuilder[key.ToString()] = value;
        OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(key.ToString()));
      }
    }

    ICollection Keys
    {
      get
      {
        return ConnectionStringBuilder.Keys;
      }
    }

    ICollection Values
    {
      get
      {
        return ConnectionStringBuilder.Values;
      }
    }

    public override IEnumerator GetEnumerator()
    {
      return ConnectionStringBuilder.Values.GetEnumerator();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return ((IDictionary)ConnectionStringBuilder).GetEnumerator();
    }
  }
}