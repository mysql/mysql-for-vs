// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;
using System.Diagnostics;

namespace MySql.Debugger.VisualStudio
{
  public class AD7DebugErrorBreakpointResolution : IDebugErrorBreakpointResolution2
  {
    public AD7Breakpoint bp { get; private set; }

    public AD7DebugErrorBreakpointResolution( AD7Breakpoint mybp )
    {
      Debug.WriteLine("AD7DebugErrorBreakpointResolution: ctor");
      bp = mybp;
    }

    int IDebugErrorBreakpointResolution2.GetBreakpointType(enum_BP_TYPE[] pBPType)
    {
      Debug.WriteLine("AD7DebugErrorBreakpointResolution: GetBreakpointType");
      pBPType[0] = enum_BP_TYPE.BPT_CODE;
      return VSConstants.S_OK;
    }

    int IDebugErrorBreakpointResolution2.GetResolutionInfo(enum_BPERESI_FIELDS dwFields, BP_ERROR_RESOLUTION_INFO[] pErrorResolutionInfo)
    {
      Debug.WriteLine("AD7DebugErrorBreakpointResolution: GetResolutionInfo");
      if ( (dwFields == enum_BPERESI_FIELDS.BPERESI_ALLFIELDS) ||
        (( dwFields & enum_BPERESI_FIELDS.BPERESI_TYPE ) != 0 ) ||
        (( dwFields & enum_BPERESI_FIELDS.BPERESI_MESSAGE ) != 0 ) )
      {
        BP_RESOLUTION_INFO[] resolutionInfo = new BP_RESOLUTION_INFO[1];
        ((IDebugBreakpointResolution2)bp).GetResolutionInfo( enum_BPRESI_FIELDS.BPRESI_ALLFIELDS, resolutionInfo);
        pErrorResolutionInfo[0].dwFields = dwFields;
        pErrorResolutionInfo[0].bpResLocation = resolutionInfo[0].bpResLocation;
        pErrorResolutionInfo[0].pProgram = resolutionInfo[0].pProgram;
        pErrorResolutionInfo[0].pThread = resolutionInfo[0].pThread;
        pErrorResolutionInfo[0].dwType = enum_BP_ERROR_TYPE.BPET_GENERAL_WARNING;
        pErrorResolutionInfo[0].bstrMessage = "Breakpoint invalid in this location.";
      }
      return VSConstants.S_OK;
    }
  }
}
