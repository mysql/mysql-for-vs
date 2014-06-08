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


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public abstract class WindowsFormsCodeGeneratorStrategy : BaseCodeGeneratorStrategy
  {
    internal protected StreamWriter Writer;
    protected string CanonicalTableName;
    internal Dictionary<string, Column> Columns;
    internal DataAccessTechnology DataAccessTech;
    protected bool ValidationsEnabled;
    protected delegate void WriterDelegate();
    protected Dictionary<string, WriterDelegate> ActionMappings;
    internal List<ColumnValidation> ValidationColumns;
    internal List<ColumnValidation> DetailValidationColumns;
    protected string ConnectionString;
    protected string TableName;
    protected string DetailTableName;
    protected string CanonicalDetailTableName;
    protected string ConstraintName;

    internal List<string> FkColumnsSource;
    internal List<string> FkColumnsDest;

    internal WindowsFormsCodeGeneratorStrategy(StrategyConfig config)
    {
      Writer = config.Writer;
      CanonicalTableName = config.CanonicalTableName;
      Columns = config.Columns;
      DataAccessTech = config.DataAccessTech;
      ValidationsEnabled = config.ValidationsEnabled;
      ValidationColumns = config.ValidationColumns;
      DetailValidationColumns = config.DetailValidationColumns;
      ConnectionString = config.ConnectionString;
      TableName = config.TableName;
      DetailTableName = config.DetailTableName;
      CanonicalDetailTableName = GetCanonicalIdentifier(DetailTableName);
      ConstraintName = config.ConstraintName;
    }

    /// <summary>
    /// Factory method to get a concrete implementation.
    /// </summary>
    /// <param name="tech"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static WindowsFormsCodeGeneratorStrategy GetInstance(StrategyConfig config)
    {
      WindowsFormsCodeGeneratorStrategy strategy = null;
      GuiType type = config.Type;
      switch (type)
      {
        case GuiType.IndividualControls:
          if (config.Language == LanguageGenerator.CSharp)
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new CSharpTypedDatasetIndividualControlsStrategy(config);
            }
            else
            {
              // Entity Framework
              strategy = new CSharpEntityFrameworkIndividualControlsStrategy(config);
            }
          }
          else
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new VBTypedDatasetIndividualControlsStrategy(config);
            }
            else
            {
              strategy = new VBEntityFrameworkIndividualControlsStrategy(config);
            }
          }
          break;
        case GuiType.Grid:
          if (config.Language == LanguageGenerator.CSharp)
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new CSharpTypedDatasetDataGridStrategy(config);
            }
            else
            {
              strategy = new CSharpEntityFrameworkDataGridStrategy(config);
            }
          }
          else
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new VBTypedDatasetDataGridStrategy(config);
            }
            else
            {
              strategy = new VBEntityFrameworkDataGridStrategy(config);
            }
          }
          break;
        case GuiType.MasterDetail:
          if (config.Language == LanguageGenerator.CSharp)
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new CSharpTypedDatasetMasterDetailStrategy(config);
            }
            else
            {
              strategy = new CSharpEntityFrameworkMasterDetailStrategy(config);
            }
          }
          else
          {
            if (config.DataAccessTech == DataAccessTechnology.TypedDataSet)
            {
              strategy = new VBTypedDatasetMasterDetailStrategy(config);
            }
            else
            {
              strategy = new VBEntityFrameworkMasterDetailStrategy(config);
            }
          }
          break;
      }
      return strategy;
    }

    public override void Execute(string LineInput)
    {
      WriterDelegate wr = null;
      if (ActionMappings.TryGetValue(LineInput.Trim(), out wr))
      {
        wr();
      }
      else
      {
        WriteNormalCode(LineInput);
      }
    }

    internal protected abstract string GetCanonicalIdentifier(string Identifier);
    internal protected abstract string GetEdmDesignerFileName();
    internal protected abstract string GetFormDesignerFileName();
    internal protected abstract string GetFormFileName();
    internal protected abstract string GetApplicationFileName();

    protected abstract void WriteUsingUserCode();
    protected abstract void WriteFormLoadCode();
    protected abstract void WriteValidationCode();
    protected abstract void WriteVariablesUserCode();
    protected abstract void WriteSaveEventCode();
    protected abstract void WriteAddEventCode();
    protected abstract void WriteDesignerControlDeclCode();
    protected abstract void WriteDesignerControlInitCode();
    protected abstract void WriteDesignerBeforeSuspendCode();
    protected abstract void WriteDesignerAfterSuspendCode();
    protected abstract void WriteBeforeResumeSuspendCode();
    protected abstract void WriteDataGridColumnInitialization();

    protected abstract void WriteControlInitialization(bool addBindings);

    protected virtual void WriteNormalCode(string LineInput)
    {
      Writer.WriteLine(LineInput);
    }

    internal virtual List<ColumnValidation> GetValidationColumns()
    {
      return ValidationColumns;
    }

    internal string CapitalizeString(string s)
    {
      return BaseWizard<BaseWizardForm, ICodeGeneratorStrategy>.CapitalizeString(s);
    }

    protected void RetrieveFkColumns()
    {
      bool sourceFirst = false;
      FkColumnsDest = new List<string>();
      FkColumnsSource = new List<string>();
      string sql = string.Format(
@"select `table_name`, `column_name`, `referenced_table_name`, `referenced_column_name`  
from information_schema.key_column_usage where `constraint_name` = '{0}'", ConstraintName);

      MySqlConnection con = new MySqlConnection(ConnectionString);
      MySqlCommand cmd = new MySqlCommand(sql, con);
      con.Open();
      try
      {
        using (MySqlDataReader r = cmd.ExecuteReader())
        {
          r.Read();
          if (r.GetString(0) == this.TableName) sourceFirst = true;
          do
          {
            if (sourceFirst)
            {
              FkColumnsSource.Add(r.GetString(1));
              FkColumnsDest.Add(r.GetString(3));
            }
            else
            {
              FkColumnsDest.Add(r.GetString(1));
              FkColumnsSource.Add(r.GetString(3));
            }
          } while (r.Read());
        }
      }
      finally
      {
        con.Close();
      }
    }
  }
}
