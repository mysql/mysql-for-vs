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

namespace MySql.Data.VisualStudio.DbObjects
{
  internal interface ITablePart
  {
    string GetDropSql();
    string GetSql(bool isNew);
    bool HasChanges();
    bool IsNew();
    void Saved();
  }

  internal class TablePartCollection<T> : List<T>
  {
    public TablePartCollection()
    {
      Deleted = new List<T>();
    }

    private List<T> _deleted;
    public List<T> Deleted
    {
      get { return _deleted; }
      private set { _deleted = value; }
    }

    public void Delete(T t)
    {
      Deleted.Add(t);
    }

    public void Delete(int index)
    {
      ITablePart part = (ITablePart)this[index];
      RemoveAt(index);
      if (!part.IsNew())
        Deleted.Add((T)part);
    }

    public bool HasChanges()
    {
      if (Deleted.Count > 0) return true;
      foreach (ITablePart part in this)
        if (part.HasChanges()) return true;
      return false;
    }

    public void Saved()
    {
      Deleted.Clear();
      foreach (ITablePart part in this)
        part.Saved();
    }

    public string GetSql(bool newTable)
    {
      List<string> parts = new List<string>();

      if (!newTable)
        foreach (ITablePart part in Deleted)
          parts.Add(part.GetDropSql());

      // process new columns
      for (int i = 0; i < Count; i++)
      {
        ITablePart part = (ITablePart)this[i];
        string partSql = part.GetSql(newTable);
        if (!String.IsNullOrEmpty(partSql))
        {
          if (part is Column && !newTable)
          {
            if (i == 0)
              partSql += " FIRST";
            else if (i < Count - 1)
            {
              Column c = this[i - 1] as Column;
              partSql += String.Format(" AFTER {0}", c.ColumnName);
            }
          }
          parts.Add(partSql);
        }
      }

      string delimiter = "";
      StringBuilder sql = new StringBuilder();
      foreach (string s in parts)
      {
        sql.AppendFormat("{0}{1}", delimiter, s);
        delimiter = ", ";
      }
      return sql.ToString();
    }
  }
}
