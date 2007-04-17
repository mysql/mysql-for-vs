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

using System;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// An editor for SQL definitions of objects
    /// </summary>
    public partial class SqlEditor : UserControl
    {
        #region Private variable
        /// <summary>A source of an SQL definition</summary>
        private ISqlSource source;
        #endregion

        #region Property
        /// <summary>
        /// A source of an SQL definition
        /// </summary>
        public ISqlSource SqlSource
        {
            get
            {
                return source;
            }

            set
            {
                source = value;

                // Displaying the definition or cleaning a text area
                txtSqlSource.TextChanged -= new EventHandler(txtSqlSource_TextChanged); 
                txtSqlSource.Text = (source != null) ? source.SqlSource : string.Empty;
                txtSqlSource.TextChanged += new EventHandler(txtSqlSource_TextChanged); 
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SqlEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Handler
        /// <summary>
        /// Changes an SQL definition in the corresponding source
        /// </summary>
        private void txtSqlSource_TextChanged(object sender, EventArgs e)
        {
            if (source != null)
                source.SqlSource = txtSqlSource.Text;
        }
        #endregion
    }
}