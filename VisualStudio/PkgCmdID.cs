// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace MySql.Data.VisualStudio
{
    static class PkgCmdIDList
    {
        public static readonly Guid guidDataCmdSet = new Guid("501822E1-B5AF-11d0-B4DC-00A0C91506EF");
        public static readonly Guid guidStandardCmdSet97 = new Guid("5efc7975-14bc-11cf-9b2b-00aa00573819");
        public static readonly Guid guidVSPackageBasedProviderCmdSet = new Guid("373de743-ea17-4735-98b8-ce24d8be01a7");

        public const int cmdidNewQuery = 0x3528;

        public const uint cmdidMyCommand = 0x100;
        public const uint cmdidMyTool =    0x101;

        public const int cmdidCreateTable = 0x102;
        public const int cmdidOpenTableDef = 0x105;
        public const int cmdidShowTableData = 0x106;
        public const int cmdidAddNewTrigger = 0x107;

        public const int cmdidOpenProcedure = 0x108;
        public const int cmdidAddNewProcedure = 0x109;

        public const int cmdidAddNewView = 0x110;
        public const int cmdidOpenViewDefinition = 0x111;
        public const int cmdidShowViewResults = 0x112;

        public const int cmdidAddNewFunction = 0x113;


        // standard ids
        public const int cmdidDelete = 17;
        public const int cmdidCopy = 15;
    };
}