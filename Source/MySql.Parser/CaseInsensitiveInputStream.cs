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
using System.IO;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace MySql.Parser
{
  /// <summary>
  /// Case insensitive implementation of an input stream.
  /// </summary>
  public class CaseInsensitiveInputStream : ANTLRInputStream
  {
    public CaseInsensitiveInputStream( Stream s ) : base( s ) 
    {
    }

    // Only the lookahead is converted to lowercase. The original case is preserved in the stream.
    public override int LA(int i) 
    {
      if (i == 0) {
        return 0;
      }

      if (i < 0) {
        i++;
      }

      if (((p + i) - 1) >= n) {
        return (int) CharStreamConstants.EndOfFile;
      }
      // This is how "case insensitive" is defined, i.e., could also use a special culture...
      return Char.ToUpperInvariant(data[(p + i) - 1]); 
    }
  }
}
