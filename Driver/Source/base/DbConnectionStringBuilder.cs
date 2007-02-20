// Copyright (C) 2004-2006 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
    public class DbConnectionStringBuilder : IDictionary, ICollection, IEnumerable, ICustomTypeDescriptor
	{
        private string connectionString;
        private Hashtable hash;
        private bool browsable;

        public DbConnectionStringBuilder()
        {
            hash = new Hashtable();
            browsable = false;
        }

        #region Properties

        public bool BrowsableConnectionString 
        {
            get { return browsable; }
            set { browsable = value; }
        }
        
        public string ConnectionString 
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public virtual object this[string key]
        {
            get { return hash[key]; }
            set { Add(key, value);  }
        }

        #endregion

        #region IDictionary Members

        public void Add(object key, object value)
        {
            hash.Add(key, value);
            //TODO: update connection string
        }

        public virtual void Clear()
        {
            connectionString = null;
            hash.Clear();
        }

        public bool Contains(object key)
        {
            return hash.ContainsKey(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return hash.GetEnumerator();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection Keys
        {
            get { return hash.Keys; }
        }

        public void Remove(object key)
        {
            hash.Remove(key);
            //TODO: update connection string
        }

        public ICollection Values
        {
            get { return hash.Values; }
        }

        public object this[object key]
        {
            get
            {
                return this[(string)key];
            }
            set
            {
                this[(string)key] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            hash.CopyTo(array, index);
        }

        public int Count
        {
            get { return hash.Count; }
        }

        public bool IsSynchronized
        {
            get { return hash.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return hash.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return hash.GetEnumerator();
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetClassName()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetComponentName()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TypeConverter GetConverter()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
