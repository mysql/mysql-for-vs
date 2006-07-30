// Guids.cs
// MUST match guids.h
using System;

namespace MySql.Data.VisualStudio
{
    static class GuidList
    {
        public const string guidPackageString = "6339d805-ee39-44f1-874c-a98e0fbc5a3e";
        public const string guidMySqlVisualStudioCmdSetString = "373de743-ea17-4735-98b8-ce24d8be01a7";
        public const string guidToolWindowPersistanceString = "ffaec6a7-e9f4-444f-b7e9-86e9ca364765";
        public const string guidEditorFactoryString = "ef2efd06-12ed-4ef3-ab46-1abc6ab7cfc9";

        public static readonly Guid guidMySqlVisualStudioPkg = new Guid(guidPackageString);
        public static readonly Guid guidMySqlVisualStudioCmdSet = new Guid(guidMySqlVisualStudioCmdSetString);
        public static readonly Guid guidToolWindowPersistance = new Guid(guidToolWindowPersistanceString);
        public static readonly Guid guidEditorFactory = new Guid(guidEditorFactoryString);
    };
}