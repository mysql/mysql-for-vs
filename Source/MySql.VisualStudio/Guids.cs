// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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

// Guids.cs
// MUST match guids.h
using System;

namespace MySql.Data.VisualStudio
{
  static class GuidStrings
  {
    public const string SqlEditorFactory = "CAA648E8-D6BD-465e-A1B3-2A0BF9DA5581";
    //public const string SqlEditorCmdSet = "52C2F587-8E01-4333-BBD7-2BE0776207B8";
    public const string Package = "79A115C9-B133-4891-9E7B-242509DAD272";
    public const string CmdSet = "B87CB51F-8A01-4c5e-BF3E-5D0565D5397D";
    public const string Provider = "C6882346-E592-4da5-80BA-D2EADCDA0359";
  }

  static class Guids
  {
    public static readonly Guid Package = new Guid(GuidStrings.Package);
    public static readonly Guid Provider = new Guid(GuidStrings.Provider);
    public static readonly Guid CmdSet = new Guid(GuidStrings.CmdSet);
    public static readonly Guid SqlEditorFactory = new Guid(GuidStrings.SqlEditorFactory);   
  }

  static class GuidList
  {
    // TODO: This is wrong GUID, it must be CLSID of editor factory.
    public static readonly Guid EditorFactoryCLSID = new Guid(
        "D949EA95-EDA1-4b65-8A9E-266949A99360");

    public static readonly Guid DavinciCommandSet = new Guid(
        "732ABE75-CD80-11D0-A2DB-00AA00A3EFFF");

    public static readonly Guid StandardCommandSet = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");

    public const string guidIDEToolbarCmdSetString = "fd607f05-3661-4e12-a327-6d71ad2e269b";
    public static readonly Guid guidIDEToolbarCmdSet = new Guid(guidIDEToolbarCmdSetString);

  };

}