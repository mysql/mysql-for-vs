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
 * This file conatins definition of the common interface for all editors.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This interface describes common functionality for each editor class. 
    /// At least each editor should support IVsWindowPane for simplified embedding 
    /// mechanism.
    /// </summary>
    public interface IEditor : IVsWindowPane
    {
        /// <summary>
        /// Reference to the owned windows frame. Used to change caption.
        /// </summary>
        IVsWindowFrame OwnerFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Command group unique identifier for this editor, used 
        /// to register editor in RDT.
        /// </summary>
        Guid CommandGroupID
        {
            get;
        }

        /// <summary>
        /// Reference to the document which is displayed by this view.
        /// </summary>
        IDocument Document
        {
            get;
        }

        

    }
}
