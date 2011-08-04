﻿// Copyright © 2011, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using NUnit.Framework;


namespace MySql.Data.Entity.ModelFirst.Tests
{    
    [TestFixture]
    public class CodeFirstTests : BaseModelFirstTest
    {
        /// <summary>
        /// Tests for fix of http://bugs.mysql.com/bug.php?id=61230
        /// ("The provider did not return a ProviderManifestToken string.").
        /// </summary>
        [Test]
        public void SimpleCodeFirstSelect() 
        {   
            try
            {
                MovieDBContext db = new MovieDBContext();
                var l = db.Movies.ToList();
                foreach (var i in l)
                {
                }
            }
            finally
            {
                suExecSQL("DROP DATABASE IF EXISTS test3");
            }
        }        
    }
}

