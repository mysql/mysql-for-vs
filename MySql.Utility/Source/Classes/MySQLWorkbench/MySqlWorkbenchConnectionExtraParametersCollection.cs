// Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Contains functionality to save parameters related to <see cref="MySqlWorkbenchConnection"/> that do not appear in the Workbench connections file format.
  /// </summary>
  internal class MySqlWorkbenchConnectionExtraParametersCollection : XmlRepository, IList<MySqlWorkbenchConnectionExtraParameters>
  {
    #region Fields

    /// <summary>
    /// Internal list of <see cref="MySqlWorkbenchConnectionExtraParameters"/> objects.
    /// </summary>
    private List<MySqlWorkbenchConnectionExtraParameters> _extraParametersList;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnectionExtraParametersCollection"/> class.
    /// </summary>
    public MySqlWorkbenchConnectionExtraParametersCollection()
    {
      _extraParametersList = new List<MySqlWorkbenchConnectionExtraParameters>();
      CreateFileUponLoadIfNotFound = true;
      CreateFileUponSaveIfNotFound = true;
      ChildrenXPathExpression = MySqlWorkbenchConnectionCollection.CHILDREN_XPATH_EXPRESSION;
      ParentXPathExpression = MySqlWorkbenchConnectionCollection.PARENT_XPATH_EXPRESSION;
      RepositoryFilePath = string.Empty;
      WorkbenchConnectionIds = new HashSet<string>();
    }

    /// <summary>
    /// A list containing the IDs of Workbench connections currently loaded.
    /// </summary>
    public HashSet<string> WorkbenchConnectionIds { get; }

    #region IList implementation

    /// <summary>
    /// Gets the number of elements actually contained in the list.
    /// </summary>
    public int Count
    {
      get
      {
        return _extraParametersList.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>A <see cref="MySqlWorkbenchConnectionExtraParameters"/> object at the given index position.</returns>
    public MySqlWorkbenchConnectionExtraParameters this[int index]
    {
      get
      {
        return _extraParametersList[index];
      }

      set
      {
        _extraParametersList[index] = value;
      }
    }

    /// <summary>
    /// Adds a <see cref="MySqlWorkbenchConnectionExtraParameters"/> object to the end of the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchConnectionExtraParameters"/> object to add.</param>
    public void Add(MySqlWorkbenchConnectionExtraParameters item)
    {
      _extraParametersList.Add(item);
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns><c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
    public bool Any(Func<MySqlWorkbenchConnectionExtraParameters, bool> predicate)
    {
      return _extraParametersList.Any(predicate);
    }

    /// <summary>
    /// Removes all elements from the list.
    /// </summary>
    public void Clear()
    {
      _extraParametersList.Clear();
    }

    /// <summary>
    /// Determines whether an element is in the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchConnectionExtraParameters"/> object.</param>
    /// <returns><c>true</c> if item is found in the list; otherwise, <c>false</c>.</returns>
    public bool Contains(MySqlWorkbenchConnectionExtraParameters item)
    {
      return _extraParametersList.Contains(item);
    }

    /// <summary>
    /// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from list.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(MySqlWorkbenchConnectionExtraParameters[] array, int arrayIndex)
    {
      _extraParametersList.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for the list.</returns>
    public IEnumerator<MySqlWorkbenchConnectionExtraParameters> GetEnumerator()
    {
      return _extraParametersList.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _extraParametersList.GetEnumerator();
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchConnectionExtraParameters"/> object to locate in the list.</param>
    /// <returns>The index of item if found in the list; otherwise, <c>-1</c>.</returns>
    public int IndexOf(MySqlWorkbenchConnectionExtraParameters item)
    {
      return _extraParametersList.IndexOf(item);
    }

    /// <summary>
    /// Inserts an item to the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <seealso cref="item"/> should be inserted.</param>
    /// <param name="item">The <see cref="MySqlWorkbenchConnectionExtraParameters"/> object to insert to the list.</param>
    public void Insert(int index, MySqlWorkbenchConnectionExtraParameters item)
    {
      _extraParametersList.Insert(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="MySqlWorkbenchConnectionExtraParameters"/> object from the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchConnectionExtraParameters"/> object to remove from the list.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
    public bool Remove(MySqlWorkbenchConnectionExtraParameters item)
    {
      int index = _extraParametersList.IndexOf(item);
      RemoveAt(index);
      return index >= 0;
    }

    /// <summary>
    /// Removes the element at the specified index of the list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt(int index)
    {
      if (index >= 0 && index < _extraParametersList.Count)
      {
        _extraParametersList.RemoveAt(index);
      }
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="MySqlWorkbenchConnectionExtraParameters"/> object with the given id from the list.
    /// </summary>
    /// <param name="id">Id if the workbench connection to remove.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
    public bool RemoveById(string id)
    {
      int index = _extraParametersList.FindIndex(conn => conn.Id == id);
      RemoveAt(index);
      return index >= 0;
    }

    #endregion IList implementation

    /// <summary>
    /// Removes an extra parameters entry from the list of connections and saves the change in disk.
    /// </summary>
    /// <param name="connectionId">ID of the connection to remove from the list.</param>
    /// <returns><c>true</c> if the removal and saving were successful, <c>false</c> otherwise.</returns>
    public void DeleteExtraParameters(string connectionId)
    {
      if (Any(eP => eP.Id == connectionId) && RemoveById(connectionId))
      {
        DeleteRepositoryElement(connectionId);
      }
    }

    /// <summary>
    /// Creates custom elements in the XML file being created.
    /// </summary>
    /// <param name="doc">The <see cref="XmlDocument"/> that contains repository information.</param>
    protected override void CreateCustomXmlElements(ref XmlDocument doc)
    {
      base.CreateCustomXmlElements(ref doc);
      XmlElement root = doc.CreateElement("data");
      root.SetAttribute("grt_format", "2.0");
      doc.AppendChild(root);
      XmlElement valueNode = doc.CreateElement("value");
      valueNode.SetAttribute("_ptr_", this.RoughSizeOf().ToString("X16"));
      valueNode.SetAttribute("type", "list");
      valueNode.SetAttribute("content-type", "object");
      valueNode.SetAttribute("content-struct-name", "db.mgmt.Connection");
      valueNode.InnerText = string.Empty;
      root.AppendChild(valueNode);
    }

    /// <summary>
    /// Loads the child elements contained in the given root element.
    /// </summary>
    /// <param name="root">The root <see cref="XmlElement"/> from which to process child elements.</param>
    protected override void LoadChildElements(XmlElement root)
    {
      if (root == null)
      {
        return;
      }

      base.LoadChildElements(root);
      foreach (XmlElement el in root.ChildNodes)
      {
        string structType = el.Attributes["content-struct-name"].Value;
        if (structType != "db.mgmt.Connection")
        {
          continue;
        }

        foreach (var parametersElement in el.ChildNodes.Cast<XmlElement>().Where(childEl => childEl.Name == "value"))
        {
          Add(new MySqlWorkbenchConnectionExtraParameters(parametersElement));
        }
      }
    }

    /// <summary>
    /// Processes the child elements contained in the given root element for saving.
    /// </summary>
    /// <param name="root">The root <see cref="XmlNode"/> from which to process child elements.</param>
    protected override void SaveChildElements(ref XmlNode root)
    {
      base.SaveChildElements(ref root);

      // Remove orphaned extraParams whose IDs are no longer found in the Workbench connections list
      var extraParamsNotInConnectionsList = this.Where(exPar => !WorkbenchConnectionIds.Contains(exPar.Id)).ToList();
      foreach (var orphanedExtraParam in extraParamsNotInConnectionsList)
      {
        RemoveById(orphanedExtraParam.Id);
      }

      // Traverse the resulting extra parameters list to update the XML repository
      foreach (var singleExtraParams in _extraParametersList)
      {
        var extraParametersXmlElement = singleExtraParams.ToElement(root.OwnerDocument);
        if (extraParametersXmlElement == null)
        {
          continue;
        }

        var extraParametersNode = GetRepositoryElement(root, singleExtraParams.Id);
        if (extraParametersNode != null)
        {
          // Replace existing information
          root.ReplaceChild(extraParametersXmlElement, extraParametersNode);
        }
        else
        {
          // Add new information
          root.AppendChild(extraParametersXmlElement);
        }
      }
    }
  }
}
