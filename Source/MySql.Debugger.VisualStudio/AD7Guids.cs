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

namespace MySql.Debugger.VisualStudio
{
  public class AD7Guids
  {
    public const string EngineString = "EEEE0740-10F7-4e5f-8BC4-1CC0AC9ED5B0";
    public static readonly Guid EngineGuid = new Guid(EngineString);

    public const string CLSIDString = "EEEE066A-1103-451f-BC7A-6AEF76558AE2";
    public static readonly Guid CLSIDGuid = new Guid(CLSIDString);

    public const string ProgramProviderString = "EEEE9AB0-511C-4bf0-BBE8-F763A73DA5EF";
    public static readonly Guid ProgramProviderGuid = new Guid(ProgramProviderString);

    public const string PortSupplierString = "EEEE547D-6B37-4F46-9567-F4AC7ACAFCBE";
    public static readonly Guid PortSupplierGuid = new Guid(ProgramProviderString);

    public const string EngineName = "MySql Stored Procedure Debug Engine";

    public const string LanguageName = "MySql language";
  }
}
