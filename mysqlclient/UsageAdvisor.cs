using System;
using System.Diagnostics;

namespace MySql.Data.MySqlClient
{
	internal class UsageAdvisor
	{
		private MySqlConnection conn;

		public UsageAdvisor(MySqlConnection conn)
		{
			this.conn = conn;
		}

		public void ReadPartialResultSet(string cmdText)
		{
			if (! conn.Settings.UseUsageAdvisor) return;

			LogUAWarning(cmdText, "Not all rows in resultset were read.");
		}

		public void UsingNoIndex(string cmdText)
		{
			if (! conn.Settings.UseUsageAdvisor) return;

			LogUAWarning(cmdText, "Not using an index.");
		}

		public void UsingBadIndex(string cmdText) 
		{
			if (! conn.Settings.UseUsageAdvisor) return;

			LogUAWarning(cmdText, "Using a bad index.");
		}

		public void ReadPartialRowSet(string cmdText, bool[] uaFieldsUsed, MySqlField[] fields)
		{
			LogUAHeader(cmdText);
			Trace.WriteLine("Reason: Every column was not accessed.  Consider a more focused query.");
			Trace.Write("Fields not accessed: ");
			for (int i=0; i < uaFieldsUsed.Length; i++)
				if (! uaFieldsUsed[i])
					Trace.Write(" " + fields[i].ColumnName);
			Trace.WriteLine(" ");
			LogUAFooter();
		}

		private void LogUAWarning(string cmdText, string reason) 
		{
			LogUAHeader(cmdText);
			Trace.WriteLine("Reason: " + reason);
			LogUAFooter();
		}

		private void LogUAHeader(string cmdText) 
		{
			Trace.WriteLine("USAGE ADVISOR WARNING -------------");
			Trace.WriteLine("Host: " + conn.Settings.Server);
			Trace.WriteLine("Command Text:  " + cmdText);
		}

		private void LogUAFooter() 
		{
			Trace.WriteLine("-----------------------------------");
		}
	}
}
