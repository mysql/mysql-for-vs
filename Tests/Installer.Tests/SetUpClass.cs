// Copyright © 2013 Oracle and/or its affiliates. All rights reserved.
//
// MySQL For Visual Studio is licensed under the terms of the GPLv2
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace Installer.Tests
{
  public class SetUpClass : IDisposable
  {
    //private const string PATH_MSI = @"C:\src\mainsource2\toolsplugin-1.0\Installer\bin\GPL\MySql.VisualStudio.Plugin.msi";
    private const string PATH_MSI = @"..\..\..\..\Installer\bin\GPL\MySql.VisualStudio.Plugin.msi";
    private Database db;

    public SetUpClass()
    {
      // Open MSI 
      db = new Database( Path.GetFullPath( PATH_MSI ));
    }

    public void GetValue( string query, out string recordValue )
    {
      IList l = db.ExecuteQuery(query);
      if (l.Count != 0)
      {
        recordValue = l[0].ToString();
      }
      else
      {
        recordValue = null;
      }
    }

    public virtual void Dispose()
    {
      db.Close();
    }
  }
}
