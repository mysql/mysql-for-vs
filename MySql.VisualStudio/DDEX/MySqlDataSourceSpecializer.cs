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

/*
 * This file contains stub for data connection specializer.
 */
using System;
using Microsoft.VisualStudio.Data;
using System.Reflection;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class needed only as a stub to get prompt dialog to work. Its methods
    /// are never called.
    /// </summary>
    class MySqlDataSourceSpecializer: DataSourceSpecializer
    {
        public override object CreateObject(Guid dataSource, Type objType)
        {
            object result = base.CreateObject(dataSource, objType);
            return result;
        }

        public override Guid DeriveDataSource(string connectionString)
        {
            Guid result = base.DeriveDataSource(connectionString);
            return result;
        }

        public override Assembly GetAssembly(Guid dataSource, string assemblyString)
        {
            Assembly result = base.GetAssembly(dataSource, assemblyString);
            return result;
        }
    }
}
