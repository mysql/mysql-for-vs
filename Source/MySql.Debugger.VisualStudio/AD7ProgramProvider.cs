using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace MySql.Debugger.VisualStudio
{
  [ComVisible(true)]
  [Guid(AD7Guids.ProgramProviderString)]
  public class AD7ProgramProvider : IDebugProgramProvider2
  {
    #region IDebugProgramProvider2 Members

    int IDebugProgramProvider2.GetProviderProcessData(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 pPort, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, PROVIDER_PROCESS_DATA[] pProcess)
    {
      throw new NotImplementedException();
    }

    int IDebugProgramProvider2.GetProviderProgramNode(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 pPort, AD_PROCESS_ID ProcessId, ref Guid guidEngine, ulong programId, out IDebugProgramNode2 ppProgramNode)
    {
      throw new NotImplementedException();
    }

    int IDebugProgramProvider2.SetLocale(ushort wLangID)
    {
      return VSConstants.S_OK;
    }

    int IDebugProgramProvider2.WatchForProviderEvents(enum_PROVIDER_FLAGS Flags, IDebugDefaultPort2 pPort, AD_PROCESS_ID ProcessId, CONST_GUID_ARRAY EngineFilter, ref Guid guidLaunchingEngine, IDebugPortNotify2 pEventCallback)
    {
      return VSConstants.S_OK;
    }

    #endregion
  }
}
