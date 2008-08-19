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

using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Utils;
using System.Drawing;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// The editor permits to define objects with their SQL definitions
    /// </summary>
    [ViewObject(ViewDescriptor.TypeName, typeof(SqlSourceEditor))]
    [ViewObject(StoredProcDescriptor.TypeName, typeof(SqlSourceEditor))]
    public partial class SqlSourceEditor : BaseEditor
    {
        #region Property
        /// <summary>
        /// Returns a displayed document as an SQL defined object
        /// </summary>
        protected new ISqlSource Document
        {
            get
            {
                return base.Document as ISqlSource;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor for the VS designer
        /// </summary>
        private SqlSourceEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs and initializes the editor
        /// </summary>        
        /// <param name="document">Reference to a document to edit</param>
        public SqlSourceEditor(IDocument document) : base(document)
        {
            if (!(document is ISqlSource))
                throw new ArgumentException(Resources.Error_UnsupportedDocument, "document");

            InitializeComponent();
            Font f = FontsAndColors.GetFont("Text Editor");
            sqlEditor.Font = f;
        }
        #endregion

        #region Method
        /// <summary>
        /// Refreshs data representaion
        /// </summary>
        protected override void RefreshView()
        {
            // Assigning a definition source to an underlying editor
            sqlEditor.SqlSource = Document;
        }
        #endregion
    }
}