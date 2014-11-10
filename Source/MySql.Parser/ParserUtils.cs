// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Parser
{
  public static class ParserUtils
  {
    public static TableWithAlias ExtractTableName(ITree child)
    {
      string alias = "";
      string table = "";
      string db = "";

      switch (child.ChildCount)
      {
        case 1:
          table = child.GetChild(0).Text;
          break;
        case 2:
          if (string.Compare(child.GetChild(1).Text, "alias", true) == 0)
          {
            table = child.GetChild(0).Text;
            alias = child.GetChild(1).GetChild(0).Text;
          }
          else
          {
            db = child.GetChild(0).Text;
            table = child.GetChild(1).Text;
          }
          break;
        case 3:
          db = child.GetChild(0).Text;
          table = child.GetChild(1).Text;
          alias = child.GetChild(2).GetChild(0).Text;
          break;
      }
      return new TableWithAlias(db, table, alias);
    }

    public static void GetTables(ITree ct, List<TableWithAlias> tables)
    {
      for (int i = 0; i < ct.ChildCount; i++)
      {
        ITree child = ct.GetChild(i);
        if (child.Text.Equals( "table_ref", StringComparison.OrdinalIgnoreCase ))
        {
          tables.Add( ExtractTableName( child ) );
        }
        else GetTables(child, tables);
      }
    }

    public static Version GetVersion(string versionString)
    {
      Version version;
      int i = 0;
      while (i < versionString.Length &&
          (Char.IsDigit(versionString[i]) || versionString[i] == '.'))
        i++;
      version = new Version(versionString.Substring(0, i));
      return version;
    }
  }

  public class TableWithAlias : IEquatable<TableWithAlias>
  {
    public TableWithAlias(string TableName)
      : this("", TableName, "")
    {
    }

    public TableWithAlias(string TableName, string Alias)
      : this("", TableName, Alias)
    {
    }

    public TableWithAlias(string Database, string TableName, string Alias)
    {
      this.Database = Database.Replace("`", "").ToLower();
      this.TableName = TableName.Replace("`", "").ToLower();
      this.Alias = Alias.Replace("`", "");
    }

    public string TableName { get; private set; }
    public string Alias { get; private set; }
    public string Database { get; private set; }

    public bool Equals(TableWithAlias other)
    {
      if (other == null) return false;      
      return
        (other.TableName.Equals(this.TableName, StringComparison.CurrentCultureIgnoreCase)) &&
        (other.Alias.Equals(this.Alias, StringComparison.CurrentCultureIgnoreCase)) &&
        (other.Database.Equals(this.Database, StringComparison.CurrentCultureIgnoreCase));
    }
  }
}
