// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MySqlConnectionStringBuilder = MySQL.Utility.Classes.MySQL.MySqlConnectionStringBuilder;

namespace MySql.Data.VisualStudio.DBExport
{
  
  public class MySqlDbExportSaveOptions
  {
    private string _connectionString;
    private List<DictionaryDbObjects> _dictionary = new List<DictionaryDbObjects>();

    /// <summary>
    /// Gets or sets the connection to be used 
    /// on the dump operation
    /// </summary>
    [XmlAttribute(AttributeName = "ConnectionString")]
    public string Connection
    {
      get
      {
        return _connectionString;      
      }

      set
      {
        if (value != null)
          _connectionString = value;
        else
          throw new ArgumentNullException("Connection");
      }
    }

    /// <summary>
    /// Gets or sets the dictionary that contains a list
    /// of schemas and db objects to be exported.
    /// </summary>
    [XmlAttribute(AttributeName = "DictionaryDbObjects")]
    public List<DictionaryDbObjects> Dictionary
    {
      get
      {
        return _dictionary;
      }

      set
      {
        if (value != null)
        {
          _dictionary = value;
        }
        else
        {
          throw new ArgumentNullException("Dictionary");
        }
      }
    }


    /// <summary>
    /// Gets or sets the path to the mysqldump script
    /// </summary>
    [XmlAttribute(AttributeName = "PathToMySQLFile")]
    public string PathToMySqlFile { get; set; }


    /// <summary>
    /// Objects that holds the options
    /// to be sent to mysqldump tool
    /// </summary>
    [XmlAttribute(AttributeName = "DumpOptions")]
    public MySqlDbExportOptions DumpOptions { get; set; }


    public MySqlDbExportSaveOptions()
    {
      DumpOptions = new MySqlDbExportOptions();
      PathToMySqlFile = "";
      _dictionary = new List<DictionaryDbObjects>();
      _connectionString = new MySqlConnectionStringBuilder().ConnectionString;
    }

    public MySqlDbExportSaveOptions(MySqlDbExportOptions optionsForDump, 
                                    string completePathToMySqlFile, 
                                    Dictionary<string, BindingList<DbSelectedObjects>> dictionaryToDbObjects,
                                    string connectionStringToUse)
    {
      DumpOptions = optionsForDump;
      PathToMySqlFile = completePathToMySqlFile;
      _connectionString = connectionStringToUse;

      foreach (var items in dictionaryToDbObjects)
      {
        foreach (var item in items.Value)
	        {
              var objectToExport = new DictionaryDbObjects();
              objectToExport.DatabaseName = items.Key;
              objectToExport.ObjectName = item.DbObjectName;
              objectToExport.ObjectType = item.Kind;
              objectToExport.Selected = item.Selected;
              _dictionary.Add(objectToExport);
	        }        
      }
    }

    public bool WriteSettingsFile(string path, string fileName)
    {
      try
      {
        using (XmlTextWriter xml = new XmlTextWriter(path + @"\" + fileName + ".dumps", Encoding.Unicode))
        {
          xml.Formatting = Formatting.Indented;          
          xml.Indentation = 4;          
          xml.WriteStartDocument();
          xml.WriteStartElement("MySQLDBExportSettings");

          //Connection
          xml.WriteStartElement("ConnectionString");
          xml.WriteValue(Connection);
          xml.WriteEndElement();
          
          //PathToMySqlFile
          xml.WriteStartElement("PathToMySQLFile");
          xml.WriteValue(PathToMySqlFile);
          xml.WriteEndElement();

          //DumpOptions                              
          var result = ObjectToXmlViaStringBuilder(DumpOptions);          
          xml.WriteRaw(result);                    
          
          //Dictionary

          result = ObjectToXmlViaStringBuilder(Dictionary);
          xml.WriteRaw(result);
          
          xml.WriteEndElement();
          xml.WriteEndDocument();
        }       
        return true;        
      }
      catch  {       
        return false;
      }    
    }

    public static MySqlDbExportSaveOptions LoadSettingsFile(string completeFilePath)
    {
      if (!File.Exists(completeFilePath))
        return null;

      var savedSettings = new MySqlDbExportSaveOptions();

      //load objects
      XmlDocument doc = new XmlDocument();
      doc.Load(completeFilePath);
      foreach (XmlNode node in doc)
      {
        foreach (XmlNode childNode in node.ChildNodes)
        {
          if (childNode.Name == "PathToMySQLFile")
          {
            savedSettings.PathToMySqlFile = childNode.InnerText;                        
          }

          if (childNode.Name == "ConnectionString")
          {
            savedSettings.Connection = childNode.InnerText;            
          }

          if (childNode.Name == "MySqlDbExportOptions")
          {
            savedSettings.DumpOptions = (MySqlDbExportOptions)DeSerializeXmlNodeToObject(childNode.OuterXml, savedSettings.DumpOptions.GetType());             
          }

          if (childNode.Name == "ArrayOfDictionaryDbObjects")
          {
            savedSettings.Dictionary = (List<DictionaryDbObjects>)DeSerializeXmlNodeToObject(childNode.OuterXml, savedSettings.Dictionary.GetType());
          }
        }
      }

      return savedSettings;
    }

    public static string ObjectToXmlViaStringBuilder(object obj)
    {
      var output = new StringBuilder();
      var settings = new XmlWriterSettings
      {
        Indent = true,
        OmitXmlDeclaration = true
      };
      using (var xmlWriter = XmlWriter.Create(output, settings))
      {
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(xmlWriter, obj);
      }

      return output.ToString();
    }

    public static object DeSerializeXmlNodeToObject(string node, Type objectType)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }

      try
      {
        var reader = new StringReader(node);
        var serializer = new XmlSerializer(objectType);
        var instance = serializer.Deserialize(reader);

        return instance;
      }
      catch 
      {
        return objectType.IsByRef ? null : Activator.CreateInstance(objectType);
      }              
    }
  }

  [XmlRoot("DictionaryDbObjects"), XmlType("DictionaryDbObjects")]
  public class DictionaryDbObjects
  {
    [XmlAttribute("DatabaseName")]
    public string DatabaseName;
    [XmlAttribute("Selected")]
    public bool Selected;
    [XmlAttribute("ObjectName")]
    public string ObjectName;
    [XmlAttribute("ObjectType")]
    public DbObjectKind ObjectType;       
  }
}
