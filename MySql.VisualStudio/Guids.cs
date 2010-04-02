// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
        //public static readonly Guid SqlEditorCmdSet = new Guid(GuidStrings.SqlEditorCmdSet);
    }

    static class GuidList
    {
        // TODO: This is wrong GUID, it must be CLSID of editor factory.
        public static readonly Guid EditorFactoryCLSID = new Guid(
            "D949EA95-EDA1-4b65-8A9E-266949A99360");

        public static readonly Guid DavinciCommandSet = new Guid(
            "732ABE75-CD80-11D0-A2DB-00AA00A3EFFF");

        public static readonly Guid StandardCommandSet = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");
    };

}