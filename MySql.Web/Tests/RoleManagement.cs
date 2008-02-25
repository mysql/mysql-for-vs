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
using MySql.Web.Security;

namespace MySql.Web.Tests
{
    [TestFixture]
    public class RoleManagement : BaseWebTest
    {
        private MySQLMembershipProvider membershipProvider;
        private MySQLRoleProvider roleProvider;

        [SetUp]
		public override void Setup()
		{
			base.Setup();

			execSQL("DROP TABLE IF EXISTS mysql_membership");
			execSQL("DROP TABLE IF EXISTS mysql_roles");

			membershipProvider = new MySQLMembershipProvider();
			NameValueCollection config = new NameValueCollection();
			config.Add("connectionStringName", "LocalMySqlServer");
			config.Add("applicationName", "/");
			membershipProvider.Initialize(null, config);
		}

        [Test]
        public void CreateAndDeleteRoles()
        {
            roleProvider = new MySQLRoleProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            config.Add("applicationName", "/");
            roleProvider.Initialize(null, config);

            // Add the role
            roleProvider.CreateRole("Administrator");
            string[] roles = roleProvider.GetAllRoles();
            Assert.AreEqual(1, roles.Length);
            Assert.AreEqual("Administrator", roles[0]);

            // now delete the role
            roleProvider.DeleteRole("Administrator", false);
            roles = roleProvider.GetAllRoles();
            Assert.AreEqual(0, roles.Length);
        }

        private void AddUser(string username, string password)
        {
            MembershipCreateStatus status;
            membershipProvider.CreateUser(username, password, "foo@bar.com", null,
                null, true, null, out status);
            if (status != MembershipCreateStatus.Success)
                Assert.Fail("User creation failed");
        }

        [Test]
        public void AddUserToRole()
        {
            roleProvider = new MySQLRoleProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            config.Add("applicationName", "/");
            roleProvider.Initialize(null, config);

            AddUser("eve", "eveeve!");
            roleProvider.CreateRole("Administrator");
            roleProvider.AddUsersToRoles(new string[] { "eve" },
                new string[] { "Administrator" });
            Assert.IsTrue(roleProvider.IsUserInRole("eve", "Administrator"));
        }
    }
}
