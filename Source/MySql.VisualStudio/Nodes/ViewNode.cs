// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.Editors;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  class ViewNode : DocumentNode, IVsTextBufferProvider
  {
    private VSCodeEditor editor = null;

    public ViewNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "View";
      editor = new VSCodeEditor((IOleServiceProvider)hierarchyAccessor.ServiceProvider);
      DocumentNode.RegisterNode(this);
    }

    public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor)
    {
      ViewNode node = new ViewNode(HierarchyAccessor, 0);
      node.Edit();
    }

    #region Properties

    public override string SchemaCollection
    {
      get { return "views"; }
    }

    public override bool Dirty
    {
      get { return editor.Dirty; }
      protected set { editor.Dirty = value; }
    }

    #endregion

    public override object GetEditor()
    {
      return editor;
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
      return LanguageServiceUtil.GetRoutineName(editor.Text);
    }

    protected override void Load()
    {
      if (IsNew)
      {
        editor.Text = GetNewViewText();
      }
      else
      {
        try
        {
          string[] restrictions = new string[3];
          restrictions[1] = Database;
          restrictions[2] = Name;
          DataTable views = this.GetSchema("Views", restrictions);
          if (views.Rows.Count != 1)
          {
            Logger.LogError(string.Format(Properties.Resources.ViewNotFound, Name), true);
          }

          var viewDefinition = views.Rows[0]["VIEW_DEFINITION"].ToString();
          if (string.IsNullOrEmpty(viewDefinition))
          {
            Logger.LogError(Properties.Resources.ShowViewPermissionMissing, true);
          }

          editor.Text = string.Format("CREATE VIEW `{0}` AS \r\n{1}", Name, viewDefinition);
          OldObjectDefinition = string.Format("CREATE VIEW `{0}` AS \r\n{1}", Name, viewDefinition);
          Dirty = false;
          OnDataLoaded();
        }
        catch (Exception ex)
        {
          Logger.LogError(string.Format(Properties.Resources.UnableToLoadViewWithError, ex.Message), true);
        }
      }
    }

    public override string GetSaveSql()
    {
      if (IsNew)
      {
        return editor.Text;
      }
      else
      {
        return string.Format("{0}{1}{2};", GetDropSQL(), BaseNode.SEPARATOR, editor.Text);
      }
    }

    #region IVsTextBufferProvider Members

    private IVsTextLines buffer;

    int IVsTextBufferProvider.GetTextBuffer(out IVsTextLines ppTextBuffer)
    {
      if (buffer == null)
      {
        Type bufferType = typeof(IVsTextLines);
        Guid riid = bufferType.GUID;
        Guid clsid = typeof(VsTextBufferClass).GUID;
        buffer = (IVsTextLines)MySqlDataProviderPackage.Instance.CreateInstance(
                             ref clsid, ref riid, typeof(object));
      }
      ppTextBuffer = buffer;
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
