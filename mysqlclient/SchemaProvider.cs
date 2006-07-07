using System.Data;
using System;
using System.Data.Common;
using System.Resources;
using System.Reflection;
using System.IO;
using MySql.Data.Common;
using System.Text;
using System.Globalization;
using MySql.Data.Types;

namespace MySql.Data.MySqlClient
{
    internal class SchemaProvider
    {
        protected MySqlConnection connection;

        public SchemaProvider(MySqlConnection connectionToUse)
        {
            connection = connectionToUse;
        }

        public virtual DataTable GetSchema(string collection, String[] restrictions)
        {
            if (connection.State != ConnectionState.Open)
                throw new MySqlException("GetSchema can only be called on an open connection.");

            collection = collection.ToLower(System.Globalization.CultureInfo.CurrentCulture);

            DataTable dt = GetSchemaInternal(collection, restrictions);

            if (dt == null)
                throw new MySqlException("Invalid collection name");
            return dt;
        }

        public virtual DataTable GetDatabases(string[] restrictions)
        {
            string sql = "SHOW DATABASES";
            if (restrictions != null && restrictions.Length == 1)
                sql = sql + " LIKE '" + restrictions[0] + "'";
            MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.TableName = "Databases";
            return dt;
        }

        public virtual DataTable GetTables(string[] restrictions)
        {
            DataTable dt = new DataTable("Tables");
            dt.Columns.Add("TABLE_CATALOG", typeof(string));
            dt.Columns.Add("TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("TABLE_NAME", typeof(string));
            dt.Columns.Add("TABLE_TYPE", typeof(string));
            dt.Columns.Add("ENGINE", typeof(string));
            dt.Columns.Add("VERSION", typeof(long));
            dt.Columns.Add("ROW_FORMAT", typeof(string));
            dt.Columns.Add("TABLE_ROWS", typeof(long));
            dt.Columns.Add("AVG_ROW_LENGTH", typeof(long));
            dt.Columns.Add("DATA_LENGTH", typeof(long));
            dt.Columns.Add("MAX_DATA_LENGTH", typeof(long));
            dt.Columns.Add("INDEX_LENGTH", typeof(long));
            dt.Columns.Add("DATA_FREE", typeof(long));
            dt.Columns.Add("AUTO_INCREMENT", typeof(long));
            dt.Columns.Add("CREATE_TIME", typeof(DateTime));
            dt.Columns.Add("UPDATE_TIME", typeof(DateTime));
            dt.Columns.Add("CHECK_TIME", typeof(DateTime));
            dt.Columns.Add("TABLE_COLLATION", typeof(string));
            dt.Columns.Add("CHECKSUM", typeof(long));
            dt.Columns.Add("CREATE_OPTIONS", typeof(string));
            dt.Columns.Add("TABLE_COMMENT", typeof(string));

            DataTable databases = GetDatabases(restrictions);
            foreach (DataRow db in databases.Rows)
            {
                restrictions[1] = db["SCHEMA_NAME"].ToString();
                string table_type = restrictions[1].ToLower() == "information_schema" ?
                    "SYSTEM VIEW" : "BASE TABLE";
                DataTable tables = FindTables(restrictions, "NOT comment = 'View'");
                foreach (DataRow table in tables.Rows)
                {
                    DataRow row = dt.NewRow();
                    row["TABLE_CATALOG"] = null;
                    row["TABLE_SCHEMA"] = restrictions[1];
                    row["TABLE_NAME"] = table[0];
                    row["TABLE_TYPE"] = table_type;
                    row["ENGINE"] = table[1];
                    row["VERSION"] = table[2];
                    row["ROW_FORMAT"] = table[3];
                    row["TABLE_ROWS"] = table[4];
                    row["AVG_ROW_LENGTH"] = table[5];
                    row["DATA_LENGTH"] = table[6];
                    row["MAX_DATA_LENGTH"] = table[7];
                    row["INDEX_LENGTH"] = table[8];
                    row["DATA_FREE"] = table[9];
                    row["AUTO_INCREMENT"] = table[10];
                    row["CREATE_TIME"] = table[11];
                    row["UPDATE_TIME"] = table[12];
                    row["CHECK_TIME"] = table[13];
                    row["TABLE_COLLATION"] = table[14];
                    row["CHECKSUM"] = table[15];
                    row["CREATE_OPTIONS"] = table[16];
                    row["TABLE_COMMENT"] = table[17];
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public virtual DataTable GetColumns(string[] restrictions)
        {
            DataTable dt = new DataTable("Columns");
            dt.Columns.Add("TABLE_CATALOG", typeof(string));
            dt.Columns.Add("TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("TABLE_NAME", typeof(string));
            dt.Columns.Add("COLUMN_NAME", typeof(string));
            dt.Columns.Add("ORDINAL_POSITION", typeof(long));
            dt.Columns.Add("COLUMN_DEFAULT", typeof(string));
            dt.Columns.Add("IS_NULLABLE", typeof(string));
            dt.Columns.Add("DATA_TYPE", typeof(string));
            dt.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(long));
            dt.Columns.Add("CHARACTER_OCTET_LENGTH", typeof(long));
            dt.Columns.Add("NUMERIC_PRECISION", typeof(long));
            dt.Columns.Add("NUMERIC_SCALE", typeof(long));
            dt.Columns.Add("CHARACTER_SET_NAME", typeof(string));
            dt.Columns.Add("COLLATION_NAME", typeof(string));
            dt.Columns.Add("COLUMN_TYPE", typeof(string));
            dt.Columns.Add("COLUMN_KEY", typeof(string));
            dt.Columns.Add("EXTRA", typeof(string));
            dt.Columns.Add("PRIVILEGES", typeof(string));
            dt.Columns.Add("COLUMN_COMMENT", typeof(string));

            // we don't allow restricting on table type here
            string columnName = null;
            if (restrictions != null && restrictions.Length == 4)
            {
                columnName = restrictions[3];
                restrictions[3] = null;
            }
            DataTable tables = GetTables(restrictions);

            foreach (DataRow row in tables.Rows)
                LoadTableColumns(dt, row["TABLE_SCHEMA"].ToString(), 
                    row["TABLE_NAME"].ToString(), columnName);

            return dt;
        }

        private void LoadTableColumns(DataTable dt, string schema,
            string tableName, string columnRestriction)
        {
            string sql = String.Format("SHOW FULL COLUMNS FROM {0}.{1}",
                schema, tableName);
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader reader = null;
            try
            {
                int pos = 0;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string colName = reader.GetString(0);
                    if (columnRestriction != null && colName != columnRestriction)
                        continue;
                    DataRow row = dt.NewRow();
                    row["TABLE_CATALOG"] = null;
                    row["TABLE_SCHEMA"] = schema;
                    row["TABLE_NAME"] = tableName;
                    row["COLUMN_NAME"] = colName;
                    row["ORDINAL_POSITION"] = pos++;
                    row["COLUMN_DEFAULT"] = reader.GetString(5);
                    row["IS_NULLABLE"] = reader.GetString(3);
                    row["DATA_TYPE"] = reader.GetString(1);
                    row["CHARACTER_MAXIMUM_LENGTH"] = null;
                    row["NUMERIC_PRECISION"] = null;
                    row["NUMERIC_SCALE"] = null;
                    row["CHARACTER_SET_NAME"] = reader.GetString(2);
                    row["COLLATION_NAME"] = row["CHARACTER_SET_NAME"];
                    row["COLUMN_TYPE"] = reader.GetString(1);
                    row["COLUMN_KEY"] = reader.GetString(4);
                    row["EXTRA"] = reader.GetString(6);
                    row["PRIVILEGES"] = reader.GetString(7);
                    row["COLUMN_COMMENT"] = reader.GetString(8);
                    ParseColumnRow(row);
                    dt.Rows.Add(row);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        private void ParseColumnRow(DataRow row)
        {
            // first parse the character set name
            string charset = row["CHARACTER_SET_NAME"].ToString();
            int index = charset.IndexOf('_');
            if (index != -1)
                row["CHARACTER_SET_NAME"] = charset.Substring(0, index);

            // now parse the data type
            string dataType = row["DATA_TYPE"].ToString();
            index = dataType.IndexOf('(');
            if (index == -1)
                return;
            row["DATA_TYPE"] = dataType.Substring(0, index);
            int stop = dataType.IndexOf(')', index);
            string dataLen = dataType.Substring(index + 1, stop - (index + 1));
            string lowerType = row["DATA_TYPE"].ToString().ToLower();
            if (lowerType == "char" || lowerType == "varchar")
                row["CHARACTER_SET_MAXIMUM"] = dataLen;
            else
            {
                string[] lenparts = dataLen.Split(new char[] { ',' });
                row["NUMERIC_PRECISION"] = lenparts[0];
                if (lenparts.Length == 2)
                    row["NUMERIC_SCALE"] = lenparts[1];
            }
        }

        public virtual DataTable GetIndexes(string[] restrictions)
        {
            DataTable dt = new DataTable("Indexes");
            dt.Columns.Add("INDEX_CATALOG", typeof(string));
            dt.Columns.Add("INDEX_SCHEMA", typeof(string));
            dt.Columns.Add("INDEX_NAME", typeof(string));
            dt.Columns.Add("TABLE_NAME", typeof(string));
            dt.Columns.Add("UNIQUE", typeof(bool));
            dt.Columns.Add("PRIMARY", typeof(bool));

            return dt;
        }

        public virtual DataTable GetIndexColumns(string[] restrictions)
        {
            DataTable dt = new DataTable("IndexColumns");
            dt.Columns.Add("INDEX_CATALOG", typeof(string));
            dt.Columns.Add("INDEX_SCHEMA", typeof(string));
            dt.Columns.Add("INDEX_NAME", typeof(string));
            dt.Columns.Add("TABLE_NAME", typeof(string));
            dt.Columns.Add("COLUMN_NAME", typeof(string));
            dt.Columns.Add("ORDINAL_POSITION", typeof(int));

            return dt;
        }

        public virtual DataTable GetForeignKeys(string[] restrictions)
        {
            DataTable dt = new DataTable("Foreign Keys");
            dt.Columns.Add("CONSTRAINT_CATALOG", typeof(string));
            dt.Columns.Add("CONSTRAINT_SCHEMA", typeof(string));
            dt.Columns.Add("CONSTRAINT_NAME", typeof(string));
            dt.Columns.Add("TABLE_CATALOG", typeof(string));
            dt.Columns.Add("TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("TABLE_NAME", typeof(string));
            dt.Columns.Add("COLUMN_NAME", typeof(string));
            dt.Columns.Add("ORDINAL_POSITION", typeof(int));
            dt.Columns.Add("REFERENCED_TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("REFERENCED_TABLE_NAME", typeof(string));
            dt.Columns.Add("REFERENCED_COLUMN_NAME", typeof(string));

            return dt;
        }

        public virtual DataTable GetUsers(string[] restrictions)
        {
            StringBuilder sb = new StringBuilder("SELECT Host, User FROM mysql.user");
            if (restrictions != null && restrictions.Length > 0)
                sb.AppendFormat(" WHERE User LIKE '{0}'", restrictions[0]);

            MySqlDataAdapter da = new MySqlDataAdapter(sb.ToString(), connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.TableName = "Users";
            dt.Columns[0].ColumnName = "HOST";
            dt.Columns[1].ColumnName = "USERNAME";

            return dt;
        }

        protected virtual DataTable GetCollections()
        {
            object[,] collections = new object[,] {
                {"MetaDataCollections", 0, 0},
                {"DataSourceInformation", 0, 0},
                {"DataTypes", 0, 0},
                {"Restrictions", 0, 0},
                {"ReservedWords", 0, 0},
                {"Databases", 1, 1},
                {"Tables", 4, 2},
                {"Columns", 4, 4}, 
                {"Users", 1, 1},
                {"Foreign Keys", 4, 3},
                {"IndexColumns", 5, 4},
                {"Indexes", 4, 3}
            };

            DataTable dt = new DataTable("MetaDataCollections");
            dt.Columns.Add(new DataColumn("CollectionName", typeof(string)));
            dt.Columns.Add(new DataColumn("NumberOfRestriction", typeof(int)));
            dt.Columns.Add(new DataColumn("NumberOfIdentifierParts", typeof(int)));

            FillTable(dt, collections);

            return dt;
        }

        private DataTable GetDataSourceInformation()
        {
            DataTable dt = new DataTable("DataSourceInformation");
            dt.Columns.Add("CompositeIdentifierSeparatorPattern", typeof(string));
            dt.Columns.Add("DataSourceProductName", typeof(string));
            dt.Columns.Add("DataSourceProductVersion", typeof(string));
            dt.Columns.Add("DataSourceProductVersionNormalized", typeof(string));
            dt.Columns.Add("GroupByBehavior", typeof(GroupByBehavior));
            dt.Columns.Add("IdentifierPattern", typeof(string));
            dt.Columns.Add("IdentifierCase", typeof(IdentifierCase));
            dt.Columns.Add("OrderByColumnsInSelect", typeof(bool));
            dt.Columns.Add("ParameterMarkerFormat", typeof(string));
            dt.Columns.Add("ParameterMarkerPattern", typeof(string));
            dt.Columns.Add("ParameterNameMaxLength", typeof(int));
            dt.Columns.Add("ParameterNamePattern", typeof(string));
            dt.Columns.Add("QuotedIdentifierPattern", typeof(string));
            dt.Columns.Add("QuotedIdentifierCase", typeof(IdentifierCase));
            dt.Columns.Add("StatementSeparatorPattern", typeof(string));
            dt.Columns.Add("StringLiteralPattern", typeof(string));
            dt.Columns.Add("SupportedJoinOperators", typeof(SupportedJoinOperators));

            DBVersion v = connection.driver.Version;
            string ver = String.Format("{0:0}.{1:0}.{2:0}",
                v.Major, v.Major, v.Build);

            DataRow row = dt.NewRow();
            row["CompositeIdentifierSeparatorPattern"] = "\\.";
            row["DataSourceProductName"] = "MySQL";
            row["DataSourceProductVersion"] = connection.ServerVersion;
            row["DataSourceProductVersionNormalized"] = ver;
            row["GroupByBehavior"] = GroupByBehavior.Unknown;
            row["IdentifierPattern"] = @"(^\[\p{Lo}\p{Lu}\p{Ll}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Nd}@$#_]*$)|(^\[[^\]\0]|\]\]+\]$)|(^\""[^\""\0]|\""\""+\""$)";
            row["IdentifierCase"] = IdentifierCase.Insensitive;
            row["OrderByColumnsInSelect"] = false;
            row["ParameterMarkerFormat"] = "{0}";
            row["ParameterMarkerPattern"] = "@([A-Za-z0-9_$#]*)";
            row["ParameterNameMaxLength"] = 128;
            row["ParameterNamePattern"] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            row["QuotedIdentifierPattern"] = @"(([^\[]|\]\])*)";
            row["QuotedIdentifierCase"] = IdentifierCase.Insensitive;
            row["StatementSeparatorPattern"] = ";";
            row["StringLiteralPattern"] = "'(([^']|'')*)'";
            row["SupportedJoinOperators"] = 15;
            dt.Rows.Add(row);

            return dt;
        }

        private DataTable GetDataTypes()
        {
            DataTable dt = new DataTable("DataTypes");
            dt.Columns.Add(new DataColumn("TypeName", typeof(string)));
            dt.Columns.Add(new DataColumn("ProviderDbType", typeof(int)));
            dt.Columns.Add(new DataColumn("ColumnSize", typeof(long)));
            dt.Columns.Add(new DataColumn("CreateFormat", typeof(string)));
            dt.Columns.Add(new DataColumn("CreateParameters", typeof(string)));
            dt.Columns.Add(new DataColumn("DataType", typeof(string)));
            dt.Columns.Add(new DataColumn("IsAutoincrementable", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsBestMatch", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsCaseSensitive", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsFixedLength", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsFixedPrecisionScale", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsLong", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsNullable", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsSearchable", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsSearchableWithLike", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsUnsigned", typeof(bool)));
            dt.Columns.Add(new DataColumn("MaximumScale", typeof(short)));
            dt.Columns.Add(new DataColumn("MinimumScale", typeof(short)));
            dt.Columns.Add(new DataColumn("IsConcurrencyType", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsLiteralsSupported", typeof(bool)));
            dt.Columns.Add(new DataColumn("LiteralPrefix", typeof(string)));
            dt.Columns.Add(new DataColumn("LitteralSuffix", typeof(string)));
            dt.Columns.Add(new DataColumn("NativeDataType", typeof(string)));

            // have each one of the types contribute to the datatypes collection
            MySqlBit.SetDSInfo(dt);
            MySqlBinary.SetDSInfo(dt);
            MySqlDateTime.SetDSInfo(dt);
            MySqlTimeSpan.SetDSInfo(dt);
            MySqlString.SetDSInfo(dt);
            MySqlDouble.SetDSInfo(dt);
            MySqlSingle.SetDSInfo(dt);
            MySqlByte.SetDSInfo(dt);
            MySqlInt16.SetDSInfo(dt);
            MySqlInt32.SetDSInfo(dt);
            MySqlInt64.SetDSInfo(dt);
            MySqlDecimal.SetDSInfo(dt);
            MySqlUByte.SetDSInfo(dt);
            MySqlUInt16.SetDSInfo(dt);
            MySqlUInt32.SetDSInfo(dt);
            MySqlUInt64.SetDSInfo(dt);

            return dt;
        }

        protected virtual DataTable GetRestrictions()
        {
            object[,] restrictions = new object[,] 
            {
                {"Users", "Name", "", 0},
                {"Databases", "Name", "", 0},
                {"Tables", "Catalog", "", 0},
                {"Tables", "Owner", "", 1},
                {"Tables", "Table", "", 2},
                {"Tables", "TableType", "", 3},
                {"Columns", "Catalog", "", 0},
                {"Columns", "Owner", "", 1},
                {"Columns", "Table", "", 2},
                {"Columns", "Column", "", 3},          

                {"IndexColumns", "Catalog", "", "db_name()"},
                {"IndexColumns", "Owner", "", "user_name()"},
                {"IndexColumns", "Table", "", "o.name"},
                {"IndexColumns", "ConstraintName", "", "x.name"},
                {"IndexColumns", "Column", "", "c.name"},
                {"Indexes", "Catalog", "", "db_name()"},
                {"Indexes", "Owner", "", "user_name()"},
                {"Indexes", "Table", "", "o.name"},
                {"Indexes", "Name", "", "x.name"},
                {"ForeignKeys", "Catalog", "", "CONSTRAINT_CATALOG"},
                {"ForeignKeys", "Owner", "", "CONSTRAINT_SCHEMA"},
                {"ForeignKeys", "Table", "", "TABLE_NAME"},
                {"ForeignKeys", "Name", "", "CONSTAINT_NAME"},
            };

            DataTable dt = new DataTable("Restrictions");
            dt.Columns.Add(new DataColumn("CollectionName", typeof(string)));
            dt.Columns.Add(new DataColumn("RestrictionName", typeof(string)));
            dt.Columns.Add(new DataColumn("RestrictionDefault", typeof(string)));
            dt.Columns.Add(new DataColumn("RestrictionNumber", typeof(int)));

            FillTable(dt, restrictions);

            return dt;
        }

        private DataTable GetReservedWords()
        {
            DataTable dt = new DataTable("ReservedWords");
            dt.Columns.Add(new DataColumn("Reserved Word", typeof(string)));

            Stream str = Assembly.GetCallingAssembly().GetManifestResourceStream(
                "MySql.Data.MySqlClient.ReservedWords.txt");
            StreamReader sr = new StreamReader(str);
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] keywords = line.Split(new char[] { ' ' });
                foreach (string s in keywords) 
                {
                    DataRow row = dt.NewRow();
                    row[0] = s;
                    dt.Rows.Add(row);
                }
                line = sr.ReadLine();
            }
            sr.Close();
            str.Close();

            return dt;
        }

        protected void FillTable(DataTable dt, object[,] data)
        {
            foreach (object[] dataItem in data)
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < dataItem.Length; i++)
                    row[i] = dataItem[i];
            }
        }

        private DataTable FindTables(string[] restrictions, string whereSQL)
        {
            string[] dbres = new string[1];
            if (restrictions != null && restrictions.Length >= 2)
                dbres[0] = restrictions[1];
            DataTable databases = GetDatabases(dbres);

            DataTable tables = new DataTable();
            foreach (DataRow db in databases.Rows)
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder where = new StringBuilder();
                sql.AppendFormat("SHOW TABLE STATUS FROM '{0}'", db["SCHEMA_NAME"]);
                if (restrictions != null && restrictions.Length >= 3 &&
                    restrictions[2] != null)
                    where.AppendFormat(" WHERE NAME='{0}'", restrictions[2]);
                if (whereSQL != null)
                {
                    if (where.Length > 0)
                        where.AppendFormat(" AND {0}", whereSQL);
                    else
                        where.AppendFormat(" WHERE {0}", whereSQL);
                }
                sql.Append(where.ToString());
                MySqlDataAdapter da = new MySqlDataAdapter(sql.ToString(), connection);
                da.Fill(tables);
            }
            return tables;
        }

        protected virtual DataTable GetSchemaInternal(string collection, string[] restrictions)
        {
            switch (collection)
            {
                // common collections
                case "metadatacollections":
                    return GetCollections();
                case "datasourceinformation":
                    return GetDataSourceInformation();
                case "datatypes":
                    return GetDataTypes();
                case "restrictions":
                    return GetRestrictions();
                case "reservedwords":
                    return GetReservedWords();

                // collections specific to our provider
                case "users":
                    return GetUsers(restrictions);
                case "databases":
                    return GetDatabases(restrictions);
                case "tables":
                    return GetTables(restrictions);
                case "columns":
                    return GetColumns(restrictions);
                case "indexes":
                    return GetIndexes(restrictions);
                case "indexcolumns":
                    return GetIndexColumns(restrictions);
                case "foreign keys":
                    return GetForeignKeys(restrictions);
            }
            return null;
        }


    }
}
