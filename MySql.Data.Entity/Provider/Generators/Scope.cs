// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Linq;
using System.Text;

namespace MySql.Data.Entity
{
    class Scope
    {
        private Dictionary<string, InputFragment> scopeTable = new Dictionary<string, InputFragment>();

        public void Add(string name, InputFragment fragment)
        {
            scopeTable.Add(name, fragment);
        }

        public void Remove(InputFragment fragment)
        {
            if (fragment == null) return;
            if (fragment.Name != null)
                scopeTable.Remove(fragment.Name);

            if (fragment is SelectStatement)
                Remove((fragment as SelectStatement).From);
            else if (fragment is JoinFragment)
            {
                JoinFragment j = fragment as JoinFragment;
                Remove(j.Left);
                Remove(j.Right);
            }
            else if (fragment is UnionFragment)
            {
                UnionFragment u = fragment as UnionFragment;
                Remove(u.Left);
                Remove(u.Right);
            }
        }

        public InputFragment GetFragment(string name)
        {
            if (!scopeTable.ContainsKey(name))
                return null;
            return scopeTable[name];
        }
    }
}
