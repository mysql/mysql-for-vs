// Guids.cs
// MUST match guids.h
using System;
using Microsoft.VisualStudio.Shell;

namespace Vsip.MyVSTools
{
    static class PackageSingleton
    {
        private static MyVSTools ourPackage;
        private static int id = 0;

        public static MyVSTools Package
        {
            get { return ourPackage; }
            set { ourPackage = value; }
        }

        public static int ToolWindowId
        {
            get { return id++; }
        }
    }

    static class GuidList
    {
        public static readonly Guid guidMyVSToolsPkg = new Guid("{5ceb61c4-7111-44f8-b7f2-ac049b81ad32}");
        public static readonly Guid guidMyVSToolsCmdSet = new Guid("{9f3a024b-f2ba-4b0b-82a3-56a3ec5e3d21}");
        public static Guid guidToolWindowPersistance = new Guid("{ae5d17fe-e250-4f36-b38b-533686e0e0ca}");
        public static readonly Guid guidEditorFactory = new Guid("{7c7ae3f3-341b-4f9a-aaf9-e5e3b475ed07}");
        public static readonly Guid guidProcedureEditor = new Guid("{5117F67E-F6EE-4b78-83F1-68A7A5C2D598}");
        public static readonly Guid guidTextCateogry = new Guid("{A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0}");
    };
}