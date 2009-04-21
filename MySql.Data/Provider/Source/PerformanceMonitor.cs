// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Diagnostics;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    internal class PerformanceMonitor
    {
        private MySqlConnection connection;
        private static PerformanceCounter procedureHardQueries;
        private static PerformanceCounter procedureSoftQueries;

        public PerformanceMonitor(MySqlConnection connection)
        {
            this.connection = connection;

            string categoryName = Resources.PerfMonCategoryName;

            if (connection.Settings.UsePerformanceMonitor && procedureHardQueries == null)
            {
                try
                {
                    procedureHardQueries = new PerformanceCounter(categoryName,
                                                                  "HardProcedureQueries", false);
                    procedureSoftQueries = new PerformanceCounter(categoryName,
                                                                  "SoftProcedureQueries", false);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void AddHardProcedureQuery()
        {
            if (!connection.Settings.UsePerformanceMonitor ||
                procedureHardQueries == null) return;
            procedureHardQueries.Increment();
        }

        public void AddSoftProcedureQuery()
        {
            if (!connection.Settings.UsePerformanceMonitor ||
                procedureSoftQueries == null) return;
            procedureSoftQueries.Increment();
        }
    }
}