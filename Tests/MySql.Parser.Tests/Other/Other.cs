// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Text;
using Antlr.Runtime;
using Xunit;


namespace MySql.Parser.Tests
{
  
  public class Other
  {
    [Fact]
    public void Purge()
    {
      Utility.ParseSql("PURGE BINARY LOGS TO 'mysql-bin.010';", false);
    }

    [Fact]
    public void Purge2()
    {
      Utility.ParseSql("PURGE BINARY LOGS BEFORE '2008-04-02 22:46:26';", false);
    }

    [Fact]
    public void ResetMaster()
    {
      Utility.ParseSql("reset master;", false);
    }

    [Fact]
    public void ChangeMaster()
    {
      Utility.ParseSql(@"STOP SLAVE; -- if replication was running
CHANGE MASTER TO MASTER_PASSWORD='new3cret';
START SLAVE; -- if you want to restart replication
", false);
    }

    [Fact]
    public void ChangeMaster2()
    {
      Utility.ParseSql("CHANGE MASTER TO IGNORE_SERVER_IDS = ();;", false);
    }

    [Fact]
    public void ChangeMaster3()
    {
      Utility.ParseSql("CHANGE MASTER TO IGNORE_SERVER_IDS = ( 1, 3 );", false);
    }

    [Fact]
    public void ChangeMaster4()
    {
      Utility.ParseSql(
        @"CHANGE MASTER TO
  MASTER_HOST='master2.mycompany.com',
  MASTER_USER='replication',
  MASTER_PASSWORD='bigs3cret',
  MASTER_PORT=3306,
  MASTER_LOG_FILE='master2-bin.001',
  MASTER_LOG_POS=4,
  MASTER_CONNECT_RETRY=10;
;", false);
    }

    [Fact]
    public void ChangeMaster5()
    {
      Utility.ParseSql(
        @"CHANGE MASTER TO
  RELAY_LOG_FILE='slave-relay-bin.006',
  RELAY_LOG_POS=4025;", false);
    }

    [Fact]
    public void ChangeMaster6()
    {
      Utility.ParseSql(
        @"CHANGE MASTER TO
  RELAY_LOG_FILE='myhost-bin.153',
  RELAY_LOG_POS=410,
  MASTER_HOST='some_dummy_string';
", false);
    }

    [Fact]
    public void StartSlave()
    {
      Utility.ParseSql("START SLAVE SQL_THREAD;", false);
    }

    [Fact]
    public void LoadData51()
    {
      Utility.ParseSql("LOAD DATA FROM MASTER", false, new Version( 5, 1 ));
    }

    [Fact]
    public void LoadData55()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD DATA FROM MASTER", true, out sb, new Version( 5, 5 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'FROM'") != -1);
    }

    [Fact]
    public void LoadTable51()
    {
      Utility.ParseSql("LOAD TABLE tbl_name FROM MASTER", false, new Version( 5, 1 ));
    }

    [Fact]
    public void LoadTable55()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD TABLE tbl_name FROM MASTER", true, out sb, new Version( 5, 5 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'TABLE'") != -1);
    }

    [Fact]
    public void ResetSlave()
    {
      Utility.ParseSql("reset slave;", false);
    }

    [Fact]
    public void SqlSlaveSkipCounter()
    {
      Utility.ParseSql("SET GLOBAL sql_slave_skip_counter = 200", false);
    }

    [Fact]
    public void StartSlave2()
    {
      Utility.ParseSql("START SLAVE UNTIL MASTER_LOG_FILE='/tmp/log', MASTER_LOG_POS=101;", false);
    }

    [Fact]
    public void StopSlave()
    {
      Utility.ParseSql("stop slave;", false);
    }

    [Fact]
    public void StopSlave2()
    {
      Utility.ParseSql("stop slave IO_THREAD;", false);
    }

    [Fact]
    public void AnalyzeLocal()
    {
      Utility.ParseSql("analyze local table tbl1;", false);
    }

    [Fact]
    public void AnalyzeNoWrite()
    {
      Utility.ParseSql("analyze no_write_to_binlog table `tab1`, `tab2`, tab3;", false);
    }

    [Fact]
    public void BackupTable51()
    {
      Utility.ParseSql("BACKUP TABLE tbl_name, tbl_name TO '/path/to/backup/directory';", false, new Version( 5, 1 ));
    }

    [Fact]
    public void BackupTable55()
    {
      StringBuilder sb;
      Utility.ParseSql("BACKUP TABLE tbl_name, tbl_name TO '/path/to/backup/directory';", true, out sb, new Version( 5, 5 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'BACKUP'") != -1);
    }

    [Fact]
    public void RestoreTable51()
    {
      StringBuilder sb;
      Utility.ParseSql("RESTORE TABLE tbl_name, tbl_name FROM '/path/to/backup/directory';", false, out sb, new Version( 5, 1 ));
    }

    [Fact]
    public void RestoreTable55()
    {
      StringBuilder sb;
      Utility.ParseSql("RESTORE TABLE tbl_name, tbl_name FROM '/path/to/backup/directory';", true, out sb, new Version( 5, 5 ));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'RESTORE'") != -1);
    }

    [Fact]
    public void CheckTable()
    {
      Utility.ParseSql("CHECK TABLE test_table FAST QUICK;", false);
    }

    [Fact]
    public void Checksum()
    {
      Utility.ParseSql("checksum table tab1 quick;", false);
    }

    [Fact]
    public void Checksum2()
    {
       Utility.ParseSql("checksum table tab1, tab3 extended", false);
    }

    [Fact]
    public void Optimize()
    {
      Utility.ParseSql("OPTIMIZE TABLE foo;", false);
    }

    [Fact]
    public void Binlog()
    {
      Utility.ParseSql("binlog 'x';", false);
    }

    [Fact]
    public void CacheIndex()
    {
      Utility.ParseSql("CACHE INDEX t1, t2, t3 IN hot_cache;", false);
    }

    [Fact]
    public void CacheIndex2()
    {
      Utility.ParseSql("CACHE INDEX t1 IN non_existent_cache;", false);
    }

    [Fact]
    public void CacheIndexPartition51_1()
    {
      StringBuilder sb;
      Utility.ParseSql("CACHE INDEX pt PARTITION (p0) IN kc_fast;", true, out sb, new Version( 5, 1 ));
      Assert.True( sb.ToString().IndexOf( "'partition'", StringComparison.OrdinalIgnoreCase ) != -1 );
    }

    [Fact]
    public void CacheIndexPartition55_1()
    {
      Utility.ParseSql("CACHE INDEX pt PARTITION (p0) IN kc_fast;", false, new Version( 5, 5 ));
    }

    [Fact]
    public void CacheIndexPartition55_2()
    {
      Utility.ParseSql("CACHE INDEX pt PARTITION (p1, p3) IN kc_slow;", false, new Version(5, 5));
    }

    [Fact]
    public void CacheIndexPartition55_3()
    {
      Utility.ParseSql("CACHE INDEX pt PARTITION (ALL) IN kc_all;", false, new Version(5, 5));
    }

    [Fact]
    public void Flush()
    {
      Utility.ParseSql("flush logs;", false);
    }

    [Fact]
    public void Flush2()
    {
      Utility.ParseSql("flush tables;", false);
    }

    [Fact]
    public void Kill()
    {
      Utility.ParseSql("kill connection 20;", false);
    }

    [Fact]
    public void Load()
    {
      Utility.ParseSql("LOAD INDEX INTO CACHE t1, t2 IGNORE LEAVES;", false);
    }

    [Fact]
    public void LoadPartition_1_51()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD INDEX INTO CACHE pt PARTITION (p0);", true, out sb, new Version(5, 1));
      Assert.True(sb.ToString().IndexOf("partition", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void LoadPartition_1_55()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD INDEX INTO CACHE pt PARTITION (p0);", false, out sb, new Version(5, 5));
    }

    [Fact]
    public void LoadPartition_2_55()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD INDEX INTO CACHE pt PARTITION (p1, p3);", false, out sb, new Version(5, 5));
    }

    [Fact]
    public void LoadPartition_3_55()
    {
      StringBuilder sb;
      Utility.ParseSql("LOAD INDEX INTO CACHE pt PARTITION (ALL);", false, out sb, new Version(5, 5));
    }

    [Fact]
    public void Reset()
    {
      Utility.ParseSql("RESET QUERY CACHE;", false);
    }

    [Fact]
    public void CreateUdf()
    {
      Utility.ParseSql("create aggregate function HyperbolicSum returns real soname 'libso';", false);
    }

    [Fact]
    public void InstallPlugin()
    {
      Utility.ParseSql("install plugin myplugin soname 'libmyplugin.so';", false);
    }

    [Fact]
    public void UninstallPlugin()
    {
      Utility.ParseSql("uninstall plugin myplugin;", false);
    }

    [Fact]
    public void LoadXml_51()
    {
      StringBuilder sb;
      Utility.ParseSql(
        @"
LOAD XML LOCAL INFILE 'person.xml'
       INTO TABLE person
       ROWS IDENTIFIED BY '<person>';", true, out sb, new Version(5, 1));
      Assert.True(sb.ToString().IndexOf("xml", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void LoadXml_1_55()
    {
      Utility.ParseSql(
        @"
LOAD XML LOCAL INFILE 'person.xml'
       INTO TABLE person
       ROWS IDENTIFIED BY '<person>';", false, new Version(5, 5));
    }

    [Fact]
    public void LoadXml_2_55()
    {
      Utility.ParseSql(
        @"
LOAD XML LOCAL INFILE 'person-dump.xml'
    INTO TABLE person2;", false, new Version(5, 5));
    }

    [Fact]
    public void ValidCharacterIdenfier_SelectTable()
    {
      string input = "SELECT $TABLE.* FROM $TABLE;";
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(input, false, new Version(5, 1));
      Assert.Equal(input, r.Start.InputStream.ToString());
    }

    [Fact]
    //Cyrillic Characters (Russian)
    public void ValidCharacterIdenfier_SelectColumn()
    {
      string input = "SELECT $TABLE.ЌФѬыь FROM $TABLE;";
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(input, false, new Version(5, 1));
      Assert.Equal(input, r.Start.InputStream.ToString());
    }

    [Fact]
    //Greek Characters
    public void ValidCharacterIdenfier_CreateTable()
    {
      string input = "CREATE TABLE ηΧΨΩιθη (ID int);";
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(input, false, new Version(5, 1));
      Assert.Equal(input, r.Start.InputStream.ToString());
    }

    [Fact]
    //Amenian Characters (Table)
    //CJK Characters (Old Column) -> CJK=China-Japan-Korea
    //Iragana Characters (New Column)
    public void ValidCharacterIdentifier_AlterColumn()
    {
      string input = "ALTER TABLE թպոնմճ CHANGE 坄坅坆均 ウオガギ INT;";
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(input, false, new Version(5, 1));
      Assert.Equal(input, r.Start.InputStream.ToString());
    }
  }
}
