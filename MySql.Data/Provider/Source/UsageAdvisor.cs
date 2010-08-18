// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
            if (!conn.Settings.UseUsageAdvisor) return;

            LogUAWarning(cmdText, "Not all rows in resultset were read.");
        }

        public void UsingNoIndex(string cmdText)
        {
            if (!conn.Settings.UseUsageAdvisor) return;

            LogUAWarning(cmdText, "Not using an index.");
        }

        public void UsingBadIndex(string cmdText)
        {
            if (!conn.Settings.UseUsageAdvisor) return;

            LogUAWarning(cmdText, "Using a bad index.");
        }

        public void AbortingSequentialAccess(MySqlField[] fields, int startIndex)
        {
            if (!conn.Settings.UseUsageAdvisor) return;
            LogUAHeader(null);
            Logger.WriteLine("");
            Logger.WriteLine("A rowset that was being accessed using SequentialAccess had to load " +
                             "all of its remaining columns.  This can cause performance problems.  This is most " +
                             "likely due to calling Prepare() on a command before reading all the columns of a " +
                             "rowset that is being accessed with SequentialAccess");
            LogUAFooter();
        }

		public void ReadPartialRowSet(string cmdText, bool[] uaFieldsUsed, MySqlField[] fields)
		{
            if (!conn.Settings.UseUsageAdvisor) return;

            LogUAHeader(cmdText);
			Logger.WriteLine("Reason: Every column was not accessed.  Consider a more focused query.");
			Logger.Write("Fields not accessed: ");
			for (int i = 0; i < uaFieldsUsed.Length; i++)
				if (!uaFieldsUsed[i])
					Logger.Write(" " + fields[i].ColumnName);
			Logger.WriteLine(" ");
			LogUAFooter();
		}

		public void Converting(string cmdText, string columnName,
									  string fromType, string toType)
		{
            if (!conn.Settings.UseUsageAdvisor) return;

            LogUAHeader(cmdText);
			Logger.WriteLine("Reason: Performing unnecessary conversion on field "
									  + columnName + ".");
			Logger.WriteLine("From: " + fromType + " to " + toType);
			LogUAFooter();
		}

        private void LogUAWarning(string cmdText, string reason)
        {
            LogUAHeader(cmdText);
            Logger.WriteLine("Reason: " + reason);
            LogUAFooter();
        }

        private void LogUAHeader(string cmdText)
        {
            Logger.WriteLine("USAGE ADVISOR WARNING -------------");
            Logger.WriteLine("Host: " + conn.Settings.Server);
            if (cmdText != null && cmdText.Length > 0)
                Logger.WriteLine("Command Text:  " + cmdText);
        }

        private static void LogUAFooter()
        {
            Logger.WriteLine("-----------------------------------");
        }
    }
}
