﻿// Copyright © 2011-2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
