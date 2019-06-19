// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;

namespace MySql.Data.VisualStudio.Common
{
  /// <summary>
  /// Provides utility methods uses on various scenarios.
  /// </summary>
  public static class Utilities
  {
    /// <summary>
    /// Gets a string containing all nested <see cref="InnerException"/> messages.
    /// </summary>
    /// <param name="">The general exception.</param>
    /// <returns>A string with the full exception message.</returns>
    public static Exception GetExceptionWithFullNestedMessage(Exception exception)
    {
      if (exception == null)
      {
        return null;
      }

      if (exception.InnerException == null)
      {
        return exception;
      }

      var builder = new StringBuilder();
      builder.AppendLine(exception.Message);
      while (exception.InnerException != null)
      {
        exception = exception.InnerException;
        builder.AppendLine(exception.Message);
      }

      return new Exception(builder.ToString());
    }
  }
}
