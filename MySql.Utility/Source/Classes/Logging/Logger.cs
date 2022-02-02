// Copyright (c) 2018, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Diagnostics;
using System.Linq;
using MySql.Utility.Forms;

namespace MySql.Utility.Classes.Logging
{
  /// <summary>
  /// Defines functionality to log messages and exceptions to a log file.
  /// </summary>
  public sealed class Logger
  {
    #region Fields

    private static readonly Lazy<Logger> _lazyLogger = new Lazy<Logger>(() => new Logger());
    private static bool _appendLogsToHome;
    private static bool _consoleMode;
    private static bool _logToConsole;
    private static string _homeDirectory;
    private static string _logFileBaseName;
    private static string _traceSourceName;
    private static bool _writeTimeStamp;

    #endregion Fields

    /// <summary>
    /// Initializes the <see cref="Logger"/> class.
    /// </summary>
    static Logger()
    {
      _appendLogsToHome = true;
      _consoleMode = false;
      _logToConsole = false;
      _homeDirectory = null;
      _logFileBaseName = null;
      MaxLogCount = 100;
      PrependUserNameToLogFileName = false;
      _writeTimeStamp = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    private Logger()
    {
      if (string.IsNullOrEmpty(_homeDirectory)
          || !string.IsNullOrEmpty(Utilities.ValidateFilePath(_homeDirectory))
          || string.IsNullOrEmpty(_logFileBaseName))
      {
        throw new LoggerUndefinedLogFileException();
      }

      Source = new TraceSource(TraceSourceName)
      {
        Switch = new SourceSwitch("sourceSwitch")
        {
          Level = SourceLevels.All
        }
      };
      if (ConsoleMode)
      {
        const string listenerName = "mysql-console";
        Source.Listeners.Add(LogToConsole
                             ? (TraceListener) new ConsoleTraceListener() { Name = listenerName }
                             : (TraceListener) new LoggerListener() { Name = listenerName });
      }
      else
      {
        Source.Listeners.Add(new LoggerListener() { Name = "mysql-gui" });
      }
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether a Logs folder is appended to the home path.
    /// </summary>
    public static bool AppendLogsToHome
    {
      get { return _appendLogsToHome; }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _appendLogsToHome = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the logging is done in console or UI mode.
    /// </summary>
    public static bool ConsoleMode
    {
      get { return _consoleMode; }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _consoleMode = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the logging is done to the console or to the Installer log.
    /// </summary>
    public static bool LogToConsole
    {
      get { return _logToConsole; }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _logToConsole = value;
      }
    }

    /// <summary>
    /// Gets or sets the number of seconds to automatically close the error dialog.
    /// If 0 or less it means the dialog is never closed.
    /// </summary>
    public static int ErrorDialogAutoCloseSeconds { get; set; }

    /// <summary>
    /// Gets or sets the home directory where logs are going to be stored.
    /// </summary>
    public static string HomeDirectory
    {
      get { return _homeDirectory; }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _homeDirectory = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Logger"/> has been initialized with its required values.
    /// </summary>
    public static bool Initialized => !string.IsNullOrEmpty(_homeDirectory)
                                      && !string.IsNullOrEmpty(_logFileBaseName)
                                      && !string.IsNullOrEmpty(_traceSourceName);

    /// <summary>
    /// Gets the singleton instance of this class.
    /// </summary>
    public static Logger Instance => _lazyLogger.Value;

    /// <summary>
    /// Gets or sets the base name of the log file, WITHOUT the extension.
    /// A .log extension will always be given.
    /// </summary>
    public static string LogFileBaseName
    {
      get
      {
        return _logFileBaseName;
      }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _logFileBaseName = value;
      }
    }

    /// <summary>
    /// Gets or sets the maximum number of log files that will be kept.
    /// </summary>
    public static int MaxLogCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user name is prepended to the log file name.
    /// </summary>
    /// <remarks>This may be set to <c>true</c> to avoid trouble when a computer is accessed by other users at the same time (e.g. via Switch User or via terminal services).</remarks>
    public static bool PrependUserNameToLogFileName { get; set; }

    /// <summary>
    /// Gets or sets the name assigned to the <seealso cref="TraceSource"/>.
    /// </summary>
    public static string TraceSourceName
    {
      get
      {
        return _traceSourceName;
      }

      set
      {
        if (_lazyLogger.IsValueCreated)
        {
          throw new LoggerPropertySetException();
        }

        _traceSourceName = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a datetime is written along with error messages output to the log.
    /// </summary>
    public static bool WriteTimeStamp
    {
      get
      {
        return _writeTimeStamp;
      }

      set
      {
        _writeTimeStamp = value;
        SetTraceListenerOption(TraceOptions.DateTime, value);
      }
    }

    /// <summary>
    /// Gets or sets the <seealso cref="TraceSource"/>.
    /// </summary>
    private TraceSource Source { get; }

    #endregion Properties

    /// <summary>
    /// Initializes the <see cref="Logger"/> with its required properties.
    /// </summary>
    /// <param name="homeDirectory">The home directory where logs are going to be stored.</param>
    /// <param name="logFileBaseName">The base name of the log file, WITHOUT the extension.</param>
    /// <param name="consoleMode">Flag indicating whether the logging is done in console or UI mode.</param>
    /// <param name="logToConsole">Flag indicating whether the logging output goes to the console or to the Installer log.
    /// This argument is only relevant when <paramref name="consoleMode"/> is set to <c>true</c>.</param>
    /// <param name="traceSourceName">The name assigned to the <seealso cref="TraceSource"/>.</param>
    public static void Initialize(string homeDirectory, string logFileBaseName, bool consoleMode, bool logToConsole, string traceSourceName)
    {
      if (Initialized)
      {
        return;
      }

      _consoleMode = consoleMode;
      _logToConsole = logToConsole;
      _homeDirectory = homeDirectory;
      _logFileBaseName = logFileBaseName;
      _traceSourceName = traceSourceName;
    }

    /// <summary>
    /// Writes an error message to the log file.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the error is shown to users.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogError(string message, bool displayOnUserInterface = false)
    {
      LogEvent(TraceEventType.Error, 50, message, displayOnUserInterface);
    }

    /// <summary>
    /// Writes an exception to the log file.
    /// </summary>
    /// <param name="exception"><see cref="Exception"/> for which to log an error.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the message is shown to users.</param>
    /// <param name="errorMessage">A custom error message.</param>
    /// <param name="errorTitle">The title displayed on the error dialog.</param>
    /// <param name="unhandled">Flag indicating if the exception was not properly handled.</param>
    /// <param name="useInnerException">Flag indicating whether the information of an <see cref="Exception.InnerException"/> is used if possible, otherwise use always the topmost exception.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogException(Exception exception, bool displayOnUserInterface = false, string errorMessage = null, string errorTitle = null, bool unhandled = false, bool useInnerException = true)
    {
      if (exception == null)
      {
        return;
      }

      var emptyErrorMessage = string.IsNullOrEmpty(errorMessage);
      var emptyErrorTitle = string.IsNullOrEmpty(errorTitle);
      if (!displayOnUserInterface
          && emptyErrorMessage
          && emptyErrorTitle)
      {
        LogExceptionSimple(exception, unhandled, useInnerException);
        return;
      }

      if (emptyErrorTitle)
      {
        errorTitle = unhandled
          ? Resources.UnhandledExceptionText
          : Resources.ErrorText;
      }

      var callingMethod = new StackFrame(1).GetMethod();
      var declaringType = callingMethod.DeclaringType;
      var shownException = useInnerException && exception.InnerException != null ? exception.InnerException : exception;
      var message = string.Format(Resources.ApplicationExceptionWithCustomMessageForLog,
                                  emptyErrorMessage ? (unhandled ? Resources.UnhandledExceptionText : Resources.ApplicationExceptionText) : errorMessage,
                                  declaringType?.Name ?? "Unknown",
                                  callingMethod.Name,
                                  shownException.Message);
      var traceEventType = unhandled ? TraceEventType.Critical : TraceEventType.Error;
      LogEvent(traceEventType, unhandled ? 100 : 50, message, false);
      if (!displayOnUserInterface)
      {
        return;
      }

      if (!ConsoleMode)
      {
        string exceptionMoreInfo = string.Format(Resources.ApplicationExceptionForMoreInfo, declaringType?.Name ?? "Unknown", callingMethod.Name, shownException.Message, shownException.StackTrace);
        var infoProperties = InfoDialogProperties.GetErrorDialogProperties(errorTitle, errorMessage, shownException.Message, exceptionMoreInfo);
        infoProperties.FitTextStrategy = InfoDialog.FitTextsAction.IncreaseDialogWidth;
        infoProperties.WordWrapMoreInfo = false;
        if (ErrorDialogAutoCloseSeconds > 0)
        {
          infoProperties.CommandAreaProperties.DefaultButton = InfoDialog.DefaultButtonType.Button1;
          infoProperties.CommandAreaProperties.DefaultButtonTimeout = ErrorDialogAutoCloseSeconds;
        }

        InfoDialog.ShowDialog(infoProperties);
      }
      else
      {
        Console.WriteLine($@"{traceEventType} : message");
      }
    }

    /// <summary>
    /// Writes an exception to the log file.
    /// </summary>
    /// <param name="exception"><see cref="Exception"/> for which to log an error.</param>
    /// <param name="unhandled">Flag indicating if the exception was not properly handled.</param>
    /// <param name="useInnerException">Flag indicating whether the information of an <see cref="Exception.InnerException"/> is used if possible, otherwise use always the topmost exception.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogExceptionSimple(Exception exception, bool unhandled = false, bool useInnerException = true)
    {
      if (exception == null)
      {
        return;
      }

      var shownException = useInnerException && exception.InnerException != null ? exception.InnerException : exception;
      var callingMethod = new StackFrame(1).GetMethod();
      var declaringType = callingMethod.DeclaringType;
      var message = string.Format(Resources.ApplicationExceptionForLog,
                                  shownException.Message,
                                  declaringType?.Name ?? "Unknown",
                                  callingMethod.Name);
      LogEvent(unhandled ? TraceEventType.Critical : TraceEventType.Error, unhandled ? 100 : 50, message, false);
    }

    /// <summary>
    /// Writes an informational message to the log file.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the message is shown to users.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogInformation(string message, bool displayOnUserInterface = false)
    {
      LogEvent(TraceEventType.Information, 10, message, displayOnUserInterface);
    }

    /// <summary>
    /// Writes a verbose message to the log file.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the error is shown to users.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogVerbose(string message, bool displayOnUserInterface = false)
    {
      LogEvent(TraceEventType.Verbose, 05, message, displayOnUserInterface);
    }

    /// <summary>
    /// Writes a warning message to the log file.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the error is shown to users.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    public static void LogWarning(string message, bool displayOnUserInterface = false)
    {
      LogEvent(TraceEventType.Warning, 15, message, displayOnUserInterface);
    }

    /// <summary>
    /// Writes a message to the log file.
    /// </summary>
    /// <param name="type">A <see cref="TraceEventType"/> to identify the severity of the error.</param>
    /// <param name="id">Numeric identifier for the message.</param>
    /// <param name="message">Message text.</param>
    /// <param name="displayOnUserInterface">Flag indicating whether the message is shown to users.</param>
    /// <remarks>The <seealso cref="LogFileBaseName"/> and <seealso cref="HomeDirectory"/> properties must be set.</remarks>
    private static void LogEvent(TraceEventType type, int id, string message, bool displayOnUserInterface)
    {
      if (string.IsNullOrEmpty(message))
      {
        return;
      }

      Instance.Source.TraceEvent(type, id, message);
      Instance.Source.Flush();
      if (!displayOnUserInterface)
      {
        return;
      }

      var errorTitle = type == TraceEventType.Critical ? Resources.HighSeverityError : type.ToString();
      if (!ConsoleMode)
      {
        var infoType = type == TraceEventType.Critical || type == TraceEventType.Error
          ? InfoDialog.InfoType.Error
          : (type == TraceEventType.Warning
              ? InfoDialog.InfoType.Warning
              : InfoDialog.InfoType.Info);
        var infoProperties = InfoDialogProperties.GetInfoDialogProperties(infoType, CommandAreaProperties.ButtonsLayoutType.OkOnly, errorTitle, message);
        infoProperties.FitTextStrategy = InfoDialog.FitTextsAction.IncreaseDialogWidth;
        if (ErrorDialogAutoCloseSeconds > 0)
        {
          infoProperties.CommandAreaProperties.DefaultButton = InfoDialog.DefaultButtonType.Button1;
          infoProperties.CommandAreaProperties.DefaultButtonTimeout = ErrorDialogAutoCloseSeconds;
        }

        InfoDialog.ShowDialog(infoProperties);
      }
      else
      {
        Console.WriteLine($@"{errorTitle} : message");
      }
    }

    /// <summary>
    /// Sets or unsets the given <see cref="TraceOptions"/> to the log <see cref="TraceListener"/>.
    /// </summary>
    /// <param name="option">A <see cref="TraceOptions"/> flag.</param>
    /// <param name="set">A flag indicating if the flag is set or unset.</param>
    private static void SetTraceListenerOption(TraceOptions option, bool set)
    {
      if (!_lazyLogger.IsValueCreated)
      {
        return;
      }

      foreach (var listener in _lazyLogger.Value.Source.Listeners.Cast<TraceListener>().Where(listener => listener.Name.IndexOf("mysql", StringComparison.OrdinalIgnoreCase) >= 0))
      {
        if (set)
        {
          listener.TraceOutputOptions |= option;
        }
        else
        {
          listener.TraceOutputOptions &= ~option;
        }
      }
    }
  }
}
