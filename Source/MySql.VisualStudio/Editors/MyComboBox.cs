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

using System.Windows.Forms;
using System.Drawing;

namespace MySql.Data.VisualStudio.Editors
{
  class MyComboBox : ComboBox
  {
    public MyComboBox()
      : base()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
      DrawItem += new DrawItemEventHandler(DrawListItem);
    }

    private void DrawListItem(object sender, DrawItemEventArgs e)
    {
      DrawComboBox(this, e);
    }

    public override Size MinimumSize
    {
      get
      {
        Size sz = base.MinimumSize;
        sz.Height += 10;
        return sz;
      }
      set
      {
        base.MinimumSize = value;
      }
    }

    public static void DrawComboBox(ComboBox cb, DrawItemEventArgs e)
    {
      if (e.Index == -1) return;

      object comboBoxItem = cb.Items[e.Index];

      e.DrawBackground();
      e.DrawFocusRectangle();

      //            bool isSeparatorItem = (comboBoxItem is SeparatorItem);

      // draw the text
      using (Brush textBrush = new SolidBrush(e.ForeColor))
      {
        Rectangle bounds = e.Bounds;
        // adjust the bounds so that the text is centered properly.

        // if we're a separator, remove the separator height
        //              if (isSeparatorItem && (e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit)
        //            {
        //              bounds.Height -= separatorHeight;
        //        }

        // Draw the string vertically centered but on the left
        using (StringFormat format = new StringFormat())
        {
          format.LineAlignment = StringAlignment.Center;
          format.Alignment = StringAlignment.Near;
          // in Whidbey consider using TextRenderer.DrawText instead
          e.Graphics.DrawString(comboBoxItem.ToString(), cb.Font, textBrush,
              bounds, format);
        }
      }

      // draw the separator line
      /*            if (isSeparatorItem && ((e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit))
                  {
                      Rectangle separatorRect = new Rectangle(e.Bounds.Left, e.Bounds.Bottom - separatorHeight, e.Bounds.Width, separatorHeight);

                      // fill the background behind the separator
                      using (Brush b = new SolidBrush(comboBox1.BackColor))
                      {
                          e.Graphics.FillRectangle(b, separatorRect);
                      }
                      e.Graphics.DrawLine(SystemPens.ControlText, separatorRect.Left + 2, separatorRect.Top + 1,
                          separatorRect.Right - 2, separatorRect.Top + 1);

                  }*/
    }
  }
}
