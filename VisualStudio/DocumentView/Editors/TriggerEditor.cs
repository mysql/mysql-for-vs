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
 * This file contains implementation of the custom editor for triggers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;
using MySql.Data.VisualStudio.Descriptors;

namespace MySql.Data.VisualStudio.DocumentView.Editors
{
    /// <summary>
    /// This editor is created to customize caption for triggers.
    /// </summary>
    [ViewObject(TriggerDescriptor.TypeName, typeof(TriggerEditor))]
    class TriggerEditor : SqlSourceEditor
    {
        /// <summary>
        /// Constructs and initializes the editor
        /// </summary>        
        /// <param name="document">Reference to a document to edit</param>
        public TriggerEditor(IDocument document)
            : base(document)
        {
            if (!(document is TriggerDocument))
                throw new ArgumentException(Resources.Error_UnsupportedDocument, "document");
        }

        /// <summary>
        /// Returns typed document instance.
        /// </summary>
        protected new TriggerDocument Document
        {
            get
            {
                Debug.Assert(base.Document is TriggerDocument, "Wrong document type is used!");
                return base.Document as TriggerDocument;
            }
        }

        /// <summary>
        /// Returns customized caption.
        /// </summary>
        protected override string EditorCaption
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture, Resources.EditorCaption_Template,
                    Document.Table + '.' + Document.Name);
            }
        }
    }
}
