// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// Custom implementation of TreeView
  /// </summary>
  internal class BufferedTreeView : TreeView
  {
    /// <summary>
    /// Overrides the OnHandleCreated event. A message is sent to avoid a flickr when the Nodes of the TreeView are expanded or collapsed.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnHandleCreated(EventArgs e)
    {
      SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
      base.OnHandleCreated(e);
    }

    //Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/ff486106(v=vs.85).aspx
    /// <summary>
    /// Informs the tree-view control to set extended styles.
    /// </summary>
    private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
    /// <summary>
    /// Retrieves the extended style for a tree-view control
    /// </summary>
    private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;

    //Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/bb759981(v=vs.85).aspx
    /// <summary>
    /// Specifies how the background is erased or filled
    /// </summary>
    private const int TVS_EX_DOUBLEBUFFER = 0x0004;

    //Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/ms644950(v=vs.85).aspx
    /// <summary>
    /// Sends the specified message to a window or windows.
    /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose window procedure will receive the message.</param>
    /// <param name="msg">The message to be sent.</param>
    /// <param name="wp">Additional message-specific information.</param>
    /// <param name="lp">Additional message-specific information.</param>
    /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    //Reference: https://msdn.microsoft.com/en-us/library/windows/desktop/bb759827(v=vs.85).aspx
    /// <summary>
    /// Causes a window to use a different set of visual style information than its class normally uses.
    /// </summary>
    /// <param name="hWnd">Handle to the window whose visual style information is to be changed.</param>
    /// <param name="pszSubAppName">Pointer to a string that contains the application name to use in place of the calling application's name. If this parameter is NULL, the calling application's name is used.</param>
    /// <param name="pszSubIdList">Pointer to a string that contains a semicolon-separated list of CLSID names to use in place of the actual list passed by the window's class. If this parameter is NULL, the ID list from the calling class is used.</param>
    /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    /// <summary>
    /// Creates a handle for the control to change the TreeView min/plus icon for the Arrows used in the TreeView of the document explorer Windows.
    /// </summary>
    protected override void CreateHandle()
    {
      base.CreateHandle();
      SetWindowTheme(this.Handle, "explorer", null);
    }
  }
}
