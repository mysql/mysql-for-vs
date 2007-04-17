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
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// The editor for user defined functions
    /// </summary>
    [ViewObject(UdfDescriptor.TypeName, typeof(UdfEditor))]
    public partial class UdfEditor : BaseEditor
    {
        #region A private variable
        /// <summary>A manager of key events</summary>
        private KeyEventsManager keyEventsManager;
        #endregion

        #region Property
        /// <summary>
        /// Returns a displayed document as a UDF describing document
        /// </summary>
        protected new UdfDocument Document
        {
            get
            {
                return base.Document as UdfDocument;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor for the VS designer
        /// </summary>
        private UdfEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs and initializes the editor
        /// </summary>        
        /// <param name="document">Reference to a document to edit</param>
        public UdfEditor(IDocument document) : base(document)
        {
            if (!(document is UdfDocument))
                throw new ArgumentException(Resources.Error_UnsupportedDocument, "document");

            InitializeComponent();

            // Adding permittable return types
            UnsubscribeEvents();
            cboReturns.DataSource = Enum.GetNames(typeof(ReturnTypes));
            SubscribeEvents();

            // Creating a key events manager
            keyEventsManager = new KeyEventsManager(this);
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Excludes schema name from the caption.
        /// </summary>
        protected override string OwnerCaption
        {
            get
            {
                return Document.Server;
            }
        }
        /// <summary>
        /// Refreshs data representaion
        /// </summary>
        protected override void RefreshView()
        {
            UnsubscribeEvents();

            // Assigning document attributes to elements of the editor
            txtName.Text = Document.Name;
            txtDll.Text = Document.Dll;
            cboReturns.SelectedItem = Document.Returns.ToString();
            chkType.Checked = Document.IsAggregate;

            SubscribeEvents();
        }

        /// <summary>
        /// Saves changes which have not yet been saved
        /// </summary>
        /// <returns>False if changes can't be flushed; true otherwise</returns>
        protected override bool ValidateAndFlushChanges()
        {
            // Validating values
            if (string.IsNullOrEmpty(txtName.Text))
            {
                UIHelper.ShowError(Resources.Error_EmptyUdfName);
                return false;
            }

            if (string.IsNullOrEmpty(txtDll.Text))
            {
                UIHelper.ShowError(Resources.Error_EmptyDll);
                return false;
            }

            // Saving values from textboxes, for the "Leave" event may haven't occured
            Document.Name = txtName.Text;
            Document.Dll = txtDll.Text;

            return base.ValidateAndFlushChanges();
        }
        #endregion

        #region Handlers
        /// <summary>
        /// A function name was changed
        /// </summary>
        private void txtName_Leave(object sender, EventArgs e)
        {
            Document.Name = txtName.Text;
            RefreshProperties();
        }

        /// <summary>
        /// A file name was changed
        /// </summary>
        private void txtDll_Leave(object sender, EventArgs e)
        {
            Document.Dll = txtDll.Text;
            RefreshProperties();
        }

        /// <summary>
        /// A function return type was changed
        /// </summary>
        private void cboReturns_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStr = cboReturns.SelectedItem as string;
            object selectedEnum = Enum.Parse(typeof(ReturnTypes), selectedStr);
            Document.Returns = (ReturnTypes)selectedEnum;
            RefreshProperties();
        }

        /// <summary>
        /// A function type was changed
        /// </summary>
        private void chkType_CheckedChanged(object sender, EventArgs e)
        {
            Document.IsAggregate = chkType.Checked;
            RefreshProperties();
        }
        #endregion

        #region Auxiliary methods
        /// <summary>
        /// Subscribes from events of inner elements
        /// </summary>
        private void SubscribeEvents()
        {
            txtName.Leave += new EventHandler(txtName_Leave);
            txtDll.Leave += new EventHandler(txtDll_Leave);
            cboReturns.SelectedIndexChanged += new EventHandler(cboReturns_SelectedIndexChanged);
            chkType.CheckedChanged += new EventHandler(chkType_CheckedChanged);
        }

        /// <summary>
        /// Unsubscribes from events of inner elements
        /// </summary>
        private void UnsubscribeEvents()
        {
            txtName.Leave -= new EventHandler(txtName_Leave);
            txtDll.Leave -= new EventHandler(txtDll_Leave);
            cboReturns.SelectedIndexChanged -= new EventHandler(cboReturns_SelectedIndexChanged);
            chkType.CheckedChanged -= new EventHandler(chkType_CheckedChanged);
        }
        #endregion
    }
}