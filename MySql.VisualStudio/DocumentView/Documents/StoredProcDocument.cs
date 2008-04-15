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
using System.ComponentModel;
using System.Data;
using System.Text;

using MySql.Data.VisualStudio.Descriptors;
using StoredProc = MySql.Data.VisualStudio.Descriptors.StoredProcDescriptor.Attributes;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;
using System.Globalization;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Implements a document functionality and represent a database stored procedure
    /// </summary>
    [DocumentObject(StoredProcDescriptor.TypeName, typeof(StoredProcDocument))]
    public class StoredProcDocument : BaseDocument, ISqlSource
    {
        #region Constants
        /// <summary>Keyword for a procedure</summary>
        private const string ProcedureType = "PROCEDURE";

        /// <summary>Keyword for a function</summary>
        private const string FunctionType = "FUNCTION";

        /// <summary>Keyword for a function's return type</summary>
        private const string ReturnsKey = "RETURNS";

        /// <summary>Valid starting of an MySql statement</summary>
        private static readonly string[] Statements =
        {
            "BEGIN",
            "RETURN",
            "SELECT",
            "INSERT",
            "UPDATE",
            "DELETE",
            "CREATE",
            "DROP",
            "ALTER",
            "RENAME"
        };

        /// <summary>Comment characteristic for routine.</summary>
        private const string Comment = "COMMENT";
        /// <summary>Current user function name.</summary>
        private const string CurretnUser = "CURRENT_USER";
        #endregion

        #region Private variables

        private string defValue;
        private RoutineTypes typeVal;

        #endregion

        #region Enumerations
        /// <summary>
        /// Types of access to data
        /// </summary>
        public enum DataAccessTypes
        {
            CONTAINS_SQL,
            NO_SQL,
            READS_SQL_DATA,
            MODIFIES_SQL_DATA
        }

        /// <summary>
        /// Types of stability of the routine's results
        /// </summary>
        public enum Deterministic
        {
            NO,
            YES
        }

        /// <summary>
        /// Types of routines
        /// </summary>
        public enum RoutineTypes
        {
            PROCEDURE,
            FUNCTION
        }
        #endregion

        #region Identifying properties
        /// <summary>
        /// Type of the routine
        /// </summary>
        [Browsable(false)]
        public RoutineTypes Type
        {
            get
            {
                if (!IsAttributesLoaded)
                    return typeVal;

                return GetRoutineType(GetAttributeAsString(StoredProc.Type));
            }

            set
            {
                SetAttribute(StoredProc.Type, value.ToString());
            }
        }

        /// <summary>
        /// SQL definition of the procedure
        /// </summary>
        [Browsable(false)]
        public string SqlSource
        {
            get
            {
                return Definition;
            }

            set
            {
                Definition = value;
            }
        }

        /// <summary>
        /// ID of the routine; includes it's type
        /// </summary>
        [Browsable(false)]
        public override object[] ObjectID
        {
            get
            {
                return new object[] { null, Schema, Name };
            }
        }

        /// <summary>
        /// The old ID of the routine (before changes)
        /// </summary>
        [Browsable(false)]
        public override object[] OldObjectID
        {
            get
            {
                return new object[] { null, Schema, OldName };
            }
        }
        #endregion

        internal bool IsFunction
        {
            get { return typeVal == RoutineTypes.FUNCTION; }
            set { typeVal = value ? RoutineTypes.FUNCTION : RoutineTypes.PROCEDURE; }
        }

        #region Checking properties
        /// <summary>
        /// Checks if just a few certain characteristics of the routine have been 
        /// changed
        /// </summary>
        private bool JustCharAltered
        {
            get
            {
                if (Attributes == null)
                    return false;

                return
                    !IsAttributeChanged(StoredProc.Definer) &&
                    !IsAttributeChanged(StoredProc.Type) &&
                    !IsAttributeChanged(StoredProc.Name) &&
                    !IsAttributeChanged(StoredProc.ParameterList) &&
                    !IsAttributeChanged(StoredProc.Returns) &&
                    !IsAttributeChanged(StoredProc.IsDeterministic) &&
                    !IsAttributeChanged(StoredProc.Definition);
            }
        }
        #endregion
        
        #region Definition of the routine
        /// <summary>
        /// Body of the stored routine
        /// </summary>
        private string Body
        {
            get
            {
                return GetAttributeAsString(StoredProc.Definition);
            }
        }

        /// <summary>
        /// Header of the stored routine; includes a new line
        /// </summary>
        private string Header
        {
            get
            {
                string procName = Type + " " + Name + " ";
                string paramList = "(" + Parameters + ")";
                string returnClause = Returns;
                string newLine = Environment.NewLine;

                return procName + paramList + returnClause + newLine;
            }
        }

        /// <proc>
        /// List of parameters
        /// </summary>
        private string Parameters
        {
            get
            {
                return GetAttributeAsString(StoredProc.ParameterList);
            }

            set
            {
                SetAttribute(StoredProc.ParameterList, value);
            }
        }

        /// <summary>
        /// Return clause of the routine
        /// </summary>
        private string Returns
        {
            get
            {
                if (Type != RoutineTypes.FUNCTION)
                    // Nothing to return
                    return string.Empty;

                // Generating the return clause including the leading space
                string returnType = GetAttributeAsString(StoredProc.Returns);
                if (string.IsNullOrEmpty(returnType))
                    returnType = "INT";
                return " " + ReturnsKey + " " + returnType;
            }

            set
            {
                SetAttribute(StoredProc.Returns, value);
            }
        }

        /// <summary>
        /// Full definition of the routine
        /// </summary>
        [Browsable(false)]
        public string Definition
        {
            get
            {
                return defValue;
            }

            set
            {
                // Here we just save the current definition. We parse the definition 
                // just before save
                defValue = value;
            }
        }
        #endregion

        #region Displayable properties
        /// <summary>
        /// Provides information about type of access to data
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_StoredProcedure_DataAccess")]
        [LocalizableDisplayName("DisplayName_StoredProcedure_DataAccess")]
        [DefaultValue(DataAccessTypes.CONTAINS_SQL)]
        public DataAccessTypes DataAccess
        {
            get
            {
                return (DataAccessTypes)GetAttributeAsSpacedEnum(StoredProc.DataAccess, DataAccessTypes.CONTAINS_SQL);
            }

            set
            {
                SetAttributeAsSpacedEnum(StoredProc.DataAccess, value);
            }
        }

        /// <summary>
        /// Definer of the view
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Object_Definer")]
        [LocalizableDisplayName("DisplayName_Object_Definer")]
        [DefaultValue(null)]
        public string Definer
        {
            get
            {
                string definer = GetAttributeAsString(StoredProc.Definer);
                if (definer == null)
                    return null;
                definer = definer.Trim();
                
                // Return name of current user if CURRENT_USER
                if (DataInterpreter.CompareInvariant(definer, CurretnUser))
                    return Connection.CurrentUser;

                // Return name of user
                return definer;
            }

            set
            {
                SetAttribute(StoredProc.Definer, value);
            }
        }

        /// <summary>
        /// Indicates if the routine returns the same result for the same input
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_StoredProcedure_IsDeterministic")]
        [LocalizableDisplayName("DisplayName_StoredProcedure_IsDeterministic")]
        [DefaultValue(Deterministic.NO)]
        public Deterministic IsDeterministic
        {
            get
            {
                return (Deterministic)GetAttributeAsEnum(StoredProc.IsDeterministic, Deterministic.NO);
            }

            set
            {
                SetAttribute(StoredProc.IsDeterministic, value.ToString());
            }
        }

        /// <summary>
        /// A name of the routine. Should be readonly here
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Object_Name")]
        [LocalizableDisplayName("DisplayName_Object_Name")]
        [ReadOnly(true)]
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// Type of security for actions on the view
        /// </summary>
        [LocalizableCategory("Category_Security")]
        [LocalizableDescription("Description_Object_SecurityType")]
        [LocalizableDisplayName("DisplayName_Object_SecurityType")]
        [DefaultValue(SecurityTypes.DEFINER)]
        public SecurityTypes SecurityType
        {
            get
            {
                return (SecurityTypes)GetAttributeAsEnum(StoredProc.SecurityType, SecurityTypes.DEFINER);
            }

            set
            {
                SetAttribute(StoredProc.SecurityType, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes private variables
        /// </summary>
        /// <param name="hierarchy">A data view hierarchy accessor used to interact 
        /// with Server Explorer. Also used to extract connection</param>
        /// <param name="isNew">Indicates if the instance of the class represents a 
        /// new database object which hasn't yet been stored in a database</param>
        /// <param name="id">An array containing the object's identifier</param>
        public StoredProcDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            : base(hierarchy, isNew, id)
        {
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Aditionaly checks for definition differences.
        /// </summary>
        protected override bool IsDirty
        {
            get
            {
                return base.IsDirty || !DataInterpreter.CompareInvariant(Definition, Header + Body);
            }
        }
        /// <summary>
        /// Returns a correct procedure comment attribute name
        /// </summary>
        protected override string CommentAttributeName
        {
            get
            {
                return StoredProc.Comment;
            }
        }

        /// <summary>
        /// Initialize attributes for a new routine
        /// </summary>
        /// <param name="newRow">A data row to write new object's attributes in</param>
        protected override void FillNewObjectAttributes(DataRow newRow)
        {
            base.FillNewObjectAttributes(newRow);

            newRow[StoredProc.Type] = Type;
            string body = "BEGIN " + Environment.NewLine + Environment.NewLine + "END";
            newRow[StoredProc.Definition] = body;
        }

/*        public override object[] ObjectIDForLoad
        {
            get
            {
                object[] id = ObjectID;
                object[] newId = new object[4];
                newId[0] = id[0];
                newId[1] = id[1];
                newId[3] = typeVal;
                newId[2] = id[2];
                return newId;
            }
        }
		*/
        /// <summary>
        /// Loads a routine from a database
        /// </summary>
        /// <param name="reloading">Indicates if the routine is reloading or loading 
        /// for the first time</param>
        /// <returns>True if load succeeds, and false otherwise</returns>
        protected override bool LoadData(bool reloading)
        {
            if (!base.LoadData(reloading))
                return false;

            defValue = Header + Body;
            return true;
        }

        /// <summary>
        /// Parses and validates input data
        /// </summary>
        /// <returns>True if parsing and validating both succeed</returns>
        protected override bool ValidateData()
        {
            if (!ParseData())
                return false;

            if (DataInterpreter.HasChanged(Attributes, StoredProc.Type))
            {
                UIHelper.ShowError(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_ChangeTypeIsNotAllowed,
                    Name,
                    DataInterpreter.GetStringNotNull(Attributes, StoredProc.Type, DataRowVersion.Original),
                    Type));
                return false;
            }

            return base.ValidateData();
        }
        #endregion

        #region Building of queries
        /// <summary>
        /// Creates a query on a creation of a routine
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildCreateQuery()
        {
            StringBuilder query = new StringBuilder();
                       
            // Build main create query
            CreateQuery(query);

            // Return results
            return query.ToString();
        }

        /// <summary>
        /// Returns query to pre-drop routine, if pre-drop is needed.
        /// </summary>
        /// <returns></returns>
        protected override string BuildPreDropQuery()
        {
            // Only routine characteristics were changed, simple alter query should be generated.
            if (JustCharAltered)
                return String.Empty;

            // Routine body or parameter list were changed, routine should be dropped and recreated.
            return String.Format(
                CultureInfo.InvariantCulture,
                "DROP {0} IF EXISTS {1}",
                new object[] { GetRoutineType(GetOldAttributeAsString(StoredProc.Type)), OldName });
        }

        /// <summary>
        /// Creates a query on a alteration of a routine. To alter a routine, one 
        /// should drop it and then create a new routine
        /// </summary>        
        protected override string BuildAlterQuery()
        {
            // Only routine characteristics were changed, simple alter query should be generated.
            if (JustCharAltered)
                // Creating a simple "ALTER" query
                return BuildCharAlter();

            // Routine body or parameter list were changed, routine should be recreated.
            StringBuilder query = new StringBuilder();

            // Build main create query
            CreateQuery(query);

            // Return results
            return query.ToString();
        }

        /// <summary>
        /// Creates a query for creation of a routine
        /// </summary>
        /// <param name="query">StringBuilder to work with.</param>   
        private void CreateQuery(StringBuilder query)
        {
            // Adding the name of the action
            query.Append("CREATE");

            // Adding a definer, if is not default
            QueryBuilder.WriteUserNameIfNotEmpty(Definer, " DEFINER = ", query);

            // Adding a routine's header
            query.Append(" ");
            query.Append(Type);
            query.Append(" ");
            query.Append(Name);
            query.Append(" (");
            query.Append(Parameters);
            query.Append(")");

            if (Type == RoutineTypes.FUNCTION)
                // Adding a return type
                query.Append(Returns);

            // Adding characteristics, if are not default
            if (IsDeterministic != Deterministic.NO)
            {
                query.Append(" ");
                query.Append(GetDeterministic(IsDeterministic));
            }

            if (DataAccess != DataAccessTypes.CONTAINS_SQL)
            {
                query.Append(" ");
                QueryBuilder.WriteValue(Attributes, StoredProc.DataAccess, query, false);
            }

            if (SecurityType != SecurityTypes.DEFINER)
            {
                query.Append(" SQL SECURITY ");
                QueryBuilder.WriteValue(Attributes, StoredProc.SecurityType, query, false);
            }

            if (!string.IsNullOrEmpty(Comments))
            {
                query.Append(" COMMENT ");
                QueryBuilder.WriteValue(Attributes, StoredProc.Comment, query);
            }

            // Adding a body
            query.Append(" ");
            query.Append(Environment.NewLine);
            QueryBuilder.WriteValue(Attributes, StoredProc.Definition, query, false);
        }

        /// <summary>
        /// Creates a query for alteration of a routine
        /// </summary>
        /// <returns>A constructed query</returns>
        private string BuildCharAlter()
        {
            StringBuilder sb = new StringBuilder();

            // Adding the name of the action
            sb.Append("ALTER");

            // Adding a routine's header
            sb.Append(" ");
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);

            // Adding characteristics, if are not default
            QueryBuilder.WriteIfChanged(Attributes, StoredProc.DataAccess, " ", sb, false);
            QueryBuilder.WriteIfChanged(Attributes, StoredProc.SecurityType, " SQL SECURITY ", sb, false);
            QueryBuilder.WriteIfChanged(Attributes, StoredProc.Comment, " COMMENT ", sb);

            return sb.ToString();
        }       
        #endregion

        #region Data parsing
        /// <summary>
        /// Parses and validates input data
        /// </summary>
        /// <returns>True if parsing succeeds</returns>
        private bool ParseData()
        {
            // Getting the full routine's description
            string routineDef = Definition;
            if (string.IsNullOrEmpty(routineDef))
                return false;

            // Searching the end of a routine's header
            int bracketIndex = Parser.LocateUnquoted(routineDef, "(");
            if (bracketIndex < 2)
            {
                UIHelper.ShowError(Resources.Error_WrongParameterList);
                return false;
            }

            // Extract braces content
            string bracesContent = Parser.ExtractUnbracedExpression(routineDef, bracketIndex);
            if (bracesContent == null)
            {
                UIHelper.ShowError(Resources.Error_WrongParameterList);
                return false;
            }

            // Adjust braces index to point closing brace
            bracketIndex += bracesContent.Length + 1;

            // Parsing a header
            
            string routineHeader = routineDef.Substring(0, bracketIndex + 1);
            if (!ParseHeader(routineHeader))
                return false;

            // Parsing a body
            string routineBody = routineDef.Substring(bracketIndex + 1).Trim();
            return ParseBody(routineBody);
        }

        /// <summary>
        /// Parses a routine header
        /// </summary>
        /// <param name="header">A header to parse</param>
        /// <returns>True if parsing succeeds</returns>
        private bool ParseHeader(string header)
        {
            // Determining a type of a routine
            string routineType = string.Empty;

            // Checking if the routine is a procedure
            int typeIndex = IndexOfToken(header, ProcedureType);
            if (typeIndex >= 0)
                routineType = ProcedureType;
            else
            {
                typeIndex = IndexOfToken(header, FunctionType);
                if (typeIndex >= 0)
                    routineType = FunctionType;
                else
                {
                    // Incorrect routine type
                    UIHelper.ShowError(Resources.Error_IncorrectRoutineType);
                    return false;
                }
            }

            // Assigning a type to the routine
            Type = GetRoutineType(routineType);

            // Getting a definer if exists
            string definer = header.Substring(0, typeIndex);
            int eqIndex = Parser.LocateUnquoted(definer, "=");
            if (eqIndex >= 0 && eqIndex < definer.Length - 1)
                Definer = definer.Substring(eqIndex + 1).Trim();

            // Getting a name and a parameter list
            string afterType = header.Substring(typeIndex + routineType.Length).Trim();
            int startParam = Parser.LocateUnquoted(afterType, "(");

            // A name should occupy at least one position
            if (startParam < 1)
            {
                UIHelper.ShowError(Resources.Error_IncorrectParameterList);
                return false;
            }

            // Assigning a name
            Name = afterType.Substring(0, startParam).Trim();

            // Assigning a parameter list
            Parameters = Parser.ExtractUnbracedExpression(afterType, startParam);

            return true;
        }

        /// <summary>
        /// Parses a routine's body
        /// </summary>
        /// <param name="routineBody">A body to parse</param>
        /// <returns>True if parsing succeeds</returns>
        private bool ParseBody(string routineBody)
        {
            // Extracting comments
            routineBody = ExtractComments(routineBody);

            // Seacrhing for a first statement of a body
            int startIndex = BodyStart(routineBody);
            if (startIndex == routineBody.Length)
            {
                UIHelper.ShowError(Resources.Error_UnrecognizedBody);
                return false;
            }

            // Saving statements of a body
            SetAttribute(StoredProc.Definition, routineBody.Substring(startIndex));

            // Parsing characteristics of the routine
            startIndex = ParseCharacteristics(routineBody.Substring(0, startIndex));

            // Checking a return type
            if (Type == RoutineTypes.FUNCTION)
            {
                string returnClause = routineBody.Substring(0, startIndex);
                int returnIndex = Parser.LocateUnquoted(returnClause, ReturnsKey);
                if (returnIndex < 0)
                {
                    UIHelper.ShowError(Resources.Error_NoReturnType);
                    return false;
                }

                string returnType = returnClause.Substring(returnIndex + ReturnsKey.Length).Trim();
                Returns = returnType;
            }

            return true;
        }

        /// <summary>
        /// Searches for a routine's characteristics in a string which can contain 
        /// only the "RETURNS" clause and characteristics
        /// </summary>
        /// <param name="routineStart">A string to parse</param>
        /// <returns>The position where characteristics begin</returns>
        private int ParseCharacteristics(string routineStart)
        {
            string str = routineStart.ToUpper();
            int startIndex = str.Length;

            // Searching for the "LANGUAGE" clause
            int pos = IndexOfToken(str, "LANGUAGE");
            if (pos >= 0 && pos < startIndex)
                startIndex = pos;

            // Searching for the "DETERMINISTIC" clause
            pos = IndexOfToken(str, "NOT");
            if (pos >= 0)
            {
                IsDeterministic = Deterministic.NO;
                if (pos < startIndex)
                    startIndex = pos;
            }
            else
            {
                pos = IndexOfToken(str, "DETERMINISTIC");
                if (pos >= 0)
                {
                    IsDeterministic = Deterministic.YES;
                    if (pos < startIndex)
                        startIndex = pos;
                }
            }

            // Searching for the "CONTAINS SQL" clase
            pos = IndexOfToken(str, "CONTAINS");
            if (pos >= 0)
            {
                DataAccess = DataAccessTypes.CONTAINS_SQL;
                if (pos < startIndex)
                    startIndex = pos;
            }

            // Searching for the "NO SQL" clase
            pos = IndexOfToken(str, "NO");
            if (pos >= 0)
            {
                DataAccess = DataAccessTypes.NO_SQL;
                if (pos < startIndex)
                    startIndex = pos;
            }

            // Searching for the "READS SQL DATA" clase
            pos = IndexOfToken(str, "READS");
            if (pos >= 0)
            {
                DataAccess = DataAccessTypes.READS_SQL_DATA;
                if (pos < startIndex)
                    startIndex = pos;
            }

            // Searching for the "MODIFIES SQL DATA" clase
            pos = IndexOfToken(str, "MODIFIES");
            if (pos >= 0)
            {
                DataAccess = DataAccessTypes.MODIFIES_SQL_DATA;
                if (pos < startIndex)
                    startIndex = pos;
            }

            // Searching for the "SQL SECURITY" clause
            pos = IndexOfToken(str, "SECURITY");
            if (pos >= 0)
            {
                if (IndexOfToken(str, "DEFINER") >= 0)
                    SecurityType = SecurityTypes.DEFINER;
                else if (IndexOfToken(str, "INVOKER") >= 0)
                    SecurityType = SecurityTypes.INVOKER;

                // Searching for the first occurence of the "SQL" keyword
                int sqlIndex = IndexOfToken(str, "SQL");
                if (sqlIndex > pos)
                    UIHelper.ShowError(Resources.Warning_WrongSecurityType);
                else
                    pos = sqlIndex;

                if (pos < startIndex)
                    startIndex = pos;
            }

            return startIndex;
        }

        /// <summary>
        /// Finds start index of the body itself in a string which can include return 
        /// type and routine's characteristics
        /// </summary>
        /// <param name="routineBody">A body in a wide sense</param>
        /// <returns>The start index of a body in a narrow sense</returns>
        private int BodyStart(string routineBody)
        {
            string body = routineBody.ToUpper();
            int startIndex = body.Length;

            // Searching the earliest occurence of a valid statement
            foreach (string statement in Statements)
            {
                int position = IndexOfToken(body, statement);
                if (position >= 0 && position < startIndex)
                    startIndex = position;
            }

            return startIndex;
        }

        /// <summary>
        /// Extracts comments form a query string
        /// </summary>
        /// <param name="query">A query string to extract comments from</param>
        /// <returns>A new query without the "COMMENT" clause</returns>
        private string ExtractComments(string query)
        {
            int pos = IndexOfToken(query, Comment);
            if (pos < 0)
                // No comments; returning the initial query
                return query;
            
            int startQuote = query.IndexOf("'", pos);
            int endQuote;
            string comments = Parser.ExtractUnquotedValue(query, startQuote, out endQuote);
            
            if (startQuote < 0 || endQuote < 0)
                UIHelper.ShowWarning(Resources.Warning_IncorrectComments);
            else
                // Saving comments from the initial string
                Comments = comments;

            // Cutting the comment for the following parsing
            string beforeComment = (pos > 0) ? query.Substring(0, pos - 1) : string.Empty;
            string afterComment =
                (endQuote < query.Length - 1) ?
                query.Substring(endQuote + 1) :
                string.Empty;

            return beforeComment + afterComment;
        }
        #endregion

        #region Auxiliary methods

        private RoutineTypes GetRoutineType(string typeStr)
        {
            try
            {
                object routineType = Enum.Parse(typeof(RoutineTypes), typeStr, true);
                return (RoutineTypes)routineType;
            }
            catch (Exception e)
            {
                throw new ArgumentException("Incorrect routine type", e);
            }
        }

        /// <summary>
        /// Casts the "IsDeterministic" property to a valid MySql clause
        /// </summary>
        /// <param name="value">The enumeration value</param>
        /// <returns>The "IsDeterministic" property as a valid string</returns>
        private string GetDeterministic(Deterministic value)
        {
            if (value == Deterministic.YES)
                return "DETERMINISTIC";

            return "NOT DETERMINISTIC";
        }

        /// <summary>
        /// Searches for the occurence of the given token in a given string
        /// </summary>
        /// <param name="str">A string to find in</param>
        /// <param name="value">A token to find</param>
        /// <returns>The position of the first occurence of the given token</returns>
        private int IndexOfToken(string str, string value)
        {
            int pos = Parser.LocateUnquoted(str, value);
            while (pos >= 0)
            {
                // Checking if we have reached the end of the string
                int nextCharIndex = pos + value.Length;
                if (nextCharIndex == str.Length)
                    return pos;

                char nextChar = str[pos + value.Length];
                if (Parser.IsWhiteSpace(nextChar) || Parser.IsDelimiter(nextChar))
                {
                    if (pos == 0)
                        return pos;

                    char prevChar = str[pos - 1];
                    if (Parser.IsWhiteSpace(prevChar))
                        return pos;
                }

                // Checking if we have reached the end of the string
                if (pos == str.Length - 1)
                    return -1;

                pos = Parser.LocateUnquoted(str, value, pos + 1);
            }

            return -1;
        }
        #endregion
    }
}
