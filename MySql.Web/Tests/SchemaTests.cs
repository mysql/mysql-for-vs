// Copyright (C) 2007 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

//  This code was contributed by Sean Wright (srwright@alcor.concordia.ca) on 2007-01-12
//  The copyright was assigned and transferred under the terms of
//  the MySQL Contributor License Agreement (CLA)

using NUnit.Framework;
using MySql.Web.Security;
using System.Collections.Specialized;
using MySql.Data.MySqlClient;

namespace MySql.Web.Security.Tests
{
    [TestFixture]
    public class SchemaTests : BaseWebTest
    {
        [SetUp]
        protected override void Setup()
        {
			base.Setup();

            execSQL("DROP TABLE IF EXISTS mysql_Membership");
            execSQL("DROP TABLE IF EXISTS mysql_Roles");
            execSQL("DROP TABLE IF EXISTS mysql_UsersInRoles");
        }

        [Test]
        public void CurrentSchema()
        {
            MySQLMembershipProvider provider = new MySQLMembershipProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            provider.Initialize(null, config);

            MySqlCommand cmd = new MySqlCommand("SHOW CREATE TABLE mysql_membership", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                string createTable = reader.GetString(1);
                int index = createTable.IndexOf("COMMENT='2'");
                Assert.AreNotEqual(-1, index);
            }
        }

        [Test]
        public void UpgradeV1ToV2()
        {
            execSQL(schema1);

            MySQLMembershipProvider provider = new MySQLMembershipProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            provider.Initialize(null, config);

            MySqlCommand cmd = new MySqlCommand("SHOW CREATE TABLE mysql_membership", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                string createTable = reader.GetString(1);
                int index = createTable.IndexOf("COMMENT='2'");
                Assert.AreNotEqual(-1, index);
            }
        }

        #region Schema

        private const string schema1 =
                @"CREATE TABLE  mysql_Membership(`PKID` varchar(36) NOT NULL,
                `Username` varchar(255) NOT NULL, 
                `ApplicationName` varchar(255) NOT NULL,
                `Email` varchar(128) NOT NULL, 
                `Comment` varchar(255) default NULL,
                `Password` varchar(128) NOT NULL, 
                `PasswordQuestion` varchar(255) default NULL,
                `PasswordAnswer` varchar(255) default NULL, 
                `IsApproved` tinyint(1) default NULL,
                `LastActivityDate` datetime default NULL, 
                `LastLoginDate` datetime default NULL,
                `LastPasswordChangedDate` datetime default NULL, 
                `CreationDate` datetime default NULL,
                `IsOnline` tinyint(1) default NULL, 
                `IsLockedOut` tinyint(1) default NULL,
                `LastLockedOutDate` datetime default NULL, 
                `FailedPasswordAttemptCount` int(10) unsigned default NULL,
                `FailedPasswordAttemptWindowStart` datetime default NULL,
                `FailedPasswordAnswerAttemptCount` int(10) unsigned default NULL,
                `FailedPasswordAnswerAttemptWindowStart` datetime default NULL,
                PRIMARY KEY  (`PKID`)) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='1'";

        private const string schema2 =
            @"ALTER TABLE mysql_Membership 
            ADD COLUMN PasswordKey char(16) AFTER Password, 
            ADD COLUMN PasswordFormat tinyint AFTER PasswordKey, COMMENT='2'";

        #endregion
    }
}
