// Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MySql.Data.VisualStudio.LanguageService
{
  internal class MySqlQuickInfoController : IIntellisenseController
  {
    private ITextView m_textView;
    private IList<ITextBuffer> m_subjectBuffers;
    private MySqlQuickInfoControllerProvider m_provider;
    private IQuickInfoSession m_session;

    internal MySqlQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, MySqlQuickInfoControllerProvider provider)
    {
      m_textView = textView;
      m_subjectBuffers = subjectBuffers;
      m_provider = provider;

      m_textView.MouseHover += this.OnTextViewMouseHover;
    }

    private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
    {
      //find the mouse position by mapping down to the subject buffer
      SnapshotPoint? point = m_textView.BufferGraph.MapDownToFirstMatch
        (new SnapshotPoint(m_textView.TextSnapshot, e.Position),
        PointTrackingMode.Positive,
        snapshot => m_subjectBuffers.Contains(snapshot.TextBuffer),
        PositionAffinity.Predecessor);

      if (point != null)
      {
        ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
        PointTrackingMode.Positive);

        if (!m_provider.QuickInfoBroker.IsQuickInfoActive(m_textView))
        {
          m_session = m_provider.QuickInfoBroker.TriggerQuickInfo(m_textView, triggerPoint, true);
        }
      }
    }

    public void Detach(ITextView textView)
    {
      if (m_textView == textView)
      {
        m_textView.MouseHover -= this.OnTextViewMouseHover;
        m_textView = null;
      }
    }

    public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
    {
    }

    public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
    {
    }
  }
}
