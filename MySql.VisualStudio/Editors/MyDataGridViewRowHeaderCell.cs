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
