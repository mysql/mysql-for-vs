// Copyright (c) 2008, 2018, Oracle and/or its affiliates. All rights reserved.
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

using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DbObjects;

namespace MySql.Data.VisualStudio.Editors
{
  class MyDataGridViewRowHeaderCell : DataGridViewRowHeaderCell
  {
    private List<Column> Columns
    {
      get { return (DataGridView.DataSource as BindingSource).DataSource as List<Column>; }
    }

    protected override void Paint(Graphics graphics, Rectangle clipBounds,
        Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
        object value, object formattedValue, string errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
    {
      if (Columns.Count > rowIndex)
      {
        Column c = Columns[rowIndex];
        if (c.PrimaryKey)
        {
          Bitmap bmp = rowIndex == DataGridView.CurrentRow.Index ?
              Properties.Resources.ArrowKey : Properties.Resources.Key;
          bmp.MakeTransparent();
          paintParts &= ~DataGridViewPaintParts.ContentBackground;
          base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
              value, formattedValue, errorText, cellStyle, advancedBorderStyle,
              paintParts);
          Rectangle r = cellBounds;
          r.Offset(bmp.Width / 2, bmp.Height / 2);
          r.Width = bmp.Width;
          r.Height = bmp.Height;
          graphics.DrawImage(bmp, r);
          return;
        }
      }

      base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState,
          value, formattedValue, errorText, cellStyle, advancedBorderStyle,
          paintParts);
    }
  }
}
