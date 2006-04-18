// PkgCmdID.cs
// MUST match PkgCmdID.h (for the satellite DLL)
using System;

namespace MySql.VSTools
{
    static class PkgCmdIDList
    {
        public const uint cmdidMyCommand =        0x100;
        public const uint cmdidMyExplorerWindow =    0x101;
        public const int cmdidMyExplorerToolbar = 0x1000;

        //  context menus
        public const uint ServerCtxtMenu = 0x1001;
        public const uint DatabaseCtxtMenu = 0x1002;
        public const uint TablesCtxtMenu = 0x1003;
        public const uint ProceduresCtxtMenu = 0x1004;
        public const uint ProcedureCtxtMenu = 0x1005;
        public const uint TableCtxtMenu = 0x1006;
        public const uint ViewsCtxtMenu = 0x1007;
        public const uint FunctionsCtxtMenu = 0x1008;
        public const uint ViewCtxtMenu = 0x1009;
        public const uint FunctionCtxtMenu = 0x1010;

        // commands
        public const int cmdidRefresh = 0x102;
        public const int cmdidModifyConnection = 0x103;
        public const int cmdidDelete = 0x104;
        public const int cmdidRename = 0x105;
        public const int cmdidProperties = 0x106;
        public const int cmdidAddNewTable = 0x107;
        public const int cmdidAddNewProcedure = 0x108;
        public const int cmdidOpen = 0x109;
        public const int cmdidClone = 0x110;
        public const int cmdidAddNewTrigger = 0x111;
        public const int cmdidOpenTableDef = 0x112;
        public const int cmdidShowTableData = 0x113;
        public const int cmdidAddNewView = 0x114;
        public const int cmdidAddNewFunction = 0x115;
    };
}