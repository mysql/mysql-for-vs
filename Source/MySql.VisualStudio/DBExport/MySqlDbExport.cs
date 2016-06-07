// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;

namespace MySql.Data.VisualStudio.DBExport
{
  public class MySqlDbExport
  {
    private string _fileName;
    private IsolatedStorageFile _isoStore;
    private MySqlDumpFacade _mysqlDumpFacade;
    private bool _appendToFile;
    private static Random _rnd = new Random();

    public string OutputFilePath { get; set; }

    public StringBuilder ErrorsOutput { get; private set; }

    public MySqlDbExportOptions ExportOptions { get; }

    public StringBuilder MySqlDumpLog { get; private set; }

    public bool OverwriteFile
    {
      get
      {
        return !_appendToFile;
      }
    }

    private List<string> _tables;

    public MySqlDbExport(MySqlDbExportOptions options, string outputFilePath, MySqlConnection conn, List<string> tables, bool overwriteFile)
    {
      if (options == null)
        throw new Exception("Export options are not valid");

      if (conn == null)
        throw new Exception("Connection is not valid for the Export operation");

      if (string.IsNullOrEmpty(outputFilePath))
        throw new Exception("Path to save dump file is not set.");

      if (tables != null)
      {
        _tables = tables;
      }

      ExportOptions = options;
      OutputFilePath = outputFilePath;

      var connBuilder = new MySqlConnectionStringBuilder(conn.ConnectionString);

      ExportOptions.host = connBuilder.Server;
      ExportOptions.port = (int)connBuilder.Port;
      ExportOptions.database = connBuilder.Database;

      if (connBuilder.SslMode != MySqlSslMode.Required)
        ExportOptions.ssl_cert = connBuilder.CertificateFile;

      _fileName = string.Empty;
      _appendToFile = !overwriteFile;
      CreateIsolatedFile(conn);
    }

    private void CreateIsolatedFile(MySqlConnection conn)
    {
      var connBuilder = new MySqlConnectionStringBuilder(conn.ConnectionString);

      var userId = connBuilder.UserID;
      var pwd = connBuilder.Password;

      _fileName = string.Format("{0}{1}.cnf", userId, _rnd.Next(999, 10000));

      _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (var isoStream = new IsolatedStorageFileStream(_fileName, FileMode.CreateNew, _isoStore))
      {
        using (var writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqld]");
          writer.WriteLine("wait_timeout=1000000");
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=" + userId);
          writer.WriteLine("password=" + pwd);
        }

        _fileName = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream).ToString();
      }
    }

    public void CancelExport()
    {
      if (_mysqlDumpFacade != null)
        _mysqlDumpFacade.CancelRequest();
    }

    public bool Export()
    {
      _mysqlDumpFacade = _tables != null
        ? new MySqlDumpFacade(ExportOptions, OutputFilePath, _fileName, _tables)
        : new MySqlDumpFacade(ExportOptions, OutputFilePath, _fileName);
      _mysqlDumpFacade.ProcessRequest(OutputFilePath);
      _isoStore.DeleteFile(Path.GetFileName(_fileName));
      _isoStore.Close();
      if (_mysqlDumpFacade.ErrorsOutput == null || string.IsNullOrEmpty(_mysqlDumpFacade.ErrorsOutput.ToString()))
      {
        return true;
      }

      ErrorsOutput = _mysqlDumpFacade.ErrorsOutput;
      return false;
    }
  }
}
