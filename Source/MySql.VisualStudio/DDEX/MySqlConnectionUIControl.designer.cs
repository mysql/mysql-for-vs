// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Data.VisualStudio.DDEX
{
  partial class MySqlDataConnectionUIControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySqlDataConnectionUIControl));
      this.loginTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.passwordLabel = new System.Windows.Forms.Label();
      this.serverNameTextBox = new System.Windows.Forms.TextBox();
      this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
      this.databaseNameLabel = new System.Windows.Forms.Label();
      this.userNameLabel = new System.Windows.Forms.Label();
      this.serverNameLabel = new System.Windows.Forms.Label();
      this.userNameTextBox = new System.Windows.Forms.TextBox();
      this.passwordTextBox = new System.Windows.Forms.TextBox();
      this.dbList = new System.Windows.Forms.ComboBox();
      this.loginTableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // loginTableLayoutPanel
      // 
      resources.ApplyResources(this.loginTableLayoutPanel, "loginTableLayoutPanel");
      this.loginTableLayoutPanel.Controls.Add(this.passwordLabel, 0, 2);
      this.loginTableLayoutPanel.Controls.Add(this.serverNameTextBox, 1, 0);
      this.loginTableLayoutPanel.Controls.Add(this.savePasswordCheckBox, 1, 3);
      this.loginTableLayoutPanel.Controls.Add(this.databaseNameLabel, 0, 4);
      this.loginTableLayoutPanel.Controls.Add(this.userNameLabel, 0, 1);
      this.loginTableLayoutPanel.Controls.Add(this.serverNameLabel, 0, 0);
      this.loginTableLayoutPanel.Controls.Add(this.userNameTextBox, 1, 1);
      this.loginTableLayoutPanel.Controls.Add(this.passwordTextBox, 1, 2);
      this.loginTableLayoutPanel.Controls.Add(this.dbList, 1, 4);
      this.loginTableLayoutPanel.Name = "loginTableLayoutPanel";
      // 
      // passwordLabel
      // 
      resources.ApplyResources(this.passwordLabel, "passwordLabel");
      this.passwordLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.passwordLabel.Name = "passwordLabel";
      // 
      // serverNameTextBox
      // 
      resources.ApplyResources(this.serverNameTextBox, "serverNameTextBox");
      this.serverNameTextBox.Name = "serverNameTextBox";
      this.serverNameTextBox.Tag = "Server";
      this.serverNameTextBox.TextChanged += new System.EventHandler(this.SetProperty);
      this.serverNameTextBox.Leave += new System.EventHandler(this.TrimControlText);
      // 
      // savePasswordCheckBox
      // 
      resources.ApplyResources(this.savePasswordCheckBox, "savePasswordCheckBox");
      this.savePasswordCheckBox.Name = "savePasswordCheckBox";
      this.savePasswordCheckBox.Tag = "Persist Security Info";
      this.savePasswordCheckBox.CheckedChanged += new System.EventHandler(this.SavePasswordChanged);
      // 
      // databaseNameLabel
      // 
      resources.ApplyResources(this.databaseNameLabel, "databaseNameLabel");
      this.databaseNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.databaseNameLabel.Name = "databaseNameLabel";
      // 
      // userNameLabel
      // 
      resources.ApplyResources(this.userNameLabel, "userNameLabel");
      this.userNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.userNameLabel.Name = "userNameLabel";
      // 
      // serverNameLabel
      // 
      resources.ApplyResources(this.serverNameLabel, "serverNameLabel");
      this.serverNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.serverNameLabel.Name = "serverNameLabel";
      // 
      // userNameTextBox
      // 
      resources.ApplyResources(this.userNameTextBox, "userNameTextBox");
      this.userNameTextBox.Name = "userNameTextBox";
      this.userNameTextBox.Tag = "User id";
      this.userNameTextBox.TextChanged += new System.EventHandler(this.SetProperty);
      this.userNameTextBox.Leave += new System.EventHandler(this.TrimControlText);
      // 
      // passwordTextBox
      // 
      resources.ApplyResources(this.passwordTextBox, "passwordTextBox");
      this.passwordTextBox.Name = "passwordTextBox";
      this.passwordTextBox.Tag = "Password";
      this.passwordTextBox.UseSystemPasswordChar = true;
      this.passwordTextBox.TextChanged += new System.EventHandler(this.SetProperty);
      // 
      // dbList
      // 
      resources.ApplyResources(this.dbList, "dbList");
      this.dbList.FormattingEnabled = true;
      this.dbList.Name = "dbList";
      this.dbList.Tag = "Database";
      this.dbList.DropDown += new System.EventHandler(this.dbList_DropDown);
      this.dbList.TextChanged += new System.EventHandler(this.SetProperty);
      // 
      // MySqlDataConnectionUIControl
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.loginTableLayoutPanel);
      this.Name = "MySqlDataConnectionUIControl";
      this.loginTableLayoutPanel.ResumeLayout(false);
      this.loginTableLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel loginTableLayoutPanel;
    private System.Windows.Forms.Label userNameLabel;
    private System.Windows.Forms.TextBox userNameTextBox;
    private System.Windows.Forms.Label passwordLabel;
    private System.Windows.Forms.TextBox passwordTextBox;
    private System.Windows.Forms.CheckBox savePasswordCheckBox;
    private System.Windows.Forms.TextBox serverNameTextBox;
    private System.Windows.Forms.Label databaseNameLabel;
    private System.Windows.Forms.Label serverNameLabel;
    private System.Windows.Forms.ComboBox dbList;
  }
}
