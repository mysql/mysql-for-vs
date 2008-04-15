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
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
	public abstract class DbParameterCollection : MarshalByRefObject, IDataParameterCollection, 
		IList, ICollection, IEnumerable
	{

		protected abstract DbParameter GetParameter(int index);
		protected abstract DbParameter GetParameter(string parameterName);
		protected abstract void SetParameter(int index, DbParameter parameter);
		protected abstract void SetParameter(string parameterName, DbParameter parameter);
		public abstract void AddRange(Array values);

		#region ICollection support

        /// <summary>
        /// Gets the number of MySqlParameter objects in the collection.
        /// </summary>
        [Browsable(false)]
		public abstract int Count { get; }

        /// <summary>
        /// Copies MySqlParameter objects from the MySqlParameterCollection to the specified array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public abstract void CopyTo(Array array, int index);

		[Browsable(false)]
		public abstract bool IsSynchronized { get; }

		[Browsable(false)]
		public abstract object SyncRoot { get; }

        #endregion

        #region IList

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public abstract void Clear();

        public abstract bool Contains(object value);

        /// <summary>
        /// Gets the location of a <see cref="MySqlParameter"/> in the collection.
        /// </summary>
        /// <param name="value">The <see cref="MySqlParameter"/> object to locate. </param>
        /// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
        /// <overloads>Gets the location of a <see cref="MySqlParameter"/> in the collection.</overloads>
        public abstract int IndexOf(object value);

        /// <summary>
        /// Inserts a MySqlParameter into the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public abstract void Insert(int index, object value);

		[Browsable(false)]
		public abstract bool IsFixedSize { get; }

		[Browsable(false)]
		public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Removes the specified MySqlParameter from the collection.
        /// </summary>
        /// <param name="value"></param>
        public abstract void Remove(object value);

        /// <summary>
        /// Removes the specified <see cref="MySqlParameter"/> from the collection using a specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter. </param>
        /// <overloads>Removes the specified <see cref="MySqlParameter"/> from the collection.</overloads>
        public abstract void RemoveAt(int index);

		[Browsable(false)]
		public DbParameter this[int index] 
		{ 
			get { return this.GetParameter(index); }
			set { this.SetParameter(index, value); }
		}

		object IList.this[int index]
		{
			get { return this.GetParameter(index); }
			set { this.SetParameter(index, value); }
		}

        /// <summary>
        /// Adds the specified <see cref="MySqlParameter"/> object to the <see cref="MySqlParameterCollection"/>.
        /// </summary>
        /// <param name="value">The <see cref="MySqlParameter"/> to add to the collection.</param>
        /// <returns>The index of the new <see cref="MySqlParameter"/> object.</returns>
        public abstract int Add(object value);

        #endregion

        #region IDataParameterCollection

        /// <summary>
        /// Gets a value indicating whether a <see cref="MySqlParameter"/> with the specified parameter name exists in the collection.
        /// </summary>
        /// <param name="name">The name of the <see cref="MySqlParameter"/> object to find.</param>
        /// <returns>true if the collection contains the parameter; otherwise, false.</returns>
        public abstract bool Contains(string name);

        /// <summary>
        /// Gets the location of the <see cref="MySqlParameter"/> in the collection with a specific parameter name.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
        /// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
        public abstract int IndexOf(string parameterName);

        /// <summary>
        /// Removes the specified <see cref="MySqlParameter"/> from the collection using the parameter name.
        /// </summary>
        /// <param name="name">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
        public abstract void RemoveAt(string name);

		[Browsable(false)]
		public DbParameter this[string name] 
		{
			get { return this.GetParameter(name); }
			set { this.SetParameter(name, value); }
		}

		object IDataParameterCollection.this[string name]
		{
			get { return this.GetParameter(name); }
			set { this.SetParameter(name, value); }
		}

        #endregion

        #region IEnumerable

		[Browsable(false)]
        public abstract IEnumerator GetEnumerator();

        #endregion


	}
}

