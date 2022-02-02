// Copyright © 2017, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using MySql.Utility.Enums;

namespace MySql.Utility.Tests.MySqlX.Base
{
  public class BaseAdminAPIProperties
  {
    #region Method Calls

    /// <summary>
    /// Validates existence of get_instance_configuration.
    /// </summary>
    public string CheckInstanceConfiguration { get; }

    /// <summary>
    /// Validates existence of configure_local_instance.
    /// </summary>
    public string ConfigureLocalInstance { get; }

    /// <summary>
    /// Validates existence of create_local_cluster.
    /// </summary>
    public string CreateLocalCluster { get; }

    /// <summary>
    /// Validates existence of delete_sandbox_instance.
    /// </summary>
    public string DeleteSandboxInstance { get; }

    /// <summary>
    /// Validates existence of deploy_sandbox_instance.
    /// </summary>
    public string DeploySandboxInstance { get; }

    /// <summary>
    /// Validates existence of drop_metadata_schema.
    /// </summary>
    public string DropMetadataSchema { get; }

    /// <summary>
    /// Validates existence of get_cluster.
    /// </summary>
    public string GetCluster { get; }

    /// <summary>
    /// Validates existence of help.
    /// </summary>
    public string Help { get; }

    /// <summary>
    /// Validates existence of kill_sandbox_instance.
    /// </summary>
    public string KillSandboxInstance { get; }

    /// <summary>
    /// Validates existence of reboot_cluster_from_complete_outage.
    /// </summary>
    public string RebootClusterFromCompleteOutage { get; }

    /// <summary>
    /// Validates existence of resetSession.
    /// </summary>
    public string ResetSession { get; }

    /// <summary>
    /// Validates existence of start_sandbox_instance.
    /// </summary>
    public string StartSandboxInstance { get; }

    /// <summary>
    /// Validates existence of stop_sandbox_instance.
    /// </summary>
    public string StopSandboxInstance { get; }

    #endregion

    #region Validation Strings

    /// <summary>
    /// Indicates that the method was not found.
    /// </summary>
    public string MethodDoesNotExist { get; }

    #endregion

    public BaseAdminAPIProperties(ScriptLanguageType type)
    {
      Help = "dba.help";

      if (type == ScriptLanguageType.JavaScript)
      {
        MethodDoesNotExist = "Invalid object member";
        CheckInstanceConfiguration = "dba.checkInstanceConfiguration";
        ConfigureLocalInstance = "dba.configureLocalInstance";
        CreateLocalCluster = "dba.createCluster";
        DeleteSandboxInstance = "dba.deleteSandboxInstance";
        DeploySandboxInstance = "dba.deploySandboxInstance";
        DropMetadataSchema = "dba.dropMetadataSchema";
        GetCluster = "dba.getCluster";
        KillSandboxInstance = "dba.killSandboxInstance";
        RebootClusterFromCompleteOutage = "dba.rebootClusterFromCompleteOutage";
        ResetSession = "dba.resetSession";
        StartSandboxInstance = "dba.startSandboxInstance";
        StopSandboxInstance = "dba.stopSandboxInstance";
      }
      else
      {
        MethodDoesNotExist = "IndexError";
        CheckInstanceConfiguration = "dba.check_instance_configuration";
        ConfigureLocalInstance = "dba.configure_local_instance";
        CreateLocalCluster = "dba.create_cluster";
        DeleteSandboxInstance = "dba.delete_sandbox_instance";
        DeploySandboxInstance = "dba.deploy_sandbox_instance";
        DropMetadataSchema = "dba.drop_metadata_schema";
        GetCluster = "dba.get_cluster";
        KillSandboxInstance = "dba.kill_sandbox_instance";
        RebootClusterFromCompleteOutage = "dba.reboot_cluster_from_complete_outage";
        ResetSession = "dba.reset_session";
        StartSandboxInstance = "dba.start_sandbox_instance";
        StopSandboxInstance = "dba.stop_sandbox_instance";
      }
    }
  }
}
