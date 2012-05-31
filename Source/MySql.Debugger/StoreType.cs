using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  /// <summary>
  /// Keeps track of the description for an argument or local variable.
  /// </summary>
  public class StoreType
  {
    public ArgTypeEnum ArgType;
    public VarKindEnum VarKind;
    //internal MySqlDbType Type;
    private bool _isDecimal;
    private string _type;
    public string Type
    {
      get { return _type; }
      set
      {
        _type = value;
        int[] values;
        if (PrecisionAndScale.TryGetValue(_type, out values))
        {
          _isDecimal = true;
          this.Length = values[0];
          this.Precision = values[1];
        }
        else
        {
          _isDecimal = false;
        }
      }
    }

    private static Dictionary<string, int[]> PrecisionAndScale = new Dictionary<string, int[]>();

    /// <summary>
    /// Casting can only be done towards: binary, char, date, datetime, decimal, signed integer, time, unsigned integer.
    /// </summary>
    private static string[,] MappingCastsData = {
      // boolean
      { "bit", "bit" },
      // int
      { "tinyint", "unsigned" },
      { "smallint", "unsigned" },
      { "mediumint", "unsigned" },
      { "int", "unsigned" },
      { "integer", "unsigned" },
      { "bigint", "unsigned" },
      // floating/fixed types
      { "real", "decimal( {0}, {1} )" },
      { "double", "decimal( {0}, {1} )" },
      { "float", "decimal( {0}, {1} )" },
      { "decimal", "decimal( {0}, {1} )" },
      { "numeric", "decimal( {0}, {1} )" },
      // date & time, http://dev.mysql.com/doc/refman/5.5/en/date-and-time-types.html
      { "date", "date" },
      { "time", "time" },
      { "timestamp", "datetime" },
      { "datetime", "datetime" },
      { "year", "date" },
      // blobs
      { "tinyblob", "binary" },
      { "blob", "binary" },
      { "mediumblob", "binary" },
      { "longblob", "binary" },
      // char
      { "char", "char" },
      { "varchar", "char" },
      // binary
      { "binary", "binary" },
      { "varbinary", "binary" },
      // text
      { "tinytext", "char" },
      { "text", "char" },
      { "mediumtext", "char" },
      { "longtext", "char" },
      // other
      { "enum", "char" },
      { "set", "char" },
    };

    public static Dictionary<string, string> Binary2Type = new Dictionary<string, string>();

    static StoreType()
    {
      // First entry native type
      // Second entry casting type
      for (int i = 0; i < MappingCastsData.GetLength(0); i++)
      {
        Binary2Type.Add(MappingCastsData[i, 0], MappingCastsData[i, 1]);
      }
      PrecisionAndScale.Add("real", new int[] { 60, 40 });
      PrecisionAndScale.Add("double", new int[] { 53, 30 });
      PrecisionAndScale.Add("float", new int[] { 23, 10 });
      PrecisionAndScale.Add("decimal", new int[] { 60, 30 });
      PrecisionAndScale.Add("numeric", new int[] { 60, 30 });
    }

    /// <summary>
    /// TODO:
    /// </summary>
    /// <returns></returns>
    public string GetCastExpressionFromBinary()
    {
      string castingType = Binary2Type[this.Type];
      if ((Debugger.Cmp(castingType, "unsigned") == 0) && (!this.Unsigned))
        castingType = "signed";
      if (!_isDecimal)
      {
        return string.Format("{0}", castingType);
      }
      else
      {
        return string.Format(string.Format("{0}", castingType), this.Length, this.Precision);
      }
    }

    public delegate void ValueChangedHandler(StoreType st);

    public event ValueChangedHandler OnValueChanged;

    internal void RaiseValueChanged(StoreType st)
    {
      if (OnValueChanged != null)
        OnValueChanged(st);
    }

    private object _value;
    public object Value
    {
      get { return _value; }
      set
      {
        if (_value != value)
        {
          _value = value;
          ValueChanged = true;
          RaiseValueChanged(this);
        }
      }
    }
    public bool ValueChanged;
    public string Name;
    public int Length;
    public int Precision;
    public bool Unsigned;

    internal bool IsOutArg
    {
      get
      {
        return (VarKind == VarKindEnum.Argument) &&
          ((ArgType == ArgTypeEnum.InOut) || (ArgType == ArgTypeEnum.Out));
      }
    }

    // For enums & sets
    public List<string> Values;

    public StoreType()
    {
      Unsigned = true;
    }

    internal StoreType(StoreType st)
    {
      this.ArgType = st.ArgType;
      this.VarKind = st.VarKind;
      this.Type = st.Type;
      this.Value = st.Value;
      this.Name = st.Name;
      this.Length = st.Length;
      this.Precision = st.Precision;
      this.Unsigned = st.Unsigned;
    }

    internal static bool IsNumeric(string type)
    {
      if (Debugger.Cmp(type, "int") == 0 || Debugger.Cmp(type, "real") == 0) return true;
      return false;
    }

    internal static bool IsString(string type)
    {
      return (Debugger.Cmp(type, "char") == 0) || (Debugger.Cmp(type, "varchar") == 0) ||
          type.EndsWith("text", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsDateTime(string type)
    {
      return (Debugger.Cmp(type, "date") == 0) || (Debugger.Cmp(type, "time") == 0) ||
        (Debugger.Cmp(type, "timestamp") == 0) || (Debugger.Cmp(type, "datetime") == 0);
    }

    internal string WrapValue()
    {
      return StoreType.WrapValue(Type, Value);
    }

    /// <summary>
    /// Generates a value suitable to be concat in a SQL query with mysql concat function.
    /// Doesn't take into account the actual value, just the type.
    /// </summary>
    /// <returns></returns>
    internal string WrapSqlValue()
    {
      if (StoreType.IsString(Type) || StoreType.IsDateTime(Type))
        return string.Format(" if( {0} is null, 'NULL', concat( '''', {0}, '''' ))", Name);
      else
        return string.Format(" if( {0} is null, 'NULL', {0} )", Name);
    }

    public static string WrapValue(string Type, object Value)
    {
      if (Value == null || Value == DBNull.Value)
        return "NULL";
      else if (StoreType.IsString(Type) || StoreType.IsDateTime(Type))
        return string.Format("'{0}'", Value);
      else if (Debugger.Cmp(Type, "bit") == 0)
        return (Convert.ToInt32(Value) == 1) ? "1" : "0";
      else
        return Value.ToString();
    }

    internal static string WrapValue(object Value)
    {
      if (Value is DBNull)
        return "NULL";
      if (Value is string)
        return string.Format("'{0}'", Value);
      else
        return Value.ToString();
    }
  }
}
