// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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

using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Data;
using MySql.Data.Entity;
using System.Reflection;
using System.Diagnostics;
using MySql.Data.Entity.Properties;
using System.Text;

namespace MySql.Data.MySqlClient
{
    internal partial class MySqlProviderServices 
    {
        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            string query = DbCreateDatabaseScript(null, storeItemCollection);
            using (MySqlConnection c = new MySqlConnection())
            {
                string connStr = connection.ConnectionString;
                MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder(connStr);
                sb.Database = "mysql";
                c.ConnectionString = sb.ConnectionString;
                c.Open();

                MySqlScript s = new MySqlScript(c, query);
                s.Execute();
            }
        }

        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            bool shouldClose = false;

            foreach (object o in storeItemCollection)
            {
                if (o is EntityContainer)
                {
                    EntityContainer ec = o as EntityContainer;

                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                            shouldClose = true;
                        }
                        connection.ChangeDatabase(ec.Name);
                        return true;
                    }
                    catch (MySqlException)
                    {
                        return false;
                    }
                    finally
                    {
                        if (shouldClose)
                            connection.Close();
                    }
                }
            }
            return false;
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            bool shouldClose = false;

            foreach (object o in storeItemCollection)
            {
                if (o is EntityContainer)
                {
                    EntityContainer ec = o as EntityContainer;
                    DbCommand cmd = connection.CreateCommand();
                    if (commandTimeout.HasValue)
                        cmd.CommandTimeout = commandTimeout.Value;
                    cmd.CommandText = String.Format("DROP DATABASE IF EXISTS `{0}`", ec.Name);
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                        shouldClose = true;
                    }
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        if (shouldClose)
                            connection.Close();
                    }
                }
            }
        }

        protected override string DbCreateDatabaseScript(string providerManifestToken, StoreItemCollection storeItemCollection)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("-- MySql script");
            sql.AppendLine("-- Created on " + DateTime.Now);
            foreach (object o in storeItemCollection)
            {
                if (o is EntityContainer)
                {
                    EntityContainer ec = (o as EntityContainer);
                    sql.AppendLine(String.Format("DROP DATABASE IF EXISTS `{0}`;", ec.Name));
                    sql.AppendLine(String.Format("CREATE DATABASE `{0}`;", ec.Name));
                    sql.AppendLine(String.Format("USE `{0}`;", ec.Name));
                    sql.AppendLine();
                }
                else if (o is EntityType)
                {
                    EntityType e = o as EntityType;
                    sql.Append(GetTableCreateScript(e));
                }
                else if (o is AssociationType)
                {
                    AssociationType a = o as AssociationType;
                    sql.Append(GetAssociationCreateScript(a));
                }
            }

            return sql.ToString();
        }

        private string GetTableCreateScript(EntityType e)
        {
            StringBuilder sql = new StringBuilder("CREATE TABLE ");
            sql.AppendFormat("`{0}`(", e.Name);
            string delimiter = "";
            bool hasPK = false;
            foreach (EdmProperty c in e.Properties)
            {
                Facet facet;
                hasPK = hasPK ||
                    (c.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet) &&
                    facet.Value.Equals(StoreGeneratedPattern.Identity));
                sql.AppendFormat("{0}{1}\t`{2}` {3}{4}", delimiter, Environment.NewLine, c.Name, 
                    GetColumnType(c.TypeUsage), GetFacetString(c));
                delimiter = ", ";
            }
            sql.AppendLine(");");
            sql.AppendLine();
            if (!hasPK && e.KeyMembers.Count > 0)
            {
                sql.Append(String.Format(
                    "ALTER TABLE `{0}` ADD PRIMARY KEY (", e.Name));
                delimiter = "";
                foreach (EdmMember m in e.KeyMembers)
                {
                    sql.AppendFormat("{0}{1}", delimiter, m.Name);
                    delimiter = ", ";
                }
                sql.AppendLine(");");
                sql.AppendLine();
            }
            return sql.ToString();
        }

        private string GetAssociationCreateScript(AssociationType a)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder keySql = new StringBuilder();

            if (a.IsForeignKey)
            {
                EntityType childType = (EntityType)a.ReferentialConstraints[0].ToProperties[0].DeclaringType;
                EntityType parentType = (EntityType)a.ReferentialConstraints[0].FromProperties[0].DeclaringType;

                sql.AppendLine(String.Format(
                    "ALTER TABLE `{0}` ADD CONSTRAINT {1}", childType.Name, a.Name));
                sql.Append("\t FOREIGN KEY (");
                string delimiter = "";
                foreach (EdmProperty p in a.ReferentialConstraints[0].ToProperties)
                {
                    EdmMember member;
                    if (!childType.KeyMembers.TryGetValue(p.Name, false, out member))
                        keySql.AppendLine(String.Format(
                            "ALTER TABLE `{0}` ADD KEY (`{1}`);", childType.Name, p.Name));
                    sql.AppendFormat("{0}{1}", delimiter, p.Name);
                    delimiter = ", ";
                }
                sql.AppendLine(")");
                delimiter = "";
                sql.Append(String.Format("\tREFERENCES {0} (", parentType.Name));
                foreach (EdmProperty p in a.ReferentialConstraints[0].FromProperties)
                {
                    EdmMember member;
                    if (!parentType.KeyMembers.TryGetValue(p.Name, false, out member))
                        keySql.AppendLine(String.Format(
                            "ALTER TABLE `{0}` ADD KEY (`{1}`);", parentType.Name, p.Name));
                    sql.AppendFormat("{0}{1}", delimiter, p.Name);
                    delimiter = ", ";
                }
                sql.AppendLine(");");
                sql.AppendLine();
            }

            keySql.Append(sql.ToString());
            return keySql.ToString();
        }

        private string GetColumnType(TypeUsage type)
        {
            string t = type.EdmType.Name;
            if (t.StartsWith("u"))
            {
                t = t.Substring(1).ToUpperInvariant() + " UNSIGNED";
            }
            else if (String.Compare(t, "guid", true) == 0)
                return "CHAR(36) BINARY";
            return t;
        }

        private string GetFacetString(EdmProperty column)
        {
            StringBuilder sql = new StringBuilder();
            Facet facet;
            ReadOnlyMetadataCollection<Facet> facets = column.TypeUsage.Facets;

            if (column.TypeUsage.EdmType.BaseType.Name == "String")
            {
                if (facets.TryGetValue("MaxLength", true, out facet))
                    sql.AppendFormat(" ({0})", facet.Value);
            }
            if (facets.TryGetValue("Nullable", true, out facet) && (bool)facet.Value == false)
                sql.Append(" NOT NULL");
            if (facets.TryGetValue("StoreGeneratedPattern", true, out facet) && facet.Value.Equals(StoreGeneratedPattern.Identity))
                sql.Append(" AUTO_INCREMENT PRIMARY KEY");
            return sql.ToString();
        }

        private bool IsStringType(TypeUsage type)
        {
            return false;
        }
    }
}
