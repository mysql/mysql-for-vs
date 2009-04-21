// Copyright © 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Collections.Generic;

namespace MySql.Data.Common
{
    internal class Cache<KeyType, ValueType>
    {
        private int _capacity;
        private Queue<KeyType> _keyQ;
        private Dictionary<KeyType, ValueType> _contents;

        public Cache(int initialCapacity, int capacity)
        {
            _capacity = capacity;
            _contents = new Dictionary<KeyType, ValueType>(initialCapacity);

            if (capacity > 0)
                _keyQ = new Queue<KeyType>(initialCapacity);
        }

        public ValueType this[KeyType key]
        {
            get
            {
                ValueType val;
                if (_contents.TryGetValue(key, out val))
                    return val;
                else
                    return default(ValueType);
            }
            set { InternalAdd(key, value); }
        }

        public void Add(KeyType key, ValueType value)
        {
            InternalAdd(key, value);
        }

        private void InternalAdd(KeyType key, ValueType value)
        {
            if (!_contents.ContainsKey(key))
            {

                if (_capacity > 0)
                {
                    _keyQ.Enqueue(key);

                    if (_keyQ.Count > _capacity)
                        _contents.Remove(_keyQ.Dequeue());
                }
            }

            _contents[key] = value;
        }
    }
}
