// Copyright (c) 2013, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MySql.Utility.Classes.Logging;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Defines methods that help working with MySQL Workbench connection passwords.
  /// </summary>
  public static class MySqlWorkbenchPasswordVault
  {
    /// <summary>
    /// The consumer application's file path of the password vault file to be used.
    /// </summary>
    private static string _applicationPasswordVaultFilePath;

    /// <summary>
    /// Character used to separate the vault key elements, which are the host name and the user name.
    /// </summary>
    internal const char DOMAIN_SEPARATOR = (char)2;

    /// <summary>
    /// Character used to separate the password from the vault key.
    /// </summary>
    private const char PASSWORD_SEPARATOR = (char)3;

    /// <summary>
    /// Gets a dictionary of saved passwords related to hosts and user names.
    /// </summary>
    public static Dictionary<string, string> PasswordCache { get; private set; }

    /// <summary>
    /// Gets or sets the consumer application's file path of the password vault file to be used.
    /// </summary>
    public static string ApplicationPasswordVaultFilePath
    {
      get
      {
        if (string.IsNullOrEmpty(_applicationPasswordVaultFilePath))
        {
          throw new Exception("Passwords vault file path must be defined.");
        }

        return _applicationPasswordVaultFilePath;
      }

      set
      {
        _applicationPasswordVaultFilePath = value;
      }
    }

    /// <summary>
    /// Gets the file path of the MySQL Workbench password vault file.
    /// </summary>
    public static string WorkbenchPasswordVaultFilePath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MySQL\Workbench\workbench_user_data.dat";

    /// <summary>
    /// Deletes the password for the given host identifier and user name, if it exists.
    /// </summary>
    /// <param name="useWorkbenchPasswordsFile">Flag indicating if the passwords vault file used is the MySQL Workbench one.</param>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    public static void DeletePassword(bool useWorkbenchPasswordsFile, string hostIdentifier, string userName)
    {
      string pwdKey = hostIdentifier + DOMAIN_SEPARATOR + userName;

      int passwordsCount = LoadPasswords(useWorkbenchPasswordsFile);
      if (passwordsCount > 0 && PasswordCache.ContainsKey(pwdKey))
      {
        PasswordCache.Remove(pwdKey);
        UnloadPasswords(useWorkbenchPasswordsFile, true);
      }
      else
      {
        UnloadPasswords(useWorkbenchPasswordsFile, false);
      }
    }

    /// <summary>
    /// Deletes the password for the given host identifier and user name, if it exists.
    /// </summary>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    public static void DeletePassword(string hostIdentifier, string userName)
    {
      DeletePassword(MySqlWorkbench.UseWorkbenchConnections, hostIdentifier, userName);
    }

    /// <summary>
    /// Returns the plain text password for the given host identifier and user name.
    /// </summary>
    /// <param name="useWorkbenchPasswordsFile">Flag indicating if the passwords vault file used is the MySQL Workbench one.</param>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    /// <returns>Password in plain text.</returns>
    public static string FindPassword(bool useWorkbenchPasswordsFile, string hostIdentifier, string userName)
    {
      string password = null;
      string pwdKey = hostIdentifier + DOMAIN_SEPARATOR + userName;

      int passwordsCount = LoadPasswords(useWorkbenchPasswordsFile);
      if (passwordsCount > 0)
      {
        if (PasswordCache.ContainsKey(pwdKey))
        {
          password = PasswordCache[pwdKey];
        }

        UnloadPasswords(useWorkbenchPasswordsFile, false);
      }

      return password;
    }

    /// <summary>
    /// Returns the plain text password for the given host identifier and user name.
    /// </summary>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    /// <returns>Password in plain text.</returns>
    public static string FindPassword(string hostIdentifier, string userName)
    {
      return FindPassword(MySqlWorkbench.UseWorkbenchConnections, hostIdentifier, userName);
    }

    /// <summary>
    /// Loads encrypted passwords from disk. These are typically held only for a short moment.
    /// </summary>
    /// <param name="useWorkbenchPasswordsFile">Flag indicating if the passwords vault file used is the MySQL Workbench one.</param>
    /// <returns>Quantity of passwords loaded.</returns>
    public static int LoadPasswords(bool useWorkbenchPasswordsFile)
    {
      int passwordsQuantity = 0;
      string passwordsVaultFilePath = useWorkbenchPasswordsFile ? WorkbenchPasswordVaultFilePath : ApplicationPasswordVaultFilePath;

      if (!File.Exists(passwordsVaultFilePath))
      {
        return passwordsQuantity;
      }

      try
      {
        byte[] encryptedData = File.ReadAllBytes(passwordsVaultFilePath);
        byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
        string decryptedText = Encoding.ASCII.GetString(decryptedData);
        string[] passwordLinesArray = decryptedText.Split('\n');

        decryptedText = null;
        decryptedData = null;

        if (PasswordCache == null)
        {
          PasswordCache = new Dictionary<string,string>(passwordLinesArray.Length);
        }
        else
        {
          PasswordCache.Clear();
        }

        for (int lineNumber = 0; lineNumber < passwordLinesArray.Length; lineNumber++)
        {
          string passwordLine = passwordLinesArray[lineNumber];
          if (passwordLine == "\0")
          {
            break;
          }

          string[] pieces = passwordLine.Split(PASSWORD_SEPARATOR);
          PasswordCache.Add(pieces[0], pieces.Length > 1 ? pieces[1] : string.Empty);
          passwordLine = null;
          passwordLinesArray[lineNumber] = null;
        }

        passwordsQuantity = PasswordCache.Count;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex, true, Resources.PasswordsFileLoadError, Resources.GenericErrorTitle);
      }

      return passwordsQuantity;
    }

    /// <summary>
    /// Loads encrypted passwords from disk. These are typically held only for a short moment.
    /// </summary>
    /// <returns>Quantity of passwords loaded.</returns>
    public static int LoadPasswords()
    {
      return LoadPasswords(MySqlWorkbench.UseWorkbenchConnections);
    }

    /// <summary>
    /// Migrates passwords from a consumer application's passwords vault file to the MySQL Workbench one.
    /// </summary>
    /// <returns>A <see cref="MigrationResult"/> instance with information about the migration.</returns>
    public static MigrationResult MigratePasswordsFromConsumerApplicationToWorkbench()
    {
      string errorMessage = null;

      // If MySQL Workbench is not installed or the consumer application's connections file does not exist
      // it means we already migrated existing connections or they were never created, no need to migrate.
      if (!MySqlWorkbench.IsInstalled || !File.Exists(ApplicationPasswordVaultFilePath))
      {
        return null;
      }

      // Load the passwords from the consumer application that are going to be migrated in a password cache.
      LoadPasswords(false);
      var consumerApplicationPasswordsCache = PasswordCache.ToDictionary(entry => entry.Key, entry => entry.Value);

      // Load the Workbench passwords in a password cache, if Workbench has not created its passwords file then create it.
      int passwordsQuantity = LoadPasswords(true);
      if (passwordsQuantity == 0 || PasswordCache == null)
      {
        PasswordCache = new Dictionary<string, string>(1);
      }

      // Migrate only the passwords that do not exist in Workbench, skip the ones already there.
      foreach (var entry in consumerApplicationPasswordsCache.Where(entry => !PasswordCache.ContainsKey(entry.Key)))
      {
        PasswordCache.Add(entry.Key, entry.Value);
      }

      // Clear the consumer's application password cache for security purposes.
      consumerApplicationPasswordsCache.Clear();

      bool saveSuccess = false;
      try
      {
        // Attempt to rename consumer application's passwords vault file, if we can rename it we proceed with saving the passwords in Workbench's passwords vault file.
        File.Move(ApplicationPasswordVaultFilePath, ApplicationPasswordVaultFilePath + ".bak");

        // Save the passwords in MySQL Workbench.
        saveSuccess = UnloadPasswords(true, true);

        // Delete the renamed passwords file if saving was successful, otherwise we revert it back.
        if (saveSuccess)
        {
          File.Delete(ApplicationPasswordVaultFilePath + ".bak");
        }
        else
        {
          File.Move(ApplicationPasswordVaultFilePath + ".bak", ApplicationPasswordVaultFilePath);
        }
      }
      catch (Exception ex)
      {
        errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
      }

      return new MigrationResult(saveSuccess, errorMessage);
    }

    /// <summary>
    /// Stores the given password in an encrypted binary file along with its related host indentifier and user name.
    /// </summary>
    /// <param name="useWorkbenchPasswordsFile">Flag indicating if the passwords vault file used is the MySQL Workbench one.</param>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    /// <param name="password">Password.</param>
    public static void StorePassword(bool useWorkbenchPasswordsFile, string hostIdentifier, string userName, string password)
    {
      if (string.IsNullOrEmpty(userName)
          || string.IsNullOrEmpty(password))
      {
        return;
      }

      string pwdKey = hostIdentifier + DOMAIN_SEPARATOR + userName;
      int passwordsQuantity = LoadPasswords(useWorkbenchPasswordsFile);
      if (passwordsQuantity > 0 && PasswordCache.ContainsKey(pwdKey))
      {
        PasswordCache[pwdKey] = password;
      }
      else
      {
        if (PasswordCache == null)
        {
          PasswordCache = new Dictionary<string, string>(1);
        }

        PasswordCache.Add(pwdKey, password);
      }

      UnloadPasswords(useWorkbenchPasswordsFile, true);
    }

    /// <summary>
    /// Stores the given password in an encrypted binary file along with its related host indentifier and user name.
    /// </summary>
    /// <param name="hostIdentifier">Host identifier usually composed of the Database Driver name and Host name.</param>
    /// <param name="userName">User name.</param>
    /// <param name="password">Password.</param>
    public static void StorePassword(string hostIdentifier, string userName, string password)
    {
      StorePassword(MySqlWorkbench.UseWorkbenchConnections, hostIdentifier, userName, password);
    }

    /// <summary>
    /// Clears the password cache so passwords aren't kept in memory any longer than necessary.
    /// </summary>
    /// <param name="useWorkbenchPasswordsFile">Flag indicating if the passwords vault file used is the MySQL Workbench one.</param>
    /// <param name="saveInFile">Flag indicating if password cache is saved to disk before clearing.</param>
    /// <returns><c>true</c> if passwords were successfully saved to disk, <c>false</c> otherwise.</returns>
    public static bool UnloadPasswords(bool useWorkbenchPasswordsFile, bool saveInFile)
    {
      bool saveSuccess = true;

      if (PasswordCache == null || PasswordCache.Count == 0)
      {
        return false;
      }

      if (saveInFile)
      {
        try
        {
          string passwordsVaultFilePath = useWorkbenchPasswordsFile ? WorkbenchPasswordVaultFilePath : ApplicationPasswordVaultFilePath;
          var decryptedText = new StringBuilder(string.Empty);
          foreach (var passwordItem in PasswordCache)
          {
            decryptedText.Append(passwordItem.Key);
            decryptedText.Append(PASSWORD_SEPARATOR);
            decryptedText.Append(passwordItem.Value);
            decryptedText.Append("\n");
          }

          decryptedText.Append("\0");

          byte[] decryptedData = Encoding.ASCII.GetBytes(decryptedText.ToString());
          byte[] encryptedData = ProtectedData.Protect(decryptedData, null, DataProtectionScope.CurrentUser);

          decryptedText.Clear();
          decryptedData = null;

          File.WriteAllBytes(passwordsVaultFilePath, encryptedData);
          encryptedData = null;
        }
        catch (Exception ex)
        {
          Logger.LogException(ex, true, Resources.PasswordsFileSaveError, Resources.GenericErrorTitle);
          saveSuccess = false;
        }
      }

      PasswordCache.Clear();
      return saveInFile && saveSuccess;
    }

    /// <summary>
    /// Clears the password cache so passwords aren't kept in memory any longer than necessary.
    /// </summary>
    /// <param name="saveInFile">Flag indicating if password cache is saved to disk before clearing.</param>
    /// <returns><c>true</c> if passwords were successfully saved to disk, <c>false</c> otherwise.</returns>
    public static bool UnloadPasswords(bool saveInFile)
    {
      return UnloadPasswords(MySqlWorkbench.UseWorkbenchConnections, saveInFile);
    }
  }
}
