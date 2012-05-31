using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;


namespace MySql.Debugger.VisualStudio
{
  public class Ad7EnumDebugErrorBreakpoints : IEnumDebugErrorBreakpoints2
  {
    private List<IDebugErrorBreakpoint2> _errors;

    public Ad7EnumDebugErrorBreakpoints()
    {
      _errors = new List<IDebugErrorBreakpoint2>();
    }

    protected Ad7EnumDebugErrorBreakpoints(Ad7EnumDebugErrorBreakpoints enumerator)
    {
      IEnumDebugErrorBreakpoints2 e = (IEnumDebugErrorBreakpoints2)enumerator;
      e.Reset();
      _errors = new List<IDebugErrorBreakpoint2>();
      uint cnt;
      e.GetCount(out cnt);
      for (int i = 0; i < cnt; i++)
      {
        IDebugErrorBreakpoint2[] err = new IDebugErrorBreakpoint2[ 1 ];
        uint fetched = 1;
        e.Next(1, err, ref fetched);
        _errors.Add(err[1]);
      }
    }

    int IEnumDebugErrorBreakpoints2.Clone(out IEnumDebugErrorBreakpoints2 ppEnum)
    {
      ppEnum = null;
      return VSConstants.E_NOTIMPL;
    }

    int IEnumDebugErrorBreakpoints2.GetCount(out uint pcelt)
    {
      pcelt = 0;
      return VSConstants.E_NOTIMPL;
    }

    int IEnumDebugErrorBreakpoints2.Next(uint celt, IDebugErrorBreakpoint2[] rgelt, ref uint pceltFetched)
    {
      pceltFetched = 0;
      return VSConstants.E_NOTIMPL;
    }

    int IEnumDebugErrorBreakpoints2.Reset()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IEnumDebugErrorBreakpoints2.Skip(uint celt)
    {
      return VSConstants.E_NOTIMPL;
    }
  }
}
