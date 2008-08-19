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
