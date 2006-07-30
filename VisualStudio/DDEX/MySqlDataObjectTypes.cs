namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents constant string values for all the supported data object
	/// types.  This list must be in sync with the data object support XML.
	/// </summary>
	internal static class MySqlDataObjectTypes
	{
		public const string Root = "";
		public const string User = "User";
		public const string Table = "Table";
		public const string Column = "Column";
		public const string Index = "Index";
		public const string IndexColumn = "IndexColumn";
		public const string ForeignKey = "ForeignKey";
		public const string ForeignKeyColumn = "ForeignKeyColumn";
		public const string View = "View";
		public const string ViewColumn = "ViewColumn";
		public const string StoredProcedure = "StoredProcedure";
		public const string StoredProcedureParameter = "StoredProcedureParameter";
		public const string StoredProcedureColumn = "StoredProcedureColumn";
		public const string Function = "Function";
		public const string FunctionParameter = "FunctionParameter";
		public const string FunctionColumn = "FunctionColumn";
	}
}
