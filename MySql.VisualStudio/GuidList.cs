// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains set of GUIDs, used in this project. MUST match guids.h
 */
using System;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class contains set of GUIDs, used in this project.
    /// </summary>
    static class GuidList
    {
        /// <summary>Package GUID</summary>
        public static readonly Guid PackageGUID = new Guid(PackageGUIDString);
        /// <summary>Package GUID string</summary>
        public const string PackageGUIDString = "79A115C9-B133-4891-9E7B-242509DAD272";
        

        /// <summary> Provider GUID</summary>
        public static readonly Guid ProviderGUID = new Guid(ProviderGUIDString);
        /// <summary> Provider GUID string</summary>
        public const string ProviderGUIDString = "C6882346-E592-4da5-80BA-D2EADCDA0359";        

        #region Commands
        /// <summary>"New Query" command group</summary>
        public static readonly Guid guidDataCmdSet = new Guid("501822E1-B5AF-11d0-B4DC-00A0C91506EF");

        /// <summary>"New Query" command ID</summary>
        public const int cmdidNewQuery = 0x3528;

        /// <summary>Data View command group (Server Explorer tree)</summary>
        public const string guidMySqlProviderCmdSetString = "B87CB51F-8A01-4c5e-BF3E-5D0565D5397D";
        /// <summary>Data View command group (Server Explorer tree)</summary>
        public static readonly Guid guidMySqlProviderCmdSet = 
            new Guid(guidMySqlProviderCmdSetString);

        /// <summary>Create table command ID</summary>
        public const int cmdidCreateTable = 0x0100;
        /// <summary>Alter table command ID</summary>
        public const int cmdidAlterTable = 0x0101;
        /// <summary>Drop table command ID</summary>
        public const int cmdidDropTable = 0x0102;
        /// <summary>Clone table command ID</summary>
        public const int cmdidCloneTable = 0x0103;

        /// <summary>Create view command ID</summary>
        public const int cmdidCreateView = 0x0110;
        /// <summary>Alter view command ID</summary>
        public const int cmdidAlterView = 0x0111;
        /// <summary>Drop view command ID</summary>
        public const int cmdidDropView = 0x0112;
        /// <summary>Clone view command ID</summary>
        public const int cmdidCloneView = 0x0113;

        /// <summary>Create procedure command ID</summary>
        public const int cmdidCreateProcedure = 0x0120;
        /// <summary>Alter procedure command ID</summary>
        public const int cmdidAlterProcedure = 0x0121;
        /// <summary>Drop procedure command ID</summary>
        public const int cmdidDropProcedure = 0x0122;
        /// <summary>Clone procedure command ID</summary>
        public const int cmdidCloneProcedure = 0x0123;

        /// <summary>Create function command ID</summary>
        public const int cmdidCreateFunction = 0x0130;

        /// <summary>"Create Trigger" command ID</summary>
        public const int cmdidCreateTrigger = 0x0140;
        /// <summary>"Alter Trigger" command ID</summary>
        public const int cmdidAlterTrigger = 0x0141;
        /// <summary>"Drop Trigger" command ID</summary>
        public const int cmdidDropTrigger = 0x0142;

        /// <summary>"Create UDF" command ID</summary>
        public const int cmdidCreateUDF = 0x0150;
        /// <summary>"Alter UDF"  command ID</summary>
        public const int cmdidAlterUDF = 0x0151;
        /// <summary>"Drop UDF" command ID</summary>
        public const int cmdidDropUDF = 0x0152;

        /// <summary>"Browse or Edit data" command ID</summary>
        public const int cmdidEditTableData = 0x0160;

        /// <summary>Table editor commands</summary>
        public static readonly Guid guidTableEditorTestCmdSet = new Guid("c689689e-5621-4453-b658-9a5b65661a92"); 
        #endregion        
        
        // TODO: This is wrong GUID, it must be CLSID of editor factory.
        public static readonly Guid EditorFactoryCLSID = new Guid("D949EA95-EDA1-4b65-8A9E-266949A99360");
    };
}