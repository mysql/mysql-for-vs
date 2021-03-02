// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
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
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.Entity.Design;
using System.Data.Metadata.Edm;
using System.Xml;
using VSLangProj;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Xml.Linq;


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
    private VSProject _vsProj;

    internal EntityFrameworkGenerator(MySqlConnection con, string modelName, List<string> tables,
      string path, string artifactNamespace, string EfVersion, LanguageGenerator Language, VSProject vsProj,
      Dictionary<string, Dictionary<string, ColumnValidation>> Mappings) :
      base(con, modelName, tables, path, artifactNamespace, Language)
    {
      efVersion = EfVersion;
      _mappings = Mappings;
      _vsProj = vsProj;
    }

    internal override bool Generate()
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
      else
      {
        filters.Add(new EntityStoreSchemaFilterEntry(null, null, _table, EntityStoreSchemaFilterObjectTypes.Table, EntityStoreSchemaFilterEffect.Allow));
      }

      filters.Add(new EntityStoreSchemaFilterEntry(null, null, "%", EntityStoreSchemaFilterObjectTypes.View, EntityStoreSchemaFilterEffect.Exclude));
      Version entityVersion = new Version(2, 0, 0, 0);
#if NET_461_OR_GREATER
      errors = essg.GenerateStoreMetadata(filters, entityVersion);
#else
      errors = essg.GenerateStoreMetadata(filters);
#endif

      // write out errors
      if ((errors != null && errors.Count > 0))
      {
        WriteErrors(errors);
        SendErrorsToGeneralOuput();
      }

      // write the SSDL to a string
      StringWriter ssdl = new StringWriter();
      XmlWriter ssdlxw = XmlWriter.Create(ssdl);
      essg.WriteStoreSchema(ssdlxw);
      ssdlxw.Flush();

      // generate the CSDL
      string csdlNamespace = _artifactNamespace;
      string csdlEntityContainerName = _modelName + "Entities";

      EntityModelSchemaGenerator emsg = new EntityModelSchemaGenerator(essg.EntityContainer, csdlNamespace, csdlEntityContainerName);
#if NET_461_OR_GREATER
      emsg.GenerateForeignKeyProperties = true;

      errors = emsg.GenerateMetadata(entityVersion);
#else
      errors = emsg.GenerateMetadata();
#endif

      // write out errors
      if ((errors != null && errors.Count > 0))
      {
        WriteErrors(errors);
        SendErrorsToGeneralOuput();
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
      string file = Path.Combine(_path, _modelName + ".edmx");
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

      AddToProject(fi.FullName);

      if (_warnings.Count > 0)
      {
        SendErrorsToGeneralOuput();
        _tablesIncluded = GetTablesInModel(fi.FullName);
      }
      else
      {
        _tablesIncluded = _tables;
      }

      return true;
    }

    internal bool GenerateItemTemplates(string path, string entityName)
    {
      entityName += ".edmx";
      string file = Path.Combine(path, entityName);
      FileInfo fi = new FileInfo(file);

      if (_mappings != null && _mappings.Count != 0)
      {
        GetColumnMappingsForItemTemplates(fi, GetEntityFrameworkModelNameSpaces(file));
      }

      if (_warnings.Count > 0)
      {
        SendErrorsToGeneralOuput();
        _tablesIncluded = GetTablesInModel(fi.FullName);
      }
      else
      {
        _tablesIncluded = _tables;
      }

      return true;
    }

    private Dictionary<string, string> GetEntityFrameworkModelNameSpaces(string entityFullPath)
    {
      Dictionary<string, string> EntityFrameworkModelNamespaces = new Dictionary<string, string>();

      XDocument configFile = XDocument.Load(entityFullPath);

      //XNamespace edmxNamespace = configFile.Elements().Attributes().ElementAt(1).Value;
      XNamespace edmxNamespace;

      edmxNamespace = configFile.Elements().Attributes().FirstOrDefault(a => a.Name.LocalName == "edmx") != null
        ? configFile.Elements().Attributes().FirstOrDefault(a => a.Name.LocalName == "edmx").Value
        : string.Empty;

      //if (configFile.Elements(edmxNamespace + "Edmx").Where(a => a.Attribute(xmlnsNamespace + "edmx") != null) != null)

      EntityFrameworkModelNamespaces.Add("edmx", configFile.Elements().Attributes().FirstOrDefault(a => a.Name.LocalName == "edmx").Value);

      if (configFile.Elements(edmxNamespace + "Edmx").Elements(edmxNamespace + "Runtime").Elements(edmxNamespace + "StorageModels").Elements().Attributes().FirstOrDefault(a => a.Name == "xmlns") != null)
      {
        EntityFrameworkModelNamespaces.Add("ssdl", configFile.Elements(edmxNamespace + "Edmx").Elements(edmxNamespace + "Runtime").Elements(edmxNamespace + "StorageModels").Elements()
          .Attributes().FirstOrDefault(a => a.Name == "xmlns").Value);
      }
      if (configFile.Elements(edmxNamespace + "Edmx").Elements(edmxNamespace + "Runtime").Elements(edmxNamespace + "Mappings").Elements().Where(a => a.Attribute("xmlns") != null) != null)
      {
        EntityFrameworkModelNamespaces.Add("cs", configFile.Elements(edmxNamespace + "Edmx").Elements(edmxNamespace + "Runtime").Elements(edmxNamespace + "Mappings").Elements()
          .Attributes().FirstOrDefault(a => a.Name == "xmlns").Value);
      }
      return EntityFrameworkModelNamespaces;
    }

    private void AddToProject(string edmxPath)
    {
      ProjectItem pi = _vsProj.Project.ProjectItems.AddFromFile(edmxPath);
      // this little magic replaces having to use System.Data.Entity.Design.EntityCodeGenerator
      pi.Properties.Item("ItemType").Value = "EntityDeploy";
      pi.Properties.Item("CustomTool").Value = "EntityModelCodeGenerator";
      if (efVersion == BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.ENTITY_FRAMEWORK_VERSION_6)
      {
        // For EF6 we use DbContext instead of ObjectContext based context.
        _vsProj.DTE.SuppressUI = true;
        EnvDTE80.Solution2 sol = (EnvDTE80.Solution2)_vsProj.DTE.Solution;
        string itemPath = "";
        if (this.Language == LanguageGenerator.CSharp)
        {
          itemPath = sol.GetProjectItemTemplate("DbCtxCSEF6", "CSharp");
        }
        else
        {
          itemPath = sol.GetProjectItemTemplate("DbCtxVBEF6", "VisualBasic");
        }
        pi.ProjectItems.AddFromTemplate(itemPath, this._modelName);
        // update $edmxInputFile$
        string path = Path.GetDirectoryName(edmxPath);
        string templateName = Path.Combine(path, _modelName + ".tt");
        string contents = File.ReadAllText(templateName);
        File.WriteAllText(templateName, contents.Replace("$edmxInputFile$", _modelName + ".edmx"));
        templateName = Path.Combine(path, _modelName + ".Context.tt");
        contents = File.ReadAllText(templateName);
        File.WriteAllText(templateName, contents.Replace("$edmxInputFile$", _modelName + ".edmx"));
      }
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
        XmlNode node = l[i];
        string tableName = node.ParentNode.Attributes["StoreEntitySet"].Value;
        string propName = node.Attributes["Name"].Value;
        string colName = node.Attributes["ColumnName"].Value;
        ColumnValidation cv = null;
        Dictionary<string, ColumnValidation> cols = null;

        if (!_mappings.TryGetValue(tableName, out cols))
          continue;

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
          if (kvp2.Value.FkInfo == null)
            continue;

          string referencedTable = kvp2.Value.FkInfo.ReferencedTableName;
          string referencedColumn = kvp2.Value.LookupColumn;

          if (string.IsNullOrEmpty(referencedTable) || string.IsNullOrEmpty(referencedColumn))
            continue;

          Dictionary<string, ColumnValidation> dicLookup;

          if (_mappings.TryGetValue(referencedTable, out dicLookup))
          {
            kvp2.Value.EfLookupColumnMapping = dicLookup[kvp2.Value.LookupColumn].EfColumnMapping;
          }
        }
      }
    }

    private void GetColumnMappingsForItemTemplates(FileInfo fileInfo, Dictionary<string, string> namespaces)
    {
      if (fileInfo != null)
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(fileInfo.FullName);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

        foreach (KeyValuePair<string, string> nspace in namespaces)
        {
          nsmgr.AddNamespace(nspace.Key, nspace.Value);
        }

        string xpath = "/edmx:Edmx/edmx:Runtime/edmx:Mappings/cs:Mapping/cs:EntityContainerMapping/cs:EntitySetMapping/cs:EntityTypeMapping/cs:MappingFragment/cs:ScalarProperty";
        XmlNodeList l = doc.SelectNodes(xpath, nsmgr);

        for (int i = 0; i < l.Count; i++)
        {
          XmlNode node = l[i];
          string tableName = node.ParentNode.Attributes["StoreEntitySet"].Value;
          string propName = node.Attributes["Name"].Value;
          string colName = node.Attributes["ColumnName"].Value;
          ColumnValidation cv = null;
          Dictionary<string, ColumnValidation> cols = null;

          if (!_mappings.TryGetValue(tableName, out cols))
            continue;

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
            if (kvp2.Value.FkInfo == null)
              continue;

            string referencedTable = kvp2.Value.FkInfo.ReferencedTableName;
            string referencedColumn = kvp2.Value.LookupColumn;

            if (string.IsNullOrEmpty(referencedTable) || string.IsNullOrEmpty(referencedColumn))
              continue;

            Dictionary<string, ColumnValidation> dicLookup;

            if (_mappings.TryGetValue(referencedTable, out dicLookup))
            {
              kvp2.Value.EfLookupColumnMapping = dicLookup[kvp2.Value.LookupColumn].EfColumnMapping;
            }
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
      StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
      try
      {
        // http://schemas.microsoft.com/ado/2009/11/edmx
        sw.WriteLine(
          @"<?xml version=""1.0"" encoding=""utf-16""?>
          <edmx:Edmx Version=""1.0"" xmlns:edmx=""http://schemas.microsoft.com/ado/2008/10/edmx"" >
  <edmx:Runtime>
    <edmx:StorageModels>");
        sw.WriteLine(ssdl);
        sw.WriteLine("</edmx:StorageModels><edmx:ConceptualModels>");
        sw.WriteLine(csdl);
        sw.WriteLine("</edmx:ConceptualModels><edmx:Mappings>");
        sw.WriteLine(msl);
        sw.WriteLine("</edmx:Mappings></edmx:Runtime>");

        if (efVersion == BaseWizard<BaseWizardForm, BaseCodeGeneratorStrategy>.ENTITY_FRAMEWORK_VERSION_6)
        {
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
        <DesignerProperty Name=""UseLegacyProvider"" Value=""false"" />
        <DesignerProperty Name=""CodeGenerationStrategy"" Value=""None"" />
      </DesignerInfoPropertySet>
    </Options>
    <!-- Diagram content (shape and connector positions) -->
    <Diagrams></Diagrams>
  </Designer>");
        }
        else
        {
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
        }
        sw.WriteLine("</edmx:Edmx>");
      }
      finally
      {
        sw.Close();
      }
    }


    private List<String> GetTablesInModel(string edmxPath)
    {
      XmlDocument edmxFile = new XmlDocument();
      edmxFile.Load(edmxPath);

      XmlNamespaceManager xmlNSManager = new XmlNamespaceManager(edmxFile.NameTable);
      xmlNSManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2008/10/edmx");
      xmlNSManager.AddNamespace("ssdl", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
      xmlNSManager.AddNamespace("cs", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
      string xpath = "/edmx:Edmx/edmx:Runtime/edmx:StorageModels/ssdl:Schema/ssdl:EntityType";
      XmlNodeList entityTypes = edmxFile.DocumentElement.SelectNodes(xpath, xmlNSManager);

      List<String> tables = new List<string>();

      foreach (XmlNode entity in entityTypes)
      {
        foreach (XmlAttribute attribute in entity.Attributes)
        {
          if (attribute.Name.Equals("Name"))
          {
            tables.Add(attribute.Value);
            break;
          }
        }
      }
      return tables;
    }

    private void WriteErrors(IList<EdmSchemaError> errors)
    {
      for (int i = 0; i < errors.Count; i++)
      {
        switch (errors[i].Severity)
        {
          case EdmSchemaErrorSeverity.Error:
            _errors.Add(errors[i].Message);
            break;
          case EdmSchemaErrorSeverity.Warning:
            _warnings.Add(errors[i].Message);
            break;
          default:
            break;
        }
      }
    }

    private void SendErrorsToGeneralOuput()
    {
      if (_generalPane == null)
        return;

      _generalPane.OutputString(Environment.NewLine);
      if (Errors.Count() > 0)
      {
        _generalPane.OutputString("The following errors were found when generating the Entity Framework model:");
        foreach (var error in Errors)
        {
          _generalPane.OutputString(Environment.NewLine + error);
        }
      }
      if (Warnings.Count() > 0)
      {
        _generalPane.OutputString("The following warnings were found when generating the Entity Framework model:");
        foreach (var warning in Warnings)
        {
          _generalPane.OutputString(Environment.NewLine + warning);
        }
      }
    }
  }
}
