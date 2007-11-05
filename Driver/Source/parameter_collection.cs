// Copyright (C) 2004-2007 MySQL AB
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
using System.Data.Common;
using System.Collections;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Represents a collection of parameters relevant to a <see cref="MySqlCommand"/> as well as their respective mappings to columns in a <see cref="System.Data.DataSet"/>. This class cannot be inherited.
	/// </summary>
	/// <include file='docs/MySqlParameterCollection.xml' path='MyDocs/MyMembers[@name="Class"]/*'/>
#if !CF
	[Editor("MySql.Data.MySqlClient.Design.DBParametersEditor,MySql.Design", typeof(System.Drawing.Design.UITypeEditor))]
	[ListBindable(true)]
#endif
	public sealed class MySqlParameterCollection : DbParameterCollection
	{
        private ArrayList items = new ArrayList();
        private Hashtable indexHash;
		private char paramMarker = '?';

		internal MySqlParameterCollection()
		{
#if NET20
			indexHash = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
#else
			indexHash = new Hashtable(new CaseInsensitiveHashCodeProvider(),
			    new CaseInsensitiveComparer());
#endif
            Clear();
		}

		internal char ParameterMarker
		{
			get { return paramMarker; }
			set { paramMarker = value; }
		}

		#region Public Methods

		/// <summary>
		/// Gets the <see cref="MySqlParameter"/> at the specified index.
		/// </summary>
		/// <overloads>Gets the <see cref="MySqlParameter"/> with a specified attribute.
		/// [C#] In C#, this property is the indexer for the <see cref="MySqlParameterCollection"/> class.
		/// </overloads>
		public new MySqlParameter this[int index]
		{
			get { return (MySqlParameter)GetParameter(index); }
			set { SetParameter(index, value); }
		}

		/// <summary>
		/// Gets the <see cref="MySqlParameter"/> with the specified name.
		/// </summary>
		public new MySqlParameter this[string name]
		{
			get { return (MySqlParameter)GetParameter(name); }
			set { SetParameter(name, value); }
		}

		/// <summary>
		/// Adds the specified <see cref="MySqlParameter"/> object to the <see cref="MySqlParameterCollection"/>.
		/// </summary>
		/// <param name="value">The <see cref="MySqlParameter"/> to add to the collection.</param>
		/// <returns>The newly added <see cref="MySqlParameter"/> object.</returns>
		public MySqlParameter Add(MySqlParameter value)
		{
            return InternalAdd(value, -1);
		}

		/// <summary>
		/// Adds a <see cref="MySqlParameter"/> to the <see cref="MySqlParameterCollection"/> given the specified parameter name and value.
		/// </summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="value">The <see cref="MySqlParameter.Value"/> of the <see cref="MySqlParameter"/> to add to the collection.</param>
		/// <returns>The newly added <see cref="MySqlParameter"/> object.</returns>
		[Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value)")]
        public MySqlParameter Add(string parameterName, object value)
		{
			return Add(new MySqlParameter(parameterName, value));
		}

        public MySqlParameter AddWithValue(string parameterName, object value)
        {
            return Add(new MySqlParameter(parameterName, value));
        }

		/// <summary>
		/// Adds a <see cref="MySqlParameter"/> to the <see cref="MySqlParameterCollection"/> given the parameter name and the data type.
		/// </summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/> values. </param>
		/// <returns>The newly added <see cref="MySqlParameter"/> object.</returns>
		public MySqlParameter Add(string parameterName, MySqlDbType dbType)
		{
			return Add(new MySqlParameter(parameterName, dbType));
		}

		/// <summary>
		/// Adds a <see cref="MySqlParameter"/> to the <see cref="MySqlParameterCollection"/> with the parameter name, the data type, and the column length.
		/// </summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/> values. </param>
		/// <param name="size">The length of the column.</param>
		/// <returns>The newly added <see cref="MySqlParameter"/> object.</returns>
		public MySqlParameter Add(string parameterName, MySqlDbType dbType, int size)
		{
			return Add(new MySqlParameter(parameterName, dbType, size));
		}

		/// <summary>
		/// Adds a <see cref="MySqlParameter"/> to the <see cref="MySqlParameterCollection"/> with the parameter name, the data type, the column length, and the source column name.
		/// </summary>
		/// <param name="parameterName">The name of the parameter.</param>
		/// <param name="dbType">One of the <see cref="MySqlDbType"/> values. </param>
		/// <param name="size">The length of the column.</param>
		/// <param name="sourceColumn">The name of the source column.</param>
		/// <returns>The newly added <see cref="MySqlParameter"/> object.</returns>
		public MySqlParameter Add(string parameterName, MySqlDbType dbType, int size, string sourceColumn)
		{
			return Add(new MySqlParameter(parameterName, dbType, size, sourceColumn));
		}

		#endregion

		#region DbParameterCollection Implementation

        /// <summary>
        /// Adds an array of values to the end of the <see cref="MySqlParameterCollection"/>. 
        /// </summary>
        /// <param name="values"></param>
		public override void AddRange(Array values)
		{
			foreach (DbParameter p in values)
				Add(p);
		}

		void CheckIndex(int index)
		{
			if (index < 0 || index >= Count)
				throw new IndexOutOfRangeException("Parameter index is out of range.");
		}

		/// <summary>
		/// Retrieve the parameter with the given name.
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		protected override DbParameter GetParameter(string parameterName)
		{
			int index = IndexOf(parameterName);
			if (index < 0)
			{
				// check to see if the user has added the parameter without a
				// parameter marker.  If so, kindly tell them what they did.
				if (parameterName.StartsWith(ParameterMarker.ToString()))
				{
					string newParameterName = parameterName.Substring(1);
					index = IndexOf(newParameterName);
					if (index != -1)
                        return (DbParameter)items[index];
				}
				throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
			}
			return (DbParameter)items[index];
		}

		protected override DbParameter GetParameter(int index)
		{
			CheckIndex(index);
			return (DbParameter)items[index];
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			int index = IndexOf(parameterName);
			if (index < 0)
				throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
            SetParameter(index, value);
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			CheckIndex(index);
            MySqlParameter p = (MySqlParameter)items[index];
            
            indexHash.Remove(p.ParameterName);
			items[index] = value;
            indexHash.Add(value.ParameterName, index);
        }

		/// <summary>
		/// Adds the specified <see cref="MySqlParameter"/> object to the <see cref="MySqlParameterCollection"/>.
		/// </summary>
		/// <param name="value">The <see cref="MySqlParameter"/> to add to the collection.</param>
		/// <returns>The index of the new <see cref="MySqlParameter"/> object.</returns>
		public override int Add(object value)
		{
			if (!(value is MySqlParameter))
				throw new MySqlException("Only MySqlParameter objects may be stored");

			MySqlParameter p = (MySqlParameter)value;

			if (p.ParameterName == null || p.ParameterName == String.Empty)
				throw new MySqlException("Parameters must be named");

			p = Add(p);
			return IndexOf(p);
		}

		/// <summary>
		/// Removes all items from the collection.
		/// </summary>
		public override void Clear()
		{
            foreach (MySqlParameter p in items)
                p.Collection = null;
			items.Clear();
			indexHash.Clear();
		}

		/// <summary>
		/// Gets a value indicating whether a <see cref="MySqlParameter"/> with the specified parameter name exists in the collection.
		/// </summary>
		/// <param name="parameterName">The name of the <see cref="MySqlParameter"/> object to find.</param>
		/// <returns>true if the collection contains the parameter; otherwise, false.</returns>
		public override bool Contains(string parameterName)
		{
			return IndexOf(parameterName) != -1;
		}

		/// <summary>
		/// Gets a value indicating whether a MySqlParameter exists in the collection.
		/// </summary>
		/// <param name="value">The value of the <see cref="MySqlParameter"/> object to find. </param>
		/// <returns>true if the collection contains the <see cref="MySqlParameter"/> object; otherwise, false.</returns>
		/// <overloads>Gets a value indicating whether a <see cref="MySqlParameter"/> exists in the collection.</overloads>
		public override bool Contains(object value)
		{
			return items.Contains(value);
		}

		/// <summary>
		/// Copies MySqlParameter objects from the MySqlParameterCollection to the specified array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public override void CopyTo(Array array, int index)
		{
			items.CopyTo(array, index);
		}

		/// <summary>
		/// Gets the number of MySqlParameter objects in the collection.
		/// </summary>
		public override int Count
		{
			get { return items.Count; }
		}

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="MySqlParameterCollection"/>. 
        /// </summary>
        /// <returns></returns>
		public override IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}

		/// <summary>
		/// Gets the location of the <see cref="MySqlParameter"/> in the collection with a specific parameter name.
		/// </summary>
		/// <param name="parameterName">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
		/// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
		public override int IndexOf(string parameterName)
		{
            object o = indexHash[parameterName];
            if (o == null)
                return -1;
            return (int)o;
		}

		/// <summary>
		/// Gets the location of a <see cref="MySqlParameter"/> in the collection.
		/// </summary>
		/// <param name="value">The <see cref="MySqlParameter"/> object to locate. </param>
		/// <returns>The zero-based location of the <see cref="MySqlParameter"/> in the collection.</returns>
		/// <overloads>Gets the location of a <see cref="MySqlParameter"/> in the collection.</overloads>
		public override int IndexOf(object value)
		{
			return items.IndexOf(value);
		}

		/// <summary>
		/// Inserts a MySqlParameter into the collection at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public override void Insert(int index, object value)
		{
            if (!(value is MySqlParameter))
                throw new MySqlException("Only MySqlParameter objects may be stored");
            InternalAdd((MySqlParameter)value, index);
		}

        /// <summary>
        /// Gets a value that indicates whether the <see cref="MySqlParameterCollection"/>
        /// has a fixed size. 
        /// </summary>
		public override bool IsFixedSize
		{
			get { return items.IsFixedSize; }
		}

        /// <summary>
        /// Gets a value that indicates whether the <see cref="MySqlParameterCollection"/>
        /// is read-only. 
        /// </summary>
		public override bool IsReadOnly
		{
			get { return items.IsReadOnly; }
		}

        /// <summary>
        /// Gets a value that indicates whether the <see cref="MySqlParameterCollection"/>
        /// is synchronized. 
        /// </summary>
		public override bool IsSynchronized
		{
			get { return items.IsSynchronized; }
		}

		/// <summary>
		/// Removes the specified MySqlParameter from the collection.
		/// </summary>
		/// <param name="value"></param>
		public override void Remove(object value)
		{
            MySqlParameter p = (value as MySqlParameter);
            p.Collection = null;
            int index = IndexOf(p);
			items.Remove(p);
            indexHash.Remove(p.ParameterName);
            AdjustHash(index, false);
		}

		/// <summary>
		/// Removes the specified <see cref="MySqlParameter"/> from the collection using the parameter name.
		/// </summary>
		/// <param name="parameterName">The name of the <see cref="MySqlParameter"/> object to retrieve. </param>
		public override void RemoveAt(string parameterName)
		{
			DbParameter p = GetParameter(parameterName);
			Remove(p);
		}

		/// <summary>
		/// Removes the specified <see cref="MySqlParameter"/> from the collection using a specific index.
		/// </summary>
		/// <param name="index">The zero-based index of the parameter. </param>
		/// <overloads>Removes the specified <see cref="MySqlParameter"/> from the collection.</overloads>
		public override void RemoveAt(int index)
		{
            object o = items[index];
            Remove(o);
		}

        /// <summary>
        /// Gets an object that can be used to synchronize access to the 
        /// <see cref="MySqlParameterCollection"/>. 
        /// </summary>
		public override object SyncRoot
		{
			get { return items.SyncRoot; }
		}

		#endregion

        internal void ParameterNameChanged(MySqlParameter p, string oldName, string newName)
        {
            int index = IndexOf(oldName);
            indexHash.Remove(oldName);
            indexHash.Add(newName, index);
        }

        private MySqlParameter InternalAdd(MySqlParameter value, int index)
        {
            if (value == null)
                throw new ArgumentException("The MySqlParameterCollection only accepts non-null MySqlParameter type objects.", "value");

            // make sure we don't already have a parameter with this name
            string inComingName = value.ParameterName;
            if (indexHash.ContainsKey(inComingName))
                throw new MySqlException(
                    String.Format(Resources.ParameterAlreadyDefined, value.ParameterName));
            if (inComingName[0] == paramMarker)
                inComingName = inComingName.Substring(1, inComingName.Length - 1);
            if (indexHash.ContainsKey(inComingName))
                throw new MySqlException(
                    String.Format(Resources.ParameterAlreadyDefined, value.ParameterName));

            if (index == -1)
            {
                index = items.Add(value);
                indexHash.Add(value.ParameterName, index);
            }
            else
            {
                items.Insert(index, value);
                AdjustHash(index, true);
                indexHash.Add(value.ParameterName, index);
            }

            value.Collection = this;
            return value;
        }

        private void AdjustHash(int keyIndex, bool addEntry)
        {
            for (int i=0; i < Count; i++)
            {
                MySqlParameter p = (MySqlParameter)items[i];
                if (!indexHash.ContainsKey(p.ParameterName))
                    return;
                int index = (int)indexHash[p.ParameterName];
                if (index < keyIndex) continue;
                indexHash[p.ParameterName] = addEntry ? ++index : --index;
            }
        }
	}
}
