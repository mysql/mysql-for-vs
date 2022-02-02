// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.ServiceProcess;
using System.Xml;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Classes.MySql;
using MySql.Utility.Forms;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Represents a collection of <see cref="MySqlWorkbenchConnection"/> objects.
  /// </summary>
  public class MySqlWorkbenchConnectionCollection : XmlRepository, IList<MySqlWorkbenchConnection>
  {
    #region Constants

    /// <summary>
    /// XPath expression used to find a single repository element.
    /// </summary>
    public const string CHILDREN_XPATH_EXPRESSION = "descendant::value[@struct-name='db.mgmt.Connection' and @id='{0}']";

    /// <summary>
    /// XPath expression used to find the parent node for repository elements.
    /// </summary>
    public const string PARENT_XPATH_EXPRESSION = "//data/value[@content-struct-name='db.mgmt.Connection']";

    #endregion Constants

    #region Fields

    /// <summary>
    /// Internal list of <see cref="MySqlWorkbenchConnection"/> objects.
    /// </summary>
    private List<MySqlWorkbenchConnection> _connections;

    /// <summary>
    /// Internal collection of <see cref="MySqlWorkbenchConnectionExtraParameters"/> objects.
    /// </summary>
    private MySqlWorkbenchConnectionExtraParametersCollection _extraParameters;

    /// <summary>
    /// Holds the value indicating whether default connections are created for the MySQL Server services running in the computer.
    /// </summary>
    private bool _createDefaultConnections;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnectionCollection"/> class.
    /// </summary>
    /// <param name="useMySqlWorkbenchConnectionsFile">Flag indicating if the MySQL Workbench connections file is used for connections.</param>
    /// <param name="createDefaultConnections">Flag indicating if default connections are created for the MySQL Server services running in the computer.</param>
    public MySqlWorkbenchConnectionCollection(bool useMySqlWorkbenchConnectionsFile, bool createDefaultConnections)
    {
      _connections = new List<MySqlWorkbenchConnection>();
      _extraParameters = new MySqlWorkbenchConnectionExtraParametersCollection();
      CreateFileUponLoadIfNotFound = !useMySqlWorkbenchConnectionsFile && !MySqlWorkbench.ConnectionsFileExists;
      CreateFileUponSaveIfNotFound = true;
      RepositoryFilePath = useMySqlWorkbenchConnectionsFile ? MySqlWorkbench.ConnectionsFilePath : string.Empty;
      UseMySqlWorkbenchConnectionsFile = useMySqlWorkbenchConnectionsFile;
      CreateDefaultConnections = createDefaultConnections;
      ChildrenXPathExpression = CHILDREN_XPATH_EXPRESSION;
      ParentXPathExpression = PARENT_XPATH_EXPRESSION;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnectionCollection"/> class.
    /// </summary>
    /// <param name="useMySqlWorkbenchConnectionsFile">Flag indicating if the MySQL Workbench connections file is used for connections.</param>
    public MySqlWorkbenchConnectionCollection(bool useMySqlWorkbenchConnectionsFile)
      : this(useMySqlWorkbenchConnectionsFile, false)
    {
    }

    #region Properties

    /// <summary>
    /// Gets or sets the file path of the MySQL connections file to be used.
    /// </summary>
    public string ConnectionsFilePath
    {
      get
      {
        return RepositoryFilePath;
      }

      set
      {
        if (!UseMySqlWorkbenchConnectionsFile)
        {
          RepositoryFilePath = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether default connections are created for the MySQL Server services running in the computer.
    /// </summary>
    public bool CreateDefaultConnections
    {
      get
      {
        return _createDefaultConnections;
      }

      set
      {
        _createDefaultConnections = !UseMySqlWorkbenchConnectionsFile && value;
      }
    }

    /// <summary>
    /// Gets or sets the file path of the MySQL extra parameters file to be used.
    /// </summary>
    public string ExtraParametersFilePath
    {
      get
      {
        return _extraParameters.RepositoryFilePath;
      }

      set
      {
        _extraParameters.RepositoryFilePath = value;
      }
    }

    /// <summary>
    /// Gets the file version of the connections file.
    /// </summary>
    public string FileVersion { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the MySQL Workbench connections file is the one used for this collection.
    /// </summary>
    public bool UseMySqlWorkbenchConnectionsFile { get; }

    #endregion Properties

    #region IList implementation

    /// <summary>
    /// Gets the number of elements actually contained in the list.
    /// </summary>
    public int Count
    {
      get
      {
        return _connections.Count;
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
    /// <returns>A <see cref="MySqlWorkbenchConnection"/> object at the given index position.</returns>
    public MySqlWorkbenchConnection this[int index]
    {
      get
      {
        return _connections[index];
      }

      set
      {
        _connections[index] = value;
      }
    }

    /// <summary>
    /// Adds a <see cref="MySqlWorkbenchConnection"/> object to the end of the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchConnection"/> object to add.</param>
    public void Add(MySqlWorkbenchConnection item)
    {
      _connections.Add(item);
    }

    /// <summary>
    /// Determines whether any element of a sequence satisfies a condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns><c>true</c> if any elements in the source sequence pass the test in the specified predicate; otherwise, <c>false</c>.</returns>
    public bool Any(Func<MySqlWorkbenchConnection, bool> predicate)
    {
      return _connections.Any(predicate);
    }

    /// <summary>
    /// Removes all elements from the list.
    /// </summary>
    public void Clear()
    {
      _connections.Clear();
      _extraParameters.Clear();
    }

    /// <summary>
    /// Determines whether an element is in the list.
    /// </summary>
    /// <param name="item">A <see cref="MySqlWorkbenchConnection"/> object.</param>
    /// <returns><c>true</c> if item is found in the list; otherwise, <c>false</c>.</returns>
    public bool Contains(MySqlWorkbenchConnection item)
    {
      return _connections.Contains(item);
    }

    /// <summary>
    /// Copies the entire list to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from list.
    /// The <see cref="Array"/> must have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
    public void CopyTo(MySqlWorkbenchConnection[] array, int arrayIndex)
    {
      _connections.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> for the list.</returns>
    public IEnumerator<MySqlWorkbenchConnection> GetEnumerator()
    {
      return _connections.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return _connections.GetEnumerator();
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchConnection"/> object to locate in the list.</param>
    /// <returns>The index of item if found in the list; otherwise, <c>-1</c>.</returns>
    public int IndexOf(MySqlWorkbenchConnection item)
    {
      return _connections.IndexOf(item);
    }

    /// <summary>
    /// Inserts an item to the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <seealso cref="item"/> should be inserted.</param>
    /// <param name="item">The <see cref="MySqlWorkbenchConnection"/> object to insert to the list.</param>
    public void Insert(int index, MySqlWorkbenchConnection item)
    {
      _connections.Insert(index, item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="MySqlWorkbenchConnection"/> object from the list.
    /// </summary>
    /// <param name="item">The <see cref="MySqlWorkbenchConnection"/> object to remove from the list.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
    public bool Remove(MySqlWorkbenchConnection item)
    {
      int index = _connections.IndexOf(item);
      RemoveAt(index);
      return index >= 0;
    }

    /// <summary>
    /// Removes the element at the specified index of the list.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt(int index)
    {
      if (index >= 0 && index < _connections.Count)
      {
        _connections.RemoveAt(index);
      }
    }

    /// <summary>
    /// Removes the first occurrence of a specific <see cref="MySqlWorkbenchConnection"/> object with the given id from the list.
    /// </summary>
    /// <param name="id">Id if the workbench connection to remove.</param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the list.</returns>
    public bool RemoveById(string id)
    {
      int index = _connections.FindIndex(conn => conn.Id == id);
      RemoveAt(index);
      return index >= 0;
    }

    #endregion IList implementation

    /// <summary>
    /// Removes a connection from the list of connections and saves the change in disk.
    /// </summary>
    /// <param name="connectionId">ID of the connection to remove from the list.</param>
    /// <returns><c>true</c> if the removal and saving were successful, <c>false</c> otherwise.</returns>
    public bool DeleteConnection(string connectionId)
    {
      if (UseMySqlWorkbenchConnectionsFile && MySqlWorkbench.IsRunning)
      {
        InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(Resources.GenericErrorTitle, Resources.UnableToDeleteConnectionsWhenWBRunning, Resources.CloseWBAdviceToDelete));
        return false;
      }

      bool success = true;
      try
      {
        MySqlWorkbenchConnection connection = _connections.Find(conn => conn.Id == connectionId);
        if (connection != null && RemoveById(connectionId))
        {
          DeleteRepositoryElement(connectionId);
          MySqlWorkbenchPasswordVault.DeletePassword(connection.HostIdentifier, connection.UserName);
          _extraParameters.DeleteExtraParameters(connectionId);
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, true, Resources.UnableToDeleteConnectionErrorDetail, Resources.GenericErrorTitle);
        success = false;
      }

      return success;
    }

    /// <summary>
    /// Gets the corresponding workbench connection with the given id.
    /// </summary>
    /// <param name="id">Id of the <see cref="MySqlWorkbenchConnection"/> object.</param>
    /// <returns>A matching <see cref="MySqlWorkbenchConnection"/> object.</returns>
    public MySqlWorkbenchConnection GetConnectionForId(string id)
    {
      return _connections.FirstOrDefault(conn => string.Compare(conn.Id, id, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <summary>
    /// Gets the corresponding workbench connection with the given name.
    /// </summary>
    /// <param name="name">Name of the <see cref="MySqlWorkbenchConnection"/> object.</param>
    /// <returns>A matching <see cref="MySqlWorkbenchConnection"/> object.</returns>
    public MySqlWorkbenchConnection GetConnectionForName(string name)
    {
      return _connections.FirstOrDefault(conn => string.Compare(conn.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <summary>
    /// Gets the first corresponding workbench connection with the given <see cref="MySqlConnection"/> instance.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlConnection"/> instance.</param>
    /// <param name="exactConnectionString">Flag indicating whether the comparison is based comparing full connection strings, otherwise only core host parameters are compared.</param>
    /// <returns>The first corresponding workbench connection with the given <see cref="MySqlConnection"/> instance.</returns>
    public MySqlWorkbenchConnection GetConnectionFromMySqlConnection(MySqlConnection connection, bool exactConnectionString)
    {
      if (connection == null)
      {
        return null;
      }

      var connectionStringBuilder = new MySqlConnectionStringBuilder(connection.ConnectionString);
      return exactConnectionString
        ? this.FirstOrDefault(wbConn => wbConn.GetConnectionStringBuilder().Equals(connectionStringBuilder))
        : this.FirstOrDefault(wbConn => wbConn.GetConnectionStringBuilder().CompareHostParameters(connectionStringBuilder, false));
    }

    /// <summary>
    /// Loads the connections collection from the connections file specified in <seealso cref="ConnectionsFilePath"/>.
    /// </summary>
    /// <param name="retryOrRecreate">Flag indicating whether the load operation should retry if it fails or recreate the file if fails on the retries, otherwise an error is shown.</param>
    /// <param name="retryTimes">The number of times the load operatio is retried.</param>
    /// <returns><c>true</c> if the loading operation was successful, <c>false</c> otherwise.</returns>
    public override bool Load(bool retryOrRecreate, byte retryTimes = 3)
    {
      Clear();
      var extraParametersLoadSuccess = _extraParameters.Load(retryOrRecreate, retryTimes);
      var connectionsLoadSuccess = base.Load(retryOrRecreate, retryTimes);
      return extraParametersLoadSuccess && connectionsLoadSuccess;
    }

    /// <summary>
    /// Save the connections collection to the connections file specified in <seealso cref="ConnectionsFilePath"/>.
    /// </summary>
    /// <param name="throwException">Throws the exception up.</param>
    /// <returns><c>true</c> if the saving operation was successful, <c>false</c> otherwise.</returns>
    public override bool Save(bool throwException)
    {
      bool success = base.Save(throwException);
      if (!success)
      {
        return false;
      }

      // Reset the connections to a saved status.
      foreach (var conn in _connections.Where(conn => conn.SavedStatus == MySqlWorkbenchConnection.SavedStatusType.New))
      {
        conn.SavedStatus = MySqlWorkbenchConnection.SavedStatusType.InDisk;
      }

      _extraParameters.WorkbenchConnectionIds.Clear();
      _extraParameters.WorkbenchConnectionIds.UnionWith(this.Select(conn => conn.Id));
      return _extraParameters.Save(throwException);
    }

    /// <summary>
    /// Adds a new connection to the list of connections and saves the change in disk.
    /// </summary>
    /// <param name="connection">The connection to be saved, it is added to the connections list if not already there.</param>
    /// <returns><c>true</c> if the addition and saving were successful, <c>false</c> otherwise.</returns>
    public bool SaveConnection(MySqlWorkbenchConnection connection)
    {
      // Find a connection with the same Id as the received connection in case of an update/edit.
      var updatedConnection = _connections.FirstOrDefault(conn => conn.Id == connection.Id);
      if (updatedConnection == null)
      {
        // Add connection to the collection and save it to disk if not already there.
        Add(connection);
      }
      else if (connection.SavedStatus == MySqlWorkbenchConnection.SavedStatusType.Updated)
      {
        // Sync the connections so the updated property values are copied to the connection in the collection.
        updatedConnection.Sync(connection, false);
      }

      // Find extra parameters with the same Id as the received connection in case of an update/edit.
      var updatedExtraParams = _extraParameters.FirstOrDefault(exP => exP.Id == connection.Id);
      if (updatedExtraParams == null)
      {
        // Add extra parameters to the collection and save it to disk if not already there.
        _extraParameters.Add(connection);
      }
      else
      {
        // Sync the connections so the updated property values are copied to the connection in the collection.
        updatedExtraParams.Sync(connection);
      }

      // Attempt to save the connection in file.
      bool saveSuccessful = Save(false);

      // Encrypt and store MySQL password in vault
      if (saveSuccessful && connection.PasswordChanged)
      {
        MySqlWorkbenchPasswordVault.StorePassword(connection.HostIdentifier, connection.UserName, connection.Password);
      }

      // Encrypt and store SSH password in vault
      if (saveSuccessful && connection.SshPasswordChanged)
      {
        MySqlWorkbenchPasswordVault.StorePassword(connection.SshHostIdentifier, connection.SshUserName, connection.SshPassword);
      }

      // Encrypt and store SSH pass phrase in vault
      if (saveSuccessful && connection.SshPassPhraseChanged)
      {
        MySqlWorkbenchPasswordVault.StorePassword(connection.SshHostIdentifier, connection.SshPrivateKeyFile, connection.SshPassPhrase);
      }

      return saveSuccessful;
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
    /// Creates the repository XML file.
    /// </summary>
    /// <param name="encoding">The value of the encoding attribute for the XML declaration. If <c>null</c> a default UTF-8 encoding is used.</param>
    /// <param name="standalone">The value must be either <c>"yes"</c> or <c>"no"</c>. If this is <c>null</c> or <c>strig.Empty</c>, the XML declaration will not have a standalone attribute.</param>
    /// <returns><c>true</c> if the file was created and information saved successfully, <c>false</c> otherwise.</returns>
    protected override bool CreateXmlFile(string encoding = null, string standalone = null)
    {
      if (!base.CreateXmlFile(encoding, standalone))
      {
        return false;
      }

      if (!CreateDefaultConnections)
      {
        return true;
      }

      // Create the connections for installed MySQL services
      var services = Service.GetInstances(".*mysqld.*");
      foreach (var service in services)
      {
        string serviceName = service.Properties["Name"].Value.ToString();
        var winService = new ServiceController(serviceName);
        var parameters = MySqlStartupParameters.GetStartupParameters(winService);
        if (string.IsNullOrEmpty(parameters.HostName))
        {
          continue;
        }

        var defaultConn = new MySqlWorkbenchConnection
        {
          Name = "Local instance " + serviceName,
          Host = parameters.HostName == "." ? MySqlWorkbenchConnection.DEFAULT_HOSTNAME : parameters.HostName,
          UserName = MySqlWorkbenchConnection.DEFAULT_USERNAME,
          Port = parameters.Port,
          ConnectionMethod = parameters.NamedPipesEnabled
            ? MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe
            : MySqlWorkbenchConnection.ConnectionMethodType.Tcp
        };
        if (!SaveConnection(defaultConn))
        {
          break;
        }
      }

      return true;
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
      FileVersion = root.Attributes["grt_format"].Value;
      foreach (XmlElement el in root.ChildNodes)
      {
        string type = el.Attributes["type"].Value;
        if (type != "list")
        {
          continue;
        }

        string structType = el.Attributes["content-struct-name"].Value;
        if (structType != "db.mgmt.Connection")
        {
          continue;
        }

        foreach (var workbenchConnectionElement in el.ChildNodes.Cast<XmlElement>().Where(childEl => childEl.Name == "value"))
        {
          var connectionId = workbenchConnectionElement.Attributes["id"].Value;
          var connectionExtraParameters = _extraParameters.FirstOrDefault(eP => eP.Id == connectionId);
          Add(new MySqlWorkbenchConnection(workbenchConnectionElement, connectionExtraParameters, UseMySqlWorkbenchConnectionsFile));
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
      foreach (MySqlWorkbenchConnection conn in _connections)
      {
        switch (conn.SavedStatus)
        {
          case MySqlWorkbenchConnection.SavedStatusType.New:
            root.AppendChild(conn.ToElement(root.OwnerDocument));
            break;

          case MySqlWorkbenchConnection.SavedStatusType.Updated:
            XmlNode connectionNode = GetRepositoryElement(root, conn.Id);
            if (connectionNode == null)
            {
              break;
            }

            root.ReplaceChild(conn.ToElement(root.OwnerDocument), connectionNode);
            break;
        }
      }
    }
  }
}