// Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System.Collections.Generic;

namespace MySql.Utility.Classes.MySqlX
{
  /// <summary>
  /// Allows traversing the Row objects returned by a Table.select BaseShell operation.
  /// </summary>
  public class RowResult : BaseResult
  {
    /// <summary>
    /// Result set data.
    /// </summary>
    public List<Dictionary<string, object>> Data { get; set; }

    /// <summary>
    /// Returns a list of the column names if data is available.
    /// </summary>
    /// <returns>A string list with the column names.</returns>
    public List<string> GetColumnNames()
    {
      var columnNames = new List<string>();
      if (Data == null || Data.Count == 0)
      {
        return columnNames;
      }

      foreach (var item in Data[0])
      {
        columnNames.Add(item.Key);
      }

      return columnNames;
    }
  }
}