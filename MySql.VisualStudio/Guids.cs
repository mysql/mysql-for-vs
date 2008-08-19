// Guids.cs
// MUST match guids.h
using System;

namespace MySql.Data.VisualStudio
{
    static class GuidList
    {
		public const string PackageGUIDString = "79A115C9-B133-4891-9E7B-242509DAD272";
		public const string guidMySqlDataPackageCmdSetString = "B87CB51F-8A01-4c5e-BF3E-5D0565D5397D";
        public const string ProviderGUIDString = "C6882346-E592-4da5-80BA-D2EADCDA0359";        

        public static readonly Guid PackageGUID = new Guid(PackageGUIDString);
        public static readonly Guid guidMySqlDataPackageCmdSet = new Guid(guidMySqlDataPackageCmdSetString);
        public static readonly Guid ProviderGUID = new Guid(ProviderGUIDString);
        // TODO: This is wrong GUID, it must be CLSID of editor factory.
        public static readonly Guid EditorFactoryCLSID = new Guid(
            "D949EA95-EDA1-4b65-8A9E-266949A99360");

        public static readonly Guid DavinciCommandSet = new Guid(
            "732ABE75-CD80-11D0-A2DB-00AA00A3EFFF");

        public static readonly Guid StandardCommandSet = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");
    };

}