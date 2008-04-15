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
 * This file contains a custom column type for flags column in the columns editor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is a custom column type for flags column in the columns editor.
    /// </summary>
    class DataGridViewFlagsColumn : DataGridViewColumn
    {
        /// <summary>
        /// Initialize base class using flags cell type.
        /// </summary>
        public DataGridViewFlagsColumn()
            : base(new DataGridViewFlagsCell())
        {
        }

        /// <summary>
        /// Gets or sets cell template. Controls template type.
        /// </summary>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a DataGridViewFlagsCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(DataGridViewFlagsCell)))
                {
                    Debug.Fail("Must be a DataGridViewFlagsCell");
                    return;
                }
                base.CellTemplate = value;
            }
        }
    }
}
