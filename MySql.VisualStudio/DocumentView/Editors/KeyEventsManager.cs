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

using System.Windows.Forms;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Handles "KeyDown" events for all children elements of the parent control
    /// </summary>
    public class KeyEventsManager
    {
        #region Private variable
        /// <summary>The parent control whose children's key events we handle</summary>
        private Control parent;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="parent">The parent control whose children's key events we are 
        /// to handle</param>
        public KeyEventsManager(Control parentCtrl)
        {
            parent = parentCtrl;
            Subscribe(parent);
        }
        #endregion

        #region Subscription
        /// <summary>
        /// Recursively subscribes on key events starting from a given parent control
        /// </summary>
        /// <param name="ctrl">The given parent control</param>
        private void Subscribe(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                // Checking if this control participates in a tab order
                if (child.TabStop)
                    child.KeyDown += new KeyEventHandler(Ctrl_KeyDown);

                // Recursively handling children of the current control
                Subscribe(child);
            }
        }
        #endregion

        #region Handler
        /// <summary>
        /// Handles the "KeyDown" event
        /// </summary>
        void Ctrl_KeyDown(object sender, KeyEventArgs e)
        {
            // Handling only the "Tab" key
            if (e.KeyCode != Keys.Tab)
                return;

            Control ctrl = sender as Control;
            if (ctrl == null)
                return;

            // Multiline text boxes should accept tabs
            if (ctrl is TextBox && (ctrl as TextBox).Multiline)
                return;

            // Getting the next control
            e.Handled = parent.SelectNextControl(ctrl, !e.Shift, true, true, true);
        }
        #endregion
    }
}