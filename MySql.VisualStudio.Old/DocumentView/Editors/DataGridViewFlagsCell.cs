// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of the custom cell type for flags column in the columns editor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Data;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is a custom cell type for flags column in the columns editor.
    /// </summary>
    class DataGridViewFlagsCell : DataGridViewCell, IDataGridViewEditingCell
    {  
        #region Public properties
        /// <summary>
        /// Gets editor type for this cell type.
        /// </summary>
        public override Type EditType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets value type for this cell type.
        /// </summary>
        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        /// Gets default new row value for this cell type.
        /// </summary>
        public override object DefaultNewRowValue
        {
            get
            {
                return String.Empty;
            }
        } 
        #endregion

        #region Columns row extraction and oter attributes
        /// <summary>
        /// Returns DataRow object for this editor.
        /// </summary>
        protected DataRow GetColumnRow(int rowIndex)
        {

            // Check if have enougth data for initial processing
            if (DataGridView == null
                || DataGridView.Rows == null
                || rowIndex < 0
                || rowIndex >= DataGridView.RowCount)
                return null;

            // Extract DataGridViewRow, DataRowView and DataRow
            DataGridViewRow gridRow = DataGridView.Rows[rowIndex];
            DataRowView rowView = gridRow != null ? gridRow.DataBoundItem as DataRowView : null;
            DataRow row = rowView != null ? rowView.Row : null;

            // Check results for emptines
            if (row == null || row.Table == null || row.Table.Columns == null)
                return null;

            // Check for available columns
            DataColumnCollection columns = row.Table.Columns;
            if (!columns.Contains(Column.MySqlType)
                || !columns.Contains(Column.Unsigned)
                || !columns.Contains(Column.Zerofill)
                || !columns.Contains(Column.Binary)
                || !columns.Contains(Column.Ascii)
                || !columns.Contains(Column.Unicode))
            {
                Debug.Fail("Wrong table structure!");
                return null;
            }

            return row;
        }
        #endregion

        #region Flags filling
        /// <summary>
        /// Builds supported flags list for the given column datatype.
        /// </summary>
        /// <param name="dataType">Column datatype.</param>
        private void BuildFlags(int rowIndex)
        {
            DataRow column = GetColumnRow(rowIndex);
            string dataType = column != null ? DataInterpreter.GetStringNotNull(column, Column.MySqlType) : String.Empty;
            
            flags.Clear();

            // If numeric, add UNSIGNED and ZEROFILL
            if (Parser.SupportUnsignedAndZerofill(dataType))
                AddNumericFlags();

            // If supports BINARY, add it
            if (Parser.SupportBinary(dataType))
                AddBinaryFlag();

            // If supports ASCII and UNICODE, add them
            if (Parser.SupportAsciiAndUnicode(dataType))
                AddAsciiAndUnicode();

            // Fill flags with data
            FillFlags(column);
        }

        /// <summary>
        /// Set state of each flag to checked or unchecked.
        /// </summary>
        private void FillFlags(DataRow column)
        {
            string[] keys = new string[flags.Count];
            flags.Keys.CopyTo(keys, 0);
            // Set checking sate for all items
            foreach (string key in keys)
            {
                // Item is checked depending on the value in the data row
                flags[key] = column != null && DataInterpreter.GetSqlBool(column, key).IsTrue;
            }
        }

        /// <summary>
        /// Adds numeric flags (UNSIGNED and ZEROFILL)
        /// </summary>
        private void AddNumericFlags()
        {
            flags.Add(Column.Unsigned, false);
            flags.Add(Column.Zerofill, false);
        }

        /// <summary>
        /// Adds BINARY option flag.
        /// </summary>
        private void AddBinaryFlag()
        {
            flags.Add(Column.Binary, false);
        }

        /// <summary>
        /// Adds ASCII and UNICODE flags.
        /// </summary>
        private void AddAsciiAndUnicode()
        {
            flags.Add(Column.Ascii, false);
            flags.Add(Column.Unicode, false);
        }
        #endregion

        #region Painting and layout calculation
        /// <summary>
        /// Computes the layout of the cell and optionally paints it.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell that is being painted.</param>
        /// <returns>Returns size required for cell content.</returns>
        private Size ComputeLayout(int rowIndex)
        {
            if (this.DataGridView == null)
                return Size.Empty;

            // Finally compute the layout of the cell and return the resulting content bounds.
            return ComputeLayout(
                    this.DataGridView.CreateGraphics(), 
                    GetInheritedStyle(null, rowIndex, false /* includeColors */), 
                    rowIndex);
        }

        /// <summary>
        /// Computes the layout of the cell and optionally paints it.
        /// </summary>
        /// <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
        /// <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
        /// <param name="rowIndex">The row index of the cell that is being painted.</param>
        /// <returns>Returns size required for cell content.</returns>
        private Size ComputeLayout(Graphics graphics,                                   
                                   DataGridViewCellStyle cellStyle,
                                   int rowIndex)
        {
            if (this.DataGridView == null)
                return Size.Empty;

            // First determine the effective border style of this cell.
            bool singleVerticalBorderAdded = !this.DataGridView.RowHeadersVisible
                && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool singleHorizontalBorderAdded = !this.DataGridView.ColumnHeadersVisible
                && this.DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();

            // Calculates actual advanced border style
            DataGridViewAdvancedBorderStyle dgvabsEffective = AdjustCellBorderStyle(this.DataGridView.AdvancedCellBorderStyle,
                dataGridViewAdvancedBorderStylePlaceholder,
                singleVerticalBorderAdded,
                singleHorizontalBorderAdded,
                this.ColumnIndex == this.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Displayed).Index /*isFirstDisplayedColumn*/,
                rowIndex == this.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Displayed) /*isFirstDisplayedRow*/);

            // Next determine the state of this cell.
            DataGridViewElementStates rowState = this.DataGridView.Rows.GetRowState(rowIndex);
            DataGridViewElementStates cellState = CellStateFromColumnRowStates(rowState);
            cellState |= this.State;

            // Then the bounds of this cell.
            Rectangle cellBounds = new Rectangle(new Point(0, 0), GetSize(rowIndex));

            // Finally compute the layout of the cell and return the resulting content bounds.
            return ComputeLayout(   graphics, 
                                    cellStyle, 
                                    rowIndex, 
                                    cellBounds, 
                                    cellBounds, 
                                    cellState, 
                                    dgvabsEffective, 
                                    DataGridViewPaintParts.ContentForeground, 
                                    false /*paint*/); 
        }

        /// <summary>
        /// Computes the layout of the cell and optionally paints it.
        /// </summary>
        /// <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
        /// <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
        /// <param name="rowIndex">The row index of the cell that is being painted.</param>
        /// <param name="clipBounds">A Rectangle that represents the area of the DataGridView that needs to be repainted.</param>
        /// <param name="cellBounds">A Rectangle that contains the bounds of the DataGridViewCell that is being painted.</param>
        /// <param name="cellState">A bitwise combination of DataGridViewElementStates values that specifies the state of the cell.</param>
        /// <param name="advancedBorderStyle">
        /// A DataGridViewAdvancedBorderStyle that contains border styles for the cell that is being painted.
        /// </param>
        /// <param name="paintParts">
        /// A bitwise combination of the DataGridViewPaintParts values that specifies which parts of the cell need to be painted.
        /// </param>
        /// <param name="paint">True if cell should be painted.</param>
        /// <returns>Returns size required for cell content.</returns>
        private Size ComputeLayout(Graphics graphics, 
                                   DataGridViewCellStyle cellStyle, 
                                   int rowIndex, 
                                   Rectangle clipBounds, 
                                   Rectangle cellBounds, 
                                   DataGridViewElementStates cellState, 
                                   DataGridViewAdvancedBorderStyle advancedBorderStyle, 
                                   DataGridViewPaintParts paintParts, 
                                   bool paint)
        {
            // Fills flags anf their values
            BuildFlags(rowIndex);

            // Clear checkboxes list
            checkboxes.Clear();

            // If border should be painted, pain it using base method
            if (paint && NeedPaintPart(paintParts, DataGridViewPaintParts.Border))
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

            // Discard the space taken up by the borders.
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            Rectangle valBounds = cellBounds;
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            // Size of content
            Size content = new Size(0, cellStyle.Font.Height + 1);

            // Brush to use
            SolidBrush backgroundBrush = null;
            try
            {
                // Create background brush for selected or not celected cell.
                bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
                backgroundBrush = new SolidBrush(cellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor);

                // If we need to paint background, paint it
                if (paint && NeedPaintPart(paintParts, DataGridViewPaintParts.Background))
                {
                    Rectangle backgroundRect = valBounds;
                    backgroundRect.Intersect(clipBounds);
                    graphics.FillRectangle(backgroundBrush, backgroundRect);
                }

                // Discard the space taken up by the padding area.
                if (cellStyle.Padding != Padding.Empty)
                {
                    valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                    valBounds.Width -= cellStyle.Padding.Horizontal;
                    valBounds.Height -= cellStyle.Padding.Vertical;
                }

                // Discard the margin
                valBounds.Inflate(-CheckBoxMargin, -CheckBoxMargin);

                // Layout / paint the checkboxes themselves
                if (valBounds.Width > 0 && valBounds.Height > 0)
                {
                    Rectangle checkBoxBounds = valBounds;
                    int itemWidth;
                    // Iterate 
                    foreach (KeyValuePair<string, bool> item in flags)
                    {
                        Rectangle itemRect = checkBoxBounds;
                        itemRect.Intersect(clipBounds);
                        itemWidth = PaintItem(
                                graphics,
                                checkBoxBounds,
                                cellStyle,
                                item.Key,
                                item.Value,
                                paint && NeedPaintPart(paintParts, DataGridViewPaintParts.ContentBackground) && !itemRect.IsEmpty);
                        
                        // Adjust bounds
                        checkBoxBounds.X += itemWidth;
                        checkBoxBounds.Width -= itemWidth;

                        // Adjust content size
                        content.Width += itemWidth;
                    }
                }

                // Append margin and padding
                content.Width += 4 * CheckBoxMargin + cellStyle.Padding.Right;

                // Return content size
                return content;
            }
            finally
            {
                if (backgroundBrush != null)
                {
                    backgroundBrush.Dispose();
                }
            }
        }

        /// <summary>
        /// Paint single flags item.
        /// </summary>
        /// <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
        /// <param name="checkBoxBounds">Rectangel that contains the bounds for checkbox</param>
        /// <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
        /// <param name="text">Text of item to paint.</param>
        /// <param name="state">Check state of item to paint.</param>
        /// <param name="paint">True if check box should be painted.</param>
        /// <returns>Returns the end (right coordinate) of the drawed checkbox.</returns>
        private int PaintItem(Graphics graphics,
                               Rectangle checkBoxBounds,
                               DataGridViewCellStyle cellStyle,
                               string text,
                               bool state,
                               bool paint)
        {
            //Calculate check box location.
            Point glyphLocation = new Point(checkBoxBounds.Left + CheckBoxMargin, checkBoxBounds.Top);

            // Initial flags to use for text in checkbox.
            TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping 
                | TextFormatFlags.NoPrefix;

            // Determine if should paint as hot flag
            bool isHot = DataInterpreter.CompareInvariant(text, hotItem);
            //Trace.WriteLine("Item " + text + " " + isHot.ToString());
            //Trace.WriteLine("Hot item " + hotItem);
            CheckBoxState cbState =
                state ?
                (isHot ? CheckBoxState.CheckedHot : CheckBoxState.CheckedNormal) :
                (isHot ? CheckBoxState.UncheckedHot : CheckBoxState.UncheckedNormal);

            // Calculate check box glyph size.
            Size checkBoxSize = CheckBoxRenderer.GetGlyphSize(graphics, cbState);   


            // Apply alingment
            switch (cellStyle.Alignment)
            {
                case DataGridViewContentAlignment.BottomCenter:
                    glyphLocation.Y = checkBoxBounds.Bottom;
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.BottomLeft:
                    glyphLocation.Y = checkBoxBounds.Bottom;
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case DataGridViewContentAlignment.BottomRight:
                    glyphLocation.Y = checkBoxBounds.Bottom;
                    flags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case DataGridViewContentAlignment.MiddleCenter:
                    glyphLocation.Y = checkBoxBounds.Top + (checkBoxBounds.Height - checkBoxSize.Height) / 2;
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.MiddleLeft:
                    glyphLocation.Y = checkBoxBounds.Top + (checkBoxBounds.Height - checkBoxSize.Height) / 2;
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case DataGridViewContentAlignment.MiddleRight:
                    glyphLocation.Y = checkBoxBounds.Top + (checkBoxBounds.Height - checkBoxSize.Height) / 2;
                    flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case DataGridViewContentAlignment.TopCenter:
                    glyphLocation.Y = checkBoxBounds.Top;
                    flags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case DataGridViewContentAlignment.TopLeft:
                    glyphLocation.Y = checkBoxBounds.Top;
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case DataGridViewContentAlignment.TopRight:
                    glyphLocation.Y = checkBoxBounds.Top;
                    flags |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
                default:
                    glyphLocation.Y = checkBoxBounds.Top;
                    flags |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
            }

            // Add check boxe to the list
            checkboxes.Add(text, new Rectangle(glyphLocation, checkBoxSize));

            // Calculate bounds for text
            Rectangle textBounds = new Rectangle(
                // Draw text after glyph plus small margin.
                                    checkBoxBounds.Left + 2 * CheckBoxMargin + checkBoxSize.Width,
                                    checkBoxBounds.Top,
                                    checkBoxBounds.Width - (2 * CheckBoxMargin + checkBoxSize.Width),
                                    checkBoxBounds.Height);

            // Darw checkbox only if we should to draw it and have enought space for glyph
            // and text
            if (paint && checkBoxBounds.Height >= checkBoxSize.Height && checkBoxBounds.Height >= cellStyle.Font.Height)
            {
                using (Region clipRegion = graphics.Clip)
                {
                    // Draw checkbox
                    CheckBoxRenderer.DrawCheckBox(graphics,
                                                        glyphLocation,
                                                        textBounds,
                                                        text,
                                                        cellStyle.Font,
                                                        flags,
                                                        false,
                                                        cbState);
                    graphics.Clip = clipRegion;
                }
            }

            // Calculate and return right coordinate for drawed checkbox with margin
            return checkBoxSize.Width + TextRenderer.MeasureText(text, cellStyle.Font).Width + CheckBoxMargin;
        }


        /// <summary>
        /// Custom paint cell. Fills flags list and draws checkbox for each flag.
        /// </summary>
        /// <param name="graphics">The Graphics used to paint the DataGridViewCell.</param>
        /// <param name="clipBounds">A Rectangle that represents the area of the DataGridView that needs to be repainted.</param>
        /// <param name="cellBounds">A Rectangle that contains the bounds of the DataGridViewCell that is being painted.</param>
        /// <param name="rowIndex">The row index of the cell that is being painted.</param>
        /// <param name="cellState">A bitwise combination of DataGridViewElementStates values that specifies the state of the cell.</param>
        /// <param name="value">The data of the DataGridViewCell that is being painted.</param>
        /// <param name="formattedValue">The formatted data of the DataGridViewCell that is being painted.</param>
        /// <param name="errorText">An error message that is associated with the cell.</param>
        /// <param name="cellStyle">A DataGridViewCellStyle that contains formatting and style information about the cell.</param>
        /// <param name="advancedBorderStyle">
        /// A DataGridViewAdvancedBorderStyle that contains border styles for the cell that is being painted.
        /// </param>
        /// <param name="paintParts">
        /// A bitwise combination of the DataGridViewPaintParts values that specifies which parts of the cell need to be painted.
        /// </param>
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
                                      object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            // Should be connecte to the DataGridView
            if (DataGridView == null)
            {
                Debug.Fail("Not connected to the DataGridView!");
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, 
                    value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                return;
            }

            // Validate inputs
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (cellStyle == null)
                throw new ArgumentNullException("cellStyle");
            if (advancedBorderStyle == null)
                throw new ArgumentNullException("advancedBorderStyle");
            if (rowIndex < 0 || rowIndex >= DataGridView.RowCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            // Recompute layout and paint cell
            ComputeLayout(graphics, 
              cellStyle, 
              rowIndex, 
              clipBounds, 
              cellBounds, 
              cellState, 
              advancedBorderStyle, 
              paintParts, 
              true  /*paint*/);
        }
        #endregion

        #region Other overridings
        /// <summary>
        /// Custom implementation of the GetPreferredSize method calculates size needed for the flags.
        /// </summary>
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            // Must be connected to the DataGridView
            if (this.DataGridView == null || this.OwningColumn == null)
            {
                Debug.Fail("Not connected to the DataGridView!");
                return new Size(-1,-1);
            }

            // Validate inputs
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (cellStyle == null)
                throw new ArgumentNullException("cellStyle");
            if (rowIndex < 0 || rowIndex >= DataGridView.RowCount)
                throw new ArgumentOutOfRangeException("rowIndex");

            // Finally compute the layout of the cell and return the resulting content bounds.
            Size content = ComputeLayout(graphics, cellStyle, rowIndex);

            return content;
        }

        /// <summary>
        /// Custom implementation of the GetContentBounds method which delegates most of the work to the ComputeLayout function.
        /// </summary>
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            // Must be connected to the DataGridView
            if (this.DataGridView == null || this.OwningColumn == null)
            {
                Debug.Fail("Not connected to the DataGridView!");
                return Rectangle.Empty;
            }

            // Validate inputs
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (cellStyle == null)
                throw new ArgumentNullException("cellStyle");
            if (rowIndex < 0 || rowIndex >= DataGridView.RowCount)
                throw new ArgumentOutOfRangeException("rowIndex");


            // Return content rectangle
            return new Rectangle(
                new Point(0, 0), 
                GetPreferredSize(graphics, cellStyle, rowIndex, new Size(0, 0)));
        }

        /// <summary>
        /// Handles click on the cell content an switch checkboxes.
        /// </summary>
        /// <param name="e">Information on the event.</param>
        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            if (this.DataGridView == null || e == null || e.RowIndex < 0
                || e.RowIndex >= this.DataGridView.RowCount)
                return;

            if (String.IsNullOrEmpty(hotItem) || !checkboxes.ContainsKey(hotItem))
                return;

            // Switch flag
            flags[hotItem] = !flags[hotItem];

            // If column row not empty, change it value
            DataRow column = GetColumnRow(e.RowIndex);
            if (column != null)
                DataInterpreter.SetValueIfChanged(
                    column,
                    hotItem,
                    flags[hotItem] ? DataInterpreter.True : DataInterpreter.False);

            // Notify grid about changes
            NotifyDataGridViewOfValueChange();
        }

        /// <summary>
        /// Handles mouse movements and makes checkboxes hot.
        /// </summary>
        /// <param name="e">Makes checkboses hot.</param>
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (this.DataGridView == null || e == null || e.RowIndex < 0
                || e.RowIndex >= this.DataGridView.RowCount)
                return;

            // Get hot flag name
            string hotFlag = GetHotFlagName(e.Location, e.RowIndex);
            
            // Skip if nothing changed
            if (DataInterpreter.CompareInvariant(hotItem, hotFlag))
            {
                base.OnMouseMove(e);
                return;
            }          

            // Store old hot item
            string oldHotItem = hotItem;

            // Store new hot item
            hotItem = hotFlag;

            // Invalidate old hot item, if any
            InvalidateItem(e.ColumnIndex, e.RowIndex, oldHotItem);

            // Invalidate new hot item, if any
            InvalidateItem(e.ColumnIndex, e.RowIndex, hotItem);
        }

        /// <summary>
        /// Handles mouse leave and removes hot item.
        /// </summary>
        /// <param name="rowIndex">Index of the row which owns this column</param>
        protected override void OnMouseLeave(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0 || rowIndex >= this.DataGridView.RowCount
                || this.OwningColumn == null)
            {
                base.OnMouseLeave(rowIndex);
                return;
            }

            // Store old hot item
            string oldHotItem = hotItem;

            // Reset hot item
            hotItem = String.Empty;

            // Invalidate old hot item, if any (doesn't work always for some reason)
            //InvalidateItem(this.OwningColumn.Index, rowIndex, oldHotItem);

            // Invalidate whole cell (this works always, but cause a little blinking)
            if (!String.IsNullOrEmpty(oldHotItem))
                this.DataGridView.InvalidateCell(this);

            base.OnMouseLeave(rowIndex);
        }
        #endregion

        #region IDataGridViewEditingCell Members
        /// <summary>
        /// Keeps track of whether the cell's value has changed or not.
        /// </summary>
        public virtual bool EditingCellValueChanged
        {
            get
            {
                return this.valueChanged;
            }
            set
            {
                this.valueChanged = value;
            }
        }

        /// <summary>
        /// Gets or sets formating value for the cell.
        /// </summary>
        public object EditingCellFormattedValue
        {
            get
            {
                StringBuilder result = new StringBuilder();

                // Iterate through availabel flags. For each checked item append it to result
                foreach (KeyValuePair<string, bool> flag in flags)
                {
                    // If item is not checked, skip it.
                    if (!flag.Value)
                        continue;

                    // If this flag is not the only one, append space
                    if (result.Length > 0)
                        result.Append(' ');

                    // Append flag name
                    result.Append(flag.Key);
                }

                return result.ToString();
            }
            set
            {
                // Just do nothing - we've read all from the row.
            }
        }

        /// <summary>
        /// Returns the current formatted value of the cell
        /// </summary>
        public object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingCellFormattedValue;
        }

        /// <summary>
        /// Prepares cell for the editing. Does nothing
        /// </summary>
        public void PrepareEditingCellForEdit(bool selectAll)
        {
            return;
        }
        #endregion

        #region Private utility methods
        /// <summary>
        /// Utility function that returns the cell state inherited from the owning row and column.
        /// </summary>
        private DataGridViewElementStates CellStateFromColumnRowStates(DataGridViewElementStates rowState)
        {
            Debug.Assert(this.DataGridView != null);
            Debug.Assert(this.ColumnIndex >= 0);
            DataGridViewElementStates orFlags = DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected;
            DataGridViewElementStates andFlags = DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Visible;
            DataGridViewElementStates cellState = (this.OwningColumn.State & orFlags);
            cellState |= (rowState & orFlags);
            cellState |= ((this.OwningColumn.State & andFlags) & (rowState & andFlags));
            return cellState;
        }


        /// <summary>
        /// Little utility function called by the Paint function to see if a particular part needs to be painted. 
        /// </summary>
        /// <param name="paintParts">Flags with parts to paint.</param>
        /// <param name="paintPart">Part to check for painting.</param>
        /// <returns>Returns true if the part should be painted.</returns>
        private static bool NeedPaintPart(DataGridViewPaintParts paintParts, DataGridViewPaintParts paintPart)
        {
            return (paintParts & paintPart) != 0;
        }

        /// <summary>
        /// Returns mouse position in the grid control
        /// </summary>
        private Point MousePosition
        {
            get
            {
                return this.DataGridView != null
                        ? this.DataGridView.PointToClient(Control.MousePosition)
                        : Control.MousePosition;
            }
        }

        /// <summary>
        /// Method that declares the cell dirty and notifies the grid of the value change.
        /// </summary>
        private void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            if(this.DataGridView != null)
                this.DataGridView.NotifyCurrentCellDirty(true);
        }

        /// <summary>
        /// Invalidates single checkbox rectangle.
        /// </summary>
        /// <param name="columnIndex">Index of column which owns this cell.</param>
        /// <param name="rowIndex">Index of row which owns this cell.</param>
        /// <param name="item">Name of item which checkbox should be invalidated.</param>
        private void InvalidateItem(int columnIndex, int rowIndex, string item)
        {
            if (this.DataGridView != null && !String.IsNullOrEmpty(item) && checkboxes.ContainsKey(item))
            {
                // Extract cell bounds
                Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, false /*cutOverflow*/);

                // Extract checkbox bounds
                Rectangle checkbox = new Rectangle(checkboxes[item].Location, checkboxes[item].Size);

                // Adjust checkbox location
                checkbox.Offset(cellBounds.Location);

                // Invalidate checkbox rectangle
                this.DataGridView.Invalidate(checkbox);
            }
        }

        /// <summary>
        /// Returns name of the flag whose checkbox contains given point.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <param name="rowIndex">Index of row which owns this cell.</param>
        /// <returns>Returns name of the flag whose checkbox contains given point.</returns>
        private string GetHotFlagName(Point point, int rowIndex)
        {
            // Recalculate layout
            ComputeLayout(rowIndex);

            // Check each checkbox
            foreach (KeyValuePair<string, Rectangle> checkbox in checkboxes)
                if (checkbox.Value.Contains(point))
                    return checkbox.Key;

            return String.Empty;
        }
        #endregion

        #region Private constants
        /// <summary>
        /// Blank pixels around each check box.
        /// </summary>
        private const byte CheckBoxMargin = 2;
        #endregion

        #region Private variables
        // Note! Do not conver this variables to properties - this causes Visual Studio to fail.

        /// <summary>
        /// Stores whether the cell's value was changed since it became the current cell.
        /// </summary>
        private bool valueChanged;                          
        
        /// <summary>
        /// Dictionary with available flags.
        /// </summary>
        Dictionary<string, bool> flags = new Dictionary<string, bool>();
        
        /// <summary>
        /// Dictionary with checkboxes areas.
        /// </summary>
        Dictionary<string, Rectangle> checkboxes = new Dictionary<string, Rectangle>();

        /// <summary>
        /// Name of current hot item.
        /// </summary>
        string hotItem = String.Empty;
        #endregion
    }
}
