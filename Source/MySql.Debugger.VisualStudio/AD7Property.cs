using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;
using System.Reflection;

namespace MySql.Debugger.VisualStudio
{
  public class AD7Property : IDebugProperty2
  {
    private AD7ProgramNode _node;

    public string Name { get; set; }
    public object Value { get; set; }

    public AD7Property(string name, object value, AD7ProgramNode node)
    {
      Name = name;
      Value = value;
      _node = node;
    }

    #region IDebugProperty2 Members

    int IDebugProperty2.EnumChildren(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, ref Guid guidFilter, enum_DBG_ATTRIB_FLAGS dwAttribFilter, string pszNameFilter, uint dwTimeout, out IEnumDebugPropertyInfo2 ppEnum)
    {
      if (Value != null)
      {
        var props = GetProperties();
        ppEnum = new AD7PropertyCollection(props.ToArray());
        return VSConstants.S_OK;
      }

      ppEnum = null;
      return VSConstants.S_FALSE;
    }

    int IDebugProperty2.GetDerivedMostProperty(out IDebugProperty2 ppDerivedMost)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetExtendedInfo(ref Guid guidExtendedInfo, out object pExtendedInfo)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetMemoryBytes(out IDebugMemoryBytes2 ppMemoryBytes)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetMemoryContext(out IDebugMemoryContext2 ppMemory)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetParent(out IDebugProperty2 ppParent)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetPropertyInfo(enum_DEBUGPROP_INFO_FLAGS dwFields, uint dwRadix, uint dwTimeout, IDebugReference2[] rgpArgs, uint dwArgCount, DEBUG_PROPERTY_INFO[] pPropertyInfo)
    {
      if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME) != 0)
      {
        pPropertyInfo[0].bstrName = Name;
        pPropertyInfo[0].dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME;
      }

      if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE) != 0)
      {
        pPropertyInfo[0].bstrValue = Value.ToString();
        pPropertyInfo[0].dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE;
      }

      if ((dwFields & enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE) != 0)
      {
        pPropertyInfo[0].bstrType = Value.GetType().FullName;
        pPropertyInfo[0].dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE;
      }

      return VSConstants.S_OK;
    }

    int IDebugProperty2.GetReference(out IDebugReference2 ppReference)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.GetSize(out uint pdwSize)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.SetValueAsReference(IDebugReference2[] rgpArgs, uint dwArgCount, IDebugReference2 pValue, uint dwTimeout)
    {
      throw new NotImplementedException();
    }

    int IDebugProperty2.SetValueAsString(string pszValue, uint dwRadix, uint dwTimeout)
    {
      _node.Debugger.SetLocalNewValue(Name, pszValue);
      return VSConstants.S_OK;
    }

    #endregion

    private IEnumerable<AD7Property> GetProperties()
    {
      var props = Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      return props.Select(propertyInfo => new AD7Property(propertyInfo.Name, propertyInfo.GetValue(Value, null), _node));
    }
  }

  public class AD7PropertyCollection : List<AD7Property>, IEnumDebugPropertyInfo2
  {
    private uint count;
    private AD7ProgramNode _node;

    public AD7PropertyCollection(AD7ProgramNode node)
    {
      _node = node;
      //TODO define auto variables
      Debugger dbg = DebuggerManager.Instance.Debugger;
      Dictionary<string, StoreType> debugVars = DebuggerManager.Instance.ScopeVariables;
      Dictionary<string, object> autoVariables = new Dictionary<string, object>();
      foreach (StoreType st in debugVars.Values)
      {
        if (st.VarKind == VarKindEnum.Internal) continue;
        autoVariables.Add(st.Name, dbg.FormatValue( dbg.Eval(st.Name)));
      }

      //autoVariables.Add("k1", "v1");
      //autoVariables.Add("k2", "v2");
      foreach (var keyVal in autoVariables)
      {
        object val = keyVal.Value != null ? keyVal.Value : null;
        this.Add(new AD7Property(keyVal.Key, val, _node));
      }
    }

    public AD7PropertyCollection(params AD7Property[] properties)
    {
      foreach (var property in properties)
      {
        this.Add(property);
      }
    }

    #region IEnumDebugPropertyInfo2 Members

    int IEnumDebugPropertyInfo2.Clone(out IEnumDebugPropertyInfo2 ppEnum)
    {
      throw new NotImplementedException();
    }

    int IEnumDebugPropertyInfo2.GetCount(out uint pcelt)
    {
      pcelt = (uint)this.Count;
      return VSConstants.S_OK;
    }

    int IEnumDebugPropertyInfo2.Next(uint celt, DEBUG_PROPERTY_INFO[] rgelt, out uint pceltFetched)
    {
      for (var i = 0; i < celt; i++)
      {
        rgelt[i].bstrName = this[(int)(i + count)].Name;
        rgelt[i].bstrValue = this[(int)(i + count)].Value != null ? this[(int)(i + count)].Value.ToString() : "$null";
        rgelt[i].bstrType = this[(int)(i + count)].Value != null ? this[(int)(i + count)].Value.GetType().ToString() : String.Empty;
        rgelt[i].pProperty = this[(int)(i + count)];
        rgelt[i].dwAttrib = GetAttributes(this[(int)(i + count)].Value);
        rgelt[i].dwFields = enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_NAME |
                            enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_VALUE |
                            enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_TYPE |
                            enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_PROP |
                            enum_DEBUGPROP_INFO_FLAGS.DEBUGPROP_INFO_ATTRIB;
      }
      pceltFetched = celt;
      return VSConstants.S_OK;
    }

    int IEnumDebugPropertyInfo2.Reset()
    {
      count = 0;
      return VSConstants.S_OK;
    }

    int IEnumDebugPropertyInfo2.Skip(uint celt)
    {
      count += celt;
      return VSConstants.S_OK;
    }

    #endregion

    private enum_DBG_ATTRIB_FLAGS GetAttributes(object obj)
    {
      if (obj == null)
      {
        return 0;
      }

      if (obj is string || obj is int || obj is char || obj is byte)
      {
        return 0;
      }

      return enum_DBG_ATTRIB_FLAGS.DBG_ATTRIB_OBJ_IS_EXPANDABLE;
    }
  }
}
