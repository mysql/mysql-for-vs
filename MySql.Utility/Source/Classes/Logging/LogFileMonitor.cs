// Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Text;

namespace MySql.Utility.Classes.Logging
{
  /// <summary>
  /// Represents a log file that can be monitored for file writes
  /// </summary>
  public class LogFileMonitor : LogFile, IDisposable
  {
    #region Fields

    /// <summary>
    /// Watcher to monitor writes against this log file
    /// </summary>
    private FileSystemWatcher _logWatcher;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the LogFileMonitor class
    /// </summary>
    /// <param name="filePath">Log file full path and name</param>
    /// <param name="encoding">File encoding</param>
    /// <param name="lineSeparator">Line separator used by the log file</param>
    public LogFileMonitor(string filePath, Encoding encoding, string lineSeparator)
      : base(filePath, encoding, lineSeparator)
    {
      ChangesMonitorRunning = false;
      ChangeTimestampsList = null;
    }

    /// <summary>
    /// Initializes a new instance of the LogFileMonitor class
    /// </summary>
    /// <param name="filePath">Log file full path and name</param>
    public LogFileMonitor(string filePath)
      : this(filePath, Encoding.ASCII, Environment.NewLine)
    {
    }

    #region Properties

    /// <summary>
    /// Gets a list of timestamps of every change (write or creation) done against the log file
    /// </summary>
    public List<DateTime> ChangeTimestampsList { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the monitor of the writes to the log file is enabled, can be set to true only if WritesMonitoringIntervalsLength > 0
    /// </summary>
    public bool ChangesMonitorRunning { get; private set; }

    /// <summary>
    /// Gets the total number of times the log file has been written to
    /// </summary>
    public int TotalChanges => ChangeTimestampsList?.Count ?? 0;

    #endregion Properties

    /// <summary>
    /// Releases all resources used by the <see cref="LogFileMonitor"/> class
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Starts monitoring the number of file writes done to the log file per interval length and resets the monitoring log
    /// </summary>
    public void StartMonitoring()
    {
      //// Checks prior monitoring starts
      if (!Exists)
      {
        Logger.LogError(Resources.LogFileNotExistsErrorText);
        return;
      }

      //// Setup the log file watcher that fires every time the system detects the file is being written or its size changes
      if (_logWatcher == null)
      {
        _logWatcher = new FileSystemWatcher(LogFileInfo.DirectoryName, LogFileInfo.Name)
        {
          NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime
        };
        _logWatcher.Changed += LogFileChanged;
      }

      _logWatcher.EnableRaisingEvents = true;

      //// Reset number of writes on current interval and the monitoring log
      ChangeTimestampsList = new List<DateTime>();
      ChangesMonitorRunning = true;
    }

    /// <summary>
    /// Stops the monitoring of file writes done to the log file
    /// </summary>
    public void StopMonitoring()
    {
      _logWatcher.EnableRaisingEvents = false;
      ChangesMonitorRunning = false;
    }

    /// <summary>
    /// Returns the number of changes (writes or creation) the log file had within the given time interval
    /// </summary>
    /// <param name="startTime">Start time of the interval</param>
    /// <param name="endTime">End time of the interval</param>
    /// <returns>Number of changes (writes or creation) within interval</returns>
    public int ChangesInTimeInterval(DateTime startTime, DateTime endTime)
    {
      int changesQty = 0;
      if (ChangeTimestampsList != null && ChangeTimestampsList.Count > 0)
      {
        if (startTime > endTime)
        {
          throw new ArgumentOutOfRangeException(nameof(startTime), Resources.StartValueGreaterThanEndValueExceptionText);
        }

        if (endTime < ChangeTimestampsList[0] || startTime > ChangeTimestampsList[ChangeTimestampsList.Count - 1])
        {
          return changesQty;
        }

        changesQty += ChangeTimestampsList.Where(dt => dt >= startTime).TakeWhile(dt => dt <= endTime).Count();
      }

      return changesQty;
    }

    /// <summary>
    /// Returns the number of changes (writes or creation) the log file had within the given time interval
    /// </summary>
    /// <param name="startTime">Start time of the interval</param>
    /// <param name="firstMilliseconds">Offset in milliseconds after the start time</param>
    /// <returns>Number of changes (writes or creation) within interval</returns>
    public int ChangesInTimeInterval(DateTime startTime, double firstMilliseconds)
    {
      return ChangesInTimeInterval(startTime, startTime.AddMilliseconds(firstMilliseconds));
    }

    /// <summary>
    /// Returns the number of changes (writes or creation) the log file had within the given time interval
    /// </summary>
    /// <param name="lastMilliseconds">Offset in milliseconds before the end time</param>
    /// <param name="endTime">End time of the interval</param>
    /// <returns>Number of changes (writes or creation) within interval</returns>
    public int ChangesInTimeInterval(double lastMilliseconds, DateTime endTime)
    {
      return ChangesInTimeInterval(endTime.AddMilliseconds(lastMilliseconds * -1), endTime);
    }

    /// <summary>
    /// Checks if the log file had changes (writes or creation) within the given time interval
    /// </summary>
    /// <param name="startTime">Start time of the interval</param>
    /// <param name="endTime">End time of the interval</param>
    /// <returns>Flag indicating if the log file had changes</returns>
    public bool HasChangesInTimeInterval(DateTime startTime, DateTime endTime)
    {
      return ChangesInTimeInterval(startTime, endTime) > 0;
    }

    /// <summary>
    /// Checks if the log file had changes (writes or creation) within the given time interval
    /// </summary>
    /// <param name="startTime">Start time of the interval</param>
    /// <param name="firstMilliseconds">Offset in milliseconds after the start time</param>
    /// <returns>Flag indicating if the log file had changes</returns>
    public bool HasChangesInTimeInterval(DateTime startTime, double firstMilliseconds)
    {
      return ChangesInTimeInterval(startTime, firstMilliseconds) > 0;
    }

    /// <summary>
    /// Checks if the log file had changes (writes or creation) within the given time interval
    /// </summary>
    /// <param name="lastMilliseconds">Offset in milliseconds before the end time</param>
    /// <param name="endTime">End time of the interval</param>
    /// <returns>Flag indicating if the log file had changes</returns>
    public bool HasChangesInTimeInterval(double lastMilliseconds, DateTime endTime)
    {
      return ChangesInTimeInterval(lastMilliseconds, endTime) > 0;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="LogFileMonitor"/> class
    /// </summary>
    /// <param name="disposing">If true this is called by Dispose(), otherwise it is called by the finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        //// Free managed resources
        if (_logWatcher != null)
        {
          _logWatcher.Changed -= LogFileChanged;
          _logWatcher.Dispose();
          _logWatcher = null;
        }
      }

      //// Add class finalizer if unmanaged resources are added to the class
      //// Free unmanaged resources if there are any
    }

    /// <summary>
    /// Delegate method fired when the log file changes to record the times the file has been written to.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event arguments</param>
    private void LogFileChanged(object sender, FileSystemEventArgs e)
    {
      if (!ChangesMonitorRunning)
      {
        return;
      }

      ChangeTimestampsList?.Add(DateTime.Now);
    }
  }
}