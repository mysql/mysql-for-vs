// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
#if NET_46_OR_GREATER
using System.Threading.Tasks;
#endif
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards.WindowsForms;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public abstract class WindowsFormsCodeGeneratorStrategy : BaseCodeGeneratorStrategy
  {
    internal IdentedStreamWriter Writer;
    protected string CanonicalTableName;
    internal Dictionary<string, Column> Columns;
    internal DataAccessTechnology DataAccessTech;
    protected bool ValidationsEnabled;
    protected delegate void WriterDelegate();
    protected Dictionary<string, WriterDelegate> ActionMappings;
    internal List<ColumnValidation> ValidationColumns;
    internal List<ColumnValidation> DetailValidationColumns;
    protected string ConnectionString;
    protected string ConnectionStringWithPassword;
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
      ConnectionStringWithPassword = config.ConnectionStringWithPassword;
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
    internal protected string GetFormResxFileName()
    {
      return "Form1.resx";
    }
    internal protected abstract string GetFormFileName();
    internal protected abstract string GetApplicationFileName();

    internal protected abstract string GetExtension();
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

    internal abstract string GetDataSourceForCombo( ColumnValidation cv );

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
      string sql = 
@"select `table_name`, `column_name`, `referenced_table_name`, `referenced_column_name`  
from information_schema.key_column_usage where `constraint_name` = '{0}' and table_schema = '{1}'";

      MySqlConnection con = new MySqlConnection(ConnectionStringWithPassword);
      con.Open();
      sql = string.Format(sql, ConstraintName, con.Database);
      MySqlCommand cmd = new MySqlCommand(sql, con);

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

  /// <summary>
  /// This class adds identation capabilities to standard StreamWriter.
  /// </summary>
  internal class IdentedStreamWriter : StreamWriter
  {
    private static readonly char IDENT_CHAR = '\t';
    private int _identationLevel = 0;
    private Stack<int> _stackIdentationLevels = new Stack<int>();
    private int lineNumber = 0;
    private StringBuilder sb = new StringBuilder();

    internal void IncreaseIdentation()
    {
      _identationLevel++;
    }

    internal void DecreaseIdentation()
    {
      _identationLevel--;
    }

    internal void PushIdentationLevel()
    {
      _stackIdentationLevels.Push(_identationLevel);
      _identationLevel = 0;
    }

    internal void PopIdentationLevel()
    {
      _identationLevel = _stackIdentationLevels.Pop();
    }

    private bool NeedIncreaseIdentation( string line )
    {
      if (line.StartsWith( "{" ) || line.StartsWith( "Namespace " ) || line.StartsWith( "Partial Class " ) ||
        line.StartsWith("Public Class") || line.StartsWith("Private Sub ") || line.Trim().EndsWith(" Then") ||
        line.StartsWith("Protected Overrides Sub "))
        return true;
      return false;
    }

    private bool NeedDecreaseIdentation( string line )
    {
      if (line.StartsWith( "}" ) || line.StartsWith( "End Sub" ) || line.StartsWith( "End Namespace" ) || 
        line.StartsWith( "End Class" ) || line.StartsWith( "End If" ) || line.StartsWith( "ElseIf" ))
        return true;
      return false;
    }

    private string GetCurrentIdentation()
    {
      return new string(IDENT_CHAR, _identationLevel);
    }
    private void WriteIdentation()
    {
      sb.Append(GetCurrentIdentation());
      base.Write( GetCurrentIdentation() );
    }
    
    public IdentedStreamWriter(Stream stream) : base( stream )
    {
    }
    
    public IdentedStreamWriter(string path) : base( path )
    {
    }
    
    public IdentedStreamWriter(Stream stream, Encoding encoding) : base(stream, encoding)
    {
    }
    
    public IdentedStreamWriter(string path, bool append) : base( path, append )
    {

    }
    
    public IdentedStreamWriter(Stream stream, Encoding encoding, int bufferSize) : base( stream, encoding, bufferSize )
    {

    }
    
    public IdentedStreamWriter(string path, bool append, Encoding encoding) : base( path, append, encoding )
    {

    }
    
#if NET_46_OR_GREATER
    public IdentedStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) : base( stream, encoding, bufferSize, leaveOpen )
    {

    }
#endif

    public IdentedStreamWriter(string path, bool append, Encoding encoding, int bufferSize) : base( path, append, encoding, bufferSize )
    {

    }

    public override void Write(char value)
    {
      Write(value.ToString());
    }

    public override void Write(char[] buffer)
    {
      Write(buffer.ToString());
    }

    public override void Write(string value)
    {
      lineNumber++;
      string line = value.Trim( new char[] { '\t', ' ' } );
      if (NeedDecreaseIdentation(line))
      {
        System.Diagnostics.Debug.WriteLine(string.Format("Decrease identation at {1} / \"{0}\"", lineNumber, line));
        DecreaseIdentation();
      }
      WriteIdentation();
      sb.Append( line );
      base.Write( line );
      if( NeedIncreaseIdentation( line ))
      {
        System.Diagnostics.Debug.WriteLine(string.Format("Increase identation at {1} / \"{0}\"", lineNumber, line));
        IncreaseIdentation();
      }
    }

    public override void WriteLine()
    {
      Write(NewLine);
    }

    public override void WriteLine( string value )
    {
      Write(value + NewLine);
    }
    
    public override void WriteLine(string format, object arg0)
    {
      WriteLine(format, new object[] { arg0 });
    }

    public override void WriteLine(string format, params object[] arg)
    {
      WriteLine(string.Format(format, arg));
    }
    
    public override void WriteLine(char[] buffer, int index, int count)
    {
      WriteLine(new string(buffer, index, count));
    }
    
    public override void WriteLine(string format, object arg0, object arg1)
    {
      WriteLine(format, new object[] { arg0, arg1 });
    }
    
    public override void WriteLine(string format, object arg0, object arg1, object arg2)
    {
      WriteLine(format, new object[] { arg0, arg1, arg2 });
    }

    public override void Write(char[] buffer, int index, int count)
    {
      Write(new string(buffer, index, count));
    }

#if NET_46_OR_GREATER
    public override Task WriteAsync(char value)
    {
      return WriteAsync(value.ToString());
    }

    public override Task WriteAsync(string value)
    {
      string line = value.Trim();
      if (NeedIncreaseIdentation(line))
      {
        IncreaseIdentation();
        WriteIdentation();
        return base.WriteAsync(value);
      }
      else if (NeedDecreaseIdentation(line))
      {
        DecreaseIdentation();
        WriteIdentation();
        return base.WriteAsync(value);
      }
      else
      {
        WriteIdentation();
        return base.WriteAsync(value);
      }
    }

    public override Task WriteAsync(char[] buffer, int index, int count)
    {
      return WriteAsync(new string(buffer, index, count));
    }

    public override Task WriteLineAsync()
    {
      WriteIdentation();
      return base.WriteLineAsync();
    }

    public override Task WriteLineAsync(char value)
    {
      return WriteLineAsync(value.ToString());
    }

    public override Task WriteLineAsync(string value)
    {
      return WriteAsync(value + NewLine);
    }

    public override Task WriteLineAsync(char[] buffer, int index, int count)
    {
      return WriteLineAsync(new string(buffer, index, count));
    }
#endif
  }
}
