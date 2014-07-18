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
using VSLangProj;
using EnvDTE;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Model Generator for ADO.NET Typed DataSet.
  /// </summary>
  internal class TypedDataSetGenerator : ModelGenerator
  {
    internal const string FILENAME_ARTIFACT = "NewDataSet";
    private VSProject _vsProj;

    internal TypedDataSetGenerator(
      MySqlConnection con, string modelName, List<string> tables, string path, string artifactNamespace,
      LanguageGenerator Language, VSProject vsProj) :
      base(con, modelName, tables, path, artifactNamespace, Language)
    {
      _vsProj = vsProj;
    }

    internal override bool Generate()
    {
      DataSet dsPrev = null;
      int i = 0;
      do
      {
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

      SetTablesIncluded(dsPrev);

      StringBuilder sb = new StringBuilder();
      StringWriter sw = new StringWriter(sb);
      dsPrev.WriteXmlSchema(sw);
      sw.Flush();
      string xsd = sb.ToString();
      string xsdPath = Path.Combine(_path, FILENAME_ARTIFACT + ".xsd");
      File.WriteAllText(xsdPath, xsd, Encoding.Unicode);
      AddToProject(xsdPath);
      return true;
    }

    private void SetTablesIncluded( DataSet ds )
    {
      for (int i = 0; i < ds.Tables.Count; i++ )
      {
        DataTable t = ds.Tables[ i ];
        if( t.PrimaryKey.Length != 0 )
        {
          _tablesIncluded.Add(t.TableName);
        }
        else
        {
          _generalPane.OutputString( string.Format( "Warning: Table '{0}' does not have primary key.", t.TableName ) );
        }
      }
    }

    private void AddToProject( string xsdPath )
    {
      ProjectItem pi = _vsProj.Project.ProjectItems.AddFromFile(xsdPath);
      // this little magic replaces having to use System.Data.Design.TypedDataSetGenerator
      pi.Properties.Item("SubType").Value = "Designer";
      pi.Properties.Item("CustomTool").Value = "MSDataSetGenerator";
    }
  }
}
