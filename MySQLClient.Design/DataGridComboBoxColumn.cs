// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
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

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for DataGridComboBoxColumn.
	/// </summary>
	internal class DataGridComboBoxColumn : DataGridColumnStyle
	{
		private TextBox textBox = new TextBox();
		private Button  button = new Button();
		private bool    buttonDown = false;
		private ListBox	valueList = new ListBox();
		private int		border = 2;

		private Panel	panel = new Panel();

		// The isEditing field tracks whether or not the user is
		// editing data with the hosted control.
		private bool isEditing;

		public DataGridComboBoxColumn() : base() 
		{
			textBox.Location = new Point(0,0);
			textBox.BorderStyle = BorderStyle.None;
			textBox.Dock = DockStyle.Fill;

			button.Paint += new PaintEventHandler(button_Paint);
			button.MouseDown += new MouseEventHandler(button_MouseDown);
			button.Dock = DockStyle.Right;

			panel.Controls.Add( textBox );
			panel.Controls.Add( button );
			panel.Visible = false;

			valueList.Visible = false;

			valueList.SelectedIndexChanged += new EventHandler(valueList_SelectedIndexChanged);
		}

		public object ListSource 
		{
			get { return valueList.DataSource; }
			set { valueList.DataSource = value; }
		}

		public string ListDisplayMember
		{
			get { return valueList.DisplayMember; }
			set { valueList.DisplayMember = value; }
		}

		public string ListValueMember 
		{
			get { return valueList.ValueMember; }
			set { valueList.ValueMember = value; }
		}

		protected override void Abort(int rowNum)
		{
			isEditing = false;
			textBox.TextChanged -= new EventHandler(TextBoxValueChanged);
			Invalidate();
		}

		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			textBox.TextChanged -= new EventHandler(TextBoxValueChanged);

			Console.WriteLine("commit called for rowNum " + rowNum);
			panel.Visible = false;
			valueList.Visible = false;

			if (!isEditing)return true;

			isEditing = false;

			try 
			{
				string value = textBox.Text;
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
	
		protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible) 
		{	
			string value = (string)GetColumnValueAtRow(source, rowNum);
			if (cellIsVisible) 
			{
				panel.Bounds = new Rectangle(bounds.X + border, bounds.Y + border, 
					bounds.Width - border, bounds.Height - border);
				valueList.Bounds = new Rectangle( bounds.X, bounds.Bottom, bounds.Width, 150 );
				button.Width = button.Height;
				panel.Visible=true;
				textBox.Text = value;
				textBox.Visible = true;
				textBox.TextChanged += new EventHandler(TextBoxValueChanged);
				textBox.SelectAll();
			} 
			else 
			{
				textBox.Text = value;
				panel.Visible = false;
			}

			if (textBox.Visible)
				DataGridTableStyle.DataGrid.Invalidate(bounds);
		}

		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			return new Size(150, textBox.PreferredHeight + 4);
		}

		protected override int GetMinimumHeight() 
		{
			return textBox.PreferredHeight + 4;
		}

		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			return textBox.PreferredHeight + 4;
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

		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
			if (panel.Parent != null) 
			{
				panel.Parent.Controls.Remove (panel);
			}
			if (value != null) 
			{
				value.Controls.Add( panel );
			}
		}

		private void TextBoxValueChanged(object sender, EventArgs e) 
		{
			this.isEditing = true;
			base.ColumnStartedEditing(panel);
		}

		private void button_Paint(object sender, PaintEventArgs e)
		{
			if (buttonDown)
			{
				ControlPaint.DrawComboButton( e.Graphics, button.ClientRectangle, ButtonState.Pushed );
				buttonDown = false;
			}
			else
				ControlPaint.DrawComboButton( e.Graphics, button.ClientRectangle, ButtonState.Normal );
		}

		private void button_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				buttonDown = true;
				valueList.Show();
			}
			else
				buttonDown = false;
		}

		private void valueList_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox.Text = valueList.SelectedValue.ToString();
		}
	}
}
