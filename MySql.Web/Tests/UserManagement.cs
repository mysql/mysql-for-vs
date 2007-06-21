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

namespace MySql.Web.Security.Tests
{
    [TestFixture]
    public class UserManagement : BaseTest
    {
        private MySQLMembershipProvider provider;

        [SetUp]
        public void SetUp()
        {
            execSQL("TRUNCATE TABLE mysql_Membership");
        }

        [Test]
        public void CreateUserWithHashedPassword()
        {
            provider = new MySQLMembershipProvider();
            NameValueCollection config = new NameValueCollection();
            config.Add("connectionStringName", "LocalMySqlServer");
            config.Add("applicationName", "/");
            provider.Initialize(null, config);

            // create the user
            MembershipCreateStatus status;
            provider.CreateUser("foo", "bar", "foo@bar.com", null, null, true, null, out status);
            Assert.AreEqual(MembershipCreateStatus.Success, status);

            // verify that the password format is hashed.
            DataTable table = GetMembers();
            MembershipPasswordFormat format = 
                (MembershipPasswordFormat)Convert.ToInt32(table.Rows[0]["PasswordFormat"]);
            Assert.AreEqual(MembershipPasswordFormat.Hashed, format);

            //  then attempt to verify the user
            provider.ValidateUser("foo", "bar");
        }
    }
}
