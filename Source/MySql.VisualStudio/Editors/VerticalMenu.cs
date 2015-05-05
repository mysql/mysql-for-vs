// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This controls load a vertical menu basis on the configuration given.
  /// </summary>
  public partial class VerticalMenu : UserControl
  {
    /// <summary>
    /// Control Constructor
    /// </summary>
    public VerticalMenu()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Configure the current control to show the options given
    /// </summary>
    /// <param name="items"></param>
    public void ConfigureControl(List<VerticalMenuButton> items)
    {
      items.ForEach(button => GenerateButton(button));
      ApplySelectedStyle(string.Format("tsbtn_{0}", items.First().Name));
    }

    /// <summary>
    /// Create a ToolStripButton and add it to the ToolStripMenu control
    /// </summary>
    /// <param name="button">Configuration for the button that will be created</param>
    private void GenerateButton(VerticalMenuButton button)
    {
      ToolStripButton newBtn = new ToolStripButton() {
                               Name = string.Format("tsbtn_{0}", button.Name),
                               Width = 58,
                               Height = 68,
                               Text = button.ButtonText,
                               ToolTipText = button.ToolTip,
                               Image = ImageButton(button.ImageToLoad),
                               ImageAlign = ContentAlignment.TopCenter,
                               ImageScaling = ToolStripItemImageScaling.None,
                               TextAlign = ContentAlignment.BottomCenter,
                               TextImageRelation = TextImageRelation.ImageAboveText,
                               AutoSize = false
                               };
      newBtn.Click += button.ClickEvent;
      newBtn.Click += (object sender, EventArgs e) => { ApplySelectedStyle(newBtn.Name); };
      tsMenu.Items.Add(newBtn);
    }

    /// <summary>
    /// Get an image from the resources file according to the enum option given
    /// </summary>
    /// <param name="imgType">Image type</param>
    /// <returns>Image type requested</returns>
    private Image ImageButton(ImageType imgType)
    {
      ComponentResourceManager resources = new ComponentResourceManager(typeof(VerticalMenu));
      switch (imgType)
      {
        case ImageType.Resultset:
          return ((System.Drawing.Image)(resources.GetObject("result_set")));
        case ImageType.FieldType:
          return ((System.Drawing.Image)(resources.GetObject("field_types")));
        case ImageType.ExecutionPlan:
          return ((System.Drawing.Image)(resources.GetObject("execution_plan")));
        case ImageType.QueryStats:
          return ((System.Drawing.Image)(resources.GetObject("query_stats")));
        default:
          return null;
      }
    }

    /// <summary>
    /// Change the button style for the current view option selected by the user
    /// </summary>
    /// <param name="buttonName">Name of the button clicked</param>
    private void ApplySelectedStyle(string buttonName)
    {
      ComponentResourceManager resources = new ComponentResourceManager(typeof(VerticalMenu));
      for (int ctr = 0; ctr < tsMenu.Items.Count; ctr++)
      {
        if (tsMenu.Items[ctr].Name == buttonName)
        {
          tsMenu.Items[ctr].ForeColor = Color.White;
          tsMenu.Items[ctr].BackgroundImage = ((System.Drawing.Image)(resources.GetObject("selected_item")));
        }
        else
        {
          tsMenu.Items[ctr].ForeColor = Color.Black;
          tsMenu.Items[ctr].BackgroundImage = null;
        }
      }
    }
  }
}
