// Copyright © 2008, 2018, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using MySql.Utility.Classes;

namespace MySql.Data.VisualStudio.DBExport
{
  internal class MySqlDumpFacade
  {
    
    private StringBuilder _dumpOutput;
    private StringBuilder _errorsOutput;
    private StringBuilder _logInfo;
    private StringBuilder _arguments;
    private string _database = string.Empty;
    private string _credentialsFile;
    private List<String> _tables { get; set; }
    private Process _mysqldumpProcess;

    private IVsOutputWindowPane _generalPane;

    internal string _dumpFilePath;
    internal string saveTofilePath { get; set; }

    public StringBuilder LogInfo
    {
      get { return _logInfo; }
    }

    public StringBuilder ErrorsOutput
    {
      get {
        return _errorsOutput;
      }
      set {
          _errorsOutput = value;      
      }
    }

    public StringBuilder DumpOutput
    {
      get {
        return _dumpOutput;
      }
    }

    public MySqlDumpFacade(MySqlDbExportOptions options, string saveToFile, string credentialsFile)
    {
      if (options == null)
        throw new Exception("MySqlDump start options are not valid");

      if (String.IsNullOrEmpty(saveToFile))
        throw new Exception("MySqlDump file Path is not set");

      if (String.IsNullOrEmpty(credentialsFile))
        throw new Exception("Options to start export action are not completed.");

      _credentialsFile = credentialsFile;

      _arguments = new StringBuilder();
      _dumpOutput = new StringBuilder();
      _errorsOutput = new StringBuilder();
      _logInfo = new StringBuilder();

      _dumpFilePath = Utilities.GetMySqlAppInstallLocation("MySQL for Visual Studio");
      if (!String.IsNullOrEmpty(_dumpFilePath))
      {
        _dumpFilePath = System.IO.Path.Combine(_dumpFilePath, @"Dependencies\mysqldump.exe");
      }

      IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
      Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;
      if (outWindow != null)
      {
        outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0);
        outWindow.GetPane(ref generalPaneGuid, out _generalPane);
      }

      _tables = null;

      saveTofilePath = saveToFile;      
      BuildCommandLine(options);
    }

    public MySqlDumpFacade(MySqlDbExportOptions options, string saveToFile, string credentialsFile, List<String> tables) : this(options, saveToFile, credentialsFile)
    {
      _arguments.Append(" --tables ");

      foreach (var table in tables)
      {
        _arguments.AppendFormat(" {0} ", table);
      }

      _tables = tables;
    }

    internal void BuildCommandLine(MySqlDbExportOptions options)
    {
      Type type = options.GetType();

      IList<PropertyInfo> props = new List<PropertyInfo>(type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

      foreach (var prop in props)
      {
        if (prop != null)
        {
          string value = null;
          options.dictionary.TryGetValue(prop, out value);

          if (value == null)
            continue;
          
          var propType = prop.PropertyType;
          switch (propType.ToString())
          {
            case "System.Boolean":
              bool propValue = false;
              propValue = (bool)prop.GetValue(options, null);              
              _arguments.AppendFormat(" {0}{1} ", value, !propValue ? "=false": "");
              break;
            case "System.String":
              string sValue;
              sValue = (string)prop.GetValue(options, null);
              if (!String.IsNullOrEmpty(sValue))
              {
                if (!prop.Name.Equals("database"))
                  _arguments.AppendFormat(" {0}={1}", value, sValue);                  
                else
                  _database = sValue;
              }
              break;
            case "System.Int32":
              int intValue = 0;
              intValue = (int)prop.GetValue(options, null);
              if (prop.Name.Equals("max_allowed_packet", StringComparison.InvariantCultureIgnoreCase))
              {
                if (intValue < 1024)                  
                  _arguments.AppendFormat(" {0}={1}MB", value, intValue.ToString());
                else
                  _arguments.AppendFormat(" {0}=1G", value);
              }
              else
              {
                _arguments.AppendFormat(" {0}={1}", value, intValue.ToString());
              }
              break;
             default:
                break;
          }
        }
      }

      _arguments.Append(" --column_statistics=false ");
      //TODO add a new property to define variables
      // so we can accept a ON/OFF value
      _arguments.Append(" --set-gtid-purged=OFF --protocol=TCP ");
      _arguments.AppendFormat(" \"{0}\"", _database);
    }

    internal void CancelRequest()
    {
      _mysqldumpProcess.Kill();
    }

    internal void ProcessRequest( string outputPath )
    {
      if(String.IsNullOrEmpty(_dumpFilePath))
        throw new Exception(Properties.Resources.MySqlDumpPathNotFound);

      _mysqldumpProcess = new Process();

      _arguments.Append(" --result-file=\"").Append(outputPath).Append('"');
      var startInfo = new ProcessStartInfo
      {
        CreateNoWindow = true,
        WorkingDirectory = Path.GetDirectoryName(_dumpFilePath),
        FileName = "\"" + @_dumpFilePath + "\"",
        Arguments = " --defaults-extra-file=\"" + @_credentialsFile + "\"" + _arguments.ToString(),
        UseShellExecute = false,
        RedirectStandardError = true,
        RedirectStandardOutput = false
      };
      _mysqldumpProcess.StartInfo = startInfo;

      _mysqldumpProcess.ErrorDataReceived += new DataReceivedEventHandler(dumpProcess_ErrorDataReceived);

      AppendToLog(string.Format(Properties.Resources.MySqlDumpStartInfoLog, string.Format("{0:MM/dd/yyyy HH:mm:ss}", DateTime.Now), _database, _tables == null ? "all tables" : string.Join(", ", _tables.ToArray())));

      _mysqldumpProcess.Start();
      _mysqldumpProcess.BeginErrorReadLine();
      _mysqldumpProcess.WaitForExit();
      _mysqldumpProcess.Close();

      AppendToLog(string.Format(Properties.Resources.MySqlDumpRunning, _arguments));

      if (!String.IsNullOrEmpty(_errorsOutput.ToString()))
          AppendToLog(_errorsOutput.ToString().Trim() + ".");

      AppendToLog(string.Format(Properties.Resources.MySqlDumpEndingInfoLog, string.Format("{0:MM/dd/yyyy HH:mm:ss}", DateTime.Now), _database));
    }

    private void AppendToLog(string line)
    {
       _logInfo.AppendLine(line);
       if (_generalPane != null)
       {
         _generalPane.Activate();
         _generalPane.OutputString(Environment.NewLine + line);       
       }
    }

    void dumpProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
      if (!String.IsNullOrEmpty(e.Data))       
        _errorsOutput.AppendLine(e.Data.Trim());      
    }
  }
}
