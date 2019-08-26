// Copyright (c) 2009, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.Windows.Forms;
using EnvDTE80;
using EnvDTE;
using System.Data.Common;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio.WebConfig
{
  public partial class AppConfigDlg : Form
  {
    internal struct WizardPage
    {
      public string Title;
      public string Description;
      public string EnabledString;
      public GenericConfig ProviderConfig;
    }

    /// <summary>
    /// The name of the configuration file for the current project.
    /// </summary>
    private string _configFileName;
    private DTE2 dte;
    private Solution2 solution;
    private Project project;
    private int page;
    private WizardPage[] pages;

    private const string APP_CONFIG_FILE_NAME = "app.config";
    private const int ENTITYFRAMEWORK_INDEX = 0;
    private const int MEMBERSHIP_INDEX = 1;
    private const int SimpleMembershipIndex = 2;
    private const int ROLES_INDEX = 3;
    private const int PROFILES_INDEX = 4;
    private const int SESSION_INDEX = 5;
    private const int SITEMAP_INDEX = 6;
    private const int PERSONALIZATION_INDEX = 7;
    private const string WEB_CONFIG_FILE_NAME = "web.config";

    private string[] pagesDescription;
    private Dictionary<string, string> ControlsFriendlyName = new Dictionary<string, string>() {
                                                              { "txtConnStringSM", "Connection String" },
                                                              { "txtUserTable", "User Table" },
                                                              { "txtUserIdCol", "User Id Column" },
                                                              { "txtUserNameCol", "User Name Column" } };

    private Version _connectorVersion;

    private bool IsSimpleMembershipPage
    {
      get { return page == SimpleMembershipIndex; }
    }

    private bool IsEntityFrameworkPage
    {
      get { return page == ENTITYFRAMEWORK_INDEX; }
    }

    public AppConfigDlg()
    {
      var factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
      _connectorVersion = factory != null ? new Version(factory.GetType().Assembly.GetName().Version.ToString(3)) : new Version(8, 0, 18);
      InitializeComponent();
      dte = MySqlDataProviderPackage.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      if (dte != null)
      {
        solution = (Solution2)dte.Solution;
      }

      FindCurrentProject();
      EnsureConfigFile();
      if (_configFileName.EndsWith(APP_CONFIG_FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
      {
        pages = new WizardPage[1];
        pagesDescription = new string[1] { "Entity Framework" };
      }
      else
      {
        pages = new WizardPage[8];
        pagesDescription = new string[8] { "Entity Framework", "Membership", "Simple Membership", "Roles", "Profiles", "Session State", "Site Map", "Web Personalization" };
      }

      CreateStepsLabels();
      SetSelectedStepLabel(pagesDescription[0]);
      LoadInitialState();
      PageChanged();
    }

    /// <summary>
    /// Method to create the pages' steps labels in the left panel of the web config dialog.
    /// </summary>
    private void CreateStepsLabels()
    {
      int xAxis = 25, yAxis = 80;

      foreach (string description in pagesDescription)
      {
        Label lbl = new Label();
        lbl.AutoSize = false;
        lbl.Size = new Size(169, 23);
        lbl.Location = new Point(xAxis, yAxis);
        lbl.BackColor = Color.FromArgb(41, 41, 41);
        lbl.Font = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Point);
        lbl.ForeColor = Color.FromArgb(138, 138, 138);
        lbl.Text = description;
        lbl.Name = string.Format("lbl{0}", description);
        pnlSteps.Controls.Add(lbl);
        yAxis += 25;
      }
    }

    /// <summary>
    /// Method to highlight the selected step label.
    /// </summary>
    /// <param name="stepName">Name of the step to be highlighted.</param>
    private void SetSelectedStepLabel(string stepName)
    {
      ResetSelectedLabels();
      Label lbl = (Label)pnlSteps.Controls.Find(string.Format("lbl{0}", stepName), true)[0];
      lbl.Font = new System.Drawing.Font("Segoe UI", 8, FontStyle.Bold, GraphicsUnit.Point);
      lbl.ForeColor = Color.FloralWhite;
    }

    /// <summary>
    /// Resets all the steps labels to its original font and size.
    /// </summary>
    private void ResetSelectedLabels()
    {
      foreach (Control control in pnlSteps.Controls)
      {
        if (control is Label)
        {
          control.Font = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Point);
          control.ForeColor = Color.FromArgb(138, 138, 138);
        }
      }
    }

    private void FindCurrentProject()
    {
      // Get the project(s?) configured as startup project, the data is stored as object array and it contains strings with the following format "namespace\projectName.csproj"
      string startupProj = "";
      object[] startupProjArray = dte.Solution.SolutionBuild.StartupProjects as object[];
      if (startupProjArray != null)
      {
        startupProj = startupProjArray[0] as string;
      }

      // Get the project configured as startup project, in case we don't have it return the first project found.
      if (!string.IsNullOrEmpty(startupProj))
      {
        foreach (Project proj in solution.Projects)
        {
          if (proj.UniqueName.Equals(startupProj, StringComparison.InvariantCultureIgnoreCase))
          {
            project = proj;
            return;
          }
        }
      }
      else
      {
        project = (Project)solution.Projects.Item(1);
      }
    }

    private void EnsureConfigFile()
    {
      foreach (ProjectItem items in project.ProjectItems)
      {
        if (!string.Equals(items.Name, WEB_CONFIG_FILE_NAME, StringComparison.InvariantCultureIgnoreCase)
            && !string.Equals(items.Name, APP_CONFIG_FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
        {
          continue;
        }

        _configFileName = items.get_FileNames(1);
        break;
      }

      if (_configFileName == null)
      {
        string template = solution.GetProjectItemTemplate("WebConfig.zip", "Web/CSharp");
        ProjectItem item = project.ProjectItems.AddFromTemplate(template, WEB_CONFIG_FILE_NAME);
        _configFileName = item.get_FileNames(1);
      }
    }

    private void LoadInitialState()
    {
      if (_configFileName == null)
      {
        return;
      }

      AppConfig wc = new AppConfig(_configFileName);
      LoadInitialEntityFrameworkState();
      if (_configFileName.EndsWith(WEB_CONFIG_FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
      {
        LoadInitialMembershipState();
        LoadInitialSimpleMembershipState();
        LoadInitialRoleState();
        LoadInitialProfileState();
        LoadInitialSessionState();
        LoadInitialSiteMapState();
        LoadInitialPersonalizationState();
        foreach (WizardPage page in pages)
        {
          page.ProviderConfig.Initialize(wc);
        }

        return;
      }

      pages[0].ProviderConfig.Initialize(wc);
    }

    /// <summary>
    /// Sets the initial state of the entity framework configuration (Title, description and initializes the EntityFrameworkConfig object).
    /// </summary>
    private void LoadInitialEntityFrameworkState()
    {
      pages[ENTITYFRAMEWORK_INDEX].Title = "Entity Framework";
      pages[ENTITYFRAMEWORK_INDEX].Description = "Set options for use with Entity Framework";
      pages[ENTITYFRAMEWORK_INDEX].EnabledString = "Use MySQL with Entity Framework";
      pages[ENTITYFRAMEWORK_INDEX].ProviderConfig = new EntityFrameworkConfig(_configFileName);
    }

    private void LoadInitialMembershipState()
    {
      pages[MEMBERSHIP_INDEX].Title = "Membership";
      pages[MEMBERSHIP_INDEX].Description = "Set options for use with the membership provider";
      pages[MEMBERSHIP_INDEX].EnabledString = "Use MySQL to manage my membership records";
      pages[MEMBERSHIP_INDEX].ProviderConfig = new MembershipConfig();
    }

    private void LoadInitialRoleState()
    {
      pages[ROLES_INDEX].Title = "Roles";
      pages[ROLES_INDEX].Description = "Set options for use with the role provider";
      pages[ROLES_INDEX].EnabledString = "Use MySQL to manage my roles";
      pages[ROLES_INDEX].ProviderConfig = new RoleConfig();
    }

    private void LoadInitialProfileState()
    {
      pages[PROFILES_INDEX].Title = "Profiles";
      pages[PROFILES_INDEX].Description = "Set options for use with the profile provider";
      pages[PROFILES_INDEX].EnabledString = "Use MySQL to manage my profiles";
      pages[PROFILES_INDEX].ProviderConfig = new ProfileConfig();
    }

    private void LoadInitialSessionState()
    {
      pages[SESSION_INDEX].Title = "Session State";
      pages[SESSION_INDEX].Description = "Set options for use with the session state provider";
      pages[SESSION_INDEX].EnabledString = "Use MySQL to manage my ASP.Net session state";
      pages[SESSION_INDEX].ProviderConfig = new SessionStateConfig();
    }

    private void LoadInitialSiteMapState()
    {
      pages[SITEMAP_INDEX].Title = "Site Map";
      pages[SITEMAP_INDEX].Description = "Set options for use with the sitemap provider";
      pages[SITEMAP_INDEX].EnabledString = "Use MySQL to manage my ASP.NET site map";
      pages[SITEMAP_INDEX].ProviderConfig = new SiteMapConfig();
    }

    private void LoadInitialPersonalizationState()
    {
      pages[PERSONALIZATION_INDEX].Title = "Web Personalization";
      pages[PERSONALIZATION_INDEX].Description = "Set options for use with the personalization provider";
      pages[PERSONALIZATION_INDEX].EnabledString = "Use MySQL to manage my ASP.NET personalization provider";
      pages[PERSONALIZATION_INDEX].ProviderConfig = new PersonalizationConfig();

    }

    private void LoadInitialSimpleMembershipState()
    {
      pages[SimpleMembershipIndex].Title = "Simple Membership";
      pages[SimpleMembershipIndex].Description = "Set options for use with the simple membership provider";
      pages[SimpleMembershipIndex].EnabledString = "Use MySQL to manage my simple membership records";
      pages[SimpleMembershipIndex].ProviderConfig = new SimpleMembershipConfig();
    }

    private void advancedBtn_Click(object sender, EventArgs e)
    {
      MembershipOptionsDlg dlg = new MembershipOptionsDlg();
      MembershipConfig config = pages[MEMBERSHIP_INDEX].ProviderConfig as MembershipConfig;
      dlg.Options = config.MemberOptions;
      DialogResult r = dlg.ShowDialog();
      if (DialogResult.Cancel == r)
      {
        return;
      }

      config.MemberOptions = dlg.Options;
    }

    private void editConnString_Click(object sender, EventArgs e)
    {
      ConnectionStringEditorDlg dlg = new ConnectionStringEditorDlg();
      try
      {
        dlg.ConnectionString = connectionString.Text;
        if (DialogResult.Cancel == dlg.ShowDialog(this))
        {
          return;
        }

        connectionString.Text = dlg.ConnectionString;
      }
      catch (ArgumentException)
      {
        Logger.LogError(Properties.Resources.ConnectionStringInvalid, true);
      }
    }

    private void nextButton_Click(object sender, EventArgs e)
    {
      if (!SavePageData())
      {
        return;
      }

      if (page == pages.Length - 1)
      {
        Cursor.Current = Cursors.WaitCursor;
        Finish();
        Cursor.Current = Cursors.Default;
      }
      else
      {
        page++;
        SetSelectedStepLabel(pagesDescription[page]);
        PageChanged();
      }
    }

    private void backButton_Click(object sender, EventArgs e)
    {
      if (!SavePageData())
      {
        return;
      }

      page--;
      SetSelectedStepLabel(pagesDescription[page]);
      PageChanged();
    }

    private bool SavePageData()
    {
      if (!IsValidData())
      {
        return false;
      }

      GenericConfig config = pages[page].ProviderConfig;
      if (IsSimpleMembershipPage && useProvider.Checked)
      {
        SimpleMembershipOptions options = new SimpleMembershipOptions();
        options.AutoGenerateTables = chbAutoGenTbl.Checked;
        options.UserTableName = txtUserTable.Text;
        options.UserIdColumn = txtUserIdCol.Text;
        options.UserNameColumn = txtUserNameCol.Text;
        ((SimpleMembershipConfig)pages[SimpleMembershipIndex].ProviderConfig).SimpleMemberOptions = options;
        config.Enabled = useProvider.Checked;
      }

      if (IsEntityFrameworkPage && useProvider.Checked)
      {
        EntityFrameworkOptions options = new EntityFrameworkOptions();
        options.EF5 = radioBtnEF5.Checked;
        options.EF6 = radioBtnEF6.Checked;
        ((EntityFrameworkConfig)pages[ENTITYFRAMEWORK_INDEX].ProviderConfig).EntityFrameworkOptions = options;
        config.Enabled = useProvider.Checked;
      }

      Options o = config.GenericOptions;
      o.AppName = appName.Text;
      o.AppDescription = appDescription.Text.Trim();
      o.WriteExceptionToLog = writeExToLog.Checked;
      o.AutoGenSchema = autogenSchema.Checked;
      o.EnableExpireCallback = enableExpCallback.Checked;
      o.ConnectionString = (IsSimpleMembershipPage && useProvider.Checked) ? txtConnStringSM.Text.Trim() : connectionString.Text.Trim();
      o.ConnectionStringName = (IsSimpleMembershipPage && useProvider.Checked) ? txtConnStringName.Text.Trim() : o.ConnectionStringName;
      config.GenericOptions = o;
      return true;
    }

    private void PageChanged()
    {
      pageLabel.Text = pages[page].Title;
      pageDesc.Text = pages[page].Description;
      useProvider.Text = pages[page].EnabledString;
      GenericConfig config = pages[page].ProviderConfig;
      Options o = config.GenericOptions;
      appName.Text = o.AppName;
      useProvider.Checked = config.Enabled;
      appDescription.Text = o.AppDescription;
      writeExToLog.Checked = o.WriteExceptionToLog;
      autogenSchema.Checked = o.AutoGenSchema;
      enableExpCallback.Checked = o.EnableExpireCallback;
      controlPanel.Enabled = config.Enabled;

      if (IsEntityFrameworkPage)
      {
        EntityFrameworkOptions options = ((EntityFrameworkConfig)pages[ENTITYFRAMEWORK_INDEX].ProviderConfig).EntityFrameworkOptions;
        radioBtnEF5.Checked = options.EF5;
        radioBtnEF6.Checked = options.EF6;
        if (_connectorVersion >= new Version(6, 10))
          radioBtnEF5.Enabled = false;
      }
      else
      {
        if (IsSimpleMembershipPage)
        {
          txtConnStringSM.Text = o.ConnectionString;
        }
        else
        {
          connectionString.Text = o.ConnectionString;
        }
      }

      advancedBtn.Visible = page == 0;
      writeExToLog.Visible = page != 3;
      enableExpCallback.Visible = page == 4;
      nextButton.Text = (page == pages.Length - 1) ? "Finish" : "Next";
      backButton.Enabled = page > 0;

      if (page == PERSONALIZATION_INDEX)
      {
        useProvider.Enabled = IsMembershipSelected();
      }
      else
      {
        useProvider.Enabled = true;
      }

      if (config.NotInstalled)
      {
        useProvider.Checked = false;
        useProvider.Enabled = false;
      }

      if (IsEntityFrameworkPage)
      {
        pnlSimpleMembership.Visible = false;
        controlPanel.Visible = false;
        entityFrameworkPanel.Visible = true;
        entityFrameworkPanel.Enabled = config.Enabled;
      }
      else
      {
        entityFrameworkPanel.Visible = false;
        entityFrameworkPanel.Enabled = false;

        if (IsSimpleMembershipPage)
        {
          pnlSimpleMembership.Visible = true;
          controlPanel.Visible = false;

          if (config.NotInstalled)
          {
            useProvider.Enabled = false;
            useProvider.Checked = false;
            pnlSimpleMembership.Enabled = false;
          }
          else
          {
            useProvider.Enabled = !IsMembershipSelected();
            if (config.Enabled && IsMembershipSelected())
            {
              useProvider.Checked = false;
            }
          }
        }
        else
        {
          pnlSimpleMembership.Visible = false;
          controlPanel.Visible = true;
        }
      }
    }

    private void Finish()
    {
      if (_configFileName.EndsWith(WEB_CONFIG_FILE_NAME, StringComparison.InvariantCultureIgnoreCase))
      {
        AppConfig w = new AppConfig(_configFileName);

        //If Membership is selected then save Simple Membership first, because these providers are in the same section,
        //so if any is removed but called second place it will remove the entire section
        if (IsMembershipSelected())
        {
          pages[SimpleMembershipIndex].ProviderConfig.Save(w);
          pages[MEMBERSHIP_INDEX].ProviderConfig.Save(w);
        }
        else
        {
          pages[MEMBERSHIP_INDEX].ProviderConfig.Save(w);
          pages[SimpleMembershipIndex].ProviderConfig.Save(w);
        }

        pages[ROLES_INDEX].ProviderConfig.Save(w);
        pages[PROFILES_INDEX].ProviderConfig.Save(w);
        pages[SESSION_INDEX].ProviderConfig.Save(w);
        pages[SITEMAP_INDEX].ProviderConfig.Save(w);
        pages[PERSONALIZATION_INDEX].ProviderConfig.Save(w);
        w.Save();
      }

      AppConfig webConfig = new AppConfig(_configFileName);
      pages[ENTITYFRAMEWORK_INDEX].ProviderConfig.Save(webConfig);
      project.DTE.Solution.SolutionBuild.Build(true);
      Close();
    }

    private void configPanel_Paint(object sender, PaintEventArgs e)
    {
      Pen darkPen = new Pen(SystemColors.ControlDark);
      Pen lightPen = new Pen(SystemColors.ControlLightLight);
      int left = configPanel.ClientRectangle.Left;
      int right = configPanel.ClientRectangle.Right;
      int top = configPanel.ClientRectangle.Top;
      int bottom = configPanel.ClientRectangle.Bottom - 2;
      e.Graphics.DrawLine(darkPen, left, top, right, top);
      e.Graphics.DrawLine(lightPen, left, top + 1, right, top + 1);
      e.Graphics.DrawLine(darkPen, left, bottom, right, bottom);
      e.Graphics.DrawLine(lightPen, left, bottom + 1, right, bottom + 1);
    }

    private void useProvider_CheckStateChanged(object sender, EventArgs e)
    {
      GenericConfig config = pages[page].ProviderConfig;
      if (IsEntityFrameworkPage)
      {
        config.Enabled = useProvider.Checked;
        entityFrameworkPanel.Enabled = config.Enabled;
        entityFrameworkPanel.Visible = true;
        controlPanel.Visible = false;
        pnlSimpleMembership.Visible = false;
      }
      else
      {
        if (!IsSimpleMembershipPage)
        {
          config.Enabled = useProvider.Checked;
          controlPanel.Enabled = config.Enabled;
          controlPanel.Visible = true;
          pnlSimpleMembership.Visible = false;
          entityFrameworkPanel.Visible = false;
        }
        else
        {
          config.Enabled = useProvider.Checked;
          pnlSimpleMembership.Enabled = config.Enabled;
          pnlSimpleMembership.Visible = true;
          controlPanel.Visible = false;
          entityFrameworkPanel.Visible = false;
        }
      }
    }

    private bool IsMembershipSelected()
    {
      MembershipConfig config = pages[MEMBERSHIP_INDEX].ProviderConfig as MembershipConfig;
      return config.Enabled;
    }

    private bool IsEntityFrameworkSelected()
    {
      EntityFrameworkConfig config = pages[ENTITYFRAMEWORK_INDEX].ProviderConfig as EntityFrameworkConfig;
      return config.Enabled;
    }

    private void btnEditSM_Click(object sender, EventArgs e)
    {
      ConnectionStringEditorDlg dlg = new ConnectionStringEditorDlg();
      try
      {
        dlg.ConnectionString = txtConnStringSM.Text;
        if (DialogResult.Cancel == dlg.ShowDialog(this)) return;
        txtConnStringSM.Text = dlg.ConnectionString;
      }
      catch (ArgumentException)
      {
        Logger.LogError(Properties.Resources.ConnectionStringInvalid, true);
      }
    }

    private bool IsValidData()
    {
      if (IsEntityFrameworkPage)
      {
        if (useProvider.Checked && !radioBtnEF5.Checked && !radioBtnEF6.Checked)
        {
          Logger.LogError("Please select the Entity Framework version.", true);
          return false;
        }
      }
      else
      {
        if (!IsSimpleMembershipPage)
        {
          if (useProvider.Checked && connectionString.Text.Trim().Length == 0)
          {
            Logger.LogError(Properties.Resources.WebConfigConnStrNoEmpty, true);
            return false;
          }
        }
        else
        {
          if (useProvider.Checked)
          {
            bool valid = true;
            string controlsToValidate = "";
            foreach (Control control in pnlSimpleMembership.Controls)
            {
              if (ControlsFriendlyName.ContainsKey(control.Name))
              {
                controlsToValidate += controlsToValidate.Length > 0 ? ", " : "";
                TextBox txt = control as TextBox;
                if (txt != null && string.IsNullOrEmpty(txt.Text))
                {
                  valid = false;
                  controlsToValidate += string.Format("{0}, ", ControlsFriendlyName[txt.Name]);
                }
              }
            }

            if (!valid)
            {
              controlsToValidate = (controlsToValidate += ".").Replace(", .", ".");
              Logger.LogError($"{Properties.Resources.WrongNetFxVersionMessage}: {controlsToValidate}", true);
              return false;
            }
          }
        }
      }
      return true;
    }
  }
}
