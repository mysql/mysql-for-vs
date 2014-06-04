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
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Design;
using System.CodeDom;
using System.CodeDom.Compiler;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Model Generator for ADO.NET Typed DataSet.
  /// </summary>
  internal class TypedDataSetGenerator : ModelGenerator
  {
    internal const string FILENAME_ARTIFACT = "NewDataSet";
    private CodeDomProvider _provider;

    internal TypedDataSetGenerator(MySqlConnection con, string modelName, string table, string path, string ArtifactNamespace, LanguageGenerator Language) : 
      base( con, modelName, table, path, ArtifactNamespace, Language  )
    {
      _tables = new string[] { table }.ToList();
    }

    internal TypedDataSetGenerator(MySqlConnection con, string modelName, List<string> tables, string path, string artifactNamespace, LanguageGenerator Language) :
      base(con, modelName, tables, path, artifactNamespace, Language)
    {
    }

    internal override string Generate()
    {
      DataSet dsPrev = null;
      int i = 0;
      do
      {
        // ...
        MySqlDataAdapter da = new MySqlDataAdapter(string.Format("select * from `{0}`", _tables[i]), _con);
        DataSet ds = new DataSet();
        da.FillSchema(ds, SchemaType.Source);
        ds.Tables[0].TableName = BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.GetCanonicalIdentifier(_tables[i]);
        if (dsPrev != null)
        {
          dsPrev.Merge(ds.Tables[0], true, MissingSchemaAction.Add);
        } else {
          dsPrev = ds;
        }
        da.Dispose();
        ds.Dispose();
      }
      while (++i < _tables.Count);

      StringBuilder sb = new StringBuilder();
      StringWriter sw = new StringWriter(sb);
      dsPrev.WriteXmlSchema(sw);
      sw.Flush();
      string xsd = sb.ToString();
      
      CodeCompileUnit unit = new CodeCompileUnit();
      CodeNamespace ns = new CodeNamespace( _artifactNamespace );
      if (Language == LanguageGenerator.CSharp)
      {
        _provider = CodeDomProvider.CreateProvider("CSharp");
      }
      else if (Language == LanguageGenerator.VBNET)
      {
        _provider = CodeDomProvider.CreateProvider("VisualBasic" );
      }
      System.Data.Design.TypedDataSetGenerator.Generate(xsd, unit, ns, _provider, null,
        System.Data.Design.TypedDataSetGenerator.GenerateOption.HierarchicalUpdate | 
        System.Data.Design.TypedDataSetGenerator.GenerateOption.LinqOverTypedDatasets,
        ns.Name);
      string file = GenerateCode(unit, ns);
      if( Language == LanguageGenerator.VBNET )
        FixVbSourceFile(file);
      return file;
    }

    /// <summary>
    /// Implementation of Workaround for:
    /// ADO.NET's TypedDataSetGenerator for VB.NET puts Option clauses at the end of file, 
    /// but syntactically they must be at the beginning.
    /// </summary>
    /// <param name="filePath"></param>
    private void FixVbSourceFile(string filePath)
    {
      string outFile = Path.GetTempFileName();
      FileStream fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 0xffff);
      FileStream fsOut = new FileStream(outFile, FileMode.Open, FileAccess.Write, FileShare.Read, 0xffff);
      using (StreamReader sr = new StreamReader(fsIn))
      {
        using (StreamWriter sw = new StreamWriter(fsOut))
        {
          sw.WriteLine("Option Strict Off");
          sw.WriteLine("Option Explicit On");

          string line;
          while ((line = sr.ReadLine()) != null)
          {
            if (line == "Option Strict Off") continue;
            if (line == "Option Explicit On") continue;
            sw.WriteLine(line);
          }
        }
      }
      File.Delete(filePath);
      File.Move(outFile, filePath);
    }

    private string GenerateCode(CodeCompileUnit compileunit, CodeNamespace ns)
    {
      string sourceFile;
      if (_provider.FileExtension[0] == '.')
      {
        sourceFile = FILENAME_ARTIFACT + _provider.FileExtension;
      }
      else
      {
        sourceFile = FILENAME_ARTIFACT + "." + _provider.FileExtension;
      }

      sourceFile = Path.Combine(_path, sourceFile);

      using (StreamWriter sw = new StreamWriter(sourceFile, false))
      {
        IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
        
        _provider.GenerateCodeFromNamespace(ns, tw, null);
        _provider.GenerateCodeFromCompileUnit(compileunit, tw,
            new CodeGeneratorOptions());
        tw.Close();
      }

      return sourceFile;
    }

  }
}
