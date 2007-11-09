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

using System.Collections.Specialized;
using System;

namespace MySql.Data.Common
{
    internal class Cache : NameObjectCollectionBase
    {
        private int capacity;

        public Cache(int initialCapacity, int capacity) :
            base(initialCapacity, StringComparer.CurrentCulture)
		{
			this.capacity = capacity;
		}

        public object this[string key]
        {
            get { return BaseGet(key); }
            set { InternalAdd(key, value); }
        }

        public void Add(string key, object value)
        {
            InternalAdd(key, value);
        }

        private void InternalAdd(string key, object value)
        {
            if (base.Count == capacity)
                RemoveOldestItem();
            BaseAdd(key, value);
        }

        private void RemoveOldestItem()
        {
            BaseRemoveAt(0);
        }
    }
}
