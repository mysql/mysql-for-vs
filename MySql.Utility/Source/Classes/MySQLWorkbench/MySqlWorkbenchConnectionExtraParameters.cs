// Copyright (c) 2016, 2019, Oracle and/or its affiliates. All rights reserved.
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

using System.ComponentModel;
using System.Xml;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Contains parameters related to <see cref="MySqlWorkbenchConnection"/> that do not appear in the Workbench connections file format.
  /// </summary>
  public class MySqlWorkbenchConnectionExtraParameters : INotifyPropertyChanged
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="newId">The identifier for the new connection.</param>
    public MySqlWorkbenchConnectionExtraParameters(string newId)
    {
      Id = newId;
      Name = string.Empty;
      SavedStatus = MySqlWorkbenchConnection.SavedStatusType.New;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="extraParametersElement">XML element representing a serialized <see cref="MySqlWorkbenchConnectionExtraParameters"/>.</param>
    internal MySqlWorkbenchConnectionExtraParameters(XmlElement extraParametersElement)
      : this(extraParametersElement.Attributes["id"].Value)
    {
      foreach(XmlElement childEl in extraParametersElement.ChildNodes)
      {
        ProcessElement(childEl);
      }
    }

    #region Delegates / Events

    /// <summary>
    /// Event occurring when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Delegates / Events

    #region Properties

    /// <summary>
    /// Gets an unique identifier for this connection.
    /// </summary>
    public string Id { get; protected set; }

    /// <summary>
    /// Gets or sets this connection's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the saved status for this connection.
    /// </summary>
    public MySqlWorkbenchConnection.SavedStatusType SavedStatus { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance property values have their initial values.
    /// </summary>
    protected bool HasInitialValues
    {
      get
      {
        // If extra parameters other than Id, Name and SavedStatus are added validate that they have their initial value, otherwise return true.
        return true;
      }
    }

    #endregion Properties

    /// <summary>
    /// Synchronizes properties in the current instance with the values of the given <see cref="MySqlWorkbenchConnectionExtraParameters"/>.
    /// </summary>
    /// <param name="fromExtraParameters">A <see cref="MySqlWorkbenchConnectionExtraParameters"/> with property values to copy from.</param>
    internal void Sync(MySqlWorkbenchConnectionExtraParameters fromExtraParameters)
    {
      if (fromExtraParameters == null)
      {
        return;
      }

      Id = fromExtraParameters.Id;
      Name = fromExtraParameters.Name;
      SavedStatus = fromExtraParameters.SavedStatus;
    }

    /// <summary>
    /// Returns only the connection's parameters not present in the Workbench connections file, to store in a different XML file.
    /// </summary>
    /// <param name="doc">The XML document where this connection will be saved to.</param>
    /// <returns>A XML element representing a base Workbench connection XML node.</returns>
    internal XmlElement ToElement(XmlDocument doc)
    {
      var parameterValues = HasInitialValues ? null : GetParameterValuesElementWithBaseConnectionParent(doc);

      // If extra parameters are added call AppendPropertyValueToElement() for each of them.
      return parameterValues?.ParentNode as XmlElement;
    }

    /// <summary>
    /// Adds a property value to the corresponding serialized representation of this object.
    /// </summary>
    /// <param name="doc">XML document.</param>
    /// <param name="el">XML element representing this serialized object.</param>
    /// <param name="key">Name of the XML node representing the property.</param>
    /// <param name="type">Data type of the serialized value.</param>
    /// <param name="value">Property value.</param>
    protected void AppendPropertyValueToElement(XmlDocument doc, XmlElement el, string key, string type, object value)
    {
      InsertPropertyValueToElementAfter(doc, el, null, key, type, value);
    }

    /// <summary>
    /// Creates the base structure for a Workbench connection XML node with a child parameters value element and returns the latter.
    /// </summary>
    /// <param name="doc">The XML document where this connection will be saved to.</param>
    /// <returns>The parameters value (child) element containing a parent node representing a base Workbench connection XML node.</returns>
    protected XmlElement GetParameterValuesElementWithBaseConnectionParent(XmlDocument doc)
    {
      if (doc == null)
      {
        return null;
      }

      XmlElement connectionElement = doc.CreateElement("value");
      connectionElement.SetAttribute("type", "object");
      connectionElement.SetAttribute("struct-name", "db.mgmt.Connection");
      connectionElement.SetAttribute("id", Id);
      connectionElement.SetAttribute("struct-checksum", "0x96ba47d8");

      XmlElement parameterValues = doc.CreateElement("value");
      parameterValues.SetAttribute("_ptr_", this.RoughSizeOf().ToString("X16"));
      parameterValues.SetAttribute("type", "dict");
      parameterValues.SetAttribute("key", "parameterValues");

      connectionElement.AppendChild(parameterValues);
      AppendPropertyValueToElement(doc, connectionElement, "name", "string", Name);
      return parameterValues;
    }

    /// <summary>
    /// Adds a property value to the corresponding serialized representation of this object.
    /// </summary>
    /// <param name="doc">XML document.</param>
    /// <param name="el">XML element representing this serialized object.</param>
    /// <param name="afterEl">The <see cref="XmlElement"/> after which the new element is going to be inserted. If <c>null</c> then the new element is appended after the last one.</param>
    /// <param name="key">Name of the XML node representing the property.</param>
    /// <param name="type">Data type of the serialized value.</param>
    /// <param name="value">Property value.</param>
    protected void InsertPropertyValueToElementAfter(XmlDocument doc, XmlElement el, XmlElement afterEl, string key, string type, object value)
    {
      XmlElement val = doc.CreateElement("value");
      val.SetAttribute("type", type);
      val.SetAttribute("key", key);
      if (type.ToLower() == "string" && value == null)
      {
        value = string.Empty;
      }

      val.InnerText = value.ToString();
      val.IsEmpty = val.InnerText.Trim().Length == 0;
      if (afterEl == null)
      {
        el.AppendChild(val);
      }
      else
      {
        el.InsertAfter(val, afterEl);
      }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets properties from serialized values.
    /// </summary>
    /// <param name="el">XML element representing this serialized object.</param>
    protected void ProcessElement(XmlElement el)
    {
      string type = el.Attributes["type"].Value;
      if (type == "dict")
      {
        foreach (XmlElement childEl in el.ChildNodes)
        {
          ProcessElement(childEl);
        }

        return;
      }

      string key = el.Attributes["key"].Value;
      string value = el.InnerText;
      switch (key)
      {
        // Add a case for each value that needs to be assigned.
        case "someValue":
          // Assign or use the value of the "value" variable
          break;
      }
    }
  }
}
