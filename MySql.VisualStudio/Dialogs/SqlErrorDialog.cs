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
 * This file contains implementation of the SQL error dialog.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Utils;
using System.Data.Common;
using System.Diagnostics;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;

namespace MySql.Data.VisualStudio.Dialogs
{
    /// <summary>
    /// This dialog is used to show information about MySQL error.
    /// </summary>
    public partial class SqlErrorDialog : Form
    {
        /// <summary>
        /// Default constructior for designer.
        /// </summary>
        public SqlErrorDialog()
        {
            InitializeComponent();

            // Load error icon
            imageList.Images.Add(SystemIcons.Error);
            iconLabel.ImageIndex = 0;
        }

        /// <summary>
        /// Constructor to use. Takes MySqlException to display.
        /// </summary>
        /// <param name="e">MySqlException to show.</param>
        /// <param name="status">Table engine statuses to display in advanced information.</param>
        public SqlErrorDialog(DbException e, string status)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            if (status == null)
                throw new ArgumentNullException("status");

            InitializeComponent();

            // Load error icon
            imageList.Images.Add(SystemIcons.Error);
            iconLabel.ImageIndex = 0;

            // Fill text boxes
            errorText.Text = e.Message;
            statusText.Text = status;

            // We try to recognize, if this exception is MySqlException
            // For this we'll use reflection. We'll try to find property "Number"
            Type exceptionType = e.GetType();
            Debug.Assert(exceptionType != null, "Failed to retrieve type information for exception!");

            // Extract property information
            PropertyInfo numberProperty = exceptionType.GetProperty("Number");

            // If there is Number property, extract its value
            object numberValueObject = null;
            if (numberProperty != null)
            {
                try
                {
                    // Try to get value of Number property
                    numberValueObject = numberProperty.GetValue(e, null);
                }
                catch (Exception ex)
                {
                    // This is bad, but not fatal - we can show unknown error code
                    Debug.Fail("Failed to get value for Number property!", ex.ToString());
                }
            }
            else
            {
                // This is bad, but not fatal - we can show unknown error code
                Debug.Fail("Failed to extract Number property description!");
            }

            // If extracted value is not null, convert it to string and append to error label
            if (numberValueObject != null)
                errorLabel.Text = String.Format(CultureInfo.CurrentCulture, errorLabel.Text, numberValueObject.ToString());
            // If value is null, append unknow error code
            else
                errorLabel.Text = String.Format(CultureInfo.CurrentCulture, errorLabel.Text, Resources.UnknownErrorCode);
        }

        /// <summary>
        /// Shows error dialog.
        /// </summary>
        /// <param name="e">MySqlException to show.</param>
        /// <param name="status">Table engine statuses to display in advanced information.</param>
        public static void ShowError(DbException e, string status)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            if (status == null)
                throw new ArgumentNullException("status");

            SqlErrorDialog dialog = new SqlErrorDialog(e, status);
            UIHelper.ShowDialog(dialog);
        }
    }
}