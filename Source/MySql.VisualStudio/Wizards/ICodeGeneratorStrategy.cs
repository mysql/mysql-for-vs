// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards.WindowsForms;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Code generator strategies for the wizards are implemented as 'Strategy' pattern.
  /// </summary>
  public interface ICodeGeneratorStrategy
  {
    void Execute( string LineInput );
  }

  public enum LanguageGenerator : int
  {
    CSharp = 1,
    VBNET = 2
  };

  /// <summary>
  /// Configuration data for a WindowsFormsCodeGenerationStrategy.
  /// </summary>
  public class StrategyConfig
  {
    internal GuiType Type;
    internal IdentedStreamWriter Writer;
    internal string CanonicalTableName;
    internal Dictionary<string, Column> Columns;
    internal Dictionary<string, Column> DetailColumns;
    internal DataAccessTechnology DataAccessTech;
    internal LanguageGenerator Language;
    internal bool ValidationsEnabled;
    internal List<ColumnValidation> ValidationColumns;
    internal List<ColumnValidation> DetailValidationColumns;
    internal string ConnectionString;
    internal string ConnectionStringWithPassword;
    internal string TableName;
    internal string DetailTableName;
    internal string ConstraintName;
    internal Dictionary<string, ForeignKeyColumnInfo> ForeignKeys;
    internal Dictionary<string, ForeignKeyColumnInfo> DetailForeignKeys;

    internal StrategyConfig(IdentedStreamWriter Writer, string CanonicalTableName,
      Dictionary<string, Column> Columns, Dictionary<string, Column> DetailColumns, DataAccessTechnology DataAccessTech,
      GuiType Type, LanguageGenerator Language, bool ValidationsEnabled,
      List<ColumnValidation> ValidationColumns, List<ColumnValidation> DetailValidationColumns, string ConnectionString, string ConnectionStringWithPassword,
      string TableName, string DetailTableName, string ConstraintName, Dictionary<string, ForeignKeyColumnInfo> ForeignKeys,
      Dictionary<string, ForeignKeyColumnInfo> DetailForeignKeys)
    {
      this.Writer = Writer;
      this.CanonicalTableName = CanonicalTableName;
      this.Columns = Columns;
      this.DetailColumns = DetailColumns;
      this.DataAccessTech = DataAccessTech;
      this.Type = Type;
      this.Language = Language;
      this.ValidationsEnabled = ValidationsEnabled;
      this.ValidationColumns = ValidationColumns;
      this.ConnectionString = ConnectionString;
      this.ConnectionStringWithPassword = ConnectionStringWithPassword;
      this.TableName = TableName;
      this.DetailTableName = DetailTableName;
      this.DetailValidationColumns = DetailValidationColumns;
      this.ConstraintName = ConstraintName;
      this.ForeignKeys = ForeignKeys;
      this.DetailForeignKeys = DetailForeignKeys;
    }
  }

  public abstract class BaseCodeGeneratorStrategy : ICodeGeneratorStrategy
  {
    public abstract void Execute(string LineInput);
  }
}
