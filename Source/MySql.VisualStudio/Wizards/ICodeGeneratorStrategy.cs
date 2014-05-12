// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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

  internal enum LanguageGenerator : int
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
    internal StreamWriter Writer;
    internal string CanonicalTableName;
    internal Dictionary<string, Column> Columns;
    internal Dictionary<string, Column> DetailColumns;
    internal DataAccessTechnology DataAccessTech;
    internal LanguageGenerator Language;
    internal bool ValidationsEnabled;
    internal List<ColumnValidation> ValidationColumns;
    internal List<ColumnValidation> DetailValidationColumns;
    internal string ConnectionString;
    internal string TableName;
    internal string DetailTableName;
    internal string ConstraintName;

    internal StrategyConfig(StreamWriter Writer, string CanonicalTableName,
      Dictionary<string, Column> Columns, Dictionary<string, Column> DetailColumns, DataAccessTechnology DataAccessTech,
      GuiType Type, LanguageGenerator Language, bool ValidationsEnabled,
      List<ColumnValidation> ValidationColumns, List<ColumnValidation> DetailValidationColumns, string ConnectionString,
      string TableName, string DetailTableName, string ConstraintName )
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
      this.TableName = TableName;
      this.DetailTableName = DetailTableName;
      this.DetailValidationColumns = DetailValidationColumns;
      this.ConstraintName = ConstraintName;
    }
  }

  public abstract class BaseCodeGeneratorStrategy : ICodeGeneratorStrategy
  {
    public abstract void Execute(string LineInput);
  }
}
