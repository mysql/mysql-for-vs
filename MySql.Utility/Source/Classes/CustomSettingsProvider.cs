// Copyright (c) 2012, 2016, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MySql.Utility.Classes
{
  public abstract class CustomSettingsProvider : SettingsProvider
  {
    public abstract string SettingsPath { get; }

    public abstract string RootElementApplicationName { get; }

    private static bool IsUserScoped(SettingsProperty property)
    {
      return property.Attributes.ContainsKey(typeof(UserScopedSettingAttribute));
    }

    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    {
      base.Initialize(ApplicationName, config);
    }

    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
      // Make sure our folder tree exists
      string path = Path.GetDirectoryName(SettingsPath);
      if (!string.IsNullOrEmpty(path))
      {
        Directory.CreateDirectory(path);
      }

      using (XmlTextWriter xml = new XmlTextWriter(SettingsPath, Encoding.Unicode))
      {
        xml.Formatting = Formatting.Indented;
        xml.Indentation = 2;
        xml.IndentChar = ' ';
        xml.WriteStartDocument();
        xml.WriteStartElement(RootElementApplicationName);
        foreach (SettingsPropertyValue propertyValue in collection.Cast<SettingsPropertyValue>().Where(propertyValue => IsUserScoped(propertyValue.Property)))
        {
          xml.WriteStartElement(propertyValue.Name);
          if (propertyValue.SerializedValue != null)
          {
            xml.WriteValue(propertyValue.SerializedValue);
          }

          xml.WriteEndElement();
        }

        xml.WriteEndElement();
        xml.WriteEndDocument();
      }
    }

    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
    {
      SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

      foreach (SettingsPropertyValue value in from SettingsProperty property in collection select new SettingsPropertyValue(property))
      {
        value.IsDirty = false;
        values.Add(value);
      }

      if (!File.Exists(SettingsPath))
      {
        return values;
      }

      using (XmlTextReader xml = new XmlTextReader(SettingsPath))
      {
        xml.ReadStartElement(RootElementApplicationName);

        while (xml.Read())
        {
          if (!xml.IsStartElement())
          {
            continue;
          }

          foreach (SettingsPropertyValue value in values.Cast<SettingsPropertyValue>().Where(value => xml.Name == value.Name))
          {
            xml.Read();
            if (!xml.IsEmptyElement && xml.NodeType != XmlNodeType.Whitespace)
            {
              value.SerializedValue = xml.ReadContentAsObject();
              xml.ReadEndElement();
            }

            break;
          }
        }
      }

      return values;
    }
  }
}
