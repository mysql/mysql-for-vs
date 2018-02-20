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

// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace MySql.Data.VisualStudio
{
  static class PkgCmdIDList
  {
    public const uint cmdidConfig = 0x800;

    public const uint cmdCreateTable = 0x100;
    public const uint cmdAlterTable = 0x101;
    public const uint cmdGenerateTableScript = 0x102;

    public const uint cmdCreateView = 0x110;
    public const uint cmdAlterView = 0x111;

    public const uint cmdCreateProcedure = 0x120;
    public const uint cmdAlterProcedure = 0x121;
    public const uint cmdDebugProcedure = 0x122;
    

    public const uint cmdCreateFunction = 0x130;

    public const uint cmdCreateTrigger = 0x140;
    public const uint cmdAlterTrigger = 0x141;

    public const uint cmdCreateUDF = 0x150;
    public const uint cmdDelete = 0x160;        

    public const uint cmdidGlobalCreateTable = 500;
    public const uint cmdidGlobalCreateView = 501;
    public const uint cmdidGlobalCreateProcedure = 502;
    public const uint cmdidGlobalCreateFunction = 503;
    public const uint cmdidGlobalCreateUDF = 504;        
    public const uint cmdidGenerateDatabaseScript = 507;
    public const uint cmdidSchemaCompareTo = 508;
    public const uint cmdidSchemaCompare = 509;        
    public const uint cmdidDBExport = 511;
    public const uint cmdidLaunchWorkbench = 512;
    public const uint cmdidOpenUtilitiesPrompt = 513;

    public const uint cmdidAddConnection = 0x301;
    public const uint cmdidMRUList = 0x0401;
  };

  static class SharedCommands
  {
    public const int cmdidPrimaryKey = 109;
    public const int cmdidIndexesAndKeys = 675;
    public const int cmdidForeignKeys = 676;
    public const int cmdidGenerateChangeScript = 173;
  }
}