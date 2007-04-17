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
 * This file contains implementation of the table and view data editor.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;
using System.Globalization;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is the editor for table and view data. It contains raw data grid only.
    /// </summary>
    [ViewObject(TableDataDescriptor.TypeName, typeof(TableDataEditor))]
    public partial class TableDataEditor : BaseEditor
    {
        #region Initialization
        /// <summary>
        /// Default constructor for table designer
        /// </summary>
        private TableDataEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs and initializes editor.
        /// </summary>        
        /// <param name="document">Reference to document object.</param>
        public TableDataEditor(IDocument document)
            : base(document)
        {
            if (!(document is TableDataDocument))
                throw new ArgumentException(
                    Resources.Error_UnsupportedDocument,
                    "document");

            InitializeComponent();            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns typed document object
        /// </summary>
        protected new TableDataDocument Document
        {
            get
            {
                return base.Document as TableDataDocument;
            }
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Returns customized caption to distinct from table ot view editor.
        /// </summary>
        protected override string EditorCaption
        {
            get
            {
                return String.Format(
                    CultureInfo.CurrentCulture, 
                    Resources.TableData_Editor_Caption, 
                    base.EditorCaption);
            }
        }

        /// <summary>
        /// Atachs columns grid to datasource and initialize header
        /// </summary>
        protected override void RefreshView()
        {
            // Extract collumns
            Debug.Assert(Document.Data != null, "Document is not loaded!");
            if (!Object.ReferenceEquals(dataGridView.DataSource, Document.Data))
            {
                dataGridView.DataSource = Document.Data;

                // Set updatable options
                bool updateable = Document.IsUpdatable;
                dataGridView.ReadOnly = !updateable;
                dataGridView.AllowUserToAddRows = updateable;
                dataGridView.AllowUserToDeleteRows = updateable;
            }
        }

        /// <summary>
        /// Commits changes for columns grid and calls base method.
        /// </summary>
        /// <returns>Returns false if column name validating fails.</returns>
        protected override bool ValidateAndFlushChanges()
        {
            if (!base.ValidateAndFlushChanges())
                return false;

            // Move focus away from grid to force it to save changes
            if (dataGridView.ContainsFocus || dataGridView.EditingControl != null)
                panelToChangeFocus.Focus();

            // End edit of the data grid (for some reason doesn't work without line above)
            dataGridView.EndEdit();

            return true;
        }
        #endregion
    }
}
