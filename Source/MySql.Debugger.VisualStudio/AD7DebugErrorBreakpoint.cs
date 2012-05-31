using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;


namespace MySql.Debugger.VisualStudio
{
  public class AD7DebugErrorBreakpoint : IDebugErrorBreakpoint2
  {
    public AD7DebugErrorBreakpoint()
    {
    }

    int IDebugErrorBreakpoint2.GetBreakpointResolution(out IDebugErrorBreakpointResolution2 ppErrorResolution)
    {
      throw new NotImplementedException();
    }

    int IDebugErrorBreakpoint2.GetPendingBreakpoint(out IDebugPendingBreakpoint2 ppPendingBreakpoint)
    {
      throw new NotImplementedException();
    }
  }
}
