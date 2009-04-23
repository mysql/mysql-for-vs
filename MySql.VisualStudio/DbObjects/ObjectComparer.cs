// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DbObjects
{
    class ObjectHelper
    {
        public static bool AreEqual(object one, object two)
        {
            if (one == null && two != null) return false;
            if (one != null && two == null) return false;
            Type firstType = one.GetType();
            Type secondType = two.GetType();

            PropertyInfo[] properties = firstType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                object firstValue = property.GetValue(one, null);
                if (!(firstValue is IComparable)) continue;

                object secondValue = property.GetValue(two, null);
                if (!firstValue.Equals(secondValue)) return false;
            }
            return true;
        }

        public static void Copy(object from, object to)
        {
            Type firstType = from.GetType();
            Type secondType = to.GetType();

            PropertyInfo[] properties = firstType.GetProperties(
                BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.SetProperty);
            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite) continue;
                object firstValue = property.GetValue(from, null);
                property.SetValue(to, firstValue, null);
            }
        }
    }
}
