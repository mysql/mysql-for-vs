// Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.IO;

namespace MySql.Debugger
{
  /// <summary>
  /// The scope of a routine, if recursivity was enabled, there could be many RoutineScopes for a single Routine.
  /// </summary>
  public class RoutineScope
  {
    /// <summary>
    /// Reference to Routine Metadata.
    /// </summary>
    public RoutineInfo OwningRoutine;

    /// <summary>
    /// A reference to a filename, in case the routine belongs to one.
    /// </summary>
    /// <remarks>This is of utility to debugger's clients, not for the core debugger itself.</remarks>
    private string _fileName;
    public string FileName { 
      get { return _fileName; }
      set { _fileName = value; }
    }

    public string GetFileName()
    {
      if (string.IsNullOrEmpty(_fileName))
      {
        string routineName = OwningRoutine.FullName;
        string path = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MySqlDebuggerCache" );
        if (!Directory.Exists(path))
        {
          Directory.CreateDirectory(path);
        }
        path = Path.Combine( path, routineName );
        _fileName = string.Format( "{0}.mysql", path );
        string fileContent = null;
        if (File.Exists(_fileName))
          fileContent = File.ReadAllText(_fileName);
        if (fileContent == null 
          || !fileContent.Equals(OwningRoutine.SourceCode))
          File.WriteAllText(_fileName, OwningRoutine.SourceCode);
      }
      return _fileName;
    }

    /// <summary>
    /// The dictionary of variables for the given scope (stack frame).
    /// </summary>
    public Dictionary<string, StoreType> Variables;

    public Breakpoint CurrentPosition { get; set; }
  }
}
