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

using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DbObjects;

namespace MySql.Data.VisualStudio.Editors
{
    class MyDataGridViewRowHeaderCell : DataGridViewRowHeaderCell
    {
        TableNode tableNode;

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
                        Resources.ArrowKey : Resources.Key;
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
