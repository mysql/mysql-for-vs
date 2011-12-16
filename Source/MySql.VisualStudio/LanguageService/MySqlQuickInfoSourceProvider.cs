﻿using System;
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
  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("ToolTip QuickInfo Source")]
  [Order(Before = "Default Quick Info Presenter")]
  [ContentType("text")]
  internal class MySqlQuickInfoSourceProvider : IQuickInfoSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    [Import]
    internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

    public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
    {
      return new MySqlQuickInfoSource(this, textBuffer);
    }

  }
}
