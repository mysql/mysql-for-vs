using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
    public class MySplitter : Splitter
    {

        public MySplitter()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle r = ClientRectangle;

            SolidBrush brush = new SolidBrush(SystemColors.Control);
            Pen light = new Pen(SystemColors.ControlLightLight);
            Pen dark = new Pen(SystemColors.ControlDarkDark);

            e.Graphics.FillRectangle(brush, r);

            e.Graphics.DrawLine(light, r.Left + 1, r.Top + 1, r.Left + 1, r.Bottom - 2);
            e.Graphics.DrawLine(light, r.Left + 1, r.Top + 1, r.Right - 2, r.Top + 1);

            e.Graphics.DrawLine(dark, r.Right - 1, r.Top, r.Right - 1, r.Bottom - 1);
            e.Graphics.DrawLine(dark, r.Left, r.Bottom - 1, r.Width, r.Bottom - 1);

//            base.OnPaint(e);
        }
    }
}
