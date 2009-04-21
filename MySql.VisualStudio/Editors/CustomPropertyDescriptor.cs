// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using System.ComponentModel;
using System;
using MySql.Data.VisualStudio.DbObjects;

namespace MySql.Data.VisualStudio.Editors
{
    class CustomPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor myProp;
        private bool readOnly;

        public CustomPropertyDescriptor(PropertyDescriptor pd) : 
            base(pd)
        {
            myProp = pd;
        }

        public override bool IsReadOnly
        {
            get { return readOnly; }
        }

        public void SetReadOnly(bool value)
        {
            readOnly = value;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return typeof(Table); }
        }

        public override object GetValue(object component)
        {
            return myProp.GetValue(component);
        }

        public override System.Type PropertyType
        {
            get { return typeof(string); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            myProp.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
