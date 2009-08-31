// Copyright (c) 2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System.Diagnostics;

namespace MySql.Data.MySqlClient
{
    internal class ResultSet
    {
        private MySqlDataReader reader;
        private Driver driver;
        private bool hasRows;
        private bool[] uaFieldsUsed;
        private MySqlField[] fields;
        private IMySqlValue[] values;
        private Hashtable fieldHashCS;
        private Hashtable fieldHashCI;
        private int rowIndex;
        private int resultsIndex;
        private bool readDone;
        private bool isSequential;
        private int seqIndex;
        private bool hasOutputParameters;

        public ResultSet(MySqlDataReader reader)
        {
            resultsIndex = -1;
            readDone = true;
            this.reader = reader;
            driver = reader.driver;
            fieldHashCS = new Hashtable();
            fieldHashCI = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        #region Properties

        public bool HasRows
        {
            get { return hasRows; }
        }

        public int RowIndex
        {
            get { return rowIndex; }
        }

        public int ResultsIndex
        {
            get { return resultsIndex; }
        }

        public int Size
        {
            get { return fields == null ? 0 : fields.Length; }
        }

        public MySqlField[] Fields
        {
            get { return fields; }
        }

        public IMySqlValue[] Values
        {
            get { return values; }
        }

        public bool HasOutputParameters
        {
            get { return hasOutputParameters; }
        }

        #endregion

        /// <summary>
        /// return the ordinal for the given column name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetOrdinal(string name)
        {
            // first we try a quick hash lookup
            object ordinal = fieldHashCS[name];
            if (ordinal != null)
                return (int)ordinal;

            // ok that failed so we use our CI hash
            ordinal = fieldHashCI[name];
            if (ordinal != null)
                return (int)ordinal;

            // Throw an exception if the ordinal cannot be found.
            throw new IndexOutOfRangeException(Resources.CouldNotFindColumnName);
        }

        public void ClearAll()
        {
            Close();
            while (NextResult())
                Close();
        }

        /// <summary>
        /// Retrieve the value as the given column index
        /// </summary>
        /// <param name="index">The column value to retrieve</param>
        /// <returns>The value as the given column</returns>
        public IMySqlValue this[int index]
        {
            get
            {
                if (rowIndex < 0)
                    throw new MySqlException(Resources.AttemptToAccessBeforeRead);

                // keep count of how many columns we have left to access
                uaFieldsUsed[index] = true;

                if (isSequential && index != seqIndex)
                {
                    if (index < seqIndex)
                        throw new MySqlException(Resources.ReadingPriorColumnUsingSeqAccess);
                    while (seqIndex < (index - 1))
                        driver.SkipColumnValue(values[++seqIndex]);
                    values[index] = driver.ReadColumnValue(index, fields[index], values[index]);
                    seqIndex = index;
                }

                return values[index];
            }
        }

        public bool NextRow(CommandBehavior behavior)
        {
            if (readDone) return false;

            if ((behavior & CommandBehavior.SingleRow) != 0 && rowIndex == 0)
                return false;

            isSequential = (behavior & CommandBehavior.SequentialAccess) != 0;
            seqIndex = -1;

            // if we are at row index >= 0 then we need to fetch the data row and load it
            if (rowIndex >= 0)
            {
                if (!driver.FetchDataRow(reader.Statement.StatementId, 0, Size))
                {
                    readDone = true;
                    return false;
                }
            }

            if (!isSequential) ReadColumnData(false);
            rowIndex++;
            return true;
        }

        /// <summary>
        /// Attempt to get the next resultset from the server
        /// </summary>
        /// <returns>true if a resultset was loaded, false otherwise</returns>
        public bool NextResult()
        {
            if (!driver.MoreResults)
                return false;

            rowIndex = -1;
            readDone = true;
            hasRows = false;
            hasOutputParameters = false;
            try
            {
                // see if resultset is on wire
                long count = 0;
                while (true)  //count <= 0 && driver.MoreResults)
                {
                    if (!driver.MoreResults) return false;
                    count = driver.ReadResult();
                    if (count == -1) continue;
                    if (count > 0) break;
                    reader.Command.lastInsertedId = driver.LastInsertedId;
                    reader.affectedRows = driver.AffectedRows;
                }

                uaFieldsUsed = new bool[count];
                LoadMetaData();

                // we know if we are in output parameters here
                bool inOutputParams = (driver.ServerStatus & ServerStatusFlags.OutputParameters) != 0;
                bool rowExists = driver.FetchDataRow(reader.Statement.StatementId, 0, fields.Length);
                if (rowIndex == -1 && !inOutputParams)
                    hasRows = rowExists;

                // if we are in the output parameters resultset then we are going to return false to our caller
                // so we go ahead and read the parameters now
                if (inOutputParams)
                {
                    ReadColumnData(true);
                    hasOutputParameters = true;
                    return false;
                }
                else
                {
                    resultsIndex++;
                    readDone = !rowExists;
                    return true;
                }
            }
			catch (MySqlException)
			{
                hasRows = false;
                readDone = true;
                throw;
            }
        }

        /// <summary>
        /// Closes the current resultset, dumping any data still on the wire
        /// </summary>
        public void Close()
        {
            if (readDone) return;

            while (!reader.Command.Canceled && driver.SkipDataRow()) { }
            readDone = true;

            MySqlConnection connection = reader.Command.Connection;

            if (!connection.Settings.UseUsageAdvisor) return;

            // we were asked to run the usage advisor so report if the resultset
            // was not entirely read.
            connection.UsageAdvisor.ReadPartialResultSet(reader.Command.CommandText);
       
            // now see if all fields were accessed
            bool readAll = true;
            foreach (bool b in uaFieldsUsed)
                readAll &= b;
            if (!readAll)
                connection.UsageAdvisor.ReadPartialRowSet(reader.Command.CommandText, uaFieldsUsed, fields);
        }

        public void SetValueObject(int i, IMySqlValue valueObject)
        {
            Debug.Assert(values != null);
            Debug.Assert(i < values.Length);
            values[i] = valueObject;
        }

        /// <summary>
        /// Loads the column metadata for the current resultset
        /// </summary>
        private void LoadMetaData()
        {
            fields = driver.ReadColumnMetadata(uaFieldsUsed.Length);
            fieldHashCS.Clear();
            fieldHashCI.Clear();
            values = new IMySqlValue[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                string columnName = fields[i].ColumnName;
                if (!fieldHashCS.ContainsKey(columnName))
                    fieldHashCS.Add(columnName, i);
                if (!fieldHashCI.ContainsKey(columnName))
                    fieldHashCI.Add(columnName, i);
                values[i] = fields[i].GetValueObject();
            }
        }

        private void ReadColumnData(bool outputParms)
        {
            for (int i = 0; i < Size; i++)
                values[i] = driver.ReadColumnValue(i, fields[i], values[i]);
            if (outputParms)
            {
                bool rowExists = driver.FetchDataRow(reader.Statement.StatementId, 0, fields.Length);
                rowIndex = 0;
                if (rowExists)
                    throw new MySqlException(Resources.MoreThanOneOPRow);
            }
        }
    }
}
