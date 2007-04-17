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
 * This file contains implementation of the Name attribute editor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This class is used to control name attribute changes. It validates name for
    /// non-emptiness and unique. 
    /// </summary>
    class NameTextBox: TextBox
    {
        #region Data properites
        /// <summary>
        /// Gets or sets attribute name to work with.
        /// </summary>
        public string AttributeName
        {
            get
            {
                return attributeNameVal;
            }
            set
            {
                attributeNameVal = value;
                RefreshText();
            }
        }

        /// <summary>
        /// Gets or sets DataRow object to work with.
        /// </summary>
        public DataRow DataSource
        {
            get
            {
                return dataSourceRef;
            }
            set
            {
                dataSourceRef = value;
                RefreshText();
            }
        }

        /// <summary>
        /// Initializes text property with new name form datasource if any.
        /// </summary>
        private void RefreshText()
        {
            if (DataSource == null || String.IsNullOrEmpty(AttributeName))
            {
                Text = String.Empty;
            }
            else
            {
                Text = DataInterpreter.GetStringNotNull(DataSource, AttributeName);
            }
        } 
        #endregion

        #region Validation
        /// <summary>
        /// Validates column name and copies it to the DataRow.
        /// </summary>
        /// <returns>Returns true if validation succeeded and false otherwize.</returns>
        public bool ValidateAndCopyName()
        {
            // Validate row
            if (DataSource == null || String.IsNullOrEmpty(AttributeName) || DataSource.RowState == DataRowState.Deleted)
                return true;

            // Name should be not empty
            if (String.IsNullOrEmpty(Text))
            {
                UIHelper.ShowWarning(Resources.Warning_EmptyName);
                Focus();
                return false;
            }

            // Name should be different from other names
            DataRow[] others = DataInterpreter.Select(DataSource.Table, AttributeName, Text);
            if (others != null && others.Length >= 1 && others[0] != DataSource)
            {
                UIHelper.ShowWarning(Resources.Warning_DuplicateName);
                Focus();
                return false;
            }

            // If all is ok, set new name
            if (NameChanging != null)
                NameChanging(this, EventArgs.Empty);
            DataInterpreter.SetValueIfChanged(DataSource, AttributeName, Text);
            if (NameChanged != null)
                NameChanged(this, EventArgs.Empty);
            return true;
        } 
        #endregion

        #region Event handlers
        /// <summary>
        /// Calls validate and copy name if name text box is leaved.
        /// </summary>
        /// <param name="sender">Event sender. Must be buttonNull.</param>
        /// <param name="e">Information about event.</param>
        protected override void OnLeave(EventArgs e)
        {
            if (ValidateAndCopyName())
                base.OnLeave(e);
        }

        /// <summary>
        /// Calls validate and copy name if Enter is pressed.
        /// </summary>
        /// <param name="sender">Event sender. Must be buttonNull.</param>
        /// <param name="e">Information about event.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e != null && e.KeyChar == '\r' && !ValidateAndCopyName())
                return;
            base.OnKeyPress(e);
        } 
        #endregion

        #region Event
        /// <summary>
        /// Fired before name changes applyed to the datasource.
        /// </summary>
        public event EventHandler NameChanging;
        /// <summary>
        /// Fired after name changes applyed to the datasource.
        /// </summary>
        public event EventHandler NameChanged;
        #endregion

        #region Private variables to store properties
        private string attributeNameVal;
        private DataRow dataSourceRef;
        #endregion
    }
}
