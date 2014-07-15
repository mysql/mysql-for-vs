// Copyright © 2009, 2014, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using EnvDTE;
using MySql.Data.VisualStudio.Properties;
using System.Web.Security;
using System.Configuration.Provider;
using System.Web.Profile;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Data.Common;

namespace MySql.Data.VisualStudio.WebConfig
{
  public partial class WebConfigDlg : Form
  {
    private string webConfigFileName;
    private DTE2 dte;
    private Solution2 solution;
    private Project project;
    private int page;
    private WizardPage[] pages = new WizardPage[7];
    private const int MEMBERSHIP_INDEX = 0;
    private const int SimpleMembershipIndex = 1;
    private const int PERSONALIZATION_INDEX = 6;

    public WebConfigDlg()
    {
      InitializeComponent();

      dte = MySqlDataProviderPackage.GetGlobalService(
          typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      solution = (Solution2)dte.Solution;
      FindCurrentWebProject();
      EnsureWebConfig();
      LoadInitialState();
      PageChanged();
    }

    private void FindCurrentWebProject()
    {
      //get the project(s?) configured as startup project, the data is stored as object array and it contains strings with the following format "namespace\projectName.csproj"
      string startupProj = "";
      object[] startupProjArray = dte.Solution.SolutionBuild.StartupProjects as object[];
      if (startupProjArray != null)
      {
        startupProj = startupProjArray[0] as string;
      }

      //get the project configured as startup project, in case we don't have it return the first project found
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

    private void EnsureWebConfig()
    {
      foreach (ProjectItem items in project.ProjectItems)
      {
        if (!String.Equals(items.Name, "web.config", StringComparison.InvariantCultureIgnoreCase)) continue;
        webConfigFileName = items.get_FileNames(1);
        break;
      }
      if (webConfigFileName == null)
      {
        string template = solution.GetProjectItemTemplate("WebConfig.zip", "Web/CSharp");
        ProjectItem item = project.ProjectItems.AddFromTemplate(template, "web.config");
        webConfigFileName = item.get_FileNames(1);
      }
    }

    private void LoadInitialState()
    {
      WebConfig wc = new WebConfig(webConfigFileName);
      LoadInitialMembershipState();
      LoadInitialSimpleMembershipState();
      LoadInitialRoleState();
      LoadInitialProfileState();
      LoadInitialSessionState();
      LoadInitialSiteMapState();
      LoadInitialPersonalizationState();

      foreach (WizardPage page in pages)
        page.ProviderConfig.Initialize(wc);
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
      pages[2].Title = "Roles";
      pages[2].Description = "Set options for use with the role provider";
      pages[2].EnabledString = "Use MySQL to manage my roles";
      pages[2].ProviderConfig = new RoleConfig();
    }

    private void LoadInitialProfileState()
    {
      pages[3].Title = "Profiles";
      pages[3].Description = "Set options for use with the profile provider";
      pages[3].EnabledString = "Use MySQL to manage my profiles";
      pages[3].ProviderConfig = new ProfileConfig();
    }

    private void LoadInitialSessionState()
    {
      pages[4].Title = "Session State";
      pages[4].Description = "Set options for use with the session state provider";
      pages[4].EnabledString = "Use MySQL to manage my ASP.Net session state";
      pages[4].ProviderConfig = new SessionStateConfig();      
    }

    private void LoadInitialSiteMapState()
    {
      pages[5].Title = "Site Map";
      pages[5].Description = "Set options for use with the sitemap provider";
      pages[5].EnabledString = "Use MySQL to manage my ASP.NET site map";
      pages[5].ProviderConfig = new SiteMapConfig();
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
      MembershipConfig config = pages[0].ProviderConfig as MembershipConfig;
      dlg.Options = config.MemberOptions;
      DialogResult r = dlg.ShowDialog();
      if (DialogResult.Cancel == r) return;
      config.MemberOptions = dlg.Options;
    }

    private void editConnString_Click(object sender, EventArgs e)
    {
      ConnectionStringEditorDlg dlg = new ConnectionStringEditorDlg();
      try
      {
        dlg.ConnectionString = connectionString.Text;
        if (DialogResult.Cancel == dlg.ShowDialog(this)) return;
        connectionString.Text = dlg.ConnectionString;
      }
      catch (ArgumentException)
      {
        MessageBox.Show(this, Resources.ConnectionStringInvalid, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void nextButton_Click(object sender, EventArgs e)
    {
      if (!SavePageData()) return;
      if (page == pages.Length - 1)
        Finish();
      else
      {
        page++;
        PageChanged();
      }
    }

    private void backButton_Click(object sender, EventArgs e)
    {
      if (!SavePageData()) return;
      page--;
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

      if(IsSimpleMembershipPage)
        txtConnStringSM.Text = o.ConnectionString;
      else
        connectionString.Text = o.ConnectionString;

      advancedBtn.Visible = page == 0;
      writeExToLog.Visible = page != 2;
      enableExpCallback.Visible = page == 3;
      nextButton.Text = (page == pages.Length - 1) ? "Finish" : "Next";
      backButton.Enabled = page > 0;

      if (page == PERSONALIZATION_INDEX)
        useProvider.Enabled = IsMembershipSelected();
      else
        useProvider.Enabled = true;

      if (config.NotInstalled)
      {
        useProvider.Checked = false;
        useProvider.Enabled = false;
      }

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
            useProvider.Checked = false;
        }
      }
      else
      {
        pnlSimpleMembership.Visible = false;
        controlPanel.Visible = true;
      }
    }

    private void Finish()
    {
      WebConfig w = new WebConfig(webConfigFileName);
      //is Membership is selected then save Simple Membership first, because these providers are in the same section so if any is removed but called second place it will remove the entire section
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
      pages[2].ProviderConfig.Save(w);
      pages[3].ProviderConfig.Save(w);
      pages[4].ProviderConfig.Save(w);
      pages[5].ProviderConfig.Save(w);
      pages[PERSONALIZATION_INDEX].ProviderConfig.Save(w);
      w.Save();
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
      if (!IsSimpleMembershipPage)
      {
        config.Enabled = useProvider.Checked;
        controlPanel.Enabled = config.Enabled;
        controlPanel.Visible = true;
        pnlSimpleMembership.Visible = false;
      }
      else
      {
        config.Enabled = useProvider.Checked;
        pnlSimpleMembership.Enabled = config.Enabled;
        pnlSimpleMembership.Visible = true;
        controlPanel.Visible = false;
      }
    }

    private bool IsMembershipSelected()
    {
      MembershipConfig config = pages[MEMBERSHIP_INDEX].ProviderConfig as MembershipConfig;
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
        MessageBox.Show(this, Resources.ConnectionStringInvalid, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private bool IsSimpleMembershipPage
    {
      get { return page == SimpleMembershipIndex; }
    }

    private bool IsValidData()
    {
      if (!IsSimpleMembershipPage)
      {
        if (useProvider.Checked && connectionString.Text.Trim().Length == 0)
        {
          MessageBox.Show(this, Resources.WebConfigConnStrNoEmpty, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(this, string.Format("Please set a valid value for the following fields: {0}", controlsToValidate), Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
          }
        }
      }
      return true;
    }

    private Dictionary<string, string> ControlsFriendlyName = new Dictionary<string, string>() {
                                                              { "txtConnStringSM", "Connection String" },
                                                              { "txtUserTable", "User Table" },
                                                              { "txtUserIdCol", "User Id Column" },
                                                              { "txtUserNameCol", "User Name Column" } };
  }

  internal struct WizardPage
  {
    public string Title;
    public string Description;
    public string EnabledString;
    public GenericConfig ProviderConfig;   
  }
}
