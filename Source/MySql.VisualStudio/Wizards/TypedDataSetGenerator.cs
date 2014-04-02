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
using Microsoft.CSharp;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Model Generator for ADO.NET Typed DataSet.
  /// </summary>
  internal class TypedDataSetGenerator : ModelGenerator
  {
    internal const string FILENAME_ARTIFACT = "NewDataSet";

    internal TypedDataSetGenerator(MySqlConnection con, string modelName, string table, string path, string ArtifactNamespace) : 
      base( con, modelName, table, path, ArtifactNamespace )
    {

    }

    internal override string Generate()
    {
      DataSet ds = new DataSet();
      MySqlConnection con = new MySqlConnection( _con.ConnectionString );
      MySqlDataAdapter da = new MySqlDataAdapter( string.Format( "select * from `{0}`", _table ), con);
      MySqlCommandBuilder builder = new MySqlCommandBuilder(da);
      da.FillSchema(ds, SchemaType.Source);

      StringBuilder sb = new StringBuilder();
      StringWriter sw = new StringWriter(sb);
      ds.WriteXmlSchema(sw);
      sw.Flush();
      string xsd = sb.ToString();
      
      CodeCompileUnit unit = new CodeCompileUnit();
      CodeNamespace ns = new CodeNamespace( _artifactNamespace );
      System.Data.Design.TypedDataSetGenerator.Generate(xsd, unit, ns,
        CodeDomProvider.CreateProvider("CSharp"), null,
        System.Data.Design.TypedDataSetGenerator.GenerateOption.HierarchicalUpdate | 
        System.Data.Design.TypedDataSetGenerator.GenerateOption.LinqOverTypedDatasets,
        ns.Name);
      string file = GenerateCSharpCode(unit, ns);
      return file;
    }

    private string GenerateCSharpCode(CodeCompileUnit compileunit, CodeNamespace ns)
    {
      CSharpCodeProvider provider = new CSharpCodeProvider();
      
      string sourceFile;
      if (provider.FileExtension[0] == '.')
      {
        sourceFile = FILENAME_ARTIFACT + provider.FileExtension;
      }
      else
      {
        sourceFile = FILENAME_ARTIFACT + "." + provider.FileExtension;
      }

      sourceFile = Path.Combine(_path, sourceFile);

      using (StreamWriter sw = new StreamWriter(sourceFile, false))
      {
        IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
        
        provider.GenerateCodeFromNamespace(ns, tw, null);
        provider.GenerateCodeFromCompileUnit(compileunit, tw,
            new CodeGeneratorOptions());
        tw.Close();
      }

      return sourceFile;
    }

  }
}
