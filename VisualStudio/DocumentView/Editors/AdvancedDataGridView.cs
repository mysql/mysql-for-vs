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
 * This file contains implementation of the advanced grid control.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;
using System.Collections;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Advanced grid control supports some additional feature, like rows reordering.
    /// </summary>
    public class AdvancedDataGridView : DataGridView
    {
        #region Public properties
        /// <summary>
        /// Returns ordinal column to be used for row movements.
        /// </summary>
        public DataGridViewColumn OrdinalColumn
        {
            get
            {
                return ordinalColumnRef;
            }
            set
            {
                ordinalColumnRef = value;
                if (ordinalColumnRef != null)
                    Sort(ordinalColumnRef, ListSortDirection.Ascending);
            }
        }
        #endregion

        #region Event handling
        /// <summary>
        /// Paints the "NULL" image in text cells containing the DBNull value
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            // Filtering header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // Filtering non-text cells
            DataGridViewColumn column = Columns[e.ColumnIndex];
            if (column.CellType != typeof(DataGridViewTextBoxCell))
                return;

            // Filtering unbound cells
            DataGridViewRow row = Rows[e.RowIndex];
            DataRowView boundItem = row.DataBoundItem as DataRowView;
            if (boundItem == null)
                return;

            // Filtering cells having sources with not null values
            DataRow dr = boundItem.Row;
            if (DataInterpreter.IsNotNull(dr, column.DataPropertyName))
                return;

            // A field to paint and a picture
            Rectangle cell = e.CellBounds;
            Bitmap pic = Resources.Null;

            // Painting background
            e.PaintBackground(cell, true);

            // Painting the required index at the "LeftMiddle"
            int x = cell.Left + column.DefaultCellStyle.Padding.Left + 4;
            int y = cell.Top + (cell.Height - pic.Height) / 2;
            Rectangle picBound = new Rectangle(x, y, pic.Width, pic.Height);
            e.Graphics.DrawImageUnscaledAndClipped(pic, picBound);
            e.Handled = true;
        }

        /// <summary>
        /// Initializes ordinal value for the new row.
        /// </summary>
        /// <param name="e">Information about row to process.</param>
        protected override void OnDefaultValuesNeeded(DataGridViewRowEventArgs e)
        {
            if (e != null && e.Row != null && OrdinalColumn != null)
            {                
                object defaultValue = GetNextValue();
                // TODO: Very strange behavior - this doesn't work for first row if
                // all rows were deleted.
                e.Row.Cells[OrdinalColumn.Index].Value = defaultValue;
            }
            base.OnDefaultValuesNeeded(e);
        }

        /// <summary>
        /// Handles key events and moves rows on Ctrl+UP and Ctrl+DOWN keys.
        /// </summary>
        /// <param name="e">Information about keys pressed.</param>
        /// <returns>Returns true if key was handled.</returns>
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            // If one and only one cell is selected, handles Ctrl+0 to reset value to null
            if (e.Control && e.KeyCode == Keys.D0 && SelectedCells != null && SelectedCells.Count == 1)
            {
                // Extract selected cell
                DataGridViewCell cell = SelectedCells[0];
                // Validate extracted cell (only cells with columns are supported)
                if (cell != null && cell.ColumnIndex >= 0 && cell.ColumnIndex < Columns.Count)
                {
                    // Extract cell column
                    DataGridViewColumn column = Columns[cell.ColumnIndex];
                    // Validate column (must be text box with NullValue set to null)
                    if (column is DataGridViewTextBoxColumn && column.DefaultCellStyle != null
                        && column.DefaultCellStyle.NullValue == null)
                    {
                        // Set default value to null
                        cell.Value = column.DefaultCellStyle.NullValue != null ?
                            column.DefaultCellStyle.NullValue : DBNull.Value;
                        return true;
                    }
                }
            }

            // If event args are empty, ordinal column not set or no row selected, return base
            if (e == null || OrdinalColumn == null || SelectedRows == null || SelectedRows.Count <= 0)
                return base.ProcessDataGridViewKey(e);

            // Move selected rows up
            if (e.Control && e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                MoveRowsUp(SortedRows());
                return true;
            }

            // Move selected rows down
            if (e.Control && e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                MoveRowsDown(SortedRows());
                return true;
            }

            return base.ProcessDataGridViewKey(e);
        }

        /// <summary>
        /// Handles databinding and detaches blob columns to avoid validation errors.
        /// </summary>
        /// <param name="e">Information about event.</param>
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            // Only total rebinding (Reset) should be handled
            if (e != null && e.ListChangedType == ListChangedType.Reset && AutoGenerateColumns)
            {
                // List of columns to remove
                ArrayList toDelete = new ArrayList();

                // Remove unbinded columns first and correct null values
                foreach (DataGridViewColumn column in Columns)
                {
                    // Text box columns should have default NullValue as null
                    if (column is DataGridViewTextBoxColumn && column.ValueType == typeof(string))
                        column.DefaultCellStyle.NullValue = null;

                    // Add already detached image columns to delete list
                    if (column is DataGridViewImageColumn && !column.IsDataBound)
                        toDelete.Add(column);
                }

                // Delete columns from delete list to avoid duplicate columns
                foreach (DataGridViewColumn column in toDelete)
                    Columns.Remove(column);

                // Data source casted to datatable
                DataTable dataSource = DataSource as DataTable;

                // Detach image columns to avoid validation errors and correct their visible indexes
                foreach (DataGridViewColumn column in Columns)
                {
                    // Skip not image columns
                    if (!(column is DataGridViewImageColumn))
                        continue;

                    // Change display index of the column to corresponding
                    if (dataSource != null && dataSource.Columns.Contains(column.DataPropertyName))
                        column.DisplayIndex = dataSource.Columns[column.DataPropertyName].Ordinal;

                    // Detach image columns (created by default for blobs)                        
                    column.DataPropertyName = String.Empty;
                }
            }
            
            base.OnDataBindingComplete(e);
        }
        #endregion

        #region Row movement
        /// <summary>
        /// Move all rows from array one step up.
        /// </summary>
        /// <param name="rows">Array with rows to move.</param>
        private void MoveRowsUp(DataRow[] rows)
        {
            // Stores processed rows to prevent swapping at the end
            List<DataRow> processed = new List<DataRow>(rows.Length);

            // Iterates through grid rows
            for (int i = 0; i < rows.Length; i++)
            {
                // Get nearest predecessor and swap with him
                DataRow predecessor = GetPredecessor(rows[i]);
                if (predecessor != null && !processed.Contains(predecessor))
                    Swap(predecessor, rows[i]);
                // Remeber row to exclude it fro swaping further
                processed.Add(rows[i]);
            }

            // Sort table and restore selection
            SortAndRestoreSelection(rows);
        }

        /// <summary>
        /// Move all rows from array one step down.
        /// </summary>
        /// <param name="rows">Array with rows to move.</param>
        private void MoveRowsDown(DataRow[] rows)
        {
            // Stores processed rows to prevent swapping at the end
            List<DataRow> processed = new List<DataRow>(rows.Length);

            // Iterates through grid rows
            for (int i = rows.Length - 1; i >= 0; i--)
            {
                // Get nearest successor and swap with him
                DataRow successor = GetSuccessor(rows[i]);
                if (successor != null && !processed.Contains(successor))
                    Swap(rows[i], successor);
                // Remeber row to exclude it fro swaping further
                processed.Add(rows[i]);
            }

            // Sort table and restore selection
            SortAndRestoreSelection(rows);
        }

        /// <summary>
        /// Sorts grid and restores selection after sort.
        /// </summary>
        /// <param name="datas">List with DataRow objects to restore selection.</param>
        private void SortAndRestoreSelection(DataRow[] datas)
        {
            // Sort table
            Sort(OrdinalColumn, ListSortDirection.Ascending);

            // Select each row which is mentioned in datas
            this.SelectAll();
            foreach (DataGridViewRow row in Rows)
            {
                bool found = false;
                foreach (object data in datas)
                {
                    if (ExtractDataRow(row) == data)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    row.Selected = false;
            }

            // Repaint grid
            Refresh();
        }
        #endregion

        #region Row and values analyze
        /// <summary>
        /// Swaps values of the ordinal column cell in two rows.
        /// </summary>
        /// <param name="pRow">First row.</param>
        /// <param name="sRow">Second row.</param>
        private void Swap(DataRow pRow, DataRow sRow)
        {
            object temp = sRow[OrdinalColumn.DataPropertyName];

            sRow[OrdinalColumn.DataPropertyName] = pRow[OrdinalColumn.DataPropertyName];
            pRow[OrdinalColumn.DataPropertyName] = temp;
        }

        /// <summary>
        /// Extracts DataRow from the DataGridViewRow.
        /// </summary>
        /// <param name="gridRow">DataGridViewRow to process.</param>
        /// <returns>Returns DataRow from the DataGridViewRow.</returns>
        private static DataRow ExtractDataRow(DataGridViewRow gridRow)
        {
            if (gridRow == null)
                return null;
            return gridRow.DataBoundItem is DataRowView ? (gridRow.DataBoundItem as DataRowView).Row : null;
        }

        /// <summary>
        /// Returns preceding row for the given row in terms of ordinal column.
        /// </summary>
        /// <param name="row">Row to search for predecessor</param>
        /// <returns>Returns preceding row for the given row in terms of ordinal column.</returns>
        private DataRow GetPredecessor(DataRow row)
        {
            DataRow result = null;
            foreach (DataGridViewRow candidate in Rows)
            {
                if (candidate == null)
                    continue;
                DataRow dataRow = ExtractDataRow(candidate);
                if (dataRow != null && IsLess(dataRow, row) && (result == null || IsLess(result, dataRow)))
                    result = dataRow;
            }
            return result;
        }

        /// <summary>
        /// Returns succeedent row for the given row in terms of ordinal column.
        /// </summary>
        /// <param name="row">Row to search for successor</param>
        /// <returns>Returns succeedent row for the given row in terms of ordinal column.</returns>
        private DataRow GetSuccessor(DataRow row)
        {
            DataRow result = null;
            foreach (DataGridViewRow candidate in Rows)
            {
                if (candidate == null)
                    continue;
                DataRow dataRow = ExtractDataRow(candidate);
                if (dataRow != null && IsLess(row, dataRow) && (result == null || IsLess(dataRow, result)))
                    result = dataRow;
            }
            return result;
        }

        /// <summary>
        /// Returns ordinal value for the new item to be inserted at the end of the table.
        /// </summary>
        /// <returns>
        /// Returns ordinal value for the new item to be inserted at the end of the table.
        /// </returns>
        private object GetNextValue()
        {
            if (OrdinalColumn.ValueType != typeof(Int64))
            {
                Debug.Fail("Unsupported ordinal column type!");
                return Rows.Count;
            }

            // TODO: This will works only with numeric ordinal columns
            if (Rows.Count == 0)
                return 0;

            DataRow result = ExtractDataRow(Rows[0]);
            if (result == null)
                return Rows.Count;

            // TODO: This is a hack to avoid DataGridViewError (default values are not applied
            // for the first row if all rows were deleted)
            if (!DataInterpreter.IsNotNull(result, OrdinalColumn.DataPropertyName))
            {
                Rows[0].Cells[OrdinalColumn.Index].Value = 1;
                return Rows.Count;
            }

            foreach (DataGridViewRow candidate in Rows)
            {
                if (candidate == null)
                    continue;
                DataRow dataRow = ExtractDataRow(candidate);
                if (dataRow != null && DataInterpreter.IsNotNull(dataRow, OrdinalColumn.DataPropertyName)
                    && IsLess(result, dataRow))
                    result = dataRow;
            }

            return DataInterpreter.GetInt(result, OrdinalColumn.DataPropertyName).Value + 1;
        }

        /// <summary>
        /// Returns array with selected grid rows which are already sorted by ordinal column value.
        /// </summary>
        /// <returns>
        /// /// Returns array with selected grid rows which are already sorted by ordinal column value.
        /// </returns>
        private DataRow[] SortedRows()
        {
            // Filter unbounded rows and extract data rows
            List<DataRow> list = new List<DataRow>(SelectedRows.Count);
            foreach (DataGridViewRow row in SelectedRows)
            {
                if (row == null)
                    continue;
                DataRow dataRow = ExtractDataRow(row);
                if (dataRow != null)
                    list.Add(dataRow);
            }

            // Prepare unsorted result
            DataRow[] result = new DataRow[list.Count];
            list.CopyTo(result, 0);

            // Sort rows using simple bubble sort
            bool hasSwapped = false;
            do
            {
                hasSwapped = false;
                for (int i = 0; i < result.Length - 1; i++)
                {
                    if (IsLess(result[i + 1], result[i]))
                    {
                        DataRow temp = result[i];
                        result[i] = result[i + 1];
                        result[i + 1] = temp;
                        hasSwapped = true;
                    }
                }
            }
            while (hasSwapped);
            return result;
        }

        /// <summary>
        /// Returns true if the first row has less value in the ordinal column and should preced second 
        /// row in the table.
        /// </summary>
        /// <param name="row1">First row to check.</param>
        /// <param name="row2">Second row to check.</param>
        /// <returns>
        /// Returns true if the first row has less value in the ordinal column and should preced second 
        /// row in the table.
        /// </returns>
        private bool IsLess(DataRow row1, DataRow row2)
        {
            IComparable ordinal1 = row1[OrdinalColumn.DataPropertyName] as IComparable;
            IComparable ordinal2 = row2[OrdinalColumn.DataPropertyName] as IComparable;

            if (ordinal1 != null && ordinal2 != null)
                return ordinal1.CompareTo(ordinal2) < 0;

            Debug.Fail("Ordinal values doesn't support IComparable interface");
            return false;
        }
        #endregion

        #region Private variables to store properties
        private DataGridViewColumn ordinalColumnRef;
        #endregion

        #region Delete all rows bugfix
        /* 
         * For some reason default values does not work for first new row after all rows are 
         * deleted. We have to switch new row ability to avoid this bug.
         */

        /// <summary>
        /// Handles deletion of rows. If after deletion grid contains only one row, switch 
        /// new row ability.
        /// </summary>
        /// <param name="e">Detailed information about event.</param>
        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            // Check if only one new row left
            if (Rows.Count == 1 && Rows[0].IsNewRow)
            {
                // Switch new row ability off
                AllowUserToAddRows = false;
                
                // Initialize timer to switch new row ability on
                switchTimer.Interval = 100;
                switchTimer.Tick += new EventHandler(OnSwitchTimerTick);
                switchTimer.Enabled = true;
            }

            base.OnRowsRemoved(e);
        }

        /// <summary>
        /// Timer is used to switch new row ability on, after it was disabled.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnSwitchTimerTick(object sender, EventArgs e)
        {
            // Disable timer (should work only once)
            switchTimer.Enabled = false;
            switchTimer.Tick -= new EventHandler(OnSwitchTimerTick);
            
            // Switch new row ability on
            AllowUserToAddRows = true;
        }

        /// <summary>
        /// Timer used for switching
        /// </summary>
        Timer switchTimer = new Timer();
        #endregion
    }
}