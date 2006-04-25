using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Globalization;

namespace MySql.VSTools
{
    internal class ColumnNode : ExplorerNode
    {
        private string type;
        private int maxLen = -1;
        private bool isPrimary;
        private bool allowNulls;
        private bool isBinary;
        private bool isZeroFill;
        private string charSet;
        private string collation;
        private string defaultValue;
        private string comment;
        private bool isUnsigned;
        private bool isAutoincrement;
        private bool isDirty;

        public ColumnNode(ExplorerNode parent, DataRow row)
            : base(parent, row["COLUMN_NAME"].ToString())
        {
            string extra = row["EXTRA"].ToString().ToLower(CultureInfo.InvariantCulture);
            type = row["DATA_TYPE"].ToString();
            ConvertTypeName();
            string temp = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
            if (temp != "NULL" && temp.Length > 0)
                maxLen = Int32.Parse(temp);
            isPrimary = row["COLUMN_KEY"].ToString().IndexOf("PRI") != -1;
            allowNulls = row["IS_NULLABLE"].Equals("YES");
            isBinary = extra.IndexOf("binary") != -1;
            isZeroFill = extra.IndexOf("zero") != -1;
            charSet = row["CHARACTER_SET_NAME"].ToString();
            collation = row["COLLATION_NAME"].ToString();
            //TODO: ask Gluh about this
            defaultValue = row["COLUMN_DEFAULT"].ToString();
            comment = row["COLUMN_COMMENT"].ToString();
            isUnsigned = type.IndexOf("unsigned") != -1;
            isAutoincrement = extra.IndexOf("auto_increment") != -1;
        }

        #region Properties

        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        public override string Name
        {
            get { return name; }
            set { isDirty |= (value != name); name = value; }
        }

        public string Typename
        {
            get { return type; }
            set { isDirty |= (value != type); type = value; }
        }

        public int CharacterMaxLength
        {
            get { return maxLen; }
            set { isDirty |= (value != maxLen); maxLen = value; }
        }

        public bool IsPrimary
        {
            get { return isPrimary; }
            set { isDirty |= (value != isPrimary); isPrimary = value; }
        }

        public bool AllowNulls
        {
            get { return allowNulls; }
            set { isDirty |= (value != allowNulls); allowNulls = value; }
        }

        public bool IsBinary
        {
            get { return isBinary; }
            set { isDirty |= (value != isBinary); isBinary = value; }
        }

        public bool IsZeroFill
        {
            get { return isZeroFill; }
            set { isDirty |= (value != isZeroFill); isZeroFill = value; }
        }

        public bool IsUnsigned
        {
            get { return isUnsigned; }
            set { isDirty |= (value != isUnsigned); isUnsigned = value; }
        }

        public bool IsAutoIncrement
        {
            get { return isAutoincrement; }
            set { isDirty |= (value != isAutoincrement); isAutoincrement = value; }
        }

        public string CharacterSet
        {
            get { return charSet; }
            set { isDirty |= (value != charSet); charSet = value; }
        }

        public string Collation
        {
            get { return collation; }
            set { isDirty |= (value != collation); collation = value; }
        }

        public string DefaultValue
        {
            get { return defaultValue; }
            set { isDirty |= (value != defaultValue); defaultValue = value; }
        }

        public string Comment
        {
            get { return comment; }
            set { isDirty |= (value != comment); comment = value; } 
        }

        #endregion

        private void ConvertTypeName()
        {
            type = type.ToUpper(CultureInfo.InvariantCulture);
            switch (type)
            {
                case "INT":
                    type = "INTEGER";
                    break;
            }
        }

        public override uint MenuId
        {
            get { return PkgCmdIDList.ColumnCtxtMenu; }
        }

        public override uint IconIndex
        {
            get { return 8; }
        }

        public override bool Expandable
        {
            get { return false; }
        }

        public override void DoCommand(int commandId)
        {
            switch (commandId)
            {
                default:
                    base.DoCommand(commandId);
                    break;
            }
        }

    }
}
