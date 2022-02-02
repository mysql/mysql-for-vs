// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Provides static methods to leverage miscelaneous tasks.
  /// </summary>
  public static class Utilities
  {
    #region Constants

    /// <summary>
    /// The text used on default collations for a specific character set.
    /// </summary>
    public const string DEFAULT_COLLATION_TEXT = "default collation";

    /// <summary>
    /// The registry key name to check applications that run at startup.
    /// </summary>
    private const string RUN_AT_STARTUP_REGISTRY_KEY_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    #endregion Constants

    /// <summary>
    /// Creates a scheduled task for the given task to run every week with the given frequency.
    /// </summary>
    /// <param name="taskName">The name the scheduled task is created with.</param>
    /// <param name="taskToRun">The pathname of the task to run.</param>
    /// <param name="parameters">The parameters passed to the task to run.</param>
    /// <param name="weeklyFrecuency">The number of times the task runs within a week.</param>
    /// <returns><c>true</c> if the scheduled task is created successfully, <c>false</c> otherwise.</returns>
    public static bool CreateScheduledTask(string taskName, string taskToRun, string parameters, int weeklyFrecuency)
    {
      DeleteScheduledTask(taskName);
      var commandLineParams = new StringBuilder();
      commandLineParams.AppendFormat(
        GetOsVersion() != OsVersion.WindowsXp
          ? "/Create /sc weekly /mo {0} /st {1} /tn {2} /tr \"\\\"{3}\\\" {4}"
          : "/Create /sc weekly /mo {0} /st {1} /tn {2} /ru \"NT Authority\\System\" /rp \"\" /tr \"\\\"{3}\\\" {4} ",
        weeklyFrecuency, DateTime.Now.ToString("HH:mm:ss"), taskName, @taskToRun, parameters);

      var startInfo = new ProcessStartInfo
      {
        UseShellExecute = false,
        FileName = "schtasks.exe",
        CreateNoWindow = true,
        Arguments = commandLineParams.ToString(),
        RedirectStandardError = true
      };

      string error;
      using (var p = new Process())
      {
        p.StartInfo = startInfo;
        p.Start();
        error = p.StandardError.ReadToEnd();
        p.WaitForExit();
      }

      return string.IsNullOrEmpty(error);
    }

    /// <summary>
    /// Helper method to recursively remove a folder with some additional handling not provided by <see cref="Directory.Delete(string)"/>.
    /// </summary>
    /// <param name="target">The folder to remove (including all its content).</param>
    /// <returns><c>true</c> if removal of directory and all its contents was successful, <c>false</c> otherwise.</returns>
    public static bool DeleteDirectory(string target)
    {
      if (string.IsNullOrEmpty(target)
          || !Directory.Exists(target))
      {
        return false;
      }

      bool success = true;
      string[] files = Directory.GetFiles(target);
      string[] dirs = Directory.GetDirectories(target);

      foreach (string file in files)
      {
        // In the case a file is set to read-only we would get an IO exception.
        try
        {
          File.SetAttributes(file, FileAttributes.Normal);
          File.Delete(file);
        }
        catch
        {
          // Cannot delete files in use.  Continue.
          success = false;
        }
      }

      success = dirs.Aggregate(success, (current, dir) => current && DeleteDirectory(dir));
      File.SetAttributes(target, FileAttributes.Normal);
      try
      {
        Directory.Delete(target, false);
      }
      catch (DirectoryNotFoundException)
      {
        // Ignore. Folder might be removed outside of the application already.
      }
      catch (IOException)
      {
        // Usually thrown if the folder is blocked.
        if (Directory.GetCurrentDirectory().Equals(target, StringComparison.InvariantCultureIgnoreCase))
        {
          Directory.SetCurrentDirectory("..");
        }

        Thread.Sleep(0);
        try
        {
          Directory.Delete(target, false);
        }
        catch (Exception)
        {
          // The directory is locked by the OS and can't be removed.  Likely this happens when the user has
          // the directory open in explorer.
          success = false;
        }
      }

      return success;
    }

    /// <summary>
    /// Attempts to delete the directory in the given path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="retryCount">The number of times the operation must be retried if an error occurs.</param>
    /// <param name="retryWaitInMilliseconds">The time to wait between retries.</param>
    /// <param name="successWaitInMilliseconds">The time to wait if the deletion is successful (in case a creation of the same folder follows the deletion).</param>
    /// <returns><c>true</c> if the directory is deleted successfully, <c>false</c> otherwise.</returns>
    public static bool DeleteDirectory(string directoryPath, int retryCount, int retryWaitInMilliseconds = 100, int successWaitInMilliseconds = 0)
    {
      return DeleteDirectory(directoryPath, retryCount, retryWaitInMilliseconds, successWaitInMilliseconds, out _);
    }

    /// <summary>
    /// Attempts to delete the directory in the given path.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="retryCount">The number of times the operation must be retried if an error occurs.</param>
    /// <param name="retryWaitInMilliseconds">The time to wait between retries.</param>
    /// <param name="successWaitInMilliseconds">The time to wait if the deletion is successful (in case a creation of the same folder follows the deletion).</param>
    /// <param name="errorMessage">An error message if an error occurs.</param>
    /// <returns><c>true</c> if the directory is deleted successfully, <c>false</c> otherwise.</returns>
    public static bool DeleteDirectory(string directoryPath, int retryCount, int retryWaitInMilliseconds, int successWaitInMilliseconds, out string errorMessage)
    {
      errorMessage = null;
      if (string.IsNullOrEmpty(directoryPath))
      {
        // Nothing to delete+
        return true;
      }

      var currentRetry = 0;
      while (Directory.Exists(directoryPath)
             && currentRetry < retryCount)
      {
        try
        {
          Directory.Delete(directoryPath, true);
          errorMessage = null;
          if (successWaitInMilliseconds > 0)
          {
            Thread.Sleep(successWaitInMilliseconds);
          }

          break;
        }
        catch (Exception ex)
        {
          errorMessage = ex.Message;
          currentRetry++;
          if (retryWaitInMilliseconds > 0)
          {
            Thread.Sleep(retryWaitInMilliseconds);
          }
        }
      }

      return string.IsNullOrEmpty(errorMessage);
    }

    /// <summary>
    /// Attempts to delete the file in the given path.
    /// </summary>
    /// <param name="fullPath">The directory path.</param>
    /// <param name="retryCount">The number of times the operation must be retried if an error occurs.</param>
    /// <param name="retryWaitInMilliseconds">The time to wait between retries.</param>
    /// <param name="successWaitInMilliseconds">The time to wait if the deletion is successful (in case a creation of the same folder follows the deletion).</param>
    /// <returns><c>true</c> if the file is deleted successfully, <c>false</c> otherwise.</returns>
    public static bool DeleteFile(string fullPath, int retryCount = 3, int retryWaitInMilliseconds = 100, int successWaitInMilliseconds = 0)
    {
      return DeleteFile(fullPath, retryCount, retryWaitInMilliseconds, successWaitInMilliseconds, out _);
    }

    /// <summary>
    /// Attempts to delete the file in the given path.
    /// </summary>
    /// <param name="fullPath">The directory path.</param>
    /// <param name="retryCount">The number of times the operation must be retried if an error occurs.</param>
    /// <param name="retryWaitInMilliseconds">The time to wait between retries.</param>
    /// <param name="successWaitInMilliseconds">The time to wait if the deletion is successful (in case a creation of the same folder follows the deletion).</param>
    /// <param name="errorMessage">An error message if an error occurs.</param>
    /// <returns><c>true</c> if the file is deleted successfully, <c>false</c> otherwise.</returns>
    public static bool DeleteFile(string fullPath, int retryCount, int retryWaitInMilliseconds, int successWaitInMilliseconds, out string errorMessage)
    {
      errorMessage = null;
      if (string.IsNullOrEmpty(fullPath))
      {
        // Nothing to delete
        return true;
      }

      var currentRetry = 0;
      while (File.Exists(fullPath)
             && currentRetry < retryCount)
      {
        try
        {
          File.Delete(fullPath);
          errorMessage = null;
          if (successWaitInMilliseconds > 0)
          {
            Thread.Sleep(successWaitInMilliseconds);
          }

          break;
        }
        catch (Exception ex)
        {
          errorMessage = ex.Message;
          currentRetry++;
          if (retryWaitInMilliseconds > 0)
          {
            Thread.Sleep(retryWaitInMilliseconds);
          }
        }
      }

      return string.IsNullOrEmpty(errorMessage);
    }

    /// <summary>
    /// Deletes a scheduled task with the given name.
    /// </summary>
    /// <param name="taskName">The name the scheduled task was created with.</param>
    /// <returns><c>true</c> if the scheduled task is deleted successfully, <c>false</c> otherwise.</returns>
    public static bool DeleteScheduledTask(string taskName)
    {
      string arguments = @"/delete /tn " + taskName + " /f";

      var startInfo = new ProcessStartInfo
      {
        UseShellExecute = false,
        FileName = "schtasks",
        CreateNoWindow = true,
        Arguments = arguments,
        RedirectStandardError = true
      };

      string error;
      using (var p = new Process())
      {
        p.StartInfo = startInfo;
        p.Start();
        error = p.StandardError.ReadToEnd();
        p.WaitForExit();
      }

      return string.IsNullOrEmpty(error) || error.Contains("ERROR: The system cannot find the file specified.");
    }

    /// <summary>
    /// Executes the given query, establishing a connection using the given connection string and returning a <see cref="DataRow"/>.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="sqlQuery">The SQL query to execute.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    /// <returns>A <see cref="DataRow"/> if the query is successful, or <c>null</c> otherwise.</returns>
    public static DataRow ExecuteDataRow(string connectionString, string sqlQuery, bool showErrors = true)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(sqlQuery))
      {
        return null;
      }

      DataRow dataRow = null;
      try
      {
        dataRow = MySqlHelper.ExecuteDataRow(connectionString, sqlQuery);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, showErrors);
      }

      return dataRow;
    }

    /// <summary>
    /// Executes the given query, establishing a connection using the given connection string.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="sqlQuery">The SQL query to execute.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    public static void ExecuteNonQuery(string connectionString, string sqlQuery, bool showErrors = true)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(sqlQuery))
      {
        return;
      }

      try
      {
        MySqlHelper.ExecuteNonQuery(connectionString, sqlQuery);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, showErrors);
      }
    }

    /// <summary>
    /// Executes the given query, establishing a connection using the given connection string and returning a single boxed value.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="sqlQuery">The SQL query to execute.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    /// <returns>A single boxed value if the query is successful, or <c>null</c> otherwise.</returns>
    public static object ExecuteScalar(string connectionString, string sqlQuery, bool showErrors = true)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(sqlQuery))
      {
        return null;
      }

      object retValue = null;
      try
      {
        retValue = MySqlHelper.ExecuteScalar(connectionString, sqlQuery);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, showErrors);
      }

      return retValue;
    }

    /// <summary>
    /// Gets the active <see cref="Form"/> attached to the calling thread's message queue.
    /// </summary>
    /// <returns>The active <see cref="Form"/> attached to the calling thread's message queue.</returns>
    public static Form GetActiveForm()
    {
      var activeForm = Form.ActiveForm;
      if (activeForm != null)
      {
        return activeForm;
      }

      activeForm = Application.OpenForms.Count > 0
        ? Application.OpenForms[0]
        : null;
      if (activeForm != null)
      {
        return activeForm;
      }

      var handle = GetActiveWindow();
      if (handle == IntPtr.Zero)
      {
        handle = GetForegroundWindow();
      }

      var control = Control.FromHandle(handle);
      return control as Form;
    }

    /// <summary>
    /// Gets a list of all MySQL character sets along with their available collations.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="firstElement">A custom string for the first element of the dictioary.</param>
    /// <returns>A list of all MySQL character sets along with their available collations.</returns>
    public static Dictionary<string, string[]> GetCollationsDictionary(string connectionString,
      string firstElement = null)
    {
      var charSetsTable = GetSchemaInformation(connectionString, SchemaInformationType.Collations, true);
      if (charSetsTable == null)
      {
        return null;
      }

      var rowsByGroup = charSetsTable.Select(string.Empty, "Charset, Collation").GroupBy(r => r["Charset"].ToString());
      var collationsDictionary = new Dictionary<string, string[]>(270);
      if (!string.IsNullOrEmpty(firstElement))
      {
        collationsDictionary.Add(firstElement, new[] {string.Empty, string.Empty});
      }

      foreach (var rowsGroup in rowsByGroup)
      {
        var charset = rowsGroup.Key;
        collationsDictionary.Add($"{charset} - {DEFAULT_COLLATION_TEXT}",
          new[] {charset, string.Empty});
        foreach (var collation in rowsGroup.Select(row => row["Collation"].ToString()))
        {
          collationsDictionary.Add($"{charset} - {collation}", new[] {charset, collation});
        }
      }

      return collationsDictionary;
    }

    /// <summary>
    /// Gets the default collation corresponding to the given character set name.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="charSet">A character set name.</param>
    /// <returns>Tthe default collation corresponding to the given character set name.</returns>
    public static string GetDefaultCollationFromCharSet(string connectionString, string charSet)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(charSet))
      {
        return null;
      }

      var infoTable = GetSchemaInformation(connectionString, SchemaInformationType.CharacterSets, true);
      var charSetRows = infoTable?.Select($"Charset = '{charSet}'");
      return charSetRows?.Length > 0 ? charSetRows[0]["Default collation"].ToString() : null;
    }

    /// <summary>
    /// Gets the IP address corresponding to the given host name.
    /// </summary>
    /// <param name="hostName">A qualified host name.</param>
    /// <returns>The IP address corresponding to the given host name.</returns>
    public static string GetIPv4ForHostName(string hostName)
    {
      string host = hostName == "." ? "localhost" : hostName;
      try
      {
        IPHostEntry ip = Dns.GetHostEntry(host);
        foreach (var ipAddress in ip.AddressList.Where(addr => addr.AddressFamily == AddressFamily.InterNetwork))
        {
          return ipAddress.ToString();
        }
      }
      catch (SocketException)
      {
        return null;
      }

      return null;
    }

    /// <summary>
    /// Gets the full path where a MySQL related application with the given name is installed.
    /// </summary>
    /// <param name="applicationName">The name of a MySQL related application.</param>
    /// <returns>The full path where a MySQL related application with the given name is installed.</returns>
    public static string GetMySqlAppInstallLocation(string applicationName)
    {
      string location = RegistryHive.CurrentUser.GetMySqlAppInstallLocation(applicationName);
      if (string.IsNullOrEmpty(location))
      {
        location = RegistryHive.LocalMachine.GetMySqlAppInstallLocation(applicationName);
      }

      return location;
    }

    /// <summary>
    /// Gets the full path where a MySQL related application with the given name is installed.
    /// </summary>
    /// <param name="registryHive">The <see cref="RegistryHive"/> where the sub key is retrieved from.</param>
    /// <param name="applicationName">The name of a MySQL related application.</param>
    /// <returns>The full path where a MySQL related application with the given name is installed.</returns>
    public static string GetMySqlAppInstallLocation(this RegistryHive registryHive, string applicationName)
    {
      string location;
      if (Environment.Is64BitOperatingSystem)
      {
        location = registryHive.GetMySqlAppInstallLocation(RegistryView.Registry64, applicationName);
        if (!string.IsNullOrEmpty(location))
        {
          return location;
        }

        location = registryHive.GetMySqlAppInstallLocation(RegistryView.Registry32, applicationName);
        if (!string.IsNullOrEmpty(location))
        {
          return location;
        }
      }
      else
      {
        location = registryHive.GetMySqlAppInstallLocation(RegistryView.Default, applicationName);
        if (!string.IsNullOrEmpty(location))
        {
          return location;
        }
      }

      return location;
    }

    /// <summary>
    /// Gets the character set and its collation used by the connected MySQL server.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns>The character set and its collation used by the connected MySQL server.</returns>
    public static Tuple<string, string> GetMySqlServerCharSetAndCollation(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return null;
      }

      const string SQL = "SELECT @@character_set_server, @@collation_server";
      var dataRow = ExecuteDataRow(connectionString, SQL);
      return dataRow == null || dataRow.ItemArray.Length < 2
        ? null
        : new Tuple<string, string>(dataRow.ItemArray[0].ToString(), dataRow.ItemArray[1].ToString());
    }

    /// <summary>
    /// Gets the value of the DEFAULT_STORAGE_ENGINE MySQL Server variable indicating the default DB engine used for new table creations.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns>The default DB engine used for new table creations.</returns>
    public static string GetMySqlServerDefaultEngine(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return string.Empty;
      }

      const string SQL = "SELECT @@default_storage_engine";
      object objEngine = ExecuteScalar(connectionString, SQL);
      return objEngine?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the value of the global SQL_MODE MySQL Server variable.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns>The value of the global SQL_MODE system variable.</returns>
    public static string GetMySqlServerGlobalMode(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return string.Empty;
      }

      const string SQL = "SELECT @@GLOBAL.sql_mode";
      var objSqlMode = ExecuteScalar(connectionString, SQL);
      return objSqlMode?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the value of the LOWER_CASE_TABLE_NAMES MySQL Server variable indicating the case sensitivity that table names are stored and compared.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns><c>true</c> if table names are stored in lowercase on disk and comparisons are not case sensitive, <c>false</c> if table names are stored as specified and comparisons are case sensitive.</returns>
    public static bool GetMySqlServerLowerCaseTableNames(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return false;
      }

      const string SQL = "SELECT @@lower_case_table_names";
      object objCaseSensitivity = ExecuteScalar(connectionString, SQL);
      return objCaseSensitivity != null && objCaseSensitivity.ToString().Equals("1");
    }

    /// <summary>
    /// Gets the value of the MAX_ALLOWED_PACKET MySQL Server variable indicating the max size in bytes of the packet returned by a single query.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns>The max size in bytes of the packet returned by a single query.</returns>
    public static int GetMySqlServerMaxAllowedPacket(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return 0;
      }

      const string SQL = "SELECT @@max_allowed_packet";
      object objCount = MySqlHelper.ExecuteScalar(connectionString, SQL);
      return objCount != null ? Convert.ToInt32(objCount) : 0;
    }

    /// <summary>
    /// Gets the version of the connected MySQL server.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <returns>The version of the connected MySQL server.</returns>
    public static string GetMySqlServerVersion(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return string.Empty;
      }

      const string SQL = "SELECT @@version";
      object version = MySqlHelper.ExecuteScalar(connectionString, SQL);
      return version?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the version of Windows istalled in the computer.
    /// </summary>
    /// <returns>The version of Windows istalled in the computer.</returns>
    public static OsVersion GetOsVersion()
    {
      OperatingSystem os = Environment.OSVersion;
      Version vs = os.Version;
      if (os.Platform != PlatformID.Win32NT)
      {
        return OsVersion.WindowsXp;
      }

      switch (vs.Major)
      {
        case 6:
          switch (vs.Minor)
          {
            case 0:
              return OsVersion.WindowsVista;

            case 1:
              return OsVersion.Windows7;

            case 2:
              return OsVersion.Windows8;
          }
          break;

        case 10:
          return OsVersion.Windows10;

        default:
          return OsVersion.WindowsXp;
      }

      return OsVersion.WindowsXp;
    }

    /// <summary>
    /// Gets the product version of an assembly with the given path and name.
    /// </summary>
    /// <param name="assemblyFilePath">The file path of a DLL or EXE file.</param>
    /// <returns>The product version of an assembly with the given path and name.</returns>
    public static string GetProductVersion(string assemblyFilePath)
    {
      if (string.IsNullOrEmpty(assemblyFilePath) || !File.Exists(assemblyFilePath))
      {
        return string.Empty;
      }

      try
      {
        var asmName = AssemblyName.GetAssemblyName(assemblyFilePath);
        return asmName?.Version.ToString() ?? string.Empty;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets a value indicating whether an application with the given name is setup to run when Windows starts.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <returns>A value indicating whether an application with the given name is setup to run when Windows starts.</returns>
    public static bool GetRunAtStartUp(string appName)
    {
      bool exists;
      using (var regKey = RegistryHive.CurrentUser.OpenRegistryKey(RUN_AT_STARTUP_REGISTRY_KEY_NAME, true))
      {
        string runValue = regKey?.GetValue(appName, "false").ToString() ?? "false";
        bool.TryParse(runValue, out exists);
      }

      return exists;
    }

    /// <summary>
    /// Gets the first found process currently running with a given full or partial name and an executable file path.
    /// </summary>
    /// <param name="processPath">The path of the executable running as a process.</param>
    /// <param name="processName">The full or partial name of the process.</param>
    /// <returns>The first found process currently running with a given full or partial name and an executable file path.</returns>
    public static Process GetRunningProcess(string processPath, string processName)
    {
      return GetRunningProcessses(processPath, processName).FirstOrDefault();
    }

    /// <summary>
    /// Gets a process currently running with the given process ID.
    /// </summary>
    /// <param name="processId">The process ID.</param>
    /// <returns>A process currently running with the given process ID.</returns>
    public static Process GetRunningProcess(int processId)
    {
      return Process.GetProcesses().FirstOrDefault(p => p.Id == processId);
    }

    /// <summary>
    /// Gets an iterable enumerator of processes currently running with a given full or partial name and an executable file path.
    /// </summary>
    /// <param name="processName">The full or partial name of the process.</param>
    /// <returns>An iterable enumerator of processes currently running with a given full or partial name and an executable file path.</returns>
    public static IEnumerable<Process> GetRunningProcessses(string processName)
    {
      var lowerProcessName = processName.ToLowerInvariant();
      return
        Process.GetProcesses()
          .Where(
            p =>
              p.ProcessName.ToLowerInvariant().Contains(lowerProcessName));
    }

    /// <summary>
    /// Gets an iterable enumerator of processes currently running with a given full or partial name and an executable file path.
    /// </summary>
    /// <param name="processPath">The path of the executable running as a process.</param>
    /// <param name="processName">The full or partial name of the process.</param>
    /// <returns>An iterable enumerator of processes currently running with a given full or partial name and an executable file path.</returns>
    public static IEnumerable<Process> GetRunningProcessses(string processPath, string processName)
    {
      return GetRunningProcessses(processName).Where(p => p.MainModule.FileName.ToLowerInvariant().Equals(processPath, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Gets the character set and its collation used by the currently selected schema.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <returns>The character set and its collation used by the currently selected schema.</returns>
    public static Tuple<string, string> GetSchemaCharSetAndCollation(string connectionString, string schemaName)
    {
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(schemaName))
      {
        return null;
      }

      string sql = $"SHOW CREATE SCHEMA `{schemaName}`";
      var dataRow = ExecuteDataRow(connectionString, sql);
      if (dataRow == null || dataRow.ItemArray.Length < 2)
      {
        return null;
      }

      var createStatement = dataRow.ItemArray[1].ToString();
      if (string.IsNullOrEmpty(createStatement))
      {
        return null;
      }

      int charSetOptionIndex = createStatement.IndexOf("character set ", StringComparison.InvariantCultureIgnoreCase);
      if (charSetOptionIndex < 0)
      {
        return null;
      }

      string schemaCollation;
      int charSetIndex = charSetOptionIndex + 14;
      int spaceAfterCharSetIndex = createStatement.IndexOf(" ", charSetIndex,
        StringComparison.InvariantCultureIgnoreCase);
      var schemaCharSet = createStatement.Substring(charSetIndex, spaceAfterCharSetIndex - charSetIndex).Trim();
      int collationOptionIndex = createStatement.IndexOf("collate ", spaceAfterCharSetIndex,
        StringComparison.InvariantCultureIgnoreCase);
      if (collationOptionIndex < 0)
      {
        schemaCollation = GetDefaultCollationFromCharSet(connectionString, schemaCharSet);
      }
      else
      {
        int collationIndex = charSetOptionIndex + 8;
        int spaceAfterCollationIndex = createStatement.IndexOf(" ", collationIndex,
          StringComparison.InvariantCultureIgnoreCase);
        schemaCollation = createStatement.Substring(collationIndex, spaceAfterCollationIndex - collationIndex).Trim();
      }

      return new Tuple<string, string>(schemaCharSet, schemaCollation);
    }

    /// <summary>
    /// Gets the schema information ofr the given database collection.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="schemaInformation">The type of schema information to query.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    /// <param name="restrictions">Specific parameters that vary among database collections.</param>
    /// <returns>Schema information within a data table.</returns>
    public static DataTable GetSchemaInformation(string connectionString, SchemaInformationType schemaInformation, bool showErrors, params string[] restrictions)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return null;
      }

      DataTable dt = null;
      try
      {
        using (var baseConnection = new MySqlConnection(connectionString))
        {
          baseConnection.Open();
          MySqlDataAdapter mysqlAdapter;
          switch (schemaInformation)
          {
            case SchemaInformationType.ColumnsSimple:
              mysqlAdapter =
                new MySqlDataAdapter($"SHOW COLUMNS FROM `{restrictions[1]}`.`{restrictions[2]}`",
                  baseConnection);
              dt = new DataTable();
              mysqlAdapter.Fill(dt);
              break;

            case SchemaInformationType.Engines:
              mysqlAdapter = new MySqlDataAdapter("SELECT * FROM information_schema.engines ORDER BY engine",
                baseConnection);
              dt = new DataTable();
              mysqlAdapter.Fill(dt);
              break;

            case SchemaInformationType.Collations:
              string queryString;
              if (restrictions != null && restrictions.Length > 0 && !string.IsNullOrEmpty(restrictions[0]))
              {
                queryString = $"SHOW COLLATION WHERE charset = '{restrictions[0]}'";
              }
              else
              {
                queryString = "SHOW COLLATION";
              }

              mysqlAdapter = new MySqlDataAdapter(queryString, baseConnection);
              dt = new DataTable();
              mysqlAdapter.Fill(dt);
              break;

            case SchemaInformationType.CharacterSets:
              mysqlAdapter = new MySqlDataAdapter("SHOW CHARSET", baseConnection);
              dt = new DataTable();
              mysqlAdapter.Fill(dt);
              break;

            case SchemaInformationType.Routines:
              dt = GetRoutines(baseConnection, restrictions);
              break;

            default:
              dt = baseConnection.GetSchema(schemaInformation.ToCollection(), restrictions);
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, showErrors);
        if (!showErrors)
        {
          throw;
        }
      }

      return dt;
    }

    /// <summary>
    /// Gets the script text contained within a resource file with the specified name.
    /// </summary>
    /// <param name="resourceFileName">The name of the resource file.</param>
    /// <returns>The script text contained within a resource file with the specified name.</returns>
    public static string GetScriptFromResource(string resourceFileName)
    {
      var resourceAssembly = Assembly.GetCallingAssembly();
      var script = GetScriptFromResource(resourceAssembly, resourceFileName);
      if (!string.IsNullOrEmpty(script))
      {
        return script;
      }

      resourceAssembly = Assembly.GetExecutingAssembly();
      return GetScriptFromResource(resourceAssembly, resourceFileName);
    }

    /// <summary>
    /// Gets the script text contained within a resource file with the specified name.
    /// </summary>
    /// <param name="resourceAssembly">The assembly containing the resource files.</param>
    /// <param name="resourceFileName">The name of the resource file.</param>
    /// <returns>The script text contained within a resource file with the specified name.</returns>
    public static string GetScriptFromResource(Assembly resourceAssembly, string resourceFileName)
    {
      if (string.IsNullOrEmpty(resourceFileName) || resourceAssembly == null)
      {
        return null;
      }

      Stream stream = null;
      try
      {
        stream = resourceAssembly.GetManifestResourceStream(resourceFileName);
      }
      catch
      {
        // ignored
      }

      if (stream == null)
      {
        return null;
      }

      using (var sr = new StreamReader(stream))
      {
        return sr.ReadToEnd();
      }
    }

    /// <summary>
    /// Gets the collation defined on a MySQL table with the given name.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <param name="tableName">The name of the database table.</param>
    /// <param name="charSet">The character set that belongs to the table collation.</param>
    /// <returns>The collation defined on a MySQL table with the given name.</returns>
    public static string GetTableCollation(string connectionString, string schemaName, string tableName, out string charSet)
    {
      charSet = null;
      if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
      {
        return null;
      }

      var infoTable = GetSchemaInformation(connectionString, SchemaInformationType.Tables, true, null, schemaName, tableName);
      var tableCollation = infoTable == null || infoTable.Rows.Count == 0 ? null : infoTable.Rows[0]["TABLE_COLLATION"].ToString();
      if (!string.IsNullOrEmpty(tableCollation))
      {
        charSet = tableCollation.Substring(0, tableCollation.IndexOf("_", StringComparison.InvariantCultureIgnoreCase));
      }

      return tableCollation;
    }

    /// <summary>
    /// Gets the names of the entries contained in a ZIP archive.
    /// </summary>
    /// <param name="zipFile">The ZIP file full path.</param>
    /// <param name="fullNames">Flag indicating whether the full names (with relative path) are returned or just the simple file names.</param>
    /// <returns></returns>
    public static List<string> GetZipArchiveEntryNames(string zipFile, bool fullNames)
    {
      if (string.IsNullOrEmpty(zipFile)
          || !File.Exists(zipFile))
      {
        return null;
      }

      List<string> entries = null;
      try
      {
        using (var archive = ZipFile.OpenRead(zipFile))
        {
          entries = archive?.Entries.Select(zipArchiveEntry => fullNames ? zipArchiveEntry.FullName : zipArchiveEntry.Name).ToList();
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }


      return entries;
    }

    /// <summary>
    /// Checks if a process with the given name is currently running.
    /// </summary>
    /// <param name="processName">The name of a process.</param>
    /// <returns><c>true</c> if a process with the given name is currently running, <c>false</c> otherwise.</returns>
    public static bool IsProcessRunning(string processName)
    {
      string name = processName.ToLowerInvariant();
      return Process.GetProcesses().Any(p => p.ProcessName.ToLowerInvariant().Contains(name));
    }

    /// <summary>
    /// Gets a value indicating whether a process currently running with a given full or partial name and an executable file path exists.
    /// </summary>
    /// <param name="processPath">The path of the executable running as a process.</param>
    /// <param name="processName">The full or partial name of the process.</param>
    /// <returns><c>true</c> if a process currently running with a given full or partial name and an executable file path exists, <c>false</c> otherwise.</returns>
    public static bool IsProcessRunning(string processPath, string processName)
    {
      return GetRunningProcess(processPath, processName) != null;
    }

    /// <summary>
    /// Gets a value indicating whether a process currently running with the given process ID exists.
    /// </summary>
    /// <param name="processId">The process ID.</param>
    /// <returns><c>true</c> if a process currently running with the given process ID exists, <c>false</c> otherwise.</returns>
    public static bool IsProcessRunning(int processId)
    {
      return GetRunningProcess(processId) != null;
    }

    /// <summary>
    /// Checks if a process with the given name is installed.
    /// </summary>
    /// <param name="uninstallDisplayName">The display name of the application used for uninstalling it.</param>
    /// <returns><c>true</c> if a process with the given name is installed, <c>false</c> otherwise.</returns>
    public static bool IsProductInstalled(string uninstallDisplayName)
    {
      return RegistryHive.CurrentUser.IsProductInstalled(uninstallDisplayName)
             || RegistryHive.LocalMachine.IsProductInstalled(uninstallDisplayName);
    }

    /// <summary>
    /// Checks if the given string representing an IP address is a valid one.
    /// </summary>
    /// <param name="ipAddressString">A string representing an IP address.</param>
    /// <returns><c>true</c> if the given string representing an IP address is a valid one, <c>false</c> otherwise.</returns>
    public static bool IsValidIpAddress(string ipAddressString)
    {
      return IPAddress.TryParse(ipAddressString, out _);
    }

    /// <summary>
    /// Returns the registry key inside CurrentUser or LocalMahine that matches the key name.
    /// </summary>
    /// <param name="keyName">The name of the key to open.</param>
    /// <param name="writable">Flag indicating if the registry key is opened with write access or read-only one.</param>
    /// <returns>The matching registry key.</returns>
    public static RegistryKey OpenRegistryKey(string keyName, bool writable = false)
    {
      return RegistryHive.CurrentUser.OpenRegistryKey(keyName, writable) ?? RegistryHive.LocalMachine.OpenRegistryKey(keyName, writable);
    }

    /// <summary>
    /// Opens the registry key that matches the key name in the specified registry hive.
    /// </summary>
    /// <param name="registryHive">The registry hive.</param>
    /// <param name="keyName">The name of the key to open.</param>
    /// <param name="writable">Flag indicating if the registry key is opened with write access or read-only one.</param>
    /// <returns>The matching registry key.</returns>
    public static RegistryKey OpenRegistryKey(this RegistryHive registryHive, string keyName, bool writable = false)
    {
      if (string.IsNullOrEmpty(keyName))
      {
        return null;
      }

      RegistryKey key;
      if (Environment.Is64BitOperatingSystem)
      {
        key = registryHive.OpenRegistryKey(RegistryView.Registry64, keyName, writable);
        if (key != null)
        {
          return key;
        }

        key = registryHive.OpenRegistryKey(RegistryView.Registry32, keyName, writable);
        if (key != null)
        {
          return key;
        }
      }
      else
      {
        key = registryHive.OpenRegistryKey(RegistryView.Default, keyName, writable);
        if (key != null)
        {
          return key;
        }
      }

      return null;
    }

    /// <summary>
    /// Opens the registry key that matches the key name in the specified registry hive.
    /// </summary>
    /// <param name="registryHive">The registry hive.</param>
    /// <param name="registryView">The <see cref="RegistryView"/> to look the registry key in.</param>
    /// <param name="keyName">The name of the key to open.</param>
    /// <param name="writable">Flag indicating if the registry key is opened with write access or read-only one.</param>
    /// <returns>The matching registry key.</returns>
    public static RegistryKey OpenRegistryKey(this RegistryHive registryHive, RegistryView registryView, string keyName, bool writable = false)
    {
      if (string.IsNullOrEmpty(keyName))
      {
        return null;
      }

      try
      {
        var key = RegistryKey.OpenBaseKey(registryHive, registryView).OpenSubKey(keyName, writable);
        if (key != null)
        {
          return key;
        }
      }
      catch (Exception e)
      {
        Logger.LogException(e);
      }

      return null;
    }

    /// <summary>
    /// Opens the registry key that matches the sub-key name in the specified <see cref="RegistryKey"/>.
    /// </summary>
    /// <param name="parentKey">A <see cref="RegistryKey"/> where the search starts in.</param>
    /// <param name="subKeyName">The name of the child key to open.</param>
    /// <param name="writable">Flag indicating if the registry key is opened with write access or read-only one.</param>
    /// <returns>The matching registry key.</returns>
    public static RegistryKey OpenRegistrySubKey(this RegistryKey parentKey, string subKeyName, bool writable = false)
    {
      if (parentKey == null || string.IsNullOrEmpty(subKeyName))
      {
        return null;
      }

      try
      {
        return parentKey.OpenSubKey(subKeyName, writable);
      }
      catch (Exception e)
      {
        Logger.LogException(e);
      }

      return null;
    }

    /// <summary>
    /// Evaluate current system tcp connections to determine if the given port is available.
    /// This is the same information provided by the netstat command line application, 
    /// just in .Net strongly-typed object form.
    /// </summary>
    /// <param name="port">System port value to evaluate</param>
    /// <returns><c>true</c> if the given port is free, otherwise <c>false</c>.</returns>
    public static bool PortIsAvailable(uint port)
    {
      var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
      var endPoints = ipGlobalProperties.GetActiveTcpListeners();
      return endPoints.All(endPoint => endPoint.Port != port);
    }

    /// <summary>
    /// Runs the NetShell in the background.
    /// </summary>
    /// <param name="arguments">The arguments passed to the NetShell.</param>
    /// <param name="standardOutput">The redirected standard output from the process.</param>
    /// <param name="standardError">The redirected standard error from the process.</param>
    /// <returns><c>true</c> if the NetShell process ran and completed without errors, <c>false</c> otherwise.</returns>
    public static bool RunNetShellProcess(string arguments, out string standardOutput, out string standardError)
    {
      standardOutput = null;
      standardError = null;
      bool success;
      try
      {
        var processStartInfo = new ProcessStartInfo("netsh.exe")
        {
          Arguments = arguments,
          RedirectStandardInput = true,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
          Verb = "runas"
        };
        using (var process = Process.Start(processStartInfo))
        {
          if (process == null)
          {
            return false;
          }

          process.WaitForExit();
          standardOutput = process.StandardOutput.ReadToEnd();
          standardError = process.StandardError.ReadToEnd();
          success = process.ExitCode == 0;
        }
      }
      catch (Exception ex)
      {
        success = false;
        if (standardError != null)
        {
          standardError += Environment.NewLine + ex.Message;
        }

        Logger.LogException(ex);
      }

      return success;
    }

    /// <summary>
    /// Runs a process in the background.
    /// </summary>
    /// <param name="executableFilePath">The full path to the executable file to run.</param>
    /// <param name="arguments">The arguments passed to the executable file.</param>
    /// <param name="workingDirectory">The full path to the directory where the process must be started.</param>
    /// <param name="outputRedirectionDelegate">A delegate to redirect standard output messages to.</param>
    /// <param name="errorRedirectionDelegate">A delegate to redirect the error messages to.</param>
    /// <param name="waitForExit">Flag indicating whether to wait until the process exits before returning it or not.</param>
    /// <param name="hideProcessMessages">Avoids printing messages to the <seealso cref="outputRedirectionDelegate"/> with process information.</param>
    /// <param name="waitLongerWorkaround">A workaround needed for some rogue processes like MySQL Shell that when <seealso cref="waitForExit"/> is <c>true</c> it needs some time to quit.</param>
    /// <returns>A <seealso cref="ProcessResult"/> instance, or <c>null</c> if the process did not run at all.</returns>
    public static ProcessResult RunProcess(string executableFilePath, string arguments, string workingDirectory, Action<string> outputRedirectionDelegate, Action<string> errorRedirectionDelegate, bool waitForExit, bool hideProcessMessages = false, bool waitLongerWorkaround = false)
    {
      if (string.IsNullOrEmpty(executableFilePath)
          || !File.Exists(executableFilePath))
      {
        return null;
      }

      ProcessResult result = null;
      try
      {
        if (string.IsNullOrEmpty(workingDirectory)
            || !Directory.Exists(workingDirectory))
        {
          workingDirectory = Path.GetDirectoryName(executableFilePath);
        }

        var processStartInfo = new ProcessStartInfo(executableFilePath)
        {
          Arguments = arguments.Trim(),
          RedirectStandardInput = true,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true
        };

        // It might seems this validation is not needed, but Path.GetDirectoryName name above could still return null or empty string.
        if (!string.IsNullOrEmpty(workingDirectory))
        {
          processStartInfo.WorkingDirectory = workingDirectory;
        }

        if (!hideProcessMessages)
        {
          outputRedirectionDelegate?.Invoke(string.Format(Resources.StartingProcessDetailsText, processStartInfo.FileName, processStartInfo.Arguments));
        }

        var process = Process.Start(processStartInfo);
        if (process == null)
        {
          return null;
        }

        int pid = process.Id;
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();
        var processName = process.ProcessName;
        process.OutputDataReceived += (sender, e) =>
        {
          outputRedirectionDelegate?.Invoke(e.Data);
          outputBuilder.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
          errorRedirectionDelegate?.Invoke(e.Data);
          errorBuilder.AppendLine(e.Data);
        };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.StandardInput.Close();
        if (waitForExit)
        {
          if (waitLongerWorkaround)
          {
            while (!process.WaitForExit(3000))
            {
              if (GetRunningProcess(pid) != null)
              {
                // This is a dummy block, its intention is purely to act as a workaround for processes
                // that do not end gracefully when waiting for them inifinitely (like the MySQL Shell one).
              }
            }
          }
          else
          {
            // Wait indefinitely
            process.WaitForExit();
          }
        }

        result = new ProcessResult(
          process.HasExited ? null : process,
          process.HasExited ? (int?)process.ExitCode : null,
          outputBuilder.ToString(),
          errorBuilder.ToString());
        if (!hideProcessMessages)
        {
          outputRedirectionDelegate?.Invoke(!process.HasExited
          ? string.Format(Resources.StartedAndRunningProcessSuccessfullyText, processName, pid)
          : string.Format(Resources.StartedAndExitedProcessSuccessfullyText, processName, pid, process.ExitCode));
        }

        if (process.HasExited)
        {
          process.Dispose();
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        errorRedirectionDelegate?.Invoke(string.Format(Resources.StartedProcessErrorText, Path.GetFileNameWithoutExtension(executableFilePath), ex.Message));
      }

      return result;
    }

    /// <summary>
    /// Sets the net_write_timeout and net_read_timeout MySQL server variables to the given value for the duration of the current client session.
    /// </summary>
    /// <param name="connectionString">A connection string to establish a DB connection.</param>
    /// <param name="timeoutInSeconds">The number of seconds to wait for more data from a connection before aborting the read or for a block to be written to a connection before aborting the write.</param>
    public static void SetClientSessionReadWriteTimeouts(string connectionString, uint timeoutInSeconds)
    {
      if (string.IsNullOrEmpty(connectionString) || timeoutInSeconds < 1)
      {
        return;
      }

      string sql = string.Format("SET SESSION net_write_timeout = {0}, SESSION net_read_timeout = {0}", timeoutInSeconds);
      ExecuteNonQuery(connectionString, sql);
    }

    /// <summary>
    /// Sets a value indicating whether an application with the given name is setup to run when Windows starts.
    /// </summary>
    /// <param name="appName">The name of the application.</param>
    /// <param name="enable">Flag indicating if the application runs when Windows starts.</param>
    public static void SetRunAtStartUp(string appName, bool enable)
    {
      using (var regKey = RegistryHive.CurrentUser.OpenRegistryKey(RUN_AT_STARTUP_REGISTRY_KEY_NAME, true))
      {
        if (regKey == null)
        {
          return;
        }

        if (enable)
        {
          var entryAssembly = Assembly.GetEntryAssembly();
          if (entryAssembly?.Location == null)
          {
            return;
          }

          regKey.SetValue(appName, entryAssembly.Location);
        }
        else
        {
          regKey.DeleteValue(appName, false);
        }
      }
    }

    /// <summary>
    /// Splits the given string containing command line arguments into an array of arguments.
    /// </summary>
    /// <param name="unsplitArgumentLine">A string containing command line arguments.</param>
    /// <returns>An array of arguments.</returns>
    public static string[] SplitArgs(string unsplitArgumentLine)
    {
      int numberOfArgs;
      IntPtr ptrToSplitArgs = CommandLineToArgvW(unsplitArgumentLine, out numberOfArgs);

      // CommandLineToArgvW returns NULL upon failure.
      if (ptrToSplitArgs == IntPtr.Zero)
      {
        throw new ArgumentException("Unable to split argument.", new Win32Exception());
      }

      // Make sure the memory ptrToSplitArgs to is freed, even upon failure.
      try
      {
        string[] splitArgs = new string[numberOfArgs];

        // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
        // Copy each of these strings into our split argument array.
        for (int i = 0; i < numberOfArgs; i++)
        {
          splitArgs[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));
        }

        return splitArgs;
      }
      finally
      {
        // Free memory obtained by CommandLineToArgW.
        LocalFree(ptrToSplitArgs);
      }
    }

    /// <summary>
    /// Extracts a specific entry in a ZIP file with a given name in the specified target directory.
    /// </summary>
    /// <param name="zipFile">The ZIP file full path.</param>
    /// /// <param name="entryName">The file name inside the ZIP file that is intended to be extracted.</param>
    /// <param name="targetEntryName">The file name given after the extraction, if null or empty the <seealso cref="entryName"/> is used.</param>
    /// <param name="targetDirectory">The target directory where the archived contents will be uncompressed to.</param>
    /// <param name="reThrowException">Flag indicating whether Exceptions should be re-thrown.</param>
    /// <returns><c>true</c> if the extraction completes successfully, <c>false</c> otherwise.</returns>
    public static bool UnZipEntry(string zipFile, string entryName, string targetEntryName, string targetDirectory, bool reThrowException = false)
    {
      if (string.IsNullOrEmpty(zipFile)
          || !File.Exists(zipFile)
          || string.IsNullOrEmpty(targetDirectory)
          || string.IsNullOrEmpty(entryName))
      {
        return false;
      }

      var error = ValidateFilePath(targetDirectory);
      if (!string.IsNullOrEmpty(error))
      {
        if (reThrowException)
        {
          throw new Exception(error);
        }

        return false;
      }

      var success = true;
      try
      {
        using (var archive = ZipFile.OpenRead(zipFile))
        {
          var msiZipEntry = archive.Entries.FirstOrDefault(entry => entry.Name.Equals(entryName, StringComparison.OrdinalIgnoreCase));
          if (msiZipEntry == null)
          {
            return false;
          }

          msiZipEntry.ExtractToFile(Path.Combine(targetDirectory, string.IsNullOrEmpty(targetEntryName) ? entryName : targetEntryName), true);
        }
      }
      catch (Exception ex)
      {
        success = false;
        Logger.LogException(ex);
        if (reThrowException)
        {
          throw;
        }
      }

      return success;
    }

    /// <summary>
    /// Extracts the content of a ZIP file in the specified target directory.
    /// </summary>
    /// <param name="zipFile">The ZIP file full path.</param>
    /// <param name="targetDirectory">The target directory where the archived contents will be uncompressed to.</param>
    /// <param name="reThrowException">Flag indicating whether Exceptions should be re-thrown.</param>
    /// <returns><c>true</c> if the extraction completes successfully, <c>false</c> otherwise.</returns>
    public static bool UnZipFile(string zipFile, string targetDirectory, bool reThrowException = false)
    {
      if (string.IsNullOrEmpty(zipFile)
          || !File.Exists(zipFile)
          || string.IsNullOrEmpty(targetDirectory))
      {
        return false;
      }

      var error = ValidateFilePath(targetDirectory);
      if (!string.IsNullOrEmpty(error))
      {
        if (reThrowException)
        {
          throw new Exception(error);
        }

        return false;
      }

      var success = true;
      try
      {
        ZipFile.ExtractToDirectory(zipFile, targetDirectory);
      }
      catch (Exception ex)
      {
        success = false;
        Logger.LogException(ex);
        if (reThrowException)
        {
          throw;
        }
      }

      return success;
    }

    /// <summary>
    /// Validates a given file path.
    /// </summary>
    /// <param name="filePath">A file path.</param>
    /// <returns>An error message if the file path is invalid, or <c>null</c> if it's valid.</returns>
    public static string ValidateFilePath(string filePath)
    {
      string errorMessage = null;
      try
      {
        var directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directoryPath)
            && !Path.IsPathRooted(filePath))
        {
          errorMessage = Resources.PathNotAbsoluteError;
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
          var absolutePath = Path.GetFullPath(filePath);
          if (!string.IsNullOrEmpty(absolutePath))
          {
            var fileName = Path.GetFileName(absolutePath);
            if (string.IsNullOrEmpty(fileName))
            {
              errorMessage = Resources.PathContainsNoFileError;
            }

            if (string.IsNullOrEmpty(errorMessage)
                && !string.IsNullOrEmpty(directoryPath)
                && !Directory.Exists(directoryPath))
            {
              errorMessage = Resources.PathNonExistentDirectoryError;
            }
          }
        }
      }
      catch (ArgumentNullException)
      {
        errorMessage = Resources.PathIsNullError;
      }
      catch (ArgumentException)
      {
        errorMessage = Resources.PathContainsInvalidCharactersError;
      }
      catch (PathTooLongException)
      {
        errorMessage = Resources.PathTooLongError;
      }
      catch (NotSupportedException)
      {
        errorMessage = Resources.PathContainsColonError;
      }
      catch (Exception)
      {
        errorMessage = Resources.PathUnknownError;
      }

      return errorMessage;
    }

    /// <summary>
    /// Parses a Unicode command line string and returns an array of pointers to the command line arguments, along with a count of such arguments, in a way that is similar to the standard C run-time argv and argc values.
    /// </summary>
    /// <param name="lpCmdLine">Pointer to a null-terminated Unicode string that contains the full command line. If this parameter is an empty string the function returns the path to the current executable file.</param>
    /// <param name="pNumArgs">Pointer to an int that receives the number of array elements returned, similar to argc.</param>
    /// <returns>A pointer to an array of LPWSTR values, similar to argv. If the method fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
    [DllImport(DllImportConstants.SHELL32, SetLastError = true)]
    private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

    /// <summary>
    /// Gets the install path of an application with the given name from the Windows Registry.
    /// </summary>
    /// <param name="registryHive">The <see cref="RegistryHive"/> where the sub key is retrieved from.</param>
    /// <param name="registryView">The <see cref="RegistryView"/> to look the registry key in.</param>
    /// <param name="applicationName">The name of a MySQL related application.</param>
    /// <returns>The install path of an application with the given name.</returns>
    private static string GetMySqlAppInstallLocation(this RegistryHive registryHive, RegistryView registryView, string applicationName)
    {
      if (string.IsNullOrEmpty(applicationName))
      {
        return null;
      }

      const string MYSQL_REGISTRY_KEY_NAME = @"Software\MySQL AB";
      using (var key = registryHive.OpenRegistryKey(registryView, MYSQL_REGISTRY_KEY_NAME))
      {
        var subKeyName = key?.GetSubKeyNames().FirstOrDefault(skn => skn.Contains(applicationName));
        if (string.IsNullOrEmpty(subKeyName))
        {
          return null;
        }

        using (var subKey = key.OpenRegistrySubKey(subKeyName))
        {
          var locationValue = subKey.GetValue("Location");
          return locationValue?.ToString();
        }
      }
    }

    /// <summary>
    /// Returns a <see cref="DataTable"/> with information for MySQL routines (stored procedures).
    /// </summary>
    /// <remarks>This is a temporary solution while C/NET fixes the problem retrieving stored procedures data in Server versions >= 8.0.</remarks>
    /// <param name="connection">The <see cref="MySqlConnection"/> used to query the database.</param>
    /// <param name="restrictions">Specific parameters that vary among database collections.</param>
    /// <returns>A <see cref="DataTable"/> with information for MySQL routines (stored procedures).</returns>
    private static DataTable GetRoutines(MySqlConnection connection, string[] restrictions)
    {
      string[] keys = new string[4];
      keys[0] = "ROUTINE_CATALOG";
      keys[1] = "ROUTINE_SCHEMA";
      keys[2] = "ROUTINE_NAME";
      keys[3] = "ROUTINE_TYPE";

      var query = new StringBuilder("SELECT * FROM INFORMATION_SCHEMA.ROUTINES");
      string whereClause = GetWhereClause(null, keys, restrictions);
      if (!string.IsNullOrEmpty(whereClause))
      {
        query.Append(" WHERE ");
        query.Append(whereClause);
      }

      var dt = GetTableFromQuery(connection, query.ToString());
      if (dt != null)
      {
        dt.TableName = "Routines";
      }

      return dt;
    }

    /// <summary>
    /// Returns a <see cref="DataTable"/> filled with data using the given query.
    /// </summary>
    /// <param name="connection">The <see cref="MySqlConnection"/> used to query the database.</param>
    /// <param name="sql">A query.</param>
    /// <returns>A <see cref="DataTable"/> filled with data using the given query.</returns>
    private static DataTable GetTableFromQuery(MySqlConnection connection, string sql)
    {
      if (connection == null || string.IsNullOrEmpty(sql))
      {
        return null;
      }

      if (connection.State != ConnectionState.Open)
      {
        connection.Open();
      }

      var dt = new DataTable();
      var cmd = new MySqlCommand(sql, connection);
      var reader = cmd.ExecuteReader();

      // add columns
      for (int i = 0; i < reader.FieldCount; i++)
      {
        var columnName = reader.GetName(i);
        var columnType = reader.GetFieldType(i);
        dt.Columns.Add(columnName, columnType ?? typeof(string));
      }

      using (reader)
      {
        while (reader.Read())
        {
          var row = dt.NewRow();
          for (int i = 0; i < reader.FieldCount; i++)
          {
            row[i] = reader.GetValue(i);
          }

          dt.Rows.Add(row);
        }
      }

      return dt;
    }

    /// <summary>
    /// Assembles a WHERE clause from given arrays of column names and their corresponding values.
    /// </summary>
    /// <param name="initialWhere">Initial where clause.</param>
    /// <param name="columnNames">An array of column names.</param>
    /// <param name="columnValues">An array of values.</param>
    /// <returns>A WHERE clause.</returns>
    private static string GetWhereClause(string initialWhere, string[] columnNames, string[] columnValues)
    {
      if (columnNames == null || columnValues == null)
      {
        return string.Empty;
      }

      var where = new StringBuilder(initialWhere);
      for(int i = 0; i < columnNames.Length; i++)
      {
        if (i >= columnValues.Length)
        {
          break;
        }

        if (string.IsNullOrEmpty(columnValues[i]))
        {
          continue;
        }

        if (where.Length > 0)
        {
          where.Append(" AND ");
        }

        where.Append(columnNames[i]);
        where.Append(" LIKE '");
        where.Append(columnValues[i]);
        where.Append("'");
      }

      return where.ToString();
    }

    /// <summary>
    /// Checks if a process with the given name is installed.
    /// </summary>
    /// <param name="registryHive">The <see cref="RegistryHive"/> where the sub key is retrieved from.</param>
    /// <param name="uninstallDisplayName">The display name of the application used for uninstalling it.</param>
    /// <returns><c>true</c> if a process with the given name is installed, <c>false</c> otherwise.</returns>
    private static bool IsProductInstalled(this RegistryHive registryHive, string uninstallDisplayName)
    {
      const string SUBKEY_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
      if (Environment.Is64BitOperatingSystem)
      {
        if (SubKeyAttributeContainsValue(SUBKEY_NAME, registryHive, RegistryView.Registry64, "DisplayName", uninstallDisplayName))
        {
          return true;
        }

        if (SubKeyAttributeContainsValue(SUBKEY_NAME, registryHive, RegistryView.Registry32, "DisplayName", uninstallDisplayName))
        {
          return true;
        }
      }
      else
      {
        if (SubKeyAttributeContainsValue(SUBKEY_NAME, registryHive, RegistryView.Default, "DisplayName", uninstallDisplayName))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Retrieves the window handle to the active window attached to the calling thread's message queue.
    /// </summary>
    /// <returns>The handle to the active window attached to the calling thread's message queue.</returns>
    [DllImport(DllImportConstants.USER32, ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.Process)]
    private static extern IntPtr GetActiveWindow();

    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently working).
    /// </summary>
    /// <remarks>The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.</remarks>
    /// <returns>A handle to the foreground window.</returns>
    [DllImport(DllImportConstants.USER32, ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.Process)]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Frees up the memory of the given pointer.
    /// </summary>
    /// <param name="hMem">A <see cref="IntPtr"/> pointer.</param>
    /// <returns><c>null</c> if the memory was freed successfully, otherwise the given pointer is returned.</returns>
    [DllImport(DllImportConstants.KERNEL32)]
    private static extern IntPtr LocalFree(IntPtr hMem);

    /// <summary>
    /// Checks if an attribute of a registry sub key with the given name contains a specific value.
    /// </summary>
    /// <param name="subKeyName">The registry sub key name.</param>
    /// <param name="registryHive">The <see cref="RegistryHive"/> where the sub key is retrieved from.</param>
    /// <param name="registryView">The <see cref="RegistryView"/> to look the registry key in.</param>
    /// <param name="attributeName">The name of the attribute within the sub key.</param>
    /// <param name="value">The value to look for in the attribute.</param>
    /// <returns><c>true if an attribute of a registry sub key with the given name contains a specific value, <c>false</c> otherwise.</c></returns>
    private static bool SubKeyAttributeContainsValue(string subKeyName, RegistryHive registryHive, RegistryView registryView, string attributeName, string value)
    {
      using (var key = registryHive.OpenRegistryKey(registryView, subKeyName))
      {
        if (key == null)
        {
          return false;
        }

        foreach (string kn in key.GetSubKeyNames())
        {
          RegistryKey subkey;
          using (subkey = key.OpenRegistrySubKey(kn))
          {
            if (subkey == null || !subkey.GetValueNames().Contains(attributeName))
            {
              continue;
            }

            var attributeValue = subkey.GetValue(attributeName).ToString();
            if (attributeValue.Contains(value))
            {
              return true;
            }
          }
        }
      }

      return false;
    }
  }
}