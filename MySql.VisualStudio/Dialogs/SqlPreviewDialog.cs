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
 * This file contains implementation of the SQL execution confirmation dialog.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Dialogs
{
    /// <summary>
    /// This dialog is used to display confirmation before SQL query execution.
    /// </summary>
    public partial class SqlPreviewDialog : Form
    {
        /// <summary>
        /// Default constructor for designer
        /// </summary>
        public SqlPreviewDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor to use with given SQL query
        /// </summary>
        /// <param name="sql">SQL query to display.</param>
        public SqlPreviewDialog(string sql)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");

            InitializeComponent();

            imageList.Images.Add(SystemIcons.Question);
            iconLabel.ImageIndex = 0;

            sqlText.Text = sql.Normalize();
        }

        /// <summary>
        /// Displays dialog with cinfiration.
        /// </summary>
        /// <param name="sql">SQL text to display.</param>
        /// <returns>
        /// Returns OK if user select tot execute query and Cancel otherwize.
        /// </returns>
        public static DialogResult Show(string sql)
        {
            if (sql == null)
                throw new ArgumentNullException("sql");

            return UIHelper.ShowDialog(new SqlPreviewDialog(sql));
        }
    }
}