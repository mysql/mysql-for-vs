// Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using InfoDialogTest.Properties;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySqlWorkbench;
using MySql.Utility.Forms;

namespace InfoDialogTest
{
  public partial class InfoDialogTestForm : Form
  {
    private const int DATA_GRID_VIEW_DELTA_HEIGHT = 100;

    private BindingList<Pair> _pairs;

    public InfoDialogTestForm()
    {
      _pairs = null;
      InitializeComponent();
      Button1DialogResultComboBox.DataSource = Enum.GetValues(typeof(DialogResult));
      Button2DialogResultComboBox.DataSource = Enum.GetValues(typeof(DialogResult));
      Button3DialogResultComboBox.DataSource = Enum.GetValues(typeof(DialogResult));
      LayoutTypeComboBox.DataSource = Enum.GetValues(typeof(CommandAreaProperties.ButtonsLayoutType));
      InfoTypeComboBox.DataSource = Enum.GetValues(typeof(InfoDialog.InfoType));
      FitTextComboBox.DataSource = Enum.GetValues(typeof(InfoDialog.FitTextsAction));
      DefaultButtonComboBox.DataSource = Enum.GetValues(typeof(InfoDialog.DefaultButtonType));
      LeftControlComboBox.DataSource = Enum.GetValues(typeof(CommandAreaProperties.LeftAreaControlType));
      CommandAreaPropertiesBindingSource.Add(new CommandAreaProperties(CommandAreaProperties.ButtonsLayoutType.BackOnly));
      InfoDialogPropertiesBindingSource.Add(new InfoDialogProperties());
      InitializeValues();
      DefaultButtonComboBox.SelectedIndex = 3;
      FitTextComboBox.SelectedIndex = 2;
      InfoTypeComboBox.SelectedIndex = 3;
      LeftControlComboBox.SelectedIndex = 2;
    }

    private Dictionary<string, string> ComboBoxKeyValuePairs
    {
      get
      {
        if (_pairs == null)
        {
          return null;
        }

        var dict = new Dictionary<string, string>(_pairs.Count);
        foreach (var pair in _pairs)
        {
          dict.Add(pair.Key, pair.Value);
        }

        return dict;
      }
    }

    private void ClearComboBoxDataButton_Click(object sender, EventArgs e)
    {
      _pairs.Clear();
    }

    private void ComboBoxItemsDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      if (LeftControlComboBox.SelectedIndex != 1)
      {
        return;
      }

      ClearComboBoxDataButton.Visible = _pairs.Count > 0;
      GenerateComboBoxDataButton.Visible = _pairs.Count == 0;
    }

    private void ComboBoxItemsDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      if (LeftControlComboBox.SelectedIndex != 1)
      {
        return;
      }

      ClearComboBoxDataButton.Visible = _pairs.Count > 0;
      GenerateComboBoxDataButton.Visible = _pairs.Count == 0;
    }

    private void GenerateComboBoxDataButton_Click(object sender, EventArgs e)
    {
      foreach (var kvp in MySqlWorkbench.ConnectionsMigrationDelayType.None.GetDescriptionsDictionary(true, true, true))
      {
        var pair = _pairs.AddNew();
        if (pair == null)
        {
          continue;
        }

        pair.Key = kvp.Key;
        pair.Value = kvp.Value;
      }
    }

    private void InfoPicButton_Click(object sender, EventArgs e)
    {
      try
      {
        if (InfoPictureOpenFileDialog.ShowDialog() == DialogResult.OK)
        {
          int selectedIndex = InfoTypeComboBox.SelectedIndex;
          string selectedPictureFile = InfoPictureOpenFileDialog.FileName;
          InfoTypeImageList.Images.Keys[selectedIndex] = selectedPictureFile;
          InfoTypeImageList.Images[selectedIndex] = Image.FromFile(selectedPictureFile);
          RefreshInfoTypeImage();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, Resources.InfoTypeImageLoadErrorText, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void InfoTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      RefreshInfoTypeImage();
    }

    private void InitializeValues()
    {
      var infoProperties = InfoDialogPropertiesBindingSource.Current as InfoDialogProperties;
      if (infoProperties == null)
      {
        return;
      }

      infoProperties.TitleText = "Sample Title";
      infoProperties.DetailText = "Sample detail text.";
      infoProperties.DetailSubText = "Sample detail sub-text.";
      InfoDialogPropertiesBindingSource.ResetCurrentItem();

      _pairs = new BindingList<Pair>();
      _pairs.AddingNew += (s, a) =>
      {
        a.NewObject = new Pair { Parent = _pairs };
      };
      ComboBoxItemsDataGridView.DataSource = _pairs;
    }

    private void LayoutTypeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      ResetGenericPropertiesAvailability();
    }

    private void LayoutTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      ResetGenericPropertiesAvailability();
    }

    private void LeftControlComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      bool comboBoxControlsWereVisible = ComboBoxItemsDataGridView.Visible;

      bool checkboxControlsVisible = LeftControlComboBox.SelectedIndex == 0;
      CheckBoxTextLabel.Visible = checkboxControlsVisible;
      CheckboxTextTextBox.Visible = checkboxControlsVisible;
      CheckedCheckBox.Visible = checkboxControlsVisible;

      bool comboBoxControlsVisible = LeftControlComboBox.SelectedIndex == 1;
      ComboBoxWidthLabel.Visible = comboBoxControlsVisible;
      ComboBoxWidthNumericUpDown.Visible = comboBoxControlsVisible;
      ComboBoxItemsLabel.Visible = comboBoxControlsVisible;
      ComboBoxItemsDataGridView.Visible = comboBoxControlsVisible;
      ClearComboBoxDataButton.Visible = comboBoxControlsVisible;
      GenerateComboBoxDataButton.Visible = comboBoxControlsVisible;

      bool moreInfoButtonControlsVisible = LeftControlComboBox.SelectedIndex == 2;
      CollapsedTextLabel.Visible = moreInfoButtonControlsVisible;
      CollapsedTextTextBox.Visible = moreInfoButtonControlsVisible;
      ExpandedTextLabel.Visible = moreInfoButtonControlsVisible;
      ExpandedTextTextBox.Visible = moreInfoButtonControlsVisible;

      if (comboBoxControlsWereVisible && !comboBoxControlsVisible)
      {
        CommandAreaGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        CommandAreaGroupBox.Height -= DATA_GRID_VIEW_DELTA_HEIGHT;
        Height -= DATA_GRID_VIEW_DELTA_HEIGHT;
        CommandAreaGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
      }
      else if (!comboBoxControlsWereVisible && comboBoxControlsVisible)
      {
        CommandAreaGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        CommandAreaGroupBox.Height += DATA_GRID_VIEW_DELTA_HEIGHT;
        Height += DATA_GRID_VIEW_DELTA_HEIGHT;
        CommandAreaGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
      }

      ResetButton3AsMoreInfoCheckBox();
    }

    private void RefreshInfoTypeImage()
    {
      int selectedIndex = InfoTypeComboBox.SelectedIndex;
      InfoTypePictureBox.Image = InfoTypeImageList.Images[selectedIndex];
      InfoPicFileTextBox.Text = InfoTypeImageList.Images.Keys[selectedIndex];
    }

    private void ResetButton3AsMoreInfoCheckBox()
    {
      if (LayoutTypeComboBox.DataSource == null || LeftControlComboBox.DataSource == null)
      {
        return;
      }

      CommandAreaProperties.ButtonsLayoutType layoutType;
      CommandAreaProperties.LeftAreaControlType leftControlType;
      Enum.TryParse(LayoutTypeComboBox.SelectedValue.ToString(), out layoutType);
      Enum.TryParse(LeftControlComboBox.SelectedValue.ToString(), out leftControlType);
      var button3AsMoreInfoWasEnabled = Button3AsMoreInfoCheckBox.Enabled;
      Button3AsMoreInfoCheckBox.Enabled = layoutType.Is2Button() && leftControlType != CommandAreaProperties.LeftAreaControlType.MoreInfoButton;
      if (button3AsMoreInfoWasEnabled && !Button3AsMoreInfoCheckBox.Enabled)
      {
        Button3AsMoreInfoCheckBox.Checked = false;
      }
    }

    private void ResetGenericPropertiesAvailability()
    {
      CommandAreaProperties.ButtonsLayoutType layoutType;
      Enum.TryParse(LayoutTypeComboBox.SelectedValue.ToString(), out layoutType);
      bool enableGenericPropertyControls = layoutType.IsGeneric() || LayoutTypeCheckBox.Checked;
      Button1NameTextBox.Enabled = enableGenericPropertyControls;
      Button1DialogResultComboBox.Enabled = enableGenericPropertyControls;
      Button2NameTextBox.Enabled = enableGenericPropertyControls;
      Button2DialogResultComboBox.Enabled = enableGenericPropertyControls;
      Button3NameTextBox.Enabled = enableGenericPropertyControls;
      Button3DialogResultComboBox.Enabled = enableGenericPropertyControls;
      ResetButton3AsMoreInfoCheckBox();
    }

    private void ShowDialogButton_Click(object sender, EventArgs e)
    {
      var infoProperties = InfoDialogPropertiesBindingSource.Current as InfoDialogProperties;
      if (infoProperties == null)
      {
        return;
      }

      infoProperties.CommandAreaProperties = CommandAreaPropertiesBindingSource.Current as CommandAreaProperties;
      if (infoProperties.CommandAreaProperties != null)
      {
        infoProperties.CommandAreaProperties.LeftAreaComboBoxDataSource = infoProperties.CommandAreaProperties.LeftAreaControl == CommandAreaProperties.LeftAreaControlType.InfoComboBox ? ComboBoxKeyValuePairs : null;
      }

      int selectedIndex = InfoTypeComboBox.SelectedIndex;
      if (selectedIndex >= 0)
      {
        infoProperties.LogoImage = InfoTypeImageList.Images[selectedIndex];
      }

      var infoResult = InfoDialog.ShowDialog(infoProperties);
      var resultsBuilder = new StringBuilder();
      resultsBuilder.AppendFormat("Dialog Result = {0}", infoResult.DialogResult);
      if (infoProperties.CommandAreaProperties != null)
      {
        switch (infoProperties.CommandAreaProperties.LeftAreaControl)
        {
          case CommandAreaProperties.LeftAreaControlType.InfoCheckBox:
            resultsBuilder.Append(Environment.NewLine);
            resultsBuilder.AppendFormat("CheckBox Value = {0}", infoResult.InfoCheckboxValue);
            break;

          case CommandAreaProperties.LeftAreaControlType.InfoComboBox:
            resultsBuilder.Append(Environment.NewLine);
            resultsBuilder.AppendFormat("ComboBox Index = {0}", infoResult.InfoComboBoxSelectedIndex);
            resultsBuilder.Append(Environment.NewLine);
            resultsBuilder.AppendFormat("ComboBox Value = {0}", infoResult.InfoComboBoxSelectedValue);
            break;
        }
      }

      MessageBox.Show(resultsBuilder.ToString());
    }
  }
}
