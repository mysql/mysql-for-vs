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
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio.SchemaComparer
{
  internal class ComparerProgressArgs : EventArgs
  {
    internal int PercentageProgress { get; private set; }
  }

  internal delegate void ComparerProgress(object Sender, ComparerProgressArgs e);

  /// <summary>
  /// A class used to compare schemas in MySql databases and give differences.
  /// </summary>
  internal class Comparer
  {
    private MySqlConnection _conSrc;
    private MySqlConnection _conDst;

    internal Comparer( MySqlConnection conSrc, MySqlConnection conDst )
    {
      _conSrc = conSrc;
      _conDst = conDst;
    }

    /// <summary>
    /// Compares database from source connection with database from destiny connection.
    /// </summary>
    internal ComparerResult Compare()
    {
      /*
      - Iterate over all source items and look for them in destiny.
      - Iterate over all destiny items and look for them in source.
      - Look for missing items (will emit create).
      - Look for different items (will emit drop/create or alter).
      - Look for extra items (will emit drop).
      */
      ComparerResult result = new ComparerResult();
      // Get Tables
      ComparerResult resTmp = GetTableDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Primary keys
      resTmp = GetPrimaryKeysDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Unique keys
      resTmp = GetUniqueKeysDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Foreign keys
      resTmp = GetForeignKeysDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Views
      resTmp = GetViewsDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Procedures
      resTmp = GetStoredProceduresDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Functions
      resTmp = GetStoredFunctionsDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      // Get Triggers
      resTmp = GetTriggersDiff();
      result.DiffsInDst.AddRange(resTmp.DiffsInDst);
      result.DiffsInSrc.AddRange(resTmp.DiffsInSrc);
      return result;
    }

    internal void GetScript(ComparerResult result, bool isSource, out string scriptSrc, out string scriptDst )
    {
      List<ComparerResultItem> src = null;
      List<ComparerResultItem> dst = null;
      StringBuilder sbDst = new StringBuilder();
      StringBuilder sbSrc = new StringBuilder();

      if (isSource)
      {
        src = result.DiffsInSrc;
        dst = result.DiffsInDst;
      }
      else
      {
        src = result.DiffsInDst;
        dst = result.DiffsInSrc;
      }
      sbSrc.AppendLine("delimiter // ");
      for (int i = 0; i < src.Count; i++)
      {
        if (src[i].Type != ComparerResultItemType.Extra)
          sbSrc.Append(src[i].GetScript()).AppendLine(" // ");
      }
      sbDst.AppendLine("delimiter // ");
      for (int i = 0; i < dst.Count; i++)
      {
        if (dst[i].Type == ComparerResultItemType.Extra)
          sbDst.Append(dst[i].GetScript()).AppendLine(" // ");
      }
      scriptDst = sbDst.ToString();
      scriptSrc = sbSrc.ToString();
    }

    private ComparerResult GetTableDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<Column>(_conSrc, _conDst, ObjectType.Column, GetColumns, result.DiffsInSrc);
      CompareObjects<Column>(_conDst, _conSrc, ObjectType.Column, GetColumns, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetViewsDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<View>(_conSrc, _conDst, ObjectType.View, GetViews, result.DiffsInSrc);
      CompareObjects<View>(_conDst, _conSrc, ObjectType.View, GetViews, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetStoredProceduresDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<StoredProcedure>(_conSrc, _conDst, ObjectType.StoredProcedure, GetProcedures, result.DiffsInSrc);
      CompareObjects<StoredProcedure>(_conDst, _conSrc, ObjectType.StoredProcedure, GetProcedures, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetStoredFunctionsDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<StoredFunction>(_conSrc, _conDst, ObjectType.StoredFunction, GetFunctions, result.DiffsInSrc);
      CompareObjects<StoredFunction>(_conDst, _conSrc, ObjectType.StoredFunction, GetFunctions, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetTriggersDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<Trigger>(_conSrc, _conDst, ObjectType.Trigger, GetTriggers, result.DiffsInSrc);
      CompareObjects<Trigger>(_conDst, _conSrc, ObjectType.Trigger, GetTriggers, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetForeignKeysDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<ForeignKey>(_conSrc, _conDst, ObjectType.ForeignKey, GetForeignKeys, result.DiffsInSrc);
      CompareObjects<ForeignKey>(_conDst, _conSrc, ObjectType.ForeignKey, GetForeignKeys, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetPrimaryKeysDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<PrimaryKey>(_conSrc, _conDst, ObjectType.PrimaryKey, GetPrimaryKeys, result.DiffsInSrc);
      CompareObjects<PrimaryKey>(_conDst, _conSrc, ObjectType.PrimaryKey, GetPrimaryKeys, result.DiffsInDst);
      return result;
    }

    private ComparerResult GetUniqueKeysDiff()
    {
      ComparerResult result = new ComparerResult();
      CompareObjects<UniqueKey>(_conSrc, _conDst, ObjectType.UniqueKey, GetUniqueKeys, result.DiffsInSrc);
      CompareObjects<UniqueKey>(_conDst, _conSrc, ObjectType.UniqueKey, GetUniqueKeys, result.DiffsInDst);
      return result;
    }

    delegate Dictionary<string, T> GetMetadataObjects<T>( MySqlConnection c );

    private void CompareObjects<T>(
      MySqlConnection src, MySqlConnection dst, ObjectType type, GetMetadataObjects<T> getter,
      List<ComparerResultItem> result ) 
      where T : MetaObject
    {
      Dictionary<string, T> dicSrc = getter(src);
      Dictionary<string, T> dicDst = getter(dst);
      foreach (T t in dicSrc.Values)
      {
        T tt;
        if (dicDst.TryGetValue(t.FullName, out tt))
        {
          if (!t.Equals(tt))
          {
            result.Add(new ComparerResultItem(
              ComparerResultItemType.Different, t, t.FullName, type, t.ParentName));
          }
          else
          {
            result.Add(new ComparerResultItem(
              ComparerResultItemType.Equal, t, t.FullName, type, t.ParentName));
          }
        }
        else
        {
          result.Add(new ComparerResultItem(
            ComparerResultItemType.Missing, t, t.FullName, type, t.ParentName));
        }
      }
    }

    private Dictionary<string, Column> GetColumns(MySqlConnection con)
    {
      string sqlFilter = string.Format(
        "select t.table_name from information_schema.tables t where ( t.table_schema = '{0}' )",
        con.Database);
       // TODO: add validatioin to include datetime_precision when using 5.6 
      string sqlData = string.Format(
        @"select c.table_schema, c.table_name, c.column_name, c.column_default, c.is_nullable, c.data_type, 
          c.character_maximum_length, c.numeric_precision, c.numeric_scale,  c.column_type 
          from information_schema.columns c where ( c.table_schema = '{0}' ) and ( c.table_name in {1} )",
          con.Database, "{0}" );
      Dictionary<string, Column> dic = GetMetadata<Column>(con, sqlFilter, sqlData);
      return dic;
    }

    private Dictionary<string, View> GetViews(MySqlConnection con)
    {
      string sqlData = string.Format( 
        @"select v.table_schema, v.table_name, v.view_definition, v.definer, v.security_type from information_schema.views v 
          where ( v.table_schema = '{0}' )", con.Database );
      string sqlFilter = "";
      Dictionary<string, View> dic = GetMetadata<View>(con, sqlFilter, sqlData);
      return dic;
    }

    private Dictionary<string, StoredProcedure> GetProcedures(MySqlConnection con)
    {
      string sqlData = string.Format(
        @"select r.routine_schema, r.routine_name, r.routine_type, r.routine_definition, 
r.is_deterministic /* (no) */, r.sql_data_access, r.security_type, r.`definer` 
  from information_schema.routines r where r.routine_type = 'PROCEDURE' and r.routine_schema = '{0}';",
        con.Database );
      string sqlFilter = "";
      Dictionary<string, StoredProcedure> dic = GetMetadata<StoredProcedure>(con, sqlFilter, sqlData);
      string sqlPars = string.Format(
        @"select p.specific_schema, p.specific_name, p.ordinal_position, p.parameter_mode, 
          p.parameter_name, p.dtd_identifier
          from information_schema.parameters p where p.specific_schema = '{0}'; ", con.Database );
      DataSet ds = new DataSet();
      MySqlDataAdapter da = new MySqlDataAdapter(sqlPars, con);
      da.Fill(ds);
      DataView vi = ds.Tables[0].DefaultView;
      vi.Sort = "specific_name, ordinal_position asc";
      foreach (StoredProcedure sp in dic.Values)
      {
        sp.InitializeParameters(vi);
      }
      return dic;
    }

    private Dictionary<string, StoredFunction> GetFunctions(MySqlConnection con)
    {
      string sqlData = string.Format(
        @"select r.routine_schema, r.routine_name, r.dtd_identifier, r.routine_definition, 
        r.is_deterministic, r.sql_data_access, r.security_type, r.`definer`
        from information_schema.routines r
        where r.routine_type != 'PROCEDURE' and r.routine_schema = '{0}';",
        con.Database);
      string sqlFilter = "";
      Dictionary<string, StoredFunction> dic = GetMetadata<StoredFunction>(con, sqlFilter, sqlData);
      string sqlPars = string.Format(
        @"select p.specific_schema, p.specific_name, p.ordinal_position, p.parameter_mode, 
          p.parameter_name, p.dtd_identifier
          from information_schema.parameters p where p.specific_schema = '{0}'; ", con.Database);
      DataSet ds = new DataSet();
      MySqlDataAdapter da = new MySqlDataAdapter(sqlPars, con);
      da.Fill(ds);
      DataView vi = ds.Tables[0].DefaultView;
      vi.Sort = "specific_name, ordinal_position asc";
      foreach (StoredFunction f in dic.Values)
      {
        f.InitializeParameters(vi);
      }
      return dic;
    }

    private Dictionary<string, Trigger> GetTriggers(MySqlConnection con)
    {
      string sqlData = string.Format(
        @"select t.trigger_schema, t.trigger_name, t.event_manipulation, t.event_object_schema, 
          t.event_object_table, t.action_statement, t.action_timing, t.`definer`
          from information_schema.triggers t
          where t.event_object_schema = '{0}';",
        con.Database);
      string sqlFilter = "";
      Dictionary<string, Trigger> dic = GetMetadata<Trigger>(con, sqlFilter, sqlData);
      return dic;
    }

    private Dictionary<string, ForeignKey> GetForeignKeys(MySqlConnection con)
    {
      string sqlData = string.Format(@"select kcu.table_schema, kcu.table_name, kcu.constraint_name, 
          kcu.column_name, kcu.referenced_table_schema, 
kcu.referenced_table_name, kcu.referenced_column_name, rc.update_rule, rc.delete_rule
from information_schema.key_column_usage kcu inner join 
  information_schema.referential_constraints rc 
on 
( kcu.table_schema = rc.constraint_schema ) and
( kcu.table_name = rc.table_name ) and
( kcu.constraint_name = rc.constraint_name )
where kcu.table_schema = '{0}'", con.Database);
      string sqlFilter = "";
      Dictionary<string, ForeignKey> dic = GetMetadata<ForeignKey>(con, sqlFilter, sqlData);
      return dic;
    }

    private Dictionary<string, PrimaryKey> GetPrimaryKeys(MySqlConnection con)
    {
      string sqlData = string.Format(
@"select kcu.table_schema, kcu.constraint_name, kcu.table_name, kcu.column_name 
  from information_schema.key_column_usage kcu
     where kcu.table_schema = '{0}' and kcu.constraint_name = 'PRIMARY'", con.Database);
      string sqlFilter = "";
      Dictionary<string, PrimaryKey> dic = GetMetadata<PrimaryKey>(con, sqlFilter, sqlData);
      return dic;
    }

    private Dictionary<string, UniqueKey> GetUniqueKeys(MySqlConnection con)
    {
      string sqlData = string.Format(
@"select kcu.table_schema, kcu.constraint_name, kcu.table_name, kcu.column_name 
  from information_schema.key_column_usage kcu
     where kcu.table_schema = '{0}' and kcu.constraint_name != 'PRIMARY'", con.Database);
      string sqlFilter = "";
      Dictionary<string, UniqueKey> dic = GetMetadata<UniqueKey>(con, sqlFilter, sqlData);
      return dic;
    }

    private void EnsureOpenedConnection(MySqlConnection con)
    {
      if ((con.State & ConnectionState.Open) == 0)
      {
        con.OpenWithDefaultTimeout();
      }
    }

    /// <summary>
    /// Gets a list of table columns for a given database.
    /// </summary>
    /// <param name="con"></param>
    /// <returns></returns>
    private Dictionary<string, T> GetMetadata<T>(
      MySqlConnection con, string sqlFilter, string sqlData ) where T : MetaObject, new()
    {
      Dictionary<string, T> dic = new Dictionary<string, T>();
      EnsureOpenedConnection(con);
      try
      {
        MySqlCommand cmd = new MySqlCommand("", con);
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(sqlFilter))
        {
          sb.Append("( ");
          cmd.CommandText = sqlFilter;
          using (MySqlDataReader r1 = cmd.ExecuteReader())
          {
            while (r1.Read())
            {
              sb.Append("'").Append(r1.GetString(0)).Append("',");
            }
          }
          sb.Length = sb.Length - 1;
          sb.Append(" ) ");
          cmd.CommandText = string.Format(sqlData, sb);
        }
        else
        {
          cmd.CommandText = sqlData;
        }
        // Get columns
        using (MySqlDataReader r = cmd.ExecuteReader())
        {
          while (r.Read())
          {
            T t = new T();
            t.Initialize(r);
            dic.Add(t.FullName, t);
          }
        }
      }
      finally
      {
        con.Close();
      }
      return dic;
    }

    private ComparerResult CompareViews()
    {
      return null;
    }

    private ComparerResult CompareStoredProcedures()
    {
      return null;
    }
  }

  /// <summary>
  /// A result of comparing two schemas, arranged as a set of CompareResultItem's collections.
  /// </summary>
  internal class ComparerResult
  {
    internal List<ComparerResultItem> DiffsInDst;
    internal List<ComparerResultItem> DiffsInSrc;
    //internal List<ComparerResultItem> SameItems;

    internal ComparerResult()
    {
      DiffsInDst = new List<ComparerResultItem>();
      DiffsInSrc = new List<ComparerResultItem>();
      //SameItems = new List<ComparerResultItem>();
    }
  }

  /// <summary>
  /// An individual item from a comparison result.
  /// </summary>
  internal class ComparerResultItem
  {
    /// <summary>
    /// Type of difference found.
    /// </summary>
    internal ComparerResultItemType Type { get; set; }

    internal MetaObject MtObject { get; set; }

    /// <summary>
    /// The name of the database object.
    /// </summary>
    internal string ObjectName { get; set; }

    /// <summary>
    /// The type of object.
    /// </summary>
    internal ObjectType ObjectType { get; set; }

    /// <summary>
    /// Name of the parent object (like table name for a column or foreign key).
    /// </summary>
    internal string ParentName { get; set; }

    internal ComparerResultItem( 
      ComparerResultItemType Type, MetaObject MtObject, string ObjectName, 
      ObjectType ObjectType, string ParentName )
    {
      this.Type = Type;
      this.MtObject = MtObject;
      this.ObjectName = ObjectName;
      this.ObjectType = ObjectType;
      this.ParentName = ParentName;
    }

    public override bool Equals(object obj)
    {
      ComparerResultItem item = obj as ComparerResultItem;
      if (item == null) return false;
      return
        (item.Type == this.Type) &&
        //(item.MtObject == this.MtObject) &&
        ( string.CompareOrdinal( item.ObjectName, this.ObjectName) == 0 ) &&
        (item.ObjectType == this.ObjectType) &&
        ( string.CompareOrdinal(item.ParentName, this.ParentName) == 0 );
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal string GetScript()
    {
      return MtObject.GetScript(Type);
    }
  }

  /// <summary>
  /// Objects that can be compared.
  /// </summary>
  internal enum ObjectType : int
  {
    Column = 1,
    ForeignKey = 2,
    PrimaryKey = 3,
    UniqueKey = 4,
    View = 5,
    StoredProcedure = 6,
    StoredFunction = 7,
    Trigger = 8
  }

  /// <summary>
  /// Result of a compare item.
  /// </summary>
  internal enum ComparerResultItemType : int
  {
    Equal = 0,
    Different = 1,
    Missing = 2,
    Extra = 3
  }

  internal enum TypeScript : int
  {
    Equal = 0,
    Missing = 1, 
    Different = 2,
    Extra = 3
  }
  
  internal static class Utils
  {

    internal static bool Eq(string s1, string s2)
    {
      return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase) == 0;
    }

    internal static bool IsAnyOf(string data, params string[] values)
    {
      bool result = false;
      foreach (string s in values)
      {
        result = result || Eq(data, s);
        if (result) return true;
      }
      return false;
    }
  }
}
