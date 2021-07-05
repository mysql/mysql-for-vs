// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio.SchemaComparer
{
    public static class SelectObjects
    {

        public static string GetDbScript(MySqlConnection con)
        {
            MySqlConnection con2 = new MySqlConnection(con.ConnectionString);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" delimiter // ");
            con2.OpenWithDefaultTimeout();
            try
            {
                // Get Stuff
                GetTables(con, con2, sb, true);   // this is for both tables & views
                GetProcedures(con, con2, sb);
                GetFunctions(con, con2, sb);
                GetTriggers(con, con2, sb);
            }
            finally
            {
                con2.Close();
            }
            return sb.ToString();
        }

        internal static List<string> GetTables(MySqlConnection con, MySqlConnection con2, StringBuilder sb, bool createScript)
        {

          if (con.State != System.Data.ConnectionState.Open)
          {
            con.OpenWithDefaultTimeout();
          }
          
          MySqlCommand cmd = new MySqlCommand("show tables", con);
          List<string> tables = new List<string>();

          using (MySqlDataReader r = cmd.ExecuteReader())
          {
            while (r.Read())
            {
              if (createScript)
              {
                MySqlCommand cmd2 = new MySqlCommand(string.Format("show create table `{0}`", r.GetString(0)), con2);
                using (MySqlDataReader r2 = cmd2.ExecuteReader())
                {
                  r2.Read();
                  sb.AppendLine(r2.GetString(1)).AppendLine(" // ");
                }
              }
              else
              {
                tables.Add(r.GetString(0));
              }
            }
          }

          con.Close();
          return tables;
        }

        private static void GetProcedures(MySqlConnection con, MySqlConnection con2, StringBuilder sb)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(
              "select `name` as routinename from mysql.proc where type = 'PROCEDURE' and db = '{0}'", con.Database), con);
            using (MySqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    MySqlCommand cmd2 = new MySqlCommand(string.Format("show create procedure `{0}`", r.GetString(0)), con2);
                    using (MySqlDataReader r2 = cmd2.ExecuteReader())
                    {
                        r2.Read();
                        sb.AppendLine(r2.GetString(2)).AppendLine(" // ");
                    }
                }
            }
        }

        private static void GetFunctions(MySqlConnection con, MySqlConnection con2, StringBuilder sb)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(
              "select `name` as routinename from mysql.proc where type = 'FUNCTION' and db = '{0}'", con.Database), con);
            using (MySqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    MySqlCommand cmd2 = new MySqlCommand(string.Format("show create function `{0}`", r.GetString(0)), con2);
                    using (MySqlDataReader r2 = cmd2.ExecuteReader())
                    {
                        r2.Read();
                        sb.AppendLine(r2.GetString(2)).AppendLine(" // ");
                    }
                }
            }
        }

        private static void GetTriggers(MySqlConnection con, MySqlConnection con2, StringBuilder sb)
        {
            MySqlCommand cmd = new MySqlCommand(string.Format(
              @"select trigger_schema, trigger_name, event_manipulation, event_object_schema, 
          event_object_table, action_statement, action_timing, `definer`
          from information_schema.triggers
          where event_object_schema = '{0}';", con.Database), con);
            using (MySqlDataReader r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    string TriggerSchema = r.GetString(0);
                    string Name = r.GetString(1);
                    string EventManipulation = r.GetString(2);
                    string EventObjectSchema = r.GetString(3);
                    string EventObjectTable = r.GetString(4);
                    string ActionStmt = r.GetString(5);
                    string ActionTiming = r.GetString(6);
                    string Definer = r.GetString(7);
                    sb.AppendFormat("create trigger {0} {1} {2} on {3} for each row {4}", Name, ActionTiming, EventManipulation,
                      string.Format("{0}.{1}", EventObjectSchema, EventObjectTable), ActionStmt).AppendLine(" // ");
                }
            }
        }

        internal static List<string> GetViews(MySqlConnection cnn)
        {
          var views = new List<String>();
          
          if (cnn == null)
            return null;

          if (cnn.State != System.Data.ConnectionState.Open)
          {
            cnn.OpenWithDefaultTimeout();
          }

          MySqlCommand cmd = new MySqlCommand("SHOW FULL TABLES IN `" + cnn.Database + "` WHERE TABLE_TYPE LIKE 'VIEW'", cnn);            
          using (MySqlDataReader reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              string viewName = reader.GetString(0);
              views.Add(viewName);
            }
          }          
          return views;                    
        }


        internal static List<String> GetSchemas(MySqlConnectionStringBuilder csb)
        {
            var schemas = new List<String>();
            try
            {
              using (MySqlConnectionSupport conn = new MySqlConnectionSupport())
              {
                conn.Initialize(null);
                conn.ConnectionString = csb.ConnectionString;
                conn.Open(false);
                using (DataReader reader = conn.Execute("SHOW DATABASES", 1, null, 0))
                {
                  while (reader.Read())
                  {
                    string dbName = reader.GetItem(0).ToString().ToLowerInvariant();
                    if (dbName == "performance_schema") continue;
                    if (dbName == "information_schema") continue;
                    if (dbName == "mysql") continue;
                    schemas.Add(dbName);
                  }
                }
              }
              return schemas;            
            }
            catch
            {              
              return null;
            }            
        }
    }
}
