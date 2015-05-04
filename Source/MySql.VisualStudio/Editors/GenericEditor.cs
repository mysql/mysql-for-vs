// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Data.Common;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// GenericEditor is intended to contain methods and functionality common to all Editor clases,
  /// also provide simplified access methods to some properties Like Editor.Pane.DocumentPath through 
  /// abstract methods, reducing cohesion of SqlEditor to VsCodeEditor, VsCodeEditorWindow and 
  /// VsCoeEditorUser classes, also making these independent and reutilizable by other Editor classes.
  /// </summary>
  internal abstract class GenericEditor : BaseEditorControl
  {    
    protected DbConnection connection;
    internal DbConnection Connection { get { return connection; } }
    protected DbProviderFactory factory;    

    protected bool[] _isColBlob = null;
    internal string CurrentDatabase = null;

    /// <summary>
    /// Gets the file format list for the 'Save File' Dialog.
    /// </summary>
    /// <returns>Imlementing Editor's file extensions</returns>
    protected abstract override string GetFileFormatList();

    /// <summary>
    /// Intended to be overwriten at inheriting child, this method should provide access to the 
    /// DocumentPath of the Pane property from a given Editor class, without requiring Editor's 
    /// consumers to cast the object to a given type.
    /// </summary>    
    public abstract string GetDocumentPath();
  }
}
