// Copyright (c) 2011-2013, Oracle and/or its affiliates. All rights reserved.
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

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Represents the provider that causes the MySQL classifier to be added to the set of classifiers.
  /// </summary>
  [Export(typeof(IClassifierProvider))]
  [ContentType("MySql")]
  [TagType(typeof(ClassificationTag))]
  internal sealed class MySqlClassifierProvider : IClassifierProvider
  {
    /// <summary>
    /// The MySQL content type.
    /// </summary>
    [Export]
    [Name("MySql")]
    [BaseDefinition("code")]
    internal static ContentTypeDefinition MySqlContentType = null;

    /// <summary>
    /// The MySQL file type.
    /// </summary>
    [Export]
    [FileExtension(".mysql")]
    [ContentType("MySql")]
    internal static FileExtensionToContentTypeDefinition MySqlFileType = null;

    /// <summary>
    /// The <see cref="IClassificationTypeRegistryService"/>.
    /// </summary>
    [Import]
    internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

    #region IClassifier

    public IClassifier GetClassifier(ITextBuffer textBuffer)
    {
      return textBuffer.Properties.GetOrCreateSingletonProperty<MySqlClassifier>(delegate { return new MySqlClassifier(ClassificationTypeRegistry); });
    }

    #endregion
  }
}
