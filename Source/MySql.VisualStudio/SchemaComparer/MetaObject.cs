// Copyright (c) 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Reflection;


namespace MySql.Data.VisualStudio.SchemaComparer
{
  internal abstract class MetaObject
  {
    internal MetaObject() { }

    internal abstract void Initialize(DbDataReader r);
    internal MySqlConnection Connection { get; set; }
    internal abstract string ParentName { get; }
    internal abstract string FullName { get; }
    internal virtual string Name { get { return ""; } }

    /// <summary>
    /// Returns the script to apply in other database when this object is different in that
    /// other database.
    /// </summary>
    /// <returns>The script.</returns>
    internal abstract string GetDifferentScript();

    /// <summary>
    /// Returns the script to apply in the other database when this object is missing in that
    /// other database.
    /// </summary>
    /// <returns>The script.</returns>
    internal abstract string GetMissingScript();

    /// <summary>
    /// Returns the script to apply in this database when this object is not in the other database
    /// (usually a drop statement).
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal abstract string GetExtraScript();

    internal string GetScript(ComparerResultItemType type)
    {
      switch (type)
      {
        case ComparerResultItemType.Different:
          return GetDifferentScript();
        case ComparerResultItemType.Equal:
          return "";
        case ComparerResultItemType.Missing:
          return GetMissingScript();
        case ComparerResultItemType.Extra:
          return GetExtraScript();
        default:
          // all options covered, this one just so the compiler doesn't cry.
          return "";
      }
    }
  }

  internal class Column : MetaObject
  {
    private bool _treatAsBoolean;

    internal override string FullName { get { return string.Format("{0}.{1}", TableName, ColumnName); } }
    internal string TableName { get; set; }
    internal string ColumnName { get; set; }
    internal string ColumnDefault { get; set; }
    internal string IsNullable { get; set; }
    internal string DataType { get; set; }
    internal int? MaxLength { get; set; }
    internal int? NumericPrecision { get; set; }
    internal int? NumericScale { get; set; }
    internal int? DatetimePrecision { get; set; }
    TypeScript TypeScript { get; set; }
    internal override string Name
    {
      get
      {
        return ColumnName;
      }
    }

    // public to avoid CS0310
    public Column()
    {
    }

    internal override void Initialize(DbDataReader r)
    {
      TableName = r.GetString(1);
      ColumnName = r.GetString(2);
      ColumnDefault = r.IsDBNull( 3 ) ? "" : r.GetString(3);
      IsNullable = r.GetString(4);
      DataType = r.GetString(5);
      int result;
      if (Int32.TryParse(r.GetValue(6).ToString(), out result))
      {
        MaxLength = r.IsDBNull(6) ? null : (int?)result;
      }

      if (Int32.TryParse(r.GetValue(7).ToString(), out result))
      {
        NumericPrecision = r.IsDBNull(7) ? null : (int?)result;
      }

      if (Int32.TryParse(r.GetValue(8).ToString(), out result))
      {
          NumericScale = r.IsDBNull(8) ? null : (int?)result;
      }
      string columnType = r.GetString(9);
      //TODO add validation when using 5.6 since datetimeprecision is no used by 5.5
      //DatetimePrecision = r.IsDBNull(9) ? null : (int?)Convert.ToInt32(r.GetString(9));
      DatetimePrecision = null;
      ConfigureBooleanType(columnType);
    }

    private void ConfigureBooleanType(string columnType)
    {
      Type t = typeof(MySqlConnection);
      PropertyInfo p = t.GetProperty("Settings", BindingFlags.NonPublic | BindingFlags.Instance);
      MySqlConnectionStringBuilder msb = ( MySqlConnectionStringBuilder )p.GetValue(Connection, null);
      _treatAsBoolean = msb.TreatTinyAsBoolean && (columnType == "tinyint(1)");
    }

    internal override string ParentName { get { return TableName; } }

    public override bool Equals(object obj)
    {
      if (!(obj is Column)) return false;
      Column c = obj as Column;
      return
        Utils.Eq(c.TableName, this.TableName) &&
        Utils.Eq(c.ColumnName, this.ColumnName) &&
        Utils.Eq(c.ColumnDefault, this.ColumnDefault) &&
        Utils.Eq(c.IsNullable, this.IsNullable) &&
        Utils.Eq(c.DataType, this.DataType) &&
        Nullable.Equals<int>(c.MaxLength, this.MaxLength) &&
        Nullable.Equals<int>(c.NumericPrecision, this.NumericPrecision) &&
        Nullable.Equals<int>(c.NumericScale, this.NumericScale) &&
        Nullable.Equals<int>(c.DatetimePrecision, this.DatetimePrecision);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Returns the type length, depending upon is a datetime, integer or float point type.
    /// </summary>
    /// <returns></returns>
    private string GetLength()
    {
      if (Utils.IsAnyOf(DataType, "char", "varchar", "varbinary", "bit", "tinyint", "smallint",
        "mediumint", "int", "integer", "bigint"))
      {
        if (MaxLength != null)
          return string.Format("({0})", MaxLength.Value);
        else
          return "";
      }
      else if (Utils.Eq(DataType, "datetime") || Utils.Eq(DataType, "time"))
      {
        if (DatetimePrecision != null)
          return string.Format("({0})", DatetimePrecision.Value);
        else
          return "";
      }
      else if (Utils.IsAnyOf(DataType, "real", "double", "float", "decimal", "numeric"))
      {
        if (NumericPrecision != null)
        {
          if (NumericScale != null)
          {
            return string.Format("( {0}, {1} )", NumericPrecision, NumericScale);
          }
          else
          {
            return string.Format("( {0} )", NumericPrecision);
          }
        }
        else
          return "";
      }
      else
        return "";
    }

    internal bool IsDateType()
    {
      if (DataType == "datetime" || DataType == "date" || DataType == "time" )
        return true;
      else
        return false;
    }

    internal bool IsDateTimeType()
    {
      string dt = DataType;
      if (dt == "datetime" || dt == "time")
      {
        return true;
      }
      return false;
    }

    internal bool IsReadOnly()
    {
      string dt = DataType;
      if (dt == "timestamp")
      {
        return true;
      }
      return false;
    }

    internal bool IsBooleanType()
    {
      string dt = DataType;
      if (_treatAsBoolean || dt == "bit" || dt == "bool" || dt == "boolean")
      {
        return true;
      }
      return false;
    }

    private string GetColumnDefinition()
    {
      return string.Format(" /* name */ `{0}` /* datatype */ {1}{2} {3} {4}",
        ColumnName, DataType, GetLength(),
        string.IsNullOrEmpty(IsNullable) ? "NOT NULL" : "NULL",
        string.IsNullOrEmpty(ColumnDefault) ? "" : " default " + ColumnDefault);
    }

    // TableName, ColumnName, ColumnDefault, IsNullable, DataType, MaxLength, NumericPrecision,
    // NumericScale, DateTimePrecision
    internal override string GetDifferentScript()
    {
      // alter table ... change ...
      return string.Format(
        @"alter table `{0}` change column /* tab name */ `{1}`", TableName,
        GetColumnDefinition());
    }

    internal override string GetMissingScript()
    {
      // alter table ... add ...
      return string.Format("alter table `{0}` add column {1}", TableName, GetColumnDefinition());
    }

    internal override string GetExtraScript()
    {
      // alter tabble ... drop ...
      return string.Format("alter table `{0}` drop column `{1}`", TableName, ColumnName);
    }
  }

  internal abstract class ColumnKey : MetaObject
  {
    internal override string ParentName { get { return TableName; } }
    internal override string FullName { get { return string.Format("{0}.{1}", TableName, ConstraintName); } }
    internal string Schema { get; private set; }
    internal string ConstraintName { get; private set; }
    internal string TableName { get; private set; }
    // TODO: Now to manage PKs/UKs made up of more than one column?
    internal string ColumnName { get; private set; }

    public override bool Equals(object obj)
    {
      ColumnKey ck = obj as ColumnKey;
      if (ck == null) return false;
      return
        Utils.Eq(ck.Schema, this.Schema) &&
        Utils.Eq(ck.ConstraintName, this.ConstraintName) &&
        Utils.Eq(ck.TableName, this.TableName) &&
        Utils.Eq(ck.ColumnName, this.ColumnName);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal override void Initialize(DbDataReader r)
    {
      /* select table_schema, constraint_name, table_name, column_name from key_column_usage 
     * where table_schema = 'mysql' and referenced_table_name is null limit 10 */
      Schema = r.GetString(0);
      ConstraintName = r.GetString(1);
      TableName = r.GetString(2);
      ColumnName = r.GetString(3);
    }
  }

  internal class ForeignKey : ColumnKey
  {
    internal string ReferencedSchema { get; private set; }
    internal string ReferencedTableName { get; private set; }
    internal string ReferencedColumnName { get; private set; }
    internal string UpdateRule { get; private set; }
    internal string DeleteRule { get; private set; }

    public ForeignKey()
    {
    }

    internal override void Initialize(DbDataReader r)
    {
      base.Initialize(r);
      ReferencedSchema = r.GetString(4);
      ReferencedTableName = r.GetString(5);
      ReferencedColumnName = r.GetString(6);
      UpdateRule = r.GetString(7);
      DeleteRule = r.GetString(8);

      /* select kcu.table_schema, kcu.table_name, kcu.constraint_name, 
          kcu.column_name, kcu.referenced_table_schema, 
kcu.referenced_table_name, kcu.referenced_column_name, rc.update_rule, rc.delete_rule
from key_column_usage kcu inner join referential_constraints rc 
on 
( kcu.table_schema = rc.constraint_schema ) and
( kcu.table_name = rc.table_name ) and
( kcu.constraint_name = rc.constraint_name )
where cu.referenced_table_name is  not null limit 10*/
    }

    public override bool Equals(object obj)
    {
      ForeignKey fk = obj as ForeignKey;
      if (fk == null) return false;
      return
        Utils.Eq(fk.Schema, this.Schema) &&
        Utils.Eq(fk.TableName, this.TableName) &&
        Utils.Eq(fk.ConstraintName, this.ConstraintName) &&
        Utils.Eq(fk.ColumnName, this.ColumnName) &&
        Utils.Eq(fk.ReferencedSchema, this.ReferencedSchema) &&
        Utils.Eq(fk.ReferencedTableName, this.ReferencedTableName) &&
        Utils.Eq(fk.ReferencedColumnName, this.ReferencedColumnName) &&
        Utils.Eq(fk.UpdateRule, this.UpdateRule) &&
        Utils.Eq(fk.DeleteRule, this.DeleteRule);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    private string GetForeignKeyDefintion()
    {
      return string.Format(
        " foreign key `{0}` ( `{1}` ) references `{2}` ( `{3}` ) on delete {4} on update {5}",
        ConstraintName, ColumnName, ReferencedTableName, ReferencedColumnName, DeleteRule, UpdateRule);
    }

    internal override string GetDifferentScript()
    {
      return string.Format("alter table `{0}` drop foreign key `{1}` // alter table `{0}` add {2}", TableName, ConstraintName, GetForeignKeyDefintion());
    }

    internal override string GetExtraScript()
    {
      return string.Format("alter table `{0}` drop foreign key `{1}`", TableName, ConstraintName);
    }

    internal override string GetMissingScript()
    {
      return string.Format("alter table `{0}` add {1}", TableName, GetForeignKeyDefintion() );
    }
  }  

  internal class PrimaryKey : ColumnKey
  {
    private string GetPrimaryKeyDefinition()
    {
      return string.Format("primary key ( `{0}` )", ColumnName);
    }

    internal override string GetDifferentScript()
    {
      return string.Format("alter table `{0}` drop primary key // alter table `{0}` add {1}", TableName, GetPrimaryKeyDefinition() );
    }

    internal override string GetExtraScript()
    {
      return string.Format("alter table `{0}` drop primary key", TableName );
    }

    internal override string GetMissingScript()
    {
      return string.Format("alter table `{0}` add {1}", TableName, GetPrimaryKeyDefinition() );
    }
  }

  internal class UniqueKey : ColumnKey
  {
    private string GetUniqueKeyDefinition()
    {
      return string.Format("unique key `{0}` ( `{1}` )", ConstraintName, ColumnName );
    }

    internal override string GetDifferentScript()
    {
      return string.Format("alter table `{0}` drop key `{1}` // alter table `{0}` add {2}", TableName, ConstraintName, GetUniqueKeyDefinition() );
    }

    internal override string GetExtraScript()
    {
      return string.Format("alter table `{0}` drop key `{1}`", TableName, ConstraintName );
    }

    internal override string GetMissingScript()
    {
      return string.Format("alter table `{0}` add {1}", TableName, GetUniqueKeyDefinition() );
    }
  }

  internal class View : MetaObject
  {
    internal string SchemaName { get; private set; }
    internal new string Name { get; private set; }
    internal string Body { get; private set; }
    internal string Definer { get; private set; }
    internal string SecurityType { get; private set; }

    internal override string FullName { get { return Name; } }

    internal override string ParentName { get { return ""; } }

    public View() { }

    internal override void Initialize(DbDataReader r)
    {
      SchemaName = r.GetString(0);
      Name = r.GetString(1);
      Body = r.GetString(2);
      Definer = r.GetString(3);
      SecurityType = r.GetString(4);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is View)) return false;
      View v = obj as View;
      return
        Utils.Eq(v.SchemaName, this.SchemaName) &&
        Utils.Eq(v.Name, this.Name) &&
        Utils.Eq(v.Body, this.Body) &&
        Utils.Eq(v.Definer, this.Definer) &&
        Utils.Eq(v.SecurityType, this.SecurityType);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    private string GetViewDefinition()
    {
      return string.Format("create {1} {2} view `{0}` as {3}",
        string.IsNullOrEmpty(Definer) ? "" : string.Format("definer {0}", Definer),
        string.IsNullOrEmpty(SecurityType) ? "" : string.Format("sql security {0}", SecurityType),
        Name, Body);
    }

    internal override string GetDifferentScript()
    {
      return string.Format("drop view `{0}`// {1} //", Name, GetViewDefinition());
    }

    internal override string GetExtraScript()
    {
      return string.Format("drop view `{0}` //", Name);
    }

    internal override string GetMissingScript()
    {
      return string.Format("{0} //", GetViewDefinition());
    }
  }

  internal abstract class RoutineWithArgs : MetaObject
  {
    protected internal List<Parameter> Parameters { get; set; }
    protected internal new string Name { get; protected set; }

    protected internal RoutineWithArgs()
    {
      Parameters = new List<Parameter>();
    }

    protected internal void InitializeParameters(DataView vi)
    {
      vi.RowFilter = string.Format("specific_name = '{0}'", this.Name);
      /*@"select specific_schema, specific_name, ordinal_position, parameter_mode, 
          parameter_name, dtd_identifier
          from information_schema.parameters where specific_schema = '{0}'; " */
      foreach (DataRowView drv in vi)
      {
        Parameters.Add(new Parameter()
        {
          OrdinalPos = Convert.ToInt32(drv[2]),
          Type = ( ParameterType )Enum.Parse( typeof( ParameterType), ( string )drv[3], true ),
          Name = (string)drv[4],
          DataType = (string)drv[5]
        });
      }
    }

    protected internal bool AreParametersEqual(RoutineWithArgs args)
    {
      if (args.Parameters.Count != Parameters.Count) return false;
      for (int i = 0; i < Parameters.Count; i++)
      {
        if ( !args.Parameters[i].Equals(Parameters[i]) ) return false;
      }
      return true;
    }

    protected internal string GetParametersScript()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("( ");
      foreach (Parameter p in Parameters)
      {
        sb.AppendFormat("{0} {1} {2},", p.Type, p.Name, p.DataType);
      }
      sb.Length = sb.Length - 1;
      sb.Append(" ) ");
      return sb.ToString();
    }
  }

  internal class StoredProcedure : RoutineWithArgs
  {
    internal string Schema { get; private set; }
    internal string Body { get; private set; }
    internal bool IsDeterministic { get; private set; }
    internal string SqlDataAccess { get; private set; }
    internal string SecurityType { get; private set; }
    internal string Definer { get; private set; }

    internal override string FullName
    {
      get { return Name; }
    }

    internal override string ParentName
    {
      get { return Schema; }
    }

    public override bool Equals(object obj)
    {
      if (!(obj is StoredProcedure)) return false;
      StoredProcedure sp = obj as StoredProcedure;
      return
        Utils.Eq(Schema, sp.Schema) &&
        Utils.Eq(Name, sp.Name) &&
        Utils.Eq(Body, sp.Body) &&
        (IsDeterministic == sp.IsDeterministic) &&
        Utils.Eq(SqlDataAccess, sp.SqlDataAccess) &&
        Utils.Eq(SecurityType, sp.SecurityType) &&
        Utils.Eq(Definer, sp.Definer) &&
        AreParametersEqual(sp);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal override void Initialize(DbDataReader r)
    {
      /*
       * select routine_schema, routine_name, routine_type, routine_definition, 
is_deterministic, sql_data_access, security_type, `definer` 
  from information_schema.routines where routine_type = 'PROCEDURE' 
       * */
      Schema = r.GetString(0);
      Name = r.GetString(1);
      Body = r.GetString(3);
      IsDeterministic = !Utils.Eq(r.GetString(4), "no");
      SqlDataAccess = r.GetString(5);
      SecurityType = r.GetString(6);
      Definer = r.GetString(7);
    }

    private string GetProcedureDefinition()
    {
      string characteristic = string.Format( " {0} deterministic {1} security type {2}", 
        IsDeterministic ? "" : "no", SqlDataAccess, SecurityType );
      return string.Format(@"create {0} procedure /* name */ `{1}` /* args */ {2} 
        /* characteristic */ {3} /* body */ as {4}", 
        string.IsNullOrEmpty( Definer ) ? "" : "definer " + Definer,
        Name,
        GetParametersScript(),
        characteristic,
        Body );
    }

    internal override string GetDifferentScript()
    {
      return string.Format("drop procedure `{0}` // {1} //", Name, GetProcedureDefinition() );
    }

    internal override string GetExtraScript()
    {
      return string.Format("drop procedure `{0}` //", Name);
    }

    internal override string GetMissingScript()
    {
      return string.Format("{0} //", GetProcedureDefinition());
    }
  }

  internal class StoredFunction : RoutineWithArgs
  {
    internal string Schema { get; private set; }
    internal string Body { get; private set; }
    internal string DataType { get; private set; }
    internal bool IsDeterministic { get; private set; }
    internal string SqlDataAccess { get; private set; }
    internal string SecurityType { get; private set; }
    internal string Definer { get; private set; }

    internal override string FullName { get { return Name; } }

    internal override string ParentName { get { return Schema; } }

    public override bool Equals(object obj)
    {
      if (!(obj is StoredFunction)) return false;
      StoredFunction sp = obj as StoredFunction;
      return
        Utils.Eq(Schema, sp.Schema) &&
        Utils.Eq(Name, sp.Name) &&
        Utils.Eq(Body, sp.Body) &&
        Utils.Eq( DataType, sp.DataType ) &&
        (IsDeterministic == sp.IsDeterministic) &&
        Utils.Eq(SqlDataAccess, sp.SqlDataAccess) &&
        Utils.Eq(SecurityType, sp.SecurityType) &&
        Utils.Eq(Definer, sp.Definer) &&
        AreParametersEqual(sp);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal override void Initialize(DbDataReader r)
    {
      /* select routine_schema, routine_name, dtd_identifier, routine_definition, 
       * is_deterministic, sql_data_access, security_type, `definer`
from information_schema.routines 
where routine_type != 'PROCEDURE' */
      Schema = r.GetString(0);
      Name = r.GetString(1);
      DataType = r.GetString(2);
      Body = r.GetString(3);
      IsDeterministic = !Utils.Eq(r.GetString(4), "no");
      SqlDataAccess = r.GetString(5);
      SecurityType = r.GetString(6);
      Definer = r.GetString(7);
    }

    private string GetFunctionDefinition()
    {
      string characteristic = string.Format(" {0} deterministic {1} security type {2}",
  IsDeterministic ? "" : "no", SqlDataAccess, SecurityType);
      return string.Format(@"create {0} function /* name */ `{1}` /* args */ {2} 
        returns {5} /* characteristic */ {3} /* body */ as {4}",
        string.IsNullOrEmpty(Definer) ? "" : "definer " + Definer,
        Name,
        GetParametersScript(),
        characteristic,
        Body, 
        DataType );
    }

    internal override string GetDifferentScript()
    {
      return string.Format("drop function `{0}` // {1} //", Name, GetFunctionDefinition());
    }

    internal override string GetExtraScript()
    {
      return string.Format("drop function `{0}` //", Name);
    }

    internal override string GetMissingScript()
    {
      return string.Format("{1} //", GetFunctionDefinition() );
    }
  }

  internal class Trigger : MetaObject
  {
    internal string TriggerSchema { get; private set; }
    internal new string Name { get; private set; }
    internal string EventManipulation { get; private set; }
    internal string EventObjectSchema { get; private set; }
    internal string EventObjectTable { get; private set; }
    internal string ActionStmt { get; private set; }
    internal string ActionTiming { get; private set; }
    internal string Definer { get; private set; }
    internal override string FullName { get { return Name; } }
    internal override string ParentName { get { return EventObjectSchema; } }

    public override bool Equals(object obj)
    {
      if (!(obj is Trigger)) return false;
      Trigger tr = obj as Trigger;
      return
        Utils.Eq(TriggerSchema, tr.TriggerSchema) &&
        Utils.Eq(Name, tr.Name) &&
        Utils.Eq(EventManipulation, tr.EventManipulation) &&
        Utils.Eq(EventObjectSchema, tr.EventObjectSchema) &&
        Utils.Eq(EventObjectTable, tr.EventObjectTable) &&
        Utils.Eq(ActionStmt, tr.ActionStmt) &&
        Utils.Eq( Definer, tr.Definer );
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    internal override void Initialize(DbDataReader r)
    {
      /* select trigger_schema, trigger_name, event_manipulation, event_object_schema, 
        event_object_table, action_statement, action_timing, `definer`
        from information_schema.triggers; */
      TriggerSchema = r.GetString( 0 );
      Name = r.GetString( 1 );
      EventManipulation = r.GetString( 2 );
      EventObjectSchema = r.GetString( 3 );
      EventObjectTable = r.GetString( 4 );
      ActionStmt = r.GetString( 5 );
      ActionTiming = r.GetString( 6 );
      Definer = r.GetString( 7 );
    }

    private string GetTriggerDefinition()
    {
      // Format is "create trigger trigger_name trigger_time trigger_event on tblName for each row body"
      return string.Format("create trigger {0} {1} {2} on {3} for each row {4}", Name, ActionTiming, EventManipulation,
        string.Format("{0}.{1}", EventObjectSchema, EventObjectTable), ActionStmt);
    }

    internal override string GetDifferentScript()
    {
      return string.Format( "drop trigger `{0}` // {1} //", Name, GetTriggerDefinition() );
    }

    internal override string GetExtraScript()
    {
      return string.Format("drop trigger `{0}` //");
    }

    internal override string GetMissingScript()
    {
      return string.Format("{0} //", GetTriggerDefinition() );
    }
  }

  internal enum ParameterType : int
  {
    In = 1,
    Out = 2,
    InOut = 3
  }

  internal class Parameter
  {
    /*
     * select specific_schema, specific_name, ordinal_position, parameter_mode, 
          parameter_name, dtd_identifier
          from information_schema.parameters where specific_schema = '{0}' and 
          specific_name = '{1}';
     * */
    //internal string Schema { get; private set; }
    //internal string RoutineName { get; private set; }
    internal int OrdinalPos { get; set; }
    internal ParameterType Type { get; set; }
    internal string Name { get; set; }
    internal string DataType { get; set; }

    public override bool Equals(object obj)
    {
      Parameter p = obj as Parameter;
      if (p == null) return false;
      return
        (p.OrdinalPos == OrdinalPos) &&
        (p.Type == Type) &&
        Utils.Eq(p.Name, Name) &&
        Utils.Eq(p.DataType, DataType);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
