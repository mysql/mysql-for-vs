// Copyright (c) 2012-2014, Oracle and/or its affiliates. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Xml;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  public class MySqlWorkbenchServerCollection : IList<MySqlWorkbenchServer>
  {
    /// <summary>
    /// The collection of <see cref="MySqlWorkbenchServer"/> objects.
    /// </summary>
    private readonly List<MySqlWorkbenchServer> _servers = new List<MySqlWorkbenchServer>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchServerCollection"/> class.
    /// </summary>
    public MySqlWorkbenchServerCollection()
    {
      FileVersion = string.Empty;
    }
    
    /// <summary>
    /// Gets the version of the servers file.
    /// </summary>
    public string FileVersion { get; private set; }

    /// <summary>
    /// Returns the first <see cref="MySqlWorkbenchServer"/> in the collection with a service name that matches the given one.
    /// </summary>
    /// <param name="serviceName">The service name to look for.</param>
    /// <returns>The first <see cref="MySqlWorkbenchServer"/> in the collection with a service name that matches the given one.</returns>
    public MySqlWorkbenchServer FindByServiceName(string serviceName)
    {
      return _servers.FirstOrDefault(server => string.Compare(server.ServiceName, serviceName, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <summary>
    /// Loads the servers collection from the servers file specified in <seealso cref="MySqlWorkbench.ServersFilePath"/>.
    /// </summary>
    public void Load()
    {
      Clear();
      string file = MySqlWorkbench.ServersFilePath;
      if (!MySqlWorkbench.IsInstalled || !File.Exists(file))
      {
        return;
      }

      var reader = new XmlTextReader(file);
      var doc = new XmlDocument();
      doc.Load(reader);
      reader.Close();

      XmlElement root = doc.DocumentElement;
      if (root == null)
      {
        return;
      }

      FileVersion = root.Attributes["grt_format"].Value;
      foreach (XmlElement el in root.ChildNodes)
      {
        string type = el.Attributes["type"].Value;
        if (type != "list")
        {
          continue;
        }

        string structType = el.Attributes["content-struct-name"].Value;
        if (structType != "db.mgmt.ServerInstance")
        {
          continue;
        }

        foreach (XmlElement serverEl in el.ChildNodes)
        {
          Add(new MySqlWorkbenchServer(serverEl));
        }
      }
    }

    #region IList implementation

    /// <summary>
    /// Gets the number of elements actually contained in the list.
    /// </summary>
    public int Count
    {
      get { return _servers.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only
    /// </summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>A <see cref="MySqlWorkbenchServer"/> object at the given index position.</returns>
    public MySqlWorkbenchServer this[int index]
    {
      get
      {
        return _servers[index];
      }
      set
      {
        _servers[index] = value;
      }
    }

    /// <summary>
    /// Adds a <see cref="MySqlWorkbenchServer"/> object to the end of the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchServer"/> object to add.</param>
    public void Add(MySqlWorkbenchServer item)
    {
      _servers.Add(item);
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns><c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
    public bool Any(Func<MySqlWorkbenchServer, bool> predicate)
    {
      return _servers.Any(predicate);
    }

    /// <summary>
    /// Removes all elements from the list.
    /// </summary>
    public void Clear()
    {
      _servers.Clear();
    }

    /// <summary>
    /// Determines whether an element is in the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchServer"/> object.</param>
    /// <returns><c>true</c> if item is found in the list; otherwise, <c>false</c>.</returns>
    public bool Contains(MySqlWorkbenchServer item)
    {
      return _servers.Contains(item);
    }

    /// <summary>
    /// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from list.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(MySqlWorkbenchServer[] array, int arrayIndex)
    {
      _servers.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for the list.</returns>
    public IEnumerator<MySqlWorkbenchServer> GetEnumerator()
    {
      return _servers.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _servers.GetEnumerator();
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchServer"/> object to locate in the list.</param>
    /// <returns>The index of item if found in the list; otherwise, <c>-1</c>.</returns>
    public int IndexOf(MySqlWorkbenchServer item)
    {
      return _servers.IndexOf(item);
    }

    /// <summary>
    /// Inserts an item to the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <seealso cref="item"/> should be inserted.</param>
    /// <param name="item">The <see cref="MySqlWorkbenchServer"/> object to insert to the list.</param>
    public void Insert(int index, MySqlWorkbenchServer item)
    {
      _servers.Insert(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="MySqlWorkbenchServer"/> object from the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchServer"/> object to remove from the list.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
    public bool Remove(MySqlWorkbenchServer item)
    {
      return _servers.Remove(item);
    }

    /// <summary>
    /// Removes the element at the specified index of the list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt(int index)
    {
      _servers.RemoveAt(index);
    }

    #endregion
  }
}
