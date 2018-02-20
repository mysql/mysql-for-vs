// Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;


namespace MySql.Data.VisualStudio.DBExport
{
  
  public class MySqlDbExportSaveOptions
  {

    private string _connectionString;
    private List<DictionaryDbObjects> _dictionary = new List<DictionaryDbObjects>();
    private string _pathToMySqlFile;
    private MySqlDbExportOptions _dumpOptions;

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
          throw new ArgumentNullException("Dictionary");
      }
    }


    /// <summary>
    /// Gets or sets the path to the mysqldump script
    /// </summary>
    [XmlAttribute(AttributeName = "PathToMySQLFile")]
    public string PathToMySqlFile
    {
      get {
        return _pathToMySqlFile;      
      }

      set {
        _pathToMySqlFile = value;
      }   
    }


    /// <summary>
    /// Objects that holds the options
    /// to be sent to mysqldump tool
    /// </summary>
    [XmlAttribute(AttributeName = "DumpOptions")]
    public MySqlDbExportOptions DumpOptions
    {
      get {
        return _dumpOptions;        
      }

      set {
        _dumpOptions = value;
      }
    }


    public MySqlDbExportSaveOptions()
    {
      _dumpOptions = new MySqlDbExportOptions();
      _pathToMySqlFile = "";
      _dictionary = new List<DictionaryDbObjects>();
      _connectionString = new MySqlConnectionStringBuilder().ConnectionString;
    }

    public MySqlDbExportSaveOptions(MySqlDbExportOptions optionsForDump, 
                                    string completePathToMySqlFile, 
                                    Dictionary<string, BindingList<DbSelectedObjects>> dictionaryToDBObjects,
                                    string connectionStringToUse)
    {
      _dumpOptions = optionsForDump;
      _pathToMySqlFile = completePathToMySqlFile;
      _connectionString = connectionStringToUse;

      foreach (var items in dictionaryToDBObjects)
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
          xml.WriteValue(this.Connection);
          xml.WriteEndElement();
          
          //PathToMySqlFile
          xml.WriteStartElement("PathToMySQLFile");
          xml.WriteValue(this.PathToMySqlFile);
          xml.WriteEndElement();

          //DumpOptions                              
          var result = ObjectToXmlViaStringBuilder(this.DumpOptions);          
          xml.WriteRaw(result);                    
          
          //Dictionary

          result = ObjectToXmlViaStringBuilder(this.Dictionary);
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

      try
      {
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
      catch (Exception)
      {        
        throw;
      }
    }


    public static string ObjectToXmlViaStringBuilder(Object obj)
    {
      var output = new StringBuilder();
      var settings = new XmlWriterSettings { Indent = true };
      settings.OmitXmlDeclaration = true;
      using (var xmlWriter = XmlWriter.Create(output, settings))
      {
        var serializer = new XmlSerializer(obj.GetType());               
        serializer.Serialize(xmlWriter, obj);
      }

      return output.ToString();
    }

    public static Object DeSerializeXmlNodeToObject(string node, Type objectType)
    {
      if (node == null)
        throw new ArgumentNullException("Argument cannot be null");

      XmlSerializer xmlSerializer = new XmlSerializer(objectType);
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
