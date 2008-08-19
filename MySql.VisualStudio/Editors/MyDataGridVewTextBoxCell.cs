using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MySql.Data.VisualStudio.Editors
{
    class MyDataGridViewTextBoxColumn : DataGridViewTextBoxColumn
    {
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return new MyDataGridViewTextBoxCell();
            }
            set
            {
                base.CellTemplate = value;
            }
        }
    }

    class MyDataGridViewTextBoxCell :DataGridViewTextBoxCell
    {
        public override void PositionEditingControl(bool setLocation, bool setSize, 
            Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, 
            bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, 
            bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
        {
            Rectangle editingControlBounds = PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
            DataGridViewTextBoxEditingControl ec = (DataGridView.EditingControl as DataGridViewTextBoxEditingControl);
            ec.Dock = DockStyle.Fill;
            ec.BorderStyle = BorderStyle.Fixed3D;
            ec.Multiline = true;
        }

        public override Rectangle PositionEditingPanel(Rectangle cellBounds, Rectangle cellClip, 
            DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, 
            bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, 
            bool isFirstDisplayedRow)
        {
            Rectangle r = base.PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);

            Rectangle bounds = base.DataGridView.EditingPanel.Bounds;
            bounds.Offset(-1, -1);
            bounds.Width += 2;
            bounds.Height += 2;
            base.DataGridView.EditingPanel.Location = bounds.Location;
            base.DataGridView.EditingPanel.Size = bounds.Size;
            r.Width += 2;
            r.Height += 2;
            return r;
        }
    }

    class MyDataGridViewComboBoxColumn : DataGridViewComboBoxColumn
    {
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return new MyDataGridViewComboBoxCell();
            }
            set
            {
                base.CellTemplate = value;
            }
        }
    }

    class MyDataGridViewComboBoxCell : DataGridViewComboBoxCell
    {
        public MyDataGridViewComboBoxCell()
            : base()
        {
        }

        public override void PositionEditingControl(bool setLocation, bool setSize,
            Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle,
            bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded,
            bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
        {
            Rectangle editingControlBounds = PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
            base.DataGridView.EditingControl.Dock = DockStyle.Fill;
        }

        public override Rectangle PositionEditingPanel(Rectangle cellBounds, Rectangle cellClip,
            DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded,
            bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn,
            bool isFirstDisplayedRow)
        {
            Rectangle r = base.PositionEditingPanel(cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);

            Rectangle bounds = base.DataGridView.EditingPanel.Bounds;
            bounds.Offset(-1, -1);
            bounds.Width += 2;
            bounds.Height += 2;
            base.DataGridView.EditingPanel.Location = bounds.Location;
            base.DataGridView.EditingPanel.Size = bounds.Size;
            base.DataGridView.EditingPanel.BackColor = Color.Red;
            r.Width += 2;
            r.Height += 2;
            return r;
        }
    }

}
