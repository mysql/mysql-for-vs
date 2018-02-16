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
using Xunit;
using MySql.VisualStudio;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.MySqlClient;
using System.IO;
using System.Reflection;
using System.IO.IsolatedStorage;
using Microsoft.VisualStudio.Data.Services;
using System.ComponentModel;


namespace MySql.VisualStudio.Tests
{
  public class DbExportTests : IUseFixture<SetUp>, IDisposable
  {
    private SetUp st;
    private MySqlDbExportOptions options;
    private MySqlConnection conn;
    private string saveToFile;    

    public void SetFixture(SetUp data)
    {
      st = data;
      options = new MySqlDbExportOptions();
      conn = new MySqlConnection(st.GetConnectionString("test", "test", false, false));
      saveToFile = Environment.CurrentDirectory + @"\mydump.sql";      
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

      var fullPath = string.Empty;

      var _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, _isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test;");
          writer.WriteLine("password=test;");
        }
        fullPath = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream).ToString();
      }

      var mysqldumpFacade = new MySqlDumpFacade(testOptions, saveToFile, fullPath);
      var arguments = mysqldumpFacade.GetType().GetField("_arguments", BindingFlags.NonPublic | BindingFlags.Instance);
      if (arguments != null)
      {
        _isoStore.DeleteFile("Testfile.cnf");
        _isoStore.Close();

        string expected = @"--add-drop-database  --add-drop-table=false  --add-locks=false  --all-databases=false  --allow-keywords  --comments  --compact=false  --complete-insert=false  --create-options  --databases  --default-character-set=utf8 --disable-keys  --events=false  --extended-insert  --flush-logs=false  --hex-blob=false  --ignore-table=nameoftable --insert-ignore  --lock-tables  --no-data  --no-create-info=false  --max_allowed_packet=1G --order-by-primary=false  --port=3306 --quote-names  --replace=false  --routines  --single-transaction=false  --column_statistics=false  --set-gtid-purged=OFF --protocol=TCP  """;
        var value = arguments.GetValue(mysqldumpFacade);
        Assert.True(value.ToString().Contains(expected));
      }
    }

    [Fact]
    public void CanDumpAListOfDatabases()
    {

      var fullPath = string.Empty;

      var _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, _isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }
        fullPath = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream).ToString();
      }
      
      var databases = new List<String>();
      databases.Add("DumpTest");
      databases.Add("SecondTest");
      databases.Add("ThirdTest");
      foreach (var database in databases)
      {        
        options.database = database;
        options.port = 3305;        
        var mysqldump = new MySqlDumpFacade(options, saveToFile, fullPath);
        mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");
        
        mysqldump.ProcessRequest(saveToFile);

        if (File.Exists(saveToFile))
        {
          using (var dump = new StreamReader(saveToFile))
          {
            var content = dump.ReadToEnd();
            Assert.True(content.Contains(database), "A database is missed in the dump file");
          }
        }
        if (!String.IsNullOrEmpty(mysqldump.ErrorsOutput.ToString()))
        {
          Assert.False(1 == 1, "CanDumpAListOfDatabases: Test Failed");
        }
      }

    }

    [Fact]
    public void CanThrowExceptionWhenUnknownDatabase()
    {

      var fullPath = string.Empty;
      var _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, _isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }
        fullPath = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream).ToString();
      }

      options.database = "unknown";
      options.port = 3305;
      var mysqldump = new MySqlDumpFacade(options, saveToFile, fullPath);
      mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");

      mysqldump.ProcessRequest(saveToFile);

      var errors = mysqldump.ErrorsOutput.ToString();
      Assert.True(errors.Contains("mysqldump: Got error: 1049: Unknown database 'unknown' when selecting the database"));
    }


    [Fact]
    public void CanThrowExceptionWhenNoFilePath()
    {
      Exception ex = Assert.Throws<Exception>(() => (new MySqlDbExport(options, "", conn, null, true)));
      Assert.Equal("Path to save dump file is not set.", ex.Message);
    }

    [Fact]
    public void CanDoDumpFileForDB()
    {

      var fullPath = string.Empty;
      var _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

      using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.OpenOrCreate, _isoStore))
      {
        using (StreamWriter writer = new StreamWriter(isoStream))
        {
          writer.WriteLine("[mysqldump]");
          writer.WriteLine("user=test");
          writer.WriteLine("password=test");
        }
        fullPath = isoStream.GetType().GetField("m_FullPath", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(isoStream).ToString();
      }

      options.database = "DumpTest";
      options.port = 3305;
      var mysqldump = new MySqlDumpFacade(options, saveToFile, fullPath);
      mysqldump._dumpFilePath = Path.GetFullPath(@"..\..\..\..\..\Dependencies\MySql\mysqldump.exe");

      mysqldump.ProcessRequest(saveToFile);

      if (File.Exists(saveToFile))
      {
        using (var dump = new StreamReader(saveToFile))
        {
          var content = dump.ReadToEnd();
          Assert.True(content.Contains("DumpTest"), "A database is missed in the dump file");
        }
      }
    }

    [Fact]
    public void CanLoadConnections()
    {
      var connections = new List<IVsDataExplorerConnection>();
      //TODO
    }

    [Fact]
    public void CanSaveDBExportSettings()
    {

      var databases = new List<String>();
      //pick some db objects      
      databases.Add("DumpTest");
      databases.Add("SecondTest");
      databases.Add("ThirdTest");

      Dictionary<string, BindingList<DbSelectedObjects>> dictionaryToDBObjects = new Dictionary<string, BindingList<DbSelectedObjects>>();
      BindingList<DbSelectedObjects> selectedObjects = null;

      foreach (var database in databases)
      {
        selectedObjects = new BindingList<DbSelectedObjects>();
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
        dictionaryToDBObjects.Add(database, selectedObjects);
      }

      string connectionToUse = conn.ConnectionString;
      MySqlDbExportSaveOptions optionsToSave = new MySqlDbExportSaveOptions(options, saveToFile, dictionaryToDBObjects, connectionToUse);
      optionsToSave.WriteSettingsFile(Environment.CurrentDirectory, "SavedSettings");
      MySqlDbExportSaveOptions.LoadSettingsFile(Environment.CurrentDirectory + @"\SavedSettings.dumps");
    }


    public virtual void Dispose()
    {
      if (File.Exists(saveToFile))
        File.Delete(saveToFile);
      if (File.Exists(Environment.CurrentDirectory + @"\settingsFile.dumps"))
        File.Delete(Environment.CurrentDirectory + @"\settingsFile.dumps");

    }
  }
}
