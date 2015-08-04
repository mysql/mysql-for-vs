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

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// This language service will provide buffer support for the MyJs language.
  /// </summary>
  [ComVisible(true)]
  [Guid(MyJsLanguageService.IID)]
  [ProvideLanguageService(typeof(MyJsLanguageService),
                            MyJsLanguageService.LanguageName,
                            -1,
    // Optional language service properties
        CodeSense = true,  // General IntelliSense support
        RequestStockColors = false, // Custom colorable items
        MatchBraces = true,  // Match braces on command
        QuickInfo = true,
        ShowCompletion = true,
        MatchBracesAtCaret = true   // Match braces while typing
        )]
  class MyJsLanguageService : Microsoft.VisualStudio.Package.LanguageService
  {
    /// <summary>
    /// The language preferences that identify it.
    /// </summary>
    private LanguagePreferences _preferences;

    /// <summary>
    /// The language name.
    /// </summary>
    public const string LanguageName = "MyJs";

    /// <summary>
    /// The unique ID for the current LanguageService.
    /// </summary>
    public const string IID = "CC93F80A-2457-4510-AC24-C926049E39DF";

    /// <summary>
    /// AuthoringScope encapsulates information about the source as obtained from a parsing operation.
    /// </summary>
    /// <param name="req">The req.</param>
    /// <returns>AuthoringScope information about the source obtained from the parsing request.</returns>
    public override AuthoringScope ParseSource(ParseRequest req)
    {
      return null;
    }

    /// <summary>
    /// Gets the language name.
    /// </summary>
    public override string Name
    {
      get { return LanguageName; }
    }

    /// <summary>
    /// Returns a list of file extension filters suitable for a Save As dialog box.
    /// </summary>
    /// <returns>Nothing, this method is only implemented because LanguageService inheritance requires this method to be override.</returns>
    public override string GetFormatFilterList()
    {
      return String.Empty;
    }

    /// <summary>
    /// Gets the scanner.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <returns>Returns IScanner object if Framework version is lower than 4.0.</returns>
    public override IScanner GetScanner(IVsTextLines buffer)
    {
#if CLR4 || NET_40_OR_GREATER
      return null;  // rely on MEF for colorizing
#else
      /// We don't actually need this class but we have to return a scanner object
      /// from our language service or the colorizing doesn't work.
      /// All the colorizing work happens in MySqlColorizer.
      return new MySqlScanner();
#endif
    }

    /// <summary>
    /// Gets the language preferences.
    /// </summary>
    /// <returns>Returns an instance of language preferences if not set already.</returns>
    public override LanguagePreferences GetLanguagePreferences()
    {
      if (_preferences == null)
      {
        _preferences = new LanguagePreferences(this.Site,
            typeof(MyJsLanguageService).GUID, LanguageName);
        _preferences.Init();
      }
      return _preferences;
    }

    /// <summary>
    /// Validates the breakpoint location.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <param name="line">The line.</param>
    /// <param name="col">The col.</param>
    /// <param name="pCodeSpan">The p code span.</param>
    /// <returns>Integer return value which determines if the given location can have a breakpoint applied to it.</returns>
    public override int ValidateBreakpointLocation(IVsTextBuffer buffer, int line, int col, TextSpan[] pCodeSpan)
    {
      return base.ValidateBreakpointLocation(buffer, line, col, pCodeSpan);
    }
  }
}
