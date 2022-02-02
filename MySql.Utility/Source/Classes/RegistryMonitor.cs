// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Allows you to monitor specific registry key.
  /// </summary>
  public class RegistryMonitor : IDisposable
  {
    #region Constants

    private const int KEY_NOTIFY = 0x0010;
    private const int KEY_QUERY_VALUE = 0x0001;
    private const int STANDARD_RIGHTS_READ = 0x00020000;

    #endregion Constants

    #region Fields

    private static readonly IntPtr _hkeyClassesRoot;
    private static readonly IntPtr _hkeyCurrentConfig;
    private static readonly IntPtr _hkeyCurrentUser;
    private static readonly IntPtr _hkeyDynData;
    private static readonly IntPtr _hkeyLocalMachine;
    private static readonly IntPtr _hkeyPerformanceData;
    private static readonly IntPtr _hkeyUsers;
    private readonly ManualResetEvent _eventTerminate;
    private readonly object _threadLock;
    private bool _disposed;
    private RegistryChangeNotifyFilter _regFilter;
    private IntPtr _registryHive;
    private string _registrySubName;
    private Thread _thread;
    #endregion Fields

    static RegistryMonitor()
    {
      _hkeyClassesRoot = new IntPtr(unchecked((int)0x80000000));
      _hkeyCurrentConfig = new IntPtr(unchecked((int)0x80000005));
      _hkeyCurrentUser = new IntPtr(unchecked((int)0x80000001));
      _hkeyDynData = new IntPtr(unchecked((int)0x80000006));
      _hkeyLocalMachine = new IntPtr(unchecked((int)0x80000002));
      _hkeyPerformanceData = new IntPtr(unchecked((int)0x80000004));
      _hkeyUsers = new IntPtr(unchecked((int)0x80000003));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
    /// </summary>
    /// <param name="registryKey">The registry key to monitor.</param>
    public RegistryMonitor(RegistryKey registryKey) : this(registryKey.Name)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
    /// </summary>
    /// <param name="fullRegistryKeyName">The full registry key name including the hive.</param>
    public RegistryMonitor(string fullRegistryKeyName) : this()
    {
      if (string.IsNullOrEmpty(fullRegistryKeyName))
      {
        throw new ArgumentNullException(nameof(fullRegistryKeyName));
      }

      InitRegistryKey(fullRegistryKeyName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
    /// </summary>
    /// <param name="registryHive">The registry hive.</param>
    /// <param name="subKey">The sub key.</param>
    public RegistryMonitor(RegistryHive registryHive, string subKey) : this()
    {
      InitRegistryKey(registryHive, subKey);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryMonitor"/> class.
    /// </summary>
    private RegistryMonitor()
    {
      _disposed = false;
      _eventTerminate = new ManualResetEvent(false);
      _regFilter = RegistryChangeNotifyFilter.Any;
      _threadLock = new object();
      WaitBetweenChecks = 0;
    }

    #region Events

    /// <summary>
    /// Occurs when the access to the registry fails.
    /// </summary>
    public event ErrorEventHandler Error;

    /// <summary>
    /// Occurs when the specified registry key has changed.
    /// </summary>
    public event EventHandler RegistryChanged;

    #endregion Events

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this instance is currently monitoring registry changes.
    /// </summary>
    public bool IsMonitoring => _thread != null;

    /// <summary>
    /// Gets or sets the current filter.
    /// </summary>
    public RegistryChangeNotifyFilter RegistryChangeNotifyFilter
    {
      get => _regFilter;
      set
      {
        lock (_threadLock)
        {
          if (IsMonitoring)
          {
            throw new InvalidOperationException(Resources.RegistryMonitorAlreadyRunningError);
          }

          _regFilter = value;
        }
      }
    }

    /// <summary>
    /// The waiting time (in milliseconds) between loop checks for registry changes.
    /// </summary>
    public int WaitBetweenChecks { get; set; }

    #endregion Properties

    /// <summary>
    /// Disposes this object.
    /// </summary>
    public void Dispose()
    {
      Stop();
      _disposed = true;
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Start monitoring.
    /// </summary>
    public void Start()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException(null, Resources.InstanceAlreadyDisposedError);
      }

      lock (_threadLock)
      {
        if (IsMonitoring)
        {
          return;
        }

        _eventTerminate.Reset();
        _thread = new Thread(MonitorThread) { IsBackground = true };
        _thread.Start();
      }
    }

    /// <summary>
    /// Stops the monitoring thread.
    /// </summary>
    public void Stop()
    {
      if (_disposed)
      {
        throw new ObjectDisposedException(null, Resources.InstanceAlreadyDisposedError);
      }

      lock (_threadLock)
      {
        var thread = _thread;
        if (thread == null)
        {
          return;
        }

        _eventTerminate.Set();
        thread.Join();
      }
    }

    /// <summary>
    /// Raises the <see cref="Error"/> event.
    /// </summary>
    /// <param name="e">The <see cref="Exception"/> which occured while watching the registry.</param>
    /// <remarks>
    /// <see cref="OnError"/> is called when an exception occurs while watching the registry.
    /// </remarks>
    protected virtual void OnError(Exception e)
    {
      var handler = Error;
      handler?.Invoke(this, new ErrorEventArgs(e));
    }

    /// <summary>
    /// Raises the <see cref="RegistryChanged"/> event.
    /// </summary>
    /// <remarks>
    /// <see cref="OnRegistryChanged"/> is called when the specified registry key has changed.
    /// </remarks>
    protected virtual void OnRegistryChanged()
    {
      var handler = RegistryChanged;
      handler?.Invoke(this, null);
    }

    [DllImport(DllImportConstants.ADVAPI32, SetLastError = true)]
    private static extern int RegCloseKey(IntPtr hKey);

    [DllImport(DllImportConstants.ADVAPI32, SetLastError = true)]
    private static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree, RegistryChangeNotifyFilter dwNotifyFilter, IntPtr hEvent, bool fAsynchronous);

    [DllImport(DllImportConstants.ADVAPI32, SetLastError = true)]
    private static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int samDesired, out IntPtr phkResult);

    /// <summary>
    /// Initializes the internal registry hive pointer and registry key name.
    /// </summary>
    /// <param name="hive">The registry hive.</param>
    /// <param name="subKeyName">The registry sub key name.</param>
    private void InitRegistryKey(RegistryHive hive, string subKeyName)
    {
      switch (hive)
      {
        case RegistryHive.ClassesRoot:
          _registryHive = _hkeyClassesRoot;
          break;

        case RegistryHive.CurrentConfig:
          _registryHive = _hkeyCurrentConfig;
          break;

        case RegistryHive.CurrentUser:
          _registryHive = _hkeyCurrentUser;
          break;

        case RegistryHive.DynData:
          _registryHive = _hkeyDynData;
          break;

        case RegistryHive.LocalMachine:
          _registryHive = _hkeyLocalMachine;
          break;

        case RegistryHive.PerformanceData:
          _registryHive = _hkeyPerformanceData;
          break;

        case RegistryHive.Users:
          _registryHive = _hkeyUsers;
          break;

        default:
          throw new InvalidEnumArgumentException(nameof(hive), (int)hive, typeof(RegistryHive));
      }

      _registrySubName = subKeyName;
    }

    /// <summary>
    /// Initializes the internal registry hive pointer and registry key name.
    /// </summary>
    /// <param name="fullRegistryKeyName">The full registry key name including the hive.</param>
    private void InitRegistryKey(string fullRegistryKeyName)
    {
      var nameParts = fullRegistryKeyName.Split('\\');
      switch (nameParts[0])
      {
        case "HKEY_CLASSES_ROOT":
        case "HKCR":
          _registryHive = _hkeyClassesRoot;
          break;

        case "HKEY_CURRENT_USER":
        case "HKCU":
          _registryHive = _hkeyCurrentUser;
          break;

        case "HKEY_LOCAL_MACHINE":
        case "HKLM":
          _registryHive = _hkeyLocalMachine;
          break;

        case "HKEY_USERS":
          _registryHive = _hkeyUsers;
          break;

        case "HKEY_CURRENT_CONFIG":
          _registryHive = _hkeyCurrentConfig;
          break;

        default:
          _registryHive = IntPtr.Zero;
          throw new ArgumentException(string.Format(Resources.RegistryMonitorInvalidHiveError, nameParts[0]), nameof(fullRegistryKeyName));
      }

      _registrySubName = string.Join("\\", nameParts, 1, nameParts.Length - 1);
    }

    /// <summary>
    /// Starts the monitor thread.
    /// </summary>
    private void MonitorThread()
    {
      try
      {
        ThreadLoop();
      }
      catch (Exception e)
      {
        OnError(e);
      }
      _thread = null;
    }

    /// <summary>
    /// Loops the monitoring thread until stopped.
    /// </summary>
    private void ThreadLoop()
    {
      var result = RegOpenKeyEx(_registryHive, _registrySubName, 0, STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_NOTIFY, out var registryKey);
      if (result != 0)
      {
        throw new Win32Exception(result);
      }

      try
      {
        var eventNotify = new AutoResetEvent(false);
        var waitHandles = new WaitHandle[] {eventNotify, _eventTerminate};
        while (!_eventTerminate.WaitOne(WaitBetweenChecks, true))
        {
          result = RegNotifyChangeKeyValue(registryKey, true, _regFilter, eventNotify.SafeWaitHandle.DangerousGetHandle(), true);
          if (result != 0)
          {
            throw new Win32Exception(result);
          }

          if (WaitHandle.WaitAny(waitHandles) == 0)
          {
            OnRegistryChanged();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      finally
      {
        if (registryKey != IntPtr.Zero)
        {
          RegCloseKey(registryKey);
        }
      }
    }
  }
}
