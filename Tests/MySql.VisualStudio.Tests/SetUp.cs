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
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MySql.VisualStudio.Tests
{
  public class SetUp : IDisposable
  {
    internal const string DEFAULT_HOST = "localhost";
    internal const string DEFAULT_USER = "test";
    internal const string DEFAULT_PASSWORD = "test";
    internal const int DEFAULT_PORT = 3357;
    internal const string DEFAULT_PIPENAME = "MYSQL";
    internal const string DEFAULT_MEMORYNAME = "MYSQL";
    internal const string DEFAULT_DATABASE = "DumpTest";
    internal const string DEFAULT_DATABASE1 = "SecondTest";
    internal const string DEFAULT_DATABASE2 = "ThirdTest";

    public string Host { get; protected set; }
    public string User { get; protected set; }
    public string Password { get; protected set; }
    public int Port { get; protected set; }
    public string PipeName { get; protected set; }
    public string MemoryName { get; protected set; }
    public string DataBase { get; protected set; }
    public string DataBase1 { get; protected set; }
    public string DataBase2 { get; protected set; }

    public SetUp()
    {
      LoadConfiguration();
      CreateData();
    }

    internal virtual void CreateData()
    {
      Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MySql.VisualStudio.Tests.Properties.Setup.sql");
      StreamReader sr = new StreamReader(stream);
      StringBuilder sql = new StringBuilder(sr.ReadToEnd());
      sr.Close();
      sql.Replace("{0}", DataBase);
      sql.Replace("{1}", DataBase1);
      sql.Replace("{2}", DataBase2);
      ExecuteSql(sql.ToString());
    }

    private void LoadConfiguration()
    {
      var appHost = ConfigurationManager.AppSettings["host"];
      var appUser = ConfigurationManager.AppSettings["user"];
      var appPassword = ConfigurationManager.AppSettings["password"];
      var appPort = ConfigurationManager.AppSettings["port"];
      var appPipeName = ConfigurationManager.AppSettings["pipename"];
      var appMemoryName = ConfigurationManager.AppSettings["memory_name"];
      var appDataBase = ConfigurationManager.AppSettings["database"];
      var appDataBase1 = ConfigurationManager.AppSettings["database1"];
      var appDataBase2 = ConfigurationManager.AppSettings["database2"];

      Host = string.IsNullOrEmpty(appHost) ? DEFAULT_HOST : appHost;
      User = string.IsNullOrEmpty(appUser) ? DEFAULT_USER : appUser;
      Password = string.IsNullOrEmpty(appPassword) ? DEFAULT_PASSWORD : appPassword;
      Port = string.IsNullOrEmpty(appPort) ? DEFAULT_PORT : int.Parse(appPort);
      PipeName = string.IsNullOrEmpty(appPipeName) ? DEFAULT_PIPENAME : appPipeName;
      MemoryName = string.IsNullOrEmpty(appMemoryName) ? DEFAULT_MEMORYNAME : appMemoryName;
      DataBase = string.IsNullOrEmpty(appDataBase) ? DEFAULT_DATABASE : appDataBase;
      DataBase1 = string.IsNullOrEmpty(appDataBase) ? DEFAULT_DATABASE1 : appDataBase1;
      DataBase2 = string.IsNullOrEmpty(appDataBase) ? DEFAULT_DATABASE2 : appDataBase2;
    }

    internal string GetConnectionString(string userId, string pw, bool persistSecurityInfo, bool includedb)
    {
      string connStr = string.Format("server={0};user id={1};pooling=false;persist security info={2};connection reset=true;allow user variables=true;port={3};",
          Host,
          userId,
          persistSecurityInfo.ToString().ToLowerInvariant(),
          Port);
      if (pw != null)
        connStr += string.Format(";password={0};", pw);
      if (includedb)
        connStr += string.Format("database={0};", DataBase);
      return connStr;
    }


    public virtual void Dispose()
    {
      var sql = string.Format("DROP DATABASE IF EXISTS {0}; DROP DATABASE IF EXISTS {1}; DROP DATABASE IF EXISTS {2};", DataBase, DataBase1, DataBase2);
      ExecuteSql(sql);
    }

    internal void ExecuteSql(string sql)
    {
      var s = new MySqlScript(new MySqlConnection(GetConnectionString(User, Password, false, false)), sql);
      s.Execute();
    }
  }
}
