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
 * This file contains implementation of the columns details editing control.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This class implements custom user control for columndetails editing.
    /// </summary>
    public partial class ColumnDetails : UserControl
    {
        #region Private variables
        /// <summary>Variables to store properties</summary>
        private DataRow columnDataRow;
        private DataConnectionWrapper connectionRef;

        /// <summary>A manager of key events</summary>
        private KeyEventsManager keyEventsManager;
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColumnDetails()
        {
            InitializeComponent();

            // Fill text boxes tags
            nameText.AttributeName = Column.Name;
            datatypeText.Tag = Column.MySqlType;
            defaultValueText.Tag = Column.Default;
            commentText.Tag = Column.Comments;

            // Fill checkboxes tags
            primaryKey.Tag = Column.IsPrimaryKey;
            notNull.Tag = Column.Nullable;
            autoIncrement.Tag = Column.IsAutoIncrement;

            // Creating a key events manager
            keyEventsManager = new KeyEventsManager(this);

            // Disabled by default
            Enabled = false;
        }
        #endregion

        #region Public properties and methods
        /// <summary>
        /// Gets or sets data row with column attribues.
        /// </summary>
        public DataRow ColumnRow
        {
            get
            {
                return columnDataRow;
            }
            set
            {
                // If value is the same, exit
                if (columnDataRow == value)
                    return;

                // If already connect to row, release connection.
                if (columnDataRow != null)
                    Disconnect();

                // Set new value
                columnDataRow = value;

                // Connect to new row or reset view to empty state.
                if (columnDataRow != null)
                {
                    Enabled = true;
                    Connect();
                }
                else
                {
                    Enabled = false;
                    ResetView();
                }
            }
        }

        /// <summary>
        /// Gets or sets connection wrapper, used to read information about 
        /// character sets and collations.
        /// </summary>
        public DataConnectionWrapper Connection
        {
            get
            {
                return connectionRef;
            }
            set
            {
                Debug.Assert(connectionRef == null, "Dublicate connection!");
                connectionRef = value;

                // On first (and only one) set fill character sets combobox.
                if (connectionRef != null)
                    FillCharacterSets();
            }
        }

        /// <summary>
        /// Validate column data and applies pending changes.
        /// </summary>
        /// <returns>Returns false if data validation fails.</returns>
        public bool ValidateAndFlushChanges()
        {
            return nameText.ValidateAndCopyName();
        }
        #endregion

        #region Column row management
        /// <summary>
        /// Connects to events of the new row and refreshes the view.
        /// </summary>
        private void Connect()
        {
            columnDataRow.Table.RowChanged += new DataRowChangeEventHandler(OnRowChanged);
            Enabled = true;
            RefreshView();
        }

        /// <summary>
        /// Disconects from the events of the data row, reset its reference and reset view to empty state.
        /// </summary>
        private void Disconnect()
        {
            columnDataRow.Table.RowChanged -= new DataRowChangeEventHandler(OnRowChanged);
            columnDataRow = null;            
            ResetView();
        }

        /// <summary>
        /// Handles data row changes and refreshes view.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Information about event.</param>
        void OnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e != null && e.Row != null && e.Row == columnDataRow)
                RefreshView();
        }
        #endregion

        #region View management
        /// <summary>
        /// Fills view with data row data.
        /// </summary>
        private void RefreshView()
        {
            // If row is empty, reset view and exit.
            if (ColumnRow == null)
            {
                ResetView();
                return;
            }

            // Disconnect from text box change event handler for default value
            // or it will reset null to empty string
            defaultValueText.TextChanged -= new EventHandler(OnTextChanged);

            // Fill text boxes.
            nameText.DataSource = ColumnRow;
            datatypeText.Text = DataInterpreter.GetString(ColumnRow, Column.MySqlType);
            defaultValueText.Text = DataInterpreter.GetString(ColumnRow, Column.Default);
            commentText.Text = DataInterpreter.GetString(ColumnRow, Column.Comments);

            // Connect to text box change event handler for default value again
            defaultValueText.TextChanged += new EventHandler(OnTextChanged);


            // Fill column options.
            primaryKey.Checked = DataInterpreter.GetSqlBool(ColumnRow, Column.IsPrimaryKey).IsTrue;
            notNull.Checked = DataInterpreter.GetSqlBool(ColumnRow, Column.Nullable).IsFalse;
            autoIncrement.Checked = DataInterpreter.GetSqlBool(ColumnRow, Column.IsAutoIncrement).IsTrue;

            // Determine datatype and build flags set.
            string dataType = DataInterpreter.GetString(ColumnRow, Column.MySqlType);
            if (dataType == null)
                dataType = String.Empty;
            BuildFlags(dataType);


            // Enable or disable comboboxes
            charsetSelect.Enabled = collationSelect.Enabled = Parser.SupportCharacterSet(dataType);

            // Select caharacter set.
            SelectCharacterSet(DataInterpreter.GetString(ColumnRow, Column.CharacterSet));

            // Select collation
            SelectCollation(DataInterpreter.GetString(ColumnRow, Column.Collation));
        }

        /// <summary>
        /// Reset view to the empty state
        /// </summary>
        private void ResetView()
        {
            nameText.DataSource = null;
            datatypeText.Text = String.Empty;
            defaultValueText.Text = String.Empty;
            commentText.Text = String.Empty;

            primaryKey.Checked = false;
            notNull.Checked = false;
            autoIncrement.Checked = false;

            BuildFlags(String.Empty);

            SelectCharacterSet(null);
        }
        #endregion

        #region Character sets and collations
        /// <summary>
        /// Fills combobox with character sets
        /// </summary>
        private void FillCharacterSets()
        {
            charsetSelect.BeginUpdate();
            charsetSelect.Items.Clear();

            // Add default character set
            charsetSelect.Items.Add(new KeyDisplayValuePair(String.Empty, Resources.Default_CharacterSet));

            // Add available character sets
            charsetSelect.Items.AddRange(connectionRef.GetCharacterSets());

            charsetSelect.EndUpdate();
        }

        /// <summary>
        /// Process character set selection changes.
        /// </summary>
        /// <param name="characterSet">New character set to select</param>
        private void SelectCharacterSet(string characterSet)
        {
            // Select new character set or default character set if new is null or unknown.
            string value = characterSet != null && charsetSelect.Items.Contains(characterSet)
                            ? characterSet : String.Empty;

            // If value has changed, select it in the combobox and refil collations.
            if (!value.Equals(charsetSelect.SelectedItem))
            {
                charsetSelect.SelectedItem = value;
                FillCollations(value);
            }
        }

        /// <summary>
        /// Fills combobox with collations.
        /// </summary>
        /// <param name="characterSet">
        /// Character set for which collations should be loaded.
        /// </param>
        private void FillCollations(string characterSet)
        {
            // Fill collations
            collationSelect.BeginUpdate();
            collationSelect.Items.Clear();

            // Add default collation
            collationSelect.Items.Add(new KeyDisplayValuePair(String.Empty, Resources.Default_Collation));

            // Add availabel colations for character set
            if (!String.IsNullOrEmpty(characterSet))
            {
                collationSelect.Items.AddRange(Connection.GetCollationsForCharacterSet(characterSet));
                // Select default collation
                SelectCollation(Connection.GetDefaultCollationForCharacterSet(characterSet));
            }
            else
            {
                // Select empty string
                SelectCollation(String.Empty);
            }

            collationSelect.EndUpdate();
        }

        /// <summary>
        /// Selects new collation in the combobox.
        /// </summary>
        /// <param name="collation">New collation to select.</param>
        private void SelectCollation(string collation)
        {
            // Select new collation or default collation if new is null or unknown.
            string value = collation != null && collationSelect.Items.Contains(collation)
                            ? collation : String.Empty;

            // If value has changed, select it in the combobox and refil collations.
            if (!value.Equals(collationSelect.SelectedItem))
                collationSelect.SelectedItem = value;
        }
        #endregion

        #region Flags management
        /// <summary>
        /// Builds supported flags list for the given column datatype.
        /// </summary>
        /// <param name="dataType">Column datatype.</param>
        private void BuildFlags(string dataType)
        {
            flagsList.BeginUpdate();
            flagsList.Items.Clear();

            // If numeric, add UNSIGNED and ZEROFILL
            if (Parser.IsNumericType(dataType))
                AddNumericFlags();

            // If supports BINARY, add it
            if (Parser.SupportBinary(dataType))
                AddBinaryFlag();

            // If supports ASCII and UNICODE, add them
            if (Parser.SupportAsciiAndUnicode(dataType))
                AddAsciiAndUnicode();

            // Fill flags with data
            if (columnDataRow != null)
                FillFlags();

            flagsList.EndUpdate();
        }

        /// <summary>
        /// Sets flags check state for each fkag.
        /// </summary>
        private void FillFlags()
        {
            // Iterate through availabel flags. For each item key is the column name 
            // in the column data row to check for the flag value.
            for (int i = 0; i < flagsList.Items.Count; i++)
            {
                // Get item for the flag.
                KeyDisplayValuePair item = flagsList.Items[i] as KeyDisplayValuePair;
                // If value of the item is true, set cheched state
                if (item != null && item.Key != null
                    && DataInterpreter.GetSqlBool(ColumnRow, item.Key.ToString()))
                    flagsList.SetItemCheckState(i, CheckState.Checked);
                else
                    flagsList.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        /// <summary>
        /// Adds numeric flags (UNSIGNED and ZEROFILL)
        /// </summary>
        private void AddNumericFlags()
        {
            flagsList.Items.Add(new KeyDisplayValuePair(Column.Unsigned, Resources.Unsigned_Flag));
            flagsList.Items.Add(new KeyDisplayValuePair(Column.Zerofill, Resources.Zerofill_Flag));
        }

        /// <summary>
        /// Adds BINARY option flag.
        /// </summary>
        private void AddBinaryFlag()
        {
            flagsList.Items.Add(new KeyDisplayValuePair(Column.Binary, Resources.Binary_Flag));
        }

        /// <summary>
        /// Adds ASCII and UNICODE flags.
        /// </summary>
        private void AddAsciiAndUnicode()
        {
            flagsList.Items.Add(new KeyDisplayValuePair(Column.Ascii, Resources.Ascii_Flag));
            flagsList.Items.Add(new KeyDisplayValuePair(Column.Unicode, Resources.Unicode_Flag));
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Handles changes in the textboxes.
        /// </summary>
        /// <param name="sender">Text box wich raise event.</param>
        /// <param name="e">Information about event.</param>
        private void OnTextChanged(object sender, EventArgs e)
        {
            // Validate row
            if (ColumnRow == null)
                return;

            // Get attribute name
            string itemName = GetAttrbuteName(sender);

            // Set attribute
            if (!String.IsNullOrEmpty(itemName))
                SetAttribute(itemName, GetText(sender));
        }

        /// <summary>
        /// Handles changes in the checkboxes direct (checked to true).
        /// </summary>
        /// <param name="sender">Checkbox which raises event.</param>
        /// <param name="e">Information about event.</param>
        private void OnCheckedChanged(object sender, EventArgs e)
        {
            // Get attribute name
            string itemName = GetAttrbuteName(sender);

            // Set attribute
            if (!String.IsNullOrEmpty(itemName))
                SetAttribute(itemName, GetChecked(sender) ? DataInterpreter.True : DataInterpreter.False);
        }

        /// <summary>
        /// Handles changes in the checkboxes reversed (checked to false).
        /// </summary>
        /// <param name="sender">Checkbox which raises event.</param>
        /// <param name="e">Information about event.</param>
        private void OnCheckedChangedReversed(object sender, EventArgs e)
        {
            // Get attribute name
            string itemName = GetAttrbuteName(sender);

            // Set attribute
            if (!String.IsNullOrEmpty(itemName))
                SetAttribute(itemName, GetChecked(sender) ? DataInterpreter.False : DataInterpreter.True);
        }

        /// <summary>
        /// Handles changes in the checked list box with flags.
        /// </summary>
        /// <param name="sender">Event sender. Must be flagsList.</param>
        /// <param name="e">Information about event.</param>
        private void OnFlagsListItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Validate event data
            if (e == null)
            {
                Debug.Fail("Empty event data!");
                return;
            }

            // Extract item
            KeyDisplayValuePair item = flagsList.Items[e.Index] as KeyDisplayValuePair;
            if (item == null || String.IsNullOrEmpty(item.Key as string))
            {
                Debug.Fail("Empty item!");
                return;
            }

            // Set row value
            SetAttribute((string)item.Key, e.NewValue == CheckState.Checked ? DataInterpreter.True : DataInterpreter.False);
        }


        /// <summary>
        /// Handles character set changes.
        /// </summary>
        /// <param name="sender">Event sender. Must be charsetSelect.</param>
        /// <param name="e">Information about event.</param>
        private void OnCharacterSetChanged(object sender, EventArgs e)
        {
            // Get selected value
            string value = GetSelectedValue(charsetSelect);

            // Change column attribute
            if (String.IsNullOrEmpty(value))
                SetAttribute(Column.CharacterSet, DBNull.Value);
            else
                SetAttribute(Column.CharacterSet, value);

            // Fill collations for new character set
            FillCollations(value);

            // Force collation changed event
            OnCollationChanged(collationSelect, EventArgs.Empty);
        }

        /// <summary>
        /// Handles collation changes.
        /// </summary>
        /// <param name="sender">Event sender. Must be collationSelect.</param>
        /// <param name="e">Information about event.</param>
        private void OnCollationChanged(object sender, EventArgs e)
        {
            // Get selected value
            string value = GetSelectedValue(collationSelect);

            // Change column attribute
            if (String.IsNullOrEmpty(value))
                SetAttribute(Column.Collation, DBNull.Value);
            else
                SetAttribute(Column.Collation, value);
        }


        /// <summary>
        /// Resets default value to NULL on button click.
        /// </summary>
        /// <param name="sender">Event sender. Must be buttonNull.</param>
        /// <param name="e">Information about event.</param>
        private void OnButtonNullClick(object sender, EventArgs e)
        {
            // Need to set DBNull twice, because it dosen't always reset it from the first time.
            SetAttribute(Column.Default, DBNull.Value);
            SetAttribute(Column.Nullable, DataInterpreter.True);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Returns tag of the sender casted to the string.
        /// </summary>
        /// <param name="sender">Sender object to work with.</param>
        /// <returns>Returns tag of the sender casted to the string.</returns>
        private static string GetAttrbuteName(object sender)
        {
            // Get and validate server control
            Control cont = sender as Control;
            if (cont == null)
            {
                Debug.Fail("Icorrect event sender!");
                return null;
            }

            // Validate tag
            if (!(cont.Tag is string))
            {
                Debug.Fail("Empty or wrong tag for sender!");
                return null;
            }

            return (string)cont.Tag;
        }

        /// <summary>
        /// Returns text of the sender control.
        /// </summary>
        /// <param name="sender">Sender object to work with.</param>
        /// <returns>Returns text of the sender control.</returns>
        private static string GetText(object sender)
        {
            // Get and validate server control
            Control cont = sender as Control;
            if (cont == null)
            {
                Debug.Fail("Icorrect event sender!");
                return null;
            }

            return cont.Text;
        }

        /// <summary>
        /// Returns checked state of the sender checkbox.
        /// </summary>
        /// <param name="sender">Sender object to work with.</param>
        /// <returns>Returns checked state of the sender checkbox.</returns>
        private static bool GetChecked(object sender)
        {
            // Get checkbox
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null)
            {
                Debug.Fail("Sender is not checkbox");
                return false;
            }

            return checkBox.Checked;
        }


        /// <summary>
        /// Sets value of given attribute in the column data row. 
        /// </summary>
        /// <param name="attribute">Name of the attribute to set.</param>
        /// <param name="value">New value for the atribute</param>
        private void SetAttribute(string attribute, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            // Validate row
            if (ColumnRow == null)
                return;

            // Validate attribute name
            DataInterpreter.SetValueIfChanged(ColumnRow, attribute, value);
        }

        /// <summary>
        /// Returns selected vlue from the combobox casted to string.
        /// </summary>
        /// <param name="source">Source combobox to extract value.</param>
        /// <returns>
        /// Returns selected vlue from the combobox casted to string.
        /// </returns>
        private string GetSelectedValue(ComboBox source)
        {
            string value;
            if (source.SelectedItem is KeyDisplayValuePair)
            {
                KeyDisplayValuePair item = source.SelectedItem as KeyDisplayValuePair;
                value = item != null ? item.Key as string : String.Empty;
            }
            else
            {
                value = source.SelectedItem as string;
            }
            return value;
        }
        #endregion
    }
}