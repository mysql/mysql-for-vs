using System;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents the command group GUID and IDs for the data view
	/// commands.  This information must stay consistent with the
	/// information in the CTC file compiled into the satellite dll.
	/// </summary>
	internal static class MySqlDataViewCommands
	{
		public static readonly Guid guidDataCmdSet = new Guid("501822E1-B5AF-11d0-B4DC-00A0C91506EF");
        public static readonly Guid guidVSPackageBasedProviderCmdSet = new Guid("373de743-ea17-4735-98b8-ce24d8be01a7");

		public const int cmdidNewQuery    = 0x3528;
		public const int cmdidCreateTable = 0x0102;
		public const int cmdidDropTable   = 0x0104;

        public const int cmdidOpenTableDef = 0x0105;
        public const int cmdidShowTableData = 0x0106;
        public const int cmdidAddNewTrigger = 0x0107;
        public const int cmdidOpenProcedure = 0x0108;
        public const int cmdidAddNewProcedure = 0x0109;
	}
}
