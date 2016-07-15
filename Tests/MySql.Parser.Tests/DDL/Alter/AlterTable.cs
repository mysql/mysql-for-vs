// Copyright © 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Windows.Forms;
using Xunit;

namespace MySql.Parser.Tests.DDL.Alter
{
  public class AlterTable
  {
    [Fact]
    public void Engine()
    {
      Utility.ParseSql(@"ALTER TABLE t1 ENGINE = InnoDB;");
    }

    [Fact]
    public void AutoIncrement()
    {
      Utility.ParseSql(@"ALTER TABLE t2 AUTO_INCREMENT = 2;");
    }

    [Fact]
    public void DropColumn()
    {
      Utility.ParseSql(@"ALTER TABLE t2 DROP COLUMN c, DROP COLUMN d;");
    }

    [Fact]
    public void ChangeColumn()
    {
      Utility.ParseSql(@"ALTER TABLE t1 CHANGE a b INTEGER;");
    }

    [Fact]
    public void ChangeColumn2()
    {
      Utility.ParseSql(@"ALTER TABLE t1 CHANGE b b BIGINT NOT NULL;");
    }

    [Fact]
    public void ModifyColumn()
    {
      Utility.ParseSql(@"ALTER TABLE t1 MODIFY b BIGINT NOT NULL;");
    }

    [Fact]
    public void ModifyColumn2()
    {
      Utility.ParseSql(@"ALTER TABLE t1 MODIFY col1 BIGINT UNSIGNED DEFAULT 1 COMMENT 'my column';");
    }

    [Fact]
    public void ForeignKey()
    {
      Utility.ParseSql(@"ALTER TABLE tbl_name DROP FOREIGN KEY fk_symbol;");
    }

    [Fact]
    public void DiscardTablespace()
    {
      Utility.ParseSql(@"ALTER TABLE tbl_name DISCARD TABLESPACE;");
    }

    [Fact]
    public void ImportTablespace()
    {
      Utility.ParseSql(@"ALTER TABLE tbl_name IMPORT TABLESPACE;");
    }

    [Fact]
    public void ConvertCharacter()
    {
      Utility.ParseSql(@"ALTER TABLE tbl_name CONVERT TO CHARACTER SET charset_name;");
    }

    [Fact]
    public void ModifyChar()
    {
      Utility.ParseSql(@"ALTER TABLE t MODIFY latin1_text_col TEXT CHARACTER SET utf8;");
    }

    [Fact]
    public void ModifyColumn3()
    {
      Utility.ParseSql(@"ALTER TABLE t1 CHANGE c1 c1 BLOB;");
    }

    [Fact]
    public void ModifyColumn4()
    {
      Utility.ParseSql(@"ALTER TABLE t1 CHANGE c1 c1 TEXT CHARACTER SET utf8;");
    }

    [Fact]
    public void DefaultCharset()
    {
      Utility.ParseSql(@"ALTER TABLE tbl_name DEFAULT CHARACTER SET charset_name;");
    }

    [Fact]
    public void ChangeColumn3()
    {
      Utility.ParseSql(@"alter table Temp_Table change column ID ID int unsigned;");
    }

    [Fact]
    public void ConvertCharacter2()
    {
      Utility.ParseSql(@"ALTER TABLE tablename CONVERT TO CHARACTER SET utf8 COLLATE utf8_general_ci;");
    }

    [Fact]
    public void DropPrimary()
    {
      Utility.ParseSql(@"ALTER TABLE mytable DROP PRIMARY KEY, ADD PRIMARY KEY(col1,col2);");
    }

    [Fact]
    public void ChangeColumn4()
    {
      Utility.ParseSql(@"ALTER TABLE tablex CHANGE colx colx int AFTER coly;");
    }

    [Fact]
    public void AddColumn()
    {
      Utility.ParseSql(
@"ALTER TABLE mytable ADD COLUMN dummy1 VARCHAR(40) AFTER id, ADD COLUMN dummy2 VARCHAR(12) AFTER dummy1;");
    }

    [Fact]
    public void ModifyColumn5()
    {
      Utility.ParseSql(@"ALTER TABLE table_name MODIFY column_to_move varchar( 20 ) AFTER column_to_reference;");
    }

    [Fact]
    public void ChangeColumn5()
    {
      Utility.ParseSql(@"ALTER TABLE tablename CHANGE columnname columnname TIMESTAMP DEFAULT CURRENT_TIMESTAMP;");
    }

    [Fact]
    public void AddColumn2()
    {
      Utility.ParseSql(@"ALTER TABLE books ADD COLUMN `author` int(10) unsigned NOT NULL ;");
    }

    [Fact]
    public void AddIndex()
    {
      Utility.ParseSql(@"ALTER TABLE books ADD INDEX (author) ;");
    }

    [Fact]
    public void AddForeignKey()
    {
      Utility.ParseSql(@"ALTER TABLE books ADD FOREIGN KEY (author) REFERENCES `users` (`id`) ;");
    }

    [Fact]
    public void ChangeColumn6()
    {
      Utility.ParseSql(@"ALTER TABLE tablex CHANGE colx colx int AFTER coly;");
    }

    [Fact]
    public void Rename()
    {
      Utility.ParseSql(@"ALTER TABLE t1 RENAME t2;");
    }

    [Fact]
    public void ModifyColumn6()
    {
      Utility.ParseSql(@"ALTER TABLE t2 MODIFY a TINYINT NOT NULL, CHANGE b c CHAR(20);");
    }

    [Fact]
    public void AddColumn3()
    {
      Utility.ParseSql(@"ALTER TABLE t2 ADD d TIMESTAMP;");
    }

    [Fact]
    public void DropColumn2()
    {
      Utility.ParseSql(@"ALTER TABLE t2 DROP COLUMN c;");
    }

    [Fact]
    public void AddColumn4()
    {
      Utility.ParseSql(@"ALTER TABLE t2 ADD c INT UNSIGNED NOT NULL AUTO_INCREMENT,
  ADD PRIMARY KEY (c);");
    }

    //[Fact]
    //public void StorageDisk()
    //{
    //  Utility.ParseSql(@"ALTER TABLE t1 TABLESPACE ts_1 STORAGE DISK;");
    //}

    //[Fact]
    //public void StorageDisk2()
    //{
    //  Utility.ParseSql(@"ALTER TABLE t2 STORAGE DISK;");
    //}

    //[Fact]
    //public void Tablespace()
    //{
    //  Utility.ParseSql(@"ALTER TABLE t2 TABLESPACE ts_1 STORAGE DISK;");
    //}

    [Fact]
    public void Modify7()
    {
      Utility.ParseSql(@"ALTER TABLE t3 MODIFY c2 INT STORAGE MEMORY;");
    }

    [Fact]
    public void AddColumn5()
    {
      // ToDo: MYSQLFORVS-612 - This should be working
//      Utility.ParseSql(@"CREATE TABLE t2 LIKE t1;
//ALTER TABLE t2 ADD id INT AUTO_INCREMENT PRIMARY KEY;
//INSERT INTO t2 SELECT * FROM t1 ORDER BY col1, col2;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void Rename2()
    {
      Utility.ParseSql(@"ALTER TABLE t2 RENAME t1;");
    }

    [Fact]
    public void Rename_Index()
    {
      //Utility.ParseSql(@"ALTER TABLE t2 RENAME INDEX Idx1 to Idx2;", false, new Version(5, 7, 0));
      Utility.ParseSql("", false, new Version(5, 7, 0));
      Utility.ParseSql(null, false, new Version(5, 7, 0));
    }

    [Fact]
    public void Rename_Key()
    {
      Utility.ParseSql(@"ALTER TABLE t2 RENAME KEY K1 to K2;", false, new Version(5, 7, 0));
    }

    [Fact]
    public void Invalid_Rename_Index()
    {
      Utility.ParseSql(@"ALTER TABLE t2 RENAME INDEX Idx1 to Idx2;", true, new Version(5, 6, 0));
    }

    [Fact]
    public void Partition()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1
      //    PARTITION BY HASH(id)
      //    PARTITIONS 8;

      //CREATE TABLE t1 (
      //    id INT,
      //    year_col INT
      //)
      //PARTITION BY RANGE (year_col) (
      //    PARTITION p0 VALUES LESS THAN (1991),
      //    PARTITION p1 VALUES LESS THAN (1995),
      //    PARTITION p2 VALUES LESS THAN (1999)
      //);
      //");
    }

    [Fact]
    public void Partition2()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.7/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1 DROP PARTITION p0, p1;");
    }

    [Fact]
    public void Partition3()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/create-table.html
      //Utility.ParseSql(@"CREATE TABLE t2 (name VARCHAR (30), started DATE) PARTITION BY HASH( YEAR(started) ) PARTITIONS 6;");
    }

    [Fact]
    public void Partition4()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t2 COALESCE PARTITION 2;");
    }

    [Fact]
    public void Partition5()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER ONLINE TABLE table1 REORGANIZE PARTITION;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void Partition6()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1 ANALYZE PARTITION p1, p2;");
    }

    [Fact]
    public void Partition7()
    {
      string result = Utility.ParseSql(@"ALTER TABLE t1 ANALYZE PARTITION p1, CHECK PARTITION p2;", true, new Version(5, 7, 12));
      Assert.True(result.IndexOf("unexpected 'CHECK' (check)", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void Partition8()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1 ANALYZE PARTITION p1, p2;");
    }

    [Fact]
    public void Partition9()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1 ANALYZE PARTITION p1;");
    }

    [Fact]
    public void Partition10()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-partition-operations.html
      //Utility.ParseSql(@"ALTER TABLE t1 CHECK PARTITION p2;");
    }

    [Fact]
    public void OnlineAddColumn()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-online-operations.html
      //Utility.ParseSql(@"ALTER ONLINE TABLE t1 ADD COLUMN c3 INT COLUMN_FORMAT DYNAMIC STORAGE MEMORY;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void OnlineAddColumn2()
    {
      // ToDo: MYSQLFORVS-612 - This should be working, as stated here: http://dev.mysql.com/doc/refman/5.5/en/alter-table-online-operations.html
      //string result = Utility.ParseSql(@"ALTER ONLINE TABLE t1 ADD COLUMN c3 INT COLUMN_FORMAT DYNAMIC;", true, new Version(5, 5, 0));
      //Assert.True(result.IndexOf("ERROR 1235 (42000): This version of MySQL doesn't yet support.", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void OnlineAddColumn3()
    {
      Utility.ParseSql(@"ALTER ONLINE TABLE t1 ADD COLUMN c3 INT STORAGE MEMORY;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void OnlineAddColumn4()
    {
      Utility.ParseSql(@"ALTER ONLINE TABLE t1 ADD COLUMN c2 INT, ADD COLUMN c3 INT;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void OnlineAddColumn5()
    {
      Utility.ParseSql(@"ALTER ONLINE TABLE t1 ADD COLUMN c2 INT, ADD COLUMN c3 INT;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void OnlineAddColumn6()
    {
      Utility.ParseSql(@"ALTER ONLINE TABLE t2 ADD COLUMN c2 INT;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void TableType50()
    {
      Utility.ParseSql(@"alter TABLE t type=innodb;", false, new Version(5, 0, 0));
    }

    [Fact]
    public void TableType51()
    {
      string result = Utility.ParseSql(@"alter TABLE t1 ENGINE=innodb;", false, new Version(5, 5, 0));
    }

    [Fact]
    public void TruncatePartition51()
    {
      string result = Utility.ParseSql(@"alter table t1 truncate partition p1, p2;", true, new Version(5, 1, 0));
      Assert.True(result.IndexOf("'truncate' (truncate) is not valid input here.", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void TruncatePartition55()
    {
      Utility.ParseSql(@"ALTER TABLE t1 TRUNCATE PARTITION p1, p3;", false, new Version(5, 7, 12));
    }

    [Fact]
    public void Algorithm_55()
    {
      string result = Utility.ParseSql(@"alter table t1 add column myname varchar( 20 ), algorithm = copy;", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("algorithm", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void Lock_55()
    {
      string result = Utility.ParseSql(@"alter table t1 add column myname varchar( 20 ), lock = none;", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("lock", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void Algorithm_56_1()
    {
      Utility.ParseSql(@"alter table t1 	algorithm = default;", false, new Version(5, 6, 1));
    }

    [Fact]
    public void Algorithm_56_2()
    {
      Utility.ParseSql(@"alter table t1 	add column myname varchar( 20 ), algorithm = copy;", false, new Version(5, 6, 2));
    }

    [Fact]
    public void Algorithm_56_3()
    {
      Utility.ParseSql(@"alter table t1 	drop column myname, algorithm = inplace;", false, new Version(5, 6, 3));
    }

    [Fact]
    public void Lock_56_1()
    {
      Utility.ParseSql(@"alter table t1 	lock = default;", false, new Version(5, 6, 1));
    }

    [Fact]
    public void Lock_56_2()
    {
      Utility.ParseSql(@"alter table t1 	add column myname varchar( 20 ), lock = none;", false, new Version(5, 6, 2));
    }

    [Fact]
    public void Lock_56_3()
    {
      Utility.ParseSql(@"alter table t1 	add column myname varchar( 20 ), lock = shared;", false, new Version(5, 6, 3));
    }

    [Fact]
    public void Lock_56_4()
    {
      Utility.ParseSql(@"alter table t1 	drop column myname, lock = exclusive;", false, new Version(5, 6, 4));
    }

    [Fact]
    public void Lock_56_5()
    {
      Utility.ParseSql(@"alter table t1 	drop column myname, lock = exclusive;", false, new Version(5, 6, 5));
    }

    [Fact]
    public void Lock_56_5_ExchangePartition()
    {
      Utility.ParseSql(@"alter table t1 exchange partition p1 with table t2;", false, new Version(5, 6, 5));
    }

    [Fact]
    public void AddColumnDateTime()
    {
      Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime DATETIME AFTER id;");
    }

    [Fact]
    public void AddColumnDateTimeWithPrecisionFailAtNotSupportedVersion()
    {
      string result = Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime DATETIME(1) AFTER id;", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("'(' (opening parenthesis) is not valid input here.\r\nThis syntax is only allowed for server versions starting with 5.6.0.",
        StringComparison.OrdinalIgnoreCase) != -1);
    }


    [Fact]
    public void AddColumnDateTimeWithPrecision()
    {
      Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime DATETIME(1) AFTER id;", false, new Version(5, 6, 0));
    }

    [Fact]
    public void AddColumnTimeWithPrecision()
    {
      Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime TIME(1) AFTER id;", false, new Version(5, 6, 0));
    }

    [Fact]
    public void AddColumnTimeWithPrecisionFailAtNotSupportedVersion()
    {
      string result = Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime TIME(1) AFTER id;", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("'(' (opening parenthesis) is not valid input here.\r\nThis syntax is only allowed for server versions starting with 5.6.0.",
        StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void AddColumnTimeStamptWithPrecision()
    {
      Utility.ParseSql(@"ALTER TABLE mytable ADD COLUMN dummydatetime TIMESTAMP(1) AFTER id;", false, new Version(5, 6, 0));
    }

    [Fact]
    public void WithIgnore56()
    {
      Utility.ParseSql(@"ALTER IGNORE TABLE mytable ADD COLUMN dummydatetime TIMESTAMP(1) AFTER id;", false, new Version(5, 6, 0));
    }

    [Fact]
    public void WithIgnore57()
    {
      Utility.ParseSql(@"ALTER IGNORE TABLE mytable ADD COLUMN dummydatetime TIMESTAMP(1) AFTER id;", false, new Version(5, 7, 12));
    }
  }
}
