// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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

namespace MySql.Data.VisualStudio
{
    class ViewNode : DocumentNode, IVsTextBufferProvider
	{
        private TextBufferEditor editor = null;

        public ViewNode(DataViewHierarchyAccessor hierarchyAccessor, int id) : 
			base(hierarchyAccessor, id)
		{
            NodeId = "View";
            editor = new TextBufferEditor(hierarchyAccessor.ServiceProvider);
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
            get { return (editor as TextBufferEditor).Dirty; }
            protected set { (editor as TextBufferEditor).Dirty = value; }
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
            string sql = editor.Text.Trim();
            string lowerSql = sql.ToLowerInvariant();
            int pos = lowerSql.IndexOf("view") + 4;
            int end = pos;
            while (++end < sql.Length)
            {
                if (lowerSql[end] == '(') break;
                if (Char.IsWhiteSpace(lowerSql[end])) break;
            }
            string procName = sql.Substring(pos, end - pos).Trim();
            return procName.Trim('`');
        }

        protected override void Load()
        {
            if (IsNew)
                editor.Text = GetNewViewText();
            else
            {
                try
                {
                    string[] restrictions = new string[3];
                    restrictions[1] = Database;
                    restrictions[2] = Name;
                    DataTable views = this.GetSchema("Views", restrictions);
                    if (views.Rows.Count != 1)
                        throw new Exception(String.Format("There is no view with the name '{0}'", Name));
                    editor.Text = String.Format("ALTER VIEW `{0}` AS \r\n{1}",
                        Name, views.Rows[0]["VIEW_DEFINITION"].ToString());
                    Dirty = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load view with error: " + ex.Message);
                }
            }
        }

        public override string GetSaveSql()
        {
            return editor.Text;
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
