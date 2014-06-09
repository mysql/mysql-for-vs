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
    private Dictionary<string, Dictionary<string, ColumnValidation>> _mappings;

    internal EntityFrameworkGenerator(MySqlConnection con, string modelName, string table, 
      string path, string artifactNamespace, string EfVersion, LanguageGenerator Language, 
      Dictionary<string, Dictionary<string, ColumnValidation>> Mappings) :
      base( con, modelName, table, path, artifactNamespace, Language )
    {
      efVersion = EfVersion;
      _mappings = Mappings;
    }

    internal EntityFrameworkGenerator(MySqlConnection con, string modelName, List<string> tables, 
      string path, string artifactNamespace, string EfVersion, LanguageGenerator Language,
      Dictionary<string, Dictionary<string, ColumnValidation>> Mappings) :
      base(con, modelName, tables, path, artifactNamespace, Language)
    {
      efVersion = EfVersion;
      _mappings = Mappings;
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
      string csdlNamespace = _artifactNamespace;
      string csdlEntityContainerName = _modelName + "Entities";

      EntityModelSchemaGenerator emsg = new EntityModelSchemaGenerator( essg.EntityContainer, csdlNamespace, csdlEntityContainerName);
#if NET_40_OR_GREATER
      emsg.GenerateForeignKeyProperties = true;

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
      if (_mappings != null && _mappings.Count != 0)
        GetColumnMappings(fi);

#if CLR4 || NET_40_OR_GREATER
      EntityCodeGenerator gen = null;
      string outputPath = "";
      if( Language == LanguageGenerator.CSharp )
      {
        gen = new EntityCodeGenerator(LanguageOption.GenerateCSharpCode);
        outputPath = Path.Combine(_path, _modelName + ".Designer.cs.bak");
      } else if( Language == LanguageGenerator.VBNET ) {
        gen = new EntityCodeGenerator(LanguageOption.GenerateVBCode);
        outputPath = Path.Combine(_path, _modelName + ".Designer.vb.bak");
      }
      errors = gen.GenerateCode(file, outputPath );
      FixNamespaces(outputPath);
#endif

      return fi.FullName;
    }

    private void GetColumnMappings(FileInfo f)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(f.FullName);
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
      nsmgr.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2008/10/edmx");
      nsmgr.AddNamespace("ssdl", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
      nsmgr.AddNamespace("cs", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
      string xpath = "/edmx:Edmx/edmx:Runtime/edmx:Mappings/cs:Mapping/cs:EntityContainerMapping/cs:EntitySetMapping/cs:EntityTypeMapping/cs:MappingFragment/cs:ScalarProperty";
      XmlNodeList l = doc.SelectNodes(xpath, nsmgr);
      for (int i = 0; i < l.Count; i++)
      {
        XmlNode node = l[ i ];
        string tableName = node.ParentNode.Attributes["StoreEntitySet"].Value;
        string propName = node.Attributes["Name"].Value;
        string colName = node.Attributes["ColumnName"].Value;
        ColumnValidation cv = null;
        Dictionary<string, ColumnValidation> cols = null;
        if (!_mappings.TryGetValue(tableName, out cols)) continue;
        if (cols.TryGetValue(colName, out cv))
        {
          cv.EfColumnMapping = propName;
        }
      }
      // Solve Lookup column mappings
      foreach (KeyValuePair<string, Dictionary<string, ColumnValidation>> kvp in _mappings)
      {
        foreach (KeyValuePair<string, ColumnValidation> kvp2 in kvp.Value)
        {
          if (kvp2.Value.FkInfo == null) continue;
          string referencedTable = kvp2.Value.FkInfo.ReferencedTableName;
          string referencedColumn = kvp2.Value.LookupColumn;
          if (string.IsNullOrEmpty(referencedTable) || string.IsNullOrEmpty(referencedColumn)) continue;
          Dictionary<string, ColumnValidation> dicLookup;
          if (_mappings.TryGetValue(referencedTable, out dicLookup))
          {
            kvp2.Value.EfLookupColumnMapping = dicLookup[kvp2.Value.LookupColumn].EfColumnMapping;
          }
        }
      }
    }

    /// <summary>
    /// Fixes several namespaces generated wrong for EF6.
    /// </summary>
    /// <param name="outputPath"></param>
    private void FixNamespaces(string outputPath)
    {
      if (efVersion == BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.ENTITY_FRAMEWORK_VERSION_6)
      {
        string contents = File.ReadAllText(outputPath);

        contents = contents.Replace("System.Data.EntityClient",
            "System.Data.Entity.Core.EntityClient");
        contents = contents.Replace("System.Data.Objects",
          "System.Data.Entity.Core.Objects");
        contents = contents.Replace("System.Data.Objects.DataClasses",
          "System.Data.Entity.Core.Objects.DataClasses");
        contents = contents.Replace("System.Data.Metadata.Edm.RelationshipMultiplicity",
          "System.Data.Entity.Core.Metadata.Edm.RelationshipMultiplicity");

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
