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
    /// Variable to store the current line. Used while navigating the previous commands list.
    /// </summary>
    private int _currentLine;

    /// <summary>
    /// Variable used to store the previous messages.
    /// </summary>
    private readonly ArrayList _prevMessages = new ArrayList();
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
    /// </value>
    public bool IsDirty { get; set; }

    /// <summary>
    /// Gets or sets the prompt string.
    /// </summary>
    /// <value>
    /// The prompt string.
    /// </value>
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
    public void AddMessage(string msg)
    {
      int prevLength = rtbMessages.Text.Length;
      if (rtbMessages.Lines.Length > 0)
      {
        rtbMessages.AppendText(string.Format("{0}{1}", Environment.NewLine, msg));
      }
      else
      {
        rtbMessages.AppendText(msg);
      }

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
        if (!string.IsNullOrEmpty(txtInput.Text))
        {
          SuspendLayout();
          string prevPrompt = lblPrompt.Text;
          Color prevPromptColor = PromptColor;

          // Add the command first
          if (rtbMessages.Lines.Length > 0)
          {
            rtbMessages.AppendText(Environment.NewLine);
          }

          AddCommand(prevPrompt, prevPromptColor, txtInput.Text);

          // Raise the command event
          XShellConsoleCommandEventArgs args = new XShellConsoleCommandEventArgs(txtInput.Text);
          OnCommand(args);
          if (args.Cancel == false)
          {
            if (!string.IsNullOrEmpty(args.Message))
            {
              AddMessage(args.Message);
            }

            rtbMessages.ScrollToCaret();
            _prevMessages.Add(txtInput.Text);
            _currentLine = _prevMessages.Count - 1;
          }

          txtInput.Text = string.Empty;
          ResumeLayout();
        }

        e.Handled = true;
        return;
      }

      if (e.KeyCode == Keys.Up)
      {
        // Shows the previous executed command
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
        // Shows the next executed command
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
        // Clear the txtInput.
        txtInput.Text = string.Empty;
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
      lblPrompt.BackColor = BackColor;
    }

    /// <summary>
    /// Handles the TextChanged event of the rtbMessages control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void rtbMessages_TextChanged(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(rtbMessages.Text))
      {
        // Set the height of the richTextBox, measuring the text contained in it
        rtbMessages.Height = TextRenderer.MeasureText(rtbMessages.Text, rtbMessages.Font, new Size(rtbMessages.Width, 0), TextFormatFlags.WordBreak).Height;
        if (rtbMessages.Height > Height - txtInput.Height)
        {
          rtbMessages.Height = Height - txtInput.Height;
          rtbMessages.ScrollBars = RichTextBoxScrollBars.Both;
        }

        // Move the caret (scroll) to the end of the text
        rtbMessages.SelectionStart = rtbMessages.Text.Length;
        rtbMessages.ScrollToCaret();
      }
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
      // If the control resizes, resize the richTextBox as well
      rtbMessages_TextChanged(sender, e);
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