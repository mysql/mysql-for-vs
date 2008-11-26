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

        public TableEditorPane(TableNode table) : base(null)
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
