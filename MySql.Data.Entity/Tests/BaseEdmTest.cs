// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Reflection;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using MySql.Data.MySqlClient.Tests;
using System.Resources;
using System.Xml;
using System.IO;
using NUnit.Framework;

namespace MySql.Data.Entity.Tests
{
    public class BaseEdmTest : BaseTest
    {
		protected override void LoadStaticConfiguration()
		{
			base.LoadStaticConfiguration();

			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string filename = config.FilePath;

            database0 = database1 = "test";

            XmlDocument configDoc = new XmlDocument();
            configDoc.PreserveWhitespace = true;
            configDoc.Load(filename);
            XmlElement configNode = configDoc["configuration"];
            configNode.RemoveAll();

            XmlElement systemData = (XmlElement)configDoc.CreateNode(XmlNodeType.Element, "system.data", "");
            XmlElement dbFactories = (XmlElement)configDoc.CreateNode(XmlNodeType.Element, "DbProviderFactories", "");
            XmlElement provider = (XmlElement)configDoc.CreateNode(XmlNodeType.Element, "add", "");
            provider.SetAttribute("name", "MySQL Data Provider");
            provider.SetAttribute("description", ".Net Framework Data Provider for MySQL");
            provider.SetAttribute("invariant", "MySql.Data.MySqlClient");

            string fullname = String.Format("MySql.Data.MySqlClient.MySqlClientFactory, {0}",
                typeof(MySqlConnection).Assembly.FullName);
            provider.SetAttribute("type", fullname);

            dbFactories.AppendChild(provider);
            systemData.AppendChild(dbFactories);
            configNode.AppendChild(systemData);
            configDoc.Save(filename);

			ConfigurationManager.RefreshSection("system.data");
		}

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            ResourceManager r = new ResourceManager("MySql.Data.Entity.Tests.Properties.Resources", typeof(BaseEdmTest).Assembly);
            string schema = r.GetString("schema");
            MySqlScript script = new MySqlScript(conn);
            script.Query = schema;
            script.Execute();

            // now create our procs
            schema = r.GetString("procs");
            script = new MySqlScript(conn);
            script.Delimiter = "$$";
            script.Query = schema;
            script.Execute();
        }
    }
}
