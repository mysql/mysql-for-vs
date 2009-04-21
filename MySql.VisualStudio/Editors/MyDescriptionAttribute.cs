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

using System.ComponentModel;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Editors
{
    class MyDescriptionAttribute : DescriptionAttribute
    {
        private string rezName;

        public MyDescriptionAttribute(string resourceName)
        {
            rezName = resourceName;
        }

        public override string Description
        {
            get
            {
                return Resources.ResourceManager.GetString(rezName);
            }
        }
    }
}
