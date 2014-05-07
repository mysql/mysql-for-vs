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
using System.Data.Entity.Design;
using System.Data.Metadata.Edm;
using System.Xml;


namespace MySql.Data.VisualStudio.Wizards
{
  /// <summary>
  /// Model Generator for Entity Framework Database First.
  /// </summary>
  internal class EntityFrameworkGenerator : ModelGenerator
  {
    private static readonly string ProviderName = "MySql.Data.MySqlClient";
    private string efVersion;

    internal EntityFrameworkGenerator(MySqlConnection con, string modelName, string table, string path, string artifactNamespace, string EfVersion ) :
      base( con, modelName, table, path, artifactNamespace )
    {
      efVersion = EfVersion;
    }

    internal EntityFrameworkGenerator(MySqlConnection con, string modelName, List<string> tables, string path, string artifactNamespace, string EfVersion ) :
      base(con, modelName, tables, path, artifactNamespace)
    {
      efVersion = EfVersion;
    }

    internal override string Generate()
    {

      IList<EdmSchemaError> errors = null;

      // generate the SSDL
      string ssdlNamespace = _modelName + "Model.Store";
      EntityStoreSchemaGenerator essg =
          new EntityStoreSchemaGenerator(ProviderName, _con.ConnectionString, ssdlNamespace);
      List<EntityStoreSchemaFilterEntry> filters = new List<EntityStoreSchemaFilterEntry>();
      if (_tables != null && _tables.Count > 0)
      {
        foreach (var tablename in _tables)
        {
          filters.Add(new EntityStoreSchemaFilterEntry(null, null, tablename, EntityStoreSchemaFilterObjectTypes.Table, EntityStoreSchemaFilterEffect.Allow));
        }

      }
      else {
        filters.Add(new EntityStoreSchemaFilterEntry(null, null, _table, EntityStoreSchemaFilterObjectTypes.Table, EntityStoreSchemaFilterEffect.Allow));
      }

      
      filters.Add(new EntityStoreSchemaFilterEntry(null, null, "%", EntityStoreSchemaFilterObjectTypes.View, EntityStoreSchemaFilterEffect.Exclude));
      Version entityVersion = new Version(2, 0, 0, 0);
#if NET_40_OR_GREATER
      errors = essg.GenerateStoreMetadata(filters, entityVersion);
#else
      errors = essg.GenerateStoreMetadata(filters);
#endif

      // write out errors
      if ((errors != null && errors.Count > 0) && !OnlyWarnings(errors))
      {
        WriteErrors(errors);
        return null;
      }

      // write the SSDL to a string
      StringWriter ssdl = new StringWriter();
      XmlWriter ssdlxw = XmlWriter.Create(ssdl);
      essg.WriteStoreSchema(ssdlxw);
      ssdlxw.Flush();

      // generate the CSDL
      string csdlNamespace = _artifactNamespace; // _modelName + "Model";
      string csdlEntityContainerName = _modelName + "Entities";
      EntityModelSchemaGenerator emsg = new EntityModelSchemaGenerator( essg.EntityContainer, csdlNamespace, csdlEntityContainerName);
#if NET_40_OR_GREATER
      errors = emsg.GenerateMetadata(entityVersion);
#else
      errors = emsg.GenerateMetadata();
#endif

      // write out errors
      if (errors != null && errors.Count > 0 && !OnlyWarnings(errors))
      {
        WriteErrors(errors);
        return null;
      }

      // write CSDL to a string
      StringWriter csdl = new StringWriter();
      XmlWriter csdlxw = XmlWriter.Create(csdl);
      emsg.WriteModelSchema(csdlxw);
      csdlxw.Flush();

      // write MSL to a string
      StringWriter msl = new StringWriter();
      XmlWriter mslxw = XmlWriter.Create(msl, new XmlWriterSettings() { });
      emsg.WriteStorageMapping(mslxw);
      mslxw.Flush();

      // Write everything + glue xml to file
      string file = Path.Combine( _path, _modelName + ".edmx" );
      string sCsdl = csdl.ToString();
      string sSsdl = ssdl.ToString();
      string sMsl = msl.ToString();
      sCsdl = sCsdl.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
      sSsdl = sSsdl.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
      sMsl = sMsl.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

      FileInfo fi = new FileInfo(file);
      WriteEdmx(sCsdl, sSsdl, sMsl, fi);

#if CLR4 || NET_40_OR_GREATER
      EntityCodeGenerator gen = new EntityCodeGenerator(LanguageOption.GenerateCSharpCode);
      string outputPath = Path.Combine( _path, _modelName + ".Designer.cs.bak");
      errors = gen.GenerateCode(file, outputPath );
      FixUsings(outputPath);
#endif

      return fi.FullName;
    }

    /// <summary>
    /// Fixes several using's clauses generated wrong for EF6.
    /// </summary>
    /// <param name="outputPath"></param>
    private void FixUsings( string outputPath )
    {
      if (efVersion == BaseWizard<BaseWizardForm>.ENTITY_FRAMEWORK_VERSION_6)
      {
        string contents = File.ReadAllText(outputPath);
        contents = contents.Replace("using System.Data.EntityClient;",
          "using System.Data.Entity.Core.EntityClient;");
        contents = contents.Replace("using System.Data.Objects;",
          "using System.Data.Entity.Core.Objects;");
        contents = contents.Replace("using System.Data.Objects.DataClasses;",
          "using System.Data.Entity.Core.Objects.DataClasses;");
        File.WriteAllText(outputPath, contents);
      }
    }

    private void WriteEdmx(string csdl, string ssdl, string msl, FileInfo f)
    {
      FileStream fs = new FileStream(f.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
      StreamWriter sw = new StreamWriter(fs);
      try
      {
        // http://schemas.microsoft.com/ado/2009/11/edmx
        sw.WriteLine(
          @"<?xml version=""1.0"" encoding=""utf-8""?>
          <edmx:Edmx Version=""1.0"" xmlns:edmx=""http://schemas.microsoft.com/ado/2008/10/edmx"" >
  <edmx:Runtime>
    <edmx:StorageModels>");
        sw.WriteLine(ssdl);
        sw.WriteLine("</edmx:StorageModels><edmx:ConceptualModels>");
        sw.WriteLine(csdl);
        sw.WriteLine("</edmx:ConceptualModels><edmx:Mappings>");
        sw.WriteLine(msl);
        sw.WriteLine("</edmx:Mappings></edmx:Runtime>");
        sw.WriteLine(
@"<Designer xmlns=""http://schemas.microsoft.com/ado/2008/10/edmx"">
    <Connection>
      <DesignerInfoPropertySet>
        <DesignerProperty Name=""MetadataArtifactProcessing"" Value=""EmbedInOutputAssembly"" />
      </DesignerInfoPropertySet>
    </Connection>
    <Options>
      <DesignerInfoPropertySet>
        <DesignerProperty Name=""ValidateOnBuild"" Value=""true"" />
        <DesignerProperty Name=""EnablePluralization"" Value=""False"" />
        <DesignerProperty Name=""IncludeForeignKeysInModel"" Value=""True"" />
        <DesignerProperty Name=""CodeGenerationStrategy"" Value=""Default"" />
      </DesignerInfoPropertySet>
    </Options>
    <!-- Diagram content (shape and connector positions) -->
    <Diagrams></Diagrams>
  </Designer>");
        sw.WriteLine("</edmx:Edmx>");
      }
      finally
      {
        sw.Close();
      }
    }

    private bool OnlyWarnings(IList<EdmSchemaError> errors)
    {
      for (int i = 0; i < errors.Count; i++)
      {
        if (errors[i].Severity == EdmSchemaErrorSeverity.Error) return false;
      }
      return true;
    }

    private void WriteErrors(IList<EdmSchemaError> errors)
    {
      for (int i = 0; i < errors.Count; i++)
      {
        if (errors[i].Severity == EdmSchemaErrorSeverity.Error)
        {
          _errors.Add(errors[i].Message);
        }
      }
    }
  }
}
