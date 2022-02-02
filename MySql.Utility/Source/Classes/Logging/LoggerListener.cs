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
using System.Diagnostics;
using System.IO;

namespace MySql.Utility.Classes.Logging
{
  public class LoggerListener : TraceListener
  {
    private TextWriterTraceListener _textFile;
    private List<string> _buffer;

    public LoggerListener()
    {
      _buffer = new List<string>();
    }

    public override void Write(string message)
    {
      CheckTextFile();
      if (_textFile != null)
      {
        _textFile.Write(message);
        _textFile.Flush();
      }
      else
      {
        _buffer.Add(message);
      }
    }

    public override void WriteLine(string message)
    {
      Write(message + Environment.NewLine);
    }

    private void CheckTextFile()
    {
      if (_textFile != null
          || string.IsNullOrEmpty(Logger.HomeDirectory)
          || string.IsNullOrEmpty(Logger.LogFileBaseName))
      {
        return;
      }

      string logPath = Logger.HomeDirectory;
      if (Logger.AppendLogsToHome)
      {
        logPath += "\\Logs";
      }

      if (!Directory.Exists(logPath))
      {
        Directory.CreateDirectory(logPath);
      }

      // Bind log files to the currently logged-in user, to avoid trouble when
      // this machine is accessed by other user at the same time
      // (e.g. via Switch User or via terminal services).
      var baseFileName = string.IsNullOrEmpty(Path.GetExtension(Logger.LogFileBaseName))
        ? Logger.LogFileBaseName
        : Path.GetFileNameWithoutExtension(Logger.LogFileBaseName);
      var userPrefix = Logger.PrependUserNameToLogFileName ? Environment.UserName + "-" : string.Empty;
      string logFile = $"{logPath}\\{userPrefix}{baseFileName}.log";
      if (File.Exists(logFile))
      {
        BackupLogFile(logFile);
      }

      _textFile = new TextWriterTraceListener(logFile);
      foreach (string msg in _buffer)
      {
        _textFile.Write(msg);
      }

      _buffer.Clear();
    }

    /// <summary>
    /// Checks if the given log file exists and if it does renames it so we can create a new
    /// log file with that name. This function implements a simple log rotation by removing the
    /// oldest log file if the upper limit of files is reached.
    /// The maximum number of files can be configured via the config file, but not more than 100.
    /// </summary>
    private void BackupLogFile(string logFile)
    {
      int maxCount = Math.Min(Logger.MaxLogCount, 100);
      if (maxCount < 2)
      {
        maxCount = 2;
      }

      string currentTime = DateTime.Now.ToString(@"yyyy-MM-dd-HH_mm_ss");
      string path = Path.GetDirectoryName(logFile);
      string baseName = Path.GetFileNameWithoutExtension(logFile);
      string[] logFiles = Directory.GetFiles(path, baseName + "*.log", SearchOption.TopDirectoryOnly);
      if (logFiles.Length >= maxCount)
      {
        // Too many backup files. Remove the oldest files to make room.
        List<string> sortedFiles = new List<string>(logFiles);
        sortedFiles.Sort();
        try
        {
          for (int i = 0; i < (logFiles.Length - maxCount + 1); i++)
          {
            File.Delete(sortedFiles[i]);
          }
        }
        catch
        {
          // Ignore file deletion errors. If that fails then we simply cannot clean up.
        }
      }

      try
      {
        // We test for the existence of the log file here, not earlier, to let
        // the cleanup from above kick in anyway.
        if (File.Exists(logFile))
        {
          string backupFile = Path.Combine(path, baseName + " " + currentTime + ".log");
          if (File.Exists(backupFile))
          {
            File.Delete(backupFile);
          }

          File.Move(logFile, backupFile);
        }
      }
      catch
      {
        // Ignore file deletion and backup errors.
      }
    }
  }
}
