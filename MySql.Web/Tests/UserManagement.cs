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
using System.Web.Security;
using System.Collections.Specialized;
using System.Data;
using System;
using System.Configuration.Provider;

namespace MySql.Web.Security.Tests
{
    [TestFixture]
    public class UserManagement : BaseTest
    {
        private MySQLMembershipProvider provider;

        [SetUp]
        public void SetUp()
        {
            execSQL("DROP TABLE IF EXISTS mysql_membership");
        }

        private void CreateUserWithFormat(MembershipPasswordFormat format)
        {
            provider = new MySQLMembershipProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            config.Add("applicationName", "/");
            config.Add("passwordFormat", format.ToString());
            provider.Initialize(null, config);

            // create the user
            MembershipCreateStatus status;
            provider.CreateUser("foo", "bar", "foo@bar.com", null, null, true, null, out status);
            Assert.AreEqual(MembershipCreateStatus.Success, status);

            // verify that the password format is hashed.
            DataTable table = GetMembers();
            MembershipPasswordFormat rowFormat =
                (MembershipPasswordFormat)Convert.ToInt32(table.Rows[0]["PasswordFormat"]);
            Assert.AreEqual(format, rowFormat);

            //  then attempt to verify the user
            Assert.IsTrue(provider.ValidateUser("foo", "bar"));
        }

        [Test]
        public void CreateUserWithHashedPassword()
        {
            CreateUserWithFormat(MembershipPasswordFormat.Hashed);
        }

        [Test]
        public void CreateUserWithEncryptedPasswordWithAutoGenKeys()
        {
            try
            {
                CreateUserWithFormat(MembershipPasswordFormat.Encrypted);
            }
            catch (ProviderException)
            {
            }
        }

        [Test]
        public void CreateUserWithClearPassword()
        {
            CreateUserWithFormat(MembershipPasswordFormat.Clear);
        }

        [Test]
        public void ChangePassword()
        {
            CreateUserWithHashedPassword();
            provider.ChangePassword("foo", "bar", "bar2");
            provider.ValidateUser("foo", "bar2");
        }

        [Test]
        public void DeleteUser()
        {
            CreateUserWithHashedPassword();
            provider.DeleteUser("foo", true);
            DataTable table = GetMembers();
            Assert.AreEqual(0, table.Rows.Count);
        }

        [Test]
        public void FindUsersByName()
        {
            CreateUserWithHashedPassword();

            int records;
            MembershipUserCollection users = provider.FindUsersByName("F%", 0, 10, out records);
            Assert.AreEqual(1, records);
            Assert.AreEqual("foo", users["foo"].UserName);
        }

        [Test]
        public void TestCreateUserOverrides()
        {
            try
            {
                Membership.CreateUser("foo", "bar");
                int records;
                MembershipUserCollection users = Membership.FindUsersByName("F%", 0, 10, out records);
                Assert.AreEqual(1, records);
                Assert.AreEqual("foo", users["foo"].UserName);

                Membership.CreateUser("test", "bar", "myemail@host.com");
                users = Membership.FindUsersByName("T%", 0, 10, out records);
                Assert.AreEqual(1, records);
                Assert.AreEqual("test", users["test"].UserName);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
