using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text;

namespace MySql.Data.VisualStudio
{
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType("MySql")]
  [Name("token completion")]
  internal class MySqlCompletionSourceProvider : ICompletionSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
    {
      return new MySqlCompletionSource(this, textBuffer);
    }

  }
}
