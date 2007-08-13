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

using System;
using System.Web.Profile;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Hosting;

namespace MySql.Web.Security
{
    class MySQLProfileProvider : ProfileProvider
    {
        private string applicationName;
        private string connectionString;

        #region Abstract Members

        public override int DeleteInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, System.DateTime userInactiveSinceDate, 
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override ProfileInfoCollection FindProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, 
            out int totalRecords)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override ProfileInfoCollection GetAllProfiles(
            ProfileAuthenticationOption authenticationOption, int pageIndex, 
            int pageSize, out int totalRecords)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override string Name
        {
            get { return "MySQLProfileProvider"; }
        }

        public override string Description
        {
            get { return "MySQL Profile provider"; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(
            SettingsContext context, SettingsPropertyCollection collection)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override void SetPropertyValues(
            SettingsContext context, SettingsPropertyValueCollection collection)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Overrides

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MySQLProfileProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MySQL Profile provider");
            }
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[
                config["connectionStringName"]];
            if (ConnectionStringSettings != null)
                connectionString = ConnectionStringSettings.ConnectionString.Trim();
            else
                connectionString = "";

            // make sure our schema is up to date
            string autoGenSchema = config["AutoGenerateSchema"];
            if ((String.IsNullOrEmpty(autoGenSchema) || Convert.ToBoolean(autoGenSchema)) &&
                connectionString != String.Empty)
                ProfileSchema.CheckSchema(connectionString);
        }

        #endregion

        private static string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

    }
}
