// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Stores user settings for MySQL for Visual Studio.
  /// </summary>
  [XmlRoot("PluginSettings")]
  public sealed class PluginSettings
  {
    #region Properties

    /// <summary>
    /// The user preference for asking to execute the Configuration Update Tool.
    /// </summary>
    [XmlElement("AskToExecuteConfigurationUpdateTool")]
    public bool AskToExecuteConfigurationUpdateTool { get; set; }

    #endregion

    public PluginSettings()
    {
      AskToExecuteConfigurationUpdateTool = true;
    }

    /// <summary>
    /// Deserializes the user settings from the specified file.
    /// </summary>
    /// <param name="fileName">The file (in XML format) containing the user settings.</param>
    /// <returns>A <see cref="PluginSettings"/> instance with the deserialized data.</returns>
    public static PluginSettings LoadFromFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
      {
        return new PluginSettings();
      }

      try
      {
        var xmlSerializer = new XmlSerializer(typeof(PluginSettings));
        PluginSettings settings = null;
        using (var reader = new StreamReader(fileName))
        {
          settings = (PluginSettings)xmlSerializer.Deserialize(reader);
          reader.Close();
        }

        return settings;
      }
      catch (Exception ex)
      {
        Trace.TraceError(ex.Message);
        return new PluginSettings();
      }
    }

    /// <summary>
    /// Serializes the user settings to the specified file.
    /// </summary>
    /// <param name="settings">The settings to serialize.</param>
    /// <param name="fileName">The name of the file where the settings will be saved to.</param>
    public static void SaveObjectToXml(PluginSettings settings, string fileName)
    {
      try
      {
        if (!File.Exists(fileName))
        {
          using (var stream = File.Create(fileName))
          {
            stream.Close();
          }
        }

        var serializer = new XmlSerializer(typeof(PluginSettings));
        using (var writer = new StreamWriter(fileName))
        {
          serializer.Serialize(writer, settings);
          writer.Close();
        }
      }
      catch (Exception ex)
      {
        Trace.TraceError($"Error saving object to XML file {fileName}. The error was: {ex}");
      }
    }
  }
}
