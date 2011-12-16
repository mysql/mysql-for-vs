using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NUnit.Framework;

namespace MySql.Parser.Tests
{
	[TestFixture]
	public class CreateDatabase
	{
		[Test]
		public void Simple()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("CREATE DATABASE dbname");
		}

        [Test]
        public void SimpleSchema()
        {
          MySQL51Parser.program_return r = Utility.ParseSql("CREATE SCHEMA dbname");
        }

		[Test]
		public void MissingDbName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("CREATE DATABASE", true);
		}

		[Test]
		public void IfNotExists()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("CREATE DATABASE IF NOT EXISTS `dbname`");
		}

		[Test]
		public void CharacterSet()
		{
          MySQL51Parser.program_return r;
			r = Utility.ParseSql("CREATE DATABASE `dbname` CHARACTER SET 'utf8'");
			r = Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT CHARACTER SET = 'bku'");
		}

		[Test]
		public void Collation()
		{
            MySQL51Parser.program_return r;
			r = Utility.ParseSql("CREATE DATABASE `dbname` COLLATE 'utf8_bin'");
			r = Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT COLLATE = 'bku'");
		}

		[Test]
		public void CharSetWithCollation()
		{
            MySQL51Parser.program_return r;
			r = Utility.ParseSql("CREATE DATABASE `dbname` CHARACTER SET 'utf8' COLLATE 'utf8_bin'");
			r = Utility.ParseSql("CREATE DATABASE `dbname1` DEFAULT CHARACTER SET 'utf8_bin' DEFAULT COLLATE 'bku'");
		}
	}
}
