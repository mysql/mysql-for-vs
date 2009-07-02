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

		public const uint cmdCreateView = 0x110;
		public const uint cmdAlterView = 0x111;

		public const uint cmdCreateProcedure = 0x120;
		public const uint cmdAlterProcedure = 0x121;

		public const uint cmdCreateFunction = 0x130;

		public const uint cmdCreateTrigger = 0x140;
		public const uint cmdAlterTrigger = 0x141;

		public const uint cmdCreateUDF = 0x150;
		public const uint cmdDelete = 0x160;

        //public const uint cmdidConfig = 0x170;

        public const uint cmdidGlobalCreateTable=500;
        public const uint cmdidGlobalCreateView=501;
        public const uint cmdidGlobalCreateProcedure=502;
        public const uint cmdidGlobalCreateFunction=503;
        public const uint cmdidGlobalCreateUDF=504;
    };

    static class SharedCommands 
    {
        public const int cmdidPrimaryKey = 109;
        public const int cmdidIndexesAndKeys = 675;
        public const int cmdidForeignKeys = 676;
        public const int cmdidGenerateChangeScript = 173;
    }
}