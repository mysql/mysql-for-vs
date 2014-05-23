using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Wizards
{

  public partial class WizardNewProjectDialog : Form
  {
#region "User Input Properties"

    internal string ProjectPath
    {
      get { return txtProjectPath.Text; }
    }

    internal string ProjectName
    {
      get { return txtProjectName.Text; }
    }

    internal string ProjectType
    {
      get {
        if (projectTypesList.SelectedItems.Count > 0)
        {
          var selectedItem = projectTypesList.SelectedItems[0];
          switch (selectedItem.Text)
          {
            case "ASP.NET MVC 3 Project":
              if (selectedItem.SubItems[1].Text.IndexOf("Visual C#") >= 0)
                return "CSharpMvc3.zip";
              else
                return "VisualBasicMvc3.zip";
            case "Windows Forms Project":
              if (selectedItem.SubItems[1].Text.IndexOf("Visual C#") >= 0)
                return "CSharpWinForms.zip";
              else
                return "VisualBasicWinForms.zip";
          }
        }
        return string.Empty;
      }
    }

    internal bool CreateDirectoryForSolution
    {
      get { return createDirectoryForSolutionChk.Checked; }
    }

    internal string SolutionName
    {
      get { return solutionNameTextBox.Text; }
    }

    internal bool CreateNewSolution
    {
      get {         
         return solutionOptions.Text.Equals("Create new solution");
      }
    }

    internal string Language
    {
      get {
        if (languageLbl.Text.IndexOf("Visual C#") >= 0)
          return "CSharp";
        else
          return "VisualBasic";
      }          
    }

#endregion

    private DTE2 _dte;

    public WizardNewProjectDialog(string projectType)
    {
      InitializeComponent();
      _dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;      
      CreateProjectTypesList();
      SetDefaults(projectType);
      txtProjectName.TextChanged += txtProjectName_TextChanged;      
      txtProjectPath.Validating += txtProjectPath_Validating;
      solutionNameTextBox.Validating += solutionNameTextBox_Validating;
      solutionNameTextBox.TextChanged += solutionNameTextBox_TextChaned;
      createDirectoryForSolutionChk.CheckedChanged += createDirectoryForSolutionChk_CheckedChanged;

      if (_dte.Solution != null && _dte.Solution.Count >= 1)
      {
        solutionOptions.SelectedIndexChanged += solutionOptions_SelectedIndexChanged;
      }
      else
      {
        solutionOptions.Enabled = false;
      }
    }

    void solutionOptions_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (solutionOptions.Text.Equals("Add to solution"))
      {        
        txtProjectPath.Text = Path.GetDirectoryName(Environment.CurrentDirectory);
        solutionNameTextBox.Enabled = false;
      }
    }

    void createDirectoryForSolutionChk_CheckedChanged(object sender, EventArgs e)
    {      
      var control = (CheckBox)sender;
      
      if (!control.Checked)
      {
        btnOK.Enabled = true;
        errorProvider1.SetError(solutionNameTextBox, "");
      }

      if (string.IsNullOrEmpty(solutionNameTextBox.Text.Trim()))
        solutionNameTextBox.Text = GetUniqueName(txtProjectPath.Text, txtProjectName.Text);        
      
      solutionNameTextBox.Enabled = control.Checked;
             
    }

    void txtProjectPath_Validating(object sender, CancelEventArgs e)
    {
      string s = string.Empty;
      
      if (string.IsNullOrEmpty(solutionNameTextBox.Text.Trim()))
        return;

      if (createDirectoryForSolutionChk.Checked)
        s = Path.Combine(s, solutionNameTextBox.Text);      

      if (!Directory.Exists(s))
      {
        errorProvider1.SetError(txtProjectPath, "");
      }
      else 
      {        
        errorProvider1.SetError(txtProjectPath, string.Format("The path '{0}' already exists.", s));
      }
    }

    void txtProjectName_TextChanged(object sender, EventArgs e)
    {
      solutionNameTextBox.Text = txtProjectName.Text;
      if (!string.IsNullOrEmpty(txtProjectName.Text.Trim()))      
        errorProvider1.SetError(txtProjectName, "");      
    }


    void solutionNameTextBox_TextChaned(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(solutionNameTextBox.Text.Trim()))
      {
        errorProvider1.SetError(solutionNameTextBox, "");
        btnOK.Enabled = true;
      }
      else
      {
        btnOK.Enabled = false;
      }
    }

    private void SetDefaults(string projectType)
    {

      var projectName = projectType.Equals("Windows Forms Project") ? "MySqlWindowsFormsApplication" : "MySqlMvcApplication";
      txtProjectPath.Text = GetDefaultProjectPath();     

      solutionNameTextBox.Text  = txtProjectName.Text = GetUniqueName(txtProjectPath.Text, projectName);      
      
      createDirectoryForSolutionChk.Checked = Settings.Default.CreateDirectoryForSolution;
      
      if (solutionOptions.Enabled)
         solutionOptions.Text = Settings.Default.CreateNewSolution;

      // select project type
      var language = String.IsNullOrEmpty(Settings.Default.NewProjectLanguageSelected) ? "Visual C#" : Settings.Default.NewProjectLanguageSelected;

      ListViewItem item = projectTypesList.Items.OfType<ListViewItem>()
                                     .FirstOrDefault(x => x.Text.Equals(projectType, StringComparison.CurrentCultureIgnoreCase) && x.SubItems[1].Text.Equals(language, StringComparison.CurrentCultureIgnoreCase));
      if (item != null)
      {
        projectTypesList.SelectedItems.Clear();
        item.Selected = item.Focused = true;
        projectTypesList.Focus();
      }

    }

    private void CreateProjectTypesList()
    {
      
      projectTypesList.View = View.Details;
      projectTypesList.SmallImageList = imageList1;

      // Initialize the tile size.
      projectTypesList.TileSize = new Size(400, 39);

      projectTypesList.LargeImageList = imageList1;
      projectTypesList.FullRowSelect = true;      

      projectTypesList.Columns.Add("Project Type", 240, HorizontalAlignment.Left);
      projectTypesList.Columns.Add("Language", 160, HorizontalAlignment.Left);

      // Create items and add them to myListView.
      ListViewItem item0 = new ListViewItem(new string[] { "ASP.NET MVC 3 Project", "Visual C#" }, 0);
      ListViewItem item1 = new ListViewItem(new string[] { "Windows Forms Project","Visual C#" }, 0);
      ListViewItem item2 = new ListViewItem(new string[] { "ASP.NET MVC 3 Project", "Visual Basic" }, 0);
      ListViewItem item3 = new ListViewItem(new string[] { "Windows Forms Project", "Visual Basic" }, 0);

      projectTypesList.Items.AddRange( new ListViewItem[] { item0, item1, item2, item3 });
      projectTypesList.SelectedIndexChanged += projectTypesList_SelectedIndexChanged;
      
    }

    private void projectTypesList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (projectTypesList.SelectedItems.Count > 0)
      {
        var selectedItem = projectTypesList.SelectedItems[0];
        languageLbl.Text = selectedItem.SubItems[1].Text;

        switch (selectedItem.Text)
        {          
          case "ASP.NET MVC 3 Project":
            lblProjectDescription.Text = Resources.MvcProjectTemplateDescription;
            solutionNameTextBox.Text = txtProjectName.Text = GetUniqueName(txtProjectPath.Text, "MySqlMvcApplication");            
            break;
           case "Windows Forms Project":
            lblProjectDescription.Text = Resources.WinFormsProjectDescription;
            solutionNameTextBox.Text = txtProjectName.Text = GetUniqueName(txtProjectPath.Text,"MySqlWindowsFormsApplication");
            break;                    
        }        
      }
    }

    private void txtProjectName_Leave(object sender, EventArgs e)
    {
      string dir = txtProjectPath.Text;
      if (string.IsNullOrEmpty(dir.Trim()))
      {
        dir = GetDefaultProjectPath();
        txtProjectPath.Text = dir;
      }
    }

    private string GetDefaultProjectPath()
    {
      if (!String.IsNullOrEmpty(Settings.Default.NewProjectSavedPath))
      {
        if (!String.IsNullOrEmpty(Path.GetDirectoryName(Settings.Default.NewProjectSavedPath)))
        {
          return Settings.Default.NewProjectSavedPath;
        }
      }

      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "");
      string vsProjPath = "";
      double version = double.Parse(_dte.Version);

      if (version == 9.0)
        vsProjPath = @"Visual Studio 2008\Projects";
      else if (version == 10.0)
        vsProjPath = @"Visual Studio 2010\Projects";
      else if (version == 11.0)
        vsProjPath = @"Visual Studio 2012\Projects";
      else if (version == 12.0)
        vsProjPath = @"Visual Studio 2013\Projects";

      return Path.Combine(path, vsProjPath);
      
    }

    private void solutionNameTextBox_Validating(object sender,  CancelEventArgs e)
    {
      e.Cancel = false;

      string s = solutionNameTextBox.Text;
      if (string.IsNullOrEmpty(s.Trim()) && createDirectoryForSolutionChk.Checked)
      {
        btnOK.Enabled = false;
        return;
      }

      s = Path.Combine(txtProjectPath.Text, solutionNameTextBox.Text);

      if (Directory.Exists(s) && createDirectoryForSolutionChk.Checked)
      {
        errorProvider1.SetError(solutionNameTextBox, "The Solution path already exists. Please choice a different name.");
        e.Cancel = true;
        return;
      }
        errorProvider1.SetError(solutionNameTextBox, "");      
    }


    private void btnBrowse_Click(object sender, EventArgs e)
    {
      DialogResult result = folderBrowserDialog1.ShowDialog();
      if (result == DialogResult.OK)
        txtProjectPath.Text = folderBrowserDialog1.SelectedPath;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      if (Validate()) Close();
    }


    private string GetUniqueName(string path, string projectName)
    {
      string dir = path;
      for (int i = 1; ; ++i)
      {
        path = Path.Combine(dir, projectName + i.ToString());
        if (!Directory.Exists(path))
          return path.Substring(Path.GetDirectoryName(path).Length + 1);        
      }
    }
  }
}
