// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Reflection;
using System;
using System.Web.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using MySql.Data.MySqlClient.Tests;
using System.Resources;
using MySql.Web.Common;
using MySql.Web.Security;

namespace MySql.Web.Tests
{
    public class BaseWebTest : BaseTest
    {
		protected override void LoadStaticConfiguration()
		{
			base.LoadStaticConfiguration();

			ConnectionStringSettings css = new ConnectionStringSettings();
			css.ConnectionString = String.Format(
				"server={0};uid={1};password={2};database={3};pooling=false",
				BaseTest.host, BaseTest.user, BaseTest.password, BaseTest.database0);
			css.Name = "LocalMySqlServer";
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.ConnectionStrings.ConnectionStrings.Add(css);

			MembershipSection ms = (MembershipSection)config.SectionGroups["system.web"].Sections["membership"];
			ms.DefaultProvider = "MySQLMembershipProvider";
			ProviderSettings ps = new ProviderSettings();
			ps.Name = "MySQLMembershipProvider";
			Assembly a = Assembly.GetAssembly(typeof(MySQLMembershipProvider));
			ps.Type = "MySql.Web.Security.MySQLMembershipProvider, " + a.FullName;
			ps.Parameters.Add("connectionStringName", "LocalMySqlServer");
			ps.Parameters.Add("enablePasswordRetrieval", "false");
			ps.Parameters.Add("enablePasswordReset", "true");
			ps.Parameters.Add("requiresQuestionAndAnswer", "true");
			ps.Parameters.Add("applicationName", "/");
			ps.Parameters.Add("requiresUniqueEmail", "false");
			ps.Parameters.Add("passwordFormat", "Hashed");
			ps.Parameters.Add("maxInvalidPasswordAttempts", "5");
			ps.Parameters.Add("minRequiredPasswordLength", "7");
			ps.Parameters.Add("minRequiredNonalphanumericCharacters", "1");
			ps.Parameters.Add("passwordAttemptWindow", "10");
			ps.Parameters.Add("passwordStrengthRegularExpression", "");
			ms.Providers.Add(ps);

            RoleManagerSection rs = (RoleManagerSection)config.SectionGroups["system.web"].Sections["roleManager"];
            rs.DefaultProvider = "MySQLRoleProvider";
            rs.Enabled = true;
            ps = new ProviderSettings();
            ps.Name = "MySQLRoleProvider";
            a = Assembly.GetAssembly(typeof(MySQLRoleProvider));
            ps.Type = "MySql.Web.Security.MySQLRoleProvider, " + a.FullName;
            ps.Parameters.Add("connectionStringName", "LocalMySqlServer");
            ps.Parameters.Add("applicationName", "/");
            rs.Providers.Add(ps);

			config.Save();
			ConfigurationManager.RefreshSection("connectionStrings");
			ConfigurationManager.RefreshSection("system.web/membership");
            ConfigurationManager.RefreshSection("system.web/roleManager");
        }

        public override void Setup()
        {
           base.Setup();

           for (int ver = 1; ver <= SchemaManager.Version; ver++)
               LoadSchema(ver);
        }

        protected void LoadSchema(int version)
        {
            if (version < 1) return;

            MySQLMembershipProvider provider = new MySQLMembershipProvider();

            ResourceManager r = new ResourceManager("MySql.Web.Properties.Resources", typeof(MySQLMembershipProvider).Assembly);
            string schema = r.GetString(String.Format("schema{0}", version));
            MySqlScript script = new MySqlScript(conn);
            script.Query = schema;
            script.Execute();
        }
    }
}
