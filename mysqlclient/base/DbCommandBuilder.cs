using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
	public abstract class DbCommandBuilder : Component
	{
		protected DbCommandBuilder() 
		{
		}

		protected abstract string GetParameterName(int ordinal);
		protected abstract string GetParameterName(string parameterName);
		protected abstract string GetParameterPlaceholder(int ordinal);
		protected abstract void ApplyParameterInfo(DbParameter parameter, DataRow row, 
			StatementType statementType, bool whereClause);
		protected abstract void SetRowUpdatingHandler(DbDataAdapter adapter);
	}
}
