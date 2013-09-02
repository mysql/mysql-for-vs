// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("Testfile.cnf", FileMode.CreateNew, _isoStore))
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
        var arguments = mysqldumpFacade.GetType().GetField("_arguments",BindingFlags.NonPublic | BindingFlags.Instance);
        if (arguments != null)
        {
          _isoStore.DeleteFile("Testfile.cnf");
          _isoStore.Close();

          string expected = @"--add-drop-database=false  --add-drop-table=false  --add-drop-trigger=false  --add-locks=false  --all-databases=false  --allow-keywords  --comments=true  --compact=false  --complete-insert=false  --create-options  --default-character-set=utf8 --delayed-insert=false  --disable-keys  --events=false  --extended-insert  --hex-blob=false  --ignore-table=nameoftable --insert-ignore  --lock-tables  --no-data  --max_allowed_packet=1G --port=3305 --quote-names  --replace=false  --routines  """;
          var value = arguments.GetValue(mysqldumpFacade);          
          Assert.True(value.ToString().Contains(expected));          
        }
    }

    [Fact]
    public void CanDumpAListOfDatabases()
    {
        var databases = new List<String>();
        databases.Add("DumpTest");
        databases.Add("SecondTest");
        databases.Add("ThirdTest");
        foreach (var database in databases)
        {
          conn.ConnectionString = st.GetConnectionString("test", "test", false, false);
          conn.ConnectionString += ";database=" + database +";";
          var mysqldump = new MySqlDbExport(options, saveToFile, conn, null);
          mysqldump.Export();
          if (File.Exists(mysqldump.OutputFilePath))
          {
              using (var dump = new StreamReader(mysqldump.OutputFilePath))
              {
                  var content = dump.ReadToEnd();
                  Assert.True(content.Contains(database));
              }
          }
          if (mysqldump.ErrorsOutput != null)
          {
              Assert.False(1 == 1, "CanDumpAListOfDatabases: Test Failed"); 
          }          
        }    
    }

    [Fact]
    public void CanThrowExceptionWhenUnknownDatabase()
    {
      conn.ConnectionString = st.GetConnectionString("root", "", false, false);
      conn.ConnectionString += ";database=unknown;";
      var mysqldump = new MySqlDbExport(options, saveToFile, conn, null);
      mysqldump.Export();
      var errors = mysqldump.ErrorsOutput.ToString();
      Assert.True(errors.Contains("mysqldump: Got error: 1049: Unknown database 'unknown' when selecting the database"));
      Assert.False(File.Exists(saveToFile));
    }


    [Fact]
    public void CanThrowExceptionWhenNoFilePath()
    {      
      Exception ex = Assert.Throws<Exception>(() => (new MySqlDbExport(options, "", conn, null)));
      Assert.Equal("Path to save dump file is not set.", ex.Message);
    }

    [Fact]
    public void CanDoDumpFileForDB()
    {
      conn.ConnectionString = "server=10.159.228.235;userid=root;pwd=toor;database=sakila;";
      //conn.ConnectionString += ";database=DumpTest;";
      //conn.ConnectionString  = st.GetConnectionString("root", "", false, false);
      //conn.ConnectionString += ";database=performance_schema;";
      var mysqldump = new MySqlDbExport(options,saveToFile, conn, null);
      if (mysqldump.Export())
      {
         if (!File.Exists(mysqldump.OutputFilePath))
          Assert.False(1 == 1, "CanDoDumpFileForDB: Creation of dump failed.");
      }                     
    }

    [Fact]
    public void CanLoadConnections()
    {
      var connections = new List<IVsDataExplorerConnection>();      
      //TODO
    }


    public virtual void Dispose()
    {
      if (File.Exists(saveToFile))
          File.Delete(saveToFile);
    }
  }
}
