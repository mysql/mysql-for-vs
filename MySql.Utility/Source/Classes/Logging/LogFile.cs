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
using System.IO;
using System.Text;

namespace MySql.Utility.Classes.Logging
{
  /// <summary>
  /// Represents a log file used by any of the bundled products
  /// </summary>
  public class LogFile
  {
    #region Fields

    /// <summary>
    /// File information for the log file
    /// </summary>
    private FileInfo _logFileInfo;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the LogFile class
    /// </summary>
    /// <param name="filePath">Log file full path and name</param>
    /// <param name="encoding">File encoding</param>
    /// <param name="lineSeparator">Line separator used by the log file</param>
    public LogFile(string filePath, Encoding encoding, string lineSeparator)
    {
      if (string.IsNullOrEmpty(filePath))
      {
        throw new ArgumentNullException(nameof(filePath));
      }

      LogFileInfo = new FileInfo(filePath);
      FileEncoding = encoding;
      LineSeparator = lineSeparator;
    }

    /// <summary>
    /// Initializes a new instance of the LogFile class
    /// </summary>
    /// <param name="filePath">Log file full path and name</param>
    public LogFile(string filePath)
      : this(filePath, Encoding.ASCII, Environment.NewLine)
    {
    }

    #region Properties

    /// <summary>
    /// Gets or sets a description of what this log file is for, empty by default
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets a value indicating whether the log file exists on disk
    /// </summary>
    public bool Exists => _logFileInfo.Exists;

    /// <summary>
    /// Gets or sets the log file encoding
    /// </summary>
    public Encoding FileEncoding { get; set; }

    /// <summary>
    /// Gets the full file path up to the log file's containing directory
    /// </summary>
    public string FullDirectory => _logFileInfo.DirectoryName;

    /// <summary>
    /// Gets the full file path and name of the log file
    /// </summary>
    public string FullName => _logFileInfo.FullName;

    /// <summary>
    /// Gets a value indicating whether the log file is locked for writing by another process
    /// </summary>
    public bool IsLocked => IsFileLocked(FullName);

    /// <summary>
    /// Gets or sets the line separator used by the log file
    /// </summary>
    public string LineSeparator { get; set; }

    /// <summary>
    /// Gets the file name of the log file
    /// </summary>
    public string Name => _logFileInfo.Name;

    /// <summary>
    /// Gets the file information of the log file
    /// </summary>
    protected FileInfo LogFileInfo
    {
      get { return _logFileInfo; }
      private set { _logFileInfo = value; }
    }

    #endregion Properties

    /// <summary>
    /// Checks if a file is open by another process
    /// </summary>
    /// <param name="filePath">File path</param>
    /// <returns>Boolean indicating if file is open</returns>
    public static bool IsFileLocked(string filePath)
    {
      bool isOpen = false;
      FileInfo file = new FileInfo(filePath);
      if (file.Exists)
      {
        FileStream stream = null;
        //// Code to check if file is open
        try
        {
          stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
          //// The file is unavailable because it is still being written to or being processed by another thread
          isOpen = true;
        }
        finally
        {
          stream?.Close();
        }
      }

      return isOpen;
    }

    /// <summary>
    /// Returns the last lines of this log file
    /// </summary>
    /// <param name="lastNLines">Number of lines to return (from the end of the file)</param>
    /// <returns>String containing the required last N lines of text file</returns>
    public string GetLastNLines(long lastNLines)
    {
      string retLines = null;
      if (Exists)
      {
        FileStream stream = null;
        try
        {
          int sizeOfChar = FileEncoding.GetByteCount("\n");
          byte[] buffer = FileEncoding.GetBytes(LineSeparator);

          using (stream = new FileStream(FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {
            if (stream.Length > 0)
            {
              long lineCount = 0;
              long endPosition = stream.Length / sizeOfChar;

              for (long position = sizeOfChar; position < endPosition; position += sizeOfChar)
              {
                stream.Seek(-position, SeekOrigin.End);
                stream.Read(buffer, 0, buffer.Length);

                if (FileEncoding.GetString(buffer) == LineSeparator)
                {
                  if (lineCount == 0)
                  {
                    byte[] veryLastlineBuffer = new byte[stream.Length - stream.Position];
                    stream.Read(veryLastlineBuffer, 0, veryLastlineBuffer.Length);
                    string veryLastLine = FileEncoding.GetString(veryLastlineBuffer);
                    lineCount += veryLastLine.Trim().Length > LineSeparator.Length ? 1 : 0;
                  }
                  else
                  {
                    lineCount++;
                  }

                  if (lineCount == lastNLines)
                  {
                    byte[] returnBuffer = new byte[stream.Length - stream.Position];
                    stream.Read(returnBuffer, 0, returnBuffer.Length);
                    retLines = FileEncoding.GetString(returnBuffer);
                    break;
                  }
                }
              }

              //// Handle case where number of lines in file is less than lastNLines
              if (retLines == null)
              {
                stream.Seek(0, SeekOrigin.Begin);
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                retLines = FileEncoding.GetString(buffer);
              }
            }
          }
        }
        catch (IOException ioEx)
        {
          //// The file is unavailable for reading
          Logger.LogError($"Error when trying to open file {FullName} to get its last {lastNLines} lines.");
          Logger.LogException(ioEx);
        }
        finally
        {
          if (stream != null)
          {
            stream.Close();
            stream.Dispose();
          }
        }
      }

      //// Replace within the returning string the line separator used with the system's line separator if they are not the same
      if (!string.IsNullOrEmpty(retLines) && LineSeparator != Environment.NewLine)
      {
        retLines = retLines.Replace(LineSeparator, Environment.NewLine);
      }

      return retLines;
    }
  }
}
