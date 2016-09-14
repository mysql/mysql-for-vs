// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.LanguageService;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes.MySql;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Nodes
{
  class ViewNode : DocumentNode, IVsTextBufferProvider
  {
    private VSCodeEditor _editor;

    public ViewNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "View";
      _editor = new VSCodeEditor((IOleServiceProvider)hierarchyAccessor.ServiceProvider);
      RegisterNode(this);
    }

    public static void CreateNew(DataViewHierarchyAccessor hierarchyAccessor)
    {
      var node = new ViewNode(hierarchyAccessor, 0);
      node.Edit();
    }

    #region Properties

    public override string SchemaCollection
    {
      get { return "views"; }
    }

    public override bool Dirty
    {
      get { return _editor.Dirty; }
      protected set { _editor.Dirty = value; }
    }

    #endregion

    public override object GetEditor()
    {
      return _editor;
    }

    private string GetNewViewText()
    {
      StringBuilder sb = new StringBuilder("CREATE VIEW ");
      sb.AppendFormat("{0}\r\n", Name);
      sb.Append("/*\r\n(column1, column2)\r\n*/\r\n");
      sb.Append("AS /* select statement */\r\n");
      return sb.ToString();
    }

    protected override string GetCurrentName()
    {
      return LanguageServiceUtil.GetRoutineName(_editor.Text);
    }

    protected override void Load()
    {
      if (IsNew)
        _editor.Text = GetNewViewText();
      else
      {
        try
        {
          string[] restrictions = new string[3];
          restrictions[1] = Database;
          restrictions[2] = Name;
          DataTable views = GetSchema("Views", restrictions);
          if (views.Rows.Count != 1)
            throw new Exception(string.Format("There is no view with the name '{0}'", Name));
          _editor.Text = string.Format("CREATE VIEW `{0}` AS \r\n{1}",
              Name, views.Rows[0]["VIEW_DEFINITION"]);
          OldObjectDefinition = string.Format("CREATE VIEW `{0}` AS \r\n{1}",
              Name, views.Rows[0]["VIEW_DEFINITION"]);
          Dirty = false;
          OnDataLoaded();
        }
        catch (Exception ex)
        {
          MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.MessageBoxErrorTitle, Resources.ViewNode_LoadViewError, true);
        }
      }
    }

    public override string GetSaveSql()
    {
      if (IsNew)
      {
        return _editor.Text;
      }
      else
      {
        return string.Format("{0}{1}{2};", GetDropSql(), SEPARATOR, _editor.Text);
      }
    }

    #region IVsTextBufferProvider Members

    private IVsTextLines _buffer;

    int IVsTextBufferProvider.GetTextBuffer(out IVsTextLines ppTextBuffer)
    {
      if (_buffer == null)
      {
        Type bufferType = typeof(IVsTextLines);
        Guid riid = bufferType.GUID;
        Guid clsid = typeof(VsTextBufferClass).GUID;
        _buffer = (IVsTextLines)MySqlDataProviderPackage.Instance.CreateInstance(
                             ref clsid, ref riid, typeof(object));
      }
      ppTextBuffer = _buffer;
      return VSConstants.S_OK;
    }

    int IVsTextBufferProvider.LockTextBuffer(int fLock)
    {
      return VSConstants.S_OK;
    }

    int IVsTextBufferProvider.SetTextBuffer(IVsTextLines pTextBuffer)
    {
      return VSConstants.S_OK;
    }

    #endregion
  }
}
