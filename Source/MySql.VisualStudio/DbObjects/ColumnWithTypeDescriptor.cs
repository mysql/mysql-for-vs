// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using System.ComponentModel;
using System;
using System.Collections.Generic;
using MySql.Data.VisualStudio.DbObjects;
using MySql.Data.VisualStudio.Editors;

namespace MySql.Data.VisualStudio.DbObjects
{
  class ColumnWithTypeDescriptor : Column, ICustomTypeDescriptor
  {
    public ColumnWithTypeDescriptor()
      : base(null)
    {
    }

    #region ICustomTypeDescriptor Members

    public TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    public string GetComponentName()
    {
      return TypeDescriptor.GetComponentName(this, true);
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    public AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
      PropertyDescriptorCollection coll =
          TypeDescriptor.GetProperties(this, attributes, true);

      List<PropertyDescriptor> props = new List<PropertyDescriptor>();

      foreach (PropertyDescriptor pd in coll)
      {
        if (!pd.IsBrowsable) continue;

        if (pd.Name == "Precision" || pd.Name == "Scale")
        {
          if (DataType != null &&
              DataType.ToLowerInvariant() == "decimal")
            props.Add(pd);
        }
        else if (pd.Name == "CharacterSet" || pd.Name == "Collation")
        {
          CustomPropertyDescriptor newPd = new CustomPropertyDescriptor(pd);
          newPd.SetReadOnly(DataType == null ||
              !Metadata.IsStringType(DataType));
          props.Add(newPd);
        }
        else
          props.Add(pd);
      }
      return new PropertyDescriptorCollection(props.ToArray());
    }

    PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
    {
      return TypeDescriptor.GetProperties(this, true);
    }

    public object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    public string GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    #endregion
  }
}
