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
using System.Data;
using System.Collections;

namespace MySql.Data.MySqlClient
{
	public class DbParameterCollection : MarshalByRefObject, IDataParameterCollection, 
		IList, ICollection, IEnumerable
	{

        #region ICollection support

		/// <summary>
		/// Gets the number of MySqlParameter objects in the collection.
		/// </summary>
		public abstract int Count { get; }

        /// <summary>
        /// Copies MySqlParameter objects from the MySqlParameterCollection to the specified array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public abstract void CopyTo(Array array, int index);
        {
        	parameters.CopyTo(array, index);
        }

        //		bool ICollection.IsSynchronized
        //		{
        //			get { return _parms.IsSynchronized; }
        //		}

        //		object ICollection.SyncRoot
        //		{
        //			get { return _parms.SyncRoot; }
        //		}
        #endregion

        #region IList

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _parms.Clear();
        }

        public bool Contains(object value)
        {
            return _parms.Contains(value);
        }

        /// <summary>
        /// Gets the location of a <see cref="MySqlParameter"/> in the collection.
        /// </summary>
        /// <param name="value">The <see cref="MySqlParameter"/> object to locate. </param>
        /// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
        /// <overloads>Gets the location of a <see cref="MySqlParameter"/> in the collection.</overloads>
        public int IndexOf(object value)
        {
            return _parms.IndexOf(value);
        }

        /// <summary>
        /// Inserts a MySqlParameter into the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Insert(int index, object value)
        {
            _parms.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return _parms.IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return _parms.IsReadOnly; }
        }

        /// <summary>
        /// Removes the specified MySqlParameter from the collection.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            _parms.Remove(value);
        }

        /// <summary>
        /// Removes the specified <see cref="MySqlParameter"/> from the collection using a specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter. </param>
        /// <overloads>Removes the specified <see cref="MySqlParameter"/> from the collection.</overloads>
        public void RemoveAt(int index)
        {
            _parms.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                if (!(value is MySqlParameter)) throw new MySqlException("Only MySqlParameter objects may be stored");
                this[index] = (MySqlParameter)value;
            }
        }

        /// <summary>
        /// Adds the specified <see cref="MySqlParameter"/> object to the <see cref="MySqlParameterCollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="MySqlParameter"/> to add to the collection.</param>
        /// <returns>The index of the new <see cref="MySqlParameter"/> object.</returns>
        public int Add(object value)
        {
            if (!(value is MySqlParameter))
                throw new MySqlException("Only MySqlParameter objects may be stored");

            MySqlParameter p = (MySqlParameter)value;

            if (p.ParameterName == null || p.ParameterName == String.Empty)
                throw new MySqlException("Parameters must be named");

            return _parms.Add(value);
        }

        #endregion

        #region IDataParameterCollection

        /// <summary>
        /// Gets a value indicating whether a <see cref="MySqlParameter"/> with the specified parameter name exists in the collection.
        /// </summary>
        /// <param name="name">The name of the <see cref="MySqlParameter"/> object to find.</param>
        /// <returns>true if the collection contains the parameter; otherwise, false.</returns>
        public bool Contains(string name)
        {
            return IndexOf(name) != -1;
        }

        /// <summary>
        /// Gets the location of the <see cref="MySqlParameter"/> in the collection with a specific parameter name.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
        /// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
        public int IndexOf(string parameterName)
        {
            if (parameterName[0] == paramMarker)
                parameterName = parameterName.Substring(1, parameterName.Length - 1);
            parameterName = parameterName.ToLower();
            for (int x = 0; x < _parms.Count; x++)
            {
                MySqlParameter p = (MySqlParameter)_parms[x];
                string listName = p.ParameterName;
                if (listName[0] == paramMarker)
                    listName = listName.Substring(1, listName.Length - 1);
                if (listName.ToLower() == parameterName) return x;
            }
            return -1;
        }

        /// <summary>
        /// Removes the specified <see cref="MySqlParameter"/> from the collection using the parameter name.
        /// </summary>
        /// <param name="name">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
        public void RemoveAt(string name)
        {
            _parms.RemoveAt(InternalIndexOf(name));
        }

        object IDataParameterCollection.this[string name]
        {
            get { return this[name]; }
            set
            {
                if (!(value is MySqlParameter)) throw new MySqlException("Only MySqlParameter objects may be stored");
                this[name] = (MySqlParameter)value;
            }
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_parms).GetEnumerator();
        }
        #endregion
*/
	}
}
