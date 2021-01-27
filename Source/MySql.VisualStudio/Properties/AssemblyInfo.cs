// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MySQL for Visual Studio")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Oracle")]
[assembly: AssemblyProduct("MySQL for Visual Studio")]
[assembly: AssemblyCopyright("Copyright (c) 2008, 2021, Oracle and/or its affiliates.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5860AC5F-9DE5-4e2b-9FDA-D2AC4B1D6FA5")]

// Disable warning for "Use command line option '/keycontainer' or appropriate project settings instead of 'AssemblyKeyName'",
// since we need to sign the assembly
#pragma warning disable 1699
[assembly: AssemblyKeyName("ConnectorNet")]
#pragma warning restore 1699

[assembly: InternalsVisibleTo("MySql.VisualStudio.Tests, PublicKey = 0024000004800000940000000602000000240000525341310004000001000100d973bda91f71752c78294126974a41a08643168271f65fc0fb3cd45f658da01fbca75ac74067d18e7afbf1467d7a519ce0248b13719717281bb4ddd4ecd71a580dfe0912dfc3690b1d24c7e1975bf7eed90e4ab14e10501eedf763bff8ac204f955c9c15c2cf4ebf6563d8320b6ea8d1ea3807623141f4b81ae30a6c886b3ee1")]