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
    protected string ConnectionString;
    protected string TableName;
    protected string DetailTableName;
    protected string CanonicalDetailTableName;
    protected string ConstraintName;

    internal WindowsFormsCodeGeneratorStrategy(StrategyConfig config)
    {
      Writer = config.Writer;
      CanonicalTableName = config.CanonicalTableName;
      Columns = config.Columns;
      DataAccessTech = config.DataAccessTech;
      ValidationsEnabled = config.ValidationsEnabled;
      ValidationColumns = config.ValidationColumns;
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

    protected abstract void WriteUsingUserCode();
    protected abstract void WriteFormLoadCode();
    protected abstract void WriteValidationCode();
    protected abstract void WriteVariablesUserCode();
    protected abstract void WriteSaveEventCode();
    protected abstract void WriteDesignerControlDeclCode();
    protected abstract void WriteDesignerControlInitCode();
    protected abstract void WriteDesignerBeforeSuspendCode();
    protected abstract void WriteDesignerAfterSuspendCode();
    protected abstract void WriteBeforeResumeSuspendCode();

    protected abstract void WriteControlInitialization(bool addBindings);

    protected virtual void WriteNormalCode(string LineInput)
    {
      Writer.WriteLine(LineInput);
    }
  }
}
