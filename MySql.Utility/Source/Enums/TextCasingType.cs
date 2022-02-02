// Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the type of casing for texts.
  /// </summary>
  public enum TextCasingType
  {
    /// <summary>
    /// All charactes are lowered cased (e.g. lower case).
    /// </summary>
    LowerCase,

    /// <summary>
    /// All characters are upper cased (e.g. UPPER CASE).
    /// </summary>
    UpperCase,

    /// <summary>
    /// The first character of every word is upper cased, the rest lower cased (e.g. Title Case).
    /// </summary>
    TitleCase,

    /// <summary>
    /// The first character of every word is upper cased, the rest lower cased and all spaces between words are removed (e.g. PascalCase).
    /// </summary>
    PascalCase,

    /// <summary>
    /// The first character of every word except the first one is upper cased, the rest lower cased and all spaces between words are removed (e.g. camelCase).
    /// </summary>
    CamelCase,

    /// <summary>
    /// All charactes are lowered cased and all spaces between words are converted to hyphens (e.g. kebab-case).
    /// </summary>
    KebabCase,

    /// <summary>
    /// All charactes are lowered cased and all spaces between words are converted to underscores (e.g. snake_case).
    /// </summary>
    SnakeCase
  }
}
