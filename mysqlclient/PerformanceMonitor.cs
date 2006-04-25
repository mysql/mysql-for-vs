using System;
using System.Diagnostics;


namespace MySql.Data.MySqlClient
{
    class PerformanceMonitor
    {
        private MySqlConnection connection;
        private static PerformanceCounter procedureHardQueries;
        private static PerformanceCounter procedureSoftQueries;
        private static string categoryName;

        public PerformanceMonitor(MySqlConnection connection)
        {
            this.connection = connection;

            if (categoryName == null)
            {
                categoryName = ".NET Data Provider for MySQL";
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    if (PerformanceCounterCategory.CounterExists("HardProcedureQueries",
                        categoryName))
                        procedureHardQueries = new PerformanceCounter(categoryName,
                            "HardProcedureQueries", false);
                    if (PerformanceCounterCategory.CounterExists("SoftProcedureQueries",
                        categoryName))
                        procedureSoftQueries = new PerformanceCounter(categoryName,
                            "SoftProcedureQueries", false);
                }
            }
        }

        public void AddHardProcedureQuery()
        {
            if (!connection.Settings.UsePerformanceMonitor) return;
            procedureHardQueries.Increment();
        }

        public void AddSoftProcedureQuery()
        {
            if (!connection.Settings.UsePerformanceMonitor) return;
            procedureSoftQueries.Increment();
        }
    }
}
