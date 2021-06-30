// Copyright (c) 2019, 2021, Oracle and/or its affiliates.
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

using MySql.Utility.Classes;
using MySql.VisualStudio.CustomAction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySql.VisualStudio.Updater
{
  /// <summary>
  /// Main form used when executing the Configuration Update Tool.
  /// </summary>
  public partial class UpdaterMainForm : Form
  {
    /// <summary>
    /// The version number of the MySql.Data library included in MySql for Visual Studio product.
    /// </summary>
    private Version _internalMySqlDataVersion;

    /// <summary>
    /// The version number of the MySql.Data library included with the installed Connector/NET product (if any). 
    /// </summary>
    private Version _installedMySqlDataVersion;

    /// <summary>
    /// The version number of the MySQL for Visual Studio product.
    /// </summary>
    private Version _mySqlForVisualStudioVersion;

    /// <summary>
    /// Delegate used when updating the progress bar.
    /// </summary>
    /// <param name="progress">The progress percentage.</param>
    delegate void SetProgressCallback(int progress);

    /// <summary>
    /// Delegate used when updating the text on the details textbox.
    /// </summary>
    /// <param name="text">The text to append.</param>
    delegate void SetStatusTextCallback(string text);

    /// <summary>
    /// Delegate used for closing the form.
    /// </summary>
    delegate void CloseFormCallback();

    /// <summary>
    /// Main constructor.
    /// </summary>
    /// <param name="mySqlForVisualStudioVersion">The version number of the MySQL for Visual Studio plugin.</param>
    /// <param name="installedMySqlDataVersion">The version number of the MySql.Data library included in the current Connector/NET installation.</param>
    /// <param name="internalMySqlDataVersion">The version number of the MySql.data library bundled with the MySQL for Visual Studio plugin.</param>
    public UpdaterMainForm(Version mySqlForVisualStudioVersion, Version internalMySqlDataVersion, Version installedMySqlDataVersion)
    {
      InitializeComponent();

      _installedMySqlDataVersion = installedMySqlDataVersion;
      _internalMySqlDataVersion = internalMySqlDataVersion;
      _mySqlForVisualStudioVersion = mySqlForVisualStudioVersion;

      Initialize();
    }

    /// <summary>
    /// Initializes the update of the PKGDEF files as an asynchronous operation.
    /// </summary>
    private async void Initialize()
    {
      await Task.Run(() => UpdatePkgDefFiles());
      CloseButton.Enabled = true;
      RestartLabel.Text = Properties.Resources.RestartVisualStudioRequired;
    }

    /// <summary>
    /// General method which triggers the update of the PKGDEF file for each version of Visual Studio.
    /// </summary>
    private void UpdatePkgDefFiles()
    {
      if (_internalMySqlDataVersion == null)
      {
        throw new ArgumentNullException(nameof(_internalMySqlDataVersion));
      }

      if (!CustomActions.IsConfigurationUpdateRequired(_mySqlForVisualStudioVersion, _installedMySqlDataVersion, _internalMySqlDataVersion))
      {
        SetStatusText(Properties.Resources.PkgdefFileUpdateNotRequired);
        SetProgress(100);
        return;
      }

      var errorMessage = Properties.Resources.PkgdefFileUpdateFailed;
      CustomActions.SetVSInstallationPaths();

      // Perform update for each version of Visual Studio that is supported.
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2017Community, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2017Community.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(17);
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2017Enterprise, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2017Enterprise.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(34);
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2017Professional, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2017Professional.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(51);
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2019Community, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2019Community.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(68);
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2019Enterprise, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2019Enterprise.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(85);
      if (!UpdatePkgDefFileFor(SupportedVisualStudioVersions.Vs2019Professional, _mySqlForVisualStudioVersion))
      {
        SetStatusText(string.Format(errorMessage, SupportedVisualStudioVersions.Vs2019Professional.ToString()));
        return;
      }

      SetStatusText();
      SetProgress(100);
      SetStatusText(Properties.Resources.PkgdefFilesUpdateCompleted);
    }

    /// <summary>
    /// Updates the PKGDEF file for a specific installation of Visual Studio.
    /// </summary>
    /// <param name="visualStudioVersion">The Visual Studio for which to update the PKGDEF file.</param>
    /// <param name="mySqlForVisualStudioVersion">The version of MySQL for Visual studio expected to be installed in the specified Visual Studio version.</param>
    /// <returns></returns>
    private bool UpdatePkgDefFileFor(SupportedVisualStudioVersions visualStudioVersion, Version mySqlForVisualStudioVersion)
    {
      SetStatusText(string.Format(Properties.Resources.AttemptingToUpdatePkgdefFile, visualStudioVersion.GetDescription()));
      string visualStudioInstallationPath = null;
      switch (visualStudioVersion)
      {
        case SupportedVisualStudioVersions.Vs2017Community:
          visualStudioInstallationPath = CustomActions.VS2017CommunityInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Enterprise:
          visualStudioInstallationPath = CustomActions.VS2017EnterpriseInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Professional:
          visualStudioInstallationPath = CustomActions.VS2017ProfessionalInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Community:
          visualStudioInstallationPath = CustomActions.VS2019CommunityInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Enterprise:
          visualStudioInstallationPath = CustomActions.VS2019EnterpriseInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Professional:
          visualStudioInstallationPath = CustomActions.VS2019ProfessionalInstallationPath;
          break;

        default:
          return false;
      }

      if (visualStudioInstallationPath == null)
      {
        SetStatusText(string.Format(Properties.Resources.ProductNotInstalled, visualStudioVersion.GetDescription()));
        return true;
      }

      if (CustomActions.UpdatePkgdefFile(visualStudioVersion, visualStudioInstallationPath, mySqlForVisualStudioVersion, _installedMySqlDataVersion, _internalMySqlDataVersion, out var logData, null))
      {
        SetStatusText(logData);
        if (logData != null && logData.Contains("Updating the PKGDEF file is not required."))
        {
          return true;
        }

        var startInfo = new ProcessStartInfo
        {
          UseShellExecute = false,
          FileName = visualStudioInstallationPath + @"\Common7\IDE\devenv.exe",
          CreateNoWindow = true,
          Arguments = "/updateconfiguration",
          RedirectStandardError = true,
          RedirectStandardOutput = true
        };

        SetStatusText(string.Format(Properties.Resources.RefreshingExtension, visualStudioVersion.GetDescription()));
        string error;
        using (var process = new Process())
        {
          process.StartInfo = startInfo;
          process.Start();
          error = process.StandardError.ReadToEnd();
          process.WaitForExit();
        }

        if (string.IsNullOrEmpty(error))
        {
          SetStatusText(string.Format(Properties.Resources.ExtensionRefreshed, visualStudioVersion.GetDescription()));
        }
        else
        {
          SetStatusText(error);
          return false;
        }
      }
      else
      {
        SetStatusText(logData);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Sets the progress for the status bar.
    /// </summary>
    /// <param name="progress">The progress percentage.</param>
    private void SetProgress(int progress)
    {
      if (UpdateConfigurationProgressBar.InvokeRequired)
      {
        SetProgressCallback d = SetProgress;
        Invoke(d, progress);
      }
      else
      {
        UpdateConfigurationProgressBar.Value = progress;
      }
    }

    /// <summary>
    /// Appends the specified text to the ConfigurationDetailsTextBox control.
    /// </summary>
    /// <param name="text">The text to append.</param>
    private void SetStatusText(string text = null)
    {
      text = string.IsNullOrEmpty(text)
             ? Environment.NewLine
             : text.EndsWith(Environment.NewLine)
               ? text
               : $"{text}{Environment.NewLine}";

      if (ConfigurationDetailsTextBox.InvokeRequired)
      {
        SetStatusTextCallback d = SetStatusText;
        Invoke(d, text);
      }
      else
      {
        ConfigurationDetailsTextBox.Text += text;
      }
    }

    /// <summary>
    /// Closes this form.
    /// </summary>
    private void CloseForm()
    {
      if (InvokeRequired)
      {
        CloseFormCallback d = CloseForm;
        Invoke(d, new object[] { });
      }
      else
      {
        Close();
      }
    }

    /// <summary>
    /// Event delegate for closing this form.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments related to this event.</param>
    private void CloseButton_Click(object sender, EventArgs e)
    {
      CloseForm();
    }
  }
}
