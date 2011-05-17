using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Net;

namespace MySql.Data.MySqlClient.Tests
{
    public class PartialTrustSandbox : MarshalByRefObject
    {
        public static AppDomain CreatePartialTrustDomain()
        {
            AppDomainSetup setup = new AppDomainSetup() { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            PermissionSet permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissions.AddPermission(new DnsPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new SocketPermission(PermissionState.Unrestricted));

            return AppDomain.CreateDomain("Partial Trust Sandbox", null, setup, permissions);
        }

        public MySqlConnection TryOpenConnection(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
