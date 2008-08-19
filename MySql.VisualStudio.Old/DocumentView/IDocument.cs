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
 * This class contains definition of the common document object interface.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Common interface for all document classes. At least each 
    /// document must support IVsPersistDocData to allow interaction 
    /// with RDT. Additionally, document must expose several 
    /// information properties and notifications. 
    /// </summary>
    public interface IDocument : IVsPersistDocData
    {
        #region Properties
        /// <summary>
        /// Name of schema to which owns this database object. This property is read-only.
        /// </summary>
        string Schema
        {
            get;
        }

        /// <summary>
        /// Name of this database object. This property is read-only.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Name of server host which owns this database. This property is read-only.
        /// </summary>
        string Server
        {
            get;
        }

        /// <summary>
        /// Indicates if this instance represents new database object 
        /// doesn’t fixed in database yet.
        /// </summary>
        bool IsNew
        {
            get;
        }

        /// <summary>
        /// ID of this object as array of strings (null, schema name, object name 
        /// by default).
        /// </summary>
        object[] ObjectID
        {
            get;
        }

        /// <summary>
        /// Name of the document object type, as it declared in the DataObjectSupport 
        /// XML.
        /// </summary>
        string TypeName
        {
            get;
        }

        /// <summary>
        /// Pointer to the IDE facade object.
        /// </summary>
        ServerExplorerFacade Hierarchy
        {
            get;
        }

        /// <summary>
        /// Data connection object used to interact with MySQL server. This property 
        /// is read-only.
        /// </summary>
        DataConnectionWrapper Connection
        {
            get;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Changes name of this object and detach it from underlying database object. This object
        /// will be considered as new after this method is called. This method should be used before
        /// data are loaded.
        /// </summary>
        /// <param name="newName">New name to use for object.</param>
        void CloneToName(string newName); 
        #endregion

        #region Events
        /// <summary>
        /// This event fires after complete load of document data.
        /// </summary>
        event EventHandler DataLoaded;

        /// <summary>
        /// This event fires after any changes in the document data hapens
        /// </summary>
        event EventHandler DataChanged;

        /// <summary>
        /// This event fires before object data are saved.
        /// </summary>
        event CancelEventHandler Saving;

        /// <summary>
        /// This event fires after object data are successfully saved.
        /// </summary>
        event EventHandler SuccessfullySaved;

        /// <summary>
        /// This event fires if attempt to save document fails.
        /// </summary>
        event EventHandler SaveAttemptFailed;
        #endregion
    }
}
