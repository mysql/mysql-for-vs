// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;

namespace MySql.Data.VisualStudio
{
  [Guid("7363513B-298D-49eb-B9D9-264CE6C47540")]
  class TableEditorPane : WindowPane
  {
    TableEditor tableEditor;
    List<object> objectsForInspection = new List<object>();

    public TableEditorPane(TableNode table)
      : base(null)
    {
      tableEditor = new TableEditor(this, table);
    }

    public override IWin32Window Window
    {
      get { return tableEditor; }
    }

    protected override void OnCreate()
    {
      base.OnCreate();

      // set up our property window tracking
      SelectionContainer selContainer = new SelectionContainer();
      selContainer.SelectableObjects = objectsForInspection;
      selContainer.SelectedObjects = objectsForInspection;
      ITrackSelection trackSelectionRef = GetService(typeof(STrackSelection)) as ITrackSelection;
      trackSelectionRef.OnSelectChange(selContainer);
    }

    internal void SelectObject(object objectToInspect)
    {
      objectsForInspection.Clear();
      objectsForInspection.Add(objectToInspect);
    }

    internal void AddCommand(Guid group, int commandId, EventHandler doCmd, EventHandler queryCmd)
    {
      IMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
      if (mcs == null) return;


      CommandID cmd = new CommandID(group, commandId);
      OleMenuCommand mc = new OleMenuCommand(doCmd, cmd);
      if (queryCmd != null)
        mc.BeforeQueryStatus += queryCmd;
      mcs.AddCommand(mc);
    }
  }
}
