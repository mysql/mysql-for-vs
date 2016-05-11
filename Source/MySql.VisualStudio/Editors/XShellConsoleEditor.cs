// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// XShellConsoleEditor class, used to mimic a console editor for executing xShell commands, using a textbox and a richtTextBox to accomplish this task.
  /// </summary>
  /// <seealso cref="System.Windows.Forms.UserControl" />
  public partial class XShellConsoleEditor : UserControl
  {
    #region Fields & Constants
    /// <summary>
    /// The prompt string used by the console editor.
    /// </summary>
    private string _promptString = ">";

    /// <summary>
    /// The border style used in the text input.
    /// </summary>
    private BorderStyle _borderInput = BorderStyle.None;

    /// <summary>
    /// Variable to store the current line. Used while navigating the previous commands list.
    /// </summary>
    private int _currentLine;

    /// <summary>
    /// Variable used to store the previous messages.
    /// </summary>
    private readonly ArrayList _prevMessages = new ArrayList();

    /// <summary>
    /// The preferred height for the textbox inputs.
    /// </summary>
    private const int PreferredHeight = 14;
    #endregion

    #region Properties
    public bool IsDirty { get; set; }

    /// <summary>
    /// Gets or sets the prompt string.
    /// </summary>
    /// <value>
    /// The prompt string.
    /// </value>
    [DefaultValue(">")]
    public string PromptString
    {
      get { return _promptString; }
      set
      {
        _promptString = value;
        lblPrompt.Text = _promptString;
      }
    }

    /// <summary>
    /// Gets or sets the color of the prompt.
    /// </summary>
    /// <value>
    /// The color of the prompt.
    /// </value>
    public Color PromptColor
    {
      get { return lblPrompt.ForeColor; }
      set { lblPrompt.ForeColor = value; }
    }

    /// <summary>
    /// Gets or sets the border around the  input box.
    /// </summary>
    /// <value>
    /// The border of the input box.
    /// </value>
    [DefaultValue(BorderStyle.None)]
    public BorderStyle BorderInput
    {
      get { return _borderInput; }
      set
      {
        _borderInput = value;
        panelBottom.BorderStyle = _borderInput;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="XShellConsoleEditor"/> class.
    /// </summary>
    public XShellConsoleEditor()
    {
      InitializeComponent();
      rtbMessages.Height = 0;
    }

    /// <summary>
    /// Adds a message to the message RichTextBox.
    /// </summary>
    /// <param name="msg">The message.</param>
    [Description("Adds a message to the message RichTextBox")]
    public void AddMessage(string msg)
    {
      int prevLength = rtbMessages.Text.Length;
      if (rtbMessages.Lines.Length > 0)
      {
        rtbMessages.AppendText(string.Format("\n{0}", msg));
      }
      else
      {
        rtbMessages.AppendText(msg);
      }

      rtbMessages.SelectionStart = prevLength;
      rtbMessages.SelectionLength = rtbMessages.Text.Length - prevLength;
      rtbMessages.SelectionStart = rtbMessages.Text.Length;
      rtbMessages.SelectionLength = 0;
      rtbMessages.SelectionColor = rtbMessages.ForeColor;
      rtbMessages.ScrollToCaret();
      txtInput.Focus();
    }

    /// <summary>
    /// Adds a command to the messages list
    /// </summary>
    /// <param name="prompt">The prompt string</param>
    /// <param name="color">The color of the text selection</param>
    /// <param name="command">The command entered</param>
    private void AddCommand(string prompt, Color color, string command)
    {
      int prevLength = rtbMessages.Text.Length;
      rtbMessages.AppendText(string.Format("{0} {1}", prompt, command));
      rtbMessages.SelectionStart = prevLength;
      rtbMessages.SelectionLength = prompt.Length;
      rtbMessages.SelectionColor = color;
      rtbMessages.SelectionLength = 0;
      rtbMessages.SelectionStart = rtbMessages.Text.Length;
      rtbMessages.ScrollToCaret();
      txtInput.Focus();
    }

    /// <summary>
    /// Clear all messages from RichTextBox.
    /// </summary>
    public void ClearMessages()
    {
      rtbMessages.Clear();
      rtbMessages.Height = 0;
      rtbMessages.ScrollBars = RichTextBoxScrollBars.None;
    }

    /// <summary>
    /// Handles the KeyDown event of the txtInput control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
    private void txtInput_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Return)
      {
        if (txtInput.Text != "")
        {
          SuspendLayout();
          string prevPrompt = lblPrompt.Text;
          Color prevPromptColor = PromptColor;
          // Raise the command event
          XShellConsoleCommandEventArgs args = new XShellConsoleCommandEventArgs(txtInput.Text);
          OnCommand(args);
          if (args.Cancel == false)
          {
            if (rtbMessages.Lines.Length > 0)
            {
              rtbMessages.AppendText("\r\n");
            }

            AddCommand(prevPrompt, prevPromptColor, txtInput.Text);
            if (args.Message != "")
            {
              AddMessage(args.Message);
            }

            rtbMessages.ScrollToCaret();
            _prevMessages.Add(txtInput.Text);
            _currentLine = _prevMessages.Count - 1;
          }

          txtInput.Text = "";
          ResumeLayout();
        }

        e.Handled = true;
        return;
      }

      if (e.KeyCode == Keys.Up)
      {
        if (_currentLine >= 0 && _prevMessages.Count > 0)
        {
          txtInput.Text = _prevMessages[_currentLine].ToString();
          txtInput.SelectionLength = 0;
          txtInput.SelectionStart = txtInput.Text.Length;
          _currentLine--;
        }

        e.Handled = true;
        return;
      }

      if (e.KeyCode == Keys.Down)
      {
        if (_currentLine < _prevMessages.Count - 2)
        {
          _currentLine++;
          txtInput.Text = _prevMessages[_currentLine + 1].ToString();
          txtInput.SelectionLength = 0;
          txtInput.SelectionStart = txtInput.Text.Length;
        }

        e.Handled = true;
        return;
      }

      if (e.KeyCode == Keys.Escape)
      {
        if (txtInput.SelectionLength > 0 && txtInput.AutoCompleteMode != AutoCompleteMode.None)
        {
          txtInput.Text = txtInput.Text.Substring(0, txtInput.SelectionStart);
          txtInput.SelectionStart = txtInput.Text.Length;
        }
        else
        {
          txtInput.Text = "";
        }

        e.Handled = true;
      }
    }

    /// <summary>
    /// Handles the KeyPress event of the txtInput control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyPressEventArgs"/> instance containing the event data.</param>
    private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)Keys.Return)
      {
        e.Handled = true;
      }
    }

    /// <summary>
    /// Handles the TextChanged event of the txtInput control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void txtInput_TextChanged(object sender, EventArgs e)
    {
      // Fire the CommandEntering event first
      XShellConsoleCommandEnteringEventArgs commandEnteringArgs = new XShellConsoleCommandEnteringEventArgs(txtInput.Text);
      OnCommandEntering(commandEnteringArgs);
    }

    /// <summary>
    /// Raises the <see cref="E:Command" /> event.
    /// </summary>
    /// <param name="e">The <see cref="XShellConsoleCommandEventArgs"/> instance containing the event data.</param>
    protected virtual void OnCommand(XShellConsoleCommandEventArgs e)
    {
      if (Command != null)
      {
        Command(this, e);
      }
    }

    /// <summary>
    /// Raises the <see cref="E:CommandEntering" /> event.
    /// </summary>
    /// <param name="e">The <see cref="XShellConsoleCommandEnteringEventArgs"/> instance containing the event data.</param>
    protected virtual void OnCommandEntering(XShellConsoleCommandEnteringEventArgs e)
    {
      if (CommandEntering != null)
      {
        CommandEntering(this, e);
      }
    }

    /// <summary>
    /// Sets input focus to the control.
    /// </summary>
    /// <returns>
    /// true if the input focus request was successful; otherwise, false.
    /// </returns>
    public new bool Focus()
    {
      Select();
      return txtInput.Focus();
    }

    /// <summary>
    /// Handles the Load event of the Prompt control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void Prompt_Load(object sender, EventArgs e)
    {
      txtInput.Height = PreferredHeight;
    }

    /// <summary>
    /// Handles the FontChanged event of the Prompt control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void Prompt_FontChanged(object sender, EventArgs e)
    {
      txtInput.Font = Font;
    }

    /// <summary>
    /// Handles the ForeColorChanged event of the Prompt control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void Prompt_ForeColorChanged(object sender, EventArgs e)
    {
      txtInput.ForeColor = ForeColor;
      rtbMessages.ForeColor = ForeColor;
    }

    /// <summary>
    /// Handles the BackColorChanged event of the Prompt control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void Prompt_BackColorChanged(object sender, EventArgs e)
    {
      rtbMessages.BackColor = BackColor;
      txtInput.BackColor = BackColor;
      panelBottom.BackColor = BackColor;
      lblPrompt.BackColor = BackColor;
    }

    /// <summary>
    /// Handles the TextChanged event of the rtbMessages control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void rtbMessages_TextChanged(object sender, EventArgs e)
    {
      if (rtbMessages.Height < Height - panelBottom.Height)
      {
        rtbMessages.Height = rtbMessages.Lines.Length * PreferredHeight;
      }
      else
      {
        rtbMessages.ScrollBars = RichTextBoxScrollBars.Both;
      }

      if (rtbMessages.Height > Height - panelBottom.Height)
      {
        rtbMessages.Height = Height - panelBottom.Height;
        rtbMessages.ScrollBars = RichTextBoxScrollBars.Both;
      }

      rtbMessages.ScrollToCaret();
    }

    /// <summary>
    /// Handles the Click event of the rtbMessages control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void rtbMessages_Click(object sender, EventArgs e)
    {
      if (rtbMessages.SelectionLength == 0)
      {
        txtInput.Focus();
      }
    }

    /// <summary>
    /// Handles the Resize event of the CommandPrompt control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CommandPrompt_Resize(object sender, EventArgs e)
    {
      if ((Height - panelBottom.Height) % PreferredHeight != 0)
      {
        Height = ((Height - panelBottom.Height) / PreferredHeight + 1) * PreferredHeight + panelBottom.Height;
      }
    }
    #endregion

    #region "Delegates & Events"
    /// <summary>
    /// Event raised when the user enters a command and presses the Enter key.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="XShellConsoleCommandEventArgs"/> instance containing the event data.</param>
    public delegate void CommandEventHandler(object sender, XShellConsoleCommandEventArgs e);
    public event CommandEventHandler Command;

    /// <summary>
    /// Event raised on KeyPress in input area.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="XShellConsoleCommandEnteringEventArgs"/> instance containing the event data.</param>
    public delegate void CommandEnteringEventHandler(object sender, XShellConsoleCommandEnteringEventArgs e);
    public event CommandEnteringEventHandler CommandEntering;
    #endregion
  }
}