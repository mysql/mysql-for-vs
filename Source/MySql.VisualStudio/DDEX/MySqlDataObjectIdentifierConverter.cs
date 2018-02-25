// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
  public class MySqlDataObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
  {
    private DataConnection connection;

    public MySqlDataObjectIdentifierConverter(DataConnection c)
      : base(c)
    {
      connection = c;
    }

    protected override string BuildString(string typeName, string[] identifierParts, bool forDisplay)
    {
      string id = String.Empty;

      if (typeName == "Table")
      {
        DbConnection c = connection.ConnectionSupport.ProviderObject as DbConnection;
        string dbName = FormatPart("Table", c.Database, true);

        if (identifierParts.Length == 1)
          id = identifierParts[0];
        if (identifierParts.Length == 2 && identifierParts[0] == dbName)
          id = identifierParts[1];
        if (identifierParts.Length == 3 && identifierParts[1] == dbName)
          id = identifierParts[2];
      }
      if (id == String.Empty || forDisplay)
        id = base.BuildString(typeName, identifierParts, forDisplay);
      return id;
    }
  }
}
