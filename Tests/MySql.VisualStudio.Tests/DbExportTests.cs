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
using Xunit;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.MySqlClient;
using System.IO;
using System.Reflection;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using MySql.Utility.Tests;

namespace MySql.VisualStudio.Tests
{
  public class DbExportTests : IUseFixture<SetUpSqlData>, IDisposable
  {
    private SetUpSqlData _st;
    private MySqlDbExportOptions _options;
    private MySqlConnection _conn;
    private string _saveToFile;    

    public void SetFixture(SetUpSqlData data)
    {
      _st = data;
      _options = new MySqlDbExportOptions();
      _conn = new MySqlConnection(SetUpDatabaseTestsBase.GetConnectionString(_st.HostName, _st.Port, "test", "test", false, null));
      _saveToFile = Environment.CurrentDirectory + @"\mydump.sql";      
    }

    [Fact]
    public void CanLoadAllOptions()
    {
      var testOptions = new MySqlDbExportOptions();
      testOptions.no_data = true;
      testOptions.insert_ignore = true;
      testOptions.ignore_table = "nameoftable";
      testOptions.routines = true;
      testOptions.lock_tables = true;
      testOptions.port = 3305;
      testOptions.allow_keywords = true;

      string fullPath;

      var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test;");
          writer.WriteLine("password=test;");
        }

        var fieldInfo = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.True(fieldInfo != null);
        fullPath = fieldInfo.GetValue(isoStream).ToString();
      }

      var mysqldumpFacade = new MySqlDumpFacade(testOptions, _saveToFile, fullPath);
      var arguments = mysqldumpFacade.GetType().GetField("_arguments", BindingFlags.NonPublic | BindingFlags.Instance);
      if (arguments != null)
      {
        isoStore.DeleteFile("Testfile.cnf");
        isoStore.Close();

        string expected = @"--add-drop-database  --add-drop-table  --add-locks=false  --all-databases=false  --allow-keywords  --comments  --compact=false  --complete-insert=false  --create-options  --databases  --default-character-set=utf8 --delayed-insert=false  --disable-keys  --events=false  --extended-insert  --flush-logs=false  --hex-blob=false  --ignore-table=nameoftable --insert-ignore  --lock-tables  --no-data  --no-create-info=false  --max_allowed_packet=1G --order-by-primary=false  --port=3305 --quote-names  --replace=false  --routines  --single-transaction=false  --set-gtid-purged=OFF  """;
        var value = arguments.GetValue(mysqldumpFacade);
        Assert.True(value.ToString().Contains(expected));
      }
    }

    [Fact]
    public void CanDumpAListOfDatabases()
    {

      string fullPath;

      var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }

        var fieldInfo = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.True(fieldInfo != null);
        fullPath = fieldInfo.GetValue(isoStream).ToString();
      }
      
      var databases = new List<string>();
      databases.Add("DumpTest");
      databases.Add("SecondTest");
      databases.Add("ThirdTest");
      foreach (var database in databases)
      {        
        _options.database = database;
        _options.port = 3305;        
        var mysqldump = new MySqlDumpFacade(_options, _saveToFile, fullPath);
        mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");
        
        mysqldump.ProcessRequest(_saveToFile);

        if (File.Exists(_saveToFile))
        {
          using (var dump = new StreamReader(_saveToFile))
          {
            var content = dump.ReadToEnd();
            Assert.True(content.Contains(database), "A database is missed in the dump file");
          }
        }
        if (!string.IsNullOrEmpty(mysqldump.ErrorsOutput.ToString()))
        {
          Assert.False(true, "CanDumpAListOfDatabases: Test Failed");
        }
      }

    }

    [Fact]
    public void CanThrowExceptionWhenUnknownDatabase()
    {

      string fullPath;
      var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }

        var fieldInfo = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.True(fieldInfo != null);
        fullPath = fieldInfo.GetValue(isoStream).ToString();
      }

      _options.database = "unknown";
      _options.port = 3305;
      var mysqldump = new MySqlDumpFacade(_options, _saveToFile, fullPath);
      mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");

      mysqldump.ProcessRequest(_saveToFile);
      
      var errors = mysqldump.ErrorsOutput.ToString();
      Assert.True(errors.Contains("mysqldump: Got error: 1044: Access denied for user 'test'@'localhost' to database 'unknown' when selecting the database"));      
    }


    [Fact]
    public void CanThrowExceptionWhenNoFilePath()
    {
      Exception ex = Assert.Throws<Exception>(() => (new MySqlDbExport(_options, "", _conn, null, true)));
      Assert.Equal("Path to save dump file is not set.", ex.Message);
    }

    [Fact]
    public void CanDoDumpFileForDb()
    {

      string fullPath;
      var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }

        var fieldInfo = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.True(fieldInfo != null);
        fullPath = fieldInfo.GetValue(isoStream).ToString();
      }

      _options.database = "DumpTest";
      _options.port = 3305;
      var mysqldump = new MySqlDumpFacade(_options, _saveToFile, fullPath);
      mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");

      mysqldump.ProcessRequest(_saveToFile);

      if (File.Exists(_saveToFile))
      {
        using (var dump = new StreamReader(_saveToFile))
        {
          var content = dump.ReadToEnd();
          Assert.True(content.Contains("DumpTest"), "A database is missed in the dump file");
        }
      }
    }

    //[Fact]
    //public void CanLoadConnections()
    //{
    //  //var connections = new List<IVsDataExplorerConnection>();
    //  //TODO
    //}

    [Fact]
    public void CanSaveDbExportSettings()
    {

      var databases = new List<string>();
      //pick some db objects      
      databases.Add("DumpTest");
      databases.Add("SecondTest");
      databases.Add("ThirdTest");

      Dictionary<string, BindingList<DbSelectedObjects>> dictionaryToDbObjects = new Dictionary<string, BindingList<DbSelectedObjects>>();

      foreach (var database in databases)
      {
        var selectedObjects = new BindingList<DbSelectedObjects>();
        DbSelectedObjects objectTable;

        switch (database)
        {
          case "DumpTest":
            objectTable = new DbSelectedObjects("items", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);

            objectTable = new DbSelectedObjects("stores", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);

            objectTable = new DbSelectedObjects("employees", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);
            break;
          case "SecondTest":
            objectTable = new DbSelectedObjects("stuff", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);

            objectTable = new DbSelectedObjects("mylines", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);

            objectTable = new DbSelectedObjects("products", DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);
            break;
          case "ThirdTest":
            objectTable = new DbSelectedObjects("brands",DbObjectKind.Table, true);
            selectedObjects.Add(objectTable);
            break;
        }
        dictionaryToDbObjects.Add(database, selectedObjects);
      }

      string connectionToUse = _conn.ConnectionString;
      MySqlDbExportSaveOptions optionsToSave = new MySqlDbExportSaveOptions(_options, _saveToFile, dictionaryToDbObjects, connectionToUse);
      optionsToSave.WriteSettingsFile(Environment.CurrentDirectory, "SavedSettings");
      MySqlDbExportSaveOptions.LoadSettingsFile(Environment.CurrentDirectory + @"\SavedSettings.dumps");
    }


    public virtual void Dispose()
    {
      if (File.Exists(_saveToFile))
        File.Delete(_saveToFile);
      if (File.Exists(Environment.CurrentDirectory + @"\settingsFile.dumps"))
        File.Delete(Environment.CurrentDirectory + @"\settingsFile.dumps");

    }
  }
}
