// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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

/*
 * This file contains implementation of customized connection properties. 
 */
using System;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data.Common;
using Microsoft.VisualStudio.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.ComponentModel;
using MySql.Utility.Classes.Logging;
using System.Text;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// This class customize standard connection properties for 
  /// MySql data base connection.
  /// </summary>
  public class MySqlConnectionProperties : DataConnectionProperties, IDictionary, ICustomTypeDescriptor, ICollection, IEnumerable
  {
    public static string InvariantName = "MySql.Data.MySqlClient";

    /// <summary>
    /// Constructor fills base object with list of custom options and their description.
    /// </summary>
    public MySqlConnectionProperties()
    {
      ConnectionStringBuilder = new MySqlConnectionStringBuilder();
    }

    public DbConnectionStringBuilder ConnectionStringBuilder
    {
      get;
      private set;
    }

    public override object this[string propertyName]
    {
      get
      {
        if (propertyName == null)  throw new ArgumentNullException("propertyName");
        object obj = (object)null;
        if (!ConnectionStringBuilder.TryGetValue(propertyName, out obj))
          return (object)null;
        if (ConnectionStringBuilder.ShouldSerialize(propertyName))
          return ConnectionStringBuilder[propertyName];
        else
          return ConnectionStringBuilder[propertyName] ?? (object)DBNull.Value;
      }
      set
      {
        if (propertyName == null)  throw new ArgumentNullException("propertyName");
        ConnectionStringBuilder.Remove(propertyName);
        if (value == DBNull.Value)
        {
          this.OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(propertyName));
        }
        else
        {
          object objA = (object)null;
          ConnectionStringBuilder.TryGetValue(propertyName, out objA);
          ConnectionStringBuilder[propertyName] = value;
          if (object.Equals(objA, value))
            ConnectionStringBuilder.Remove(propertyName);
          this.OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(propertyName));
        }
      }
    }

    protected override void InitializeProperties()
    {
      PropertyDescriptorCollection props = TypeDescriptor.GetProperties(ConnectionStringBuilder, true);
      foreach (PropertyDescriptor prop in props)
      {
        NameAttribute nameAttribute = prop.Attributes[typeof(NameAttribute)] as NameAttribute;
        if (nameAttribute != null)
        {
          Attribute[] attributeArray = new Attribute[prop.Attributes.Count];
          prop.Attributes.CopyTo((Array)attributeArray, 0);
          this.AddProperty(nameAttribute.Name, prop.PropertyType, attributeArray);
        }
        else
          this.AddProperty(prop, new Attribute[0]);
      }
    }

    /// <summary>
    /// Test connection for these properties. Uses MySqlConnection support for version validation.
    /// </summary>
    public override void Test()
    {
      // Create connection support.
      MySqlConnectionSupport conn = new MySqlConnectionSupport();
      try
      {
        // Initializes it with empty provider.
        conn.Initialize(null);

        conn.ConnectionString = ConnectionStringBuilder.ConnectionString;
        conn.Open(false);        
        conn.Close();
      }
      catch(Exception exception)
      {
        exception = Common.Utilities.GetExceptionWithFullNestedMessage(exception);
        Logger.LogException(exception);
        throw exception;
      }
      finally
      {
        // In any case dispose connection support.
        if (conn != null)
        {
          conn.Dispose();
        }
      }
    }

    /// <summary>
    /// Connection properties are complete if server and database specified.
    /// </summary>
    public override bool IsComplete
    {
      get
      {
        DbConnectionStringBuilder cb = this.ConnectionStringBuilder;
        return !String.IsNullOrEmpty((string)cb["Server"])
               && !String.IsNullOrEmpty((string)cb["Database"]);
      }
    }


    public override string ToDisplayString()
    {
      string str2;
      PropertyDescriptorCollection properties = ((ICustomTypeDescriptor)this).GetProperties(new System.Attribute[] { PasswordPropertyTextAttribute.Yes });
      System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, object>> list = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, object>>();
      foreach (PropertyDescriptor descriptor in properties)
      {
        string str = descriptor.DisplayName;
        if (ConnectionStringBuilder.ShouldSerialize(str))
        {
          list.Add(new System.Collections.Generic.KeyValuePair<string, object>(str, ConnectionStringBuilder[str]));
          ConnectionStringBuilder.Remove(str);
        }
      }
      try
      {
        str2 = ConnectionStringBuilder.ToString();
      }
      finally
      {
        foreach (System.Collections.Generic.KeyValuePair<string, object> pair in list)
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
      return this.ConnectionStringBuilder.ConnectionString;
    }
        
    public override string ToString()
    {
        return this.ToFullString();
    }

    public override bool EquivalentTo(DataConnectionProperties connectionProperties)
    {
      return this.ToFullString().Equals(((MySqlConnectionProperties)connectionProperties).ToFullString());
    }


    object this[object key]
    {
      get
      {
        object valueKey;
        this.ConnectionStringBuilder.TryGetValue(key.ToString(), out valueKey);
        return valueKey;
      }
      set
      {
        this.ConnectionStringBuilder[key.ToString()] = value;
        this.OnPropertyChanged(new DataConnectionPropertyChangedEventArgs(key.ToString()));
      }
    }

    System.Collections.ICollection Keys
    {
      get
      {
        return this.ConnectionStringBuilder.Keys;
      }
    }

    System.Collections.ICollection Values
    {
      get
      {
        return this.ConnectionStringBuilder.Values;
      }
    }

    public override IEnumerator GetEnumerator()
    {
      return this.ConnectionStringBuilder.Values.GetEnumerator();
    }

    IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
    {
      return ((System.Collections.IDictionary)this.ConnectionStringBuilder).GetEnumerator();
    }

  }
}