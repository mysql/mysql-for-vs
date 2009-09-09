// Copyright (c) 2009 Sun Microsystems, Inc.
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

namespace MySql.Data.MySqlClient
{
#if CF

    class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string cat) { }
    }

    class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string desc) { }
    }

    class DisplayNameAttribute : Attribute
    {
        private string displayName;

        public DisplayNameAttribute(string name)
        {
            displayName = name;
        }

        public string DisplayName
        {
            get { return displayName; }
        }
    }

    class RefreshPropertiesAttribute : Attribute
    {
        public RefreshPropertiesAttribute(RefreshProperties prop) { }
    }

    class PasswordPropertyTextAttribute : Attribute
    {
        public PasswordPropertyTextAttribute(bool v) { }
    }

    public enum RefreshProperties
    {
        None = 0,
        All = 1,
        Repaint = 2,
    }

#endif
}
