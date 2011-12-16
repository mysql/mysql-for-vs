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
  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("ToolTip QuickInfo Controller")]
  [ContentType("text")]
  internal class MySqlQuickInfoControllerProvider : IIntellisenseControllerProvider
  {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }

    public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
    {
      return new MySqlQuickInfoController(textView, subjectBuffers, this);
    }
  }
}
