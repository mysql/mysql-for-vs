using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration.Provider;
using MySql.Web.Properties;

namespace MySql.Web.General
{
    internal class Application
    {
        public Application(string name, string desc)
        {
            Id = -1;
            Name = name;
            Description = desc;
        }

        public int Id { get; private set; }
        public string Name;
        public string Description { get; private set; }

        public int FetchId(MySqlConnection connection)
        {
            if (Id == -1)
            {
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT id FROM my_aspnet_Applications WHERE name=@name", connection);
                cmd.Parameters.AddWithValue("@name", Name);
                object id = cmd.ExecuteScalar();
                Id = id == null ? -1 : Convert.ToInt32(id);
            }
            return Id;
        }

        /// <summary>
        /// Creates the or fetch application id.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="applicationId">The application id.</param>
        /// <param name="applicationDesc">The application desc.</param>
        /// <param name="connection">The connection.</param>
        public int EnsureId(MySqlConnection connection)
        {
            // first try and retrieve the existing id
            if (FetchId(connection) <= 0)
            {
                MySqlCommand cmd = new MySqlCommand(
                    "INSERT INTO my_aspnet_Applications VALUES (NULL, @appName, @appDesc)", connection);
                cmd.Parameters.AddWithValue("@appName", Name);
                cmd.Parameters.AddWithValue("@appDesc", Description);
                int recordsAffected = cmd.ExecuteNonQuery();
                if (recordsAffected != 1)
                    throw new ProviderException(Resources.UnableToCreateApplication);

                Id = Convert.ToInt32(cmd.LastInsertedId);
            }
            return Id;
        }
    }
}
