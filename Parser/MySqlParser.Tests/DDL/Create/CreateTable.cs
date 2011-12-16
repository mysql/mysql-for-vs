using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NUnit.Framework;


namespace MySql.Parser.Tests.Create
{
	[TestFixture]
	public class CreateTable
	{
		[Test]
		public void Simple()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("CREATE TABLE T1 ( id int, name varchar( 20 ) )");
		}

		[Test]
		public void CreateSelect()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				@"CREATE TABLE test (a INT NOT NULL AUTO_INCREMENT,
				PRIMARY KEY (a) )
				ENGINE=MyISAM SELECT b,c FROM test2;" );
		}

		[Test]
		public void Complex1()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				@"CREATE TABLE IF NOT EXISTS `schema`.`Employee` (
				`idEmployee` VARCHAR(45) NOT NULL ,
				`Name` VARCHAR(255) NULL ,
				`idAddresses` VARCHAR(45) NULL ,
				PRIMARY KEY (`idEmployee`) ,
				CONSTRAINT `fkEmployee_Addresses`
				FOREIGN KEY `fkEmployee_Addresses` (`idAddresses`)
				REFERENCES `schema`.`Addresses` (`idAddresses`)
				ON DELETE NO ACTION
				ON UPDATE NO ACTION)
				ENGINE = InnoDB,
				DEFAULT CHARACTER SET = utf8,
				COLLATE = utf8_bin");
		}

		[Test]
		public void MergeUnion()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"create temporary table tmp2 ( Id int primary key, Name varchar( 50 ) ) engine merge union (tmp1);");
		}

		[Test]
		public void AllOptions()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
@"
create temporary table if not exists Table1 ( id int ) 
engine = innodb, auto_increment = 7, avg_row_length = 100,
default character set = latin1, checksum = 1, collate = 'latin1_swedish_ci', comment = 'A test script',
connection = 'unknown', data directory = '/home/user/data', delay_key_write = 0, index directory = '/tmp',
insert_method = last, max_rows = 65536, min_rows = 1, pack_keys = default, password = 'ndn789w4^%$tf', 
row_format = dynamic, union = ( `db1`.`table2` );
");
		}

		//[Test]
		//public void f1()
		//{
		//    MySQL51Parser.program_return r = Utility.ParseSql("");
		//}
	}
}
