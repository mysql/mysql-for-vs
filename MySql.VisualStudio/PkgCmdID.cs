// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace MySql.Data.VisualStudio
{
    static class PkgCmdIDList
    {
        public const uint cmdCreateTable =        0x100;
        public const uint cmdAlterTable =        0x101;
        public const uint cmdCloneTable =        0x103;

		public const uint cmdCreateView = 0x110;
		public const uint cmdAlterView = 0x111;
		public const uint cmdDropView = 0x112;
		public const uint cmdCloneView = 0x113;

		public const uint cmdCreateProcedure = 0x120;
		public const uint cmdAlterProcedure = 0x121;
		public const uint cmdCloneProcedure = 0x123;

		public const uint cmdCreateFunction = 0x130;

		public const uint cmdCreateTrigger = 0x140;
		public const uint cmdAlterTrigger = 0x141;

		public const uint cmdCreateUDF = 0x150;
		public const uint cmdDelete = 0x151;

		public const uint cmdEditTableData = 0x160;
    };

    static class SharedCommands 
    {
        public const uint cmdidPrimaryKey = 109;
        public const uint cmdidIndexesAndKeys = 675;
        public const uint cmdidForeignKeys = 676;
        public const uint cmdidGenerateChangeScript = 173;
    }
}