// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Windows.Forms;
using System.Drawing;

namespace MySql.Design
{
	/// <summary>
	/// Summary description for DataGridComboBoxColumn.
	/// </summary>
	internal class DataGridComboBoxColumn : DataGridColumnStyle
	{
		private ComboBox	comboBox; 
		private DataGrid	myGrid;
		private int			border = 0;


		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		public DataGridComboBoxColumn( string headerText, object dataSource, string mappingName ) : base() 
		{
			comboBox = new ComboBox();
			comboBox.DataSource = dataSource;

			this.HeaderText = headerText;
			this.MappingName = mappingName;
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			comboBox.Visible = false;

			isEditing = false;

			try 
			{
				object value = comboBox.SelectedValue;
				SetColumnValueAtRow(dataSource, rowNum, value);
			} 
			catch (Exception) 
			{
				Abort(rowNum);
				return false;
			}

			Invalidate();
			return true;
		}
	
		protected override void SetDataGridInColumn(DataGrid value)
		{
			base.SetDataGridInColumn (value);

			myGrid = value;
			myGrid.Controls.Add( comboBox );
			comboBox.Font = myGrid.Font;
			comboBox.Visible = false;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
		}


		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, 
			bool readOnly, string instantText, bool cellIsVisible) 
		{	
				object value = (string)GetColumnValueAtRow(source, rowNum);
			isEditing = true;
			if (cellIsVisible) 
			{
				comboBox.Bounds = new Rectangle(bounds.X + border, bounds.Y + border, 
					bounds.Width - border, bounds.Height - border);
				comboBox.Visible = true;
				comboBox.Text = (string)value;
			} 

			if (comboBox.Visible)
				DataGridTableStyle.DataGrid.Invalidate(bounds);
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size( this.Width, this.comboBox.PreferredHeight );
		}

		protected override int GetMinimumHeight() 
		{
			return comboBox.PreferredHeight;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return comboBox.PreferredHeight;
		}

		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) 
		{
			Paint(g, bounds, source, rowNum, false);
		}

		protected override void Paint(Graphics g, Rectangle bounds,	CurrencyManager source, int rowNum,	bool alignToRight) 
		{
			Paint(g,bounds, source, rowNum, Brushes.Red, Brushes.Blue, alignToRight);
		}

		protected override void Paint( Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight) 
		{
			string value = (string)GetColumnValueAtRow(source, rowNum);
			Rectangle rect = bounds;
			g.FillRectangle(backBrush,rect);
			rect.Offset(0, 2);
			rect.Height -= 2;
			g.DrawString(value, this.DataGridTableStyle.DataGrid.Font, foreBrush, rect);
		}

	}
}
